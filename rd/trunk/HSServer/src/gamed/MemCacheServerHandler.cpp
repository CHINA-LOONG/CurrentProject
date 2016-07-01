/* 
* File:   MemCacheServerHandler.cpp
* Author: Kidd
* 
* Created on 2011年12月23日, 上午10:32
*/

#include <list>

#include "EventQueue.h"
#include "common/DBC.h"
#include "common/string-util.h"
#include "common/counter.h"
#include "EventDefine.h"
#include "GameEventHandler.h"

#include "MemCacheServerHandler.h"
#include "logic/User.h"
#include "GameDataHandler.h"
#include "common/Clock.h"
#include "logic/GameConfig.h"
#include "common/Ini.h"

MemCacheServerHandler::MSH_T* MemCacheServerHandler::g_pInst = NULL;
log4cxx::LoggerPtr MemCacheServerHandler::logger = log4cxx::Logger::getLogger("MemCacheServer");

MemCacheServerHandler::MemCacheServerHandler()
{
	m_bEnabled			= false;
	m_bInited			= false;
	m_eh				= NULL;
	m_RemoveUpdate		= Clock::getCurrentSystemTime();
	pthread_mutex_init(&mutex_,NULL);
}

MemCacheServerHandler::~MemCacheServerHandler()
{
	pthread_mutex_destroy(&mutex_);
}

MemCacheServerHandler::MSH_T* MemCacheServerHandler::GetInst()
{
	if (g_pInst == NULL)
	{
		g_pInst = new MemCacheServerHandler();
	}
	return g_pInst;
}

int MemCacheServerHandler::GetServerId(const int64 &uid)
{
	int hash = getUidHash(uid);
	return hash % GetServerNumber() + 1;
}

bool MemCacheServerHandler::GetUserInfo(const int64 &uid, std::string& dist)
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}

// 	if (uid != 2928562105553271)
// 	{
// 		return false;
// 	}

	int     dbid    = g_pInst->GetServerId(uid);
	TCRDB*  pConn   = g_pInst->GetDB(dbid);
	if (pConn == NULL)
	{
		g_pInst->SetEnbale(false);
		LOG4CXX_ERROR(logger,"memcache_get_connect_null_error");
		return false;
	}

	char suid[32];
	sprintf(suid, "%016llX", uid);

	int klen = 16;
	int len  = 0;
	//char* buffer    = (char*) tcrdbget2(pConn, key.c_str());
	char* buffer = (char*) tcrdbget(pConn, suid, klen, &len);
	if (buffer == NULL)
	{
		int ecode = tcrdbecode(pConn);
		if (ecode != TTENOREC)
		{
			g_pInst->SetEnbale(false);
			LOG4CXX_ERROR(logger,"memcache_get_ecode_error");
		}
		return false;
	}
	std::string outbuf(buffer, len);
	dist = outbuf;
	free(buffer);
	g_pInst->GetCounter().increase("mem_get_user");
	return true;
}

bool MemCacheServerHandler::UpdateUserInfo(const int64 &uid,std::string& dist)
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}
	time_t time_first = Clock::getUTime();
	int     dbid    = g_pInst->GetServerId(uid);
	TCRDB*  pConn   = g_pInst->GetDB(dbid);
	if (pConn == NULL)
	{
		g_pInst->SetEnbale(false);
		LOG4CXX_ERROR(logger,"memcache_put_connect_null_error");
		return false;
	}

	const char* buf = dist.c_str();
	int         len = dist.length();
	char suid[32];
	sprintf(suid, "%016llX", uid);
	int klen = 16;
	if (!tcrdbput(pConn, suid, klen, buf, len))
	{
		int ecode = tcrdbecode(pConn);
		g_pInst->SetEnbale(false);
		LOG4CXX_ERROR(logger,"memcache_update_put_error"<<tcrdberrmsg(ecode) << uid);
		return false;
	}
	
	time_t time_last = Clock::getUTime();
	g_pInst->GetCounter().increase("mem_put_user");
	g_pInst->GetStatistics().capture("mem_put_user_time",(double)(time_last-time_first));
	g_pInst->GetStatistics().capture("mem_put_user_cnt",g_pInst->GetCounter().count("mem_put_user"));
	if (Clock::GetMinute()%30 == 0)
	{
		g_pInst->GetCounter().clear("mem_put_user");
	}
	
	return true;
}

int MemCacheServerHandler::GetServerNumber()
{
	return m_nServerNum;
}

TCRDB* MemCacheServerHandler::GetDB(int id)
{
	TCRDB* pResult = (TCRDB*) (m_xConnList[id]);
	if (pResult == NULL)
	{
		pResult = tcrdbnew();
		std::string ip = m_xIpList[id];
		if (tcrdbopen(pResult, ip.c_str() , m_xProtList[id]))
		{
			m_xConnList[id] = pResult;
		}

	}
	return pResult;
}

