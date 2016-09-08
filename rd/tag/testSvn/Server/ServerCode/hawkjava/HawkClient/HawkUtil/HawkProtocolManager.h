#ifndef HAWK_PROTOCOLMANAGER_H
#define HAWK_PROTOCOLMANAGER_H

#include "HawkProtocol.h"

namespace Hawk
{
	/************************************************************************/
	/* Э���������װ                                                       */
	/************************************************************************/
	class UTIL_API HawkProtocolManager
	{
	protected:
		//����
		HawkProtocolManager();

		//����
		virtual ~HawkProtocolManager();

		//��������������
		HAKW_SINGLETON_DECL(ProtocolManager);

	public:
		//����Э������
		virtual HawkProtocol*  Decode(HawkOctetsStream& rhsOS);

	public:
		//��ȡЭ��ͷ�ֽڴ�С
		virtual UInt32		   GetProtoHeaderSize() const;

		//�ͷ�Э��
		virtual Bool		   ReleaseProto(HawkProtocol*& pProto);
		
		//�Ƿ��пɶ�Э���ͷ
		virtual Bool		   CheckDecodeProtocol(const HawkOctetsStream& xOS, UInt32* pChunkSize = 0);

		//дЭ���ͷ
		virtual Bool		   ReadProtocolHeader(HawkOctetsStream& xOS, ProtoType& iType, ProtoSize& iSize, ProtoCrc& iCrc);

		//��Э���ͷ
		virtual Bool		   WriteProtocolHeader(HawkOctetsStream& xOS,ProtoType iType, ProtoSize iSize, ProtoCrc iCrc);

	private:
		UInt32  m_iProtoSize;
	};

	#define P_ProtocolManager  HawkProtocolManager::GetInstance()

	//����ϵͳ����֧��Э���
	#define REGISTER_PROTO(protocol_class)\
	{\
		protocol_class* _ptr_ = new protocol_class;\
		P_ProtocolManager->Register(_ptr_->GetType(),_ptr_);\
		HAWK_RELEASE(_ptr_);\
	}
}
#endif
