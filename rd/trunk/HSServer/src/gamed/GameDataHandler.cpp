#define _CRT_SECURE_NO_WARNINGS
#include "../event/DataHandler.h"
#include "GameDataHandler.h"
#include "GameDataSaver.h"
#include "GameEventHandler.h"
#include <algorithm>
#include <map>
#include <math.h>
#include "logic/dbinterface.pb.h"
#include "common/Clock.h"
#include "common/distribution.h"
#include "common/SysLog.h"
#include "common/Msg2QQ.h"
#include "logic/Player.h"
#include "logic/GameConfig.h"
#include "ShareMemory/ShareMemHandler.h"
using namespace std;

GameDataHandler::GameDataHandler(int nid) : DataHandler(nid)
{
    nid_ = nid;
    printf("GameDataHandler 0\n");

    ds_ = new GameDataSaver(nid);
    TEST[E_DATE_SAVER]++;
    printf("GameDataHandler 1\n");

    users_.clear();
    remove_list_.clear();
    remove_friend_list_.clear();
    revision_ = 1;
    printf("GameDataHandler 2\n");

    logger_ = log4cxx::Logger::getLogger("GameDataHandler");
    diamond_logger_ = log4cxx::Logger::getLogger("diamond");
    sys_msg_id_ = 0;
    printf("GameDataHandler 3\n");

    next_user_id_ = 1;
    next_item_id_ = 1;
    next_employee_id_ = 1;

    printf("GameDataHandler 4\n");

    sys_msg_ = "";
    sys_msg_id_ = 0;
    last_sys_msg_ = "";
    last_sys_msg_id_ = 0;
    printf("GameDataHandler 5\n");

    loadAllocateSetting();
    printf("GameDataHandler 6\n");

    platid_uid_.clear();
    printf("GameDataHandler 7\n");

    pthread_mutex_init(&data_mutex_, NULL);
    pthread_mutex_init(&map_mutex_, NULL);
    printf("GameDataHandler 8\n");

    timeUpdate = time(NULL);
    printf("GameDataHandler 9\n");

    timeRemoveUpdate = Clock::getCurrentSystemTime();
    printf("GameDataHandler 10\n");

    timeRemoveFriendUpdate = Clock::getCurrentSystemTime();

    timeFirstInit = timeRemoveUpdate;
    login_platid_.clear();
    remove_cnt = 0;

    printf("GameDataHnadler inited !\n");

    //取消启动时读取共享内存
    //LoadAllUserFromShareMem();
}

GameDataHandler::~GameDataHandler(void)
{
    hash_map<int64, User*>::iterator iter;
    for (iter = users_.begin(); iter != users_.end(); iter ++)
    {
        delete iter->second;
        TEST[E_USER]--;
    }
    acquireDataLock();
    map<int64, User*>::iterator iter1;
    for (iter1 = loaded_user_.begin(); iter1 != loaded_user_.end(); iter1 ++)
    {
        delete iter1->second;
        TEST[E_USER]--;
    }
    releaseDataLock();
    remove_list_.clear();
    remove_friend_list_.clear();
    delete ds_;
    TEST[E_DATE_SAVER]--;
}

void
GameDataHandler::loadAllocateSetting()
{
    ds_->loadAllocateSetting(next_user_id_, next_item_id_, next_employee_id_);
    int step      = 10000;
    int step_item = 100000;
    if (next_user_id_ > 1)
    {
        next_user_id_ += step;
    }
    if (next_item_id_ > 1)
    {
        next_item_id_ += step_item;
    }
    if (next_employee_id_ > 1)
    {
        next_employee_id_ += step;
    }
    ds_->saveAllocateSetting(next_user_id_, next_item_id_, next_employee_id_);
}

void
GameDataHandler::saveAllocateSetting()
{
    ds_->saveAllocateSetting(next_user_id_, next_item_id_, next_employee_id_);
}

int64
GameDataHandler::allocateUserID(const string& platid)
{
    int hash = getPlatidHash(platid);
    int64 uid = next_user_id_;
    next_user_id_ ++;
    uid |= (((int64) (nid_ & 0xffff)) << 48) | (((int64) (hash & 0xffff)) << 32);
    //uid |= (((int64)(nid_ & 0xff)) << 24) | (((int64)(hash & 0xff)) << 16);
    return uid;
}

