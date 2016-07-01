#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMseAuthState.h"


DealMseAuthState::DealMseAuthState()
:CBaseEvent()
{

}
DealMseAuthState::~DealMseAuthState()
{

}

bool DealMseAuthState::CheckEvent(Event* e)
{

	return false;
}
void DealMseAuthState::handle(Event* e)
{

	int64 nUserID = e->uid();
	GameDataHandler* dh = eh_->getDataHandler();
	User* pUser = dh->getUser(nUserID);
	if(pUser==NULL)
		return;
	MseAuthState* pMseAuthState = e->mutable_mse_mseauthstate();
	//write code start



	//write code end
	//here send proto to flash
	string text;
	pMseAuthState->SerializeToString(&text);
	eh_->sendDataToUser(pUser->fd(), S2C_MseAuthState, text);
}
