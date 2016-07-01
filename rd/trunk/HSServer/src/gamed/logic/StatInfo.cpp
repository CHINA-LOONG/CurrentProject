#include "StatInfo.h"

CStatInfo::CStatInfo()
{
	Clear();
}
CStatInfo::~CStatInfo()
{

}

void CStatInfo::Clear()
{
	m_nEnterHallCount	= 0;
	m_nLoginTimes		= 0;
	m_nVisitFrdCount	= 0;
	m_nEnterRoomCount	= 0;
	m_nBattleCount		= 0;

}