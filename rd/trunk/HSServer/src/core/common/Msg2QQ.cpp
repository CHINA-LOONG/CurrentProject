#include <time.h>
#include <math.h>

#include "Msg2QQ.h"
#include <sys/types.h>
#include "string-util.h"
#ifndef _WIN32
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#endif

#include "../logic/User.h"
#include "../logic/Player.h"

const int QQ_LOGID		= 1000000017;
const int QQ_APPID		= 100633004 ;
const int QQ_VERSION		= 1;
const int QQ_SOURCE_XIAOYOU	= 2;
const int QQ_SOURCE_KONGJIAN    = 1;
const int QQ_MANYOU	= 15;
const int QQ_SHOUJIKONGJIAN = 8;
const int QQ_IOS = 18;
const int QQ_ANDORID = 19;
const int QQ_STATE		= 0;

/*
log日志ID：
2025000021 

模块ID:
梦幻海底用户数据读取   216000028 
梦幻海底用户数据写入   216000029
梦幻海底用户数据库      216000031

接口ID：
读取梦幻海底用户数据  116418067  
写入梦幻海底用户数据  116418068 

string ip1 = “172.30.10.12”;  //主调模块所在server的IP

string ip2 = “172.30.10.15”; //被调模块所在server的IP

int res = 0;                    //调用返回值

int use_time = 327.478;       //调用延时
 */
const long unsigned int QQ_DB_LOGID		= 2025000021L;
const unsigned int QQ_DB_LOAD_MID	= 216000028; //被调模块ID
const unsigned int QQ_DB_SAVE_MID	= 216000029; //被调模块ID
const unsigned int QQ_DB_ID			= 216000031; //主调模块ID

const unsigned int QQ_DB_SAVE_FID	= 116418067; //被调模块接口ID
const unsigned int QQ_DB_LOAD_FID	= 116418068; //被调模块接口ID

CMsg2QQ::CMsg2QQ() : m_msg(3, "app"),m_nSafetoQQMax(0),m_id(0)
{
    logger_ = log4cxx::Logger::getLogger("Msg2QQ");
}

CMsg2QQ::~CMsg2QQ()
{
}

CMsg2QQ* CMsg2QQ::GetInstance()
{
    static CMsg2QQ log;
    return &log;
}

void CMsg2QQ::TellMsg_deleted_function(MSG2QQType emType, User* pUser, int nGoldChg, int nExpChg, unsigned int uFrdPlatformID)
{
    if (pUser == NULL)
        return;
    //nGoldChg and nExpChg is unsigned int
    //nGoldChg = abs(nGoldChg);
    //nExpChg = abs(nExpChg);
    unsigned int uiRequestUin = pUser->platform_id_2int();
    unsigned int uiClientIP = inet_addr(pUser->lastLoginIP().c_str());
    unsigned int uiOperTime = (unsigned int) time(NULL);
    unsigned int uiSource = 2;
    if (pUser->getPlatType() == PLAT_XIAOYOU)
    {
        uiSource = QQ_SOURCE_XIAOYOU;
    }
    else
    {
        uiSource = QQ_SOURCE_KONGJIAN;
    }
    int nRtn = m_msg.netprintf(QQ_LOGID, uiRequestUin, "%d%d%d%d%d%d%d%s%d%d%d%d%s",
            QQ_APPID, QQ_VERSION, uiSource, (int) emType, QQ_STATE,
            uiRequestUin, uFrdPlatformID, pUser->platform_id().c_str(),
            uiClientIP, uiOperTime,
            nGoldChg, nExpChg, "");

    char szInfo[1024];
    sprintf(szInfo, "Rtn:%d,Send:%d,%d,%d,%d,%d,%u,%u,%s,%u,%u,%u,%u,%s\n",
            nRtn, QQ_APPID, QQ_VERSION, uiSource, (int) emType, QQ_STATE,
            uiRequestUin, uFrdPlatformID, pUser->platform_id().c_str(),
            uiClientIP, uiOperTime,
            nGoldChg, nExpChg, "");
    LOG4CXX_INFO(logger_, "Msg2QQ:" << szInfo);
}

