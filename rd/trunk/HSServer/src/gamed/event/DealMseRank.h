#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMseRank : public CBaseEvent
{
public:
	DealMseRank();
	~DealMseRank();
	RegistEvent(DealMseRank,S2C_MseRank)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
