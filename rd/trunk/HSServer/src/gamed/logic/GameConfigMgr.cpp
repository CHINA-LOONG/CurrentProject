#include "GameConfigMgr.h"
#include "logic/GameConfig.h"
#include "UserCtrl.h"
#include "common/DBC.h"


GameConfigMgr* GameConfigMgr::_sMgr = NULL;

GameConfigMgr::GameConfigMgr(void)
{
    GameConfig::CreateInstance();

}

GameConfigMgr::~GameConfigMgr(void)
{
    GameConfig::ReleaseInstance();
  
}


void
GameConfigMgr::LoadRuntimeConfig()
{
    GameConfig::GetInstance()->LoadInfo();
    GameConfig::GetInstance()->LoadWebPath();
    LoadHotCacheDBConfig();
  
}

void
GameConfigMgr::LoadGameConfigInfo()
{
  
}

void
GameConfigMgr::LoadHotCacheDBConfig()
{
    DBCFile file(0);
    file.OpenFromTXT("HotCacheDb.dat");

    enum FileStruct
    {
        FS_ID, FS_IP, FS_PORT,
    } ;
    int cnt = file.GetRecordsNum();
    if (cnt <= 0)
    {
        cnt = 0;
        return;
    }
    for (int line = 0; line < cnt; line++)
    {
        int id = file.Search_Posistion(line, FS_ID)->iValue;
        std::string ip = file.Search_Posistion(line, FS_IP)->pString;
        int port = file.Search_Posistion(line, FS_PORT)->iValue;
        GameConfig::GetInstance()->m_xHotCacheIpList[id] = ip;
        GameConfig::GetInstance()->m_xHotCachePortList[id] = port;
    }
}