void CMsg2QQ::SetIP(const std::string& ip)
{
	m_SvrIP = ip;
    unsigned long test_ip = inet_addr(ip.c_str());
    if ((int)test_ip == -1)
    {
        cout << "error_ip:" << ip << "	" << "error_ret:" << test_ip << "	" << endl;
        m_uiServerIp = ntohl(inet_addr("192.168.1.137"));
    }
    else
    {
        m_uiServerIp = ntohl(test_ip);
    }
}

void CMsg2QQ::TellMsg(MSG2QQType emType, User* pUser, int nGoldChg, int nExpChg, unsigned int uFrdPlatformID)
{
    if (m_enable == false)
    {
        return;
    }
    if (pUser == NULL)
        return;
    //nGoldChg and nExpChg is unsigned int
    //nGoldChg = abs(nGoldChg);
    //nExpChg = abs(nExpChg);
    
    //unsigned int uiRequestUin = pUser->platform_id_2int();
    unsigned int uiClientIP = ntohl(inet_addr(pUser->lastLoginIP().c_str()));
    unsigned int uiOperTime = (unsigned int) time(NULL);
#ifndef _WIN32
#ifdef _NEWLOG2QQ
    //	LOG4CXX_INFO(logger_, "Msg2QQ:" << request.c_str());
    DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
    timeval systime;
    QQLogger.get_time(systime);
    uiOperTime = systime.tv_sec;
#endif
#endif
    unsigned int lastTime  = (unsigned int) (time(NULL) - pUser->lastLoginTime());
    unsigned int uiSource = ConvertUiSource(pUser);

    // 	version=&appid=&userip=&svrip=&time=&domain=&worldid=&optype=&actionid=
    // 		&opuid=&opopenid=&touid=&toopenid=&level=&source=&itemid=&itemtype=
    // 	 &itemcnt=&modifyexp=&totalexp=&modifycoin=&totalcoin=&modifyfee=&totalfee=
    // 	 &onlinetime=&key=&keycheckret=&safebuf=&remark=&user_num=
    std::string request = "";
    AddValueToString<int>(request, "version", QQ_VERSION);
    AddValueToString<int>(request, "appid", QQ_APPID);
    AddValueToString<unsigned int>(request, "userip", uiClientIP);
    AddValueToString<unsigned int>(request, "svrip", m_uiServerIp);
    AddValueToString<unsigned int>(request, "time", uiOperTime);
    AddValueToString<int>(request, "domain", uiSource);
    AddValueToString<int>(request, "worldid", 1);
    AddValueToString<int>(request, "optype", OT_OTHER);
    AddValueToString<int>(request, "actionid", emType);
    AddValueToString<const char*>(request, "opuid", (toString<int64 > (pUser->id())).c_str());
    AddValueToString<const char*>(request, "opopenid", pUser->platform_id().c_str());
    AddValueToString<const char*>(request, "touid", ""/*toString<int64>(toUid)*/);
    AddValueToString<const char*>(request, "toopenid", ""/*toOpenID*/);
	AddValueToString<int>(request, "level", pUser->GetPlayer()->GetLevel());
    AddValueToString<const char*>(request, "source", pUser->getActionFrom().c_str());
    AddValueToString<const char*>(request, "itemid", "");
    AddValueToString<const char*>(request, "itemtype", "");
    AddValueToString<const char*>(request, "itemcnt", "");
    AddValueToString<int>(request, "modifyexp", nExpChg);
    AddValueToString<int64 > (request, "totalexp", pUser->GetPlayer()->GetExp());
    AddValueToString<int>(request, "modifycoin", nGoldChg);
    AddValueToString<int64 > (request, "totalcoin", pUser->GetPlayer()->GetMoney());
    AddValueToString<const char*>(request, "modifyfee", "");
    AddValueToString<const char*>(request, "totalfee", "");
    AddValueToString<int>(request, "onlinetime", lastTime);
    AddValueToString<const char*>(request, "key", pUser->getOpenKey().c_str());
    AddValueToString<int>(request, "keycheckret", 0);
    AddValueToString<char*>(request, "safebuf", "");
    AddValueToString<char*>(request, "remark", "");
    AddValueToString<const char*>(request, "user_num", "");

#ifndef _WIN32
#ifdef _NEWLOG2QQ
    LOG4CXX_INFO(logger_, "Msg2QQ:" << request.c_str());
    //    DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
    int err = QQLogger.write_baselog(DataCollector::LT_NORMAL, request, true);
    if (err != 0)
    {
        std::string err_msg = QQLogger.get_errmsg();
        LOG4CXX_INFO(logger_, "Msg2QQ: [ERR] " << err_msg);
    }
#endif
#endif
}

