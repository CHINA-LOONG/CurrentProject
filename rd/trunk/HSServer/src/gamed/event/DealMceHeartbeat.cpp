#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMceHeartbeat.h"


DealMceHeartbeat::DealMceHeartbeat()
:CBaseEvent()
{

}
DealMceHeartbeat::~DealMceHeartbeat()
{

}

bool DealMceHeartbeat::CheckEvent(Event* e)
{

	const MceHeartbeat& request = e->mce_mceheartbeat();
	//No_default_comp_pass!
	//return false;
	return true;
}
void DealMceHeartbeat::handle(Event* e)
{

	if (!CheckEvent(e)){return;}
	int64 nUserID = e->uid();
	GameDataHandler* dh = eh_->getDataHandler();
	const MceHeartbeat& request = e->mce_mceheartbeat();
	User* pUser = dh->getUser(nUserID);
	if(pUser==NULL)
		return;

	CMsg2QQ::GetInstance()->TellMsgProto(pUser,C2S_MceHeartbeat,true,"Heartbeat");
}
