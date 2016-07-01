#include "ClientHandler.h"
#include <stdlib.h>
#include "GameNetHandler.h"
#include "EventDefine.h"
#include "common/string-util.h"
#include "common/json-util.h"
#include "EventQueue.h"
#include "common/Msg2QQ.h"
#include "event/MessageDef.h"

string ClientHandler::policy_content;

ClientHandler::ClientHandler(GameNetHandler *nh, int fd, int nid, NetCache *cache)
{
    this->nh = nh;
    this->fd = fd;
    this->nid_ = nid;
    ClientHandler::policy_content =            \
		string("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\
			   <cross-domain-policy>\
			   <allow-access-from domain=\"*\" to-ports=\"*\" secure=\"false\" />\
			   </cross-domain-policy>");
    logger_ = log4cxx::Logger::getLogger("ClientHandler");
    this->pCheatDetector = new CheatDetector();
}

ClientHandler::~ClientHandler(void)
{
    if (pCheatDetector != NULL)
    {
        delete pCheatDetector;
    }
}

int ClientHandler::handlerType()
{
    return ProtocolHandler::CLIENT;
}


#define DEFINE_AUTO_EVENT(EventID,SubEvent,SubEventFunc)\
case EventID:\
{\
	SubEvent* pSubEvn = e->SubEventFunc();\
	if (pSubEvn->ParseFromString(req)) bRight = true;\
		break;\
}

void ClientHandler::HandleAutoEvent(int64 uid, int nType, string& req)
{
    if (nType < C2S_EVENT_BASE || nType >= EVENT_ADMIN)
    {
        return;
    }
    bool bRight = false;

    Event* e = nh->eq->allocateEvent();
    e->set_cmd(nType);
    e->set_state(Status_Normal_Game);
    e->set_time(0);
    e->set_uid(uid);

    switch (nType)
    {
#include "ClientHandler_Auto.h"
        default:
            break;
    }
    if (bRight)
        nh->eq->safePushEvent(e);
    else
        nh->eq->freeEvent(e);

    if (pCheatDetector != NULL)
    {
        CheatDetector::GlableEvent(nType, uid);
    }
}

void ClientHandler::handle(int64 uid, string &req)
{
    if (req == "<policy-file-request/>")
    {
        processPolicy();
    }
    else if (req.c_str()[0] == 'a')
    {
        vector<string> tokens;
        static string delims = ",";
        tokenize(req, tokens, delims);
        int64 uid, secret;
        if (safe_atoll(tokens[1].c_str(), uid) &&
                safe_atoll(tokens[2].c_str(), secret))
        {
           
        }
    }
    else
    {
        unsigned short nType = 0;
        if (req.size()<sizeof (unsigned short) )
            return;
        nType = ntohs(*((unsigned short*) req.c_str()));
        LOG4CXX_INFO(logger_, "Receive Command Number: " << nType);
        try
        {
            string subreq = req.substr(sizeof (unsigned short) + sizeof (unsigned int) );
            HandleAutoEvent(uid, nType, subreq);
        }
        catch (...)
        {
        }
    }
}

void ClientHandler::processPolicy()
{
    nh->sendFdString(fd, policy_content.c_str(), policy_content.size());
    nh->closeConnection(fd);
}

void ClientHandler::leave(int64 uid)
{
    Event* e = nh->eq->allocateEvent();
    e->set_cmd(EVENT_USER_LOGOUT);
    e->set_time(0);
    e->set_state(Status_Normal_Game);
    e->set_uid(uid);
    WG_UserLeave* pUserLeave = e->mutable_wg_userleave();
    pUserLeave->set_fd(fd);
    nh->eq->safePushEvent(e);
}
