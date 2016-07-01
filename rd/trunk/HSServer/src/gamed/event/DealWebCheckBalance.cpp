#include "friendrequest.h"
#include "MessageDef.h"
#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/json/json.h"
#include "../../common/string-util.h"
#include "../../logic/User.h"
#include "../../logic/Player.h"
//#include "../../logic/PropInfoMgr.h"
//#include "../../logic/ShopMgr.h"
#include "../../common/SysLog.h"
#include "DealWebCheckBalance.h"

DealWebCheckBalance* DealWebCheckBalance::instance_ = NULL;

/**
 * 处理web服务器发来的给物品的请求
 * @param e
 */
void DealWebCheckBalance::handle(Event* e)
{
//     const WebCheckBandCoin& request    = e->webcheckbandcoin();
//     WebCheckBandCoin*       pResponse   = e->mutable_webcheckbandcoin();
// 
//     GameDataHandler* pUserManager   = eh_->getDataHandler();
//     ShopMgr* pShopMgr               = ShopMgr::GetInst();
//     PropInfoMgr* pPropInfoMgr       = PropInfoMgr::GetInst();
// 
//     LoadStatus state                = LOAD_INVALID;
//     User* pUser                     = pUserManager->getUser(request.openid(), &state, true);
//     if (pUser == NULL)
//     {
//         if (state == LOAD_WAITING)
//         {
//             eh_->postBackEvent(e);
//             return;
//         }
//         else if (state == LOAD_MISS || state == LOAD_EMPTY)
//         {
//             pResponse->set_value(-1);
//         }
//     }
//     else
//     {
//         Player* pPlayer = pUser->GetPlayer();
//         if (pPlayer != NULL && pPlayer->GetExData() != NULL)
//         {
//             pResponse->set_value(pPlayer->GetExData()->bandcoin());
//         }
//     }
//     e->set_state(Status_Normal_Back_World);
//     eh_->sendEventToWorld(e);

}
