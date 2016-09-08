#include "HawkUtil.h"

Bool	glb_CltRunning	= true;
AString glb_SvrAddr		= "";

int main(int argc, Char* argv[])
{
	HawkUtil::Init();

	HawkSession* pSession = new HawkSession;

	if (pSession->Init(SocketAddr(glb_SvrAddr)))
	{
		UInt32 iLastTime = 0;
		while (glb_CltRunning)
		{
			//发包频率控制
			UInt32 iCurTime = HawkOSOperator::GetTickCount();
			if (iCurTime - iLastTime < 200)
			{
				HawkSleep(200 + iLastTime - iCurTime);
			}
			iLastTime = HawkOSOperator::GetTickCount();

			if (!pSession->Tick(DEFAULT_SLEEP))
			{
				HawkFmtPrint("Session Disconnect");
				break;
			}

			Protocol* pProto = 0;
			while (pSession->DecodeProtocol(pProto) && pProto)
			{
#ifdef _DEBUG
				HawkFmtPrint("Recv Proto, Type: %u, Size: %u, Crc: %u", pProto->GetType(),pProto->GetSize(), pProto->GetCrc());
#endif
				P_ProtocolManager->ReleaseProto(pProto);
			}

			/* 发送协议
			SysProtocol::Sys_Octets sCmd;
			AString sTxt = HawkStringUtil::RandomString<AString>(HawkRand::RandInt(32, 128));
			sCmd.m_sOctets.Replace(sTxt.c_str(), sTxt.size());
			if (!pSession->SendProtocol(&sCmd, true))
			{
				HawkFmtPrint("Session Exception");
				break;
			}
			*/
		}
	}
	HAWK_RELEASE(pSession);

	HawkUtil::Release();
	return 0;
}
