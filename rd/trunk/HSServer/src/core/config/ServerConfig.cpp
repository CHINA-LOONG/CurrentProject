#include <time.h>
#include "ServerConfig.h"

ServerConfig::ServerConfig(const char* server_file): server_file_(server_file)
{
	if (!loadFromFile()) {
		printf("WARNING! read config file failed.\n");
		exit(1);
	}
}

ServerConfig::~ServerConfig(void)
{
}

bool ServerConfig::loadFromFile()
{
	//server.cfg
	try
	{
		srv.readFile(server_file_.c_str());
		srv.lookupValue("version",version_);
		usersharemem_ = false;
		srv.lookupValue("usesharemem",usersharemem_);
		printf("use_share_mem:%d\n",usersharemem_);
		// worldd
		world_id_.clear();
		world_web_server_port_.clear();
		world_server_address_.clear();
		world_gamed_port_.clear();
		world_bind_ip_.clear();
		worldd_num_ = srv.lookup("app.worldd_num");
		Setting &worldd = srv.lookup("app.worldd");
		for( int i =0; i< worldd_num_; i++ )
		{
			int id = worldd[i]["id"];
			world_id_.push_back(id);
			world_web_server_port_.insert(make_pair(id, (int)worldd[i]["world_web_server_port"]));
			world_server_address_.insert(make_pair(id, (const char*)worldd[i]["world_server_address"]));
			world_gamed_port_.insert(make_pair(id,(int) worldd[i]["world_gamed_port"]));
			world_bind_ip_.insert(make_pair(id, (const char*)worldd[i]["world_bind_ip"]));
		}

		// pksrv
		pksrv_id_.clear();
		pksrv_server_address_.clear();
		pksrv_gamed_port_.clear();
		pksrv_bind_ip_.clear();
		pksrv_num_ = srv.lookup("app.pksrv_num");
		Setting &pksrv = srv.lookup("app.pksrv");
		for ( int i = 0; i < pksrv_num_; i++ )
		{
			int id = pksrv[i]["id"];
			pksrv_id_.push_back( id );
			pksrv_server_address_.insert( make_pair( id, (const char*) pksrv[i]["pksrv_server_address"]));
			pksrv_gamed_port_.insert( make_pair( id, (int) pksrv[i]["pksrv_gamed_port"]));
			pksrv_bind_ip_.insert( make_pair( id, (const char*) pksrv[i]["pksrv_bind_ip"]));
		}

		// proxy
		proxy_addr.clear();
		proxy_port_1.clear();
		proxy_port_2.clear();
		proxy_policy_port.clear();
		proxy_num_ = srv.lookup("app.proxy_num");
		Setting &proxyd = srv.lookup("app.proxy");
		for(int i =0; i< proxy_num_; i++)
		{
			int id = proxyd[i]["id"];
			proxy_addr.insert(make_pair(id, (const char*)proxyd[i]["proxy_address"]));
			proxy_port_1.insert(make_pair(id, (int)proxyd[i]["proxy_port_1"]));
			proxy_port_2.insert(make_pair(id,(int) proxyd[i]["proxy_port_2"]));
			proxy_policy_port.insert(make_pair(id, (int)proxyd[i]["proxy_policy_port"]));
		}

		// gamed
		gamed_ip_.clear();
		gamed_port_.clear();
		gamed_bind_ip_.clear();
		gamed_server_port_.clear();
		gamed_policy_port_.clear();
		gamed_admin_port_.clear();
		gamed_num_ = srv.lookup("app.gamed_num");
		printf("gamed_num:%d\n", gamed_num_);
		Setting &gamed = srv.lookup("app.gamed");

		for (int i=0; i<gamed_num_; i++) {
			int gamed_id = gamed[i]["id"];
			gamed_ip_.insert(make_pair(gamed_id, (const char *)gamed[i]["addr"]));
			gamed_port_.insert(make_pair(gamed_id, (int) gamed[i]["port"]));
			gamed_bind_ip_.insert(make_pair(gamed_id, (const char*) gamed[i]["bind_addr"]));
			gamed_server_port_.insert(make_pair(gamed_id, (int)gamed[i]["server_port"]));
			gamed_policy_port_.insert(make_pair(gamed_id, (int)gamed[i]["policy_port"]));
			gamed_admin_port_.insert(make_pair(gamed_id, (int)gamed[i]["admin_port"]));
		}

		// database
		db_user_addr_1.clear();
		db_user_port_1.clear();
		db_user_addr_2.clear();
		db_user_port_2.clear();
		
		db_user_addr_3.clear();
		db_user_port_3.clear();
		db_user_addr_4.clear();
		db_user_port_4.clear();

		db_plat_addr_1.clear();
		db_plat_port_1.clear();
		db_plat_addr_2.clear();
		db_plat_port_2.clear();

		db_plat_addr_4.clear();
		db_plat_port_4.clear();

		db_num_ = srv.lookup("app.db_num");
		printf("db_num:%d\n", db_num_);
		Setting &db = srv.lookup("app.database");

		for (int i=0; i<db_num_; i++) {
			int db_id = db[i]["id"];
			db_user_addr_1.insert(make_pair(db_id, (const char*)db[i]["addr_user_1"]));
			db_user_port_1.insert(make_pair(db_id, (int)db[i]["port_user_1"]));
			db_plat_addr_1.insert(make_pair(db_id, (const char*)db[i]["addr_plat_1"]));
			db_plat_port_1.insert(make_pair(db_id, (int)db[i]["port_plat_1"]));
			db_user_addr_2.insert(make_pair(db_id, (const char*)db[i]["addr_user_2"]));
			db_user_port_2.insert(make_pair(db_id, (int)db[i]["port_user_2"]));
			db_plat_addr_2.insert(make_pair(db_id, (const char*)db[i]["addr_plat_2"]));
			db_plat_port_2.insert(make_pair(db_id, (int)db[i]["port_plat_2"]));

			db_user_addr_3.insert(make_pair(db_id, (const char*)db[i]["addr_user_3"]));
			db_user_port_3.insert(make_pair(db_id, (int)db[i]["port_user_3"]));

			db_user_addr_4.insert(make_pair(db_id, (const char*)db[i]["addr_user_4"]));
			db_user_port_4.insert(make_pair(db_id, (int)db[i]["port_user_4"]));

			db_plat_addr_4.insert(make_pair(db_id, (const char*)db[i]["addr_plat_4"]));
			db_plat_port_4.insert(make_pair(db_id, (int)db[i]["port_plat_4"]));
		}
	}
	catch (ParseException ex)
	{
		printf("Parsing config file %s failed.\n", server_file_.c_str());
		return false;
	}
	catch (FileIOException ex) {
		printf("Read config file %s failed. IOExcetpion.\n", server_file_.c_str());
		return false;
	}
	catch (SettingNotFoundException ex) {
		printf("Read config file %s failed. Setting \"%s\" not found.\n", server_file_.c_str());
		return false;
	}

	//syslog.cfg
	try
	{
		srv.readFile(SZSYSLOGCFG);
		gamed_log_dir_.clear();
		gamed_log_name_.clear();
		gamed_log_start_.clear();
		gamed_log_stat_level_.clear();
		gamed_log_stat_modul_.clear();
		gamed_log_stat_modul_val_.clear();
		gamed_log_stat_addr_.clear();
		gamed_log_stat_port_.clear();

		hall_log_dir_.clear();
		hall_log_name_.clear();
		hall_log_start_.clear();
		hall_log_stat_level_.clear();
		//hall_log_stat_modul_.clear();
		//hall_log_stat_modul_val_.clear();
		hall_log_stat_addr_.clear();
		hall_log_stat_port_.clear();

		//以server.cfg中的gamed_num为准
		int gamed_num = srv.lookup("app.gamed_num");
		Setting &gamed = srv.lookup("app.gamed");
		for (int i=0; i<gamed_num_; i++) {
			int gamed_id = gamed[i]["id"];
			gamed_log_start_.insert(make_pair(gamed_id, (int)gamed[i]["log_file_start"]));
			gamed_log_dir_.insert(make_pair(gamed_id, (const char *)gamed[i]["log_file_dir"]));
			gamed_log_name_.insert(make_pair(gamed_id, (const char *)gamed[i]["log_file_name"]));
			gamed_log_stat_level_.insert(make_pair(gamed_id, (int)gamed[i]["send_lv"]));
			gamed_log_stat_modul_.insert(make_pair(gamed_id, (int)gamed[i]["user_mod"]));
			gamed_log_stat_modul_val_.insert(make_pair(gamed_id, (int)gamed[i]["user_mod_val"]));
			gamed_log_stat_addr_.insert(make_pair(gamed_id, (const char *)gamed[i]["statsrv_addr"]));
			gamed_log_stat_port_.insert(make_pair(gamed_id, (const char *)gamed[i]["statsrv_port"]));
		}

		int hall_num = srv.lookup("app.hall_num");
		Setting &hall = srv.lookup("app.hall");
		for (int i=0; i<hall_num; i++) {
			int hall_id = hall[i]["id"];
			hall_log_start_.insert(make_pair(hall_id, (int)hall[i]["log_file_start"]));
			hall_log_dir_.insert(make_pair(hall_id, (const char *)hall[i]["log_file_dir"]));
			hall_log_name_.insert(make_pair(hall_id, (const char *)hall[i]["log_file_name"]));
			hall_log_stat_level_.insert(make_pair(hall_id, (int)hall[i]["send_lv"]));
			//hall_log_stat_modul_.insert(make_pair(hall_id, (int)hall[i]["user_mod"]));
			//hall_log_stat_modul_val_.insert(make_pair(hall_id, (int)hall[i]["user_mod_val"]));
			hall_log_stat_addr_.insert(make_pair(hall_id, (const char *)hall[i]["statsrv_addr"]));
			hall_log_stat_port_.insert(make_pair(hall_id, (const char *)hall[i]["statsrv_port"]));
		}
	}
	

	catch (ParseException ex)
	{
		printf("Parsing config file %s failed.%d\n",SZSYSLOGCFG);
		return false;
	}
	catch (FileIOException ex) {
		gamed_log_dir_.clear();
		gamed_log_name_.clear();
		gamed_log_start_.clear();
		gamed_log_stat_level_.clear();
		gamed_log_stat_modul_.clear();
		gamed_log_stat_modul_val_.clear();
		gamed_log_stat_addr_.clear();
		gamed_log_stat_port_.clear();

		printf("Parsing config file %s failed.\n", SZSYSLOGCFG);
		//return false;
	}
	catch (SettingNotFoundException ex) {
		printf("Parsing config file %s failed.%d\n", SZSYSLOGCFG);
		return false;
	}

	return true;
}

