#include "friendrequest.h"
#include "MessageDef.h"
#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/json/json.h"
#include "../../logic/GoodsInfoMgr.h"
#include "../../common/string-util.h"
#include "../../logic/User.h"
#include "../../logic/Player.h"
#include "../../common/SysLog.h"
#include "../../common/Msg2QQ.h"
#include "DealWebBuy.h"
#include "../../logic/GoodsBaseInfo.pb.h"
#include "../../logic/ItemManager.h"
#include "../../logic/UserCtrl.h"
#include "../../logic/WebBuyRecords.h"
#include "../../logic/GameConfig.h"
#include "EventQueue.h"
#include "../../logic/Clock.h"
#include "../../logic/ItemManager.h";
#include "../../logic/ConfigUnit/UniverseCfgUnit.h"
#include "../../logic/UniverseMgr.h"
#include "../../logic/UniverseCtrl.h"
#include "../../logic/ConfigUnit/ItemCfgUnit.h"
#include "DealMceGatewayFunction.h"


DealWebBuy* DealWebBuy::instance_ = NULL;

/**
 * 处理web服务器发来的给物品的请求
 * @param e
 */
void
DealWebBuy::handle(Event* e)
{

    User* pUser = GetUser(e);
    if (pUser == NULL)
    {
        return;
    }
    if (e->has_webbuy())
    {
        handle_per(e);
    }
    if (e->has_webbugcfm())
    {
        handle_after(e, e->has_webbuy());
    }
    else
    {

    }
    eh_->getDataHandler()->markUserDirty(pUser, true, true);
}

/**
 * 预先检查. 储存购买信息
 * @param e
 */
void
DealWebBuy::handle_per(Event* e)
{

}

/**
 * 确定购买
 * @param e
 * @param direct_give
 */
void
DealWebBuy::handle_after(Event* e, bool direct_give/*=false*/)
{
    if (e == NULL)
    {
        return ;
    }
    User* pUser = GetUser(e);
    if (pUser == NULL)
    {
        return ;
    }
    Player* pPlayer = pUser->GetPlayer();

    const WebBuyCfm& requestCfm     = e->webbugcfm();
    const WebBuy& request = e->webbuy();
    WebBuy*  pResponse = e->mutable_webbuy();

    bool bCheckResult = WebBuyCheck(pUser, request, pResponse);

    //<editor-fold desc="log data"> 
    std::string log_lvl_type = "";
    UniverseCfgUnit* pCfgUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(pPlayer->GetUniverseCtrl().GetNowPlayingLvl());
    if (pCfgUnit != NULL)
    {
        log_lvl_type = pCfgUnit->level_type;
    }
    int log_cash = pUser->GetDbUser().player().bag().cash();

    //</editor-fold>
    for (int i = 0; bCheckResult && i < request.items_size(); i++)
    {
        type_id tid = request.items(i).shopid();
        ItemCfgUnit* pCfg =  (ItemCfgUnit*) ItemCfgMgr::GetInst()->GetUnit(tid);

        int num = request.items(i).count();
        if (num == 1)
        {
            num = ItemCfgMgr::GetWebBuyItemNum(tid);
        }

        if (pCfg != NULL && pCfg->IsLimited())
        {
            pPlayer->GetLimit().AddLimit(pCfg->nLimitId, num);
        }
        if (pCfg != NULL && !pCfg->SubItemSet.IsEmpty())
        {
            pUser->GetPlayer()->GetBag().AddItemSet( &(pCfg->SubItemSet));
        }
        else
        {
            pUser->GetPlayer()->GetBag().AddItem(tid, num);
        }

        if (tid != ItemManager::GetCashItemTypeId() && tid != ItemManager::GetCashPacket100() && tid != ItemManager::GetCashPacket20() && tid != ItemManager::GetCashPacket50() && tid != ItemManager::GetCashPacket500())
        {
            pPlayer->GetDailyCounter().AddCounter(DC_BUY_ITEM, 1);
        }
        SYS_LOG(pUser->id(), LT_WebBuy, 0, 0 , tid << num);
        pUser->GetDbUser().set_qpoint(pUser->GetDbUser().qpoint() + request.items(i).price() );
        int nNowCash = pUser->GetDbUser().player().bag().cash();
        pPlayer->GetLogCtrl().LogBegin(LT_WebBuy)->
                LogItem("lvlid", pPlayer->GetUniverseCtrl().GetNowPlayingLvl())->
                LogItem("lvltype", log_lvl_type)->
                LogItem("lvlid", tid)->
                LogItem("nCostNum", ItemCfgMgr::GetLogPrice(tid, num))->
                LogItem("nItemNum", num)->
                LogSubType(LogCtrl::ST_Consume, LogCtrl::ST_Get)->LogEnd("BuyCandy");

        log_cash = nNowCash;


    }

    pResponse->set_bandcoincost(0);
    pResponse->set_uid(pUser->id());
    pResponse->set_sid(requestCfm.sid());
    pResponse->set_leftcoin(0);
    pResponse->set_bandcoincost(0);
    pResponse->set_key(requestCfm.key());
    pResponse->set_fd(request.fd());
    pResponse->set_succ(bCheckResult);
    e->clear_webbugcfm();
    e->set_state(Status_Normal_Back_World);
    eh_->sendEventToWorld(e);

    UserCtrl uc(pUser);
    uc.SendBalance();
    uc.SendPoll();
    uc.SendMapBag();
    //send data to clinet
    // with fake action index -2 (which means static getbalance callback)
    Value json_fake(objectValue);
    pPlayer->GetBag().FillToJson(json_fake);
    Value fake_result(objectValue);
    fake_result["status"] = 1;
    fake_result["data"] = json_fake;

    DealMceGatewayFunction::getInstance()->
            SendResponse(pUser, -2, "PlayerService", "getBalance", fake_result);


}

