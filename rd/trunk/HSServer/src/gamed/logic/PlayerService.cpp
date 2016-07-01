/* 
 * File:   PlayerService.cpp
 * Author: Kidd
 * 
 * 一些说明 
 * 因为有相同的结构.
 * 所以在 ServeiceMacro.h里定义了一些宏
 * _M_RequestInit_ 初始化了 request和pResponse
 * _M_DecodeAttr_(参数个数) 将参数变成string atts[]; 参数不足将会return
 * _M_Err_ 构造了一个默认的错误返回
 * _M_Result_(成功与否,返回值json对象) 是正常的返回
 * 
 * 所有的_PS_xxxxx 都是为了利用对象构造函数机制.来注册消息处理的.
 * 
 * 多人消息依然沿用了 handle->handle_taker->handle_sander_after 的逻辑结构
 * 
 * 
 * Created on 2013年1月15日, 上午10:39
 */

#include "PlayerService.h"
#include "../event/event.pb.h"
#include "../event/MseGatewayFunction.pb.h"
#include "../event/MceGatewayFunction.pb.h"
#include "User.h"
#include "Player.h"
#include "ConfigUnit/UniverseCfgUnit.h"
#include "UniverseMgr.h"
#include "../logic/ServicesMacro.h"
#include "OpenLevelMgr.h"
#include "MessageCtrl.h"
#include "../gamed/event/friendrequest.h"
#include "ConfigUnit/ItemCfgUnit.h"
#include "ItemManager.h"
#include "../logic/Clock.h"
#include "RankInfoMgr.h"
#include "UserCtrl.h"
#include "../logic/CMEMReader.h"
#include "phpdecoder.h"
#include "LogCtrl.h"
#include "BitCache.h"
#include "DallyUniverseMgr.h"
#include "ActiveCtrl.h"
#include "ConfigUnit/MaterialCfgUnit.h"
#include "../gamed/event/DealSimpleMark.h"
#include "RuntimeVariableMgr.h"
#include "KingOfStar.h"

const std::string ServerGroupName = "PlayerService";
const std::string MapGroupName = "Map";
const std::string defaulterr = "[{msg:err}]";

void
PlayerService::RegAllActions()
{
    static GatewayActionUnit _PS_gameInit(ServerGroupName, "gameInit", PlayerService::gameInit);
    static GatewayActionUnit _PS_frontenter("FrontService", "enterGame", PlayerService::gameInit);
    static GatewayActionUnit _PS_gameStart(ServerGroupName, "gameStart", PlayerService::gameStart);
    static GatewayActionUnit _PS_gameEnd(ServerGroupName, "gameEnd", PlayerService::gameEnd);
    static GatewayActionUnit _PS_poll(ServerGroupName, "poll", PlayerService::poll);
    static GatewayActionUnit _PS_getGameConfigurations(ServerGroupName, "getGameConfigurations", PlayerService::getGameConfigurations);
    static GatewayActionUnit _PS_getCandyProperties(ServerGroupName, "getCandyProperties", PlayerService::getCandyProperties);
    static GatewayActionUnit _PS_setCandyProperties2(ServerGroupName, "setCandyProperty", PlayerService::setCandyProperty);
    static GatewayActionUnit _PS_getRecipes(ServerGroupName, "getRecipes", PlayerService::getRecipes);
    static GatewayActionUnit _PS_getMessages(ServerGroupName, "getMessages", PlayerService::getMessages);
    static GatewayActionUnit _PS_getLevelToplist(ServerGroupName, "getLevelToplist", PlayerService::getLevelToplist);
    static GatewayActionUnit _PS_openLevel(ServerGroupName, "openLevel", PlayerService::openLevel);
    static GatewayActionUnit _PS_reportFramerate(ServerGroupName, "reportFramerate", PlayerService::reportFramerate);
    static GatewayActionUnit _PS_setSoundFx(ServerGroupName, "setSoundFx", PlayerService::setSoundFx);
    static GatewayActionUnit _PS_setSoundMusic(ServerGroupName, "setSoundMusic", PlayerService::setSoundMusic);
    static GatewayActionUnit _PS_unlockItem(ServerGroupName, "unlockItem", PlayerService::unlockItem);
    static GatewayActionUnit _PS_handOutItemWinnings(ServerGroupName, "handOutItemWinnings", PlayerService::handOutItemWinnings);
    static GatewayActionUnit _PS_useItemsInGame(ServerGroupName, "useItemsInGame", PlayerService::useItemsInGame);
    static GatewayActionUnit _PS_useItemsInGameMobile(ServerGroupName, "useItemsInGameMobile", PlayerService::useItemsInGameMobile);

    static GatewayActionUnit _PS_useTicket(ServerGroupName, "useTicket", PlayerService::useTicket);
    static GatewayActionUnit _PS_request(ServerGroupName, "request", PlayerService::request, PlayerService::request_taker, PlayerService::request_sender_after);
    static GatewayActionUnit _PS_getGift(ServerGroupName, "getGift", PlayerService::getGift);
    static GatewayActionUnit _PS_getMyStarsRecord(ServerGroupName, "getMyStarsRecord", PlayerService::getMyStarsRecord);
    static GatewayActionUnit _PS_updateGuide(ServerGroupName, "updateGuide", PlayerService::updateGuide);
    static GatewayActionUnit _PS_buyGoods(ServerGroupName, "buyGoods", PlayerService::buyGoods);
    static GatewayActionUnit _PS_getFriendsData(ServerGroupName, "getFriendsData", PlayerService::getFriendsData);
    static GatewayActionUnit _PS_getBalance(ServerGroupName, "getBalance", PlayerService::getBalance);
    static GatewayActionUnit _PS_getSignin(ServerGroupName, "getSignin", PlayerService::getSignin);
    static GatewayActionUnit _PS_getFanhua(ServerGroupName, "getFanhua", PlayerService::getFanhua);
    static GatewayActionUnit _PS_getFanhuanReward(ServerGroupName, "getFanhuanReward", PlayerService::getFanhuanReward);
    static GatewayActionUnit _PS_getTodayCost(ServerGroupName, "getTodayCost", PlayerService::getTodayCost);
    static GatewayActionUnit _PS_stuffReward(ServerGroupName, "stuffReward", PlayerService::stuffReward);
    static GatewayActionUnit _PS_getCDkeyReward(ServerGroupName, "getCDkeyReward", PlayerService::getCDkeyReward);
    static GatewayActionUnit _PS_getSessionId(ServerGroupName, "getSessionId", PlayerService::getSessionId);
    static GatewayActionUnit _PS_action(ServerGroupName, "active", PlayerService::action);
    static GatewayActionUnit _PS_LoadFriendForMobile(ServerGroupName, "FriendData", PlayerService::LoadFriendForMobile, PlayerService::LoadFriendForMobile, PlayerService::LoadFriendForMobile);
    static GatewayActionUnit _PS_LoadFriendForMobileUid(ServerGroupName, "FriendDataUid", PlayerService::LoadFriendForMobile, PlayerService::LoadFriendForMobile, PlayerService::LoadFriendForMobile);
    static GatewayActionUnit _PS_gameEndMobile(ServerGroupName, "gameEndMobile", PlayerService::gameEndMobile);
    static GatewayActionUnit _PS_getItemAmount(ServerGroupName, "getItemAmount", PlayerService::getItemAount);
    static GatewayActionUnit _PS_RemoteLog(ServerGroupName, "remotelog", PlayerService::RemoteLog);
    static GatewayActionUnit _PS_AddDevice(ServerGroupName, "NewDevice", PlayerService::AddMobileDevice);
    static GatewayActionUnit _PS_lvlScores(ServerGroupName, "levelScoreSet", PlayerService::getLvlSocres);
    static GatewayActionUnit _PS_pollMobile(ServerGroupName, "pollMobile", PlayerService::pollMobile);
    static GatewayActionUnit _PS_lFlashAddDC(ServerGroupName, "jscount", PlayerService::FlashAddDailyCounter);

    static GatewayActionUnit _MP_PutItem(MapGroupName, "putItem", PlayerService::Map_PutItem);
    static GatewayActionUnit _MP_PeekItem(MapGroupName, "peekItem", PlayerService::Map_PeekItem);
    static GatewayActionUnit _MP_MoveItem(MapGroupName, "moveItem", PlayerService::Map_MoveItem);
    static GatewayActionUnit _MP_MapDetail(MapGroupName, "house", PlayerService::Map_MapDetail);
    static GatewayActionUnit _MP_Bat(MapGroupName, "bag", PlayerService::Map_Bag);
    static GatewayActionUnit _MP_SyncItem(MapGroupName, "compose", PlayerService::Map_SyncMapItem);
    static GatewayActionUnit _MP_SyncMat(MapGroupName, "composeMaterial", PlayerService::Map_SyncMaterial);
    static GatewayActionUnit _MP_GetEggItem(MapGroupName, "stone", PlayerService::Map_GetEggItem);
    static GatewayActionUnit _MP_GetReward(MapGroupName, "getLife", PlayerService::Map_GetDailyRewardByLuxcy) ;
    static GatewayActionUnit _MP_stoneLevel(MapGroupName, "stoneLevel", PlayerService::Map_GetDropLvl);

}

PlayerService::PlayerService()
{
}

PlayerService::~PlayerService()
{
}

/******************************************************************************/
/*    登录逻辑 GameInit                                                       */
/******************************************************************************/
//<editor-fold desc="登录逻辑 GameInit  ">

