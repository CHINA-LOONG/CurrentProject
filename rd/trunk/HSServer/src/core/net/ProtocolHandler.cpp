#include "ProtocolHandler.h"

#include "common/string-util.h"
/*
bool parseCommandHeader(const vector<string> &tokens, event_header &eh) {
	if (tokens.size()<4) return false;
	if (tokens[1]=="0") {
		eh.type = EVENT_TYPE_ORG;
	} else if (tokens[1]=="1") {
		eh.type = EVENT_TYPE_TRANSFER;
	} else if (tokens[1]=="2") {
		eh.type = EVENT_TYPE_RESPONSE;
	} else return false;
	bool succ = true;
	succ = succ && safe_atoi(tokens[2].c_str(), eh.src);
	succ = succ && safe_atoi(tokens[3].c_str(), eh.dest);
	return succ;
}*/
void ProtocolHandler::leave(int64 uid)
{

}

