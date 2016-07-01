/* 
 * File:   DealTimer.cpp
 * Author: Kidd
 * 
 * Created on 2012年11月22日, 上午10:35
 */

#include "DealTimer.h"
#include "../../common/const_def.h"
#include "../../logic/Clock.h"
#include "../../common/string-util.h"
#include "../../common/SysLog.h"
#include "../../common/Ini.h"
#include "../../logic/Rank.h"
#include "../../event/EventQueue.h"
#include "../gamed/MemCacheServerHandler.h"
#include "../../logic/Rank.h"
#include "../../logic/RankInfoMgr.h"
#include "../../logic/UniverseCtrl.h"
#include "../../logic/KingOfStar.h"
#include "../../logic/ExpLevelCtrl.h"

static int64 g_llTimerRutimeCount;
static int64 llLastUpdateSecond = llInvalidTime;
#define SavingTimeSpanFileName "UpdateSecondSaving.txt" 
#define SavingRankTimeFileName "interval.ini"

static int64 g_llLastUpdateDayCahce  = llInvalidTime;
static int64 g_llLastUpdateWeekCache = llInvalidTime;
DealTimer* DealTimer::instance_ = NULL;

void
LoadUpdateTimeSpan()
{
    FILE* pFile = NULL;
    char buf[256];
    pFile = fopen(SavingTimeSpanFileName, "r");
    if (pFile != NULL)
    {
        fscanf(pFile, "%s", buf);
        safe_atoll(buf, llLastUpdateSecond);
        fclose(pFile);
    }
}

void
SaveUpdateTimeSpan()
{
    FILE* pFile = NULL;
    pFile = fopen(SavingTimeSpanFileName, "w");
    if (pFile != NULL)
    {
        std::string t = toString<int64 > (llLastUpdateSecond);
        fprintf(pFile, "%s", t.c_str());
        fclose(pFile);
    }
}

/**
 * 简易时间判断函数
 * @param llVal 待判断的秒. 通常是需要执行hendler的时间
 * @param from 起始时间 . 通常是 LastUpdate
 * @param to 结束时间. 通常是Now
 * @return 是否在范围内
 */
bool
IsInSecondRange(int64 llVal, int64 from, int64 to)
{
    bool bTest = true;
    bTest = bTest  && llVal >from;
    bTest = bTest && llVal <= to;
    return bTest;
}