int64
GameDataHandler::allocateItemID(int num)
{
    int64 iid = next_item_id_;
    next_item_id_ += num;
    iid |= ((int64) (nid_ & 0xffff)) << 48;
    //iid |= ((int64)(nid_ &0x7f)) << 24;
    return iid;
}

int64
GameDataHandler::allocateEmployeeID(int num)
{
    int64 iid = next_employee_id_;
    next_employee_id_ += num;
    iid |= ((int64) (nid_ & 0xffff)) << 48;
    //iid |= ((int64)(nid_ &0x7f)) << 24;
    return iid;
}

void
GameDataHandler::mapUidToUser(int64& uid, User* user)
{
    if (user && user->GetPlayer()->IsEmptyUser())
    {
        int pt = (int) user->getPlatType();
        user->InitNewUser();
        user->SetPlatType((PLAT_TYPE) pt);
        map<string, bool>::iterator itor = login_platid_.find(user->platform_id());
        if (itor != login_platid_.end())
        {
            //SYS_LOG(user->id(), LT_Register, 0, 0, user->platform_id().c_str() << user->getPlatType() << user->czDeviceType.c_str());
            if (user->GetDbUser().user_from() == 0)
            {

            }
            else
            {

            }
            // printf("Reg %l\n", user->id());
        }
    }
    hash_map<int64, User*>::iterator iter = users_.find(uid);
    if (iter != users_.end())
    {
        if (user != NULL)
        {
            if (iter->second == NULL)
            {
                users_[uid] = user;
                remove_list_.push_back(uid);
            }
            else
            {
                delete user;
            }
        }
    }
    else
    {
        users_.insert(make_pair(uid, user));
        remove_list_.push_back(uid);
    }

    map<int64, LoadStatus>::iterator load_iter = load_list_.find(uid);
    if (load_iter != load_list_.end())
    {

        load_list_.erase(load_iter);
    }
}

void
GameDataHandler::mapPlatidToUid(const string& pid, int64& uid)
{
    hash_map<string, int64>::iterator iter = platid_uid_.find(pid);
    if (iter != platid_uid_.end())
    {
        platid_uid_[pid] = uid;
    }
    else
    {

        platid_uid_.insert(make_pair(pid, uid));
    }
}

void
GameDataHandler::tick()
{

    updateUserMap();
    updatePlatidMap();
    saveAllUserData();
    UpdateRemoveUser();
}

void
GameDataHandler::quit()
{
    saveAllUserData(true);
    ds_->termThreads();
    exit(0);
}

void
GameDataHandler::updateUserMap()
{
    acquireDataLock();
    map<int64, User*>::iterator iter = loaded_user_.begin();
    while (iter != loaded_user_.end())
    {
        map<int64, User*>::iterator oiter = iter;
        iter ++;
        int64 uid = oiter->first;
        User* user = oiter->second;
        loaded_user_.erase(oiter);

        if (user != NULL)
        {

            const string& pid = user->platform_id();
            mapPlatidToUid(pid, uid);
        }
        LOG4CXX_DEBUG(logger_, "Update user map uid=" << uid << " ,user=" << user);
        mapUidToUser(uid, user);
    }
    releaseDataLock();
}

void
GameDataHandler::updatePlatidMap()
{
    acquireMapLock();
    map<string, int64>::iterator iter = loaded_platid_.begin();
    while (iter != loaded_platid_.end())
    {

        map<string, int64>::iterator oiter = iter;
        iter ++;
        std::string pid = oiter->first;
        int64 uid = oiter->second;
        loaded_platid_.erase(oiter);

        LOG4CXX_DEBUG(logger_, "Update user map platid=" << pid << " ,uid=" << uid);
        mapPlatidToUid(pid, uid);
    }
    releaseMapLock();
}

void
GameDataHandler::updateLoadUser(int64& uid, User* user)
{
    acquireDataLock();
    map<int64, User*>::iterator iter = loaded_user_.find(uid);
    if (iter != loaded_user_.end())
    {
        if (user != NULL)
        {
            if (iter->second == NULL)
            {
                loaded_user_[uid] = user;
            }
            else
            {
                delete user;
            }
        }
    }
    else
    {

        loaded_user_.insert(make_pair(uid, user));
    }
    releaseDataLock();
}

