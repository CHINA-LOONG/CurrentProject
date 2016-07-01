#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMseFuncSwich.h"


DealMseFuncSwich::DealMseFuncSwich()
:CBaseEvent()
{

}
DealMseFuncSwich::~DealMseFuncSwich()
{

}

bool DealMseFuncSwich::CheckEvent(Event* e)
{

	return false;
}
void DealMseFuncSwich::handle(Event* e)
{

	int64 nUserID = e->uid();
	GameDataHandler* dh = eh_->getDataHandler();
	User* pUser = dh->getUser(nUserID);
	if(pUser==NULL)
		return;
	MseFuncSwich* pMseFuncSwich = e->mutable_mse_msefuncswich();
	//write code start



	//write code end
	//here send proto to flash
	string text;
	pMseFuncSwich->SerializeToString(&text);
	eh_->sendDataToUser(pUser->fd(), S2C_MseFuncSwich, text);
}
