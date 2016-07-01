#include "ShareMemHandler.h"

const int SIZE_1K = 1024;
const int SIZE_8K = 8*SIZE_1K;
const int SIZE_16K = 16*SIZE_1K;
const int SIZE_32K = 32*SIZE_1K;
const int SIZE_64K = 64*SIZE_1K;
const int SIZE_128K = 128*SIZE_1K;
const int SIZE_256K = 256*SIZE_1K;
const int SIZE_512K = 512*SIZE_1K;
const int SIZE_1024K = 1024*SIZE_1K;

ShareMemHandler::ShareMemHandler()
:m_pRefObjPtr(NULL)
,m_pSMHeader(NULL)
,m_pSMUnitHeader(NULL)
,m_bInit(false)
,m_bCanUse(false)
,m_bWriteOnly(true)
{
	logger_ = log4cxx::Logger::getLogger("EventHandler");
	for(int i=0;i<ST_Cnt;i++)
	{
		m_lstEmyUnit[i].clear();
	}
	m_mapUsedUnit.clear();
	m_nUnuseHeadIdx = 0;
}

ShareMemHandler::~ShareMemHandler()
{
	if(m_pRefObjPtr)
	{
		delete m_pRefObjPtr;
		m_pRefObjPtr = NULL;
	}
	m_pSMHeader		= NULL;
	m_pSMUnitHeader = NULL;
	for(int i=0;i<ST_Cnt;i++)
	{
		m_lstEmyUnit[i].clear();
	}
	m_mapUsedUnit.clear();
	m_nUnuseHeadIdx = 0;
	m_nUsedSize	= 0;
}

bool ShareMemHandler::Init(char* szName,int nSrvID,UINT nSize,SM_KEY key)
{

	m_pRefObjPtr = new ShareMemAO();

	bool ret;
	UINT nUnitHeadSize = sizeof(SMUnit);
	ret = m_pRefObjPtr->Attach(key,nSize);
	if(ret)
	{
		//附着成功，读取内存块
		ReadMemUnit();
	}
	if(!ret)
	{//attach失败或版本变更
		ret = m_pRefObjPtr->Create(key,nSize);
		if(ret)
		{
#ifndef WIN32
			int nHandle = m_pRefObjPtr->GetHold();
			char filebuff[256];
			sprintf(filebuff, "%s_SM_%d.ipc", szName, nSrvID);
			FILE * fp = fopen(filebuff, "w");
			fprintf(fp, "%d\n", nHandle);
			fclose(fp);
#endif
			ResetMemUnit();
		}
	}
	if(!ret)
	{
		LOG4CXX_ERROR(logger_,"ShareMemHandler::ReAllocCMem Create Mem Error!!!,total size:"<<nSize);
		return ret;
	}
	m_bInit = true;
	m_bCanUse = true;
	return true;
}

void ShareMemHandler::UseShareMem(bool bUse)
{
	if(!bUse)
	{
		m_bCanUse = false;
	}
	else if(m_bInit)
	{//此函数用于启动后打开关闭共享内存，当共享内存关闭后启动时，清空原内存中的所有数据，防止回档
		m_bCanUse = true;
		ResetMemUnit();
	}
}

