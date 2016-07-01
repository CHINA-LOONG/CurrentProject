#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
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
