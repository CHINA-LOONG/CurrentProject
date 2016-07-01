#define _CRT_SECURE_NO_WARNINGS
#include "logic/dbinterface.pb.h"
#include "GameDataSaver.h"
#include <iomanip>
#include "GameDataHandler.h"
#include <sys/types.h>
#include "common/Clock.h"
#include "common/distribution.h"
#include "common/Msg2QQ.h"
#include "MemCacheServerHandler.h"
#ifndef _WIN32
#include <unistd.h>
#include <sys/ipc.h>
#include <sys/sem.h>
#else
#include <io.h>
#endif
#include "logic/GameConfig.h"
#include "stdio.h"
extern ServerConfig serverConfig;

GameDataSaver::GameDataSaver(int nid)
{
    nid_ = nid;
    db_num = serverConfig.dbNum();
    sprintf(allocate_file, "allocate_g%d.cfg", nid_);
    sprintf(allocate_dbstr, "ALLOCATE_G%05d", nid_);
    logger_ = log4cxx::Logger::getLogger("GameDataSaver");
    alloc_rdb = NULL;
    log_time = 0;
    count_dirty_ = 0;

    initDatabase();
	
    pthread_mutex_init(&load_user_mutex_, NULL);
    pthread_mutex_init(&save_user_mutex_, NULL);
    pthread_mutex_init(&load_map_mutex_, NULL);
    pthread_mutex_init(&save_map_mutex_, NULL);

    b_serving = true;
    m_bHotCacheEnable = GameConfig::GetInstance()->m_bHotCacheEnableWrite || GameConfig::GetInstance()->m_bHotCacheEnableLoad;
    m_bHotCacheEnableLoad = GameConfig::GetInstance()->m_bHotCacheEnableLoad;
#ifdef WIN32
    if (_access(HotCacheErrFile, 0) != 0)
#else
    if (access(HotCacheErrFile, 0) == 0)
#endif
    {
        m_bHotCacheEnable = false;
        m_bHotCacheEnableLoad = false;
        printf("HotCache disabled because err file %s exist!\n", HotCacheErrFile);
    }

    initThreads();
    printf("GameDataSaver %d started![with config %d,%d]\n", nid, m_bHotCacheEnable, m_bHotCacheEnableLoad);


}

GameDataSaver::~GameDataSaver()
{
    termThreads();
}

void GameDataSaver::initThreads()
{
    int ret = 0;
    ret = pthread_create(&th_user_load, NULL, GameDataSaver::loadUserThread, (void *) this);
    if (ret != 0)
    {
        LOG4CXX_ERROR(logger_, "ERROR creating load data thread");
    }
    ret = pthread_create(&th_user_save, NULL, GameDataSaver::saveUserThread, (void *) this);
    if (ret != 0)
    {
        LOG4CXX_ERROR(logger_, "ERROR creating save data thread");
    }

    ret = pthread_create(&th_map_load, NULL, GameDataSaver::loadMapThread, (void *) this);
    if (ret != 0)
    {
        LOG4CXX_ERROR(logger_, "ERROR creating load map thread");
    }

    ret = pthread_create(&th_map_save, NULL, GameDataSaver::saveMapThread, (void *) this);
    if (ret != 0)
    {
        LOG4CXX_ERROR(logger_, "ERROR creating save thread");
    }
}

void GameDataSaver::termThreads()
{
    b_serving = false;
    pthread_join(th_user_load, NULL);
    pthread_join(th_map_load, NULL);
    pthread_join(th_map_save, NULL);
    pthread_join(th_user_save, NULL);

    LOG4CXX_ERROR(logger_, "Threads terminated. Quit.");
    termDatabase();
}

void GameDataSaver::initDatabase()
{
    void * tmp = NULL;
    for (int i = 0; i < db_num; i ++)
    {
        db_user_load.insert(make_pair(i + 1, tmp));
        db_user_save.insert(make_pair(i + 1, tmp));
        db_plat_load.insert(make_pair(i + 1, tmp));
        db_plat_save.insert(make_pair(i + 1, tmp));

        db_user_load3.insert(make_pair(i + 1, tmp));

        db_plat_load4.insert(make_pair(i + 1, tmp));
        db_user_load4.insert(make_pair(i + 1, tmp));
    }
}