void
PlayerService::gameInit(Event* e, User* pUser)
{
    _M_RequestInit_;
    Value json_player(objectValue);
    if (request.actionname() == "gameInit")
    {
        pPlayer->GetDailyCounter().AddCounter(DC_LOGINCNT);
    }
    //道具默认解锁 
    pPlayer->GetBag().MarkItemUnlock(ItemManager::GetKingOfStarTicketTypeId());
    KingOfStar::GetInst()->OnLogin(pPlayer);

    pPlayer->FillToJson(json_player, request.servername() == "FrontService");
    //pResponse->set_jsonresult(json_player.toStyledString());
    if (request.servername() == "FrontService")
    {
        //TryLoad(e, pUser);
        pUser->UpdateUserInfoLite();
        for (int i = 0; i < 20; i++)
        {
            pPlayer->GetUniverseCtrl().BuildEgg(i + 1);
        }
        pUser->GetPlayer()->GetExpLevelCtrl().AutoFix();
    }



    if (request.actionname() == "gameInit")
    {
        DealSimpleMark::getInstance()->PushRequest(pUser->id(), 2, 0);

        if (!pPlayer->GetUniverseCtrl().TestSeed(llInvalidId, 0, 0))
        {
            pPlayer->GetLiveCtrl().CostLives();
            pPlayer->GetLiveCtrl().RemoveSeed();
        }
        pPlayer->GetUniverseCtrl().AutoUnlockFix();
        //for (int i=0;i<pPlayer->GetMessageCtrl().)
        pUser->qqgroup_friends_platid_.clear();
        pPlayer->GetMessageCtrl().AddOpenidToFriendList(pUser->qqgroup_friends_platid_);
        FriendRequest::getInstance()->PushFriendRequestList(pUser);
        pPlayer->OnGameInit();
        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pPlayer->GetBag().LogAllItem(pLog);
        pLog->LogEnd("login_bag_cache");
        Value json_send_list(arrayValue);
        pPlayer->GetMessageCtrl().FillToTodaySendList(json_send_list);
        json_player["send_list"] = json_send_list;
        Value json_ask_list(arrayValue);
        pPlayer->GetMessageCtrl().FillToTodayAskList(json_ask_list);
        json_player["ask_list"]  = json_ask_list;
        json_player["isOpenGroupList"] = true;
        pUser->UpdateLogTime();
        pPlayer->GetActiveCtrl().OnSendFriend(-1, -1);
        int type_search[] = {1, 11, 21, 36, 51, 66, 81};
        if (!Clock::NowAfter("2013-7-29-0:0:0"))
        {
            for (int i = 0; i < 6; i++)
            {
                DB_Universe* pDb = pPlayer->GetUniverseCtrl().GetDBUniByTotalLvl(type_search[i]);
                if (pDb != NULL)
                {
                    pPlayer->GetUniverseCtrl().SubmitSocreToRank(type_search[i], pDb->value());
                }
            }
        }
        pPlayer->GetUser()->ReSetFriendNeedSendCnt();
        pPlayer->GetUniverseCtrl().SubmitToDallyLevelRank(7105, pPlayer->GetUniverseCtrl().DallyTotalSocre());
    }
    _M_Result_(true, json_player);

}
//</editor-fold>

/******************************************************************************/
/*    PC 游戏开始&结束                                                        */
/******************************************************************************/
//<editor-fold desc="PC 游戏开始&结束">

void
PlayerService::gameStart(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    int totallevel = llInvalidId;
    safe_atoi(atts[0], totallevel);

    Value json_reuslt(objectValue);
    Value json_currectUser(objectValue);
    pPlayer->FillBasicToJson(json_currectUser);
    json_reuslt["currentUser"] = json_currectUser;
    UniverseCfgUnit* pUni = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(totallevel);

    if (pUni == NULL)
    {
        _M_Err_;
        return;
    }
    if (!pPlayer->GetUniverseCtrl().CanPlayLevel(totallevel))
    {
        _M_Err_;
    }
    pPlayer->GetUniverseCtrl().FillLevelData(totallevel, json_reuslt["levelData"]);
    json_reuslt["seed"] = pPlayer->GetUniverseCtrl().AllocSeed();

    std::string logname = "StartLevel";
    LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
    pLog->LogItem("lvlid", totallevel)->
            LogItem("lvlytpe", pUni->level_type)->
            LogItem("item1", 0)->
            LogItem("item2", 0)->
            LogItem("item3", 0)->
            LogItem("item3", 0)->
            LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume);
    DB_DallyUniverse* pDBDally = pPlayer->GetUniverseCtrl().GetDBDallyUniByTotalLvl(totallevel);
    if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel) != NULL && pDBDally != NULL)
    {
        logname = "StartDailyLevel";
        pLog->LogItem("pay open", (pDBDally->pay_open() && pDBDally->pay_open_day() == Clock::GetDay()) ? 1 : 0)->LogItem("nCost", pDBDally->pay_cash())->
                LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume, LogCtrl::ST_Daily);

        if (pDBDally->played_cnt() <= 1)
        {
            if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel)->index == 6)
            {
                pPlayer->GetUniverseCtrl().LogDallyCache(false);
            }
            else if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel)->index == 7)
            {
                pPlayer->GetUniverseCtrl().LogDallyCache(true);
            }
        }
    }

    pLog->LogEnd(logname.c_str());

    pPlayer->GetUniverseCtrl().SetNowPlayingLvl(totallevel);
    pPlayer->GetUniverseCtrl().OnEnterDallyLevel(totallevel);

    int nEggCnt = 0;
    if (pPlayer->GetLevel() > 100)
    {
        nEggCnt = pPlayer->GetUniverseCtrl().BuildEgg(totallevel);
    }
    Value eggs(arrayValue);
    for (int i = 0; i < nEggCnt; i++)
    {
        eggs.append(pPlayer->GetDbPlayer()->universeinfo().lvlstockeditem(i) % 6000);
    }

    json_reuslt["eggs"] = eggs;
    _M_Result_(true, json_reuslt);

    // pResponse->set_jsonresult(json_reuslt.toStyledString());
}

