#ifndef HAWK_UTIL_H
#define HAWK_UTIL_H

#if !defined(UTIL_EXPORT)
#	include "HawkAnyType.h"
#	include "HawkAtomic.h"
#	include "HawkBase64.h"
#	include "HawkByteOrder.h"
#	include "HawkCounter.h"
#	include "HawkDemutexTable.h"
#	include "HawkDiskFile.h"
#	include "HawkException.h"
#	include "HawkFile.h"
#	include "HawkHeapArray.h"
#	include "HawkIPAddr.h"
#	include "HawkKVFile.h"
#	include "HawkLogger.h"
#	include "HawkLoggerManager.h"
#	include "HawkMalloc.h"
#	include "HawkMarshal.h"
#	include "HawkMarshalData.h"
#	include "HawkMath.h"
#	include "HawkMd5.h"
#	include "HawkMemoryFile.h"
#	include "HawkOSOperator.h"
#	include "HawkOctets.h"
#	include "HawkOctetsStream.h"
#	include "HawkProtocol.h"
#	include "HawkProtocolManager.h"
#	include "HawkRWEFds.h"
#	include "HawkRand.h"
#	include "HawkRefCounter.h"
#	include "HawkRingBuffer.h"
#	include "HawkScope.h"
#	include "HawkSecurity.h"
#	include "HawkSession.h"
#	include "HawkSmartPtr.h"
#	include "HawkSocket.h"
#	include "HawkSocketAddr.h"
#	include "HawkSocketPair.h"
#	include "HawkStdHeader.h"
#	include "HawkStreamCompress.h"
#	include "HawkStringUtil.h"
#	include "HawkUtil.h"
#	include "HawkZlib.h"
	using namespace Hawk;
#else
#	include "HawkStdHeader.h"
#endif

namespace Hawk
{
	/************************************************************************/
	/* 应用底层初始化,更新,停止,释放的统一接口封装                          */
	/************************************************************************/
	class UTIL_API HawkUtil
	{
	public:
		//初始化
		static void Init();

		//周期更新
		static void Tick(UInt32 iPeriod);

		//释放资源
		static void Release();
	};
};
#endif