void GameDataSaver::termDatabase()
{
    for (int i = 0; i < db_num; i ++)
    {
        TCRDB* rdb = (TCRDB*) db_user_load[i];
        if (rdb != NULL)
        {
            tcrdbclose(rdb);
            tcrdbdel(rdb);
            db_user_load[i] = NULL;
        }

        rdb = (TCRDB*) db_user_load3[i];
        if (rdb != NULL)
        {
            tcrdbclose(rdb);
            tcrdbdel(rdb);
            db_user_load3[i] = NULL;
        }

        rdb = (TCRDB*) db_user_load4[i];
        if (rdb != NULL)
        {
            tcrdbclose(rdb);
            tcrdbdel(rdb);
            db_user_load4[i] = NULL;
        }


        rdb = (TCRDB*) db_user_save[i];
        if (rdb != NULL)
        {
            tcrdbclose(rdb);
            tcrdbdel(rdb);
            db_user_save[i] = NULL;
        }

        rdb = (TCRDB*) db_plat_load[i];
        if (rdb != NULL)
        {
            tcrdbclose(rdb);
            tcrdbdel(rdb);
            db_plat_load[i] = NULL;
        }

        rdb = (TCRDB*) db_plat_load4[i];
        if (rdb != NULL)
        {
            tcrdbclose(rdb);
            tcrdbdel(rdb);
            db_plat_load4[i] = NULL;
        }

        rdb = (TCRDB*) db_plat_save[i];
        if (rdb != NULL)
        {
            tcrdbclose(rdb);
            tcrdbdel(rdb);
            db_plat_save[i] = NULL;
        }
    }
    db_user_load.clear();
    db_user_load3.clear();
    db_user_load4.clear();
    db_user_save.clear();
    db_plat_load.clear();
    db_plat_load4.clear();
    db_plat_save.clear();

    if (alloc_rdb != NULL)
    {
        tcrdbclose(alloc_rdb);
        tcrdbdel(alloc_rdb);
        alloc_rdb = NULL;
    }
}

TCRDB* GameDataSaver::getAllocateDb()
{
    TCRDB* rdb = tcrdbnew();
    if (!tcrdbopen(rdb, serverConfig.dbUserAddr1(1).c_str(), serverConfig.dbUserPort1(1)))
    {
        LOG4CXX_ERROR(logger_, "open db[" << serverConfig.dbUserAddr1(1)
                << ":" << serverConfig.dbUserPort1(1)
                << "] error: " << tcrdberrmsg(tcrdbecode(rdb)));
        tcrdbdel(rdb);
        rdb = NULL;
    }

    if (rdb == NULL )
    {
        rdb = tcrdbnew();
        if (!tcrdbopen(rdb, serverConfig.dbUserAddr2(1).c_str(), serverConfig.dbUserPort2(1)))
        {
            LOG4CXX_ERROR(logger_, "open db[" << serverConfig.dbUserAddr2(1)
                    << ":" << serverConfig.dbUserPort2(1)
                    << "] error: " << tcrdberrmsg(tcrdbecode(rdb)));
            tcrdbdel(rdb);
            rdb = NULL;
        }
    }
    return rdb;
}

void GameDataSaver::loadAllocateSetting(int64& uid, int64& iid, int64& eid)
{
    if (alloc_rdb == NULL)
    {
        alloc_rdb = getAllocateDb();
    }
    printf("Loaded Allocate Settings 1 \n");

    char* buffer = NULL;
    if (alloc_rdb != NULL)
    {
        buffer = (char*) tcrdbget2(alloc_rdb, allocate_dbstr);
    }
    printf("Loaded Allocate Settings 2 \n");

    if (buffer != NULL)
    {
        sscanf(buffer, "%lld,%lld,%lld", &uid, &iid, &eid);
        free(buffer);
        printf("Loaded Allocate Settings 3 \n");

    }
    else
    {
        //if not in db, read info from file,this is use for change info from file to db,will delete later.
        FILE *fp = fopen(allocate_file, "r");
        if (NULL != fp)
        {
            char setting[256];
            fgets(setting, 256, fp);
            if (strlen(setting) >= 7)
            {
                sscanf(setting, "%lld,%lld,%lld", &uid, &iid, &eid);
            }
            fclose(fp);
            printf("Loaded Allocate Settings 4 \n");

        }
    }
    printf("Loaded Allocate Settings\n");
}

