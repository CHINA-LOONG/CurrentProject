#pragma once
#include <string>
#include <time.h>
#include "common/const_def.h"
using namespace std;
struct Message {
	Message(int type, string &name)
		: type_(type), name_(name) {
			time_ = time(NULL);
	}
	Message (int type, string &name, int param_int)
		:type_(type), name_(name), param_int_(param_int) {
			time_ = time(NULL);
	}
	Message (int type, string &name, int64 fid1, int64 fid2) 
		: type_(type), name_(name), fid1_(fid1), fid2_(fid2) {
			time_ = time(NULL);
	}
	Message (int type, string &name, int64 fid)
		: type_(type), name_(name), fid1_(fid) {
			time_ = time(NULL);
	}
	Message (int type, int param_int, int64 fid1, int64 fid2)
		: type_(type), param_int_(param_int), fid1_(fid1), fid2_(fid2) {
			time_ = time(NULL);
	}
	Message (int type, int param_int, int64 fid1) 
		: type_(type), param_int_(param_int), fid1_(fid1) {
			time_ = time(NULL);
	}
	Message (int type, int param_int, int64 fid1, int64 fid2, int64 fid3)
		: type_(type), param_int_(param_int), fid1_(fid1), fid2_(fid2), fid3_(fid3) {
			time_ = time(NULL);
	}
	/*
	Message(int type, int64 uid): type_(type), uid_(uid), param_int_(0) {
		time_ = time(NULL);
	}
	Message(int type, int64 uid, int param_int): type_(type), uid_(uid), param_int_(param_int) {
		time_ = time(NULL);
	}
	
	Message(int type, int64 uid, long long fid1, long long fid2): type_(type), uid_(uid), fid1_(fid1), fid2_(fid2) {
		time_ = time(NULL);
	}
	
	Message(int type, int64 uid, long long fid): 
	type_(type), uid_(uid), fid1_(fid) {
		time_ = time(NULL);
	}
	*/

	int type_;
	time_t time_;
	string name_;
	int param_int_;	
	int64 fid1_;
	int64 fid2_;
	int64 fid3_;
	int64 param_ll_;
    bool operator== (const Message& f) {
		return (type_==f.type_ && /*uid_==f.uid_*/ name_==f.name_ && (type_==1||type_==4));
	}
};

