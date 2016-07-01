#ifndef _SHARE_MEM_ACCESS_OBJECT_H_
#define _SHARE_MEM_ACCESS_OBJECT_H_

#include "ShareMemDef.h"
#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif


#define		SHMEM_LOG_PATH	"./Log/ShareMemory.log"

//共享内存头
struct SMHead
{
	int	m_Key;	
	UINT m_Size;	//内存总大小
};

//单个内存存储单元
struct SMUnit
{
	UINT	m_nSize;	//单元总大小
	int		m_nSucc;	//保存成功标记，必须在数据修改前置0，修改后置1，否则可能会导致数据错误！！！！！！
	int		m_bUsed;
	UINT	m_nUseSize;	//使用的大小
	int64	m_nUserID;
	char*	m_pBuf;		//偏移位置	
};

/*
 *		共享内存访问对象
 *		ShareMemory	Access	Object
 */
class ShareMemAO
{
public:
	ShareMemAO()
	{
		m_pDataPtr	=	0;
		m_hold		=	0;
		m_Size		=	0;
		m_pHeader	=	0;	
		logger_ = log4cxx::Logger::getLogger("EventHandler");

	}
	~ShareMemAO(){};
	/*
	 *	创建ShareMem 访问对象(新创建)
	 *
	 *  SM_KEY	key		访问键值
	 *
	 *	uint		Size	访问数据区字节个数
	 *  int		nCount  单元数
	 */
	bool	Create(SM_KEY key,UINT Size);
	/*
	 *	销毁对象
	 */
	void	Destory();

	/*
	 *		附着ShareMem 访问对象(不是新创建)
	 *		SM_KEY	key		访问键值
	 *
	 *		UINT		Size	访问数据区字节个数
	 *		
	 */
	bool	Attach(SM_KEY key,UINT Size);
	/*
	 *		取消附着(不销毁)
	 */
	bool	Detach();

	/*
	 *	 获得数据区指针
	 */
	char*	GetDataPtr()
	{
		return m_pDataPtr;
	}
	/*
	 *	 获得head
	 */	
	SMHead* GetHead()
	{
		if(m_pHeader)
		{
			return ((SMHead*)m_pHeader);
		}
		return NULL;
	}
	/*
	 *	获得 大小为tSize 的第tIndex 个smu的数据
	 */
	char*	GetTypePtr(UINT tSize,UINT tIndex)
	{
		//Assert(tSize>0);
		//Assert(tSize*tIndex<m_Size);
		if( tSize<=0 || tIndex>=m_Size )
			return NULL ;
		return m_pDataPtr+tSize*tIndex;
	}
	/*
	 *	获得数据区总大小
	 */
	int	GetSize()
	{
		return m_Size;
	}

	bool	DumpToFile(char* FilePath);
	bool	MergeFromFile(char* FilePath);

	UINT	GetHeadVer();
	void	SetHeadVer(UINT ver);
	
	SMHandle GetHold() {return m_hold;}
	//命令
	int					m_CmdArg;
private:
	
	//ShareMemory	内存大小
	int				m_Size;
	// ShareMemory  数据指针
	char*				m_pDataPtr;
	// ShareMemory	内存头指针
	char*				m_pHeader;
	// ShareMemory	句柄	
	SMHandle			m_hold;

	log4cxx::LoggerPtr logger_;
};	

#endif

