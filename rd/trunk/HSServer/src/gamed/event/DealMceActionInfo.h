#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMceActionInfo : public CBaseEvent
{
public:
	DealMceActionInfo();
	~DealMceActionInfo();
	RegistEvent(DealMceActionInfo,C2S_MceActionInfo)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
