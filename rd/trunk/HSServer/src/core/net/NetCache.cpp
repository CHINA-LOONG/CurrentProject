#define _CRT_SECURE_NO_WARNINGS
#include "NetCache.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "../net/ProtocolHandler.h"
#include "common/counter.h"
//#include "AdminHandler.h"

const size_t NetCache::DEFAULT_READ_SIZE = 4096;
const size_t NetCache::INIT_WRITE_SIZE = 16384;
const size_t NetCache::WEBSERVER_READ_SIZE = 2097152;

NetCache::NetCache(int fd, struct sockaddr_in addr, size_t rsize)
{
	rpos = 0;
	wpos = 0;
	this->fd = fd;
	this->addr = addr;
	uid = -1;
	ph = NULL;
	remove = false;
	aborted = false;
	idle = false;
	pthread_mutex_init(&write_mutex, NULL);
	logger_ = log4cxx::Logger::getLogger("NetCache");
	wsize = INIT_WRITE_SIZE;
	wbuf = new char[INIT_WRITE_SIZE];
	TEST[E_NET_CACHE_W]++;
	if (wbuf==NULL)
	{
		LOG4CXX_FATAL(logger_, "Cannot allocate memory for write buffer!!");
		exit(1);
	}
	this->rsize = rsize;
	rbuf = new char[rsize];
	cmdbuf = new char[rsize+1];
	TEST[E_NET_CACHE_CMD]++;
	TEST[E_NET_CACHE_R]++;
	if (rbuf==NULL || cmdbuf==NULL)
	{
		LOG4CXX_FATAL(logger_, "Cannot allocate memory for read buffer!!");
		exit(1);
	}
}

NetCache::NetCache()
{
	rpos = 0;
	wpos = 0;
	this->fd = -1;
	//this->addr = addr;
	uid = -1;
	ph = NULL;
	remove = false;
	aborted = false;
	idle = false;
	pthread_mutex_init(&write_mutex, NULL);
	logger_ = log4cxx::Logger::getLogger("NetCache");
	wsize = INIT_WRITE_SIZE;
	wbuf = new char[INIT_WRITE_SIZE];
	TEST[E_NET_CACHE_W]++;
	if (wbuf==NULL)
	{
		LOG4CXX_FATAL(logger_, "Cannot allocate memory for write buffer!!");
		exit(1);
	}
	this->rsize = DEFAULT_READ_SIZE;
	rbuf = new char[rsize];
	cmdbuf = new char[rsize+1];
	TEST[E_NET_CACHE_CMD]++;
	TEST[E_NET_CACHE_R]++;
	if (rbuf==NULL || cmdbuf==NULL)
	{
		LOG4CXX_FATAL(logger_, "Cannot allocate memory for read buffer!!");
		exit(1);
	}
}
NetCache::~NetCache(void)
{
	pthread_mutex_destroy(&write_mutex);
	if (ph!=NULL)
	{
		delete ph;
		TEST[E_POLICY_HANDLER]--;
	}
	if (wbuf!=NULL)
	{
		delete []wbuf;
		TEST[E_NET_CACHE_W]--;
	}
	if (rbuf!=NULL)
	{
		delete []rbuf;
		TEST[E_NET_CACHE_R]--;
	}
	if (cmdbuf!=NULL)
	{
		delete []cmdbuf;
		TEST[E_NET_CACHE_CMD]--;
	}
}

char *NetCache::addrstr()
{
	return inet_ntoa(addr.sin_addr);
}

bool NetCache::read()
{
	if ((size_t)rpos+1==rsize)
	{
		LOG4CXX_WARN(logger_, "Read cache overflow for uid=" << uid << " and fd=" << fd);
		return false;
	}
	int size = recv(fd, rbuf+rpos, rsize-rpos-1, 0);
	if (size<=0)
	{
	    LOG4CXX_WARN(logger_, "recv error for uid=" << uid << " and fd=" << fd << " and rbuf+rpos"<< rbuf+rpos <<"and rsize-rpos-1"<<rsize-rpos-1);
        return false;
	}
	rpos += size;
	rbuf[rpos] = '\0';
	idle = false;
	return true;
}

bool NetCache::assemble(string &str)
{
	if (ph!=NULL)
	{
		int type = ph->handlerType();
		if (type==ProtocolHandler::POLICY)
		{
			if (rpos<2)
			{
				return false;
			}
			str = string(rbuf);
			rpos = 0;
			return true;
		}
		else if (type==ProtocolHandler::CLIENT || type==ProtocolHandler::GAMED 
				|| type==ProtocolHandler::WORLD) 
		{
			if (rpos<2)
			{
				return false;
			}
			else if (type==ProtocolHandler::CLIENT && rbuf[0]=='<')
			{
				// should be handled as a policy request
				str = string(rbuf);
				rpos = 0;
				return true;
			}
			else
			{
				int length = 0;
				int lsize = 2;
				if (type==ProtocolHandler::CLIENT)
				{
					length = ntohs(*((unsigned short*)rbuf));
				}
				else
				{
					length = *((unsigned int*)rbuf);
					lsize = sizeof(unsigned int);
				}

				//if (type==ProtocolHandler::CLIENT) {
				//	LOG4CXX_INFO(logger_, "assemble string, rpos = " << rpos << ", length = " << length);
				//}

				if (length<=0 || length>(int)rsize || length>rpos-lsize)
				{
					if( length<=0 || length>(int)rsize )
					{
						LOG4CXX_DEBUG(logger_, "Net assemble error:fd=" << fd << " uid=" << uid << " length=" << length << " rpos-lsize:"<< rpos-lsize<< " content:"<< (int)*(rbuf)<<" " << (int)*(rbuf+1)<<" "<<(int) *(rbuf+2)<<" "<< (int)*(rbuf+3));
						remove = true; 
					}

					return false;
				}
				memcpy(cmdbuf, rbuf+lsize, length);
				cmdbuf[length] = '\0';
				memmove(rbuf, rbuf+length+lsize, rpos-length-lsize);
				rpos -= length+lsize;
				rbuf[rpos] = '\0';
				str = string(cmdbuf, length);
				return true;
			}
		}
		else if (type==ProtocolHandler::ADMIN || type==ProtocolHandler::WEBSERVER)
		{
			// admin client or web server
			int len = 0;
			if (type==ProtocolHandler::ADMIN && rbuf[0]==(char)255)
			{
				// telnet command
				len = 3;
			}
			else
			{
				char *p = strchr(rbuf, '\n'), *q = strchr(rbuf, '\r');
				if (p==NULL&&q==NULL)
				{
					return false;
				}
				len = max(p,q)-rbuf+1;
			}
			strncpy(cmdbuf, rbuf, len);
			cmdbuf[len] = '\0';
			int n = len-1;
			while (n>=0 && (cmdbuf[n]=='\n'||cmdbuf[n]=='\r'))
			{
				cmdbuf[n] = '\0';
				n--;
			}
			memmove(rbuf, rbuf+len, rpos-len);
			rpos -= len;
			rbuf[rpos] = '\0';
			str = string(cmdbuf);
			return true;
		}
	}
	else // ph==NULL
	{
		LOG4CXX_WARN(logger_, "Protocol handler is NULL for fd:"<<fd);
	}
	return false;
}

