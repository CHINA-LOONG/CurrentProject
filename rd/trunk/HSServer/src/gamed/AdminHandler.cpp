#define _CRT_SECURE_NO_WARNINGS
#include "AdminHandler.h"
#include <stdarg.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <vector>
#include <string>
#include "../common/string-util.h"
#include "GameNetHandler.h"
#include "../net/NetCache.h"
#include "../event/EventQueue.h"
#include "event/DealAdminEvent.h"

using namespace std;

AdminHandler::AdminHandler(GameNetHandler *nh, int fd, int nid)
{
	this->nh = nh;
	this->fd = fd;
	this->nid_ = nid;
	welcome();
}

AdminHandler::~AdminHandler(void)
{
}

int AdminHandler::handlerType()
{
	return ProtocolHandler::ADMIN;
}

void AdminHandler::print(bool prompt, const char* format, ...)
{
	char buf[1024];
	va_list args;
	va_start(args, format);
	vsprintf(buf, format, args);
	va_end(args);
	if(prompt)
	{
		strcat(buf, "\r\nBombBaby# ");
	}

	nh->sendFdString(fd, buf, strlen(buf));
}

void AdminHandler::welcome()
{
	print(true, "Welcome!\r\n");
}

void AdminHandler::invalid()
{
	print(true, "Invalid command!\r\n");
}

void AdminHandler::handle(int64 uid, string &req){}
/*
{
	if (req[0]==(char)255) // process telnet commands
	{
		if (req[1]==(char)0xfb && req[2]==(char)0x22) // WILL LINEMODE
		{
			req[1] = (char)0xfd;
			print(true, req.c_str());
		}
	}
	else
	{
		vector<string> tokens;
		static string delims = ",";
		tokenize(req, tokens, delims);
		if (tokens.size()<1)
		{
			invalid();
			return;
		}
		string cmd = tokens[0];
		tokens.erase(tokens.begin());
		if (cmd=="exit")
		{
			print(false, "bye");
			nh->closeConnection(fd);
		}
		else if (checkCmd(cmd, "addexp", tokens, 2))
		{
			int64 puid;
			int exp;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), exp))
			{
				DealAdminEvent::AddEvent_AddExp(nh->eq, nid_, fd, puid, exp);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "addgold", tokens, 2))
		{
			int64 puid;
			int gold;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), gold))
			{
				DealAdminEvent::AddEvent_AddGold(nh->eq, nid_, fd, puid, gold);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "addprop", tokens, 4))
		{
			int64 puid;
			int prop;
			int amount;
			int level;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), prop) &&
				safe_atoi(tokens[2].c_str(), level) &&
				safe_atoi(tokens[3].c_str(), amount))
			{
				DealAdminEvent::AddEvent_AddProp(nh->eq, nid_, fd, puid, prop, level, amount);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "setlevel", tokens, 2))
		{
			int64 puid;
			int level;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), level))
			{
				DealAdminEvent::AddEvent_SetLevel(nh->eq, nid_, fd, puid, level);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "setequip", tokens, 8))
		{
			int64 puid;
			int infoID;
			int postion;
			int equipLevel;
			int atkLevel;
			int defLevel;
			int aglLevel;
			int luckLevel;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), infoID) &&
				safe_atoi(tokens[2].c_str(), postion) &&
				safe_atoi(tokens[3].c_str(), equipLevel) &&
				safe_atoi(tokens[4].c_str(), atkLevel) &&
				safe_atoi(tokens[5].c_str(), defLevel) &&
				safe_atoi(tokens[6].c_str(), aglLevel) &&
				safe_atoi(tokens[7].c_str(), luckLevel))
			{
				DealAdminEvent::AddEvent_SetEquip(nh->eq, nid_, fd, puid, infoID, postion, equipLevel, atkLevel, defLevel, aglLevel, luckLevel);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "userinfo", tokens, 1))
		{
			int64 puid;
			if (safe_atoll(tokens[0].c_str(), puid))
			{
				DealAdminEvent::AddEvent_UserInfo(nh->eq, nid_, fd, puid);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "delprop", tokens, 2))
		{
			int64 puid;
			int postion;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), postion))
			{
				DealAdminEvent::AddEvent_DelProp(nh->eq, nid_, fd, puid, postion);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "runtask", tokens, 3))
		{
			int64 puid;
			int taskid;
			int taskstep;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), taskid) &&
				safe_atoi(tokens[2].c_str(), taskstep))
			{
				DealAdminEvent::AddEvent_RunTask(nh->eq, nid_, fd, puid, taskid, taskstep);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "banchat", tokens, 3))
		{
			int64 puid;
			int isban;
			int min;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), isban) &&
				safe_atoi(tokens[2].c_str(), min))
			{
				DealAdminEvent::AddEvent_BanChat(nh->eq, nid_, fd, puid, isban, min);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "banlogin", tokens, 3))
		{
			int64 puid;
			int isban;
			int min;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), isban) &&
				safe_atoi(tokens[2].c_str(), min))
			{
				DealAdminEvent::AddEvent_BanLogin(nh->eq, nid_, fd, puid, isban, min);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "addpoint", tokens, 2))
		{
			int64 puid;
			int point;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), point))
			{
				DealAdminEvent::AddEvent_AddPoint(nh->eq, nid_, fd, puid, point);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "addindate", tokens, 4))
		{
			int64 puid;
			int infoID;
			int postion;
			int hour;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), infoID) &&
				safe_atoi(tokens[2].c_str(), postion) &&
				safe_atoi(tokens[3].c_str(), hour))
			{
				DealAdminEvent::AddEvent_AddIndate(nh->eq, nid_, fd, puid, infoID, postion, hour);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "freshuser", tokens, 1))
		{
			int64 puid;
			if (safe_atoll(tokens[0].c_str(), puid))
			{
				DealAdminEvent::AddEvent_Freshuser(nh->eq, nid_, fd, puid);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "addmedal", tokens, 2))
		{
			int64 puid;
			int medal;
			if (safe_atoll(tokens[0].c_str(), puid) &&
				safe_atoi(tokens[1].c_str(), medal))
			{
				DealAdminEvent::AddEvent_AddMedal(nh->eq, nid_, fd, puid, medal);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "broadcast", tokens, 1))
		{
			if (tokens[0].c_str())
			{
				DealAdminEvent::AddEvent_BroadCast(nh->eq, nid_, fd, tokens[0]);
			}
			else
			{
				invalid();
			}
		}
		else if (checkCmd(cmd, "reload", tokens, 0))
		{
			DealAdminEvent::AddEvent_Reload(nh->eq, nid_, fd);
		}


		else if (checkCmd(cmd, "adrl", tokens, 0)) {
			nh->eq->pushReloadConfig(nid_, 1, -fd);
		}
		else if (checkCmd(cmd, "adc", tokens, 2)) {
			nh->eq->pushSee(nid_, 1, -fd, type, param);
		}

		// ------------ Client admin commands ends here ------------

		else {
			invalid();
		}
	}
}
*/

