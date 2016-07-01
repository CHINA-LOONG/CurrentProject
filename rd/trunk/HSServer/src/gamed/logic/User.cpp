#include "User.h"
#include "Player.h"
#include "common/SysLog.h"
#include "GameDataHandler.h"
#include "logic/Message.h"
#include "GameConfig.h"
#include "UserCtrl.h"
#include "common/Clock.h"
#include "common/json-util.h"
using namespace std;

User::User()
{
    m_pPlayer       = NULL;
    bOnline_		= false;
    fd_				= 0;
    m_bTestUser     = false;
    for (int i = 0; i < PLAT_TYPE_MAX; i++)
    {
        m_dbUser.add_name("");
        m_dbUser.add_profile_link("");
    }
    czDeviceType = "PC";
    Init();
}

User::User(int64 id, const string& pid, const string &name,
        const string &profile_link, int gender, PLAT_TYPE plat_type,
        bool bIsYellowDmd, bool bIsYellowDmdYear, int i4YellowDmdLv,
        const vector<string> &friends_platid)
{
    Init();
    m_dbUser.set_id(id);
    m_dbUser.set_platform_id(pid);
    m_dbUser.set_name(plat_type, name);
    m_dbUser.set_profile_link(plat_type, profile_link);
    setRegistTime(time(NULL));
    SetIsYellowDmd(bIsYellowDmd);
    SetIsYellowDmdYear(bIsYellowDmdYear);
    SetYellowDmdLv(i4YellowDmdLv);
    //m_ePlatType = plat_type;
    //plat_type_  = plat_type;
    setGender(gender);
}

User::~User(void)
{
    //	time_t time_first = Clock::getUTime();
    if (m_pPlayer)
    {
        delete m_pPlayer;
        TEST[E_PLAYER]--;
        m_pPlayer = NULL;
    }
    // 	time_t time_last = Clock::getUTime();
    // 	cout<<"free player:"<<time_last-time_first<<endl;
}

void
User::Init()
{
    fd_				= 0;
    //    secret_			= 0;
    //    secret_gentime_	= 0;
    timeLogin_		= 0;
    revision_		= 0;
    mem_revision_	= 0;
    friends_id_.clear();
    friends_platid_.clear();
    messages_.clear();
    unread_mgs_		= 0;
    last_sys_msg_id_ = 0 ;

    bOnline_		= false;

    /*bIsYellowDmd_	= false;
    bIsYellowDmdYear_ = false;
    i4YellowDmdLv_	= 0;*/
    //plat_type_ = PLAT_XIAOYOU;

    m_pPlayer       = NULL;
    m_nRoomType		= 0;
    m_bTestUser     = false;
    m_bAdmin		= false;
    InitDBUser();
}

void
User::InitDBUser()
{
    m_dbUser.Clear();
    m_dbUser.set_id(0);

    m_dbUser.set_last_login_ip("");
    m_dbUser.set_last_login_time(0);
    m_dbUser.set_platform_id("");
    m_dbUser.set_regist_time(0);

    for (int i = 0; i < PLAT_TYPE_MAX; i++)
    {
        m_dbUser.add_name("");
        m_dbUser.add_profile_link("");
    }
    OnSetDbUser();
}

bool
User::addMessage(const Message &msg)
{
    if (!messages_.empty() && messages_.front() == msg)
    {
        messages_.front().time_ = msg.time_;
    }
    else
    {
        static int nMaxFeedNum = 50;
        messages_.push_front(msg);
        while ((int) messages_.size() > nMaxFeedNum) messages_.pop_back();
        unread_mgs_ ++;
        if (unread_mgs_ > nMaxFeedNum)
        {
            unread_mgs_ = nMaxFeedNum;
        }
        return true;
    }
    return false;
}

int
User::platform_id_2int()
{
    return PlatId2Int(m_dbUser.platform_id());
}

void
User::ClearStatInfo()
{
    m_StatInfo.Clear();
}

void
User::OnSetDbUser()
{
    if (m_pPlayer != NULL)
    {
        delete m_pPlayer;
        TEST[E_PLAYER]--;
        m_pPlayer = NULL;
    }
    if (m_pPlayer == NULL)
    {
        m_pPlayer = new Player(this, m_dbUser.mutable_player());
        TEST[E_PLAYER]++;
    }
}

void
User::InitNewUser()
{

    m_dbUser.mutable_player()->Clear();
    OnSetDbUser();
    m_dbUser.set_last_login_ip("0.0.0.0");
    m_dbUser.mutable_player()->set_inited(true);
}

DB_User&
User::GetDbUser()
{
    return m_dbUser;
}

int
User::GetUserLevel()
{
   return 0;
}

void
User::SetDbUser(const DB_User& dbuser)
{

    m_dbUser.Clear();
    m_dbUser.CopyFrom(dbuser);
    OnSetDbUser();
}

void
User::AddExpiateFlag(int nFlag)
{
    /*int nOldFlag = 0;
    if (m_dbUser.has_expiateflag())
    {
        nOldFlag = m_dbUser.expiateflag();
    }
    m_dbUser.set_expiateflag(nOldFlag | nFlag);*/
}