void MemCacheServerHandler::LoadConfig()
{
	const char* filename = "MemCacheServer.dat";
	DBCFile file(0);
	file.OpenFromTXT(filename);

	enum FS
	{
		FS_ID = 0,
		FS_IP = 1,
		FS_PROT = 2,
	} ;
	int cnt = file.GetRecordsNum();
	m_nServerNum = 0;
	for (int line = 0; line < cnt; line++)
	{

		int ind         = file.Search_Posistion(line, FS_ID)->iValue;
		std::string ip  = file.Search_Posistion(line, FS_IP)->pString;
		int prot        = file.Search_Posistion(line, FS_PROT)->iValue;
		m_xIpList[ind]  = ip;
		m_xProtList[ind] = prot;
		if (ind > m_nServerNum)
		{
			m_nServerNum = ind;
		}
	}
	m_bInited = true;
	SetEnbale(true);
}

void MemCacheServerHandler::SetEnbale(bool enable)
{
	m_bEnabled = enable;
	if (!enable)
	{
		Ini myini;
		if (myini.Open("MemErr.ini") == false)
		{
			//return;
		}
		myini.Write("MemCache", "MemCacheError",1);
		struct tm  tm_val; 
		time_t rawtime = 0; 
		time_t time_now = time ( &rawtime ); 
		tm_val = *localtime ( &rawtime ); 
		char time_str[128] = {0};
		sprintf(time_str,"%d-%d-%d-%d:%d:%d",tm_val.tm_year + 1900,tm_val.tm_mon +1,tm_val.tm_mday,tm_val.tm_hour,tm_val.tm_min,tm_val.tm_sec);
		myini.Write("MemCache", "ErrDate",time_str);
		myini.Save();
	}
}

void MemCacheServerHandler::SetEventHandler( GameEventHandler* eh )
{
	m_eh = eh;
}

bool MemCacheServerHandler::RemoveUserInfo( const int64 &uid )
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}
	int     dbid    = g_pInst->GetServerId(uid);
	TCRDB*  pConn   = g_pInst->GetDB(dbid);
	if (pConn == NULL)
	{
		g_pInst->SetEnbale(false);
		int ecode = tcrdbecode(pConn);
		LOG4CXX_ERROR(logger,"memcache_remove_connect_null_error"<<tcrdberrmsg(ecode));
		return false;
	}
	char suid[32];
	sprintf(suid, "%016llX", uid);
	int klen = 16;
	if (!tcrdbout(pConn, suid, klen))
	{
		int ecode = tcrdbecode(pConn);
		//g_pInst->SetEnbale(false);
		LOG4CXX_ERROR(logger,"memcache_remove_out_error"<<tcrdberrmsg(ecode)<<uid);
		return false;
	}
	g_pInst->GetCounter().increase("mem_remove_cnt");
	return true;
}

bool MemCacheServerHandler::SafePushUser(const int64 &uid,User * pUser)
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}
//	g_pInst->acquireLock();
// 	if (!pUser->Online())
// 	{
// 		return false;
// 	}
	
	map<int64,User*> & user_map = g_pInst->GetUserList();
	user_map[uid] = pUser;
//	g_pInst->releaseLock();
	return true;
}

bool MemCacheServerHandler::UpdateRemoveUser()
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}

	time_t timeNow = Clock::getCurrentSystemTime();
	time_t last_time = g_pInst->GetRemoveUpdateTime();
	if (timeNow - last_time < GameConfig::GetInstance()->GetMemCacheUpdateTime())
	{
		return false;
	}

//	vector<int64> vecRemoveUser;
//	g_pInst->acquireLock();
	map<int64,bool>& remove_list = g_pInst->GetRemoveList();
