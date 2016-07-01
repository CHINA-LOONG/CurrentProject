#ifndef HAWK_PROTOCOL_H
#define HAWK_PROTOCOL_H

#include "HawkMarshalData.h"
#include "HawkSocket.h"

namespace Hawk
{
	/************************************************************************/
	/* 协议格式定义操作封装                                                 */
	/************************************************************************/
	class UTIL_API HawkProtocol : public HawkMarshal
	{
	public:
		//协议构造
		HawkProtocol(ProtoType iType = 0, OctetsStream* pOctets = 0);

		//协议析构
		virtual ~HawkProtocol();

	public:		
		//协议打包: Type + Size + Reserve + Crc + Data
		virtual Bool  Encode(const OctetsStream& rhsOS);

		//协议解包
		virtual Bool  Decode(OctetsStream& rhsOS);

	public:
		//获取协议类型
		ProtoType	  GetType() const;

		//获取字节数
		ProtoSize     GetSize() const;

		//获取协议数据校验CRC
		ProtoCrc	  GetCrc() const;

		//获取Buffer数据
		Bool		  GetOctets(OctetsStream*& pOS);

	protected:
		//类型
		ProtoType	 m_iType;
		//字节数
		ProtoSize	 m_iSize;
		//校验码
		ProtoCrc	 m_iCrc;
		//数据存储器
		OctetsStream m_sDecode;
	};

	//协议类型简便定义
	typedef HawkProtocol Protocol;
}
#endif
