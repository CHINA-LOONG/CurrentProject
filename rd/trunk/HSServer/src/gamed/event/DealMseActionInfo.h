#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMseActionInfo : public CBaseEvent
{
public:
	DealMseActionInfo();
	~DealMseActionInfo();
	RegistEvent(DealMseActionInfo,S2C_MseActionInfo)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