void GameDataSaver::saveAllocateSetting(int64& uid, int64& iid, int64& eid)
{
    if (alloc_rdb == NULL)
    {
        alloc_rdb = getAllocateDb();
    }
    if (alloc_rdb != NULL)
    {
        char setting[256];
        sprintf(setting, "%lld,%lld,%lld", uid, iid, eid);
        if (!tcrdbput2(alloc_rdb, allocate_dbstr, setting))
        {
            LOG4CXX_ERROR(logger_, "Put allocate ERROR : " << tcrdberrmsg(tcrdbecode(alloc_rdb)));
            tcrdbclose(alloc_rdb);
            tcrdbdel(alloc_rdb);
            alloc_rdb = NULL;
        }
    }

    FILE *fp = fopen(allocate_file, "w");
    if (NULL != fp)
    {
        fprintf(fp, "%lld,%lld,%lld", uid, iid, eid);
        fclose(fp);
    }
}

void GameDataSaver::writeSaveListLength(int len)
{
    time_t now = time(NULL);
    if (now != log_time && now % 10 == 0)
    {
        log_time = now;
        char logfile[256];
        sprintf(logfile, "savelist%d.log", nid_);
        FILE *fp = fopen(logfile, "a");
        if (NULL != fp)
        {
            struct tm *tmnow = NULL;
            char szTime[128];
            tmnow = localtime(&now);
            strftime(szTime, 128 - 1, "%Y-%m-%d %H:%M:%S", tmnow);
            fprintf(fp, "%s - %d (%d)\n", szTime, len, count_dirty_);
            fclose(fp);
        }
    }
}

void GameDataSaver::writeSaveError(const char *strError)
{
    time_t now = time(NULL);
    char logfile[256];
    sprintf(logfile, "saveerror%d.log", nid_);
    FILE *fp = fopen(logfile, "a");
    if (NULL != fp)
    {
        struct tm *tmnow = NULL;
        char szTime[128];
        tmnow = localtime(&now);
        strftime(szTime, 128 - 1, "%Y-%m-%d %H:%M:%S", tmnow);
        fprintf(fp, "%s - change db error:%s\n", szTime, strError);
        fclose(fp);
    }
}

int GameDataSaver::safeSaveAllUser(time_t revision, GameDataHandler * const dh, vector<int64>& lstSaveID, bool force/* =false */)
{
    // user data
    pthread_mutex_lock(&save_user_mutex_);
    map<int64, User*> &users = dh->dirty_users_;
    map<int64, User*>::iterator iter = users.begin();
    int list_len = save_user_list_.size();
    int save_interval = GameConfig::GetInstance()->GetSaveInterval();
    if (save_interval <= 0)
    {
        save_interval = 5;
    }
#ifdef WIN32
    save_interval = 0;
#endif
    if (list_len > 500)
    {
        save_interval += (list_len - 500) / 100;
        //        if (save_interval > 10)
        //        {
        //            save_interval = 10;
        //        }
    }
    lstSaveID.clear();
    while (iter != users.end())
    {
        map<int64, User*>::iterator oiter = iter;
        iter ++;
        User *user = oiter->second;
        if (force || user->revision() < revision - save_interval * 60 * 1000)
        {
            pushSaveUser(dh, user->id(), user);
            lstSaveID.push_back(user->id());
            users.erase(oiter);
        }
    }
    count_dirty_ = users.size();
    pthread_mutex_unlock(&save_user_mutex_);
    return 0;
}

int GameDataSaver::safeSaveUser(GameDataHandler * const dh, User* user)
{
    if (user != NULL)
    {
        pthread_mutex_lock(&save_user_mutex_);
        pushSaveUser(dh, user->id(), user);
        pthread_mutex_unlock(&save_user_mutex_);
        LOG4CXX_DEBUG(logger_, "Save user data:uid=" << user->id());
    }
    return 0;
}

int GameDataSaver::safeLoadUser(GameDataHandler * const dh, int64& uid)
{
    pthread_mutex_lock(&load_user_mutex_);
    pushLoadUser(dh, uid);
    pthread_mutex_unlock(&load_user_mutex_);
    LOG4CXX_DEBUG(logger_, "Load user data " << uid);
    return 0;
}

int GameDataSaver::safeSaveAllMap(GameDataHandler * const dh)
{
    // map data
    pthread_mutex_lock(&save_map_mutex_);
    hash_map<string, int64> &list = dh->platid_uid_;
    for (hash_map<string, int64>::iterator iter = list.begin(); iter != list.end(); iter++)
    {
        const string& pid = iter->first;
        int64 uid = iter->second;
        if (uid > 0)
        {
            pushSaveMap(dh, pid, uid);
        }
    }
    pthread_mutex_unlock(&save_map_mutex_);
    LOG4CXX_DEBUG(logger_, "All PlatformID->UID Map saved.");
    return 0;
}