void
PlayerService::gameEnd(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(6);
    int totallevel    = String2Int(atts[0]);
    int score           = String2Int(atts[1]);
    int seed            = String2Int(atts[2]);
    int status         = String2Int(atts[3]);
    int chv             = String2Int(atts[5]);
    int nRandom       = 0;
    std::string  log = "";
    bool bBuyOpen = false;
    if (atts.size() >= 5 && String2Int(atts[4]) != 0 && atts[4] != "")
    {
        bBuyOpen = true;
    }
    if (atts.size() >= 8)
    {
        nRandom = String2Int(atts[6]);
        log = atts[7];
    }

    //自动封号处理
    //<editor-fold desc="自动封号处理">
    int nAutoBanScore = 60 * 10000 ;
    if (totallevel == 68 )
    {
        nAutoBanScore = 120 * 10000;
    }
    if (totallevel == 18 || totallevel == 75 || totallevel == 111 || totallevel == 170)
    {
        nAutoBanScore = 80 * 10000;
    }
    if (totallevel > 7000)
    {
        nAutoBanScore = 200 * 10000;
    }
    if ( score > nAutoBanScore)
    {
        pPlayer->GetUser()->GetDbUser().mutable_player()->mutable_dally_universe()->set_cheat_bit(true);
        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pLog->LogItem("lvlid", totallevel)->
                LogItem("score", score)->
                LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume);
        pLog->LogEnd("AutoSetCheatBit");
    }
    //</editor-fold>

    bool LvlSucc      = (status == 0); //status 0:过关  1:失败
    bool bCheckLvlCfg = totallevel <= UniverseMgr::GetInst()->GetConfigLvlMax() || pPlayer->GetUniverseCtrl().IsDallyLevel(totallevel);
    LvlSucc = LvlSucc && bCheckLvlCfg;
    Value json_result(objectValue);

    UniverseCtrl& uni = pPlayer->GetUniverseCtrl();
    UniverseCfgUnit* pCfgUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(totallevel);
    pPlayer->GetLiveCtrl().Update();
    if ((!uni.TestSeed(seed, chv, score) && !bBuyOpen) || pCfgUnit == NULL || pPlayer->GetLiveCtrl().GetLives() <= 0)
    {
        pResponse->set_err(1);
        _M_Result_(false, json_result);
        return;
    }

    bool bNowTop  = totallevel == uni.GetTotalLvl();

    pPlayer->FillBasicToJson(json_result["currentUser"]);


    if (LvlSucc)
    {
        uni.AddRank(totallevel, pUser->id(), score);
        uni.RebuildTopPlayerList();
    }
    else
    {
        pPlayer->GetLiveCtrl().CostLives();
    }
    json_result["score"] = score;

    uni.FillToRankJson(totallevel, json_result["levelToplist"]);

    DB_Universe* pDbUni = uni.GetDBUniByTotalLvl(totallevel) ;
    int nOldScore = pDbUni == NULL ? 0 : pDbUni->value_kos();
    int nOldStar  = pDbUni == NULL ? 0 : pDbUni->stars_kos();
    int nOldExpStar = pDbUni == NULL ? 0 : pDbUni->stars();
    int nOldExpSocre = pDbUni == NULL ? 0 : pDbUni->value();



    int nStar = pCfgUnit->GetStarLvl(score);
    json_result["stars"] = nStar;
    json_result["bestResult"] = LvlSucc && uni.IsBest(totallevel, score);
    json_result["newStarLevel"] = LvlSucc && uni.IsBestStar(totallevel, nStar);
    Value ja_event (arrayValue);
    uni.FillToUnlockEvent(totallevel, score, ja_event);
    json_result["events"] = ja_event;

    json_result["episodeId"] =  pCfgUnit->episodeId;
    json_result["levelId"] = pCfgUnit->levelId;


    json_result["NextUse"] = KingOfStar::GetInst()->GetEnterLevelNeedItemCnt(pPlayer, totallevel);

    //关卡挑战
    if (LvlSucc)
    {
        pPlayer->GetExpLevelCtrl().OnGetStar(totallevel, nOldExpStar, nStar, score > nOldExpSocre);
        if (KingOfStar::GetInst()->OnPlayerLevelSucc(pPlayer, totallevel, nOldScore, score, nOldStar, nStar, json_result))
        {
            if (score > nOldScore)
            {
                pDbUni->set_value_kos(score);
            }
            if (nStar > nOldStar)
            {
                pDbUni->set_stars_kos(nStar);
            }
        }

    }
    json_result["ExpAdd"] = pPlayer->GetExpLevelCtrl().nExpLastAdd;
    pPlayer->GetExpLevelCtrl().nExpLastAdd = 0;

    //关卡里的掉落逻辑
    //<editor-fold desc="关卡里的掉落逻辑">

    Value json_egg(arrayValue);
    if (LvlSucc)
    {
        const int nItemDropMax = UniverseMgr::GetInst()->GetDropItemCnt(totallevel);
        int nCnt = 0;
        int tid[UniverseMgr::s_nItemDropMaxCnt];
        int num[UniverseMgr::s_nItemDropMaxCnt];

        for (int i = 0; i < pPlayer->GetDbPlayer()->universeinfo().egg_logic_size(); i++)
        {
            int tmp_tid = pPlayer->GetDbPlayer()->universeinfo().egg_logic(i);
            int find_index = -1;
            bool bFind = false;
            for (find_index = 0; !bFind && find_index < nCnt; )
            {
                if (tid[find_index] == tmp_tid)
                {
                    bFind = true;
                }
                else
                {
                    find_index++;
                }
            }
            if (nCnt <= find_index)
            {
                nCnt = find_index + 1;
                tid[find_index] = tmp_tid;
                num[find_index] = 1;
            }
            else
            {
                num[find_index] ++;
            }
        }
        for (int i = 0; i < nCnt ; i++)
        {
            Value egg(objectValue);
            egg["id"] = tid[i];
            egg["num"] = num[i];
            json_egg.append(egg);
            pPlayer->GetBag().AddItem(tid[i], num[i]);
        }
    }
    pPlayer->GetDbPlayer()->mutable_universeinfo()->clear_egg_logic();
    json_result["items"] = json_egg;
    //</editor-fold>

    uni.FillToJson(json_result) ;
    if (LvlSucc)
    {
        uni.SubmitSocre(totallevel, score);
    }

    //每日关卡处理
    if (uni.IsDallyLevel(totallevel))
    {
        uni.SubmitSocreForDally(totallevel, score, LvlSucc);

        uni.AddRankDally(pUser->id(), uni.DallyTotalSocre());

        if (LvlSucc)
        {
            pPlayer->GetDailyCounter().AddCounter(DC_DAILY_LVL_SUCC);
        };
        int nPlayCnt = 0;
        int nTotalValue = 0;
        for (int i = 0; i < pPlayer->GetDbPlayer()->dally_universe().uniset_size(); i++)
        {
            if (pPlayer->GetDbPlayer()->dally_universe().uniset(i).played_cnt() > 0)
            {
                nPlayCnt ++;
            }
            nTotalValue += pPlayer->GetDbPlayer()->dally_universe().uniset(i).uni().value();
        }
        pPlayer->GetDailyCounter().SetCounter(DC_DAILY_LVL_TOTAL_SOCRE, 0);
        pPlayer->GetDailyCounter().AddCounter(DC_DAILY_LVL_TOTAL_SOCRE, nTotalValue);
        if (nPlayCnt >= 8)
        {
            pPlayer->GetDailyCounter().AddCounter(DC_DAILY_LVL_ALL_PLAYED, 1);
        }

    }


    if (LvlSucc)
    {
        pPlayer->GetDailyCounter().AddCounter(DC_LVLUP, 1);
        pPlayer->GetDailyCounter().AddCounter(DC_SOCRE, score);

        std::string logname = "EndWin";
        if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel) != NULL)
        {
            logname = "DailyEndWin";
        }
        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pLog->LogItem("lvlid", totallevel)->
                LogItem("type", pCfgUnit->level_type)->
                LogItem("score", score)->
                LogItem("star", nStar)->
                LogItem("history best", uni.GetHistoryBestValue(totallevel))->
                LogItem("time", 0)->
                LogItem("ext_step", 0)->
                LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume);
        if (bBuyOpen)
        {
            pLog->LogItem("buy open", "buyOpen");
            int nCost = OpenLevelMgr::GetInst()->GetCostNum(totallevel);
            pLog->LogItem("buy open cost", nCost);


        }
        else
        {
            pLog->LogItem("buy open", "noramlPlay");
            pLog->LogItem("buy open cost", 0);

        }
        if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel) != NULL)
        {
            logname = "DailyEndWin";

            pLog->LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume, LogCtrl::ST_Daily);

        }
        pLog->LogEnd(logname.c_str());

        if (bNowTop)
        {
            pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
                    LogItem("关卡id", totallevel)->
                    LogItem("尝试次数(1~xxx)", uni.GetFailCnt(totallevel) + 1)->
                    LogItem("是否是付费通关(1是;0不是)", bBuyOpen ? 1 : 0)->
                    LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume)->
                    LogEnd("TryTimesPerLevel");
            if (!uni.IsDallyLevel(totallevel))
            {
                pPlayer->OnLevelUp();
            }
        }
    }
    else
    {
        std::string logname = "EndLose";

        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pLog->LogItem("lvlid", totallevel)->
                LogItem("type", pCfgUnit->level_type)->
                LogItem("score", score)->
                LogItem("fail result", "unknowen")->
                LogItem("time", 0)->
                LogItem("buy win", 0)->
                LogItem("ext_step", 0)->
                LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume);
        if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel) != NULL)
        {
            logname = "DailyEndLose";
            pLog->LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume, LogCtrl::ST_Daily);

        }
        pLog->LogEnd(logname.c_str());
        uni.AddFailCnt(totallevel);
    }
    if ( score > 200000)
    {
        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pLog->LogItem("lvlid", nRandom);
        pLog->LogItem("lvlid", log.c_str());
        for (int i = 0; i < atts.size(); i++)
        {
            pLog->LogItem("lvlid", atts[i].c_str());

        }
        pLog->LogEnd("record");
    }
    if (bBuyOpen)
    {
        int nCost = OpenLevelMgr::GetInst()->GetCostNum(totallevel);

        pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
                LogItem("lvlid", totallevel)->
                LogItem("lvltype", pCfgUnit->level_type)->
                LogItem("lvlid", 2000)->
                LogItem("nCostNum", nCost)->
                LogItem("nItemNum", 1)->
                LogSubType(LogCtrl::ST_Consume, LogCtrl::ST_Get)->
                LogEnd("BuyBooster");
    }
    _M_Result_(true, json_result);
    pPlayer->GetLiveCtrl().RemoveSeed(); //change the seed for unknown
    pPlayer->GetUniverseCtrl().m_nTotalStarCache = llInvalidId;
}

void
PlayerService::gameEndMobile(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(2);
    int totallevel    = String2Int(atts[0]);
    int score           = String2Int(atts[1]);

    pPlayer->GetUniverseCtrl().SetMobileCache(totallevel, score);


    if ( score > 60 * 10000)
    {
        pPlayer->GetUser()->GetDbUser().mutable_player()->mutable_dally_universe()->set_cheat_bit(true);
        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pLog->LogItem("lvlid", totallevel)->
                LogItem("score", score)->
                LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume);
        pLog->LogEnd("AutoSetCheatBit");
    }

    bool LvlSucc      = true;
    bool bCheckLvlCfg = totallevel <= UniverseMgr::GetInst()->GetConfigLvlMax() || pPlayer->GetUniverseCtrl().IsDallyLevel(totallevel);
    LvlSucc = LvlSucc && bCheckLvlCfg;
    Value json_result(objectValue);

    UniverseCtrl& uni = pPlayer->GetUniverseCtrl();
    UniverseCfgUnit* pCfgUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(totallevel);
    pPlayer->GetLiveCtrl().Update();
    //if ((!uni.TestSeed(seed, chv, score) && !bBuyOpen) || pCfgUnit == NULL || pPlayer->GetLiveCtrl().GetLives() <= 0)
    //{
    //    pResponse->set_err(1);
    //    _M_Result_(false, json_result);
    //    return;
    //}

    bool bNowTop  = totallevel == uni.GetTotalLvl();


    pPlayer->FillBasicToJson(json_result["currentUser"]);

    if (LvlSucc)
    {
        uni.AddRank(totallevel, pUser->id(), score);
        uni.RebuildTopPlayerList();
    }
    else
    {
        pPlayer->GetLiveCtrl().CostLives();
    }
    json_result["score"] = score;

    uni.FillToRankJson(totallevel, json_result["levelToplist"]);
    int nStar = pCfgUnit->GetStarLvl(score);
    json_result["stars"] = nStar;
    json_result["bestResult"] = LvlSucc && uni.IsBest(totallevel, score);
    json_result["newStarLevel"] = LvlSucc && uni.IsBestStar(totallevel, nStar);



    Value ja_event (arrayValue);
    uni.FillToUnlockEvent(totallevel, score, ja_event);

    UniverseCtrl::MobileCacheLvlSet pMoblieCache  = uni.GetMobileCacheList();
    for (int i = uni.GetTotalLvl() - 1; i < pMoblieCache->size(); i++)
    {
        int lid = i + 1;
        if (pMoblieCache->data()[i] > 0)
        {
            Value j (arrayValue);
            uni.FillToUnlockEvent(lid, pMoblieCache->data()[i], j);
            uni.SubmitSocre(lid, pMoblieCache->data()[i] );
        }
        else
        {
            i = pMoblieCache->size() + 1;
        }
    }

    json_result["events"] = ja_event;

    json_result["episodeId"] =  pCfgUnit->episodeId;
    json_result["levelId"] = pCfgUnit->levelId;

    uni.FillToJson(json_result) ;
    if (LvlSucc)
    {

        uni.SubmitSocre(totallevel, score);

    }
    if (uni.IsDallyLevel(totallevel))
    {
        uni.AddRankDally(pUser->id(), uni.DallyTotalSocre());
        if (LvlSucc)
        {
        };
        uni.SubmitSocreForDally(totallevel, score, LvlSucc);

    }


    if (LvlSucc)
    {
        pPlayer->GetDailyCounter().AddCounter(DC_LVLUP, 1);
        pPlayer->GetDailyCounter().AddCounter(DC_SOCRE, score);

        std::string logname = "EndWin";
        if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel) != NULL)
        {
            logname = "DailyEndWin";
        }
        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pLog->LogItem("lvlid", totallevel)->
                LogItem("type", pCfgUnit->level_type)->
                LogItem("score", score)->
                LogItem("star", nStar)->
                LogItem("history best", uni.GetHistoryBestValue(totallevel))->
                LogItem("time", 0)->
                LogItem("ext_step", 0)->
                LogItem("from", "mobile")->
                LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume);
        if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel) != NULL)
        {
            logname = "DailyEndWin";
            pLog->LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume, LogCtrl::ST_Daily);

        }
        pLog->LogEnd(logname.c_str());

        if (bNowTop)
        {
            pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
                    LogItem("关卡id", totallevel)->
                    LogItem("尝试次数(1~xxx)", uni.GetFailCnt(totallevel) + 1)->
                    LogItem("是否是付费通关(1是;0不是)", 0)->
                    LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume)->
                    LogItem("from", "mobile")->

                    LogEnd("TryTimesPerLevel");

            pPlayer->GetDbPlayer()->mutable_universeinfo()->mutable_universe_set( totallevel - 1)->set_first_pass_from(1);
        }
    }
    else
    {
        std::string logname = "EndLose";

        LogCtrl* pLog = pPlayer->GetLogCtrl().LogBegin();
        pLog->LogItem("lvlid", totallevel)->
                LogItem("type", pCfgUnit->level_type)->
                LogItem("score", score)->
                LogItem("fail result", "unknowen")->
                LogItem("time", 0)->
                LogItem("buy win", 0)->
                LogItem("ext_step", 0)->
                LogItem("from", "mobile")->

                LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume);
        if (DallyUniverseMgr::GetInst()->GetCfgByTotalLvl(totallevel) != NULL)
        {
            logname = "DailyEndLose";
            pLog->LogSubType(LogCtrl::ST_Play, LogCtrl::ST_Consume, LogCtrl::ST_Daily);

        }
        pLog->LogEnd(logname.c_str());
        uni.AddFailCnt(totallevel);
    }
    _M_Result_(true, json_result);
}
//</editor-fold>