void
GameDataHandler::updateLoadPlatid(const string& pid, int64& uid)
{
    acquireMapLock();
    map<string, int64>::iterator iter = loaded_platid_.find(pid);
    if (iter != loaded_platid_.end())
    {
        loaded_platid_[pid] = uid;
    }
    else
    {

        loaded_platid_.insert(make_pair(pid, uid));
    }
    releaseMapLock();
}

void
GameDataHandler::markUserDirty(User* user, bool urgent /* = false */, bool savedb /* = true */)
{
    if (user == NULL)
    {
        return;
    }
    //存入共享内存
    PushUser2ShareMem(user);
    int64 uid = user->id();
    time_t time_first = Clock::getUTime();

    time_t time_last = Clock::getUTime();
    stat_.capture("mem_update_time", (double) time_last - time_first);

    if (urgent == false && savedb == false)
    {
        return;
    }

    map <int64, User*>::iterator iter = dirty_users_.find(uid);
    if (iter == dirty_users_.end())
    {
        if (urgent)
        {
            user->setRevision(revision_ - 10 * 60 * 1000);
        }
        else
        {
            user->setRevision(revision_);
        }
        dirty_users_.insert(make_pair(uid, user));
        LOG4CXX_DEBUG(logger_, "Marking user dirty: uid=" << uid << ", revision" << revision_);
    }
    else
    {
        if (urgent)
        {

            user->setRevision(revision_ - 10 * 60 * 1000);
        }
    } 

}

bool
GameDataHandler::PushUser2ShareMem(User* pUser)
{
    if (pUser == NULL || !ShareMemHandler::GetInst()->CanUse())
        return false;
    DB_User & dbUser = pUser->GetDbUser();
    string oString;
    dbUser.SerializeToString( &oString );
    bool bRet = ShareMemHandler::GetInst()->Push(pUser->id(), oString.c_str(), oString.length());

    return bRet;
}

void
GameDataHandler::CleanShareMemUser(vector<int64>& lstUserID)
{
    if (!ShareMemHandler::GetInst()->CanUse())
        return;

    vector<int64>::iterator iter;
    for (iter = lstUserID.begin(); iter != lstUserID.end(); iter++)
    {

        int64 nUserID = *iter;
        ShareMemHandler::GetInst()->Clean(nUserID);
    }
}

bool
GameDataHandler::LoadUserFromShareMem(int64 nUserID)
{
    if (!ShareMemHandler::GetInst()->CanUse() || nUserID <= 0)
        return false;

    UINT nSize;
    //绝对不要free！！
    const char* pBuf = ShareMemHandler::GetInst()->Get(nUserID, nSize);
    if (pBuf)
    {
        User* pUser = new User();
        string oString(pBuf, nSize);

        DB_User dbUser;
        if (dbUser.ParseFromString( oString ))
        {
            pUser->SetDbUser(dbUser);
            updateLoadUser(nUserID, pUser);
            return true;
        }
        else
        {
            delete pUser;
            LOG4CXX_ERROR(logger_, "GameDataHandler::LoadAllUserFromShareMem() ERROR!!! ParseFromString UserID:" << nUserID);

            return false;
        }
    }
    return false;
}

void
GameDataHandler::LoadAllUserFromShareMem()
{
    if (!ShareMemHandler::GetInst()->CanUse())
        return;

    if (ShareMemHandler::GetInst()->CanUse())
    {
        vector<int64> lstErrorID;
        const map<int64, SMUnit*>& mapMem = ShareMemHandler::GetInst()->GetUsedUnitMap();
        map<int64, SMUnit*>::const_iterator iter;
        for (iter = mapMem.begin(); iter != mapMem.end(); iter++)
        {
            int64 nUserID = iter->first;
            SMUnit* pUnit = iter->second;
            if (pUnit->m_nUserID != nUserID || !pUnit->m_bUsed ||
                    pUnit->m_pBuf == NULL || pUnit->m_nSize <= 0 ||
                    pUnit->m_nUseSize <= 0 || pUnit->m_nUseSize > pUnit->m_nSize)
            {
                LOG4CXX_ERROR(logger_, "GameDataHandler::LoadAllUserFromShareMem() ERROR!!! UserID:" << nUserID);
                lstErrorID.push_back(nUserID);
                continue;
            }
            User* pUser = new User();
            string oString(pUnit->m_pBuf, pUnit->m_nUseSize);

            DB_User dbUser;
            if (dbUser.ParseFromString( oString ))
            {
                pUser->SetDbUser(dbUser);
                updateLoadUser(nUserID, pUser);
            }
            else
            {
                delete pUser;
                LOG4CXX_ERROR(logger_, "GameDataHandler::LoadAllUserFromShareMem() ERROR!!! ParseFromString UserID:" << nUserID);
            }
        }
        vector<int64>::iterator iterID;
        for (iterID = lstErrorID.begin(); iterID != lstErrorID.end(); iterID++)
        {

            int64 nID = *iterID;
            ShareMemHandler::GetInst()->Clean(nID);
        }
    }
}