//	map<int64,bool> remove_temp_list = remove_list;
//	g_pInst->releaseLock();

	Counter & counter = g_pInst->GetCounter();
	Statistics& stat = g_pInst->GetStatistics();

	GameEventHandler * eh = g_pInst->GetEventHandler();
	const hash_map<int64, User*> & user_map = eh->getDataHandler()->getUsers();
	const map<int64, User*> &dirty_map = eh->getDataHandler()->getDirtyUsers();
	int free_cnt(0);

	map<int64,bool>::iterator iter = remove_list.begin();
	while(iter != remove_list.end())
	{
		map<int64, bool>::iterator oiter = iter;
		iter ++;
		int64 uid = oiter->first;
		bool remove_flag(false);
		do 
		{
			hash_map<int64, User*>::const_iterator iter_user = user_map.find(uid);
			if (iter_user == user_map.end() )
			{
				remove_flag = true;
				break;
			}

			User * pUser = iter_user->second;
			if (pUser == NULL)
			{
				remove_flag = true;
				break;
			}

			if (pUser->Online())
			{
				break;
			}

			map<int64, User*>::const_iterator iter_dirty = dirty_map.find(pUser->id());
			if (iter_dirty != dirty_map.end())
			{
				break;
			}

			if (timeNow < pUser->getMemRevision() + GameConfig::GetInstance()->GetMemCacheRemoveTime() *1000/*3600 * 1000 * 24*/)
			{//delete less than 24h
				break;
			}

			remove_flag = true;

		} while (0);


		if (remove_flag)
		{
			g_pInst->RemoveUserInfo(uid);
			remove_list.erase(oiter);
			free_cnt++;
		}

		if (free_cnt >= GameConfig::GetInstance()->GetMemCacheFreeCnt())
		{
			break;
		}

		time_t time_last = Clock::getCurrentSystemTime();
		if (time_last - timeNow > 20)
		{//more than 20ms return
			break;
		}
	}

	g_pInst->SetRemoveUpdateTime(timeNow);
	return true;
}

Statistics & MemCacheServerHandler::GetStatistics()
{
	static Statistics temp;
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return temp;
	}
	return g_pInst->GetEventHandler()->getDataHandler()->getStatistics();
}

Counter & MemCacheServerHandler::GetCounter()
{
	static Counter temp;
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return temp;
	}
	return g_pInst->GetEventHandler()->getDataHandler()->getCounter();
}

bool MemCacheServerHandler::SafePushRemoveList( const int64& uid )
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}
	map<int64,bool> &remove_list = g_pInst->GetRemoveList();
	remove_list[uid] = true;
	return true;
}

bool MemCacheServerHandler::SaveAllUserData( int64 revision,bool urgent /* = false */ )
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}

	if (!urgent && revision < g_pInst->GetSaveUpdateTime() + GameConfig::GetInstance()->GetMemCacheSaveUpdateTime())
	{
		return false;
	}

	map<int64, User*> &users = g_pInst->GetUserList();
	map<int64, User*>::iterator iter = users.begin();
	int save_interval = GameConfig::GetInstance()->GetMemCacheSaveInterval();		//10s
	while(iter!=users.end())
	{
		map<int64, User*>::iterator oiter = iter;
		iter ++;
		User *user = oiter->second;
		if (user == NULL)
		{
			continue;
		}
		if( urgent || user->getMemRevision() < revision - save_interval)
		{
			string user_info;
			//user->GetDbUser().set_version(user->GetDbUser().version()+1);
			user->GetDbUser().SerializeToString(&user_info);
			bool sucess = MemCacheServerHandler::UpdateUserInfo(user->id(),user_info);
			if (sucess)
			{
				g_pInst->SafePushRemoveList(user->id());
				users.erase(oiter);
			}
		}
	}

	g_pInst->SetSaveUpdateTime(revision);
	return true;
}

bool MemCacheServerHandler::UpdateRankInfo(int rank_id,std::string dist)
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}

	TCRDB*  pConn   = g_pInst->GetDB(1);
	if (pConn == NULL)
	{
		g_pInst->SetEnbale(false);
		LOG4CXX_ERROR(logger,"memcache_put_rank_connect_null_error");
		return false;
	}

	const char* buf = dist.c_str();
	int         len = dist.length();
	char suid[32] = {0};
	sprintf(suid,"R%015d",rank_id);
	int klen = 16;
	if (!tcrdbput(pConn, suid, klen, buf, len))
	{
		int ecode = tcrdbecode(pConn);
		g_pInst->SetEnbale(false);
		LOG4CXX_ERROR(logger,"memcache_update_rank_error"<<tcrdberrmsg(ecode) << rank_id);
		return false;
	}

	return true;
}

bool MemCacheServerHandler::GetRankInfo(int rank_id,std::string& dist)
{
	if (g_pInst == NULL || !g_pInst->CanUse())
	{
		return false;
	}

	TCRDB*  pConn   = g_pInst->GetDB(1);
	if (pConn == NULL)
	{
		g_pInst->SetEnbale(false);
		LOG4CXX_ERROR(logger,"memcache_get_rank_connect_null_error");
		return false;
	}

	char suid[32];
	sprintf(suid,"R%015d",rank_id);

	int klen = 16;
	int len  = 0;

	char* buffer = (char*) tcrdbget(pConn, suid, klen, &len);
	if (buffer == NULL)
	{
		int ecode = tcrdbecode(pConn);
		if (ecode != TTENOREC)
		{
			g_pInst->SetEnbale(false);
			LOG4CXX_ERROR(logger,"memcache_get_rank_ecode_error");
		}
		return false;
	}
	std::string outbuf(buffer, len);
	dist = outbuf;
	free(buffer);
	return true;
}