int GameDataSaver::safeSaveMap(GameDataHandler * const dh, const string& pid, int64& uid)
{
    if (uid > 0)
    {
        pthread_mutex_lock(&save_map_mutex_);
        pushSaveMap(dh, pid, uid);
        pthread_mutex_unlock(&save_map_mutex_);
        LOG4CXX_DEBUG(logger_, "Save platid map: platid=" << pid << "->uid=" << uid);
    }
    return 0;
}

int GameDataSaver::safeLoadMap(GameDataHandler * const dh, const string& pid)
{
    pthread_mutex_lock(&load_map_mutex_);
    pushLoadMap(dh, pid);
    pthread_mutex_unlock(&load_map_mutex_);
    LOG4CXX_DEBUG(logger_, "Load user map " << pid);
    return 0;
}

int GameDataSaver::pushSaveUser(GameDataHandler * const dh, int64 uid, User* user)
{
    string * data = new string;
    TEST[E_SAVER_STRING]++;
    MakeUserString(dh, user, *data);


    list<UserItem>::iterator iter = save_user_list_.begin();
    while (iter != save_user_list_.end())
    {
        if (iter->uid == uid)
        {
            delete iter->data;
            TEST[E_SAVER_STRING]--;
            iter->data = data;
            return 0;
        }
        iter ++;
    }

    UserItem save_data;
    save_data.dh = dh;
    save_data.data = data;
    save_data.uid = uid;

    save_user_list_.push_front(save_data);
    //user->setRevision(dh->revision());
    return 0;
}

int GameDataSaver::pushLoadUser(GameDataHandler * const dh, int64& uid)
{
    UserItem load_data;
    load_data.dh = dh;
    load_data.data = NULL;
    load_data.uid = uid;

    load_user_list_.push_front(load_data);
    return 0;
}

int GameDataSaver::pushSaveMap(GameDataHandler * const dh, const string& platid,
        int64& uid)
{
    MapItem save_data;
    save_data.dh = dh;
    save_data.platid = platid;
    save_data.uid = uid;

    save_map_list_.push_front(save_data);
    return 0;
}

int GameDataSaver::pushLoadMap(GameDataHandler * const dh, const string& platid)
{
    MapItem load_data;
    load_data.dh = dh;
    load_data.platid = platid;
    load_data.uid = 0;

    load_map_list_.push_front(load_data);
    return 0;
}

TCRDB* GameDataSaver::getDb(const string& addr, int port)
{
    TCRDB* rdb = tcrdbnew();
    if (!tcrdbopen(rdb, addr.c_str(), port))
    {
        LOG4CXX_ERROR(logger_, "open db[" << addr << ":" << port
                << "] error: " << tcrdberrmsg(tcrdbecode(rdb)));
        tcrdbdel(rdb);
        rdb = NULL;
    }

    return rdb;
}

TCRDB* GameDataSaver::getUserLoadDb(int dbid)
{
    TCRDB *rdb = (TCRDB *) db_user_load[dbid];
    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbUserAddr1(dbid), serverConfig.dbUserPort1(dbid));
    }

    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbUserAddr2(dbid), serverConfig.dbUserPort2(dbid));
    }

    if (rdb != NULL)
    {
        db_user_load[dbid] = rdb;
    }



    return rdb;
}

TCRDB* GameDataSaver::getUserLoadDb3(int dbid)
{
    TCRDB *rdb3 = (TCRDB *) db_user_load3[dbid];
    if (rdb3 == NULL)
    {
        rdb3 = getDb(serverConfig.dbUserAddr3(dbid), serverConfig.dbUserPort3(dbid));
    }
    db_user_load3[dbid] = rdb3;

    return rdb3;
}

TCRDB* GameDataSaver::getUserLoadDb4(int dbid)
{
    TCRDB *rdb4 = (TCRDB *) db_user_load4[dbid];
    if (rdb4 == NULL)
    {
        rdb4 = getDb(serverConfig.dbUserAddr4(dbid), serverConfig.dbUserPort4(dbid));
    }
    db_user_load4[dbid] = rdb4;

    return rdb4;
}

