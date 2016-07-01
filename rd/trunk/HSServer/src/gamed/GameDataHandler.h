#pragma once

#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif

#include <vector>
#include <map>
#include <list>
#include "logic/User.h"
#include "common/statistics.h"
#include "common/counter.h"
#include <pthread.h>
#ifndef WIN32
#include <ext/hash_map>
using namespace __gnu_cxx;
namespace __gnu_cxx
{

    template<>
    struct hash<long long>
    {

        size_t
                operator()(long long __x) const
        {
            return __x;
        }
    } ;

    template<>
    struct hash<std::string>
    {

        size_t
                operator()(std::string __s) const
        {
            return __stl_hash_string(__s.c_str());
        }
    } ;
}
#else
#include <hash_map>
using namespace stdext;
#endif
#ifndef WIN32
using namespace __gnu_cxx;
#endif 
using namespace std;
class Event;

enum LoadStatus
{
    LOAD_INVALID = 0,
    LOAD_MISS = 1,
    LOAD_WAITING = 2,
    LOAD_HANDLING = 3,
    LOAD_SUCCESS = 4,
    LOAD_EMPTY = 5,
    LOAD_BUSY = 6,
    LOAD_ERROR = 99
} ;

struct GameFriendInfo
{
    int64 timestamp;
    int64 fid;

    string jsonstr;

    void Clear(void)
    {
        timestamp = 0;
        fid = 0;
        jsonstr = "";
    }
} ;

class GameDataSaver;
class GameEventHandler;
class Clock;

#define MAX_LOAD_WAITING 10000
const float FROZEN_RATE = 0.5;

class GameDataHandler : public DataHandler
{
public:
    typedef hash_map<int64, User*> UserList;
public:
    GameDataHandler(int nid);
    virtual ~GameDataHandler(void);

    virtual void tick();
    virtual void quit();

    void loadAllocateSetting();
    void saveAllocateSetting();
    int64 allocateUserID(const string& platid);
    int64 allocateItemID(int num = 1);
    int64 allocateEmployeeID(int num = 1);

    void markUserDirty(User* user, bool urgent = false, bool savedb = true);

    void saveAllUserData(bool urgent = false);
    void saveUserData(User* user);
    void loadUserData(int64& uid);
    void updateLoadUser(int64& uid, User* user);
    void updateUserMap();

    void savePlatidMap(const string& pid, int64& uid);
    void loadPlatidMap(const string& pid);
    void updateLoadPlatid(const string& pid, int64& uid);
    void updatePlatidMap();
    void updateUser(GameEventHandler * eventHandler);

    void UpdateRemoveUser();
    void RemoveUser(User *pUser);

    void UpdateRemoveFriend();
    void RemoveFriend(GameFriendInfo * pInfo);

    bool isPlatidLocal(const string& pid);
    bool isUidLocal(int64 uid);
    LoadStatus getUserStatus(int64 uid, bool load = false);

    inline pthread_mutex_t* getDataMutex()
    {
        return &data_mutex_;
    }

    inline void acquireDataLock()
    {
        pthread_mutex_lock(&data_mutex_);
    }

    inline void releaseDataLock()
    {
        pthread_mutex_unlock(&data_mutex_);
    }

    inline pthread_mutex_t* getMapMutex()
    {
        return &map_mutex_;
    }

    inline void acquireMapLock()
    {
        pthread_mutex_lock(&map_mutex_);
    }

    inline void releaseMapLock()
    {
        pthread_mutex_unlock(&map_mutex_);
    }

    void updateUser(User* user, const string &name, const string &profile_link,
                    int gender, PLAT_TYPE plat_type, bool bIsYellowDmd, bool bIsYellowDmdYear,
                    int i4YellowDmdLv, const vector<string> &fpid, GameEventHandler* eh_, const string& openkey, int register_type, const string& action_from);
    User* createUser(const string& pid, const string &name, const string& profile_link,
                     int gender, PLAT_TYPE plat_type, bool bIsYellowDmd, bool bIsYellowDmdYear,
                     int i4YellowDmdLv, const vector<string> &fpid, GameEventHandler* eh_, const string& openkey, int register_type, const string& action_from, const string& device_from);
    User* newUser(int64 uid, const string& pid, const string &name,
                  const string &profile_link, int gender, PLAT_TYPE plat_type,
                  bool bIsYellowDmd, bool bIsYellowDmdYear, int i4YellowDmdLv,
                  const vector<string> &fpid, GameEventHandler* eh_, const string& device_from);