void CMsg2QQ::TellMsgEx(MSG2QQType action_id, string action_from, OPTTYPE op_type, User * user, int64 to_uid, string to_open_id, int gold_change, int exp_change, int item_id, int item_type, int item_num, int qq_change /* = 0 */)
{
    if (m_enable == false)
    {
        return;
    }
    if (user == NULL)
        return;
    unsigned int uiClientIP = ntohl(inet_addr(user->lastLoginIP().c_str()));
    unsigned int uiOperTime = (unsigned int) time(NULL);
#ifndef _WIN32
#ifdef _NEWLOG2QQ
    //	LOG4CXX_INFO(logger_, "Msg2QQ:" << request.c_str());
    DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
    timeval systime;
    QQLogger.get_time(systime);
    uiOperTime = systime.tv_sec;
#endif
#endif

    unsigned int lastTime  = (unsigned int) (time(NULL) - user->lastLoginTime());
    unsigned int uiSource = ConvertUiSource(user);
    if (action_from.empty())
    {
        action_from = user->getActionFrom();
    }
    // 	version=&appid=&userip=&svrip=&time=&domain=&worldid=&optype=&actionid=
    // 		&opuid=&opopenid=&touid=&toopenid=&level=&source=&itemid=&itemtype=
    // 	 &itemcnt=&modifyexp=&totalexp=&modifycoin=&totalcoin=&modifyfee=&totalfee=
    // 	 &onlinetime=&key=&keycheckret=&safebuf=&remark=&user_num=
    std::string request = "";
    AddValueToString<int>(request, "version", QQ_VERSION);
    AddValueToString<int>(request, "appid", QQ_APPID);
    AddValueToString<unsigned int>(request, "userip", uiClientIP);
    AddValueToString<unsigned int>(request, "svrip", m_uiServerIp);
    AddValueToString<unsigned int>(request, "time", uiOperTime);
    AddValueToString<int>(request, "domain", uiSource);
    AddValueToString<int>(request, "worldid", 1);
    AddValueToString<int>(request, "optype", op_type);
    AddValueToString<int>(request, "actionid", action_id);
    AddValueToString<const char*>(request, "opuid", (toString<int64 > (user->id())).c_str());
    AddValueToString<const char*>(request, "opopenid", user->platform_id().c_str());
    AddValueToString<const char*>(request, "touid", (toString<int64 > (to_uid)).c_str());
    AddValueToString<const char*>(request, "toopenid", to_open_id.c_str());
    AddValueToString<int>(request, "level", user->GetPlayer()->GetLevel());
#ifndef _WIN32
#ifdef _NEWLOG2QQ
    //	LOG4CXX_INFO(logger_, "Msg2QQ:" << request.c_str());
    //	DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
    std::string str_dec;
    QQLogger.encode(action_from, str_dec);
    action_from = str_dec;
#endif
#endif
    AddValueToString<const char*>(request, "source", action_from.c_str());
    AddValueToString<int>(request, "itemid", item_id);
    AddValueToString<int>(request, "itemtype", item_type);
    AddValueToString<int>(request, "itemcnt", item_num);
    AddValueToString<int>(request, "modifyexp", exp_change);
    AddValueToString<int64 > (request, "totalexp", user->GetPlayer()->GetExp());
    AddValueToString<int>(request, "modifycoin", gold_change);
    AddValueToString<int64 > (request, "totalcoin", user->GetPlayer()->GetMoney());
    AddValueToString<int>(request, "modifyfee", qq_change);
    AddValueToString<const char*>(request, "totalfee", "");
    AddValueToString<int>(request, "onlinetime", lastTime);
    AddValueToString<const char*>(request, "key", user->getOpenKey().c_str());
    AddValueToString<int>(request, "keycheckret", 0);
    AddValueToString<char*>(request, "safebuf", "");
    AddValueToString<char*>(request, "remark", "");
    AddValueToString<const char*>(request, "user_num", "");

#ifndef _WIN32
#ifdef _NEWLOG2QQ
    LOG4CXX_INFO(logger_, "Msg2QQ:" << request.c_str());
    //DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
    int err = QQLogger.write_baselog(DataCollector::LT_NORMAL, request, true);
    if (err != 0)
    {
        std::string err_msg = QQLogger.get_errmsg();
        LOG4CXX_INFO(logger_, "Msg2QQ: [ERR] " << err_msg);
    }
#endif
#endif
}