void
User::Logon(GameDataHandler* dh)
{

}

void
User::UpdateLogTime()
{

}

void
User::UpdateUserInfoLite()
{

}

bool
User::IsNewLoginDay()
{
    time_t lasttime = lastLoginTime();
    struct tm lastlogintime = *localtime(&lasttime);

    time_t now = time(NULL);
    struct tm nowtime = *localtime(&now);

    if (lastlogintime.tm_yday != nowtime.tm_yday)
    {
        return true;
    }

    return false;
}

bool
User::IsContinueLoginDay()
{
    time_t lasttime = lastLoginTime();
    struct tm lastlogintime = *localtime(&lasttime);

    time_t now = time(NULL);
    struct tm nowtime = *localtime(&now);

    if (lastlogintime.tm_year == nowtime.tm_year
            && lastlogintime.tm_yday + 1 == nowtime.tm_yday)
    {
        return true;
    }
    else if (lastlogintime.tm_year + 1 == nowtime.tm_year)
    {
        // 闰年
        if (lastlogintime.tm_year % 400 == 0
                || (lastlogintime.tm_year % 4 == 0 && lastlogintime.tm_year % 100 != 0 ))
        {
            if (lastlogintime.tm_yday == 365 && nowtime.tm_yday == 0)
            {
                return true;
            }
        }
        else
        {
            if (lastlogintime.tm_yday == 364 && nowtime.tm_yday == 0)
            {
                return true;
            }
        }
    }

    return false;
}

void
User::SetYellowToDb()
{
    m_dbUser.set_yellowdmdlvl( getYellowDmdLv());
    m_dbUser.set_isyellowdmdyear( isYellowDmdYear());
    m_dbUser.set_isyellowdmd(isYellowDmd());
}

void
User::FillForwardInfo(ForwardInfo* pInfo)
{
    if (pInfo == NULL)
    {
        return;
    }
    pInfo->set_actionsenderplattype((int) getPlatType());
    pInfo->set_sendername(DefaultNameForSelfLoad());
    pInfo->set_senderurl( profile_link(getPlatType()) );
    pInfo->set_platid(platform_id());
}

void
User::ClearPlayer()
{
    OnSetDbUser();
}

int
GetPassRever(int a)
{
    int    pass     = 95178624;
    int    passcpy  = pass;
    int    nPassLen = 0;
    double factor0  = 1.0864592;
    double factor1  = 0.9684627;
    double factor2  = 1;
    int    nVal     = (int) (a * factor0 * factor1 + factor2 ) >> 2 ;
    int    nValCpy  = nVal;
    int    nLen     = 0;
    int    nResult  = 0;
    char   strPass[32];
    char   strVal[32];

    for (int i = 0; i < 32 && passcpy != 0; i++)
    {
        nPassLen++;
        strPass[i] = passcpy % 2;
        passcpy = passcpy >> 1;

    }

    for (int i = 0; i < 32 &&  nVal != 0; i++)
    {
        nLen++;
        strVal[i] = nVal % 2;
        nVal = nVal >> 1;
    }

    for (int i = 0; i < 32 && i < nLen; i++)
    {
        int c = strPass[nPassLen - (i % nPassLen) - 1];
        int v = strVal[nLen - i - 1];
        //int pv = v;

        if (!c)
        {
            v = (~v ) & 1;
        }

        nResult = nResult * 2 + v;
        //printf("%d, i [%d] pchar[%d] char[%d] , v[%d] val[%d] result[%d]\n",a,i,c,pv,v, (v<< (i)),nResult);
    }
    return nResult;
}

bool
User::BuildKickPass()
{

    m_dbUser.set_checkval_first( rand() % (32768 * 4) + 32768);
    m_dbUser.set_checkval_second( rand() % (32768 * 12) + 32768);
    m_dbUser.set_passval( GetPassRever(m_dbUser.checkval_first()) & GetPassRever(m_dbUser.checkval_second()));
    return true;
}

bool
User::BuildSeed()
{

    int a = rand() % (32768 * 4) + 32768;
    int b = rand() % (32768 * 12) + 32768;
    m_dbUser.mutable_player()->mutable_universeinfo()->set_checkval_first(a);
    m_dbUser.mutable_player()->mutable_universeinfo()->set_checkval_second(b);
    m_dbUser.set_passval( GetPassRever(m_dbUser.checkval_first()) & GetPassRever(m_dbUser.checkval_second()));
    return true;
}

/**
 * 通过json数据构造玩家数据
 * @param json_player 玩家基本信息
 * @param json_universe 玩家关卡信息
 * @param json_itemBalance 玩家背包信息
 * @param json_gift 玩家签到信息
 */
void
User::InitNewUserWithJson(const Value& json_player, const Value& json_universe, const Value& json_itemBalance, const Value& json_gift)
{
  
}

void
User::Save()
{
	m_pPlayer->save();
}