void
DealTimer::handle(Event* e)
{
    if (llLastUpdateSecond == llInvalidTime)
    {
        LoadUpdateTimeSpan();
        if (llLastUpdateSecond == llInvalidTime)
        {
            llLastUpdateSecond = Clock::GetSecond() - 300;
        }
    }

    //if any of timer handlers is touched . 
    //bChanged is set to ture
    //to make LastUpdateTime Save to file
    bool bChanged = false;
    int64 llSecond = Clock::GetSecond();
    // Dally Timer 
    //int64 llFactoryCheckTime = OccupiableRoom::GetDallyRewardSecond() + 30;
    //if (IsInSecondRange(llFactoryCheckTime, llLastUpdateSecond, llSecond))
    //{
    //    bChanged = true;
    //    OccupiableRoomMgr::GlobalTimerHandler(eh_);
    //    SYS_LOG(-1, LT_TimerEvent, 0, 0, "FactoryUpdate");
    //}

    if (g_llLastUpdateDayCahce != Clock::GetDay())
    {
        g_llLastUpdateDayCahce = Clock::GetDay();

        int day = Clock::GetDay() - 1;
        int week = Clock::GetWeek(day + 1) - 1;

        Event fake_event;
        fake_event.set_state(Status_Normal_To_World);
        fake_event.set_cmd(S2C_MseRank);
        fake_event.set_time(time(NULL));
        //fake_event.set_state()
        fake_event.set_uid(llInvalidId);
        fake_event.set_gameid(eh_->GetSrvID());
        fake_event.mutable_mse_mserank()->set_opernum(RankInfoMgr::act_GetRankAll);
        for (int i = 0; i < 6; i++)
        {
            fake_event.mutable_mse_mserank()->set_rankname(RankInfoMgr::GetInst()->GetRankWeek(i, week)->GetName());
            eh_->sendEventToWorld(&fake_event, GameConfig::GetInstance()->GetRankWorldId() - 1);
            fake_event.mutable_mse_mserank()->set_rankname(RankInfoMgr::GetInst()->GetRankDay(i, day)->GetName());
            eh_->sendEventToWorld(&fake_event, GameConfig::GetInstance()->GetRankWorldId() - 1);
        }

        fake_event.mutable_mse_mserank()->set_rankname(RankInfoMgr::GetInst()->GetRankWeek(UniverseCtrl::g_nDallyRankType, week + 1)->GetName());
        eh_->sendEventToWorld(&fake_event, GameConfig::GetInstance()->GetRankWorldId() - 1);
        fake_event.mutable_mse_mserank()->set_rankname(RankInfoMgr::GetInst()->GetRankWeek(UniverseCtrl::g_nDallyRankType, week)->GetName());
        eh_->sendEventToWorld(&fake_event, GameConfig::GetInstance()->GetRankWorldId() - 1);


        fake_event.mutable_mse_mserank()->set_rankname(KingOfStar::GetInst()->GetRankName() );
        eh_->sendEventToWorld(&fake_event, GameConfig::GetInstance()->GetRankWorldId() - 1);
        fake_event.mutable_mse_mserank()->set_rankname(KingOfStar::GetInst()->GetLastRankName() );
        eh_->sendEventToWorld(&fake_event, GameConfig::GetInstance()->GetRankWorldId() - 1);
        KingOfStar::GetInst()->GetRank()->NeedFile(false);

        fake_event.mutable_mse_mserank()->set_rankname(ExpLevelCtrl::GetRankName() );
        eh_->sendEventToWorld(&fake_event, GameConfig::GetInstance()->GetRankWorldId() - 1);
    }

    llLastUpdateSecond = llSecond;
    if (bChanged)
    {
        SaveUpdateTimeSpan();
    }

    UpdataRankInterval();
}

DealTimer::DealTimer()
{
    logger_ = log4cxx::Logger::getLogger("EventHelper");
    //LoadRankIntervalTime();
}

void
DealTimer::LoadRankIntervalTime()
{
    //	Ini myini;
    //	if (myini.Open(SavingRankTimeFileName) == false)
    //	{
    //		return;
    //	}
    //
    //	std::map<int,bool> &rank_list = RankConfig::GetInstance()->getRankList();
    //	for (std::map<int,bool>::iterator iter = rank_list.begin(); iter != rank_list.end(); iter++)
    //	{
    //		if (RankConfig::GetInstance()->IsIntervalRank(iter->first))
    //		{
    //			char buff[32] = {0};
    //			char last_time[128] = {0};
    //			sprintf(buff,"rank_%d",iter->first);
    //			myini.ReadTextIfExist("Rank",buff,last_time,128);
    //			int64 last(0);
    //			safe_atoll(last_time,last);
    //			interval_time_list_[iter->first] = last;
    //		}
    //	}
    //	myini.Close();
}

void
DealTimer::SaveRankIntervalTime()
{
    //	Ini myini;
    //	myini.Open(SavingRankTimeFileName);
    //	for (std::map<int,int64>::iterator iter = interval_time_list_.begin(); iter != interval_time_list_.end(); iter++)
    //	{
    //		char buff[32] = {0};
    //		sprintf(buff,"rank_%d",iter->first);
    //		char last_time[128] = {0};
    //		sprintf(last_time,"%lld",iter->second);
    //		myini.Write("Rank",buff,last_time);
    //	}
    //	myini.Save();
    //	myini.Close();
}

