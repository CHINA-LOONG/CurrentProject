#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMseFuncSwich : public CBaseEvent
{
public:
	DealMseFuncSwich();
	~DealMseFuncSwich();
	RegistEvent(DealMseFuncSwich,S2C_MseFuncSwich)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