/******************************************************************************/
/*    Mobile 上报分数                                                         */
/******************************************************************************/
//<editor-fold desc="Mobile 上报分数">


//</editor-fold>


/******************************************************************************/
/*    刷新基础信息\                                                           */
/******************************************************************************/
//<editor-fold desc="刷新基础信息\">

void
PlayerService::poll(Event* e, User * pUser)
{

    _M_RequestInit_;
    Value data(objectValue);
    pPlayer->FillBasicToJson(data);
    _M_Result_(true, data);
}

void
PlayerService::pollMobile(Event* e, User* pUser)
{
    _M_RequestInit_;
    Value data(objectValue);
    pPlayer->FillBasicToJson(data);
    int nMaxLvl = pPlayer->GetUniverseCtrl().GetTotalLvl();
    Value lvls(arrayValue);

    for (int i = 0; i < nMaxLvl; i++)
    {
        DB_Universe* pInfo = pPlayer->GetUniverseCtrl().GetDBUniByTotalLvl(i + 1);
        int v = pInfo == NULL ? 0 : pInfo->value();
        lvls.append(v);
    }
    data["userUniverse"] = lvls;
    _M_Result_(true, data);
}
//</editor-fold>

/******************************************************************************/
/*    刷新游戏配置                                                            */
/******************************************************************************/
//<editor-fold desc="刷新游戏配置">

void
PlayerService::getGameConfigurations(Event* e, User * pUser)
{

    _M_RequestInit_;
    pResponse->set_jsonresult(UniverseMgr::GetInst()->gameConfigJson);
}
//</editor-fold>


/******************************************************************************/
/*    道具属性                                                                */
/******************************************************************************/
//<editor-fold desc="道具属性 ">

void
PlayerService::getCandyProperties(Event* e, User * pUser)
{

    _M_RequestInit_;
    Value data_result(objectValue);
    pPlayer->GetCandyProperties().FillToJson(data_result);
    _M_Result_(true, data_result);

}

void
PlayerService::setCandyProperty(Event* e, User * pUser)
{

    _M_RequestInit_;
    _M_DecodeAttr_(2);
    bool v = (bool) (String2Int(atts[0]) != 0);
    const std::string key = atts[1];
    Value data_result(objectValue);
    pPlayer->GetCandyProperties().SetProperties(key, v);
    pPlayer->GetCandyProperties().FillToJson(data_result);
    _M_Result_(true, data_result);
}
//</editor-fold>

void
PlayerService::getRecipes(Event* e, User * pUser)
{
}
/******************************************************************************/
/*    游戏消息 old                                                            */
/******************************************************************************/
//<editor-fold desc="游戏消息 old">

void
PlayerService::getMessages(Event* e, User * pUser)
{

    _M_RequestInit_;
    Value json;
    Value cu(objectValue);
    pPlayer->FillBasicToJson(cu);
    json["currentUser"] = cu;
    pPlayer->GetMessageCtrl().GetAllEventEffect(json);
    _M_Result_(true, json);
}
//</editor-fold>

/******************************************************************************/
/*    关卡排行榜                                                              */
/******************************************************************************/
//<editor-fold desc="关卡排行榜">

void
PlayerService::getLevelToplist(Event* e, User * pUser)
{

    _M_RequestInit_
    _M_DecodeAttr_(1);
    int totalLvl = String2Int(atts[0]);
    Value json_result(objectValue);
    pPlayer->GetUniverseCtrl().FillToRankJson(totalLvl, json_result);
    _M_Result_(true, json_result);

}
//</editor-fold>

/******************************************************************************/
/*    付费通关                                                                */
/******************************************************************************/
//<editor-fold desc="付费通关 ">

void
PlayerService::openLevel(Event* e, User * pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(2);
    int totalLvl = String2Int(atts[0]);
    int score = String2Int(atts[1]);


    bool bSucc = true;
    UniverseCfgUnit* pUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(totalLvl);
    score = pUnit->star_need[0];
    int nCost = OpenLevelMgr::GetInst()->GetCostNum(totalLvl);
    bSucc = bSucc && pPlayer->GetUniverseCtrl().GetTotalLvl() == totalLvl;
    bSucc = bSucc && pUnit != NULL;
    bSucc = bSucc && nCost > 0;
    bSucc = bSucc && pPlayer->GetBag().HasCash(nCost);
    bSucc = bSucc && pPlayer->GetBag().CostCash(nCost);
    //bSucc = bSucc && pPlayer->GetUniverseCtrl().FillToUnlockEvent(totalLvl,score,json_result);
    if (bSucc)
    {
        Event fe;
        std::string fake_attr = "";
        fake_attr += toString<int>(totalLvl) + "," + toString<int>(score) + ",";
        fake_attr += toString<int>(pPlayer->GetUniverseCtrl().AllocSeed()) + ",0,1,"; // 0  for status succ. 1 for buy open att
        int chv = GetPassRever(pPlayer->GetDbPlayer()->universeinfo().seed() & score);
        fake_attr += toString<int>(chv) + ",";
        fe.mutable_mce_mcegatewayfunction()->set_jsonattr(fake_attr);
        int nLife = pPlayer->GetLiveCtrl().GetLives();
        pPlayer->GetDbPlayer()->mutable_lives()->set_lives(nLife > 0 ? nLife : 1);
        PlayerService::gameEnd(&fe, pUser);
        pPlayer->GetDbPlayer()->mutable_lives()->set_lives(nLife);

        pResponse->set_jsonresult(fe.mse_msegatewayfunction().jsonresult());
        pUser->GetDbUser().set_ingame_cash(pUser->GetDbUser().ingame_cash() + nCost);

    }
    else
    {

        Value json_result(objectValue);
        _M_Result_(bSucc, json_result);
    }
}
//</editor-fold>

void
PlayerService::reportFramerate(Event* e, User * pUser)
{
}
/******************************************************************************/
/*    声音 音效                                                               */
/******************************************************************************/
//<editor-fold desc="声音 音效">

void
PlayerService::setSoundFx (Event* e, User * pUser)
{

    _M_RequestInit_;
    _M_DecodeAttr_(1);
    bool bSetValue = (String2Int(atts[0]) != 0);
    if (atts[0] == "false")
    {
        bSetValue = false;
    }

    pUser->GetDbUser().mutable_player()->mutable_option()->set_sound(bSetValue);
    Value json(objectValue);
    _M_Result_(true, json);
}

void
PlayerService::setSoundMusic(Event* e, User * pUser)
{

    _M_RequestInit_;
    _M_DecodeAttr_(1);
    bool bSetValue = (String2Int(atts[0]) != 0);
    if (atts[0] ==  "false")
    {
        bSetValue = false;
    }
    pUser->GetDbUser().mutable_player()->mutable_option()->set_music(bSetValue);
    Value json(objectValue);
    _M_Result_(true, json);
}
//</editor-fold>

/******************************************************************************/
/*    解锁标记                                                                */
/******************************************************************************/
//<editor-fold desc="解锁标记 ">

void
PlayerService::unlockItem(Event* e, User * pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    type_id tid = String2Int(atts[0]);
    bool bSucc = pPlayer->GetBag().MarkItemUnlock(tid);
    Value json_result(objectValue);
    if (false && !bSucc)
    {
        json_result["msg"] = "fail!";
    }
    else
    {

        pPlayer->GetBag().FillToJson(json_result);
    }
    pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
            LogItem("lvlid", "UnlockItem")->
            LogItem("lvltype", tid )->
            LogEnd("Debug");
    _M_Result_(true, json_result);
}
//</editor-fold>

void
PlayerService::handOutItemWinnings (Event* e, User * pUser)
{
    // php代码里直接return了. 
}
/******************************************************************************/
/*    游戏中消耗道具                                                          */
/******************************************************************************/
//<editor-fold desc="游戏中消耗道具">

