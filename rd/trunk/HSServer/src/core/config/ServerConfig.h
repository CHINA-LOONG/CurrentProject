#pragma once
#include <vector>
#include <string>
#include <set>
#include "common/const_def.h"

#ifdef _WIN32
#pragma warning(disable: 4290)
#include <libconfig.hpp>
#else
#include <libconfig.h++>
#endif

const char SZSYSLOGCFG[] = "config/syslog.cfg";

using namespace std;
using namespace libconfig;

class ServerConfig
{
public:
	ServerConfig(const char* server_file);
public:
	~ServerConfig(void);

	bool loadFromFile();
	const string& getVersion(){return version_;}

	bool useShareMem(){return usersharemem_;}

	int pksrv_num() { return pksrv_num_; };
	int PksrvID( int index )	{ return pksrv_id_[index]; }; 
	int pksrv_gamed_Port(int id); 
	const string pksrv_server_address(int id) ; 
	const string pksrv_bind_ip(int id); 

	int worlddNum() { return worldd_num_; }
	int worlddId(int id) {return world_id_[id];};
	const string worlddIp(int id);
	const string worlddBindIp(int id);
	int worlddGamedPort(int id);
	int worlddWebPort(int id);

	int gamedNum() {return gamed_num_;}
	const string gamedIp(int nid);
	int gamedPort(int nid);
	const string gamedBindIp(int nid);
	int gamedServerPort(int gid) {return gamed_server_port_[gid];}
	int gamedAdminPort(int gid) {return gamed_admin_port_[gid];}
	int gamedPolicyPort(int gid) {return gamed_policy_port_[gid];}

	int dbNum() {return db_num_;}
	const string& dbUserAddr1(int gid) {return db_user_addr_1[gid];}
	int dbUserPort1(int gid) {return db_user_port_1[gid];}
	const string& dbPlatAddr1(int gid) {return db_plat_addr_1[gid];}
	int dbPlatPort1(int gid) {return db_plat_port_1[gid];}
	const string& dbUserAddr2(int gid) {return db_user_addr_2[gid];}
	int dbUserPort2(int gid) {return db_user_port_2[gid];}
	const string& dbPlatAddr2(int gid) {return db_plat_addr_2[gid];}
	int dbPlatPort2(int gid) {return db_plat_port_2[gid];}

	const string& dbUserAddr3(int gid) {return db_user_addr_3[gid];}
	int dbUserPort3(int gid) {return db_user_port_3[gid];}

	const string& dbUserAddr4(int gid) {return db_user_addr_4[gid];}
	int dbUserPort4(int gid) {return db_user_port_4[gid];}

	const string& dbPlatAddr4(int gid) {return db_plat_addr_4[gid];}
	int dbPlatPort4(int gid) {return db_plat_port_4[gid];}

	int proxyNum() {return proxy_num_;}
	const string proxyIp(int pid) {return proxy_addr[pid];}
	int proxyBindPort1(int pid) {return proxy_port_1[pid];}
	int proxyBindPort2(int pid) {return proxy_port_2[pid];}
	int proxyPolicyPort(int pid) {return proxy_policy_port[pid];}


	//日志系统相关函数
	bool		 IsGameLogStart(int gid){return gamed_log_start_[gid]>0;}
	const string GetGameLogDir(int gid) {return gamed_log_dir_[gid];}
	const string GetGameLogName(int gid){return gamed_log_name_[gid];}
	int			 GetGameLogStatLv(int gid){return gamed_log_stat_level_[gid];}
	int			 GetGameLogStatModul(int gid){return gamed_log_stat_modul_[gid];}
	int			 GetGameLogStatModulVal(int gid){return gamed_log_stat_modul_val_[gid];}
	const string GetGameLog2SrvAddr(int gid){return gamed_log_stat_addr_[gid];}
	const string GetGameLog2SrvPort(int gid){return gamed_log_stat_port_[gid];}
	
	bool		 IsHallLogStart(int gid){return hall_log_start_[gid]>0;}
	const string GetHallLogDir(int gid) {return hall_log_dir_[gid];}
	const string GetHallLogName(int gid){return hall_log_name_[gid];}
	int			 GetHallLogStatLv(int gid){return hall_log_stat_level_[gid];}
	//int			 GetHallLogStatModul(int gid){return hall_log_stat_modul_[gid];}
	//int			 GetHallLogStatModulVal(int gid){return hall_log_stat_modul_val_[gid];}
	const string GetHallLog2SrvAddr(int gid){return hall_log_stat_addr_[gid];}
	const string GetHallLog2SrvPort(int gid){return hall_log_stat_port_[gid];}

private:
	string version_;
	bool usersharemem_;
	int worldd_num_;
	vector<int> world_id_;
	map<int, string> world_bind_ip_;
	map<int, string> world_server_address_;
	map<int, int> world_gamed_port_;
	map<int, int> world_web_server_port_;

	//pk srv
	int pksrv_num_;
	vector<int> pksrv_id_;
	map<int, string> pksrv_bind_ip_;
	map<int, string> pksrv_server_address_;
	map<int, int> pksrv_gamed_port_;

	int gamed_num_;
	map<int, string> gamed_ip_;
	map<int, int> gamed_port_;
	map<int, string> gamed_bind_ip_;
	map<int, int> gamed_server_port_;
	map<int, int> gamed_admin_port_;
	map<int, int> gamed_policy_port_;

	int db_num_;
	map<int, string> db_user_addr_1;
	map<int, int>    db_user_port_1;
	map<int, string> db_plat_addr_1;
	map<int, int>    db_plat_port_1;
	map<int, string> db_user_addr_2;
	map<int, int>    db_user_port_2;
	map<int, string> db_plat_addr_2;
	map<int, int>    db_plat_port_2;

	map<int, string> db_user_addr_3;
	map<int, int>    db_user_port_3;

	map<int, string> db_user_addr_4;
	map<int, int>    db_user_port_4;

	map<int, string> db_plat_addr_4;
	map<int, int>    db_plat_port_4;


	int proxy_num_;
	map<int, string> proxy_addr;
	map<int, int> proxy_port_1;
	map<int, int> proxy_port_2;
	map<int, int> proxy_policy_port;

	//log stat args:
	map<int, string> gamed_log_dir_;
	map<int, string> gamed_log_name_;
	map<int, int>	 gamed_log_start_;
	map<int, int>    gamed_log_stat_level_;
	map<int, int>    gamed_log_stat_modul_;
	map<int, int>    gamed_log_stat_modul_val_;
	map<int, string> gamed_log_stat_addr_;
	map<int, string> gamed_log_stat_port_;

	map<int, string> hall_log_dir_;
	map<int, string> hall_log_name_;
	map<int, int>	 hall_log_start_;
	map<int, int>    hall_log_stat_level_;
	//map<int, int>    hall_log_stat_modul_;
	//map<int, int>    hall_log_stat_modul_val_;
	map<int, string> hall_log_stat_addr_;
	map<int, string> hall_log_stat_port_;

	string server_file_;
	Config srv;
};
