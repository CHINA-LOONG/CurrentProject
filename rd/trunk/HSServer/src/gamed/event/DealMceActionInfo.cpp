#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMceActionInfo.h"

DealMceActionInfo::DealMceActionInfo()
: CBaseEvent()
{

}

DealMceActionInfo::~DealMceActionInfo()
{

}

bool DealMceActionInfo::CheckEvent(Event* e)
{

    const MceActionInfo& request = e->mce_mceactioninfo();
    //No_default_comp_pass!
    //return false;
    return true;
}

void DealMceActionInfo::handle(Event* e)
{

    if (!CheckEvent(e))
    {
        return;
    }
    int64 nUserID = e->uid();
    GameDataHandler* dh = eh_->getDataHandler();
    const MceActionInfo& request = e->mce_mceactioninfo();
    User* pUser = dh->getUser(nUserID);
    if (pUser == NULL)
        return;

    CMsg2QQ::GetInstance()->TellMsgProto(pUser, C2S_MceActionInfo, true, "ActionInfo");
}
