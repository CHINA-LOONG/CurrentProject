#ifndef _LOG_SERVER_MSG_LOG_H_
#define _LOG_SERVER_MSG_LOG_H_

#include <stdarg.h>
#ifdef _WIN32
#include <WinSock2.h>
#include <Windows.h>
#else
#include <sys/time.h>
#include <stdlib.h>
#endif
#include <time.h>

#include "logfmt.h"
#include "loglevel.h"

typedef struct x_va_list
{
	char* a0;
	int offset;
} va_mylist;

class TMsgLog
{
	typedef void*   TMsgLogHandle;

public: //method
	TMsgLog (unsigned int levelvalue, const char* operID);
	virtual ~TMsgLog (void);

	int local_log_init (const char* attribute);

	void setMsgID (unsigned long msgid);
	unsigned long getMsgID (void);

	void setLevel (unsigned int levelvalue);
	unsigned int getLevel (void);

	int reload (void);
	int setReload (int flag /*0: no auto reload; 1: auto reload*/);
	int IsReload (void); /*return 0: no auto reload; 1: auto reload*/

	int getRate (unsigned long msgid, double* rate);
	unsigned int getlocalip();	//return local ip in unsigned int

	int msglog (unsigned int level, time_t timestamp, const void* data, int len);
	inline int msglog (unsigned int level, const void* data, int len)
	{
		return msglog (level, time(0), data, len);
	}

	//binary debug log
	int binlog (unsigned int level, time_t timestamp, const void* data, int len);
	inline int binlog (unsigned level, const void* data, int len)
	{
		return binlog (level, time(0), data, len);
	}

	int msglog (unsigned int level, unsigned long msgid, time_t timestamp, const void* data, int len);
	inline int msglog (unsigned int level, unsigned long msgid, const void* data, int len)
	{
		return msglog (level, msgid, time(0), data, len);
	}

#ifdef _WIN32
	//通过UDP上报的msg，传入参数hashkey为计算上报server的依据
	int netprintf (unsigned long msgid, unsigned int hashkey, const char* fmt, ...);
	int netprintf (unsigned long msgid, unsigned int hashkey, const char* fmt, char* ap[]);
	//原有日志API
	int msgprintf (unsigned int level, const char* fmt, ...);
	int msgprintf (unsigned int level, time_t timestamp, const char* fmt, ...);
	int msgprintf (unsigned int level, unsigned long msgid, const char* fmt, ...);
	int msgprintf (unsigned int level, unsigned long msgid, time_t timestamp, const char* fmt, ...);
#else
	//通过UDP上报的msg，传入参数hashkey为计算上报server的依据
	int netprintf (unsigned long msgid, unsigned int hashkey, const char* fmt, ...)
		__attribute__((format(__printf__,4,5)));
	int netprintf (unsigned long msgid, unsigned int hashkey, const char* fmt, char* ap[]);
	//原有日志API
	int msgprintf (unsigned int level, const char* fmt, ...)
		__attribute__((format(__printf__,3,4)));
	int msgprintf (unsigned int level, time_t timestamp, const char* fmt, ...)
		__attribute__((format(__printf__,4,5)));
	int msgprintf (unsigned int level, unsigned long msgid, const char* fmt, ...)
		__attribute__((format(__printf__,4,5)));
	int msgprintf (unsigned int level, unsigned long msgid, time_t timestamp, const char* fmt, ...)
		__attribute__((format(__printf__,5,6)));
#endif
	int msgprintf (unsigned int level, unsigned long msgid, time_t timestamp, const char* fmt, char* ap[]);

	char* errmsg (void);

public: //property

private: //method
	//no allow copy construct & operator=
	TMsgLog (const TMsgLog&);
	TMsgLog& operator= (const TMsgLog&);

	bool IsTop (void *p, unsigned long msgid);
	int initlocalip(char *ethname = "eth1", int len = 4);

	inline unsigned int getMicroSecond (void)
	{
#ifdef _WIN32
		FILETIME filetime;
		GetSystemTimeAsFileTime(&filetime);
		return ((time_t(filetime.dwHighDateTime)<<32)+filetime.dwLowDateTime)/10000;
#else
		timeval systime;
		gettimeofday(&systime, NULL);
		return (time_t(systime.tv_sec)*1000 + systime.tv_usec/1000);
#endif
	}

	int udpmsg_stat(void *p, time_t now, int errcode);
	int getRateInternal (unsigned long msgid, double* rate);

private: //property
	TMsgLogHandle   _pMsgLogHandle;
	unsigned int    m_uiSeed;
	int             m_iTimes;
	unsigned long 	_local_ip;	//refer to struct in_addr.s_addr
};

class TLogFilter
{
public: //method
	TLogFilter (void);
	virtual ~TLogFilter (void);

	int reload (void);
	int setReload (int flag /*0: no auto reload; 1: auto reload*/);
	int IsReload (void); /*return 0: no auto reload; 1: auto reload*/
	int getRate (unsigned long msgid, double* rate);
	int getLocalIp ( char *szLocalIp, int len ); //add by tomsonxu on 20080110

	bool IsTop (unsigned long msgid);
	inline unsigned int getMicroSecond (void)
	{
#ifdef _WIN32
		FILETIME filetime;
		GetSystemTimeAsFileTime(&filetime);
		return ((time_t(filetime.dwHighDateTime)<<32)+filetime.dwLowDateTime)/10000;
#else
		timeval systime;
		gettimeofday(&systime, NULL);
		return (time_t(systime.tv_sec)*1000 + systime.tv_usec/1000);
#endif
	}

public: //property

protected: //method
protected: //property

private: //method
	//no allow copy construct & operator=
	TLogFilter (const TLogFilter&);
	TLogFilter& operator= (const TLogFilter&);

private: //property
	unsigned int    m_uiSeed;
	int             m_iTimes;
	void*			m_pHandle;
};

class TLogColoration
{
public: //method
public: //property
	TLogColoration (const char* bitmapfile);
	virtual ~TLogColoration (void);

	bool InSet (unsigned int id);

protected: //method
protected: //property

private: //method
private: //property
	void*           m_pBitMap;
	unsigned int    m_iFileSize;
};

#endif //_LOG_SERVER_MSG
