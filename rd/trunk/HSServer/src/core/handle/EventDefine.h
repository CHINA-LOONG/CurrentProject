#pragma once

#ifndef EVENT_BASE
#define EVENT_BASE 0

enum EVENT_DEFINE
{
    EVENT_UPDATE_WORKING_STATUS	= EVENT_BASE + 1,
    EVENT_USER_LOGIN,
    EVENT_USER_LOGOUT,
    EVENT_USER_AUTH,
    EVENT_TIMER,
    EVENT_CHANGE_ROOM,
    EVENT_SEND_REMOTE_USER,
    EVENT_BE_LOTTERY,
    EVENT_BE_UPDATEWININFO,
    EVENT_BE_UPDATEEXP,
    EVENT_BE_UPDATETOOL,
    EVENT_DP_NORMALRESULT,
    EVENT_GWG_FRIEND_REQUEST,
    EVENT_LOTTERYTIMER,
    EVENT_BE_UPDATEBATTLECNT,
    EVENT_USER_LEAVEBATTLE,
    EVENT_KILLUSERSTAT,
    EVENT_MATCHING_TIMER,
    EVENT_ROLEINFO,
    EVENT_WEB_BUY,
    EVENT_HALL_TIMER,
    EVENT_WEB_LENGTHEN_INDATE,
    EVENT_WEB_CHECKBALANCE,
    EVENT_REFALSH_PRICE,
    EVENT_UPDATE_GUEST_NUMBER,
    EVENT_WEB_UNLOCK,
    EVENT_WEB_ADDEMPLOYEE,
    EVENT_WEB_INVITE,
    EVENT_INVITE,
    EVENT_WEB_ASK,
    EVENT_WEB_GIVE,
    EVENT_WEB_QQUNION_CHECK_KEY,
    EVENT_WEB_QQUNION_CHECK_OPENID,
    EVENT_WEB_QQUNION_ADD_ITEM,
    EVENT_MULTI_LEVEL_FRIEND_POINT,
    EVENT_RALENT_SHOW,
    EVENT_RALENT_SHOW_LIST,
    EVENT_WEB_GIFT_ASK,
    EVENT_FRIEND_SEND_GIFT,
    EVENT_FORTUNE_STEAL,
    EVENT_WEB_FRIEND_RECALL_SEND,	// 发送老友召回请求
    EVENT_WEB_FRIEND_RECALL_BACK,	// 接受老友召回请求
    EVENT_FRIEND_LOGTIME_REQUEST,	// 把最后一次登陆游戏的时间放到FriendInfoLite里
    EVENT_ALARM_POSTDATA,           // 闹钟上报
    EVENT_WEB_RETURN_GIFT,			// 召唤好友,礼品卡
    EVENT_SIMPLE_MARK,              // 简单设置属性
    EVENT_WEB_RETURN_GIFT_Y,		// 黄钻召回,礼品卡
    EVENT_WEB_BUY3,
    EVENT_CHIEF_VOTE,
	EVENT_INV_FRIEND_LVL_SUCC,      //被邀请的好友升级时通知邀请人
    EVENT_CenterLogicCounter,       //同步关键字
} ;

enum EVENT_WAP_DEFINE
{
    EVENT_WAP = 3048,
    EVENT_WAP_MAX,
} ;

#define EVENT_ADMIN 2048

