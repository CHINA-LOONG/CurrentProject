#include "HawkUtil.h"
#include "HawkOSOperator.h"

namespace Hawk
{
	void HawkUtil::Init()
	{
#ifdef PLATFORM_WINDOWS
		//网络初始化
		WSADATA sData;
		WSAStartup(MAKEWORD(2, 2),&sData);		

		//设置CRT调试模式
		_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF|_CRTDBG_REPORT_FLAG|_CRTDBG_LEAK_CHECK_DF);
#else
		struct sigaction sAction;  
		sAction.sa_handler = SIG_IGN;  
		sigaction(SIGPIPE, &sAction, 0); 
#endif		
	}

	void HawkUtil::Tick(UInt32 iPeriod)
	{
	}

	void HawkUtil::Release()
	{
#ifdef PLATFORM_WINDOWS
		//网络清理
		WSACleanup();
#endif
	}
}