void
GameDataHandler::saveAllUserData(bool urgent)
{
    if (!urgent && revision_ < timeUpdate + 10 * 1000)
    {

        return;
    }
    vector<int64> lstUserID;
    ds_->safeSaveAllUser(revision_, this, lstUserID, urgent);
    saveAllocateSetting();
    timeUpdate = revision_;

    CleanShareMemUser(lstUserID);
    //    if (urgent)
    //    {
    //        quit();
    //    }
}

void
GameDataHandler::saveUserData(User* user)
{

    ds_->safeSaveUser(this, user);
}

void
GameDataHandler::loadUserData(int64& uid)
{
    if (LoadUserFromShareMem(uid))
        return;

    ds_->safeLoadUser(this, uid);
}

void
GameDataHandler::savePlatidMap(const string& pid, int64& uid)
{

    ds_->safeSaveMap(this, pid, uid);
}

void
GameDataHandler::loadPlatidMap(const string& pid)
{

    ds_->safeLoadMap(this, pid);
}

User*
GameDataHandler::getUser(int64 uid, LoadStatus* status, bool load)
{
    //	time_t first_time = Clock::getUTime();
    LoadStatus tmp_status, *status_ = (status == NULL) ? &tmp_status : status;
    if (!isUidLocal(uid))
    {
        *status_ = LOAD_MISS;
        return NULL;
    }

    hash_map<int64, User*>::iterator iter = users_.find(uid);
    if (iter != users_.end())
    {
        if (iter->second == NULL)
        {
            *status_ = LOAD_EMPTY;
            return NULL;
        }
        *status_ = LOAD_SUCCESS;
        return iter->second;
    }

    if (!load)
    {
        *status_ = LOAD_EMPTY;
        return NULL;
    }

    map<int64, LoadStatus>::iterator load_iter = load_list_.find(uid);
    if (load_iter != load_list_.end())
    {
        *status_ = LOAD_WAITING;
        return NULL;
    }

    // max wait for 10000
    int load_count = load_list_.size();
    if (load_count >= MAX_LOAD_WAITING)
    {
        *status_ = LOAD_BUSY;
        return NULL;
    }

    load_list_.insert(make_pair(uid, LOAD_WAITING));
    *status_ = LOAD_WAITING;
    loadUserData(uid);
    //	time_t last_time = Clock::getUTime();
    //	stat_.capture("get_uid_user",(float)(last_time-first_time));

    return NULL;
}

User*
GameDataHandler::getUser(int64 uid, Event* e)
{
    LoadStatus state = LOAD_INVALID;
    User* pResult = getUser(uid, &state, true );
    if (pResult == NULL)
    {
        if (state == LOAD_WAITING)
        {

            GameEventHandler::GetInst()->postBackEvent(e);
            //eh_->postBackEvent(e);
        }
    }
    return pResult;
}

User*
GameDataHandler::getUser(const std::string& openid, Event* e)
{
    LoadStatus state = LOAD_INVALID;
    User* pResult = getUser(openid, &state, true );
    if (pResult == NULL)
    {
        if (state == LOAD_WAITING)
        {

            GameEventHandler::GetInst()->postBackEvent(e);
            //eh_->postBackEvent(e);
        }
    }
    return pResult;
}

bool
GameDataHandler::isPlatidLocal(const string& platid)
{
    int hash = getPlatidHash(platid);

    return hash % serverConfig.gamedNum() + 1 == nid_;
}

bool
GameDataHandler::isUidLocal(int64 uid)
{
    int hash = getUidHash(uid);

    return hash % serverConfig.gamedNum() + 1 == nid_;
}

