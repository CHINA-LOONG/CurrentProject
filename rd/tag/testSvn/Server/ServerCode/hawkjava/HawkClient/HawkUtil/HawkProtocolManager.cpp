#include "HawkProtocolManager.h"
#include "HawkLoggerManager.h"
#include "HawkOSOperator.h"

namespace Hawk
{
	HAKW_SINGLETON_IMPL(ProtocolManager);

	HawkProtocolManager::HawkProtocolManager()
	{
		m_iProtoSize = sizeof(ProtoType) + sizeof(ProtoSize) + sizeof(Int32) + sizeof(ProtoCrc);
	}

	HawkProtocolManager::~HawkProtocolManager()
	{
	}

	UInt32 HawkProtocolManager::GetProtoHeaderSize() const
	{
		return m_iProtoSize;
	}

	Bool HawkProtocolManager::ReleaseProto(HawkProtocol*& pProto)
	{
		if (pProto)
		{
			HAWK_RELEASE(pProto);
			return true;
		}
		return false;
	}

	Bool HawkProtocolManager::CheckDecodeProtocol(const HawkOctetsStream& xOS, UInt32* pChunkSize)
	{
		if (xOS.AvailableSize() >= m_iProtoSize)
		{
			ProtoSize iSize = *((ProtoSize*)((Char*)xOS.AvailableData() + sizeof(ProtoType)));
			if (xOS.AvailableSize() >= m_iProtoSize + iSize)
			{
				if (pChunkSize)
					*pChunkSize = m_iProtoSize + iSize;

				return true;
			}
		}
		return false;
	}

	Bool HawkProtocolManager::ReadProtocolHeader(HawkOctetsStream& xOS, ProtoType& iType, ProtoSize& iSize, ProtoCrc& iCrc)
	{
		Int32 iReserve = 0;
		xOS.Pop(iType);
		xOS.Pop(iSize);
		xOS.Pop(iReserve);
		xOS.Pop(iCrc);
		return true;
	}

	Bool HawkProtocolManager::WriteProtocolHeader(HawkOctetsStream& xOS, ProtoType iType, ProtoSize iSize, ProtoCrc iCrc)
	{
		Int32 iReserve = 0;
		xOS.Push(iType);
		xOS.Push(iSize);
		xOS.Push(iReserve);
		xOS.Push(iCrc);
		return true;
	}

	HawkProtocol*  HawkProtocolManager::Decode(HawkOctetsStream& rhsOS)
	{
		if (!CheckDecodeProtocol(rhsOS))
			return 0;

		//创建协议,开始解析
		ProtoType iType	 = *((ProtoType*)rhsOS.AvailableData());
		Protocol* pProto = new Protocol(iType);
		if (pProto && !pProto->Decode(rhsOS))
		{
			HAWK_RELEASE(pProto);
		}
		return pProto;
	}
}