void CMsg2QQ::TellMsgLogin0n(User * pUser, string action_from)
{
    TellMsgEx(MQ_USER_LOGIN, action_from, OT_READ, pUser, 0, "", 0, 0, 0, 0, 0, 0);
}

void CMsg2QQ::TellMsgLoginOut( User * pUser )
{
    TellMsgEx(MQ_USER_LOGOUT, "", OT_READ, pUser, 0, "", 0, 0, 0, 0, 0, 0);
}

void CMsg2QQ::TellMsgRegister(MSG2QQType action_id, User * pUser, string action_from)
{
    TellMsgEx(action_id, action_from, OT_WRITE, pUser, 0, "", 0, 0, 0, 0, 0, 0);
}

void CMsg2QQ::TellMsgVisitFrd( User * pUser, int64 toUid, string toOpenID )
{
    TellMsgEx(MQ_VisitFrd, "", OT_OTHER, pUser, toUid, toOpenID, 0, 0, 0, 0, 0, 0);
}

void CMsg2QQ::TellMsgVisitBack( User * pUser, int64 toUid, string toOpenID )
{
    TellMsgEx(MQ_Back2Home, "", OT_OTHER, pUser, toUid, toOpenID, 0, 0, 0, 0, 0, 0);
}

void CMsg2QQ::TellMsgWebBuy(User * pUser, int item_id, int item_type, int item_num, int qq_change)
{
    TellMsgEx(MQ_WebBuy, "", OT_OTHER, pUser, 0, "", 0, 0, item_id, item_type, item_num, qq_change);
}

void CMsg2QQ::TellMsgSimple( MSG2QQType action_id, User * pUser )
{
    TellMsgEx(action_id, "", OT_OTHER, pUser, 0, "", 0, 0, 0, 0, 0, 0);
}

void CMsg2QQ::TellMsgMore( MSG2QQType action_id, User * pUser, int nGoldChg, int nExpChg )
{
    TellMsgEx(action_id, "", OT_OTHER, pUser, 0, "", nGoldChg, nExpChg, 0, 0, 0, 0);
}

void CMsg2QQ::TellMsgWebMore( MSG2QQType action_id, User * pUser, int item_id, int item_type, int item_num, int qq_change )
{
    TellMsgEx(action_id, "", OT_OTHER, pUser, 0, "", 0, 0, item_id, item_type, item_num, qq_change);
}

