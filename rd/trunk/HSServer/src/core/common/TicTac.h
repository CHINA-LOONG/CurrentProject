#pragma once
#include "const_def.h"
#include "common/Clock.h"

class TicTac {
public:
	TicTac(){
		tic();
	}
	void tic() {
		tic_ = Clock::getCurrentSystemTime();
	}
	time_t tac() {
		time_t tac = Clock::getCurrentSystemTime();
		time_t elapsed = tac - tic_;
		tic_ = tac;
		return elapsed;
	}
	bool crctac(time_t time_tac) {
		time_t tac = Clock::getCurrentSystemTime();
		time_t elapsed = tac - tic_;
		if (time_tac < elapsed)
		{
			tic_ = tac;
			return true;
		}
		else
		{
			return false;
		}
	}
	time_t crctacex(time_t time_tac) {
		time_t tac = Clock::getCurrentSystemTime();
		time_t elapsed = tac - tic_;
		if (time_tac < elapsed)
		{
			tic_ = tac;
		}
		return elapsed;
	}
private:
	// the unit is milliseconds
	time_t tic_;
};
