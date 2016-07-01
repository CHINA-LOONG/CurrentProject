#pragma once

#include <map>
#include <string>

using namespace std;

class Counter {
public:
	void clearAll();
	void clear(const string &name);
	int set(const string &name, int value);
	int increase(const string &name);
	int decrease(const string &name);
	int count(const string &name);
	map<string, int>&	getCounterMap(){return counter_;}
	static Counter* GetInst();
private:
	map<string, int> counter_;
	typedef map<string, int>::iterator counter_iter;
	static Counter* m_pInst;
};

inline Counter* Counter::GetInst()
{
	if (m_pInst == NULL)
	{
		m_pInst = new Counter;
	}
	return m_pInst;
}

enum TEST_TYPE
{
	E_WORLD_HANDLER = 0,
	E_POLICY_HANDLER,
	E_CLIENT_HANDLER,
	E_EVENT_HANDLER,
	E_FRIEND_HANDLER,
	E_GAME_EVENT_HANDLER,
	E_GAME_DATE_HANDLER,
	E_GAME_NET_HANDLER,
	E_EVENT_QUEUE,
	E_PLAYER,
	E_USER,
	E_SHELF,
	E_GUEST_RECALL,
	E_BOX_LOTTERY,
	E_LIST_NODE,
	E_PROCESS_BUYING,
	E_MAP,
	E_GUESTGROUP,
	E_SHELF_MANAGER,
	E_EMPLOYEE_DB,
	E_EMPLOYEE_PROCESS,
	E_EMPLOYEE_RULE,
	E_EMPLOYEE_SKILL,
	E_ASTAR_RECT,
	E_ASTAR_WIDTH,
	E_ASTAR_HEIGHT,
	E_SAVER_STRING,
	E_GAME_FRIENDINFO,
	E_DATE_SAVER,
	E_INI_TEMP,
	E_INI_STR,
	E_INI_RET,
	E_INI_INDEXLIST,
	E_NET_CACHE,
	E_NET_CACHE_NW,
	E_NET_CACHE_CMD,
	E_NET_CACHE_R,
	E_NET_CACHE_W,
	E_SYSLOG,
	E_ASTAR,
	E_EVENT,
	E_INI_MSTR,
	E_MAX,
};
extern int TEST[E_MAX];
