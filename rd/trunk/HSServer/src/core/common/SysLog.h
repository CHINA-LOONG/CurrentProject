#pragma once
#include <fstream>
#include <iostream>
#include <string>
#ifdef _WIN32
#include <WinSock2.h>
#include "wtypes.h"
#include <Windows.h>
#endif

#include <pthread.h> 
#include "const_def.h"
#include "SysLogNet.h"
const int	TURNON_LOG				= 1;			//是否写入日志	
const int	TURNON_DEBUG			= 1;			//是否写入debug日志
using namespace std;

enum LOG_TYPE
{
    LT_LogOn = 0,
    LT_LogOut,
    LT_Register,
    LT_TimerEvent,

    LT_WebBuy,
    LT_WebBuyDetail,

    LT_LogTypeNormal,
    LT_LogTypeMustSend,
    LT_logTypeNoSend,
} ;

const char LOG_MSG[][16] = {
    "1001", "LogOn          ",
    "1002", "LogOut         ",
    "1003", "RegisterUser   ",
    "1009", "TimerEvent     ",

    "2001", "WebBuy         ",
    "2002", "WebBuyDetail   ",
    "9999", "Normal         ",
};

const int MAX_LOG_SMG_LENGTH = 1024 * 5;

class CSysLog
{
public:

private:
    CSysLog(void);
    ~CSysLog(void);
public:
    static CSysLog* GetInstance();

    void SetLogInfo(bool bGameLog, int nSrvID, string strLogDir, string strLogName, string strAddr, string strPort, bool bShowLog, int nSendLogLv, int nModul, int nModulVal);
    bool CreateLog();
    bool CreateDir(const char* szDir);
    void ChgLogFile();

    void WriteCurTime();

    ofstream* GetSysLogOF()
    {
        return m_pLogSys;
    }

    bool IsStatUser(int64 nUserID);

    int	 GetSrvID()
    {
        return m_nSrvID;
    }
    void BeginMsg();
    void EndMsg(int64 nUserID, LOG_TYPE emType, int nDmd);
    bool InUse();
    bool NeedSend(int64 nUserID, LOG_TYPE emType, int nDmd);
#ifdef _WIN32

    inline void LogLock()
    {
        EnterCriticalSection( &m_Section );
    }

    inline void LogUnLock()
    {
        LeaveCriticalSection(&m_Section);
    }
#else

    inline void LogLock()
    {
        pthread_mutex_lock(&m_mutex);
    }

    inline void LogUnLock()
    {
        pthread_mutex_unlock(&m_mutex);
    }
#endif

    template <class T> CSysLog & operator << (const T &value)
    {
        sprintf(m_szBuf + m_nPos, "%d,", value);
        m_nPos = strlen(m_szBuf);

        //if(m_pLogSys&&m_bShowLog)
        //	*m_pLogSys << value <<",";
        //if(m_bSendLog)
        //{
        //	sprintf(m_szBuf+m_nPos,"%d,",value);
        //	m_nPos = strlen(m_szBuf);
        //}
        return *this;
    }
    CSysLog & operator << (int64 value);
    CSysLog & operator << (char value) ;
    CSysLog & operator << (double value) ;
    CSysLog & operator << (float value) ;
    CSysLog & operator << (char* szBuf);
    CSysLog & operator << (const char* szBuf);
    CSysLog & operator << (string& str);
    CSysLog & operator << (const string& str);
private:

private:
    CSysLogNet	m_SysLogNet;
    bool		m_bShowLog;
    int			m_nSendLogLv;
    int			m_nModul;
    int			m_nModulVal;
    bool		m_bInUse;
    int			m_nSrvID;
    string		m_strLogDir;
    string		m_strLogName;
    bool		m_bGameLog;		//true:gamelog false:halllog

    string		m_strLogDay;
    ofstream*	m_pLogSys;
    time_t		m_lastCheck;
    int			m_nPos;
    char		m_szBuf[MAX_LOG_SMG_LENGTH];
#ifdef _WIN32
    CRITICAL_SECTION m_Section;
#else
    pthread_mutex_t m_mutex;
#endif

} ;

#define CHG_LOG_FILE()	{ CSysLog::GetInstance()->ChgLogFile();}
#define SYS_LOG(uid,logType,succ,dmd,msg) \
{\
	{\
	CSysLog* pLog = CSysLog::GetInstance();\
	if(pLog->InUse())\
	{\
		pLog->LogLock();\
		pLog->BeginMsg();\
		pLog->WriteCurTime();\
		*pLog << pLog->GetSrvID()<<uid<<LOG_MSG[logType*2]<<LOG_MSG[logType*2+1]<<succ<<msg;\
		pLog->EndMsg(uid,logType,dmd);\
		pLog->LogUnLock();\
	}\
	}\
}
#define SYS_LOG_EASY(uid,openid,logType,logTypeStr,msg) \
{\
	{\
	CSysLog* pLog = CSysLog::GetInstance();\
	if(pLog->InUse())\
	{\
		pLog->LogLock();\
		pLog->BeginMsg();\
		pLog->WriteCurTime();\
		*pLog << pLog->GetSrvID()<<uid<<openid<<logTypeStr<<msg;\
		pLog->EndMsg(uid,logType,0);\
		pLog->LogUnLock();\
	}\
	}\
}
//old,need "," at code
/**
//#define SYS_LOG(uid,logType,succ,msg) //{//	CSysLog* pLog = CSysLog::GetInstance();//	ofstream* pOF = pLog->GetSysLogOF();//	if(pOF)//	{//		pLog->LogLock();//		pLog->WriteCurTime(pOF);//		*pOF << pLog->GetSrvID()<<","<<uid<<","<<LOG_MSG[logType*2]<<","<<LOG_MSG[logType*2+1]<<","<<succ<<",";//		*pOF << msg <<std::endl;//		pLog->LogUnLock();//	}
//}

 */

