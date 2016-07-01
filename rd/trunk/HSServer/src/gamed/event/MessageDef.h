#pragma once
//该文件为使用工具自动生成，请不要在文件中修改，如需要修改，请与sskylin联系。
enum S2C_EVENT
{
	S2C_EVENT_BASE = 511,
	S2C_MseAuthState=512,		//MseAuthState(511+1)
	S2C_MseFuncSwich=513,		//MseFuncSwich(511+2)
	S2C_MseGatewayFunction=514,		//MseGatewayFunction(511+3)
	S2C_MseFriendList=515,		//MseFriendList(511+4)
	S2C_MseActionInfo=516,		//MseActionInfo(511+5)
	S2C_MseRank=517,		//MseRank(511+6)
};
enum C2S_EVENT
{
	C2S_EVENT_BASE = 1023,
	C2S_MceGatewayFunction=1024,		//MceGatewayFunction(1023+1)
	C2S_MceHeartbeat=1025,		//MceHeartbeat(1023+2)
	C2S_MceActionInfo=1026,		//MceActionInfo(1023+3)

	C2S_Max_NotAMessage,};
// class def for header files.
//class MceGatewayFunction;
//class MceHeartbeat;
//class MceActionInfo;
//class MseAuthState;
//class MseFuncSwich;
//class MseGatewayFunction;
//class MseFriendList;
//class MseActionInfo;
//class MseRank;
//class AchievementItem;
//class BagItem;
//class BroadcastMessage;
//class FriendInfoLite;
//class LeaveMessage;
//class SingleRankItem;