    // event data process routine
    int loadUser(int64 uid, bool self_load, bool &lottery, bool &loginPrize, bool& showInvite);
    // getters
    User* getUser(int64 uid, LoadStatus* status = NULL, bool load = true);
    User* getUser(const string& pid, LoadStatus* status = NULL, bool load = true);
    User* getUser(int64 uid, Event* e);
    User* getUser(const std::string& openod, Event* e);

    const hash_map<int64, User*>& getUsers()
    {
        return users_;
    }

    const map<int64, User*> & getDirtyUsers()
    {
        return dirty_users_;
    }
    // helper
    //void addMessage(User* user, Message msg);
    bool addExperience(User *user, int etype, bool add_gold);
    bool addExperience(User *user, int experience);
    void removeItem(int64 item_id);


    int loadRecord(int64 uid, int key, string &value);
    int saveRecord(int64 uid, int key, const string &value);

    inline string getSysMsg()
    {
        return sys_msg_;
    }

    inline int sysMsgId()
    {
        return sys_msg_id_;
    }

    inline void setSysMsgId(const int sys_msg_id)
    {
        sys_msg_id_ = sys_msg_id;
    }

    inline int GameId()
    {
        return nid_;
    }

    inline int64 getRemoveCnt(void)
    {
        return remove_cnt;
    }

    inline Statistics& getStatistics(void)
    {
        return stat_;
    }

    inline Counter& getCounter(void)
    {
        return counter_;
    }
    int addSysMsg(const string &sys_msg);
    int removeSysMsg();

    // return succ
    void clearAllUserExprienceCounter();
    int generateChildFishType(int ftype1, int ftype2);

    GameFriendInfo* getFriendInfo(const string& fpid, enum PLAT_TYPE i4PlatType);
    GameFriendInfo* getFriendInfo(const int64 uid, enum PLAT_TYPE i4PlatType);
    void setFriendInfo(const string& fpid, int64& fid, const string& friendinfo, enum PLAT_TYPE i4PlatType);

    int getGamedIdByUserId(int64 uid);
    int getGameIdfromPlatId(string const& platid);
    int FriendInfoLiteCacheCount();
    int64 FriendInfoLiteCacheSize();

    //共享内存相关函数
    bool PushUser2ShareMem(User* pUser);	//将用户信息保存到共享内存
    void CleanShareMemUser(vector<int64>& lstUserID);
    bool LoadUserFromShareMem(int64 nUserID);
    void LoadAllUserFromShareMem();

private:
    void mapUidToUser(int64& uid, User* user);
    void mapPlatidToUid(const string& pid, int64& uid);

    log4cxx::LoggerPtr logger_;
    log4cxx::LoggerPtr diamond_logger_;

    int64 next_user_id_;
    int64 next_item_id_;
    int64 next_employee_id_;

    string sys_msg_;
    string last_sys_msg_;
    int sys_msg_id_;
    int last_sys_msg_id_;

    UserList        users_;
    list<int64>	remove_list_;
    list<int64> remove_friend_list_;
    hash_map<string, int64> platid_uid_;
    map<int64, LoadStatus> load_list_;
    map<int64, User*> loaded_user_;
    map<string, int64> loaded_platid_;
    map<int64, User*> dirty_users_;
    hash_map<string, GameFriendInfo*> platid_friendinfo_[PLAT_TYPE_MAX];
    hash_map<int64, string> friendinfo_platid_uid_[PLAT_TYPE_MAX];

    GameDataSaver* ds_;
    int nid_;
    pthread_mutex_t data_mutex_;
    pthread_mutex_t map_mutex_;

    time_t	timeUpdate;
    time_t	timeRemoveUpdate;
    time_t	timeRemoveFriendUpdate;
    time_t	timeFirstInit;
    int64	remove_cnt;
    Statistics stat_;
    Counter counter_;
public:
    map<string, bool> login_platid_;
public:
    friend class GameDataSaver;

} ;