bool NetCache::waitToWrite()
{
	return wpos>0;
}

bool NetCache::prepareWrite(const char *str, size_t size)
{
	lockWrite();
	if (size>wsize-1-wpos) // buffer overflow, allocate new buffer
	{
		size_t nwsize = max(wsize*2, wpos + size + 1024);
		size_t nwsize_mb = nwsize>>20;
		if (nwsize_mb >= 100) {
			LOG4CXX_INFO(logger_, "Resizing write buffer for fd " << fd << " to " << nwsize_mb << " MB.");
		} else {
			LOG4CXX_DEBUG(logger_, "Resizing write buffer for fd " << fd << " to " << nwsize_mb << " MB.");
		}
		char *nwbuf = new (nothrow) char[nwsize];
		TEST[E_NET_CACHE_NW]++;
		if (nwbuf==NULL)
		{
			LOG4CXX_FATAL(logger_, "Cannot allocate memory for write buffer!!");
			unlockWrite();
			return false;
		}
		else
		{
			if (wpos>0)
			{
				memcpy(nwbuf, wbuf, wpos);
			}
			delete []wbuf;
			TEST[E_NET_CACHE_NW]--;
			wsize = nwsize;
			wbuf = nwbuf;
		}
	}
	memcpy(wbuf+wpos, str, size);
	wpos += size;
	unlockWrite();
	return true;
}


bool NetCache::init( int fd,struct sockaddr_in addr )
{
	rpos = 0;
	wpos = 0;
	this->fd = fd;
	this->addr = addr;
	uid = -1;
	ph = NULL;
	remove = false;
	aborted = false;
	idle = false;
//	pthread_mutex_init(&write_mutex, NULL);
//	wsize = INIT_WRITE_SIZE;
	memset(wbuf,0,sizeof(char)* INIT_WRITE_SIZE);
//	this->rsize = rsize;
	memset(rbuf,0,sizeof(char)* rsize);
	memset(cmdbuf,0,sizeof(char)*(rsize+1));
	return true;
}

#ifdef _WIN32

bool NetCache::write(bool block) {
	return true;
}

#else

bool NetCache::write(bool block)
{
	lockWrite();
	if (wpos==0)
	{
		unlockWrite();
		return true;
	}
	int size = send(fd, wbuf, wpos, block ? 0 : MSG_DONTWAIT);
	if (size<0)
	{
		unlockWrite();
		LOG4CXX_DEBUG(logger_, "net send failed!");
		return false;
	}
	else
	{
		memmove(wbuf, wbuf+size, wpos-size);
		wpos -= size;
		bool finished = (wpos==0);
		unlockWrite();
		if (!finished) {
			LOG4CXX_DEBUG(logger_, "sent, but not finished, wpos=" << wpos);
		}
		return finished;
	}
}

#endif

NetCacheManager* NetCacheManager::m_pInst = NULL;
NetCacheManager::NetCacheManager()
{
// 	for (int i=0;i<NetCacheManager::nNetCacheMax;i++)
// 	{
// 		m_CacheQueue.push_back(new NetCache());
// 		TEST[E_NET_CACHE]++;
// 	}
}

NetCacheManager::~NetCacheManager()
{
	while(!m_CacheQueue.empty())
	{
		NetCache * pCache = m_CacheQueue.front();
		if (pCache)
		{
			delete pCache;
			pCache = NULL;
			TEST[E_NET_CACHE]--;
		}
		m_CacheQueue.pop_front();
	}
}

NetCache* NetCacheManager::getCache()
{
	if(m_CacheQueue.empty())
	{
		TEST[E_NET_CACHE]++;
		return new NetCache();
	}
	else
	{
		NetCache * pCache = m_CacheQueue.front();
		m_CacheQueue.pop_front();
		return pCache;
	}
}

void NetCacheManager::RemoveCache( NetCache* pCache )
{
	if (pCache->ph!=NULL)
	{
		delete pCache->ph;
		pCache->ph = NULL;
		TEST[E_POLICY_HANDLER]--;
	}
	m_CacheQueue.push_back(pCache);
}

NetCacheManager* NetCacheManager::getInst()
{
	if(m_pInst == NULL)
	{
		m_pInst = new NetCacheManager;
	}
	return m_pInst;
}