void
PlayerService::useItemsInGame(Event* e, User * pUser)
{
    _M_RequestInit_;
    std::string str = request.jsonattr();
    std::vector<string> atts;
    static string delims = ";";
    tokenize(str, atts, delims);
    bool bSucc = true;
    //<editor-fold desc="log data"> 
    std::string log_lvl_type = "";
    UniverseCfgUnit* pCfgUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(pPlayer->GetUniverseCtrl().GetNowPlayingLvl());
    if (pCfgUnit != NULL)
    {
        log_lvl_type = pCfgUnit->level_type;
    }
    //</editor-fold>

    for (int i = 0; bSucc && i < atts.size(); i++)
    {
        std::vector<string> subatts;
        static string delims_sub = ",";

        tokenize(atts[i], subatts, delims_sub);
        if (subatts.size() > 1)
        {
            type_id tid = String2Int(subatts[0]);

            int num = 1;
            if (subatts.size() >= 2)
            {
                num = String2Int(subatts[1]);
            }
            if ( tid == ItemManager::GetKingOfStarTicketTypeId())
            {
                num = KingOfStar::GetInst()->GetEnterLevelNeedItemCnt(pPlayer, pPlayer->GetUniverseCtrl().GetNowPlayingLvl());
            }
            if (num <= 0)
            {

                num = 1;
            }
            bSucc = bSucc && pPlayer->GetBag().CostItem(tid, num);
            if (bSucc)
            {
                pPlayer->GetDailyCounter().AddCounter(DC_USE_ITEM, 1);
                if (tid == ItemManager::GetKingOfStarTicketTypeId())
                {
                    pPlayer->GetDbPlayer()->mutable_kingofstar()->set_now_playing(pPlayer->GetUniverseCtrl().GetNowPlayingLvl());
                    KingOfStar::GetInst()->OnPlayerEnterLevel(pPlayer, pPlayer->GetUniverseCtrl().GetNowPlayingLvl());
                }
            }
            pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
                    LogItem("lvlid", pPlayer->GetUniverseCtrl().GetNowPlayingLvl())->
                    LogItem("lvltype", log_lvl_type)->
                    LogItem("lvlid", tid)->
                    LogSubType(LogCtrl::ST_Consume)->
                    LogEnd("UseBooster");
        }
    }
    Value json_result(objectValue);
    pPlayer->GetBag().FillToJson(json_result);

    _M_Result_(bSucc, json_result);
}

void
PlayerService::useTicket(Event* e, User * pUser)
{
    // 客户端没有调用. 
}
//</editor-fold>
/******************************************************************************/
/*    手机客户端独有消息                                                      */
/******************************************************************************/
//<editor-fold desc="手机客户端独有消息">

void
PlayerService::useItemsInGameMobile(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(2);
    type_id tid = String2Int(atts[0]);
    int      num = String2Int(atts[1]);
    int      log_num = num;
    bool bSucc = true;
    std::string log_lvl_type = "";
    UniverseCfgUnit* pCfgUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(pPlayer->GetUniverseCtrl().GetNowPlayingLvl());
    if (pCfgUnit != NULL)
    {
        log_lvl_type = pCfgUnit->level_type;
    }
    int nHaveNum = pPlayer->GetBag().GetItemCnt(tid);
    if (nHaveNum < num)
    {
        num = nHaveNum;
    }
    if (tid == ItemManager::GetLiveMaxChangeTypeId()
            || tid == 18
            || tid == 3 )
    {
        pPlayer->GetBag().AddItem(tid, 1);
        bSucc = true;
    }
    else
    {
        bSucc = bSucc && pPlayer->GetBag().CostItem(tid, num);
    }

    pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
            LogItem("lvlid", pPlayer->GetUniverseCtrl().GetNowPlayingLvl())->
            LogItem("lvltype", log_lvl_type)->
            LogItem("lvlid", tid)->
            LogItem("lvlid", num)->
            LogItem("lvlid", log_num)->

            LogSubType(LogCtrl::ST_Consume)->
            LogEnd("UseBoosterMobile");

    Value json_result(objectValue);
    pPlayer->GetBag().FillToJson(json_result);
    _M_Result_(bSucc, json_result);
}

void
PlayerService::RemoteLog(Event* e, User* pUser)
{
    _M_RequestInit_;
    pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
            LogItem("Detail", request.jsonattr().c_str())->
            LogEnd("RemoteLog");
    int n = 0;
    _M_Result_(true, n);
}

void
PlayerService::UnlockItemMobile(Event* e, User* pUser)
{
    unlockItem(e, pUser);
}

void
PlayerService::AddMobileDevice(Event* e, User* pUser)
{
    _M_RequestInit_;
    int v = pPlayer->GetDbPlayer()->mobiledevicesynccnt() + 1;
    pPlayer->GetDbPlayer()->set_mobiledevicesynccnt( v );
    pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
            LogItem("cnt", v)->
            LogEnd("MobileDeviceAdd");
    _M_Result_(true, v);

}

void
PlayerService::getLvlSocres(Event* e, User* pUser)
{
    _M_RequestInit_;
    int nMaxLvl = pPlayer->GetUniverseCtrl().GetTotalLvl();
    Value lvls(arrayValue);

    for (int i = 0; i < nMaxLvl; i++)
    {
        DB_Universe* pInfo = pPlayer->GetUniverseCtrl().GetDBUniByTotalLvl(i + 1);
        int v = pInfo == NULL ? 0 : pInfo->value();
        lvls.append(v);
    }
    _M_Result_(true, lvls);

}
//</editor-fold>

/******************************************************************************/
/*    单独获取道具数量                                                        */
/******************************************************************************/
//<editor-fold desc="单独获取道具数量">

void
PlayerService::getItemAount(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    type_id tid = String2Int(atts[0]);
    int      num = pPlayer->GetBag().GetItemCnt(tid);
    bool  bSucc = true;

    _M_Result_(bSucc, num);
}
//</editor-fold>

/******************************************************************************/
/*    好友交互: 赠送/请求体力                                                 */
/******************************************************************************/
//<editor-fold desc="好友交互: 赠送/请求体力">

void
PlayerService::request(Event* e, User * pUser)
{
    _M_RequestInit_;
    Value json_atts = parseJsonStr(request.jsonattr());
    //for (int i = 0; i < json_atts.size() && i < 1; i++)
    //{

    int64 llTargetUid = llInvalidId;
    std::string oid = "";
    int ns  = 0;
    bool bHasUid = true;
    bool bHasOid = true;
    if (json_atts.isArray() )
    {
        json_atts[ns]["sender_oid"] = pPlayer->GetUser()->platform_id();
        e->mutable_mce_mcegatewayfunction()->set_jsonattr(json_atts.toStyledString());
    }
    if (!json_atts.isArray() || !HasJsonSubMember(json_atts[ns], "player_id") || !json_atts[ns]["player_id"].isString())
    {
        bHasUid = false;
    }
    else
    {
        safe_atoll(json_atts[ns]["player_id"].asString(), llTargetUid);
    }

    if (!json_atts.isArray() || !HasJsonSubMember(json_atts[ns], "player_oid") || !json_atts[ns]["player_oid"].isString())
    {
        bHasOid = false;
    }
    else
    {
        oid = json_atts[ns]["player_oid"].asString();
    }

    if ( (!bHasOid && !bHasUid))
    {
        return;
    }
    //Value jtar = parseJsonStr(json_atts[ns]["player_id"].asString());
    //if (!jtar.isArray() || jtar.isMember("player_id"))
    if (bHasUid && ! bHasOid)
    {
        if (HasJsonSubMember(json_atts[ns], "is_answer"))
        {
            if (pPlayer->GetMessageCtrl().IsToadyAlreadySendGift_AnswerRequest(llTargetUid))
            {
                _M_Err_;

                return;
            }
        }
        else
        {
            if (pPlayer->GetMessageCtrl().IsToadyAlreadySendGift(llTargetUid))
            {
                _M_Err_;

                return;
            }
        }
        pPlayer->SetForward(e, llTargetUid);
        pPlayer->GetLogCtrl().LogBegin()->LogItem("", llTargetUid)->LogItem("请求类型", "GiftLife")->LogItem("", ItemManager::GetLiveAddTypeId())->LogEnd("request_uid");

        pPlayer->GetDailyCounter().AddCounter(DC_SENDLIFE, 1);
    }

    if (bHasOid)
    {
        pPlayer->SetForward(e, oid);
        pPlayer->GetLogCtrl().LogBegin()->LogItem("", llTargetUid)->LogItem("请求类型", "GiftLife")->LogItem("", ItemManager::GetLiveAddTypeId())->LogEnd("request_oid");

        pPlayer->GetDailyCounter().AddCounter(DC_ASKLIFE, 1);

        pPlayer->GetMessageCtrl().AddGiftLog_AskList(oid);
    }


    if (HasJsonSubMember(json_atts[ns], "is_answer"))
    {
        pPlayer->GetMessageCtrl().AddGiftLog_AnswerRequest(llTargetUid);
    }
    else
    {
        pPlayer->GetMessageCtrl().AddGiftLog(llTargetUid);
    }

    //}
}

void
PlayerService::request_taker(Event* e, User * pUser)
{
    _M_RequestInit_;
    Value json_atts = parseJsonStr(request.jsonattr());
    int ns  = 0;
    bool bOid = false;
    if (!json_atts.isArray() || !HasJsonSubMember(json_atts[ns], "player_id") || !json_atts[ns]["player_id"].isString())
    {
        bOid = true;
        // return;
    }
    pPlayer->GetDailyCounter().AddCounter(DC_GETGIFT, 1);
    //Value jtar = parseJsonStr(json_atts[ns]["player_id"].asString());
    //if (!jtar.isArray() || jtar.isMember("player_id"))
    // safe_atoll(json_atts[ns]["player_id"].asString(), llTargetUid);
    json_atts[ns]["name"] = e->forwardinfo().sendername();
    json_atts[ns]["url"] = e->forwardinfo().senderurl();
    pPlayer->GetMessageCtrl().AddLifeGift(e->uid(), json_atts[ns].toStyledString());
    pPlayer->GetLogCtrl().LogBegin(LT_logTypeNoSend)->
            LogItem("", pPlayer->GetUniverseCtrl().GetNowPlayingLvl())->
            LogItem("", (int64) (e->uid()))->
            LogItem("", e->forwardinfo().sendername().c_str() )->
            LogEnd("request_taker");
}

void
PlayerService::request_sender_after(Event* e, User * pUser)
{

    _M_RequestInit_;
    Value json_result(objectValue);
    json_result["data"] = 1;
    _M_Result_(true, json_result);
}
//</editor-fold>

/******************************************************************************/
/*    flash add daily counter                                                 */
/******************************************************************************/
//<editor-fold desc="flash add daily counter">

void
PlayerService::FlashAddDailyCounter(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(2);
    std::string key = "c_" + atts[0] + "_"  + atts[1];
    pPlayer->GetDailyCounter().AddCounter(key);
    int nValue = pPlayer->GetDailyCounter().GetCounter(key);
    _M_Result_(true, nValue);
}
//</editor-fold>

void
PlayerService::getGift(Event* e, User * pUser)
{
}

