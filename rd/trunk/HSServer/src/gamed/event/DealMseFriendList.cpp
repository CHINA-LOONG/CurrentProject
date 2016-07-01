#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMseFriendList.h"


DealMseFriendList::DealMseFriendList()
:CBaseEvent()
{

}
DealMseFriendList::~DealMseFriendList()
{

}

bool DealMseFriendList::CheckEvent(Event* e)
{

	return false;
}
void DealMseFriendList::handle(Event* e)
{

	int64 nUserID = e->uid();
	GameDataHandler* dh = eh_->getDataHandler();
	User* pUser = dh->getUser(nUserID);
	if(pUser==NULL)
		return;
	MseFriendList* pMseFriendList = e->mutable_mse_msefriendlist();
	//write code start



	//write code end
	//here send proto to flash
	string text;
	pMseFriendList->SerializeToString(&text);
	eh_->sendDataToUser(pUser->fd(), S2C_MseFriendList, text);
}
