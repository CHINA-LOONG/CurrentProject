#pragma once
#ifndef _WIN32
#include <log4cxx/logger.h>
#else
#include "common/Logger_win.h"
#endif

#include <string>
#include <queue>
#include "common/const_def.h"
#include "common/counter.h"
#include "event.pb.h"
#include <pthread.h>

using namespace std;

struct EventComp
{

    bool operator()(const pair<unsigned int, Event*> le, const pair<unsigned int, Event*> re) const
    {
        return (le.second->time() > re.second->time()) || (le.second->time() == re.second->time() && le.first > re.first);
    }
} ;

class EventQueue
{
public:
    EventQueue(void);
    ~EventQueue(void);

    void pushStringEvent(const string &req, int worldFD = -1 , int nHallSrvID = -1);

    /*
    // push request from ClientHanlder
    // using safePushEvent
    void pushLoadUser(int nid, int state, int64 uid, int64 puid, int firstload);
    void pushLoadTank(int nid, int state, int64 uid, int64 tuid, int64 tid);
    void pushLoadFish(int nid, int state, int64 uid, int64 tuid, int64 tid);
    void pushFeed(int nid, int state, int64 uid, int64 fid, int fcat, int ftype, int x, int y, int64 tuid);
    void pushPickGold(int nid, int state, int64 uid, int64 tuid, int64 tid);
    void pushBuyFish(int nid, int state, int64 uid, int ftype, int mtype);
    void pushFreeFish(int nid, int state, int64 uid, int64 fid);
    void pushMateFish(int nid, int state, int64 uid1, int64 fid1, int64 uid2, int64 fid2);
    void pushPutFish(int nid, int state, int64 uid, int64 fid, int64 tid, int x, int y, int64 src_tid);
    void pushLeave(int nid, int state, int64 uid, int64 fd);
    void pushBuyFood(int nid, int state, int64 uid, int cat, int dtype, int amount);	
    void pushLoadMessage(int nid, int state, int64 uid, int mtype);
    void pushSaveDecoration(int nid, int state, int64 uid, int64 tid, const vector<int64> &buy, 
        const vector<int64> &change, const vector<int64> &sell, const vector<int64> &other);
    void pushSaveRecord(int nid, int state, int64 uid, int key, const string& value);
    void pushLoadRecord(int nid, int state, int64 uid, int key, int subkey);
	
    // request from Admin ClientHandler
    void pushAddSysMessage(int nid, int state, int64 uid, const string &msg);
    void pushRemoveSysMessage(int nid, int state, int64 uid);
    void pushReloadConfig(int nid, int state, int64 uid);
    void pushShowFrdAndDeco(int nid, int state, int64 uid,int nShowDeco,int nShowFrd);
    void pushAddGold(int nid, int state, int64 uid, int amount, int64 puid);
    void pushAddDiamond(int nid, int state, int64 uid, int amount, int64 puid);
    void pushAddExperience(int nid, int state, int64 uid, int level, int64 puid);
    void pushClearExperienceCounter(int nid, int state, int64 uid);

    // 管理员操作用户信息指令
    void pushUpdateExperience(int nid, int state, int64 uid, int64 puid, int exp);

    void pushExpandTank(int nid, int state, int64 uid, int64 tid, int flimit, int type);
    void pushExpandBox(int nid, int state, int64 uid, int64 tid, int gboxlevel, int dboxlevel, int etype);
    void pushSaveCommand(int nid, int state, int64 uid, int type);
    void pushLoadHistory(int nid, int state, int64 uid, int64 puid);
	
    void pushStatistics(int nid, int state, int64 uid);
    void pushEnter(int nid, int state, int64 uid, string strIP);
    void pushSee(int nid, int state, int64 uid, int type, int64 param);
    void pushBuyTank(int nid, int state, int64 uid, int tank_num, int diamond_buy);
    //Invite Friend Lottery 
    void pushLottery(int nid, int state, int64 uid, int type);
    // 玩家点击格子选择抽奖
    void pushLotteryNineGrid(int nid, int state, int64 uid, int i4Index);
    // 请求可抽奖次数信息
    void pushLotteryInfo(int nid, int state, int64 uid);
    // 有新的抽奖机会通知
    void pushLotteryNew(int nid, int state, int64 uid);
    // void pushMakeIllCommand(int64 uid);
    // void pushMakeIll(time_t time, int start_uid);
    // void pushUpdateTank(int64 tid, time_t time);
    void pushUseProps(int nid, int state, int64 uid, int type, int64 tid);
	
    // push request from EventHandler
    // using safePushEvent
    void pushUpdateFishes(int64 tuid, int64 tid, time_t time);
    void pushSaveData(int nid, int state, int64 uid, time_t time, int type);
	
    // push request from WebServerHandler
    // using safePushEvent
    void pushUserLogin(int nid, int fd, int state, const string &platid, const string &name, 
        const string &head, int gender, int star, const vector<string> &fpid, int sid, int siteid,
        int i4IsYellowDmd, int i4IsYellowDmdYear, int i4YellowDmdLv);
    void pushPay(int nid, int fd, int state, const string &platid, int64 orderid, int amount, int sid);
    void pushInvite(int nid, int fd, int state, const string &strInviteUserPlatID, const string& strInvitedUserPlatID, int sid,int64 invitedUID,const string& strInvitedUserName);
    void pushUninstall(int nid, int fd, int state, const string &platid, int sid);
    void pushSyncFriend(int nid, int fd, int state, const string &platid, const vector<string> &fpid, int siteid, int sid);
    void pushAcceptGift(int nid, int fd, int state, int64 uid1, int64 uid2, int gtype, int gsubtype, int gamount, int sid);
    void pushWebSee(int nid, int fd, int state, int type, long long param, int sid);
    // push request from GameNetHandler
    void pushUpdateWorkingStatus(int nid, int state, int param);
    void pushSyncUser(int nid, int state, int level, int exp);
    void pushFriendRequest(int nid, int state, int64 uid, const string &platid, const string &fpid, PLAT_TYPE i4PlatType);
    //void pushFriendRequest(int nid, int state, int64 uid, int64 fuid);
    //dmd buy
    void pushWorldBuyFish(int nid, int fd, int state, int64 uid,int mid,int dmd,int trueDmd,int sid,int leftdmd);
    void pushWorldBuyFood(int nid, int fd, int state, int64 uid,int ftype,int dmd,int trueDmd,int count,int sid,int leftdmd);
    void pushWorldBuyDeco(int nid, int fd, int state, int64 uid,int64 tid,vector<int64>& buy,int sid,int leftdmd);
    void pushWorldExpandBox(int nid,int fd,int state, int64 uid,int dLevel,int dmd,int sid,int leftdmd);
    void pushWorldExpandTank(int nid,int fd,int state, int64 uid,int limit,int dmd,int sid,int leftdmd);
    void pushWorldAddTank(int nid,int fd,int state,int64 uid,int level,int dmd,int sid,int leftdmd);
    void pushUseYanzheng(int nid, int state, int64 uid, string& strYanzheng);
    void pushUseNewYZ(int nid, int state, int64 uid);
    void pushSetRestrict(int nid,int fd, int state, int64 uid, int sid, int64 time);
    void pushWorldBuyGiftPack(int nid, int fd, int state, int64 uid,int ftype,int dmd,int trueDmd,int count,int sid,int leftdmd);

    //无参数事件
    void pushEventNoParam(int nid, int state, int64 uid, int i4EventIndex);
    //使用参数X的事件
    void pushEventWithParamX(int nid, int state, int64 uid, int i4EventIndex, int i4Param_X);
    void pushEventWithParamsXY(int nid, int state, int64 uid, int i4EventIndex, int i4Param_X, int i4Param_Y);

    void pushTestInterface(int nid, int state, int64 uid, int64 tuid,int nType, int nParam,int64 llParam);
     */
    inline pthread_mutex_t* mutex()
    {
        return &mutex_;
    }