void ShareMemHandler::ReadMemUnit()
{
	for(int i=0;i<ST_Cnt;i++)
	{
		m_lstEmyUnit[i].clear();
	}
	m_mapUsedUnit.clear();

	int nUnitHeadSize = sizeof(SMUnit);
	m_nUnuseHeadIdx = 0; 
	m_nUsedSize = nUnitHeadSize*SM_UNIT_CNT+sizeof(SMHead);
	m_pSMHeader = m_pRefObjPtr->GetHead();
	m_pSMUnitHeader = m_pRefObjPtr->GetDataPtr();

	int nIdx = 0;
	
	//单元头必须是顺序分配的，如果有一个单元头的pUnit->m_pBuf不小心置空，
	//则后面的单元数据全部认为是无效的！
	char* pTmp = (char*)m_pSMHeader+m_nUsedSize;
	for(;nIdx<SM_UNIT_CNT;nIdx++)
	{
		SMUnit* pUnit = (SMUnit*)(m_pSMUnitHeader+nUnitHeadSize*nIdx);
		//string strTest((char*)pUnit,sizeof(SMUnit));
		if(pUnit->m_nSize>0)
		{
			pUnit->m_pBuf = (char*)m_pSMHeader+m_nUsedSize;
			m_nUsedSize += pUnit->m_nSize;
			//已分配内存	
			if(pUnit->m_bUsed&&pUnit->m_nUseSize>0)
			{
				if(pUnit->m_nSucc)
				{
					m_mapUsedUnit[pUnit->m_nUserID] = pUnit;
				}
				else
				{//错误数据
					pUnit->m_bUsed = false;
					pUnit->m_nUseSize = 0;
					SizeType nTp = CacuSizeType(pUnit->m_nSize);
					if(nTp!=ST_ERROR)
					{
						m_lstEmyUnit[nTp].push_back(pUnit);
					}
					else
					{//错误
						break;
					}
				}
			}
			else
			{//无效内存数据
				SizeType nTp = CacuSizeType(pUnit->m_nSize);
				if(nTp!=ST_ERROR)
				{
					m_lstEmyUnit[nTp].push_back(pUnit);
				}
				else
				{//错误
					break;
				}
			}
		}
		else
		{
			break;
		}
	}
	m_nUnuseHeadIdx = nIdx;//指向最后一块已分配头的下一个

	//后面的单元头全部置空
	memset((char*)(m_pSMUnitHeader+m_nUnuseHeadIdx*nUnitHeadSize),0,(SM_UNIT_CNT-m_nUnuseHeadIdx)*sizeof(SMUnit)*sizeof(char));
	
}


void ShareMemHandler::ResetMemUnit()
{
	for(int i=0;i<ST_Cnt;i++)
	{
		m_lstEmyUnit[i].clear();
	}
	m_mapUsedUnit.clear();


	int nUnitHeadSize = sizeof(SMUnit);
	m_nUsedSize = nUnitHeadSize*SM_UNIT_CNT+sizeof(SMHead);
	m_pSMHeader = m_pRefObjPtr->GetHead();
	m_pSMUnitHeader = m_pRefObjPtr->GetDataPtr();
	m_nUnuseHeadIdx = 0;

	//所有单元头全部置空
	memset(m_pSMUnitHeader,0,SM_UNIT_CNT*sizeof(SMUnit));
	
}
bool ShareMemHandler::Update(int64 nUserID,const char* buffer,UINT nSize)
{

	SMUnit* pGetUnit = FindUsedUnit(nUserID);
	if(pGetUnit)
	{
		pGetUnit->m_nSucc = 0;

		int nNewAllocSize = CacuAllocSize(nSize);
		if(pGetUnit->m_nSize==nNewAllocSize)
		{
			pGetUnit->m_nUseSize = nSize;
			memset(pGetUnit->m_pBuf,0,nNewAllocSize);
			memcpy(pGetUnit->m_pBuf,buffer,nSize);

			pGetUnit->m_nSucc = 1;
			return true;
		}
		else
		{//尺寸变了
			Clean(nUserID);
			return false;
		}
	}
	return false;

}
bool ShareMemHandler::Push(int64 nUserID,const char* buffer,UINT nSize)
{
	if(!CanUse()||buffer==NULL||nSize<=0)
		return false;

	if(Update(nUserID,buffer,nSize))
		return true;

	SMUnit* pEmpUnit = GetEmptyUnit(nSize);
	if(pEmpUnit==NULL)
	{
		pEmpUnit = AllocSMUnit(nSize);	
	}
	if(pEmpUnit==NULL)
	{
		LOG4CXX_ERROR(logger_,"ShareMemHandler::Push UserID:"<<nUserID<<"AllocSMUnit Failed!!! Need Size:"<<nSize);

		return false;
	}
	pEmpUnit->m_nSucc = 0;

	pEmpUnit->m_bUsed = 1;
	pEmpUnit->m_nUserID = nUserID;
	pEmpUnit->m_nUseSize = nSize;

	m_mapUsedUnit[nUserID] = pEmpUnit;
	memset(pEmpUnit->m_pBuf,0,pEmpUnit->m_nSize);
	memcpy(pEmpUnit->m_pBuf,buffer,nSize);

	//string strTest((char*)pEmpUnit,sizeof(SMUnit));
		
	pEmpUnit->m_nSucc = 1;
	return true;
}

const char* ShareMemHandler::Get(int64 nUserID,UINT& nSize)
{
	if(m_bWriteOnly||!CanUse()||nUserID<=0)
		return NULL;	
	SMUnit* pUnit = FindUsedUnit(nUserID);
	if(pUnit)
	{
		if(pUnit->m_bUsed&&pUnit->m_nSucc)
		{
			nSize = pUnit->m_nUseSize;
			return pUnit->m_pBuf;
		}
		else
		{
			Clean(nUserID);
		}
		
	}
	nSize = 0;
	return NULL;
}