TCRDB* GameDataSaver::getUserSaveDb(int dbid)
{
    TCRDB *rdb = (TCRDB *) db_user_save[dbid];
    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbUserAddr1(dbid), serverConfig.dbUserPort1(dbid));
    }

    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbUserAddr2(dbid), serverConfig.dbUserPort2(dbid));
    }

    if (rdb != NULL)
    {
        db_user_save[dbid] = rdb;
    }
    return rdb;
}

TCRDB* GameDataSaver::getPlatLoadDb(int dbid)
{
    TCRDB *rdb = (TCRDB *) db_plat_load[dbid];
    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbPlatAddr1(dbid), serverConfig.dbPlatPort1(dbid));
    }

    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbPlatAddr2(dbid), serverConfig.dbPlatPort2(dbid));
    }

    if (rdb != NULL)
    {
        db_plat_load[dbid] = rdb;
    }
    return rdb;
}

TCRDB* GameDataSaver::getPlatLoadDb4(int dbid)
{
    TCRDB *rdb4 = (TCRDB *) db_plat_load4[dbid];
    if (rdb4 == NULL)
    {
        rdb4 = getDb(serverConfig.dbPlatAddr4(dbid), serverConfig.dbPlatPort4(dbid));
    }
    if (rdb4 != NULL)
    {
        db_plat_load4[dbid] = rdb4;
    }
    return rdb4;
}

TCRDB* GameDataSaver::getPlatSaveDb(int dbid)
{
    TCRDB *rdb = (TCRDB *) db_plat_save[dbid];
    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbPlatAddr1(dbid), serverConfig.dbPlatPort1(dbid));
    }

    if (rdb == NULL)
    {
        rdb = getDb(serverConfig.dbPlatAddr2(dbid), serverConfig.dbPlatPort2(dbid));
    }

    if (rdb != NULL)
    {
        db_plat_save[dbid] = rdb;
    }
    return rdb;
}

int GameDataSaver::saveUser(GameDataHandler * const dh, int64& uid, string& data)
{
    time_t ltStart = Clock::getCurrentSystemTime();
    int dbid = getUidHash(uid) % db_num + 1;
    TCRDB* rdb = getUserSaveDb(dbid);
    if (rdb == NULL)
    {
        return -1;
    }

    char suid[32];
    sprintf(suid, "%016llX", uid);
    const char* buf = data.c_str();
    int len = data.length();
    if (!tcrdbput(rdb, suid, 16, buf, len))
    {
        int ecode = tcrdbecode(rdb);
        LOG4CXX_ERROR(logger_, "Put user ERROR : " << tcrdberrmsg(ecode));
        writeSaveError(tcrdberrmsg(ecode));
        tcrdbclose(rdb);
        tcrdbdel(rdb);
        db_user_save[dbid] = NULL;
        return -1;
    }
    //标记是否存完
    //MemCacheServerHandler::SavePushRemoveQueue(uid);

    time_t ltEnd = Clock::getCurrentSystemTime();
    saveUserToDBHotCache(dh, uid, data);
    LOG4CXX_DEBUG(logger_, "Saved User: uid=" << uid);
    return 0;
}

char*  GameDataSaver::loadUserFromDB3( int64& uid, int &len )
{
    int dbid = getUidHash(uid) % db_num + 1;
    TCRDB* rdb3 = getUserLoadDb3(dbid);
    if (rdb3 == NULL)
    {
        return NULL;
    }
    char suid[32];
    sprintf(suid, "%016llX", uid);

    char* buffer = (char*) tcrdbget(rdb3, suid, 16, &len);
    if ( NULL == buffer )
    {
        int ecode = tcrdbecode(rdb3);
        if (ecode != TTENOREC)
        {
            tcrdbclose(rdb3);
            tcrdbdel(rdb3);
            db_user_load3[dbid] = NULL;
        }
    }
    return buffer;
}

char*  GameDataSaver::loadUserFromDB4( int64& uid, int &len )
{
    int dbid = getUidHash(uid) % db_num + 1;
    TCRDB* rdb4 = getUserLoadDb4(dbid);
    if (rdb4 == NULL)
    {
        return NULL;
    }
    char suid[32];
    sprintf(suid, "%016llX", uid);

    char* buffer = (char*) tcrdbget(rdb4, suid, 16, &len);
    if ( NULL == buffer )
    {
        int ecode = tcrdbecode(rdb4);
        if (ecode != TTENOREC)
        {
            tcrdbclose(rdb4);
            tcrdbdel(rdb4);
            db_user_load4[dbid] = NULL;
        }
    }
    return buffer;
}