void CMsg2QQ::TellMsgProto(User* user, int protonum, bool succ, const string protoname)
{
//     if (m_enable == false)
//     {
//         return;
//     }
	if (m_nSafetoQQMax < m_id)
	{
		return;
	}
    if (user == NULL)
        return;

    int64 uiOperTime = time(NULL);
#ifndef _WIN32
#ifdef _NEWLOG2QQ
    DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
    timeval systime;
    QQLogger.get_time(systime);
    uiOperTime = systime.tv_sec * 1000 + systime.tv_usec/1000;
#endif
#endif
    unsigned int uiSource = ConvertUiSource(user);

	//APPV=1.0&MSGV=1.1&VER=1.3&APPID=58473&OID=efad1235t67d&WID=1&UIP=178.14.
	//147.12&OKY=&SIP=&MTM=1320056796032&DOM=1&MLV=0&AID=1
    std::string request = "";
    AddValueToString<const char*>(request,"APPV","20120306");
    AddValueToString<const char*>(request,"MSGV","20120306");
    AddValueToString<int>(request, "VER", QQ_VERSION);
    AddValueToString<int>(request, "APPID", QQ_APPID);
    AddValueToString<const char*>(request, "OID", user->platform_id().c_str());
    AddValueToString<int>(request, "WID", 1);
    AddValueToString<const char*>(request, "UIP", user->lastLoginIP().c_str());
	AddValueToString<const char*>(request,"OKY",user->getOpenKey().c_str());
    AddValueToString<const char*>(request, "SIP", m_SvrIP.c_str());
    AddValueToString<int64>(request, "MTM", uiOperTime);
    AddValueToString<int>(request, "DOM", uiSource);
    AddValueToString<int>(request, "MLV", 0);
	AddValueToString<int>(request,"AID",13);
	AddValueToString<int>(request,"PID",protonum);
    AddValueToString<const char*>(request, "FID", protoname.c_str());
    AddValueToString<int>(request, "PTP", 1);
    AddValueToString<int>(request, "RST", (succ ? 0 : 1));

#ifndef _WIN32
#ifdef _NEWLOG2QQ
    LOG4CXX_INFO(logger_, "Msg2QQ:" << request.c_str());
    //DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
    int err = QQLogger.write_baselog(DataCollector::LT_SECDATA, request, false);
    if (err != 0)
    {
        std::string err_msg = QQLogger.get_errmsg();
        LOG4CXX_INFO(logger_, "Msg2QQ: [ERR] " << err_msg);
    }
#endif
#endif
}

void CMsg2QQ::TellMsgFriendOPRP(User * user,const string friend_openid,bool succ, int protonum,const string protoname)
{
	if (m_nSafetoQQMax < m_id)
	{
		return;
	}
	if (user == NULL)
		return;

	int64 uiOperTime = time(NULL);
#ifndef _WIN32
#ifdef _NEWLOG2QQ
	DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
	timeval systime;
	QQLogger.get_time(systime);
	uiOperTime = systime.tv_sec * 1000 + systime.tv_usec/1000;
#endif
#endif
	unsigned int uiSource = ConvertUiSource(user);
	//APPV=1.0&MSGV=1.1&VER=1.3&APPID=58473&OID=efad1235t67d&WID=1&UIP=178.14.
	//147.12&OKY=&SIP=&MTM=1320056796032&DOM=1&MLV=0&AID=1
	std::string request = "";
	AddValueToString<const char*>(request,"APPV","20120306");
	AddValueToString<const char*>(request,"MSGV","20120306");
	AddValueToString<int>(request, "VER", QQ_VERSION);
	AddValueToString<int>(request, "APPID", QQ_APPID);
	AddValueToString<const char*>(request, "OID", user->platform_id().c_str());
	AddValueToString<int>(request, "WID", 1);
	AddValueToString<const char*>(request, "UIP", user->lastLoginIP().c_str());
	AddValueToString<const char*>(request,"OKY",user->getOpenKey().c_str());
	AddValueToString<const char*>(request, "SIP", m_SvrIP.c_str());
	AddValueToString<int64>(request, "MTM", uiOperTime);
	AddValueToString<int>(request, "DOM", uiSource);
	AddValueToString<int>(request, "MLV", 0);
	AddValueToString<int>(request,"AID",10);

	AddValueToString<int>(request,"OPID",protonum);
	AddValueToString<const char*>(request, "FID", protoname.c_str());
	AddValueToString<int>(request,"BID",0);
	AddValueToString<int>(request,"MDX",0);
	AddValueToString<int>(request,"MDY",0);
	AddValueToString<int>(request,"TTI",0);
	AddValueToString<int>(request,"TID",0);
	AddValueToString<int>(request,"OBT",0);
	AddValueToString<int>(request,"ORT",0);
	AddValueToString<int>(request, "RST", (succ ? 0 : 1));
	AddValueToString<const char*>(request,"FOID",friend_openid.c_str());

#ifndef _WIN32
#ifdef _NEWLOG2QQ
	LOG4CXX_INFO(logger_, "Msg2QQ:" << request.c_str());
	//DataCollector::CLogger& QQLogger = CMsg2QQDB::GetInstance()->GetQQLogger();
	int err = QQLogger.write_baselog(DataCollector::LT_SECDATA, request, false);
	if (err != 0)
	{
		std::string err_msg = QQLogger.get_errmsg();
		LOG4CXX_INFO(logger_, "Msg2QQ: [ERR] " << err_msg);
	}
#endif
#endif
}

