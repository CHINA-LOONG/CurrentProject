#pragma once

#include <list>
#include <algorithm>
#include <pthread.h>
#include "common/const_def.h"
#include "logic/User.h"
#include "config/ServerConfig.h"

#ifdef _WIN32
#include "common/Logger_win.h"
#include "common/tcrdb_win.h"
#else
#include <log4cxx/logger.h>
#include <tcrdb.h>
#endif
#define HotCacheErrFile "hc_err"

using namespace std;

class GameDataHandler;
class UserLogin_Req;
class Tank;
class Fish;
class DB_Tank;
class DB_fish;

extern ServerConfig serverConfig;

struct UserItem
{
    GameDataHandler* dh;
    int64 uid;
    string* data;
} ;

struct MapItem
{
    GameDataHandler* dh;
    string platid;
    int64 uid;
} ;

enum DT_ActionType
{
    ACTION_USER_LOAD = 0,
    ACTION_USER_SAVE = 1,
    ACTION_MAP_LOAD = 2,
    ACTION_MAP_SAVE = 3
} ;

class GameDataSaver
{
public:
    GameDataSaver(int nid);
    ~GameDataSaver();

    bool initMutex();
    bool termMutex();

    void initThreads();
    void termThreads();

    void initDatabase();
    void termDatabase();

    void loadAllocateSetting(int64& uid, int64& iid, int64& eid);
    void saveAllocateSetting(int64& uid, int64& iid, int64& eid);

    //return succ
    int safeSaveAllUser(time_t revision, GameDataHandler * const dh, vector<int64>& lstSaveID, bool force = false);
    int safeSaveUser(GameDataHandler * const dh, User* user);
    int safeLoadUser(GameDataHandler * const dh, int64& uid);
    int safeSaveAllMap(GameDataHandler * const dh);
    int safeSaveMap(GameDataHandler * const dh, const string& pid, int64& uid);
    int safeLoadMap(GameDataHandler * const dh, const string& pid);

    void routineUser(DT_ActionType type);
    void routineMap(DT_ActionType type);

protected:
    void MakeUserString(GameDataHandler *dh, User *pUser, string &oString );
    void LoadUserFromString(GameDataHandler *dh, User *pUser, string &iString);

    int saveUser(GameDataHandler * const dh, int64& uid, string& data);
    int loadUser(GameDataHandler * const dh, int64& uid);

    char*  loadUserFromDB3( int64& uid, int &len );
    char*  loadUserFromDB4( int64& uid, int &len );

    char*  loadUserFromDBHotCache(int64& uid, int &len);
    int    saveUserToDBHotCache(GameDataHandler * const dh, int64& uid, string& data);

    int saveMap(GameDataHandler * const dh, const string& pid, int64& uid);
    int loadMap(GameDataHandler * const dh, const string& pid);
    char* loadMapFromPlat4(const string& platid);

    int pushSaveUser(GameDataHandler * const dh, int64 uid, User* user);
    int pushLoadUser(GameDataHandler * const dh, int64& uid);
    int pushSaveMap(GameDataHandler * const dh, const string& pid, int64& uid);
    int pushLoadMap(GameDataHandler * const dh, const string& pid);

    static void* saveUserThread(void* arg);
    static void* loadUserThread(void* arg);
    static void* saveMapThread(void* arg);
    static void* loadMapThread(void* arg);

    void writeSaveListLength(int len);
    void writeSaveError(const char *strError);

    TCRDB* getDb(const string& addr, int port);
    TCRDB* getUserLoadDb(int dbid);

    TCRDB* getUserLoadDb3(int dbid);
    TCRDB* getUserLoadDb4(int dbid);

    TCRDB* getUserDbHotCache(int dbid);

    TCRDB* getUserSaveDb(int dbid);
    TCRDB* getPlatLoadDb(int dbid);
    TCRDB* getPlatLoadDb4(int dbid);
    TCRDB* getPlatSaveDb(int dbid);
    TCRDB* getAllocateDb();

    void   MarkHotCacheDisable(int64 uid = -1);
protected:
    bool b_serving;

    pthread_t th_user_load;
    pthread_t th_user_save;
    pthread_t th_map_load;
    pthread_t th_map_save;

    pthread_mutex_t load_user_mutex_;
    pthread_mutex_t save_user_mutex_;
    pthread_mutex_t load_map_mutex_;
    pthread_mutex_t save_map_mutex_;

    list<UserItem> save_user_list_;
    list<UserItem> load_user_list_;
    list<MapItem> save_map_list_;
    list<MapItem> load_map_list_;

    int db_num;
    map<int, void *> db_user_load;
    map<int, void *> db_user_save;
    map<int, void *> db_plat_load;
    map<int, void *> db_plat_save;

    map<int, void *> db_user_load3;

    map<int, void *> db_plat_load4;
    map<int, void *> db_user_load4;

    map<int, void *> db_user_hotcache;

    log4cxx::LoggerPtr logger_;
    int nid_;
    char allocate_file[256];
    char allocate_dbstr[256];
    TCRDB* alloc_rdb;
    time_t log_time;
    int count_dirty_;
    bool m_bHotCacheEnable;
    bool m_bHotCacheEnableLoad;
} ;