    inline time_t topTime()
    {
        return event_queue_.top().second->time();
    }

    inline Event* popEvent()
    {
        Event *e = event_queue_.top().second;
        event_queue_.pop();
        return e;
    }

    inline void freeEvent(Event *e)
    {
        delete e;
        TEST[E_EVENT]--;
    }

    inline Event* allocateEvent()
    {
        TEST[E_EVENT]++;
        return new Event();
    }

    inline bool isEmpty()
    {
        return event_queue_.empty();
    }

    inline int	getSize()
    {
        return event_queue_.size();
    }

    inline void safePushEvent(Event *e)
    {
        acquireLock();
        pushEvent(e);
        releaseLock();
    }

    inline void safePushCopyEvent(Event *e, int nMax)
    {
        acquireLock();
        if (event_queue_.size() < (unsigned int)nMax)
        {
            Event* ne = allocateEvent();
            ne->CopyFrom(*e);
            pushEvent(ne);
        }
        releaseLock();
    }
protected:
    typedef priority_queue<pair<unsigned int, Event*>, vector<pair<unsigned int, Event*> >, EventComp> PriorityEventQueue;
    PriorityEventQueue event_queue_;
    pthread_mutex_t mutex_;
    unsigned int inner_counter_;

    inline void acquireLock()
    {
        pthread_mutex_lock(&mutex_);
    }

    inline void releaseLock()
    {
        pthread_mutex_unlock(&mutex_);
    }

    inline void pushEvent(Event *e)
    {
        if (!e->IsInitialized())
        {
            LOG4CXX_ERROR (logger_, "Event: " << e->cmd() << " is not initialized! in pushEvent\n" << e->DebugString());
        }
        else
        {
            event_queue_.push(make_pair(inner_counter_++, e));
        }
    }
    static const string delims_;
    log4cxx::LoggerPtr logger_;
} ;
