#pragma once
//���ļ���.cpp�ļ�Ϊʹ�ù��ߴ����������ļ����޸ģ�����������ָ��ļ������������´�����
#include "BaseEvent.h"
class DealMseFriendList : public CBaseEvent
{
public:
	DealMseFriendList();
	~DealMseFriendList();
	RegistEvent(DealMseFriendList,S2C_MseFriendList)

private:
	void handle(Event* e);
	bool CheckEvent(Event* e);
};