int GameDataSaver::loadUser(GameDataHandler * const dh, int64& uid)
{
    int dbid = getUidHash(uid) % db_num + 1;
    TCRDB* rdb = getUserLoadDb(dbid);
    if (rdb == NULL)
    {
        return -1;
    }
    char suid[32];
    sprintf(suid, "%016llX", uid);

    User* user = NULL;
    int len;
    time_t ltStart = Clock::getCurrentSystemTime();
    char* buffer = (char*) loadUserFromDBHotCache(uid, len);
    //char* buffer = (char*) tcrdbget(rdb, suid, 16, &len);

    if (buffer == NULL)
    {
        buffer = (char*) tcrdbget(rdb, suid, 16, &len);
        int ecode = tcrdbecode(rdb);
        if (NULL == buffer)
        {
            if (ecode == TTENOREC)
            {
                dh->updateLoadUser(uid, user);
                return 1;
                //                tcrdbput(rdb, suid, 16, buffer, len);
                //                LOG4CXX_ERROR(logger_, "Recreate User: uid=" << uid);
            }
            else
            {
                LOG4CXX_ERROR(logger_, "Get user ERROR : " << tcrdberrmsg(ecode));
                tcrdbclose(rdb);
                tcrdbdel(rdb);
                db_user_load[dbid] = NULL;
                return -1;
            }
        }
    }
    LOG4CXX_DEBUG(logger_, "Loaded User: uid=" << uid);
    time_t ltEnd = Clock::getCurrentSystemTime();

    string data(buffer, len);
    dh->acquireDataLock();
    user = new User();
    TEST[E_USER]++;
    LoadUserFromString(dh, user, data);
    dh->releaseDataLock();
#ifdef WIN32
    uid = user->GetDbUser().id();
#endif
    dh->updateLoadUser(uid, user);
    free(buffer);
    return 0;
}

int GameDataSaver::saveMap(GameDataHandler * const dh, const string& pid, int64& uid)
{
    int dbid = getPlatidHash(pid) % db_num + 1;
    TCRDB* rdb = getPlatSaveDb(dbid);
    if (rdb == NULL)
    {
        return -1;
    }

    char suid[32];
    sprintf(suid, "%016llX", uid);
    if (!tcrdbput2(rdb, pid.c_str(), suid))
    {
        int ecode = tcrdbecode(rdb);
        LOG4CXX_ERROR(logger_, "Put map ERROR : " << tcrdberrmsg(ecode));
        tcrdbclose(rdb);
        tcrdbdel(rdb);
        db_plat_save[dbid] = NULL;
        return -1;
    }
    LOG4CXX_DEBUG(logger_, "Saved Map:platid=" << pid << "->uid=" << uid);
    return 0;
}

char* GameDataSaver::loadMapFromPlat4(const string& platid)
{
    int dbid = getPlatidHash(platid) % db_num + 1;
    TCRDB* rdb4 = getPlatLoadDb4(dbid);
    if (rdb4 == NULL)
    {
        return NULL;
    }

    int64 uid = 0;
    char* buffer = (char*) tcrdbget2(rdb4, platid.c_str());


    if (buffer == NULL)
    {
        int ecode = tcrdbecode(rdb4);
        if (ecode != TTENOREC)
        {

            tcrdbclose(rdb4);
            tcrdbdel(rdb4);
            db_plat_load4[dbid] = NULL;
        }
    }
    return buffer;
}

