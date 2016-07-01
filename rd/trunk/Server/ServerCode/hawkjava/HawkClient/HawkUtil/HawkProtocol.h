#ifndef HAWK_PROTOCOL_H
#define HAWK_PROTOCOL_H

#include "HawkMarshalData.h"
#include "HawkSocket.h"

namespace Hawk
{
	/************************************************************************/
	/* Э���ʽ���������װ                                                 */
	/************************************************************************/
	class UTIL_API HawkProtocol : public HawkMarshal
	{
	public:
		//Э�鹹��
		HawkProtocol(ProtoType iType = 0, OctetsStream* pOctets = 0);

		//Э������
		virtual ~HawkProtocol();

	public:		
		//Э����: Type + Size + Reserve + Crc + Data
		virtual Bool  Encode(const OctetsStream& rhsOS);

		//Э����
		virtual Bool  Decode(OctetsStream& rhsOS);

	public:
		//��ȡЭ������
		ProtoType	  GetType() const;

		//��ȡ�ֽ���
		ProtoSize     GetSize() const;

		//��ȡЭ������У��CRC
		ProtoCrc	  GetCrc() const;

		//��ȡBuffer����
		Bool		  GetOctets(OctetsStream*& pOS);

	protected:
		//����
		ProtoType	 m_iType;
		//�ֽ���
		ProtoSize	 m_iSize;
		//У����
		ProtoCrc	 m_iCrc;
		//���ݴ洢��
		OctetsStream m_sDecode;
	};

	//Э�����ͼ�㶨��
	typedef HawkProtocol Protocol;
}
#endif
