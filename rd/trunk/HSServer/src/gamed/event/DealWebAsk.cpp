///* 
// * File:   DealWebAsk.cpp
// * Author: Kidd
// * 
// * Created on 2011年9月19日, 下午6:10
// */
//#include "DealWebAsk.h"
//#include "../GameNetHandler.h"
//#include "../GameDataHandler.h"
//#include "../../event/event.pb.h"
//#include "../../common/json/json.h"
//#include "../../common/string-util.h"
//#include "../../logic/User.h"
//#include "../../logic/Player.h"
//#include "../../common/SysLog.h"
//#include "../../event/EventQueue.h"
//#include "../../logic/Player.h"
//#include "../../logic/UserCtrl.h"
//
//DealWebAsk* DealWebAsk::instance_ = NULL;
//
//void DealWebAsk::handle(Event* e)
//{
//    if (e->state() == Status_Normal_Game)
//    {
//        handle_selfload(e);
//    }
//    else if (e->state() == Status_Normal_Logic_Game )
//    {
//        handle_romateload(e);
//    }
//    else
//    {
//        LOG4CXX_ERROR(logger_, "Invalid Event.\n" << e->DebugString());
//    }
//
//}
//
//void DealWebAsk::handle_selfload(Event* e)
//{
//    WebAskFor* request = e->mutable_webask();
//    GameDataHandler* pUserManager = eh_->getDataHandler();
//    LoadStatus state = LOAD_INVALID;
//    User* pUser = pUserManager->getUser(request->uid_from() , &state, true);
//
//    if (pUser == NULL)
//    {
//        if (state == LOAD_WAITING)
//        {
//            eh_->postBackEvent(e);
//			return;
//        }
//    }
//	int succ = 0;
//	do 
//	{
//		if (pUser == NULL)
//		{
//			succ = 1;
//			break;
//		}
//		
//		Player * pPlayer = pUser->GetPlayer();
//		if (pPlayer == NULL)
//		{
//			succ = 1;
//			break;
//		}
//		
//	} while (0);
//	
//	if (succ == 0)
//	{
//		pUser->FillForwardInfo(e->mutable_forwardinfo());
//		request->set_succ(0);
//		for (int i=0;i<request->openid_to_size();i++)
//		{
//			e->set_state(Status_Normal_To_World);
//			request->set_openid_forward(request->openid_to(i));
//			eh_->sendEventToWorld(e);
//		}
//		
//	}
//	request->set_succ(succ);
//	e->set_state(Status_Normal_Back_World);
//	eh_->sendEventToWorld(e);
//	
//}
//
//void DealWebAsk::handle_romateload(Event* e)
//{
//	WebAskFor* request = e->mutable_webask();
//	GameDataHandler* pUserManager = eh_->getDataHandler();
//	LoadStatus state = LOAD_INVALID;
//	User* pUser = pUserManager->getUser(request->openid_forward() , &state, true);
//
//	if (pUser == NULL)
//	{
//		if (state == LOAD_WAITING)
//		{
//			eh_->postBackEvent(e);
//			return;
//		}
//	}
//	int succ = 0;
//	do 
//	{
//		if (pUser == NULL)
//		{
//			succ = 1;
//			break;
//		}
//
//		Player * pPlayer = pUser->GetPlayer();
//		if (pPlayer == NULL)
//		{
//			succ = 1;
//			break;
//		}
//
//		ForwardInfo * pForward = e->mutable_forwardinfo();
//		if (pForward == NULL)
//		{
//			succ = 1;
//			break;
//		}
//		
//		if (request->item_type_id_size() < 1 || request->item_num_size() < 1)
//		{
//			succ = 1;
//			break;
//		}
//
//		for (int i=0;i<request->item_type_id_size();i++)
//		{
//			if (request->item_type_id(i) == ChallengeSystem::nBallItemTypeId)
//			{
//				pPlayer->GetChallengeSystem().AddActionRecord(request->uid_from(),pForward->sendername(),pForward->senderurl(),ActionRecord::AskHelpChallenge);
//				if (pUser->Online())
//				{
//					UserCtrl uc(pUser);
//					uc.SendChallengeInfo();
//				}
//				SYS_LOG(pUser->id(),LT_AskChallengeBall,0,0,request->uid_from());
//			}
//			else
//			{
//				DB_ActionRecord * pRecord = NULL;
//				if (request->fd() == -1)
//				{
//					pRecord = pPlayer->AddChristmasActivetyBackHistory(request->uid_from(),pForward->platid(),pForward->sendername(),pForward->senderurl(),request->item_type_id(0),request->item_num(0));
//				}
//				else
//				{
//					pRecord = pPlayer->AddChristmasActivetyAskHistory(request->uid_from(),pForward->platid(),pForward->sendername(),pForward->senderurl(),request->item_type_id(0),request->item_num(0));
//					//加入索要列表
//				}
//
//				pUserManager->markUserDirty(pUser);
//				if (pRecord)
//				{
//					UserCtrl uc(pUser);
//					uc.SendSingleFriendAction(pRecord);
//				}
//			}
//		}
//	} while (0);
//}
//
//void DealWebAsk::handle_romatereturn(Event* e)
//{
//}