void
PlayerService::getMyStarsRecord(Event* e, User * pUser)
{
}

void
PlayerService::updateGuide(Event* e, User * pUser)
{
}

void
PlayerService::buyGoods(Event* e, User * pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(7);
    //PHP:buyGoods($itemId, $itemType, $appmode = 1, $costType = 'LB', $pfkey = null, $imgurl = null, $num = 1)

    type_id tid = String2Int(atts[0]);
    int type     = String2Int(atts[1]);
    int num      = String2Int(atts[2]) ;
    int nCostNum = 0;
    //pItemCfg->nCostNum* num;

    //<editor-fold desc="log data"> 
    std::string log_lvl_type = "";
    UniverseCfgUnit* pCfgUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(pPlayer->GetUniverseCtrl().GetNowPlayingLvl());
    if (pCfgUnit != NULL)
    {
        log_lvl_type = pCfgUnit->level_type;
    }
    int log_live_add = 0;
    //</editor-fold>


    if (type == ItemCfgMgr::GetBuyTypeLive())
    {
        num = 5;
        nCostNum = num * ItemCfgMgr::GetLiveCashPrice();
        tid = ItemCfgMgr::GetLiveAddTypeId();
        log_live_add = num;
    }
    else if (type == ItemCfgMgr::GetBuyTypeGoods())
    {
        if (num <= 0)
        {
            num = 1;
        }
        ItemCfgUnit* pItemCfg = (ItemCfgUnit*) ItemCfgMgr::GetInst()->GetUnit(tid);
        if (pItemCfg == NULL)
        {
            _M_Err_;
            return;
        }
        nCostNum = pItemCfg->nCostNum* num;
    }
    if (nCostNum <= 0 || !pPlayer->GetBag().HasCash( nCostNum) || nCostNum == 0 || !pPlayer->GetBag().CostCash(nCostNum))
    {
        _M_Err_;

        return;
    }

    pUser->GetDbUser().set_ingame_cash(pUser->GetDbUser().ingame_cash() + nCostNum);
    ItemCfgUnit* pItemCfg = (ItemCfgUnit*) ItemCfgMgr::GetInst()->GetUnit(tid);
    if (pItemCfg != NULL && !pItemCfg->SubItemSet.IsEmpty())
    {
        pPlayer->GetBag().AddItemSet( &(pItemCfg->SubItemSet));
    }
    else
    {
        pPlayer->GetBag().AddItem(tid, num);
    }
    Value json_result;
    pPlayer->FillBasicToJson(json_result);
    _M_Result_(true, json_result);

    pPlayer->GetDailyCounter().AddCounter(DC_BUY_ITEM, 1);
    pPlayer->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
            LogItem("lvlid", pPlayer->GetUniverseCtrl().GetNowPlayingLvl())->
            LogItem("lvltype", log_lvl_type)->
            LogItem("lvlid", tid)->
            LogItem("nCostNum", nCostNum)->
            LogItem("nItemNum", num)->
            LogSubType(LogCtrl::ST_Consume, LogCtrl::ST_Get)->
            LogEnd("BuyBooster");
}

void
PlayerService::getFriendsData(Event* e, User * pUser)
{
}

void
PlayerService::getBalance(Event* e, User * pUser)
{

    _M_RequestInit_;
    Value json_result;
    pPlayer->GetBag().FillToJson(json_result);
    _M_Result_(true, json_result["itemBalance"]);
}

void
PlayerService::getSignin(Event* e, User * pUser)
{
}

void
PlayerService::getFanhua(Event* e, User * pUser)
{
}

void
PlayerService::getFanhuanReward(Event* e, User * pUser)
{
}

void
PlayerService::getTodayCost(Event* e, User * pUser)
{
}

/**
 * 合成配方
 * @param int $formulaId
 */

void
PlayerService::stuffReward(Event* e, User * pUser)
{
    //todo :
    //客户端未发现调用
}
/******************************************************************************/
/*    Cdkey 奖励                                                              */
/******************************************************************************/
//<editor-fold desc="Cdkey 奖励">

void
PlayerService::getCDkeyReward(Event* e, User * pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    std::string cdkey = atts[0];
    BitCache bc(pPlayer->GetUser()->GetDbUser().mutable_player()->mutable_cdkey_pickhistory());
    Value json(objectValue);
    bool bSucc = true;

    if (cdkey == "NewYear2013")
    {
        const int nBitPos = 0;
        if (!bc.HasBit(nBitPos) && Clock::GetSecond() < GetTimeVal("2013-01-30-12:00:00"))
        {
            bc.SetBit(nBitPos, true);
            pPlayer->GetBag().AddItem(ItemManager::GetLiveAddTypeId(), 60);
            pPlayer->GetBag().AddItem(5, 8);
            pPlayer->GetBag().AddItem(2, 5);
            pPlayer->GetBag().AddItem(17, 5);
            pPlayer->GetLogCtrl().LogBegin()->
                    LogItem("live", pPlayer->GetLiveCtrl().GetLives())->
                    LogItem("bird", pPlayer->GetBag().GetItemCnt(5))->
                    LogItem("bottle", pPlayer->GetBag().GetItemCnt(2))->
                    LogItem("move", pPlayer->GetBag().GetItemCnt(17))->
                    LogEnd("cdkey_0130");
            pPlayer->FillBasicToJson(json["currentUser"]);
            pPlayer->GetBag().FillToJson(json);
        }

        else
        {
            bSucc = false;
            json["msg"] = "CdKey check failed!";
        }
    }
    else if (cdkey == "HappySpring2013")
    {
        const int nBitPos = 1;
        if (!bc.HasBit(nBitPos) && Clock::GetSecond() < GetTimeVal("2013-03-25-23:59:00"))
        {
            bc.SetBit(nBitPos, true);
            //25点体力、5个炸弹糖、3个摇摇瓶、3个黏黏爪
            pPlayer->GetBag().AddItem(ItemManager::GetLiveAddTypeId(), 25);
            pPlayer->GetBag().AddItem(ItemManager::GetBombTypeId(), 5);
            pPlayer->GetBag().AddItem(ItemManager::GetShockBottleTypeId(), 3);
            pPlayer->GetBag().AddItem(ItemManager::GetFouceMoveTypeId(), 3);
            pPlayer->GetLogCtrl().LogBegin()->
                    LogItem("25点体力", pPlayer->GetLiveCtrl().GetLives())->
                    LogItem("5个炸弹糖", pPlayer->GetBag().GetItemCnt(ItemManager::GetBombTypeId()))->
                    LogItem("3个摇摇瓶", pPlayer->GetBag().GetItemCnt(ItemManager::GetShockBottleTypeId()))->
                    LogItem("3个黏黏爪", pPlayer->GetBag().GetItemCnt(ItemManager::GetFouceMoveTypeId()))->
                    LogEnd("cdkey_HappySpring2013");
            pPlayer->FillBasicToJson(json["currentUser"]);
            pPlayer->GetBag().FillToJson(json);
        }
        else
        {
            bSucc = false;
            json["msg"] = "CdKey check failed!";
        }
    }
    else if (cdkey == "HUANGZUAN1306")
    {
        const int nBitPos = 2;
        int nLvl = pPlayer->GetUser()->getYellowDmdLv() - 1;
        if (pPlayer->GetActiveCtrl().IsNewUserFromDate("2013-7-1-0:0:0"))
        {
            nLvl = 0;
        }
        if (nLvl > 0 && !bc.HasBit(nBitPos) && Clock::GetSecond() < GetTimeVal("2013-07-06-0:0:00"))
        {
            bc.SetBit(nBitPos, true);
            for (int i = 0; i < 3; i++)
            {
                ItemSet* pItemSet = pPlayer->GetActiveCtrl().GetPlayerYellDmdRewardConfig(nLvl, false);
                pPlayer->GetBag().AddItemSet(pItemSet);
                if (pPlayer->GetUser()->isYellowDmdYear())
                {
                    ItemSet* pItemSetYear = pPlayer->GetActiveCtrl().GetPlayerYellDmdRewardConfig(nLvl, pPlayer->GetUser()->isYellowDmd());
                    pPlayer->GetBag().AddItemSet(pItemSetYear);

                }
                pPlayer->FillBasicToJson(json["currentUser"]);
                pPlayer->GetBag().FillToJson(json);

            }
            pPlayer->GetLogCtrl().LogBegin()->
                    LogItem("25点体力", nLvl)->
                    LogEnd("HUANGZUAN1306");

        }
        else
        {
            bSucc = false;
            json["msg"] = "CdKey check failed!";
        }
    }
    else
    {
        bSucc = false;
    }
    _M_Result_(bSucc, json);
}
//</editor-fold>

void
PlayerService::getSessionId(Event* e, User * pUser)
{

    _M_RequestInit_;
    pResponse->set_jsonresult(toString<int64 > (pPlayer->GetUid()));
    //_M_Result_
}

/******************************************************************************/
/*    Web 好友数据                                                            */
/******************************************************************************/
//<editor-fold desc="Web 好友数据">

void
PlayerService::LoadFriendForMobile(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    Value json_result(objectValue);
    const std::string friend_openid = atts[0];
    int64 lltargetUid = llInvalidId;
    if (request.actionname() == "FriendDataUid")
    {
        safe_atoll(atts[0], lltargetUid);
    }
    json_result["data"];
    if (pUser->platform_id() == friend_openid || lltargetUid == pUser->id())// action taker
    {
        pPlayer->FillBasicToJson(json_result);
        //pPlayer->GetUniverseCtrl().FillToJson(json_result, false) ;
    }
    else if (e->mutable_mse_msegatewayfunction()->jsonresult().size() > 0
            )
    {
        return;
    }
    else
    {
        if (lltargetUid != llInvalidId)
        {
            pUser->GetPlayer()->SetForward(e, lltargetUid);
        }
        else
        {
            pUser->GetPlayer()->SetForward(e, friend_openid);
        }
    }
    _M_Result_(true, json_result);
}
//</editor-fold>

/******************************************************************************/
/*    Map logic                                                               */
/******************************************************************************/
//<editor-fold desc="Map logic">