void
DealTimer::UpdataRankInterval()
{
    //	//printf("in_update_rank");
    //	return;
    //	time_t time_now = time(NULL);
    //	std::map<int,bool> &rank_list = RankConfig::GetInstance()->getRankList();
    //	for (std::map<int,bool>::iterator iter = rank_list.begin(); iter != rank_list.end(); iter++)
    //	{
    //		if (RankConfig::GetInstance()->IsIntervalRank(iter->first))
    //		{
    //			Rank* pRank = eh_->getRank(iter->first);
    //			if (pRank == NULL)
    //			{
    //				continue;
    //			}
    //
    //			int interval = RankConfig::GetInstance()->Interval(iter->first);			
    //			if (pRank->LastTime() <= 0)
    //			{
    //				printf("last_time_0");
    // 				pRank->LastTime(time_now);
    //// 				string save_str;
    //// 				pRank->TransToJson(save_str);
    //// 				MemCacheServerHandler::GetInst()->UpdateRankInfo(iter->first,save_str);
    //			}
    //			else
    //			{
    //				int64 last_time = pRank->LastTime();
    //				if (time_now-last_time > interval)
    //				{
    //					std::list<RankItem*> *plist = pRank->GetIntervalList();
    //					std::map<int64,int> rank_list;
    //					for (std::list<RankItem*>::iterator iter_list=plist->begin(); iter_list != plist->end(); iter_list++)
    //					{
    //						RankItem *pRankItem = *iter_list;
    //						if (pRankItem == NULL)
    //						{
    //							continue;
    //						}
    //						if (eh_->getDataHandler()->getGamedIdByUserId(pRankItem->Uid()) != eh_->GetSrvID())
    //						{
    //							continue;
    //						}
    //						rank_list[pRankItem->Uid()] = pRank->GetCopyIntervalIndex(pRankItem->Uid(),pRankItem->Point());
    //					}
    //					pRank->CopyIntervalRank();
    //					
    //					for(std::map<int64,int>::iterator rank_iter = rank_list.begin(); rank_iter != rank_list.end(); rank_iter++)
    //					{
    //						//reward
    //						char buff[512] = {0};
    //						sprintf(buff,"rankreward,0,%lld,%d,%d,-1",rank_iter->first,iter->first,pRank->GetCopyIntervalIndexEx(rank_iter->first));
    //						string add = buff;
    //						Event* e = eh_->getEventQueue()->allocateEvent();						
    //						e->set_cmd(EVENT_ADMIN_STR);
    //						e->set_state(Admin_WG_Req);
    //						e->set_time(-1);
    //						e->set_uid(-1);
    //						e->set_sid("-1");
    //						Admin_STR_Req* req = e->mutable_adminstr_req();
    //						req->set_adminfd(-1);
    //						req->set_str(add);
    //						req->set_uid(rank_iter->first);
    //						eh_->getEventQueue()->safePushEvent(e);
    //					}
    //
    //					std::list<RankItem*> *pNextlist = pRank->GetIntervalList();
    //					for (std::list<RankItem*>::iterator next_iter = pNextlist->begin(); next_iter != pNextlist->end(); next_iter++)
    //					{
    //						RankItem *pRankItem = *next_iter;
    //						if (pRankItem == NULL)
    //						{
    //							continue;
    //						}
    //						if (eh_->getDataHandler()->getGamedIdByUserId(pRankItem->Uid()) != eh_->GetSrvID())
    //						{
    //							continue;
    //						}
    //						//update rank
    //						char buff[512] = {0};
    //						sprintf(buff,"rankinterval,0,%lld,%d,%d,-1",pRankItem->Uid(),iter->first,pRank->GetCopyIntervalIndex(pRankItem->Uid(),pRankItem->Point()));
    //						string add = buff;
    //						Event* e = eh_->getEventQueue()->allocateEvent();						
    //						e->set_cmd(EVENT_ADMIN_STR);
    //						e->set_state(Admin_WG_Req);
    //						e->set_time(-1);
    //						e->set_uid(-1);
    //						e->set_sid("-1");
    //						Admin_STR_Req* req = e->mutable_adminstr_req();
    //						req->set_adminfd(-1);
    //						req->set_str(add);
    //						req->set_uid(pRankItem->Uid());
    //						eh_->getEventQueue()->safePushEvent(e);
    //					}
    //
    //					pRank->LastTime(time_now);
    //					string save_str;
    //					pRank->TransToJson(save_str);
    //					MemCacheServerHandler::GetInst()->UpdateRankInfo(iter->first,save_str);
    //				}
    //						
    //			}
    //		}
    //	}

}