LoadStatus
GameDataHandler::getUserStatus(int64 uid, bool load)
{
    LoadStatus status = LOAD_INVALID;

    User* user = getUser(uid, &status, load);
    // found
    if (user != NULL)
    {
        return LOAD_SUCCESS;
    }
    // remote user
    if (status == LOAD_MISS)
    {
        return LOAD_MISS;
    }
    // loading data
    if (status == LOAD_WAITING)
    {

        return LOAD_WAITING;
    }
    // not found or error
    return LOAD_ERROR;
}

User*
GameDataHandler::getUser(const string& pid, LoadStatus* status, bool load)
{
    //	time_t first_time = Clock::getUTime();
    LoadStatus tmp_status, *status_ = (status == NULL) ? &tmp_status : status;
    if (!isPlatidLocal(pid))
    {
        *status_ = LOAD_MISS;
        return NULL;
    }

    hash_map<string, int64>::iterator iter = platid_uid_.find(pid);
    if (iter != platid_uid_.end())
    {
        *status_ = LOAD_SUCCESS;
        int64 uid = iter->second;
        if (uid == 0)
        {
            *status_ = LOAD_EMPTY;
            return NULL;
        }
        if (uid == -1)
        {
            *status_ = LOAD_WAITING;
            return NULL;
        }
        return getUser(uid, status_, load);
    }
    int64 uid = -1ll;
    platid_uid_.insert(make_pair(pid, uid));
    loadPlatidMap(pid);
    *status_ = LOAD_WAITING;
    //	time_t last_time = Clock::getUTime();
    //	stat_.capture("get_pid_user",(float)(last_time-first_time));

    return NULL;
}

void
GameDataHandler::updateUser(User* user, const string &name,
        const string &profile_link, int gender,
        PLAT_TYPE plat_type, bool bIsYellowDmd,
        bool bIsYellowDmdYear, int i4YellowDmdLv,
        const vector<string> &fpid, GameEventHandler* eh_, const string &openkey, int register_type, const string& action_from)
{
    user->setName(name, plat_type);
    user->setOpenKey(openkey);
    user->setActionFrom(action_from);
    user->setProfileLink(profile_link, plat_type);
    user->setGender(gender);
    if (fpid.size() != 0)
    {
        user->friends_platid().clear();
    }
    else
    {
        user->friends_platid().clear();
    }
    // 注册时间为空才设置
    if (user->registTime() <= 0)
    {

        user->setRegistTime(time(NULL));
    }
    user->SetPlatType(plat_type);
    user->SetIsYellowDmd(bIsYellowDmd);
    user->SetIsYellowDmdYear(bIsYellowDmdYear);
    user->SetYellowDmdLv(i4YellowDmdLv);
    //    markUserDirty(user);
}

User*
GameDataHandler::createUser(const string& pid, const string &name,
        const string& profile_link, int gender,
        PLAT_TYPE plat_type, bool bIsYellowDmd,
        bool bIsYellowDmdYear, int i4YellowDmdLv,
        const vector<string> &fpid, GameEventHandler* eh_, const string &openkey,
        int register_type, const string& action_from, const string& device_from)
{
    int64 uid = allocateUserID(pid);
    User* u = newUser(uid, pid, name, profile_link, gender, plat_type,
            bIsYellowDmd, bIsYellowDmdYear, i4YellowDmdLv, fpid, eh_, device_from);

    u->setOpenKey(openkey);
    u->czDeviceType = device_from;
    mapPlatidToUid(pid, uid);
    savePlatidMap(pid, uid);

    //    markUserDirty(u);
    saveUserData(u);

    return u;
}

User*
GameDataHandler::newUser(int64 uid, const string& pid,
        const string &name, const string &profile_link,
        int gender, PLAT_TYPE plat_type,
        bool bIsYellowDmd, bool bIsYellowDmdYear,
        int i4YellowDmdLv, const vector<string> &fpid,
        GameEventHandler* eh_, const string& device_from)
{
    User *u = new User(uid, pid, name, profile_link, gender, plat_type,
            bIsYellowDmd, bIsYellowDmdYear, i4YellowDmdLv, fpid);
    TEST[E_USER]++;
    u->czDeviceType = device_from;
    mapUidToUser(uid, u);

    return u;
}