unsigned int CMsg2QQ::ConvertUiSource( User *pUser )
{
	unsigned int uiSource = QQ_SOURCE_KONGJIAN;
	if (pUser == NULL)
	{
		return uiSource;
	}

	if (pUser->getPlatType() == PLAT_QZONE)
	{
		uiSource = QQ_SOURCE_KONGJIAN;
	}
	if (pUser->getPlatType() == PLAT_XIAOYOU)
	{
		uiSource = QQ_SOURCE_XIAOYOU;
	}
	else if (pUser->getPlatType() == PLAT_MANYOU)
	{
		uiSource = QQ_MANYOU;
	}
	else if (pUser->getPlatType() == PLAT_WAP)
	{
		uiSource = QQ_SHOUJIKONGJIAN;
	}
	else if (pUser->getPlatType() == 7)
	{
		if (pUser->czDeviceType.find("iphone") != -1)
		{
			uiSource = QQ_IOS;
		}
		else
		{
			uiSource = QQ_ANDORID;
		}
	}
	else
	{
		uiSource = QQ_SHOUJIKONGJIAN;
	}

	return uiSource;
}

CMsg2QQDB::CMsg2QQDB()
: m_msg(3, "app")
{
    logger_ = log4cxx::Logger::getLogger("Daemon");
#ifndef _WIN32
#ifdef _NEWLOG2QQ
    std::string appname = "appoperlog";
    QQLogger.init(appname);
#endif
#endif
}

CMsg2QQDB::~CMsg2QQDB()
{

}

CMsg2QQDB* CMsg2QQDB::GetInstance()
{
    static CMsg2QQDB log;
    return &log;
}

void CMsg2QQDB::TellSaveDB(int nUseTime, const char* szIP1, const char* szIP2, int nPort2)
{
    if (m_enable == false)
    {
        return;
    }
    int nRtn = 0;
    /*
    int nRtn = m_msg.msgprintf(
        MAINTENANCE_LOG_LEVEL_ERR, QQ_DB_LOGID, time(NULL),
        MAINTENANCE_MONITOR_MODULE_INTERFACE_LOG_FORMAT,
        QQ_DB_ID, QQ_DB_SAVE_MID, QQ_DB_SAVE_FID,
        inet_addr(szIP1), inet_addr(szIP2), 0, nPort2,
        "GameDataSaver.cpp", 490, "2010-11-02 19:48:43",
        "write", 0, 0, nUseTime,
        -1, -1, -1, -1, -1, "", "", "", "", ""
        );
     */
    // 	tname=&tip=&spath=&tpath=&sproc=&tproc=&sport=&tport=
    // 		&smod=&tmod=&sact=&tact=&code=&ptime=&log=
    char szInfo[1024];
    sprintf(szInfo, "Rtn:%d,Send:%d,%d,%d,%s,%s,%d,%d\n",
            nRtn, QQ_DB_ID, QQ_DB_SAVE_MID, QQ_DB_SAVE_FID, szIP1, szIP2, nPort2, nUseTime);
    std::string request = "";
    AddValueToStringEx<string > (request, "tname", "DB");
    AddValueToStringEx<unsigned int>(request, "tip", ntohl(inet_addr(szIP2)));
    AddValueToStringEx<char*>(request, "spath", "/usr/local/services/market");
    AddValueToStringEx<char*>(request, "tpath", "/usr/local/services/bin");
    AddValueToStringEx<char*>(request, "sproc", "gamed");
    AddValueToStringEx<char*>(request, "tproc", "ttserver");
    AddValueToStringEx<char*>(request, "tact" , "SaveDB");
    AddValueToStringEx<int>(request, "ptime", nUseTime);
    AddValueToStringEx<int>(request, "code", 0);
    AddValueToStringEx<const char*>(request, "log", "");

#ifndef _WIN32
#ifdef _NEWLOG2QQ
    LOG4CXX_INFO(logger_, "Msg2QQ:" << szInfo);
    int err = QQLogger.write_baselog(DataCollector::LT_MOD, request, false);
    if (err != 0)
    {
        std::string err_msg = QQLogger.get_errmsg();
        LOG4CXX_INFO(logger_, "Msg2QQ: [ERR] " << err_msg);
    }
#endif
#endif
}

