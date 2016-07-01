#pragma once

typedef long long       int64;
typedef unsigned char   byte;
typedef unsigned int    u_int;
typedef int             type_id;
typedef int             goods_type;
typedef int             item_only_id;
typedef long long       user_id;

const int llInvalidId = -1;
const int nInvalidPos = -1;
const int nInvalidId  = -1;
const int nInvalidLvl = -1;
const int nInvalidFd  = -1;
const int64 llInvalidTime = -1;

//性别
const int Gender_NULL = 0;
const int Gender_Girl = 1;
const int Gender_Boy  = 2;
const int Gender_ALL  = 3;
// 平台类型，对应WEB的siteid

enum PLAT_TYPE
{
    PLAT_XIAOYOU = 0,	//校友
    PLAT_QZONE,			//空间
    PLAT_WEIBO,         //微博
    PLAT_WAP,           //WAP
    PLAT_MANYOU,        //漫游
    PLAT_UNKNOW1,        //
    PLAT_UNKNOW2,
    PLAT_UNKNOW3,
    PLAT_UNKNOW4,
    PLAT_TYPE_MAX
} ;

//对战模式

enum BattleMode
{
    BM_Persion = 0,		//单挑模式
    BM_Group,			//组队模式
    BM_Count,
} ;
//房间类型

enum RoomType
{
    RT_Athletics = 0,	//竞技
    RT_Challenge,		//挑战
    RT_Count,
} ;
//选择房间模式

enum EnterMode
{
    EM_Rand = 0,		//随机进入
    EM_Create,			//创建房间
    EM_Find,			//查找房间进入
    EM_Quick,			//单人快速进入
} ;

//大厅房间进出模式

enum HallRoomEnterMode
{
    HRM_EnterHall = 0,
    HRM_LeaveHall,
    HRM_EnterRoom,
    HRM_LeaveRoom,
    LeaveRoom_Normal,	//离开房间
    LeaveRoom_Enter,		//切换房间
    LeaveRoom_CutDown,		//服务器当机

    LeaveHall_Normal,		//离开大厅
    LeaveHall_Enter,		//切换大厅
    LeaveRoom_LeaveHall,	//离开大厅

} ;

enum ChatType
{
    // 0:当前 1:私聊 2:工会 3:小喇叭 4:大喇叭 5:小队
    Chat_Current = 0,
    Chat_Persion,
    Chat_Union,
    Chat_SmallBugle,
    Chat_BigBugle,
    Chat_Group,
} ;

const int MAX_HALLSRV_COUNT		= 1000;	//最大服务器数
const int CHANNEL_OFFSET		= 100;		//频道偏移量
const int ROOM_OFFSET			= 1000;		//房间偏移量
const int HALLSRV_OFFSET		= CHANNEL_OFFSET*ROOM_OFFSET;

const int EXPIATE_FLAG_GOLDPOINT = 0x01;		//礼券补偿标志
const int EXPIATE_FLAG_STONE	= 0x02;		//2011.03.30补偿标志
const int EXPIATE_FLAG_GOLDBOX	= 0x04;		//等级宝箱补偿标志

#define Locat_CH "CH_QZ"
#define Locat_US "US_FB"
#define MAX_USER_LEVEL			99
//解决winsock冲突
#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#ifdef _WIN32
#pragma warning(disable:4996)
#endif

#ifdef _WIN32
#define usleep Sleep
#endif

#define EasyInline(type,x,obj) \
private: type m_##x##obj; \
public: inline type Get##obj() const {return m_##x##obj;}\
inline void Set##obj(type obj) {m_##x##obj = obj;}


#define EasyInlineReadOnly(type,x,obj) \
private: type m_##x##obj; \
public: inline type Get##obj() const {return m_##x##obj;}


