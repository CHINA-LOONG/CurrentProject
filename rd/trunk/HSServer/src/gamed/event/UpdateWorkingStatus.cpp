#include "UpdateWorkingStatus.h"
#include "../../event/event.pb.h"
#include "../../event/EventQueue.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"

void UpdateWorkingStatus::addEvent(EventQueue* eq, int state, int gid) {
	Event* e=eq->allocateEvent();
	e->set_cmd(EVENT_UPDATE_WORKING_STATUS);
	e->set_state(state);
	e->set_time(-2);
	WorkingStatus_Req* wg_status = e->mutable_workingstatus_req();
	wg_status->set_gid(gid);
	wg_status->set_finish(false);
	eq->safePushEvent(e);
}

void UpdateWorkingStatus::handle(Event* e)
{
	if (e->state() == UpdateWorkingStatus_WG_Disconn)
	{
		handle_WG_Disconn(e);
	}
	else if (e->state() == UpdateWorkingStatus_WG_Sync)
	{
		handle_WG_Sync(e);
	}
	else if (e->state() == UpdateWorkingStatus_WG_Fin)
	{
		handle_WG_Fin(e);
	}
	else if (e->state() == UpdateWorkingStatus_HG_Disconn)
	{
		handle_HG_Disconn(e);
	}
	else if (e->state() == UpdateWorkingStatus_HG_Sync)
	{
		handle_HG_Sync(e);
	}
	else if (e->state() == UpdateWorkingStatus_HG_Fin)
	{
		handle_HG_Fin(e);
	}
	else
	{
		LOG4CXX_ERROR(logger_, "Invalid Event.\n"<<e->DebugString());
	}
}

void UpdateWorkingStatus::handle_WG_Disconn(Event* e)
{
	eh_->setWorkingStatus(NOT_CONNECTED);
}

void UpdateWorkingStatus::handle_WG_Sync(Event* e)
{
	// sync friends
	if(!e->has_workingstatus_req())
	{
		return;
	}
	WorkingStatus_Req* req = e->mutable_workingstatus_req();
	int gid = req->gid();
	LOG4CXX_INFO(logger_, "Game #"<<gid<<" sync to World.");
	e->set_state(UpdateWorkingStatus_GW_Sync);
	req->set_finish(true);
	eh_->sendEventToWorld(e);
}

void UpdateWorkingStatus::handle_WG_Fin(Event* e)
{
	if(!e->has_workingstatus_req())
	{
		return;
	}
	const WorkingStatus_Req& req = e->workingstatus_req();
	int gid = req.gid();
	eh_->setWorkingStatus(NORMAL);
	LOG4CXX_INFO(logger_, "Game #"<<gid<<" ready.");
}

void UpdateWorkingStatus::handle_HG_Disconn(Event* e)
{
	eh_->setWorkingStatus(NOT_CONNECTED);
	const WorkingStatus_Req& req = e->workingstatus_req();
	int gid = req.gid();
	//eh_->HandleHallDown(gid);
}

void UpdateWorkingStatus::handle_HG_Sync(Event* e)
{
	// sync friends
	if(!e->has_workingstatus_req())
	{
		return;
	}
	WorkingStatus_Req* req = e->mutable_workingstatus_req();
	int gid = req->gid();
	LOG4CXX_INFO(logger_, "Game #"<<gid<<" sync to Hall.");
	e->set_state(UpdateWorkingStatus_GH_Sync);
	req->set_finish(true);
	//eh_->sendEventToHall(e);
}

void UpdateWorkingStatus::handle_HG_Fin(Event* e)
{
	if(!e->has_workingstatus_req())
	{
		return;
	}
	eh_->setWorkingStatus(NORMAL);
	const WorkingStatus_Req& req = e->workingstatus_req();
	int gid = req.gid();
	LOG4CXX_INFO(logger_, "Game #"<<gid<<" ready.");
}

