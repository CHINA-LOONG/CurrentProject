#pragma once
//-----------------------------------------------------------------------------------
//˵��������Ϊ�û�ͳ����Ϣ���ݽṹ���������ݲ�����DB�����ܻ�д����־
//-----------------------------------------------------------------------------------

class CStatInfo
{
public:
	CStatInfo();
	~CStatInfo();
	void Clear();
public:
	int		m_nVisitFrdCount;
	int		m_nLoginTimes;	//��½����
	int		m_nEnterRoomCount;
	int		m_nEnterHallCount;
	int		m_nBattleCount;	
};
