#pragma once

#include "ShareMemAPI.h"
#include "ShareMemAO.h"

#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif

const int SM_UNIT_CNT = 50000;

/***************************************************
ShareMem 使用位置：
1.服务器启动的时候，调用Init函数
2.在
***************************************************/

class ShareMemHandler
{
	enum SizeType
	{
		ST_ERROR = -1,
		ST_8K = 0,
		ST_16K,
		ST_32K,
		ST_64K,
		ST_128K,
		ST_256K,
		ST_512K,
		ST_1024K,
		ST_Cnt,
	};
private:
	ShareMemHandler();
	~ShareMemHandler();
public:
	static ShareMemHandler* GetInst()
	{
		static ShareMemHandler inst;
		return &inst;
	}
public:
	bool Init(char* szName,int nSrvID,UINT nSize,SM_KEY key);
	void UseShareMem(bool bUse);				//开启或关闭共享内存，当共享内存关闭后重新开启，会清空共享内存中原有数据，以避免数据回档
	void SetWriteOnly(bool bWriteOnly){m_bWriteOnly = bWriteOnly;}//设置为仅可写（不可读）状态，默认可读
	bool CanUse(){return (m_bInit&&m_bCanUse);}	//可否使用
	bool DumpToFile(char* FilePath);			//将共享内存数据写入文件
//	bool ReAllocCMem(int n32Cnt,int n64Cnt,int n128Cnt,int n256Cnt,int n512Cnt,SM_KEY key,int nVersion);
public:
	bool Push(int64 nUserID,const char* buffer,UINT nSize);
	const char* Get(int64 nUserID,UINT& nSize);
	void CleanAll();
	void Clean(int64 nUserID);

	const map<int64,SMUnit*>& GetUsedUnitMap(){return m_mapUsedUnit;}

private:
	bool Update(int64 nUserID,const char* buffer,UINT nSize);	//更新信息
	void ReadMemUnit();
	void ResetMemUnit();

	SMUnit* FindUsedUnit(int64 nUserID);	//获得已使用的数据

	SMUnit* GetEmptyUnit(UINT nSize);	//获得未使用的单元块
	SMUnit* AllocSMUnit(UINT nSize);	//申请新的单元块
	UINT CacuAllocSize(UINT nSize);		//计算需要填充nSize的数据，需要申请多少内存
	SizeType CacuSizeType(UINT nSize);	//根据nSize，计算所属的尺寸区间

private:
	char* _AllocMem(UINT nSize);			//申请nSize尺寸的内存，返回地址，如果为空，则表示无内存可以分配
private:
	bool			m_bInit;
	bool			m_bCanUse;
	bool			m_bWriteOnly;
	ShareMemAO*		m_pRefObjPtr;		//引用SMObject
	
	SMHead* m_pSMHeader;				//内存头
	char*	m_pSMUnitHeader;			//单元头
	
	log4cxx::LoggerPtr logger_;
	map<int64,SMUnit*> m_mapUsedUnit;
	list<SMUnit*> m_lstEmyUnit[ST_Cnt];
	UINT	m_nUsedSize;			//使用的总内存数
	int		m_nUnuseHeadIdx;		//未使用的单元头序号
};	