void
PlayerService::Map_MoveItem(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(4);
    item_only_id oid  = String2Int(atts[0]);
    int x = String2Int(atts[1]);
    int y = String2Int(atts[2]);
    int d = String2Int(atts[3]);

    RoomCtrl& rc = pUser->GetPlayer()->GetRoomCtrl();
    bool bSucc = rc.MoveItem(oid, x, y, d);
    if (!bSucc)
    {
        rc.MovieItemOnWall(oid, x, y, d);
    }
    pPlayer->GetLogCtrl().LogBegin()->
            LogItem ("atts", request.jsonattr().c_str())->
            LogItem("succ", bSucc ? 1 : 0)->
            LogEnd("Room_MoveItem");
    Value json(objectValue);
    _M_Result_(bSucc, json);

}

void
PlayerService::Map_PeekItem(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    item_only_id oid  = String2Int(atts[0]);

    RoomCtrl& rc = pUser->GetPlayer()->GetRoomCtrl();
    bool bSucc =  rc.PeekItem(oid);
    if (!bSucc)
    {
        bSucc = rc.PeekItemOnWall(oid);
    }

    Value json(objectValue);
    _M_Result_(bSucc, json);
    pPlayer->GetLogCtrl().LogBegin()->
            LogItem ("atts", request.jsonattr().c_str())->
            LogItem("succ", bSucc ? 1 : 0)->
            LogEnd("Room_PeekItem");
    UserCtrl uc(pPlayer->GetUser());
    uc.SendMapBag();
}

void
PlayerService::Map_PutItem(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    for (int i = 0; i < 3; i++)
    {
        atts.push_back("0");
    }
    type_id       tid = String2Int(atts[0]);
    int             x   = String2Int(atts[1]);
    int             y   = String2Int(atts[2]);
    int             d   = String2Int(atts[3]);
    bool bSucc = true;
    Value json(objectValue);

    RoomCtrl& rc = pUser->GetPlayer()->GetRoomCtrl();

    bool bWallItem = false;
    ItemOnMapCfgUnit* pCfg = (ItemOnMapCfgUnit*) ItemManager::GetInst()->GetMapItemConfig(tid);
    if (pCfg == NULL)
    {
        _M_Result_(false, json);
        return;
    }
    if (pCfg->iom_type == ItemOnMapCfgUnit::IOMT_WALL)
    {
        bSucc = rc.PutItemOnWall(tid, x, y, d);
    }
    else if (pCfg->iom_type == ItemOnMapCfgUnit::IOMT_WALLPAPER)
    {
        bSucc = rc.PutWallpaper(tid);
    }
    else if (pCfg->iom_type == ItemOnMapCfgUnit::IOMT_FLOOR)
    {
        bSucc = rc.PutFloor(tid);
    }
        //    else if (pCfg->iom_type == ItemOnMapCfgUnit::IOMT_WALLPAPER)
        //    {
        //
        //    }
    else
    {
        bSucc = rc.PutItem(tid, x, y, d);
    }
    json["id"] = rc.GetLatestItemOnlyId();


    pPlayer->GetLogCtrl().LogBegin()->
            LogItem ("atts", request.jsonattr().c_str())->
            LogItem("succ", bSucc ? 1 : 0)->
            LogEnd("Room_PutItem");
    _M_Result_(bSucc, json);

    UserCtrl uc(pPlayer->GetUser());
    uc.SendMapBag();
}

void
PlayerService::Map_MapDetail(Event* e, User* pUser)
{
    _M_RequestInit_;
    Value json;
    pPlayer->GetRoomCtrl().FillAsMapDetail(json);
    pPlayer->GetLimit().AddLimit(ActiveCtrl::lidRoomSystemFirstEnter, 1);
    _M_Result_(true, json);
}

void
PlayerService::Map_Bag(Event* e, User* pUser)
{
    _M_RequestInit_;
    Value json(objectValue);
    pPlayer->GetBag().FillMapItemToJson(json);

    _M_Result_(true, json);
}

void
PlayerService::Map_SyncMapItem(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    type_id targetId = String2Int(atts[0]);
    Value json(objectValue);
    bool bSucc = true;
    ItemOnMapCfgUnit* pCfg = (ItemOnMapCfgUnit*) ItemManager::GetInst()->GetMapItemConfig(targetId);
    if (pCfg == NULL)
    {
        _M_Result_(false, json);
        return;
    }

    bool bCost = pPlayer->GetBag().CostItemSet(&(pCfg->build_need));
    if (bCost)
    {
        pPlayer->GetBag().AddItem(targetId, 1);
    }
    bSucc = bCost;
    _M_Result_(bSucc, json);
    pPlayer->GetLogCtrl().LogBegin()->
            LogItem ("atts", request.jsonattr().c_str())->
            LogItem("succ", bSucc ? 1 : 0)->
            LogEnd("Room_SyncMapItem");
    UserCtrl uc(pPlayer->GetUser());
    uc.SendMapBag();
}

void
PlayerService::Map_SyncMaterial(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    type_id targetId = String2Int(atts[0]);
    Value json(objectValue);

    bool bSucc = true;
    MaterialCfgUnit* pCfg = (MaterialCfgUnit*) ItemManager::GetInst()->GetMapMaterialConfig(targetId);
    if (pCfg == NULL)
    {
        _M_Result_(false, json);
        return;
    }
    bool bCost = pPlayer->GetBag().CostItemSet(&(pCfg->build_need));
    if (bCost)
    {
        pPlayer->GetBag().AddItem(targetId, 1);
    }
    bSucc = bCost;
    _M_Result_(bSucc, json);
    pPlayer->GetLogCtrl().LogBegin()->
            LogItem ("atts", request.jsonattr().c_str())->
            LogItem("succ", bSucc ? 1 : 0)->
            LogEnd("Room_SyncMaterial");
    UserCtrl uc(pPlayer->GetUser());
    uc.SendMapBag();
}

void
PlayerService::Map_GetEggItem(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(1);
    int nIndex = String2Int(atts[0]);
    type_id tid = pPlayer->GetUniverseCtrl().GetEggItem(nIndex);
    if (tid > 6000)
    {
        //pPlayer->GetBag().AddItem(tid, 1);
        pPlayer->GetDbPlayer()->mutable_universeinfo()->add_egg_logic(tid);
    }
    Value json(objectValue);
    _M_Result_(true, json);
}

void
PlayerService::Map_GetDailyRewardByLuxcy(Event* e, User* pUser)
{
    _M_RequestInit_;
    bool bSucc = false;
    Value json(objectValue);
    json["data"] = 0;

    bool bCanGetReward = Clock::GetSecond() - pPlayer->GetDbPlayer()->mutable_room()->reward_time() >= 24 * 3600;
    bCanGetReward = bCanGetReward && !Clock::NowAfter("2013-8-1-0:0:0");
    if (bCanGetReward)
    {
        int lvl = pPlayer->GetRoomCtrl().GetDecLvl();
        if (lvl > 0)
        {
            pPlayer->GetDbPlayer()->mutable_room()->set_reward_time( Clock::GetSecond());
            int nLiveAdd = lvl * 2 + 1;
            pPlayer->GetBag().AddItem(ItemManager::GetLiveAddTypeId(), nLiveAdd);
            UserCtrl uc(pPlayer->GetUser());
            uc.SendPoll();
            bSucc = true;
            uc.SendMapBag();
            json["data"] = nLiveAdd;
            pPlayer->GetLogCtrl().LogBegin()->
                    LogItem ("atts", nLiveAdd)->
                    LogEnd("Room_GetDailyRewardByLuxcy");
        }
    }

    _M_Result_(bSucc, json);

}

void
PlayerService::Map_GetDropLvl(Event* e, User* pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(2);
    type_id targetId = String2Int(atts[0]);
    int num = String2Int(atts[1]);

    bool bSucc = false;
    Value json(objectValue);

    MaterialCfgUnit* pMater =  (MaterialCfgUnit*) ItemManager::GetInst()->GetMapMaterialConfig(targetId);
    Value lvls(arrayValue);
    json["price"] = 0;

    if (pMater != NULL)
    {
        int nMax = 3;
        int nCnt = 0;
        char key[64];
        for (int nTotalLvl = pPlayer->GetLevel(); nTotalLvl > 0 && nCnt < nMax; nTotalLvl--)
        {
            UniverseCfgUnit* pUni = UniverseMgr::GetInst()->GetUniByTotalLvl_QK(nTotalLvl);
            if (pUni != NULL &&  pMater->IsDropInEspoid(pUni->episodeId))
            {
                sprintf( key, "%s_%d", DC_EGG_BUILD_LVL, nTotalLvl);
                if (pPlayer->GetDailyCounter().GetCounter(key) == 0)
                {
                    nCnt ++ ;
                    lvls.append(nTotalLvl);
                }
            }
        }
        json["price"] = pMater->nPrice * num;
    }
    json["levels"] = lvls;

    _M_Result_(bSucc, json);
}
//</editor-fold>


/******************************************************************************/
/*    玩家数据获取                                                            */
/******************************************************************************/
//<editor-fold desc="玩家数据获取">

/**
 * 尝试获取玩家数据. 自动从CMEM中比较版本
 * @param e
 * @param pUser
 */
void
PlayerService::TryLoad(Event* e, User * pUser)
{
    bool bTotalNew = false;
    if (pUser != NULL &&  pUser->GetPlayer()->GetUniverseCtrl().GetTotalLvl() <= 1 && pUser->GetDbUser().user_cmem_v() < 10)
    {
        UserCtrl::GetEventHandler()->getDataHandler()->TryLoadUserFromCMem(pUser, 2);
        bTotalNew = true;
    }
    if (!bTotalNew && pUser->GetDbUser().user_cmem_v() < 15)
    {
        static CMEMReader sgCReader;
        std::string json_universe = "";
        std::string json_universe_decoder  = "";
        sgCReader.GetModel("universe", pUser->platform_id(), json_universe);
        phpdecoder::DecoderForm(json_universe.c_str(), 0, json_universe.length(), json_universe_decoder);
        Value juni = parseJsonStr(json_universe_decoder);
        if (HasJsonSubMember(juni, "episodes"))
        {
            int i = pUser->ParseUniverseFromJson(juni["episodes"]);
        }
        Value testobj(objectValue);
        pUser->GetDbUser(). set_user_cmem_v(16);
    }
    if (!bTotalNew && pUser->GetDbUser().user_cmem_v() < 17)
    {
        static CMEMReader sgCReader;
        std::string json_itembalance = "";
        std::string json_itembalance_decoder  = "";
        sgCReader.GetModel("itembalance", pUser->platform_id(), json_itembalance);
        if (json_itembalance.find(";N;") != json_itembalance.npos)
        {
            phpdecoder::DecoderForm(json_itembalance.c_str(), 0, json_itembalance.length(), json_itembalance_decoder);
            Value jbag = parseJsonStr(json_itembalance_decoder);
            if (HasJsonSubMember(jbag, "balance"))
            {

                pUser->ParseBagFromJson(jbag["balance"]);
            }
        }
        pUser->GetDbUser(). set_user_cmem_v(18);
    }
}
//</editor-fold>