int ServerConfig::pksrv_gamed_Port(int id) 
{
	map<int, int>::const_iterator iter = pksrv_gamed_port_.find(id);
	if (iter==pksrv_gamed_port_.end()) 
		return 0;
	return iter->second; 
}; 

const string ServerConfig::pksrv_server_address(int id) 
{
	map<int, string>::const_iterator iter = pksrv_server_address_.find(id);
	if (iter==pksrv_server_address_.end()) 
		return "";
	return iter->second; 
}; 

const string ServerConfig::pksrv_bind_ip(int id)
{
	map<int, string>::const_iterator iter = pksrv_bind_ip_.find(id);
	if (iter==pksrv_bind_ip_.end()) 
		return "";
	return iter->second; 
}; 

int ServerConfig::worlddGamedPort(int id) 
{
	map<int, int>::const_iterator iter = world_gamed_port_.find(id);
	if (iter==world_gamed_port_.end()) 
		return 0;
	return iter->second; 
}

int ServerConfig::worlddWebPort(int id)
{
	map<int, int>::const_iterator iter = world_web_server_port_.find(id);
	if (iter==world_web_server_port_.end()) 
		return 0;
	return iter->second; 
}

const string ServerConfig::worlddIp(int id) 
{
	map<int, string>::const_iterator iter = world_server_address_.find(id);
	if (iter==world_server_address_.end()) 
		return "";
	return iter->second; 
}

const string ServerConfig::worlddBindIp(int id)
{
	map<int, string>::const_iterator iter = world_bind_ip_.find(id);
	if (iter==world_bind_ip_.end()) 
		return "";
	return iter->second; 
}

const string ServerConfig::gamedIp(int id) {
	map<int, string>::const_iterator iter = gamed_ip_.find(id);
	if (iter==gamed_ip_.end())
		return "";
	return iter->second;
}

int ServerConfig::gamedPort(int id) {
	map<int, int>::const_iterator iter = gamed_port_.find(id);
	if (iter==gamed_port_.end())
		return 0;
	return iter->second;
}

const string ServerConfig::gamedBindIp(int id) {
	map<int, string>::const_iterator iter = gamed_bind_ip_.find(id);
	if (iter==gamed_bind_ip_.end())
		return "";
	return iter->second;
}