enum EVENT_ADMIN_DEFINE
{
    EVENT_ADMIN_ADDEXP = EVENT_ADMIN + 1,
    EVENT_ADMIN_ADDGOLD,
    EVENT_ADMIN_ADDPROP,
    EVENT_ADMIN_SETLEVEL,
    EVENT_ADMIN_SETEQUIP,
    EVENT_ADMIN_USERINFO,
    EVENT_ADMIN_DELPROP,
    EVENT_ADMIN_RUNTASK,
    EVENT_ADMIN_BANCHAT,
    EVENT_ADMIN_BANLOGIN,
    EVENT_ADMIN_ADDPOINT,
    EVENT_ADMIN_ADDINDATE,
    EVENT_ADMIN_FRESHUSER,
    EVENT_ADMIN_ADDMEDAL,
    EVENT_ADMIN_BROADCAST,
    EVENT_ADMIN_RELOAD,
    EVENT_ADMIN_FLUENT_PRICE,
    EVENT_ADMIN_ADDGOODS,
    EVENT_ADMIN_COMMON,
    EVENT_ADMIN_DELGOODS,
    EVENT_ADMIN_ADDGUEST,
    EVENT_ADMIN_SETPUBLICPRAISE,
    EVENT_ADMIN_CLEAR,
    EVENT_ADMIN_ONLINE,
    EVENT_ADMIN_KICK,
    EVENT_ADMIN_STR,
    EVENT_ADMIN_SETVAL,
} ;
// #define EVENT_ADMIN_ADDEXP EVENT_ADMIN + 1
// #define EVENT_ADMIN_ADDGOLD EVENT_ADMIN_ADDEXP + 1
// #define EVENT_ADMIN_ADDPROP EVENT_ADMIN_ADDGOLD + 1
// #define EVENT_ADMIN_SETLEVEL EVENT_ADMIN_ADDPROP + 1
// #define EVENT_ADMIN_SETEQUIP EVENT_ADMIN_SETLEVEL + 1
// #define EVENT_ADMIN_USERINFO EVENT_ADMIN_SETEQUIP + 1
// #define EVENT_ADMIN_DELPROP EVENT_ADMIN_USERINFO + 1
// #define EVENT_ADMIN_RUNTASK EVENT_ADMIN_DELPROP + 1
// #define EVENT_ADMIN_BANCHAT EVENT_ADMIN_RUNTASK + 1
// #define EVENT_ADMIN_BANLOGIN EVENT_ADMIN_BANCHAT + 1
// #define EVENT_ADMIN_ADDPOINT EVENT_ADMIN_BANLOGIN + 1
// #define EVENT_ADMIN_ADDINDATE EVENT_ADMIN_ADDPOINT + 1
// #define EVENT_ADMIN_FRESHUSER EVENT_ADMIN_ADDINDATE + 1
// #define EVENT_ADMIN_ADDMEDAL EVENT_ADMIN_FRESHUSER + 1
// #define EVENT_ADMIN_BROADCAST EVENT_ADMIN_ADDMEDAL + 1
// #define EVENT_ADMIN_RELOAD EVENT_ADMIN_BROADCAST + 1

enum Status_UpdateWorkingStatus
{
    UpdateWorkingStatus_GW_Disconn = 0,
    UpdateWorkingStatus_WG_Disconn,
    UpdateWorkingStatus_GW_Conn,
    UpdateWorkingStatus_WG_Sync,
    UpdateWorkingStatus_GW_Sync,
    UpdateWorkingStatus_WG_Fin,

    UpdateWorkingStatus_GH_Disconn,
    UpdateWorkingStatus_HG_Disconn,
    UpdateWorkingStatus_GH_Conn,
    UpdateWorkingStatus_HG_Sync,
    UpdateWorkingStatus_GH_Sync,
    UpdateWorkingStatus_HG_Fin,
} ;

enum Status_UserLogin
{
    UserLogin_WW_Req = 0,
    UserLogin_WG_Req,
    UserLogin_GW_Rsp
} ;

enum Status_NormalStats
{
    Status_Normal_WW_Req = 0,
    Status_Normal_Game,
    Status_Normal_To_World,
    Status_Normal_Logic_Game,
    Status_Normal_Back_World,
    Status_Normal_Back_Game,
    Status_Normal_To_Hall,
    Status_Normal_Hall_Back,
    Status_Normal_HallSlf,
    Status_Err_StopEvent = 254,
} ;

enum Status_UserAuth
{
    UserAuth_CG_Req = 0,
    UserAuth_GC_Rsp
} ;

enum Status_SendRemoteUser
{
    SendRemoteUser_GW_Req = 0,
    SendRemoteUser_WG_Req
} ;

enum Status_Admin
{
    Admin_AG_Req = 0,
    Admin_GW_Req,
    Admin_WG_Req,
    Admin_GW_Rsp,
    Admin_WG_Rsp,
    Admin_GA_Rsp,
    Admin_WW_Req,
} ;

enum Error_Define
{
    Error_UnKnow = 0,
    Error_NoUser,		//未找到用户
    Error_NoRoom,		//未找到房间
    Error_RoomFull,		//房间已满或正在游戏
    Error_HasRoom,		//已在其他房间中
    Error_RoomNameTooLong, //房间名过长
    Error_KeyTooLong,	//密码过长
} ;

enum Center_Logic_Operation
{
     //0: sub 1:add 2:init 4:world sync to game 
    CLO_SUB = 0,
    CLO_ADD = 1,
    CLO_INIT= 2,
    CLO_SYNC= 4,
};
#endif

