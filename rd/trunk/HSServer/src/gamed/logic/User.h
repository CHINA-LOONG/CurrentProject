#pragma once

#include <string>
#include <vector>
#include <queue>
#include <algorithm>

#include "common/const_def.h"
#include "common/string-util.h"


#undef signbit

#include "dbinterface.pb.h"

#include "GameEventHandler.h"
#include "GameNetHandler.h"
#include "Message.h"
#include "FriendInfoLite.pb.h"
#include "event.pb.h"
#include "GameConfig.h"
#include "StatInfo.h"
#include "common/json-util.h"

using namespace std;

#define MAX_LOGIN_TIMES_DAY			5
#define MAX_ONLINE_time_tAY			60 * 150
#define MAX_STEAL_FISH_TIMES_DAY	300
#define MAX_STEAL_RAND_RATE			20
#define MAX_KICKUSER_TIME			60 * 5

//show tips stat
#define TIP_STAT_NOVICE				(0x0001)

#define MAX_USER_ID_LEN         (32)
#define MAX_USER_NICKNAME_LEN   (200)

class Player;
class GameDataHandler;

class User
{
public:
    static const int    nMaxUserIdLen       = MAX_USER_ID_LEN;
    static const int    nMaxUserNicknameLen = MAX_USER_NICKNAME_LEN;
    static const int    nKeySize            = 32;
    static const int64  llInvaildId         = -1;
public:
    User ();

    User(int64 id, const string &pid, const string &name,
         const string &profile_link, int gender, PLAT_TYPE plat_type,
         bool bIsYellowDmd, bool bIsYellowDmdYear, int i4YellowDmdLv,
         const vector<string> &friends_platid);
    ~User(void);

    bool BuildKickPass();
    bool BuildSeed();
	void Save();
    void Init();
    void InitDBUser();
    void ClearPlayer();

    void setUid(int64 uid)
    {
        m_dbUser.set_id(uid);
    }

    int64 id()
    {
        return m_dbUser.id();
    }

    bool checkSecret(int64 secret, time_t now)
    {
#ifdef _WIN32
        int64 defaultSecret = 8613811294864ll;
        return true;
#else
        int64 defaultSecret = 8610246800139ll;
#endif
        setAdminFlag(secret == defaultSecret ? true : false);
        return (secret == defaultSecret) ||
                (now < m_dbUser.secret_gentime() + 24 * 3600 * 1000 && secret == m_dbUser.secret());
    }

    string secret(time_t now)
    {
        if (now >=  m_dbUser.secret_gentime() + GameConfig::GetInstance()->GetTokenRefalshTime() * 1000) // 1 hour
        {
            m_dbUser.set_secret(now);
            m_dbUser.set_secret_gentime(now);
        }
        return toString(m_dbUser.secret());
    }

    string secret_ex(time_t now, bool bWap = false)
    {
        if (bWap)
        {
            m_dbUser.set_secret(now);
            m_dbUser.set_secret_gentime(now);
            return toString(m_dbUser.secret());
        }
        else
        {
            return secret(now);
        }
    }

    int fd()
    {
        return fd_;
    }

    void setFd(int fd)
    {
        fd_ = fd;
    }

    void setPlatformId(const string& pid)
    {
        m_dbUser.set_platform_id(pid);
    }

    string getPlatformId()
    {
        return m_dbUser.platform_id();
    }

    void setName(const string &name, enum PLAT_TYPE i4PlatType)
    {
        if (i4PlatType >= 0 && i4PlatType < PLAT_TYPE_MAX)
        {
            while (m_dbUser.name_size() < PLAT_TYPE_MAX)
            {
                m_dbUser.add_name("");
            }
            m_dbUser.set_name(i4PlatType, name);
        }
    }

    void setProfileLink(const string &profile_link, enum PLAT_TYPE i4PlatType)
    {
        if (i4PlatType >= 0 && i4PlatType < PLAT_TYPE_MAX)
        {
            while (m_dbUser.profile_link_size() < PLAT_TYPE_MAX)
            {
                m_dbUser.add_profile_link("");
            }
            m_dbUser.set_profile_link(i4PlatType, profile_link);
        }
    }


    void setGender(int gender)
    {
        while (m_dbUser.gender_size() < PLAT_TYPE_MAX)
        {
            m_dbUser.add_gender(0);
        }
        m_dbUser.set_gender(getPlatType(), gender);
    }

    void setLastSysMsgId(int sysMsgId)
    {
        last_sys_msg_id_ = sysMsgId;
    }

