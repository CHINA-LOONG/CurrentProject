#ifndef _SHARE_MEM_ACCESS_OBJECT_H_
#define _SHARE_MEM_ACCESS_OBJECT_H_

#include "ShareMemDef.h"
#ifdef _WIN32
#include "common/Logger_win.h"
#else
#include <log4cxx/logger.h>
#endif


#define		SHMEM_LOG_PATH	"./Log/ShareMemory.log"

//�����ڴ�ͷ
struct SMHead
{
	int	m_Key;	
	UINT m_Size;	//�ڴ��ܴ�С
};

//�����ڴ�洢��Ԫ
struct SMUnit
{
	UINT	m_nSize;	//��Ԫ�ܴ�С
	int		m_nSucc;	//����ɹ���ǣ������������޸�ǰ��0���޸ĺ���1��������ܻᵼ�����ݴ��󣡣���������
	int		m_bUsed;
	UINT	m_nUseSize;	//ʹ�õĴ�С
	int64	m_nUserID;
	char*	m_pBuf;		//ƫ��λ��	
};

/*
 *		�����ڴ���ʶ���
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
	 *	����ShareMem ���ʶ���(�´���)
	 *
	 *  SM_KEY	key		���ʼ�ֵ
	 *
	 *	uint		Size	�����������ֽڸ���
	 *  int		nCount  ��Ԫ��
	 */
	bool	Create(SM_KEY key,UINT Size);
	/*
	 *	���ٶ���
	 */
	void	Destory();

	/*
	 *		����ShareMem ���ʶ���(�����´���)
	 *		SM_KEY	key		���ʼ�ֵ
	 *
	 *		UINT		Size	�����������ֽڸ���
	 *		
	 */
	bool	Attach(SM_KEY key,UINT Size);
	/*
	 *		ȡ������(������)
	 */
	bool	Detach();

	/*
	 *	 ���������ָ��
	 */
	char*	GetDataPtr()
	{
		return m_pDataPtr;
	}
	/*
	 *	 ���head
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
	 *	��� ��СΪtSize �ĵ�tIndex ��smu������
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
	 *	����������ܴ�С
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
	//����
	int					m_CmdArg;
private:
	
	//ShareMemory	�ڴ��С
	int				m_Size;
	// ShareMemory  ����ָ��
	char*				m_pDataPtr;
	// ShareMemory	�ڴ�ͷָ��
	char*				m_pHeader;
	// ShareMemory	���	
	SMHandle			m_hold;

	log4cxx::LoggerPtr logger_;
};	

#endif

