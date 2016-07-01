#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMseGatewayFunction.h"


DealMseGatewayFunction::DealMseGatewayFunction()
:CBaseEvent()
{

}
DealMseGatewayFunction::~DealMseGatewayFunction()
{

}

bool DealMseGatewayFunction::CheckEvent(Event* e)
{

	return false;
}
void DealMseGatewayFunction::handle(Event* e)
{

	int64 nUserID = e->uid();
	GameDataHandler* dh = eh_->getDataHandler();
	User* pUser = dh->getUser(nUserID);
	if(pUser==NULL)
		return;
	MseGatewayFunction* pMseGatewayFunction = e->mutable_mse_msegatewayfunction();
	//write code start



	//write code end
	//here send proto to flash
	string text;
	pMseGatewayFunction->SerializeToString(&text);
	eh_->sendDataToUser(pUser->fd(), S2C_MseGatewayFunction, text);
}