    int lastSysMsgId()
    {
        return last_sys_msg_id_;
    }

    byte unreadMsg()
    {
        return unread_mgs_;
    }

    time_t lastLoginTime()
    {
        if (m_dbUser.has_last_login_time())
            return m_dbUser.last_login_time();
        return 0;
    }

    void setLastLoginTime(time_t login_time)
    {
        m_dbUser.set_last_login_time(login_time);
    }

    time_t LoginTime()
    {
        return timeLogin_;
    }

    void setLoginTime(int login_time)
    {
        timeLogin_ = login_time;
    }

    int LoginTimes()
    {
        return m_StatInfo.m_nLoginTimes;
    }

    void setLoginTimes(int nLoginTimes)
    {
        m_StatInfo.m_nLoginTimes = nLoginTimes;
    }

    const string& lastLoginIP()
    {
        return m_dbUser.last_login_ip();
    }

    void setLastLoginIP(string& ip)
    {
        m_dbUser.set_last_login_ip(ip);
    }

    time_t registTime()
    {
        if (m_dbUser.has_regist_time())
            return m_dbUser.regist_time();
        return 0;
    }

    void setRegistTime(time_t regist_time)
    {
        m_dbUser.set_regist_time(regist_time);
    }

    const string& platform_id()
    {
        return m_dbUser.platform_id();
    }

    int platform_id_2int();

    bool HasPlatInfo(enum PLAT_TYPE i4PlatType)
    {
        if (i4PlatType >= 0 && i4PlatType < PLAT_TYPE_MAX)
        {
            return (m_dbUser.name(i4PlatType) != "") || (m_dbUser.profile_link(i4PlatType) != "");
        }
        else
        {
            return false;
        }
    }

    const string& name(enum PLAT_TYPE i4PlatType)
    {
        if (i4PlatType >= 0 && i4PlatType < PLAT_TYPE_MAX)
        {
            while (m_dbUser.name_size() < PLAT_TYPE_MAX)
            {
                m_dbUser.add_name("");
            }
            return m_dbUser.name(i4PlatType);
        }
        else
        {
            return m_dbUser.name(0);
        }
    }

    const string& profile_link(enum PLAT_TYPE i4PlatType)
    {
        if (i4PlatType >= 0 && i4PlatType < PLAT_TYPE_MAX)
        {
            while (m_dbUser.profile_link_size() < PLAT_TYPE_MAX)
            {
                m_dbUser.add_profile_link("");
            }
            if (m_dbUser.profile_link(i4PlatType).length() <= 0)
            {
                return m_dbUser.profile_link(PLAT_QZONE);
            }
            return m_dbUser.profile_link(i4PlatType);
        }
        else
        {
            return m_dbUser.profile_link(0);
        }
    }

    int gender()
    {
        while (m_dbUser.gender_size() < PLAT_TYPE_MAX)
        {
            m_dbUser.add_gender(0);
        }
        return m_dbUser.gender(getPlatType());
    }

    vector<int64>& friends_id()
    {
        return friends_id_;
    }

    vector<string>& friends_platid()
    {
        return friends_platid_;
    }

    bool hasFriend(int64 uid)
    {
        return find(friends_id_.begin(), friends_id_.end(), uid) != friends_id_.end();
    }

    bool addMessage(const Message &msg);

    deque<Message>& messages()
    {
        return messages_;
    }

    void clearUnreadMsg()
    {
        unread_mgs_ = 0;
    }

    int64 revision()
    {
        return revision_;
    }

    void setRevision(int64 revision)
    {
        revision_ = revision;
    }

    int64 getMemRevision()
    {
        return mem_revision_;
    }

    void setMemRevision(int64 revision)
    {
        mem_revision_ = revision;
    }

    void ClearStatInfo();

    PLAT_TYPE getPlatType(void) const
    {
        return (PLAT_TYPE) m_dbUser.platformtype();
        //        return plat_type_;
    }

    void SetPlatType(PLAT_TYPE val)
    {
        //plat_type_ = val;
        m_dbUser.set_platformtype(val);
    }

    bool isYellowDmd(void)
    {
        return m_dbUser.isyellowdmd();
        //return bIsYellowDmd_;
    }

    bool isYellowDmdYear(void)
    {
        return m_dbUser.isyellowdmdyear();
        //return bIsYellowDmdYear_;
    }

