#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMseGatewayFunction : public CBaseEvent
{
public:
	DealMseGatewayFunction();
	~DealMseGatewayFunction();
	RegistEvent(DealMseGatewayFunction,S2C_MseGatewayFunction)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
