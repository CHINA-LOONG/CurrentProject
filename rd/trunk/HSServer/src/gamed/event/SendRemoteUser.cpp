#include "SendRemoteUser.h"
#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/json/json.h"
#include "../../common/string-util.h"
#include "../../logic/User.h"
#include "../../event/EventQueue.h"

void SendRemoteUser::handle(Event* e)
{
	if (e->state() == SendRemoteUser_WG_Req)
	{
		if(!e->has_sendremoteuser_req())
		{
			return;
		}
		handle_WG_Req(e);
	}
	else
	{
		LOG4CXX_ERROR(logger_, "Invalid Event.\n"<<e->DebugString());
	}
}

void SendRemoteUser::handle_WG_Req(Event* e)
{
	GameDataHandler* dh = eh_->getDataHandler();
	const SendRemoteUser_Req& req = e->sendremoteuser_req();
	int64 uid = e->uid();

	User *user = dh->getUser(uid);
	if(user == NULL)
	{
		return;
	}

	eh_->sendDataToUser(user->fd(), req.cmd(), base64_decode(req.text()));
}