/******************************************************************************/
/*    活动相关操作                                                            */
/******************************************************************************/
//<editor-fold desc="活动相关操作 ">

/**
 * 处理活动消息
 *  包括黄钻. 签到. 排行榜. 邀请好友. 次日留存. 微博收听等游戏右边栏的轰动
 * 每个活动都包含 nAid 和 nType两个参数
 * nAid标示了具体是哪个活动
 * nType 表示了详细的操作
 * 通常 nType == -1 表示请求这个活动的数据
 * @param e
 * @param pUser
 */
void
PlayerService::action(Event* e, User * pUser)
{
    _M_RequestInit_;
    _M_DecodeAttr_(2);
    int nAid = String2Int(atts[0]);
    int nType = String2Int(atts[1]);
    int nSubType = -1;
    if (atts.size() >= 3)
    {
        nSubType = String2Int(atts[2]);
    }
    Value json(objectValue);
    bool bSucc = true;
    if (nType != -1)
    {
        pPlayer->GetLogCtrl().LogBegin(LT_logTypeNoSend)->LogItem("AID", nAid)->LogItem("TYPE", nType)->LogEnd("action");
    }
    switch (nAid)
    {
        case ActiveCtrl::aidActiveList:
        {
            pPlayer->GetActiveCtrl().DealActiveList(nType, json);
        }
            break;
        case 6: // 6 for yell dmd
        {
            if (nType == 0)
            {
                pPlayer->GetActiveCtrl().AcceptYellDmdGiftNewer();
            }
            else
            {
                pPlayer->GetActiveCtrl().AcceptYellDmdGift();
            }
        }
            break;
        case 7: // 7 for 每日签到
        {
            if (nType == -1) // nType = -1 means send info
            {
                time_t timep = Clock::GetSecond();
                struct tm *p;
                time(&timep);
                p = localtime(&timep);
                int m = p->tm_mon + 1;
                int d = p->tm_mday;
                pPlayer->GetActiveCtrl().SetSignMonth(m);
                pPlayer->GetActiveCtrl().FillSignedToJson(json);
            }
            else if (nType >= 1 && nType <= 6) // 1 for login total reward
            {
                pPlayer->GetActiveCtrl().AcceptLoginReward(nType);
                pPlayer->GetActiveCtrl().FillSignedToJson(json);
            }
            else if (nType == 0)
            {
                pPlayer->OnSigned();
                time_t timep = Clock::GetSecond();
                struct tm *p;
                time(&timep);
                p = localtime(&timep);
                int m = p->tm_mon + 1;
                int d = p->tm_mday;
                pPlayer->GetActiveCtrl().SetSignMonth(m);
                pPlayer->GetActiveCtrl().AddSignHistory(d, true);
                pPlayer->GetActiveCtrl().AcceptSign();
            }
        }
            break;
        case 12: // 周排行
        {
            if (nType > 100)
            {
                bSucc =   pPlayer->GetActiveCtrl().DealRankReawrd(nType - 1, json);
            }
            else
            {
                int rid = nType + 1 + (13 - nAid)*6 ;
                int nWeek = Clock::GetWeek(Clock::GetDay()) - 1;
                Rank* pRankLastWeek = RankInfoMgr::GetInst()->GetRankWeek(nType, nWeek);
                if (pRankLastWeek != NULL)
                {
                    Value jRank (objectValue);
                    jRank["id"] = rid;
                    pRankLastWeek->FileToJson(jRank, pPlayer->GetUid(), pPlayer->GetActiveCtrl().IsWeekRankRewardGet(nType));
                    std::string rankname = "rank_" + toString<int>(rid);
                    json[rankname] = jRank;
                }
            }
        }
            break;
        case 13: // 日排行
        {
            if (nType > 100)
            {
                bSucc =  pPlayer->GetActiveCtrl().DealRankReawrd(nType - 1, json);
            }
            else
            {
                int rid = nType + 1 + (13 - nAid)*6 ;
                int64 llDay = Clock::GetDay() - 1;
                Rank* pRankDay  = RankInfoMgr::GetInst()->GetRankDay(nType, llDay);

                if (pRankDay != NULL)
                {
                    Value jRank (objectValue);
                    jRank["id"] = rid;
                    pRankDay->FileToJson(jRank, pPlayer->GetUid(), pPlayer->GetActiveCtrl().IsDayRankRewardGet(nType));
                    std::string rankname = "rank_" + toString<int>(rid);
                    json[rankname] = jRank;
                }
            }
        }
            break;
        case ActiveCtrl::aidNewPlayerInvite://新版本邀请好友 aid ==11 
        {
            pPlayer->GetActiveCtrl(). DealNewPlayerInvite (nType, json);
        }
            break;
        case ActiveCtrl::aidNewPlayerSaveGift: //10.20.次日登陆
        {
            pPlayer->GetActiveCtrl().DealNewPlayerSaveGift(nType, json);
        }

            break;
        case ActiveCtrl::aidGoldCoupon: //金券活动 7001,7002,7003,7004
        {
            pPlayer->GetActiveCtrl().DealGoldCoupon(nType, json);
        }
            break;

        case ActiveCtrl::aidWeiboListen:
        {
            pPlayer->GetActiveCtrl().DealWeiboListen(nType, json);
        }
            break;
        case ActiveCtrl::aidDallylevel:
        {
            bSucc =  pPlayer->GetActiveCtrl().DealDallyLevel(nType, json);
        }
            break;
        case ActiveCtrl::aidGamerankReward:
        {
            bSucc = pPlayer->GetActiveCtrl().DealGameRankReward(nType, json);
        }
            break;
        case ActiveCtrl::aidDellyRankReward:
        {
            bSucc = pPlayer->GetActiveCtrl().DealDallyLevelWeekReward(nType, json);
        }
            break;
        case  ActiveCtrl::aidCallback:
        {
            bSucc = pPlayer->GetActiveCtrl().DealCallbackLevelList(nType, json);
        }
            break;
        case ActiveCtrl::aidCallbackDetail:
        {
            bSucc = pPlayer->GetActiveCtrl().DealCallbackLevelDetail(nType, json);
        }
            break;
        case ActiveCtrl::aidQQMainPanel:
        {
            bSucc = pPlayer->GetActiveCtrl().DealQQMainPanelReward(nType, json);
        }
            break;
        case ActiveCtrl::aidZyCityReward:
        {
            bSucc = pPlayer->GetActiveCtrl().DealZyCityReward(nType, json);
        }
            break;
        case ActiveCtrl::aidlogin30reward:
        {
            bSucc = pPlayer->GetActiveCtrl().DealLogin30Days(nType, json);
        }
            break;

        case ActiveCtrl::aidNewPlayerStarReword:
        {
            bSucc = pPlayer->GetActiveCtrl().DealNewPlayerStarReword(nType, json);
        }
            break;
        case ActiveCtrl::aidTmpQuestWeek:
        {
            bSucc = pPlayer->GetActiveCtrl().DealTmpQuestWeek(nType, json);
        }
            break;
        case ActiveCtrl::aidDragboatfestival:
        {
            if (nType == -1 || nType == -2)
            {
                bSucc = pPlayer->GetActiveCtrl().Dealricetaking(nType, json);
            }
            else
            {
                bSucc  = pPlayer->GetActiveCtrl().DealexchangeGift(nType, json);
            }

        }
            break;
        case ActiveCtrl::aidActiveInvite_39:
        {
            if ( nType == -1 )
            {
                bSucc = pPlayer->GetActiveCtrl().DealFriendData(nType, json);
            }
            if (nType == 1  || nType == 3  || nType == 5 )
            {

                bSucc = pPlayer->GetActiveCtrl().DealtkFridData(nType, json);

            }
            if (nType == 2 || nType == 4 || nType == 6)
            {

                bSucc = pPlayer->GetActiveCtrl().DealtkEnergyData(nType, json);

            }

            if ( nType == 7 || nType == 8 || nType == 9 )
            {
                bSucc = pPlayer->GetActiveCtrl().DealtkUnivData(nType, json);

            }
        }
            break;

        case ActiveCtrl::aidFullBack:
        {
            bSucc = pPlayer->GetActiveCtrl().DealFullback(nType, json);
        }
            break;
        case ActiveCtrl::aidActivecallback:
        {

            bSucc = pPlayer->GetActiveCtrl().DealActiveCallBack(nType, json);


        }
            break;
        case ActiveCtrl::aidWeekEndmession:
        {
            bSucc = pPlayer->GetWeekendMissionCtrl().DealTask(nType, json);
        }
            break;
        case ActiveCtrl::aidHouseCloseGift:
        {
            bSucc = pPlayer->GetActiveCtrl().DealHouseCloseGift(nType, json);
        }
            break;
        case ActiveCtrl::aidMobileInstall:
        {
            bSucc = pPlayer->GetActiveCtrl().DealMobileInstallReward(nType, json);
        }
            break;

        case ActiveCtrl::aidItemRecycle:
        {
            bSucc = pPlayer->GetActiveCtrl().DealItemExchange(nType, json);
        }
            break;

        case ActiveCtrl::aidChanllange:
        {
            bSucc = pPlayer->GetActiveCtrl().DealChallenge(nType, json);
        }
            break;
        case ActiveCtrl::aidExpLevel:
        {
            bSucc = pPlayer->GetActiveCtrl().DealExpLevel(nType, json);
        }
            break;
    }
    _M_Result_(bSucc, json);
}
//</editor-fold>



