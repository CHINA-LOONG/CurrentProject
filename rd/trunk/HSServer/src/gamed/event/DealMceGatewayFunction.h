#pragma once
//该文件及.cpp文件为使用工具创建，可在文件中修改，工具如果发现该文件，将不再重新创建。
#include "BaseEvent.h"
#include "common/json-util.h"
class User;

class DealMceGatewayFunction : public CBaseEvent
{
public:
    DealMceGatewayFunction();
    ~DealMceGatewayFunction();
    RegistEvent(DealMceGatewayFunction, C2S_MceGatewayFunction)

private:
    void handle(Event* e);
    bool CheckEvent(Event* e);
public:
    bool SendResponse(User* pUser,int nIndex, const std::string& servername, const std::string& actionname, const Value& data);
    bool SendResponseToAdm(User* pUser,Event* e);
} ;