void CMsg2QQDB::TellLoadDB(int nUseTime, const char* szIP1, const char* szIP2, int nPort2)
{
    if (m_enable == false)
    {
        return;
    }
    int nRtn = m_msg.msgprintf(
            MAINTENANCE_LOG_LEVEL_ERR, QQ_DB_LOGID, time(NULL),
            MAINTENANCE_MONITOR_MODULE_INTERFACE_LOG_FORMAT,
            QQ_DB_ID, QQ_DB_LOAD_MID, QQ_DB_LOAD_FID,
            inet_addr(szIP1), inet_addr(szIP2), 0, nPort2,
            "GameDataSaver.cpp", 527, "2010-11-02 19:48:43",
            "read", 0, 0, nUseTime,
            -1, -1, -1, -1, -1, "", "", "", "", ""
            );

    //    char szInfo[1024];
    //    sprintf(szInfo, "Rtn:%d,Send:%d,%d,%d,%s,%s,%d,%d\n",
    //            nRtn, QQ_DB_ID, QQ_DB_LOAD_MID, QQ_DB_LOAD_FID, szIP1, szIP2, nPort2, nUseTime);
    //    LOG4CXX_INFO(logger_, "Msg2QQ:" << szInfo);
    char szInfo[1024];
    sprintf(szInfo, "Rtn:%d,Send:%d,%d,%d,%s,%s,%d,%d\n",
            nRtn, QQ_DB_ID, QQ_DB_SAVE_MID, QQ_DB_SAVE_FID, szIP1, szIP2, nPort2, nUseTime);


    std::string request = "";
    AddValueToStringEx2<string > (request, "tname", "DB");
    AddValueToStringEx2<unsigned int>(request, "tip", ntohl(inet_addr(szIP2)));
    AddValueToStringEx2<char*>(request, "spath", "/usr/local/services/market");
    AddValueToStringEx2<char*>(request, "tpath", "/usr/local/services/bin");
    AddValueToStringEx2<char*>(request, "sproc", "gamed");
    AddValueToStringEx2<char*>(request, "tproc", "ttserver");
    AddValueToStringEx2<char*>(request, "tact" , "LoadDB");
    AddValueToStringEx2<int>(request, "ptime", nUseTime);
    AddValueToStringEx2<int>(request, "code", 0);
    AddValueToStringEx2<const char*>(request, "log", "");
#ifndef _WIN32
#ifdef _NEWLOG2QQ
    LOG4CXX_INFO(logger_, "Msg2QQ:" << szInfo);
    int err = QQLogger.write_baselog(DataCollector::LT_MOD, request, false);
    if (err != 0)
    {
        std::string err_msg = QQLogger.get_errmsg();
        LOG4CXX_INFO(logger_, "Msg2QQ: [ERR] " << err_msg);
    }
#endif
#endif
}

#ifndef _WIN32
#ifdef _NEWLOG2QQ

DataCollector::CLogger& CMsg2QQDB::GetQQLogger()
{
    return QQLogger;
}
#endif
#endif