int GameDataSaver::loadMap(GameDataHandler * const dh, const string& platid)
{
    char* buffer4 = loadMapFromPlat4( platid);

    int dbid = getPlatidHash(platid) % db_num + 1;
    TCRDB* rdb = getPlatLoadDb(dbid);
    if (rdb == NULL)
    {
        return -1;
    }

    int64 uid = 0;
    int64 uid4 = 0;
    char* buffer = (char*) tcrdbget2(rdb, platid.c_str());

    if ( NULL != buffer4 )
    {
        sscanf(buffer4, "%016llX", &uid4);
        if ( NULL != buffer )
        {
            sscanf(buffer, "%016llX", &uid);
        }

        if ( NULL == buffer || uid != uid4 )
        {
            char suid[32];
            sprintf(suid, "%016llX", uid4);
            tcrdbput2(rdb, platid.c_str(), suid);
            LOG4CXX_ERROR(logger_, "Recreate PlatIDUserIDMap ERROR : " << platid << uid4);
        }

        dh->updateLoadPlatid(platid, uid4);
        LOG4CXX_DEBUG(logger_, "Loaded Map:platid=" << platid << "->uid=" << uid4);
        free(buffer4 );
        return 0;
    }

    if (buffer == NULL)
    {
        int ecode = tcrdbecode(rdb);
        if (ecode == TTENOREC)
        {
            dh->updateLoadPlatid(platid, uid);
            return 1;
        }
        LOG4CXX_ERROR(logger_, "Get map ERROR : " << tcrdberrmsg(ecode));
        tcrdbclose(rdb);
        tcrdbdel(rdb);
        db_plat_load[dbid] = NULL;
        return -1;
    }

    sscanf(buffer, "%016llX", &uid);
    dh->updateLoadPlatid(platid, uid);
    LOG4CXX_DEBUG(logger_, "Loaded Map:platid=" << platid << "->uid=" << uid);
    free(buffer);
    return 0;
}

void GameDataSaver::routineUser(DT_ActionType type)
{
    int ret = 0;
    pthread_mutex_t* mutex_ = (type == ACTION_USER_LOAD) ? &load_user_mutex_ : &save_user_mutex_;
    list<UserItem>* list = (type == ACTION_USER_LOAD) ? &load_user_list_ : &save_user_list_;

    while (true)
    {
        pthread_mutex_lock(mutex_);
        if (type == ACTION_USER_SAVE)
        {
            int listlen = list->size();
            writeSaveListLength(listlen);
        }
        if (list->empty())
        {
            pthread_mutex_unlock(mutex_);
            if (b_serving)
            {
                usleep(500);
                continue;
            }
            else
            {
                break;
            }
        }
        UserItem item;
        if (!list->empty())
        {
            item = list->back();
            list->pop_back();
        }
        pthread_mutex_unlock(mutex_);

        if (type == ACTION_USER_SAVE)
        {
            ret = saveUser(item.dh, item.uid, *item.data);
        }
        else
        {
            ret = loadUser(item.dh, item.uid);
        }
        if (ret >= 0)
        {
            if (item.data != NULL)
            {
                delete item.data;
                TEST[E_SAVER_STRING]--;
            }
        }
        else
        {
            // retry
            pthread_mutex_lock(mutex_);
            list->push_front(item);
            pthread_mutex_unlock(mutex_);
            usleep(500);
            continue;
        }
        usleep(500);
    }
}

void GameDataSaver::routineMap(DT_ActionType type)
{
    int ret = 0;
    pthread_mutex_t* mutex_ = (type == ACTION_MAP_LOAD) ? &load_map_mutex_ : &save_map_mutex_;
    list<MapItem>* list = (type == ACTION_MAP_LOAD) ? &load_map_list_ : &save_map_list_;

    while (true)
    {
        pthread_mutex_lock(mutex_);
        if (list->empty())
        {
            pthread_mutex_unlock(mutex_);
            if (b_serving)
            {
                usleep(500);
                continue;
            }
            else
            {
                break;
            }
        }

        MapItem item;
        if (!list->empty())
        {
            item = list->back();
            list->pop_back();
        }
        pthread_mutex_unlock(mutex_);

        if (type == ACTION_MAP_SAVE)
        {
            ret = saveMap(item.dh, item.platid, item.uid);
        }
        else
        {
            ret = loadMap(item.dh, item.platid);
        }
        if (ret < 0)
        {
            // retry
            pthread_mutex_lock(mutex_);
            list->push_front(item);
            pthread_mutex_unlock(mutex_);
            usleep(500);
            continue;
        }
        usleep(500);
    }
}

void* GameDataSaver::saveUserThread(void* arg)
{
    GameDataSaver* ds = static_cast<GameDataSaver*> (arg);
    ds->routineUser(ACTION_USER_SAVE);
    pthread_exit(NULL);
    return NULL;
}

void* GameDataSaver::loadUserThread(void* arg)
{
    GameDataSaver* ds = static_cast<GameDataSaver*> (arg);
    ds->routineUser(ACTION_USER_LOAD);
    pthread_exit(NULL);
    return NULL;
}

