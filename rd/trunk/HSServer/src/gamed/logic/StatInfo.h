#pragma once
//-----------------------------------------------------------------------------------
//说明：该类为用户统计信息数据结构，类中数据不存入DB，可能会写入日志
//-----------------------------------------------------------------------------------

class CStatInfo
{
public:
	CStatInfo();
	~CStatInfo();
	void Clear();
public:
	int		m_nVisitFrdCount;
	int		m_nLoginTimes;	//登陆次数
	int		m_nEnterRoomCount;
	int		m_nEnterHallCount;
	int		m_nBattleCount;	
};
