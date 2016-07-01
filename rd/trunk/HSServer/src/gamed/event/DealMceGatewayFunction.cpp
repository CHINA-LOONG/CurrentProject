#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMceGatewayFunction.h"
#include "../../logic/GatewayLogicEventMgr.h"
#include "../../logic/Player.h"

DealMceGatewayFunction::DealMceGatewayFunction()
: CBaseEvent()
{

}

DealMceGatewayFunction::~DealMceGatewayFunction()
{

}

bool
DealMceGatewayFunction::CheckEvent(Event* e)
{

    const MceGatewayFunction& request = e->mce_mcegatewayfunction();

    //No_default_comp_pass!
    //return false;
    return true;
}

void
DealMceGatewayFunction::handle(Event* e)
{

    if (!CheckEvent(e))
    {
        return;
    }
    int64 nUserID = e->uid();
    GameDataHandler* dh = eh_->getDataHandler();
    const MceGatewayFunction& request = e->mce_mcegatewayfunction();
    User* pUser = NULL;
    if (e->state() == Status_Normal_Game || e->state() == Status_Normal_Back_Game)
    {
        pUser = dh->getUser(nUserID, e);
    }
    else if (e->state() == Status_Normal_Logic_Game)
    {
        bool bUseOpenid =  e->forwardinfo().has_platid() && e->forwardinfo().platid() != "";
        if (bUseOpenid)
        {
            pUser = dh->getUser(e->forwardinfo().platid(), e);
        }
        else
        {
            pUser = dh->getUser(e->forwardinfo().uid(), e);
        }
    }
    if (pUser == NULL)
        return;

#ifdef WIN32
    printf("DBS:%s,%s,attr:[%s]\n", request.servername().c_str(), request.actionname().c_str(), request.jsonattr().c_str());
#endif
    GatewayLogicEventMgr::GetInst()->AccetpEvent(request.servername(), request.actionname(), e, pUser);
    MseGatewayFunction* pResponse = e->mutable_mse_msegatewayfunction();
    std::string text;
    pResponse->set_action_idx(request.action_idx());
    pResponse->set_servername(request.servername());
    pResponse->set_actionname(request.actionname());

    pResponse->SerializeToString(&text);


    eh_->getDataHandler()->markUserDirty(pUser);

    // 多人转发逻辑
    if (e->forwardinfo().has_platid() || e->forwardinfo().has_uid())
    {

        if (e->state() == Status_Normal_Game)
        {
            e->set_state(Status_Normal_To_World);
            eh_->sendEventToWorld(e);
        }
        else if (e->state() == Status_Normal_Logic_Game)
        {
            e->set_state(Status_Normal_Back_World);
            eh_->sendEventToWorld(e);
        }
        else if (e->state() == Status_Normal_Back_Game)
        {
            if (!SendResponseToAdm(pUser, e))
            {
                eh_->sendDataToUser(pUser->fd(), S2C_MseGatewayFunction, text);
            }

        }
    }
    else if (!SendResponseToAdm(pUser,  e))
    {

        if (pUser->GetPlayer()->DebugLevel > 0)
        {
            printf("%s\n-----\n", pResponse->jsonresult().c_str());
        }

        const static int SendSize = 1024 * 6;
        if (pResponse->jsonresult().length() > SendSize)
        {
            static char sendbuf[SendSize + 1];
            int nLength = pResponse->jsonresult().length();
            MseGatewayFunction subsend;
            subsend.CopyFrom(*pResponse);
            int nGrow = 0;
            int nSize = 0;
            int nTotal = nLength / SendSize;
            if (nTotal * SendSize < nLength)
            {
                nTotal++;
            }
            subsend.set_totalpage(nTotal);
            const char* pMsg = pResponse->jsonresult().c_str();
            for (int i = 0; i < nLength; i++)
            {
                sendbuf[nGrow++] = pMsg[i];
                if (i == nLength - 1 || nGrow >= SendSize)
                {
                    sendbuf[nGrow] = '\0';
                    subsend.set_jsonresult(sendbuf);
                    subsend.set_currpage(++nSize);
                    subsend.SerializeToString(&text);
                    eh_->sendDataToUser(pUser->fd(), S2C_MseGatewayFunction, text);
                    nGrow = 0;
                }
            }
        }
        else
        {
            eh_->sendDataToUser(pUser->fd(), S2C_MseGatewayFunction, text);
        }
    }
    CMsg2QQ::GetInstance()->TellMsgProto(pUser, C2S_MceGatewayFunction, true, "GatewayFunction");
}

