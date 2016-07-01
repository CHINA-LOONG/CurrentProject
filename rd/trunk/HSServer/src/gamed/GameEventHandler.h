#pragma once
#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif
#include "common/const_def.h"
#include "EventHandler.h"
#include "event.pb.h"

class EventHandler;
class EventQueue;
class GameDataHandler;
class GameNetHandler;
class User;

class GameEventHandler
{
public:
    GameEventHandler(EventQueue *eq, GameDataHandler *dh, GameNetHandler *nh, int nid);
    ~GameEventHandler(void);
    void start();
    void sendFdString(int fd, const char* str, size_t size);

    int  GetSrvID()
    {
        return nid_;
    }
    void initialEventProcessors();
    void initAutoEventProcessors();
    //     void sendEventToHall(Event* e);
    // 	void sendEventToAllHall(Event* e);	
    void sendEventToWorld(Event *e);
    void sendEventToWorld(Event *e, int wid);
    bool sendEventToWorldEx(Event *e, int wid);
    void sendDataToUser(int fd, int type, const string &text);
    void createUserFdMap(int fd, int64 uid);
    void removeUserFdMap(int fd, int64 uid);
    void sendToGlobalUser(int64 uid, int cmd, const string &text);
    void SendToAllOnlineUser(int cmd, const string& text);

    void SendErrorToUser(User* pUser, Event* e);
    void SendErrorToUser(User* pUser, int emErrType);
    void InitRank();
public:

    inline EventHandler* getEventHandler()
    {
        return eh_;
    }

    inline GameDataHandler* getDataHandler()
    {
        return dh_;
    }

    inline GameNetHandler* getNetHandler()
    {
        return nh_;
    }

    inline time_t getReversion()
    {
        return eh_->getReversion();
    }

    inline void postBackEvent(Event* e)
    {
        eh_->postBackEvent(e);
    }

    inline const string& getServerIp()
    {
        return serverIp;
    }

    inline int getServerPort()
    {
        return serverPort;
    }

    inline GameWorkingStatus getWorkingStatus()
    {
        return eh_->getWorkingStatus();
    }

    inline void setWorkingStatus(GameWorkingStatus status)
    {
        eh_->setWorkingStatus(status);
    }

    inline EventQueue*  getEventQueue()
    {
        return eq_;
    }

    void	setProcessIgnore();
    unsigned int getCacheFdSize();

public:
    int AllocHallSrvID(User* pUser);		//为用户分配一个大厅ID
    int GetHallSrvIDByRoomID(int nRoomID);	//根据房间ID寻找大厅服务器ID

    void sendChangeToRankWorld( int _kind, int64 _llUid, std::string _strName, int _llWeight );	// _kind=1总重排行榜,2=单次排行榜
    void sendChangeToRankWorldEx( int _kind, int64 _llUid, std::string _strName, int _llWeight, int64 rank_version = -1 , std::string _strUrl = "");
    //bool FillRankInfo(int _kind,int from,int to,MseRankInfoAsk &response);
    //bool FillIntervalRankInfo(int _kind,int from, int to,MseRankInfoAsk &response);
    //bool UpdateIntervalRank();
public:

    static GameEventHandler* GetInst();
private:
    void registerToRankWorld(int rank_type = -1);

private:
    string serverIp;
    int serverPort;

    EventHandler* eh_;
    GameDataHandler* dh_;
    EventQueue* eq_;
    GameNetHandler* nh_;
    int nid_;
    log4cxx::LoggerPtr logger_;
    static GameEventHandler* g_pInst;
} ;
