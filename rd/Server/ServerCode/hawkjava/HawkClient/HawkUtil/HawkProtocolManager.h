#ifndef HAWK_PROTOCOLMANAGER_H
#define HAWK_PROTOCOLMANAGER_H

#include "HawkProtocol.h"

namespace Hawk
{
	/************************************************************************/
	/* 协议管理器封装                                                       */
	/************************************************************************/
	class UTIL_API HawkProtocolManager
	{
	protected:
		//构造
		HawkProtocolManager();

		//析构
		virtual ~HawkProtocolManager();

		//管理器单例申明
		HAKW_SINGLETON_DECL(ProtocolManager);

	public:
		//解析协议数据
		virtual HawkProtocol*  Decode(HawkOctetsStream& rhsOS);

	public:
		//获取协议头字节大小
		virtual UInt32		   GetProtoHeaderSize() const;

		//释放协议
		virtual Bool		   ReleaseProto(HawkProtocol*& pProto);
		
		//是否有可读协议包头
		virtual Bool		   CheckDecodeProtocol(const HawkOctetsStream& xOS, UInt32* pChunkSize = 0);

		//写协议包头
		virtual Bool		   ReadProtocolHeader(HawkOctetsStream& xOS, ProtoType& iType, ProtoSize& iSize, ProtoCrc& iCrc);

		//读协议包头
		virtual Bool		   WriteProtocolHeader(HawkOctetsStream& xOS,ProtoType iType, ProtoSize iSize, ProtoCrc iCrc);

	private:
		UInt32  m_iProtoSize;
	};

	#define P_ProtocolManager  HawkProtocolManager::GetInstance()

	//定义系统所能支持协议宏
	#define REGISTER_PROTO(protocol_class)\
	{\
		protocol_class* _ptr_ = new protocol_class;\
		P_ProtocolManager->Register(_ptr_->GetType(),_ptr_);\
		HAWK_RELEASE(_ptr_);\
	}
}
#endif