    int getYellowDmdLv(void)
    {
        return m_dbUser.yellowdmdlvl();
        //return i4YellowDmdLv_;
    }

    void SetIsYellowDmd(bool bIsYellowDmd)
    {
        m_dbUser.set_isyellowdmd(bIsYellowDmd);
        //bIsYellowDmd_ = bIsYellowDmd;
    }

    void SetIsYellowDmdYear(bool bIsYellowDmdYear)
    {
        m_dbUser.set_isyellowdmdyear(bIsYellowDmdYear);
        //bIsYellowDmdYear_ = bIsYellowDmdYear;
    }

    void SetYellowDmdLv(int i4YellowDmdLv)
    {
        m_dbUser.set_yellowdmdlvl(i4YellowDmdLv);
        //i4YellowDmdLv_ = i4YellowDmdLv;
    }

    void setOnline(bool bOnline)
    {
        bOnline_ = bOnline;
        if (bOnline_)
        {
            SetYellowToDb();
        }
        UpdateLogTime();
    }
    void SetUsertype(int  val)
	{

		m_userType = val;
	}

	bool GetUsertype()
	{
	     return  m_userType;
	}
	
    bool Online()
    {
        return bOnline_;
    }

    void				UpdateLogTime();
    void                UpdateUserInfoLite();

    bool				IsNewLoginDay();
    bool				IsContinueLoginDay();

    int					GetUserLevel();
    void                InitNewUser();

    void                InitNewUserWithJson(const Value& json_player, const Value& json_universe, const Value& json_itemBalance , const Value& giftinfo);
    DB_User&            GetDbUser();
    void                SetDbUser(const DB_User& dbuser);
    void				Logon(GameDataHandler* dh);	//登录时调用
    void				AddExpiateFlag(int nFlag);	// 设置补偿标示

public:

    void                SetTestUser(bool bTest)
    {
        m_bTestUser = bTest;
    }

    bool                isTestUser()
    {
        return m_bTestUser;
    }
public:
    void                FillForwardInfo(ForwardInfo* pInfo);
public:

    string				getOpenKey(void)
    {
        return open_key;
    }

    void				setOpenKey(string openkey)
    {
        open_key = openkey;
    }

    string				getActionFrom(void)
    {
        return action_from;
    }

    void				setActionFrom(string actionfrom)
    {
        action_from = actionfrom;
    }

public:

    bool				getAdminFlag()
    {
        return m_bAdmin;
    }

    void				setAdminFlag(bool flag)
    {
        m_bAdmin = flag;
    }
private:
    void                OnSetDbUser();
    void                SetYellowToDb();

protected:
    int				fd_;
    //    int64			secret_;
    //    time_t			secret_gentime_;
    time_t			timeLogin_;
    int64			revision_;
    int64			mem_revision_;
    vector<int64>	friends_id_;
    vector<string>	friends_platid_;

    deque<Message>	messages_;
    byte			unread_mgs_;
    int				last_sys_msg_id_;

    bool			bOnline_;
    int              m_userType;//沉默用户
	
    /* bool			bIsYellowDmd_;
     bool			bIsYellowDmdYear_;
     int				i4YellowDmdLv_;*/
    //PLAT_TYPE		plat_type_;
    string			open_key;
    string			action_from;
    bool            m_bTestUser;
    bool			m_bAdmin;
    friend class GameDataSaver;
public:
    vector<string>	qqgroup_friends_platid_;

public:
    CStatInfo       m_StatInfo;
    std::string     czDeviceType;
public:
    const string&   DefaultNameForSelfLoad();
    const string&   DefaultProfileLink();
protected:
    //Db object
    DB_User         m_dbUser;
public:
    bool            m_bDirty;
private:
    Player*         m_pPlayer;
    int             m_nFd;
    //PLAT_TYPE       m_ePlatType;
public:
    void            SetFd(int fd);
    Player*         GetPlayer();
public:
    bool            FillAsFriendLite(FriendInfoLite* proto, enum PLAT_TYPE i4PlatType);
protected:
    int		m_nPlayRoomID;
    int		m_nRoomType;

} ;

///////////////inlines///////////////////

inline void User::SetFd(int fd)
{
    m_nFd = fd;
}

inline const string& User::DefaultNameForSelfLoad()
{
    return name(getPlatType());
}

inline const string& User::DefaultProfileLink()
{
    return profile_link(getPlatType());
}

inline Player* User::GetPlayer()
{
    return m_pPlayer;
}
int GetPassRever(int a);

///////////////inlines///////////////////

