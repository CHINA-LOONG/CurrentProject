#include "ShareMemAO.h"
#include "ShareMemAPI.h"

using namespace ShareMemAPI;

bool	ShareMemAO::Create(SM_KEY key,UINT Size)
{
		m_hold	= ShareMemAPI::CreateShareMem(key,Size);
		if(m_hold == INVALID_SM_HANDLE)
		{
			LOG4CXX_ERROR(logger_,"Create ShareMem Error SM_KET = "<<key);
			
			return false;
		}

		m_pHeader = ShareMemAPI::MapShareMem(m_hold);

		if(m_pHeader)
		{	
			m_pDataPtr = m_pHeader+sizeof(SMHead);
			((SMHead*)m_pHeader)->m_Key	 =	key;
			((SMHead*)m_pHeader)->m_Size =	Size;
			m_Size	=	Size;
			LOG4CXX_ERROR(logger_,"Create ShareMem Ok SM_KET = "<<key);
	
			return true;
		}
		else
		{
			LOG4CXX_ERROR(logger_,"Map ShareMem Error SM_KET ="<<key);
			return false;
		}
		return false ;
}

void	ShareMemAO::Destory()
{
	if( m_pHeader )
	{
		ShareMemAPI::UnMapShareMem(m_pHeader);
		m_pHeader = 0;
	}
	if( m_hold )
	{
		ShareMemAPI::CloseShareMem(m_hold);
		m_hold = 0;
	}

	m_Size	=	0;
}

bool	ShareMemAO::Attach(SM_KEY key,UINT	Size)
{
	m_hold	=	ShareMemAPI::OpenShareMem(key,Size);

	//if(m_CmdArg == CMD_MODE_CLEARALL)
	//{
	//	Destory();
	//	LOG4CXX_ERROR(logger_,"Close ShareMemory key = "<<key);
	//	return false;
	//}
	

	if(m_hold == INVALID_SM_HANDLE)
	{
		LOG4CXX_ERROR(logger_,"Attach ShareMem Error SM_KET = "<<key);
		
		return false;
	}
	
	m_pHeader = ShareMemAPI::MapShareMem(m_hold);

	if(m_pHeader)
	{
		m_pDataPtr = m_pHeader+sizeof(SMHead);
		m_Size	=	Size;
		LOG4CXX_ERROR(logger_,"Attach ShareMem OK SM_KET = "<<key);
		return true;
	}
	else
	{
		LOG4CXX_ERROR(logger_,"Map ShareMem Error SM_KET = "<<key);
		return false;
	}
		return false;

}

bool	ShareMemAO::DumpToFile(char* FilePath)
{	
		FILE* f	= fopen(FilePath,"wb");
		if(!f)	
			return false;
		fwrite(m_pHeader,1,m_Size,f);
		fclose(f);	
		return true;
}

bool ShareMemAO::MergeFromFile(char* FilePath)
{
		
		FILE*	f = fopen(FilePath,"rb");
		if(!f)
			return false;
		fseek(f,0L,SEEK_END);
		int FileLength =ftell(f);
		fseek(f,0L,SEEK_SET);
		fread(m_pHeader,FileLength,1,f);
		fclose(f);

		return true;
}

//void ShareMemAO::SetHeadVer(UINT ver)
//{
//	//__ENTER_FUNCTION
//		
//		((SMHead*)m_pHeader)->m_HeadVer = ver;
//
//	//__LEAVE_FUNCTION
//}
//
//UINT ShareMemAO::GetHeadVer()
//{
//	//__ENTER_FUNCTION
//
//		UINT ver = ((SMHead*)m_pHeader)->m_HeadVer;
//		return ver;
//
//	//__LEAVE_FUNCTION
//
//		return 0;
//}

