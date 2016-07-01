#include "EventQueue.h"
#include "common/string-util.h"

const string EventQueue::delims_ = ",";

EventQueue::EventQueue() {
	logger_ = log4cxx::Logger::getLogger("EventQueue");
	pthread_mutex_init(&mutex_, NULL);
	inner_counter_ = 0;
}
EventQueue::~EventQueue(void)
{
	pthread_mutex_destroy(&mutex_);
}

//////////////////////////////////////////////////////////////////////////
// general 
//////////////////////////////////////////////////////////////////////////
void EventQueue::pushStringEvent(const string &req, int worldFD /* = -1  */,int nHallSrvID /* = -1 */) {
	Event *e = allocateEvent();
	if (e->ParseFromString(req)) {
		if( worldFD >=0  )
		{
			e->set_fromworld_fd( worldFD ); 
		}
		if(nHallSrvID >0 )
		{
			e->set_hallsrvid(nHallSrvID);
		}
		safePushEvent(e);
	} else {
		LOG4CXX_ERROR(logger_, "Parse event error: \n"<<req);
		freeEvent(e);
	}
}
/*
//////////////////////////////////////////////////////////////////////////
// web server client use
//////////////////////////////////////////////////////////////////////////

void EventQueue::pushUserLogin(int nid, int fd, int state, const string& platid,
							   const string &name, const string &head,
							   int gender, int star,
							   const vector<string> &fpid, int sid, int siteid,
							   int i4IsYellowDmd, int i4IsYellowDmdYear, int i4YellowDmdLv) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(0);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_time(0);
	e->set_state(state);
	UserLoginParam *p = e->mutable_user_login_param();
	p->set_platform_id(platid);
	p->set_name(name);
	p->set_profile_link(head);
	p->set_gender(gender);
	p->set_star(star);
	p->clear_friends_platid();
	for (size_t i=0; i<fpid.size(); i++)
	{
		p->add_friends_platid(fpid[i]);
	}
	p->set_sid(sid);
	p->set_is_yellow_dmd(0!=i4IsYellowDmd);
	p->set_is_yellow_dmd_year(0!=i4IsYellowDmdYear);
	p->set_yellow_dmd_lv(i4YellowDmdLv);
	p->set_siteid(siteid);
	safePushEvent(e);
}

void EventQueue::pushPay(int nid, int fd, int state, const string& platid, 
						 int64 orderid, int amount, int sid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(15);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_time(0);
	e->set_state(state);
	PayParam *p = e->mutable_pay_param();
	p->set_amount(amount);
	p->set_order_id(orderid);
	p->set_platform_id(platid);
	p->set_sid(sid);
	safePushEvent(e);
}

void EventQueue::pushInvite(int nid, int fd, int state, 
							const string &strInviteUserPlatID, 
							const string& strInvitedUserPlatID, 
							int sid,int64 invitedUID,
							const string& strInvitedUserName)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(32);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_time(0);
	e->set_state(state);
	InviteParam *p = e->mutable_invite_param();
	p->set_sid(sid);
	p->set_strinviteuserplatid(strInviteUserPlatID);
	p->set_strinviteduserplatid(strInvitedUserPlatID);
	p->set_inviteduserid(invitedUID);
	p->set_strinvitedusername(strInvitedUserName);
	safePushEvent(e);
}
void EventQueue::pushUninstall(int nid, int fd, int state, const string& platid, int sid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(34);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_time(0);
	e->set_state(state);
	UninstallParam *p = e->mutable_uninstall_param();
	p->set_platform_id(platid);
	e->set_param_int(sid);
	safePushEvent(e);
}
void EventQueue::pushSyncFriend(int nid, int fd, int state, const string& platid, 
								const vector<string> &fpid, int siteid, int sid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(35);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_time(0);
	e->set_state(state);
	SyncFriendParam* p = e->mutable_sync_friend_param();
	p->set_platform_id(platid);
	for (size_t i=0; i < fpid.size(); i++)
	{
		p->add_friends_platid(fpid[i]);
	}
	p->set_sid(sid);
	p->set_siteid(siteid);
	safePushEvent(e);
}
void EventQueue::pushAcceptGift(int nid, int fd, int state, int64 uid1, int64 uid2, 
								int gtype, int gsubtype, int gamount, int sid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(42);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_time(0);
	e->set_state(state);
	AcceptGiftParam *p = e->mutable_accept_gift_param();
	p->set_uid1(uid1);
	p->set_uid2(uid2);
	p->set_gtype(gtype);
	p->set_gsubtype(gsubtype);
	p->set_gamount(gamount);
	p->set_sid(sid);
	safePushEvent(e);
}
void EventQueue::pushWebSee(int nid, int fd, int state, int type, 
							long long param, int sid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(50);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(type);
	e->set_param_ll_1(param);
	e->set_y(sid);
	safePushEvent(e);
}
//////////////////////////////////////////////////////////////////////////
// flash client use
//////////////////////////////////////////////////////////////////////////

void EventQueue::pushLoadUser(int nid, int state, int64 uid, int64 puid,
							  int firstload) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(2);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(puid);
	e->set_param_int(firstload);
	safePushEvent(e);
}
void EventQueue::pushLoadFish(int nid, int state, int64 uid, int64 tuid,
							  int64 tid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(3);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(tuid);
	// e->set_param_int(tid);
	e->set_param_ll_1(tid);
	safePushEvent(e);
}
void EventQueue::pushLoadTank(int nid, int state, int64 uid, int64 tuid,
							  int64 tid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(4);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(tuid);
	//e->set_param_int(tid);
	e->set_param_ll_1(tid);
	safePushEvent(e);
}
void EventQueue::pushFeed(int nid, int state, int64 uid, int64 fid, int fcat,
						  int ftype, int x, int y, int64 tuid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(5);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(tuid);

	FeedFishParam *p = e->mutable_feed_fish_param();
	p->set_x(x);
	p->set_y(y);
	p->set_cat(fcat);
	p->set_type(ftype);
	p->set_fid(fid);
	p->set_feedcount(1);
	safePushEvent(e);
}
void EventQueue::pushPickGold(int nid, int state, int64 uid, int64 tuid,
							  int64 tid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(6);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(tuid);
	//e->set_param_int(tid);
	e->set_param_ll_1(tid);
	safePushEvent(e);
}
void EventQueue::pushBuyFish(int nid, int state, int64 uid, int ftype,
							 int mtype) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(7);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(ftype);
	e->set_x(mtype);
	safePushEvent(e);
}
void EventQueue::pushFreeFish(int nid, int state, int64 uid, int64 fid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(8);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_ll_1(fid);
	safePushEvent(e);
}
void EventQueue::pushMateFish(int nid, int state, int64 uid1, int64 fid1,
							  int64 uid2, int64 fid2) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(9);
	e->set_src_node(nid);
	e->set_src_fd(uid1);
	e->set_time(0);
	e->set_state(state);
	e->set_param_ll_1(fid1);
	e->set_uid(uid2);
	e->set_param_ll_2(fid2);
	safePushEvent(e);
}
void EventQueue::pushPutFish(int nid, int state, int64 uid, int64 fid, int64 tid,
							 int x, int y, int64 src_tid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(10);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_ll_1(fid);
	e->set_param_ll_3(tid);
	e->set_x(x);
	e->set_y(y);
	e->set_param_ll_2(src_tid);
	safePushEvent(e);
}
void EventQueue::pushLeave(int nid, int state, int64 uid, int64 fd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(11);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}
void EventQueue::pushBuyFood(int nid, int state, int64 uid, int cat, int dtype,
							 int amount) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(12);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(cat);
	e->set_x(dtype);
	e->set_y(amount);
	safePushEvent(e);
}
void EventQueue::pushLoadMessage(int nid, int state, int64 uid, int mtype) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(17);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(mtype);
	safePushEvent(e);
}
void EventQueue::pushEnter(int nid, int state, int64 uid, string strIP) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(33);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(uid);
	e->set_response(strIP);
	safePushEvent(e);
}
void EventQueue::pushExpandTank(int nid, int state, int64 uid, int64 tid,
								int flimit, int type) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(24);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(flimit);
	e->set_param_ll_1(tid);
	e->set_y(type);
	safePushEvent(e);
}
void EventQueue::pushExpandBox(int nid, int state, int64 uid, int64 tid,
							   int gboxlevel, int dboxlevel, int etype) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(25);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(etype);
	e->set_x(gboxlevel);
	e->set_y(dboxlevel);
	e->set_param_ll_1(tid);
	safePushEvent(e);
}
void EventQueue::pushLoadHistory(int nid, int state, int64 uid, int64 puid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(28);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_ll_1(puid);
	safePushEvent(e);
}
void EventQueue::pushSaveDecoration(int nid, int state, int64 uid, int64 tid, 
									const vector<int64> &buy, const vector<int64> &change, 
									const vector<int64> &sell, const vector<int64> &other) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(37);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	//e->set_param_int(tid);
	e->set_param_ll_1(tid);
	SaveDecorationParam *p = e->mutable_save_decoration_param();
	p->clear_buy();
	for (size_t i=0; i<buy.size(); i++)
	{
		p->add_buy(buy[i]);
	}
	p->clear_change();
	for (size_t i=0; i<change.size(); i++)
	{
		p->add_change(change[i]);
	}
	p->clear_sell();
	for (size_t i=0; i<sell.size(); i++)
	{
		p->add_sell(sell[i]);
	}
	p->clear_other();
	for (size_t i=0; i<other.size(); i++)
	{
		p->add_other(other[i]);
	}
	safePushEvent(e);
}
void EventQueue::pushSaveRecord(int nid, int state, int64 uid, int key,
								const string& value) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(38);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(uid);
	e->set_param_int(key);
	// trick: response has other specially meaning, using it here just for efficiency.
	e->set_response(value);
	safePushEvent(e);
}
void EventQueue::pushLoadRecord(int nid, int state, int64 uid, int key, int subkey) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(39);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(uid);
	e->set_param_int(key);
	e->set_x(subkey);
	safePushEvent(e);
}
void EventQueue::pushBuyTank(int nid, int state, int64 uid, int tank_num, int diamond_buy) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(41);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_uid(uid);
	e->set_param_int(tank_num);
	e->set_x(diamond_buy);
	safePushEvent(e);
}

//////////////////////////////////////////////////////////////////////////
// flash admin client use
//////////////////////////////////////////////////////////////////////////
void EventQueue::pushAddSysMessage(int nid, int state, int64 uid, const string &msg) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(18);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	SysMessageParam *p = e->mutable_sys_message_param();
	p->set_msg(msg);
	safePushEvent(e);
}
void EventQueue::pushRemoveSysMessage(int nid, int state, int64 uid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(19);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}
void EventQueue::pushShowFrdAndDeco(int nid, int state, int64 uid,int nShowDeco,int nShowFrd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(68);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(nShowDeco);
	e->set_y(nShowFrd);
	safePushEvent(e);
}
void EventQueue::pushReloadConfig(int nid, int state, int64 uid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(20);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}
void EventQueue::pushAddGold(int nid, int state, int64 uid, int amount, int64 puid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(21);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(amount);
	e->set_param_ll_1(puid);
	safePushEvent(e);
}
void EventQueue::pushAddDiamond(int nid, int state, int64 uid, int amount, int64 puid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(22);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(amount);
	//e->set_x(puid);
	e->set_param_ll_1(puid);
	safePushEvent(e);
}
void EventQueue::pushAddExperience(int nid, int state, int64 uid, int level, int64 puid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(23);
	e->set_src_node(nid);
	e->set_time(0);
	e->set_src_fd(uid);
	e->set_param_int(level);
	//e->set_x(puid);
	e->set_param_ll_1(puid);
	e->set_state(state);
	safePushEvent(e);
}

void EventQueue::pushSaveCommand(int nid, int state, int64 uid, int type) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(26);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(type);
	safePushEvent(e);
}
void EventQueue::pushClearExperienceCounter(int nid, int state, int64 uid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(27);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}
void EventQueue::pushStatistics(int nid, int state, int64 uid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(31);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}
void EventQueue::pushSee(int nid, int state, int64 uid, int type, int64 param) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(36);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(type);
	e->set_param_ll_1(param);
	safePushEvent(e);
}

void EventQueue::pushUpdateExperience(int nid, int state, int64 uid, int64 puid, int exp)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(67);
	e->set_src_node(nid);
	e->set_time(0);
	e->set_src_fd(uid);
	e->set_param_int(exp);
	e->set_param_ll_1(puid);
	e->set_state(state);
	safePushEvent(e);
}

void EventQueue::pushLottery(int nid, int state, int64 uid, int type) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(70);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(type);
	safePushEvent(e);
}
void EventQueue::pushLotteryNineGrid(int nid, int state, int64 uid, int i4Index) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(43);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(i4Index);
	safePushEvent(e);
}
void EventQueue::pushLotteryInfo(int nid, int state, int64 uid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(57);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}
void EventQueue::pushLotteryNew(int nid, int state, int64 uid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(58);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}
void EventQueue::pushUseProps(int nid, int state, int64 uid, int type, int64 tid) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(48);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(type);
	// e->set_x(tid);
	e->set_param_ll_1(tid);
	safePushEvent(e);
}
void EventQueue::pushFriendRequest(int nid, int state, int64 uid, const string &platid, const string &fpid, PLAT_TYPE i4PlatType) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(49);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(0);
	e->set_state(state);
	FriendRequestParam *p = e->mutable_friend_request_param();
	p->set_platform_id(platid);
	p->set_friend_platid(fpid);
	p->set_plat_type(i4PlatType);
	safePushEvent(e);
}
//void EventQueue::pushFriendRequest(int nid, int state, int64 uid, int64 fuid) {
//	Event *e = ep_.safeAllocate();
//	e->set_cmd(49);
//	e->set_src_node(nid);
//	e->set_src_fd(uid);
//	e->set_time(0);
//	e->set_state(state);
//	e->set_param_ll_1(fuid);
//	safePushEvent(e);
//}

//////////////////////////////////////////////////////////////////////////
// EventHandler use, run in another thread, using 	ep_for_eventhandler_
// ATTENTION deallocation in freeEvent()
//////////////////////////////////////////////////////////////////////////
void EventQueue::pushUpdateFishes(int64 tuid, int64 tid, time_t time) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(13);
	e->set_src_node(-1);
	e->set_src_fd(-1);
	e->set_time(time);
	//e->set_param_int(tid);
	//e->set_x(tuid);
	e->set_param_ll_1(tid);
	e->set_param_ll_2(tuid);
	e->set_state(1);
	safePushEvent(e);
}
void EventQueue::pushSaveData(int nid, int state, int64 uid, time_t time, int type) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(14);
	e->set_src_node(nid);
	e->set_src_fd(uid);
	e->set_time(time);
	e->set_state(state);
	e->set_param_int(type);
	safePushEvent(e);
}

//////////////////////////////////////////////////////////////////////////
// GameNetHandler use
//////////////////////////////////////////////////////////////////////////
void EventQueue::pushUpdateWorkingStatus(int nid, int state, int param) {
	Event *e = ep_.safeAllocate();
	e->set_cmd(47);
	e->set_src_node(nid);
	e->set_src_fd(0);
	e->set_time(-2);
	e->set_state(state);
	e->set_param_int(param);
	safePushEvent(e);
}


void EventQueue::pushWorldBuyFish(int nid, int fd, int state, int64 uid,int mid,int dmd,int trueDmd,int sid,int leftdmd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(51);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(mid);
	e->set_y(dmd);
	e->set_param_ll_1(trueDmd);
	e->set_param_ll_2(leftdmd);
	e->set_param_ll_3(sid);
	safePushEvent(e);
}

void EventQueue::pushWorldBuyFood(int nid, int fd, int state, int64 uid,int ftype,int dmd,int trueDmd,int count,int sid,int leftdmd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(52);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(ftype);
	e->set_x(count);
	e->set_y(dmd);
	e->set_param_ll_1(trueDmd);
	e->set_param_ll_2(leftdmd);
	e->set_param_ll_3(sid);
	safePushEvent(e);
}

void EventQueue::pushWorldBuyDeco(int nid, int fd, int state, int64 uid,int64 tid,vector<int64>& buy,int sid,int leftdmd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(53);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_ll_1(tid);
	e->set_param_ll_2(leftdmd);
	e->set_param_ll_3(sid);
	SaveDecorationParam *p = e->mutable_save_decoration_param();
	p->clear_buy();
	p->clear_change();
	p->clear_other();
	p->clear_sell();
	for (size_t i=0; i<buy.size(); i++)
	{
		p->add_buy(buy[i]);
	}
	p->add_change(0);
	p->add_sell(0);
	p->add_other(0);
	safePushEvent(e);
}

void EventQueue::pushWorldExpandTank(int nid,int fd,int state, int64 uid,int limit,int dmd,int sid,int leftdmd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(56);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(limit);
	e->set_y(dmd);
	e->set_param_ll_2(leftdmd);
	e->set_param_ll_3(sid);
	safePushEvent(e);
}
void EventQueue::pushWorldExpandBox(int nid,int fd,int state, int64 uid,int dLevel,int dmd,int sid,int leftdmd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(54);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(dLevel);
	e->set_y(dmd);
	e->set_param_ll_2(leftdmd);
	e->set_param_ll_3(sid);
	safePushEvent(e);
}

void EventQueue::pushWorldAddTank(int nid,int fd,int state,int64 uid,int level,int dmd,int sid,int leftdmd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(55);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(level);
	e->set_y(dmd);
	e->set_param_ll_2(leftdmd);
	e->set_param_ll_3(sid);
	safePushEvent(e);
}

void EventQueue::pushUseYanzheng(int nid, int state, int64 uid, string& strYanzheng)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(59);
	e->set_src_node(nid);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_response(strYanzheng);
	safePushEvent(e);
}

void EventQueue::pushUseNewYZ(int nid, int state, int64 uid)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(60);
	e->set_src_node(nid);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}

void EventQueue::pushEventNoParam( int nid, int state, int64 uid, int i4EventIndex )
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(i4EventIndex);
	e->set_src_node(nid);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	safePushEvent(e);
}

void EventQueue::pushEventWithParamX(int nid, int state, int64 uid, int i4EventIndex, int i4Param_X)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(i4EventIndex);
	e->set_src_node(nid);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(i4Param_X);
	safePushEvent(e);
}


void EventQueue::pushEventWithParamsXY(int nid, int state, int64 uid, int i4EventIndex, int i4Param_X, int i4Param_Y)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(i4EventIndex);
	e->set_src_node(nid);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(i4Param_X);
	e->set_y(i4Param_Y);
	safePushEvent(e);
}

void EventQueue::pushSetRestrict(int nid, int fd, int state, int64 uid,int sid,int64 time)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(64);
	e->set_src_fd(fd);
	e->set_src_node(nid);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_ll_1(time);
	e->set_param_ll_2(sid);
	safePushEvent(e);
}

void EventQueue::pushWorldBuyGiftPack(int nid, int fd, int state, int64 uid,int ftype,int dmd,int trueDmd,int count,int sid,int leftdmd)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(65);
	e->set_src_node(nid);
	e->set_src_fd(fd);
	e->set_uid(uid);
	e->set_time(0);
	e->set_state(state);
	e->set_param_int(ftype);
	e->set_x(count);
	e->set_y(dmd);
	e->set_param_ll_1(trueDmd);
	e->set_param_ll_2(leftdmd);
	e->set_param_ll_3(sid);
	safePushEvent(e);
}

void EventQueue::pushTestInterface(int nid, int state, int64 uid, int64 tuid,int nType, int nParam, int64 llParam)
{
	Event *e = ep_.safeAllocate();
	e->set_cmd(69);
	e->set_src_fd(uid);
	e->set_src_node(nid);
	e->set_uid(tuid);
	e->set_time(0);
	e->set_state(state);
	e->set_x(nType);
	e->set_y(nParam);
	e->set_param_ll_1(llParam);
	safePushEvent(e);
}
*/

