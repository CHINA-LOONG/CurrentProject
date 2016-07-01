#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMseActionInfo.h"


DealMseActionInfo::DealMseActionInfo()
:CBaseEvent()
{

}
DealMseActionInfo::~DealMseActionInfo()
{

}

bool DealMseActionInfo::CheckEvent(Event* e)
{

	return false;
}
void DealMseActionInfo::handle(Event* e)
{

	int64 nUserID = e->uid();
	GameDataHandler* dh = eh_->getDataHandler();
	User* pUser = dh->getUser(nUserID);
	if(pUser==NULL)
		return;
	MseActionInfo* pMseActionInfo = e->mutable_mse_mseactioninfo();
	//write code start



	//write code end
	//here send proto to flash
	string text;
	pMseActionInfo->SerializeToString(&text);
	eh_->sendDataToUser(pUser->fd(), S2C_MseActionInfo, text);
}