void ShareMemHandler::CleanAll()
{
	if(!CanUse())
		return ;
	ResetMemUnit();
}

void ShareMemHandler::Clean(int64 nUserID)
{
	if(!CanUse())
		return ;

	map<int64,SMUnit*>::iterator iter;
	iter = m_mapUsedUnit.find(nUserID);
	if(iter != m_mapUsedUnit.end())
	{
		SMUnit* pUnit = iter->second;
		m_mapUsedUnit.erase(iter);
		pUnit->m_nSucc=0;
		pUnit->m_nUserID = 0;
		pUnit->m_bUsed = false;
		pUnit->m_nSucc = 1;
		pUnit->m_nUseSize = 0;
		SizeType nTp = CacuSizeType(pUnit->m_nSize);
		if(nTp != ST_ERROR)
		{
			m_lstEmyUnit[nTp].push_back(pUnit);
		}
	}
}

SMUnit* ShareMemHandler::AllocSMUnit(UINT nSize)
{
	if(!CanUse())
		return NULL;
	//申请新的内存块
	if(m_nUnuseHeadIdx>=SM_UNIT_CNT)
	{
		LOG4CXX_ERROR(logger_,"hareMemHandler::Push All Unit Head Is Used!!! UnitCnt:"<<SM_UNIT_CNT);
		return NULL;
	}
	UINT nAllocSize = CacuAllocSize(nSize);
	if(nAllocSize<=0)
	{
		LOG4CXX_ERROR(logger_,"hareMemHandler::AllocSMUnit Need Mem:"<<nSize<<"Return False!!!!");
		return NULL;
	}
	char* pBuf = _AllocMem(nAllocSize);
	if(pBuf==NULL)
	{
		LOG4CXX_ERROR(logger_,"hareMemHandler::No Mem To Alloc!!! NeedSize:"<<nAllocSize);
		return NULL;
	}
	SMUnit* pEmpUnit = (SMUnit*)(m_pSMUnitHeader+m_nUnuseHeadIdx*sizeof(SMUnit));
	pEmpUnit->m_nSize = nAllocSize;
	pEmpUnit->m_pBuf = pBuf;
	pEmpUnit->m_nSucc = 0;
	m_nUnuseHeadIdx++;
	m_nUsedSize += nAllocSize;

	return pEmpUnit;
}

char* ShareMemHandler::_AllocMem(UINT nSize)
{
	if(m_nUsedSize+nSize>m_pSMHeader->m_Size)
		return NULL;
	return ((char*)m_pSMHeader+m_nUsedSize);
}

SMUnit* ShareMemHandler::GetEmptyUnit(UINT nSize)
{
	SizeType nType = CacuSizeType(nSize);
	if(nType==ST_ERROR)
		return NULL;

	if(m_lstEmyUnit[nType].size()>0)
	{
		SMUnit* pUnit = m_lstEmyUnit[nType].front();
		m_lstEmyUnit[nType].pop_front();
		return pUnit;
	}
	return NULL;
}

SMUnit* ShareMemHandler::FindUsedUnit(int64 nUserID)
{
	map<int64,SMUnit*>::iterator iter;
	iter = m_mapUsedUnit.find(nUserID);
	if(iter!=m_mapUsedUnit.end())
	{
		SMUnit* pUnit = iter->second;
		return pUnit;
	}
	return NULL;
}

UINT ShareMemHandler::CacuAllocSize(UINT nSize)
{
	if(nSize>SIZE_1024K)
		return 0;
	for(int ret = SIZE_8K;ret<=SIZE_1024K;)
	{
		if(nSize<=ret)
			return ret;
		ret *= 2;
	}
	return 0;
}
ShareMemHandler::SizeType ShareMemHandler::CacuSizeType(UINT nSize)
{
	if(nSize>SIZE_1024K)
		return ST_ERROR;
	UINT nMaxSize = SIZE_8K;
	int nTP = ST_8K;
	for(;nTP<=ST_1024K;)
	{
		if(nSize<=nMaxSize)
		{
			return (SizeType)nTP;
		}
		nTP++;
		nMaxSize*=2;
	}
	return ST_ERROR;
}

bool ShareMemHandler::DumpToFile(char* FilePath)
{
	if(m_pRefObjPtr)
	{
		return m_pRefObjPtr->DumpToFile(FilePath);
	}
	return false;
}