void* GameDataSaver::saveMapThread(void* arg)
{
    GameDataSaver* ds = static_cast<GameDataSaver*> (arg);
    ds->routineMap(ACTION_MAP_SAVE);
    pthread_exit(NULL);
    return NULL;
}

void* GameDataSaver::loadMapThread(void* arg)
{
    GameDataSaver* ds = static_cast<GameDataSaver*> (arg);
    ds->routineMap(ACTION_MAP_LOAD);
    pthread_exit(NULL);
    return NULL;
}

void GameDataSaver::MakeUserString(GameDataHandler *dh, User *pUser, string &oString )
{
    if (pUser == NULL)
    {
        return;
    }
    DB_User& dbUser = pUser->GetDbUser();
	pUser->Save();
    dbUser.SerializeToString( &oString );
}

void GameDataSaver::LoadUserFromString(GameDataHandler *dh, User *pUser, string &oString )
{
    if (pUser == NULL)
    {
        return;
    }

    DB_User dbUser;
    if (dbUser.ParseFromString( oString ))
    {
        pUser->SetDbUser(dbUser);
    }
    else
    {
        LOG4CXX_ERROR(logger_, "DBERROR!!!!! ParseFromString ERROR:" << pUser->id() << pUser->platform_id());
        pUser->SetDbUser(dbUser);
    }

}

int GameDataSaver::saveUserToDBHotCache(GameDataHandler * const dh, int64& uid, string& data)
{
    LOG4CXX_DEBUG(logger_, "try Save User 2 hotcache: uid=" << uid);
    if (!m_bHotCacheEnable)
    {
        return -1;
    }
    time_t ltStart = Clock::getCurrentSystemTime();
    int dbid = getUidHash(uid) % db_num + 1;
    TCRDB* rdbhc = getUserDbHotCache(dbid);
    char suid[32];
    sprintf(suid, "%016llX", uid);
    const char* buf = data.c_str();
    int len = data.length();
    if (rdbhc != NULL && !tcrdbput(rdbhc, suid, 16, buf, len))
    {
        int ecode = tcrdbecode(rdbhc);
        LOG4CXX_ERROR(logger_, "Put user to hotcache ERROR : " << tcrdberrmsg(ecode));
        writeSaveError(tcrdberrmsg(ecode));
        tcrdbclose(rdbhc);
        tcrdbdel(rdbhc);
        db_user_hotcache[dbid] = NULL;
        MarkHotCacheDisable(uid);
        return -1;
    }

    //标记是否存完


    time_t ltEnd = Clock::getCurrentSystemTime();

    LOG4CXX_DEBUG(logger_, "Saved User 2 hotcache: uid=" << uid);
    return 0;
}

char*  GameDataSaver::loadUserFromDBHotCache( int64& uid, int &len )
{
    int dbid = getUidHash(uid) % db_num + 1;
    TCRDB* rdbhc = getUserDbHotCache(dbid);
    if (rdbhc == NULL || !m_bHotCacheEnableLoad)
    {
        return NULL;
    }
    char suid[32];
    sprintf(suid, "%016llX", uid);

    char* buffer = (char*) tcrdbget(rdbhc, suid, 16, &len);
    if ( NULL == buffer )
    {
        int ecode = tcrdbecode(rdbhc);
        if (ecode != TTENOREC)
        {
            tcrdbclose(rdbhc);
            tcrdbdel(rdbhc);
            db_user_hotcache[dbid] = NULL;
        }
    }
    return buffer;
}

TCRDB* GameDataSaver::getUserDbHotCache(int dbid)
{
    if (!m_bHotCacheEnable)
    {
        return NULL;
    }
    TCRDB *rdb_hc = (TCRDB *) db_user_hotcache[dbid];
    if (rdb_hc == NULL)
    {
        std::string ip   = GameConfig::GetInstance()->m_xHotCacheIpList[dbid];
        int         port = GameConfig::GetInstance()->m_xHotCachePortList[dbid];
        rdb_hc = getDb(ip.c_str(), port);
    }
    db_user_hotcache[dbid] = rdb_hc;

    return rdb_hc;
}

void GameDataSaver::MarkHotCacheDisable(int64 uid)
{
    m_bHotCacheEnable = false;
    m_bHotCacheEnableLoad = false;
    FILE* pFile = fopen(HotCacheErrFile, "w");
    if (pFile != NULL)
    {
        fprintf(pFile, "%lld\n", uid);
        fclose(pFile);
    }
}