/**
 * 检查购买是可以进行
 * @param pUser
 * @param request
 * @return
 */
bool
DealWebBuy::WebBuyCheck(User* pUser, const WebBuy& request, WebBuy* pResponse)
{
    bool bCheckResult = true;
    bCheckResult = pUser != NULL;
    for (int i = 0; bCheckResult && i < request.items_size(); i++)
    {
        ItemCfgUnit* pCfg = ( ItemCfgUnit*) ItemCfgMgr::GetInst()->GetUnit(request.items(i).shopid());
        if (pCfg == NULL) pCfg = (ItemCfgUnit*) ItemCfgMgr::GetInst()->GetMapItemConfig(request.items(i).shopid());
        if (pCfg == NULL) pCfg = (ItemCfgUnit*) ItemCfgMgr::GetInst()->GetMapMaterialConfig(request.items(i).shopid());

        bool bIsCash      = request.items(i).shopid() == ItemCfgMgr::GetCashItemTypeId();
        bool bIsMaxLive  = request.items(i).shopid() == ItemCfgMgr::GetLiveMaxChangeTypeId();
        bool bHasCfg      = pCfg != NULL;
        bool bCount       = request.items(i).count() > 0;
        bool bStaticTid  = request.items(i).shopid() == ItemCfgMgr::GetCashPacket20()
                || request.items(i).shopid() == ItemCfgMgr::GetCashPacket50()
                || request.items(i).shopid() == ItemCfgMgr::GetCashPacket100()
                || request.items(i).shopid() == ItemCfgMgr::GetCashPacket500();
        bool bLimit       = bHasCfg && !pUser->GetPlayer()->GetLimit().Full(pCfg->nLimitId, request.items(i).count());
        bCheckResult      = bCheckResult && bCount &&  (bIsMaxLive || bIsCash || bHasCfg || bStaticTid) && (bLimit || !bHasCfg);
    }
    return bCheckResult;
}

/**
 * 获取玩家的指针.如果获取不到则返回NULL
 * @param e
 * @return
 */
User*
DealWebBuy::GetUser(Event* e)
{
    const WebBuy& request           = e->webbuy();
    GameDataHandler* pUserManager   = eh_->getDataHandler();
    User* pUser                     = NULL;
    if (request.use_openid())
    {
        LoadStatus state            = LOAD_INVALID;
        const std::string& openid   = request.openid();
        pUser = pUserManager->getUser(openid, &state, true);
        if (pUser == NULL)
        {
            if (state == LOAD_WAITING)
            {
                eh_->postBackEvent(e);
            }
            if (state == LOAD_MISS)
            {
                WebBuy* pResponse = e->mutable_webbuy();
                pResponse->set_bandcoincost(0);
                pResponse->set_uid(pUser->id());
                pResponse->set_sid(request.sid());
                pResponse->set_leftcoin(0);
                pResponse->set_bandcoincost(0);
                pResponse->set_key(request.key());
                pResponse->set_fd(request.fd());
                pResponse->set_succ(false);
                pResponse->set_infodetail("UserLoadMiss");
                e->set_state(Status_Normal_Back_World);
                eh_->sendEventToWorld(e);
            }
        }
    }
    else
    {
        pUser = pUserManager->getUser(e->uid());
    }
    return pUser;
}