int
GameDataHandler::loadUser(int64 uid, bool self_load, bool &lottery, bool &loginPrize, bool& showInvite)
{
    User* user = getUser(uid);
    if (user == NULL)
        return 1;
    lottery		= false;
    loginPrize	= false;
    showInvite  = false;
    if (self_load)
    {

        time_t atime	= time(NULL);
        tm * atimeinfo	= localtime(&atime);
        int ayday		= atimeinfo->tm_yday;
        user->setLastLoginTime(atime);

        //user->setRevision(revision_);
        // markUserDirty(user);
    }
    return 0;
}

bool
GameDataHandler::addExperience(User *user, int etype, bool add_gold)
{

    return false;
}

bool
GameDataHandler::addExperience(User* user, int experience)
{

    return false;
}

int
GameDataHandler::addSysMsg(const string &sys_msg)
{
    last_sys_msg_id_ = sys_msg_id_;
    last_sys_msg_ = sys_msg_;
    sys_msg_id_ ++;
    sys_msg_ = sys_msg;

    return sys_msg_id_;
}

int
GameDataHandler::removeSysMsg()
{
    if (sys_msg_id_ != last_sys_msg_id_)
    {

        sys_msg_id_ = last_sys_msg_id_;
        sys_msg_ = last_sys_msg_;
    }
    return sys_msg_id_;
}

GameFriendInfo*
GameDataHandler::getFriendInfo(const string& fpid, enum PLAT_TYPE i4PlatType)
{
    if (i4PlatType < 0 || i4PlatType >= PLAT_TYPE_MAX)
    {
        return NULL;
    }
    hash_map<string, GameFriendInfo*> &refPlatid_FrInfo = platid_friendinfo_[i4PlatType];
    hash_map<string, GameFriendInfo*>::iterator iter = refPlatid_FrInfo.find(fpid);
    if (iter == refPlatid_FrInfo.end())
    {
        return NULL;
    }
    GameFriendInfo* friendinfo = iter->second;
    //测试
    if (friendinfo->timestamp < revision_ - 12 * 3600 * 1000)
    {

        refPlatid_FrInfo.erase(iter);
        delete friendinfo;
        friendinfo = NULL;
    }
    return friendinfo;
}

GameFriendInfo*
GameDataHandler::getFriendInfo(const int64 uid, enum PLAT_TYPE i4PlatType)
{
    hash_map<int64, string>  &friPidtoUid = friendinfo_platid_uid_[i4PlatType];
    hash_map<int64, string> ::iterator itor = friPidtoUid.find(uid);
    if (itor == friPidtoUid.end())
    {

        return NULL;
    }
    return getFriendInfo(itor->second, i4PlatType);
}

void
GameDataHandler::setFriendInfo(const string& fpid, int64& fid, const string& jsonstr, enum PLAT_TYPE i4PlatType)
{
    if (i4PlatType < 0 || i4PlatType >= PLAT_TYPE_MAX)
    {
        return;
    }
    if (FriendInfoLiteCacheCount() > GameConfig::GetInstance()->GetMaxFriendCacheCnt())
    {
        return;
    }
    hash_map<string, GameFriendInfo*> &refPlatid_FrInfo = platid_friendinfo_[i4PlatType];
    hash_map<string, GameFriendInfo*>::iterator iter = refPlatid_FrInfo.find(fpid);
    hash_map<int64, string>  &friPidtoUid = friendinfo_platid_uid_[i4PlatType];
    GameFriendInfo* pInfo = NULL;
    if (iter != refPlatid_FrInfo.end())
    {
        GameFriendInfo* oldinfo = iter->second;
        refPlatid_FrInfo.erase(iter);
        oldinfo->Clear();
        pInfo = oldinfo;
    }
    else
    {
        pInfo = new GameFriendInfo;
        TEST[E_GAME_FRIENDINFO]++;
    }

    if (NULL == pInfo)
    {

        return;
    }

    pInfo->timestamp = revision_;
    pInfo->fid = fid;
    pInfo->jsonstr = jsonstr;
    //	pInfo->ref_cnt++;
    refPlatid_FrInfo.insert(make_pair(fpid, pInfo));
    friPidtoUid[fid] = fpid;
}

int
GameDataHandler::getGamedIdByUserId(int64 uid)
{
    int hash = getUidHash(uid);

    return hash % serverConfig.gamedNum() + 1;
}

int
GameDataHandler::getGameIdfromPlatId( string const& platid )
{
    int hash = getPlatidHash(platid);

    return hash % serverConfig.gamedNum() + 1;
}

