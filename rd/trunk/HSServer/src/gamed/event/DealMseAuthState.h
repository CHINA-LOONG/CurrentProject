#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMseAuthState : public CBaseEvent
{
public:
	DealMseAuthState();
	~DealMseAuthState();
	RegistEvent(DealMseAuthState,S2C_MseAuthState)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
