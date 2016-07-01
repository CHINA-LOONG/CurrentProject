#include "WorldHandler.h"
#include "EventQueue.h"
#include "net/NetHandler.h"
#include "common/string-util.h"
#include "GameNetHandler.h"

WorldHandler::WorldHandler(EventQueue *eq, int fd, NetHandler *nh) : fd_(fd), eq_(eq), nh_(nh)
{
    logger_ = log4cxx::Logger::getLogger("WorldHandler");
}

WorldHandler::~WorldHandler()
{
}

void
WorldHandler::handle(int64 uid, string &req)
{
    if (req.substr(0, 3) == "ev,")
    {
        eq_->pushStringEvent(req.substr(3), fd_ );
    }
    else if (req == "pass")
    { // register passed
        LOG4CXX_INFO(logger_, "Register to world done.");
        OnReg2World();
    }
    else if (req == "failed")
    { // register failed
        LOG4CXX_INFO(logger_, "Register to world failed. Closing connection...");
        nh_->closeConnection(fd_);
    }
    else if (req == "ka")
    {
        // keep alive message, do nothing
    }
}

void
WorldHandler::OnReg2World()
{
    int wid = -1;
    GameNetHandler* pNetHandler = (GameNetHandler*) nh_;
    for (int i = 0; wid == -1 && i < pNetHandler->GetWorldServerList().size(); i++)
    {
        if (fd_ == pNetHandler->GetWorldServerList()[i].iFd && fd_ > 0)
        {
            wid = i;
        }
    }

}