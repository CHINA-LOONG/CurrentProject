#include "HawkProtocol.h"
#include "HawkOSOperator.h"
#include "HawkLoggerManager.h"
#include "HawkStringUtil.h"
#include "HawkProtocolManager.h"

namespace Hawk
{
	HawkProtocol::HawkProtocol(ProtoType iType, OctetsStream* pOctets) : m_iType(iType)
	{
		if (pOctets != 0) {
			Encode(*pOctets);
		}
	}

	HawkProtocol::~HawkProtocol()
	{
	}

	ProtoType HawkProtocol::GetType() const
	{
		return m_iType;
	}	

	ProtoSize HawkProtocol::GetSize() const
	{
		return m_iSize;
	}

	ProtoCrc HawkProtocol::GetCrc() const
	{
		return m_iCrc;
	}

	Bool HawkProtocol::GetOctets(OctetsStream*& pOS)
	{
		pOS = &m_sDecode;
		return true;
	}

	Bool HawkProtocol::Encode(const OctetsStream& rhsOS)
	{
		try
		{				
			m_iSize = rhsOS.Size();
			m_iCrc  = HawkOSOperator::CalcCrc((UChar*)rhsOS.Begin(), m_iSize);
			
			m_sDecode.Clear();
			P_ProtocolManager->WriteProtocolHeader(m_sDecode, m_iType, m_iSize, m_iCrc);
			m_sDecode.Push(rhsOS.Begin(),m_iSize);
		}
		catch (HawkException& rhsExcep)
		{
			HawkFmtError(rhsExcep.GetMsg().c_str());
			return false;
		}

		return true;
	}

	Bool HawkProtocol::Decode(HawkOctetsStream& rhsOS)
	{
		//检测协议完整性
		if (!P_ProtocolManager->CheckDecodeProtocol(rhsOS))
			return false;
		
		m_iSize = 0;
		m_iCrc  = 0;
		m_sDecode.Clear();

		ProtoType iType    = 0;
		ProtoCrc  iCrc     = 0;
		try
		{		
			//读协议头
			rhsOS >> HawkOctetsStream::TransBegin;

			P_ProtocolManager->ReadProtocolHeader(rhsOS, iType, m_iSize, m_iCrc);
			
			if ((ProtoSize)rhsOS.AvailableSize() >= m_iSize)
			{
				//读协议数据
				if (m_iSize)
				{
					m_sDecode.Reserve(m_iSize);
					rhsOS.Pop((void*)m_sDecode.Begin(), m_iSize);
					m_sDecode.Resize(m_iSize);
					iCrc = HawkOSOperator::CalcCrc((UChar*)m_sDecode.Begin(),m_iSize);
				}

				//读取完全,提交
				rhsOS >> HawkOctetsStream::TransCommit;
			}
			else
			{
				//数据包不完全,回滚
				rhsOS >> HawkOctetsStream::TransRollback;
				return false;
			}
		}
		catch (HawkException& rhsExcep)
		{
			rhsOS >> HawkOctetsStream::TransRollback;
			HawkFmtError(rhsExcep.GetMsg().c_str());
			return false;
		}

		//数据校验
		if(m_iType != iType)
		{
			HawkFmtError("Protocol Type Inconformity, Type: %u, Crc: %u", iType, m_iCrc);
			T_Exception("Protocol Type Inconformity.");
			return false;
		}
		else if(m_iCrc != iCrc)
		{
			HawkFmtError("Protocol Crc Inconformity, Type: %u, Crc: %u", iType, m_iCrc);
			T_Exception("Protocol Crc Inconformity.");
			return false;
		}

		return true;
	}
}
