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
ShareMem ʹ��λ�ã�
1.������������ʱ�򣬵���Init����
2.��
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
	void UseShareMem(bool bUse);				//������رչ����ڴ棬�������ڴ�رպ����¿���������չ����ڴ���ԭ�����ݣ��Ա������ݻص�
	void SetWriteOnly(bool bWriteOnly){m_bWriteOnly = bWriteOnly;}//����Ϊ����д�����ɶ���״̬��Ĭ�Ͽɶ�
	bool CanUse(){return (m_bInit&&m_bCanUse);}	//�ɷ�ʹ��
	bool DumpToFile(char* FilePath);			//�������ڴ�����д���ļ�
//	bool ReAllocCMem(int n32Cnt,int n64Cnt,int n128Cnt,int n256Cnt,int n512Cnt,SM_KEY key,int nVersion);
public:
	bool Push(int64 nUserID,const char* buffer,UINT nSize);
	const char* Get(int64 nUserID,UINT& nSize);
	void CleanAll();
	void Clean(int64 nUserID);

	const map<int64,SMUnit*>& GetUsedUnitMap(){return m_mapUsedUnit;}

private:
	bool Update(int64 nUserID,const char* buffer,UINT nSize);	//������Ϣ
	void ReadMemUnit();
	void ResetMemUnit();

	SMUnit* FindUsedUnit(int64 nUserID);	//�����ʹ�õ�����

	SMUnit* GetEmptyUnit(UINT nSize);	//���δʹ�õĵ�Ԫ��
	SMUnit* AllocSMUnit(UINT nSize);	//�����µĵ�Ԫ��
	UINT CacuAllocSize(UINT nSize);		//������Ҫ���nSize�����ݣ���Ҫ��������ڴ�
	SizeType CacuSizeType(UINT nSize);	//����nSize�����������ĳߴ�����

private:
	char* _AllocMem(UINT nSize);			//����nSize�ߴ���ڴ棬���ص�ַ�����Ϊ�գ����ʾ���ڴ���Է���
private:
	bool			m_bInit;
	bool			m_bCanUse;
	bool			m_bWriteOnly;
	ShareMemAO*		m_pRefObjPtr;		//����SMObject
	
	SMHead* m_pSMHeader;				//�ڴ�ͷ
	char*	m_pSMUnitHeader;			//��Ԫͷ
	
	log4cxx::LoggerPtr logger_;
	map<int64,SMUnit*> m_mapUsedUnit;
	list<SMUnit*> m_lstEmyUnit[ST_Cnt];
	UINT	m_nUsedSize;			//ʹ�õ����ڴ���
	int		m_nUnuseHeadIdx;		//δʹ�õĵ�Ԫͷ���
};	



