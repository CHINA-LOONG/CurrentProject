#pragma once
#include "stdio.h"

class GameConfigMgr
{
public:

	static GameConfigMgr* CreateInst()
	{
		if ( !_sMgr )
		{
			_sMgr = new GameConfigMgr();
		}
		return _sMgr;
	}

	static void ReleaseInst()
	{
		delete _sMgr;
		_sMgr = NULL;
	}

	static GameConfigMgr* GetInst()
	{
		return _sMgr;
	}

	void LoadRuntimeConfig();
	void LoadGameConfigInfo();
    void LoadHotCacheDBConfig();
    
private:
	GameConfigMgr(void);
	~GameConfigMgr(void);

private:
	static GameConfigMgr*   _sMgr;
} ;
