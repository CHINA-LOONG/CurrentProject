#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMceHeartbeat : public CBaseEvent
{
public:
	DealMceHeartbeat();
	~DealMceHeartbeat();
	RegistEvent(DealMceHeartbeat,C2S_MceHeartbeat)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
