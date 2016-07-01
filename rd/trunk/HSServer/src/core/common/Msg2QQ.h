#pragma once
#include "../common/const_def.h"
#ifndef _WIN32
#include <log4cxx/logger.h>
#include "../logapi/include/msglogapi.h"
#define _NEWLOG2QQ
#ifdef _NEWLOG2QQ

#include "../logapi/include/dcapi_cpp.h"
#endif
#else
#include "../common/Logger_win.h"
#define MAINTENANCE_LOG_LEVEL_ERR 3
#define MAINTENANCE_MONITOR_MODULE_INTERFACE_LOG_FORMAT ""

class TMsgLog
{
public:

    TMsgLog (unsigned int levelvalue, const char* operID)
    {
    }

    ~TMsgLog (void)
    {
    }
    //ͨ��UDP�ϱ���msg���������hashkeyΪ�����ϱ�server������

    int netprintf (unsigned long msgid, unsigned int hashkey, const char* fmt, ...)
    {
        return 0;
    }

    int netprintf (unsigned long msgid, unsigned int hashkey, const char* fmt, char* ap[])
    {
        return 0;
    }
    //ԭ����־API

    int msgprintf (unsigned int level, const char* fmt, ...)
    {
        return 0;
    }

    int msgprintf (unsigned int level, time_t timestamp, const char* fmt, ...)
    {
        return 0;
    }

    int msgprintf (unsigned int level, unsigned long msgid, const char* fmt, ...)
    {
        return 0;
    }

    int msgprintf (unsigned int level, unsigned long msgid, time_t timestamp, const char* fmt, ...)
    {
        return 0;
    }
} ;
#endif

 class User;
// ��¼Ϊ1��
// ����ע��Ϊ2��
// ��������ע��Ϊ3��
// ��������ע����4��
// ֧������Ϊ5��
// ����Ϊ6��
// ���Իظ�Ϊ7��
// �����������͵����Է��������������
// 8��
// �û��ǳ�Ϊ9��
// ��ɫ��¼Ϊ11��
// ������ɫΪ12��
// ��ɫ�˳�Ϊ13��
// ��ɫʵʱ����Ϊ14��
// ֧����ֵΪ15��

enum	MSG2QQMSGDEF
{
	MQ_MSG_START_GROUPBUYING_AUTO = 100000,
	MQ_MSG_START_GROUPBUYING_ACC  = 100001,
	MQ_MSG_BACK_HOME			  = 100002,
};

enum MSG2QQType
{
	MQ_USER_LOGIN				= 1,
	MQ_REGISTER_SELF			= 2,
	MQ_REGISTER_INVITE			= 3,
	MQ_REGISTER_OTHER			= 4,
	MQ_CONSUME					= 5,
	MQ_LEAVE_MESSAGE			= 6,
	MQ_LEAVE_MESSAGE_BACK		= 7,
	MQ_LEAVE_OTHERS				= 8,
	MQ_USER_LOGOUT				= 9,
	MQ_PLAYER_LOGIN				= 11,
	MQ_CREATE_PLAYER			= 12,
	MQ_PLAYER_LOGOUT			= 13,
	MQ_PLAYER_ONLINE			= 14,
	MQ_FEE						= 15,

    MQ_Regist					= 100,
    MQ_Logon					= 101,
    MQ_Logout					= 102,
    MQ_Back2Home				= 103,
    MQ_VisitFrd					= 104,
    MQ_WebBuy					= 105,
	MQ_GUEST					= 106,			//����ӭ��
	MQ_SPECIAL_GUEST			= 107,			//���
	MQ_BOSS						= 108,			//ˢ��BOSS
	MQ_TASK						= 109,			//����
	MQ_BUY_GOODS				= 110,			//����
	MQ_PROCESS_GOODS			= 111,			//�ϻ�
	MQ_SUB_GOODS				= 112,			//����
// 
// 	���ܲ���
// 	���ܵ���
// 	���
// 	װ��
	MQ_BUY_SHELF				= 113,
	MQ_BUY_PROP					= 114,
	MQ_BUY_GIFT					= 115,
	MQ_BUY_DECORATER			= 116,

} ;