int
GameDataHandler::FriendInfoLiteCacheCount()
{
    int ret = 0;
    for (int i = 0; i < PLAT_TYPE_MAX; i++)
    {

        ret += platid_friendinfo_[i].size();
    }
    return ret;
}

int64
GameDataHandler::FriendInfoLiteCacheSize()
{
    int64 ret = 0;
    for (int i = 0; i < PLAT_TYPE_MAX; i++)
    {
        int index = platid_friendinfo_[i].size() / 2;
        hash_map<string, GameFriendInfo*> ::iterator itor = platid_friendinfo_[i].begin();
        for (int j = 0; j < index; j++)
        {
            itor++;
        }
        if ((GameFriendInfo*) itor->second != NULL)
        {

            ret += itor->second->jsonstr.length() * platid_friendinfo_[i].size();
        }
    }
    return ret;
}

void
GameDataHandler::UpdateRemoveUser()
{
    if (GameConfig::GetInstance()->GetEnableFreeFlag() == 0)
    {
        return;
    }

    if ((int) users_.size() <  GameConfig::GetInstance()->GetMaxUserCnt())
    {
        return;
    }

    time_t timeNow = Clock::getCurrentSystemTime();
    if (timeNow - timeRemoveUpdate < GameConfig::GetInstance()->GetFreeUpdateTime())
    {
        return;
    }

    if (timeNow < timeFirstInit + GameConfig::GetInstance()->GetMaxNoUseTime() *1000/*3600 * 1000 * 24*/)
    {//time less than min delete span return
        return;
    }

    time_t time_first = Clock::getUTime();
    int free_cnt(0);
    int check_times(0), end_times(0), user_null_times(0), user_online_times(0), dirty_times(0);
    vector<User*> vecRemoveUser;
    list<int64>::iterator iter = remove_list_.begin();
    while (iter != remove_list_.end())
    {
        check_times++;
        int64 uid = *iter;
        hash_map<int64, User*>::iterator iter_user = users_.find(uid);
        if (iter_user == users_.end() )
        {//error
            iter++;
            end_times++;
            continue;
        }

        User * pUser = iter_user->second;
        if (pUser == NULL)
        {
            //iter++;
            iter = remove_list_.erase(iter);
            user_null_times++;
            continue;
        }

        if (pUser->Online() == true)
        {
            iter++;
            user_online_times++;
            continue;
        }

        if (timeNow < pUser->revision() + GameConfig::GetInstance()->GetMaxNoUseTime() *1000/*3600 * 1000 * 24*/)
        {//delete less than 24h
            iter++;
            continue;
        }

        map<int64, User*>::iterator iter_dirty = dirty_users_.find(pUser->id());
        if (iter_dirty != dirty_users_.end())
        {
            iter++;
            dirty_times++;
            continue;
        }

        RemoveUser(pUser);
        iter = remove_list_.erase(iter);
        free_cnt++;
        remove_cnt++;

        if (free_cnt >= GameConfig::CreateInstance()->GameConfig::GetInstance()->GetMaxFreeCnt())
        {
            break;
        }

        time_t time_last = Clock::getCurrentSystemTime();
        if (time_last - timeNow > 50)
        {//more than 50ms return

            break;
        }
    }

    stat_.capture("free_check_times", check_times);
    stat_.capture("end_times", end_times);
    stat_.capture("user_null_times", user_null_times);
    stat_.capture("user_online_times", user_online_times);
    stat_.capture("dirty_times", dirty_times);
    time_t time_second = Clock::getUTime();
    stat_.capture("free_logic", (float) (time_second - time_first));
    timeRemoveUpdate = Clock::getCurrentSystemTime();
}

void
GameDataHandler::RemoveUser( User *pUser )
{
    if (pUser)
    {
        stat_.capture("db_size", pUser->GetDbUser().ByteSize());
        time_t time_first = Clock::getUTime();
        hash_map<int64, User*>::iterator iter;
        iter = users_.find(pUser->id());
        if (iter != users_.end())
        {//by ssneptune 2012.03.02

            users_.erase(iter);
            time_t time_second = Clock::getUTime();
            stat_.capture("erase_user", (float) (time_second - time_first));
            delete pUser;
            pUser = NULL;
            TEST[E_USER]--;
            time_t time_last = Clock::getUTime();
            stat_.capture("free_user", (float) (time_last - time_second));
        }
        //cout<<"free user:"<<time_last-time_first<<endl;
    }
}


