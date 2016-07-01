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

//�Ա�
const int Gender_NULL = 0;
const int Gender_Girl = 1;
const int Gender_Boy  = 2;
const int Gender_ALL  = 3;
// ƽ̨���ͣ���ӦWEB��siteid

enum PLAT_TYPE
{
    PLAT_XIAOYOU = 0,	//У��
    PLAT_QZONE,			//�ռ�
    PLAT_WEIBO,         //΢��
    PLAT_WAP,           //WAP
    PLAT_MANYOU,        //����
    PLAT_UNKNOW1,        //
    PLAT_UNKNOW2,
    PLAT_UNKNOW3,
    PLAT_UNKNOW4,
    PLAT_TYPE_MAX
} ;

//��սģʽ

enum BattleMode
{
    BM_Persion = 0,		//����ģʽ
    BM_Group,			//���ģʽ
    BM_Count,
} ;
//��������

enum RoomType
{
    RT_Athletics = 0,	//����
    RT_Challenge,		//��ս
    RT_Count,
} ;
//ѡ�񷿼�ģʽ

enum EnterMode
{
    EM_Rand = 0,		//�������
    EM_Create,			//��������
    EM_Find,			//���ҷ������
    EM_Quick,			//���˿��ٽ���
} ;

//�����������ģʽ

enum HallRoomEnterMode
{
    HRM_EnterHall = 0,
    HRM_LeaveHall,
    HRM_EnterRoom,
    HRM_LeaveRoom,
    LeaveRoom_Normal,	//�뿪����
    LeaveRoom_Enter,		//�л�����
    LeaveRoom_CutDown,		//����������

    LeaveHall_Normal,		//�뿪����
    LeaveHall_Enter,		//�л�����
    LeaveRoom_LeaveHall,	//�뿪����

} ;

enum ChatType
{
    // 0:��ǰ 1:˽�� 2:���� 3:С���� 4:������ 5:С��
    Chat_Current = 0,
    Chat_Persion,
    Chat_Union,
    Chat_SmallBugle,
    Chat_BigBugle,
    Chat_Group,
} ;

const int MAX_HALLSRV_COUNT		= 1000;	//����������
const int CHANNEL_OFFSET		= 100;		//Ƶ��ƫ����
const int ROOM_OFFSET			= 1000;		//����ƫ����
const int HALLSRV_OFFSET		= CHANNEL_OFFSET*ROOM_OFFSET;

const int EXPIATE_FLAG_GOLDPOINT = 0x01;		//��ȯ������־
const int EXPIATE_FLAG_STONE	= 0x02;		//2011.03.30������־
const int EXPIATE_FLAG_GOLDBOX	= 0x04;		//�ȼ����䲹����־

#define Locat_CH "CH_QZ"
#define Locat_US "US_FB"
#define MAX_USER_LEVEL			99
//���winsock��ͻ
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