class CMsg2QQ
{
public:

    enum OPTTYPE
    {
        OT_PAY = 1,
        OT_LEAVE_MESSAGE = 2,
        OT_WRITE         = 3,
        OT_READ          = 4,
		OT_OTHER		 = 5,
     } ;
	enum LOGIN_TYPE
	{
		LG_QZONE = 1,
		LG_PENGYOU = 2,
		LG_WEIBO = 3,
		LG_Q_PLUS = 4,
		LG_CAIFUTONG= 5,
		LG_QQ_GAME=10,
	};

	enum DECORATER_TYPE
	{
		DEC_WALL = 1,
		DEC_FLOOR = 2,
		DEC_ITEM_ON_WALL = 3,
		DEC_ITEM_ON_MAP = 4,
	};
public:
    CMsg2QQ();
    ~CMsg2QQ();
    static CMsg2QQ* GetInstance();

    void TellMsg_deleted_function(MSG2QQType emType, User* pUser, int nGoldChg, int nExpChg, unsigned int uFrdPlatformID);
    void TellMsg(MSG2QQType emType, User* pUser, int nGoldChg, int nExpChg, unsigned int uFrdPlatformID);
	void TellMsgEx(MSG2QQType action_id,string action_from,OPTTYPE op_type,User * user,int64 to_uid,string to_open_id,int gold_change,int exp_change,int item_id,int item_type,int item_num,int qq_change = 0);
	void TellMsgLogin0n(User * pUser,string action_from);
	void TellMsgLoginOut(User * pUser);
	void TellMsgRegister(MSG2QQType action_id,User * pUser,string action_from);
	void TellMsgWebMore(MSG2QQType action_id,User * pUser,int item_id,int item_type,int item_num,int qq_change);
	void TellMsgVisitFrd(User * pUser,int64 toUid,string toOpenID);
	void TellMsgVisitBack(User * pUser,int64 toUid,string toOpenID);
	void TellMsgWebBuy(User * pUser,int item_id,int item_type,int item_num,int qq_change);
	void TellMsgSimple(MSG2QQType action_id,User * pUser);
	void TellMsgMore(MSG2QQType action_id,User * pUser,int nGoldChg,int nExpChg);
    void TellMsgProto(User* user, int protonum, bool succ,const string protoname);
	void TellMsgFriendOPRP(User * user,const string friend_openid,bool succ, int protonum,const string protoname);
//	void TellMsg(MSG2QQType emType,User* pUser,int nGoldChg,int nExpChg,unsigned int uFrdPlatformID);
    void SetIP(const std::string& ip);
	void SetEnable(bool enable){m_enable = enable;}
	void SetSafeDataToQQMax(int cnt){m_nSafetoQQMax = cnt;}
	void SetID(int id){m_id = id;}
	unsigned int	 ConvertUiSource(User *pUser);
private:
    log4cxx::LoggerPtr logger_;
    TMsgLog m_msg;
    unsigned int m_uiServerIp;
	bool	m_enable;
	int		m_nSafetoQQMax;
	int		m_id;
	string	m_SvrIP;
} ;

class CMsg2QQDB
{
public:
    CMsg2QQDB();
    ~CMsg2QQDB();
    static CMsg2QQDB* GetInstance();

    void TellSaveDB(int nUseTime, const char* szIP1, const char* szIP2, int nPort2);
    void TellLoadDB(int nUseTime, const char* szIP1, const char* szIP2, int nPort2);
	void SetEnable(bool enable){m_enable = enable;}
private:
    log4cxx::LoggerPtr logger_;
    TMsgLog m_msg;
	bool	m_enable;
#ifndef _WIN32
#ifdef _NEWLOG2QQ

    DataCollector::CLogger QQLogger;
public:
    DataCollector::CLogger& GetQQLogger();
#endif

#endif
} ;