bool
DealMceGatewayFunction::SendResponse(User* pUser, int nIndex, const std::string& servername, const std::string& actionname, const Value& data)
{
    if (pUser == NULL)
    {
        return false;
    }
    MseGatewayFunction response;
    MseGatewayFunction* pResponse = &response;
    std::string text;
    pResponse->set_action_idx(nIndex);
    pResponse->set_servername(servername);
    pResponse->set_actionname(actionname);
    pResponse->set_jsonresult(data.toStyledString());
    pResponse->SerializeToString(&text);
    const static int SendSize = 1024 * 6;
    if (pUser->GetPlayer()->DebugLevel > 0)
    {
        printf("%s\n-----\n", pResponse->jsonresult().c_str());
    }
    if (pResponse->jsonresult().length() > SendSize)
    {
        static char sendbuf[SendSize + 1];
        int nLength = pResponse->jsonresult().length();
        MseGatewayFunction subsend;
        subsend.CopyFrom(*pResponse);
        int nGrow = 0;
        int nSize = 0;
        int nTotal = nLength / SendSize;
        if (nTotal * SendSize < nLength)
        {
            nTotal++;
        }
        subsend.set_totalpage(nTotal);
        const char* pMsg = pResponse->jsonresult().c_str();
        for (int i = 0; i < nLength; i++)
        {
            sendbuf[nGrow++] = pMsg[i];
            if (i == nLength - 1 || nGrow >= SendSize)
            {
                sendbuf[nGrow] = '\0';
                subsend.set_jsonresult(sendbuf);
                subsend.set_currpage(++nSize);
                subsend.SerializeToString(&text);
                eh_->sendDataToUser(pUser->fd(), S2C_MseGatewayFunction, text);
                nGrow = 0;
            }
        }
    }
    else
    {
        eh_->sendDataToUser(pUser->fd(), S2C_MseGatewayFunction, text);
    }
    return true;
}

bool
DealMceGatewayFunction::SendResponseToAdm(User* pUser, Event* e)
{
    if (e->has_adminstr_req())//如果是从gm平台来的逻辑
    {
        MseGatewayFunction* pResponse = e->mutable_mse_msegatewayfunction();
        Admin_STR_Req* req = e->mutable_adminstr_req();
        Admin_STR_Rsp* rsp = e->mutable_adminstr_rsp();
        vector<string> tokens;
        static string delims = ",";
        std::string txt = "";
        tokenize(req->str(), tokens, delims);
        for (int i = 5; i < tokens.size(); i++)
        {
            txt += tokens[i];
            if (i != tokens.size())
            {
                txt += ",";
            }
        }

        Value objResponseJson(objectValue);
        objResponseJson["cmd"] = tokens[0];
        objResponseJson["sid"] = e->sid();
        objResponseJson["succ"] = true ? 0 : 1;

        Value rsp_json1(objectValue);
        rsp_json1["cmd"]  = "test";
        rsp_json1["ServerName"] = tokens[3];
        rsp_json1["ActName"] = tokens[4];
        rsp_json1["attlist"] = txt;
        rsp_json1["rlt"] = parseJsonStr(pResponse->jsonresult());
        rsp_json1["succ"] = 0;
        rsp_json1["uid"]  = toString<int64 > (pUser->id());
        rsp_json1["openid"] = pUser->platform_id();
        rsp_json1["sid"] = e->sid();
        objResponseJson["result"] = rsp_json1;

        std::string str_response = objResponseJson.toStyledString();

        rsp->set_uid(req->uid());
        rsp->set_succ(true);
        rsp->set_adminfd(req->adminfd());
        rsp->set_str(str_response);
        e->set_state(Admin_GW_Rsp);

        Event fe;
        fe.CopyFrom(*e);
        fe.set_state(Admin_GW_Rsp);
        fe.set_cmd(EVENT_ADMIN_STR);
        eh_->sendEventToWorld(&fe);
        return true;
    }
    return false;
}