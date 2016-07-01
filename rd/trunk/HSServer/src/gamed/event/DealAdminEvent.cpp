#include "DealAdminEvent.h"
#include "../../event/event.pb.h"
#include "../../event/EventQueue.h"
#include "../GameEventHandler.h"
#include "../GameDataHandler.h"
#include "../../logic/Player.h"
#include "../../logic/UserCtrl.h"
#include "../../common/statistics.h"
#include "../../common/Ini.h"
#include "../../logic/ItemManager.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/FuncSwitch.h"
#include "../../gamed/MemCacheServerHandler.h"
#include "../../common/const_def.h"
#include "../../gamed/FriendInfoServerHandler.h"
#include "../../logic/GameConfigMgr.h"
#include "../../logic/phpdecoder.h"
#include "../../logic/CMEMReader.h"
#include "DealMceGatewayFunction.h"
#include "../../common/string-util.h"
#include "../../logic/MessageCtrl.h"
#include "../../logic/BitCache.h"
#include "../../logic/ConfigUnit/UniverseCfgUnit.h"
#include "../../logic/UniverseMgr.h"
#include "../../logic/UniverseCtrl.h"
#include "../../logic/PlayerService.h"
#include "../../logic/TimeSetCtrl.h"
#include "../../logic/dictionary.h"
#include "../../logic/CLDaysCtrl.h"
#include "../../logic/ActiveCallManger.h"
#include "../../logic/RuntimeVariableMgr.h"
#include "../../logic/KingOfStar.h"
#include "../../gamed/event/DealCenterLogicCounter.h"
#include "../../logic/ConfigUnit/CLCounterMgr.h"

bool
Deconstruct(int64 llItemId, int64& llGameId, int64& llAllocId)
{
    int64 mask = (int64) (0xffff) << 48;
    llAllocId  = llItemId & ~mask;
    llGameId   = (llItemId & mask) >> 48;
    return true;
}

DealAdminEvent::DealAdminEvent()
: CBaseEvent()
{
}

DealAdminEvent::~DealAdminEvent()
{
}

void
DealAdminEvent::handle(Event* e)
{

    switch (e->cmd())
    {
        case EVENT_ADMIN_CLEAR:
            processAdminClear(e);
            break;
        case EVENT_ADMIN_RELOAD:
            processAdminReload(e);
            break;
            //            break;
            //        case EVENT_ADMIN_KICK:
            //            processAdminKickPlayer(e);
            //            break;
        case EVENT_ADMIN_ONLINE:
            processAdminOnLineNum(e);
            break;
        case EVENT_ADMIN_STR:
            processAdminStr(e);
            break;
        default:
            break;
    }
}
/******************************************************************************/
/*    Reload logic                                                            */
/******************************************************************************/
//<editor-fold desc="Reload logic">

void
DealAdminEvent::processAdminReload( Event* e )
{
    if (e->has_sid())
    {
        vector<string> tokens;
        static string delims = "#";
        tokenize(e->sid(), tokens, delims);
        if (tokens.size() >= 3)
        {
            RuntimeVariableMgr::Set(tokens[1], tokens[2]);
            return;
        }

    }

    eh_->getDataHandler()->acquireDataLock();
    GameConfigMgr::ReleaseInst();
    GameConfigMgr::CreateInst();
    GameConfigMgr::GetInst()->LoadGameConfigInfo();
    eh_->getCDKeyCounter().ConfigClear();
    eh_->getCDKeyCounter().LoadConifg();
    eh_->setProcessIgnore();
    CMsg2QQ::GetInstance()->SetSafeDataToQQMax(GameConfig::GetInstance()->GetSafeDataToQQMax());
    if (GameConfig::GetInstance()->GetDataToQQEnableFlag() == 0)
    {
        CMsg2QQ::GetInstance()->SetEnable(false);
        CMsg2QQDB::GetInstance()->SetEnable(false);
    }
    else
    {
        CMsg2QQ::GetInstance()->SetEnable(true);
        CMsg2QQ::GetInstance()->SetEnable(true);
    }
    //eh_->InitRank();
    eh_->getDataHandler()->releaseDataLock();
    Admin_Reload_Req* req = e->mutable_adminreload_req();
    Admin_Reload_Rsp* rsp = e->mutable_adminreload_rsp();
    rsp->set_gid(req->gid());
    rsp->set_succ(true);
    rsp->set_fromweb(req->fromweb());
    rsp->set_adminfd(req->adminfd());
    e->clear_adminfluentprice_req();
    e->set_state(Admin_GW_Rsp);
    eh_->sendEventToWorld(e);
}
//</editor-fold>

/******************************************************************************/
/*    clear logic                                                             */
/******************************************************************************/
//<editor-fold desc="clear logic">

void
DealAdminEvent::processAdminClear( Event* e )
{
    if (e->state() == Admin_WG_Req)
    {
        if (!e->has_adminclear_req())
        {
            return;
        }
        Admin_Clear_Req* req = e->mutable_adminclear_req();
        //刷新玩家信息
        int64 nUserID = req->uid();
        GameDataHandler* dh = eh_->getDataHandler();
        int gid = dh->getGamedIdByUserId(nUserID);
        if (gid != eh_->GetSrvID())
        {
            return;
        }

        LoadStatus state;
        User* pUser = dh->getUser(nUserID, &state, true);
        if (pUser == NULL)
        {
            if (state == LOAD_WAITING)
            {
                eh_->postBackEvent(e);
            }
            return;
        }
        eh_->removeUserFdMap(pUser->fd(), pUser->id());
        pUser->setFd(0);
        pUser->setOnline(false);
        pUser->GetDbUser().clear_player();
        pUser->ClearPlayer();
        pUser->InitNewUser();
        pUser->GetDbUser().set_user_cmem_v(999);
        pUser->GetPlayer()->SetEventHandler(eh_);
        GameDataHandler* pUserManager = eh_->getDataHandler();
        pUserManager->markUserDirty(pUser);
        Admin_Clear_Rsp* rsp = e->mutable_adminclear_rsp();
        rsp->set_gid(req->gid());
        rsp->set_uid(req->uid());
        rsp->set_succ(true);
        rsp->set_fromweb(req->fromweb());
        rsp->set_adminfd(req->adminfd());
        e->clear_adminclear_req();
        e->set_state(Admin_GW_Rsp);
        eh_->sendEventToWorld(e);
    }
    else
    {
        LOG4CXX_DEBUG(logger_, "Invalid admin command status:" << e->state());
    }
}
//</editor-fold>

/******************************************************************************/
/*    online logic                                                            */
/******************************************************************************/
//<editor-fold desc="online logic">

void
DealAdminEvent::processAdminOnLineNum( Event* e )
{
    if (e->state() == Admin_WG_Req)
    {
        if (!e->has_adminonlinenum_req())
        {
            return;
        }
        Admin_StatisticsOnLinePeople_Req* req = e->mutable_adminonlinenum_req();
        int online(0);
        long long cached_size = 0 ;
        std::string debug_str = "";
        std::string simple_str = "";
        GameDataHandler* pUserManager = eh_->getDataHandler();
        const hash_map<int64, User*>& usermap = pUserManager->getUsers();
        for (hash_map<int64, User*>::const_iterator iter = usermap.begin(); iter != usermap.end(); iter++)
        {
            User* pUser = iter->second;
            if (pUser == NULL || pUser->Online() == false)
            {
                continue;
            }
            ++online;
            cached_size += pUser->GetDbUser().GetCachedSize();
        }
        debug_str += "ID:(" + toString<int>(eh_->GetSrvID()) + ")";
        debug_str += "Version:(" + GameConfig::GetInstance()->GetGameVersion() + ")";
        debug_str += "User_cache_szie(" + toString<int64 > (cached_size) + ") ";
        debug_str += "User_count (" + toString<int64 > (usermap.size()) + ") ";
        debug_str += "FriendInfoCnt(" + toString<int64 > (pUserManager->FriendInfoLiteCacheCount()) + ")\n";
        debug_str += "Online(" + toString<int>(online) + ")\n";
        debug_str += "FDCacheSize(" + toString<unsigned int>(eh_->getCacheFdSize()) + ")\n";
        debug_str += "RemoveUserCnt(" + toString<int64 > (eh_->getDataHandler()->getRemoveCnt()) + ")\n";
        bool memstate(false);
        if (MemCacheServerHandler::GetInst())
        {
            memstate = MemCacheServerHandler::GetInst()->CanUse();
        }
        debug_str += "MemCacheState(" + toString<bool>(memstate) + ")\n";
        Counter &counter = eh_->getDataHandler()->getCounter();
        map<string, int> &basemap = counter.getCounterMap();
        for (map<string, int>::iterator iter = basemap.begin(); iter != basemap.end(); iter++)
        {
            debug_str += iter->first;
            debug_str += "(";
            debug_str += toString<int>(iter->second);
            debug_str += ")\n";
        }
        map<string, int> &basemap_instance = Counter::GetInst()->getCounterMap();
        for (map<string, int>::iterator iter = basemap_instance.begin(); iter != basemap_instance.end(); iter++)
        {
            debug_str += iter->first;
            debug_str += "(";
            debug_str += toString<int>(iter->second);
            debug_str += ")\n";
        }
        simple_str = debug_str;
        for (int i = 0; i < E_MAX; i++)
        {
            debug_str += toString<int>(i);
            debug_str += "(";
            debug_str += toString<int>(TEST[i]);
            debug_str += ")\n";
        }
        //         debug_str += "LogonTimes(" + toString<int>(counter.count("user_log_in")) + ")\n";
        //         debug_str += "LogonCreate(" + toString<int>(counter.count("user_create")) + ")\n";
        //         debug_str += "LogoutTimes(" + toString<int>(counter.count("user_log_out")) + ")\n";
        //         debug_str += "LogoutUserNull(" + toString<int>(counter.count("user_equal_null")) + ")\n";
        //         debug_str += "LogoutFDError(" + toString(counter.count("fd_error")) + ")\n";
        if (eh_ && eh_->getEventHandler() && eh_->getEventHandler()->getEventProfile())
        {
            int server_id = 0;
            safe_atoi(e->sid(), server_id);
            if (server_id != eh_->GetSrvID())
            {

            }
            else
            {
                debug_str += eh_->getEventHandler()->getEventProfile()->report();
                Statistics *stat_ = eh_->getEventHandler()->getStatistics();
                vector<string> names = stat_->names();
                Value json(objectValue);
                Value stat (arrayValue);
                for (size_t i = 0; i < names.size(); i++)
                {
                    vector<double> minitue = stat_->recentOneHour(names[i]);
                    vector<double> hour = stat_->recentOneDay(names[i]);
                    vector<double> digest = stat_->recentDigest(names[i]);
                    Value s (objectValue);
                    s["name"] = names[i];
                    Value jm(arrayValue);
                    Value jh(arrayValue);
                    Value jd(arrayValue);
                    for (size_t i = 0; i < minitue.size(); i++) jm.append(minitue[i]);
                    for (size_t i = 0; i < hour.size(); i++) jh.append(hour[i]);
                    for (size_t i = 0; i < digest.size(); i++) jd.append(digest[i]);
                    s["minute"] = jm;
                    s["hour"] = jh;
                    s["digest"] = jd;
                    stat.append(s);
                }
                json["stat"] = stat;

                stat_ = &(eh_->getDataHandler()->getStatistics());
                vector<string> names_free = stat_->names();
                Value stat_free (arrayValue);
                for (size_t i = 0; i < names_free.size(); i++)
                {
                    vector<double> minitue = stat_->recentOneHour(names_free[i]);
                    vector<double> hour = stat_->recentOneDay(names_free[i]);
                    vector<double> digest = stat_->recentDigest(names_free[i]);
                    Value s (objectValue);
                    s["name"] = names_free[i];
                    Value jm(arrayValue);
                    Value jh(arrayValue);
                    Value jd(arrayValue);
                    for (size_t i = 0; i < minitue.size(); i++) jm.append(minitue[i]);
                    for (size_t i = 0; i < hour.size(); i++) jh.append(hour[i]);
                    for (size_t i = 0; i < digest.size(); i++) jd.append(digest[i]);
                    s["minute"] = jm;
                    s["hour"] = jh;
                    s["digest"] = jd;
                    stat_free.append(s);
                }
                json["stat_free"] = stat_free;


                stat_ = &(FriendInfoServerHandler::GetInstAlarm()->getStat());
                vector<string> names_alarm = stat_->names();
                Value stat_alarm (arrayValue);
                for (size_t i = 0; i < names_alarm.size(); i++)
                {
                    vector<double> minitue = stat_->recentOneHour(names_alarm[i]);
                    vector<double> hour = stat_->recentOneDay(names_alarm[i]);
                    vector<double> digest = stat_->recentDigest(names_alarm[i]);
                    Value s (objectValue);
                    s["name"] = names_alarm[i];
                    Value jm(arrayValue);
                    Value jh(arrayValue);
                    Value jd(arrayValue);
                    for (size_t i = 0; i < minitue.size(); i++) jm.append(minitue[i]);
                    for (size_t i = 0; i < hour.size(); i++) jh.append(hour[i]);
                    for (size_t i = 0; i < digest.size(); i++) jd.append(digest[i]);
                    s["minute"] = jm;
                    s["hour"] = jh;
                    s["digest"] = jd;
                    stat_alarm.append(s);
                }
                json["stat_alarm"] = stat_alarm;

                debug_str += getJsonStr(json);
                debug_str += "\n";

                Ini myini;
                time_t time_now = time(NULL);
                tm * atimeinfo	= localtime(&time_now);
                int year  = atimeinfo->tm_year + 1900;
                int month = atimeinfo->tm_mon + 1;
                int day   = atimeinfo->tm_mday;
                int hour  = atimeinfo->tm_hour;
                int minute = atimeinfo->tm_min;
                char temp[128] = {0};
                sprintf(temp, "Config/test%d_%d_%d_%d_%d.ini", year, month, day, hour, minute);

                if (myini.Open(temp) == false)
                {
                    //return false;
                }

                myini.Write("test", "1", const_cast<char*> (debug_str.c_str()));
                myini.Save();
                myini.Close();

                //LOG4CXX_ERROR(logger_, debug_str);
            }
        }
        //debug_str += "FriendInfoSize(" + toString<int64 > (pUserManager->FriendInfoLiteCacheSize());
        Admin_StatisticsOnLinePeople_Rsp* rsp = e->mutable_adminonlinenum_rsp();
        rsp->set_succ(true);
        rsp->set_fromweb(req->fromweb());
        rsp->set_adminfd(req->adminfd());
        rsp->set_online_num(online);
        e->mutable_adminstr_rsp()->set_adminfd(req->fromweb());
        e->mutable_adminstr_rsp()->set_succ(true);
        e->mutable_adminstr_rsp()->set_str(simple_str);
        e->clear_adminonlinenum_req();
        e->set_state(Admin_GW_Rsp);
        eh_->sendEventToWorld(e);
    }
    else
    {
        LOG4CXX_DEBUG(logger_, "Invalid admin command status:" << e->state());
    }
}
//</editor-fold>

/******************************************************************************/
/*    kick logic                                                              */
/******************************************************************************/
//<editor-fold desc="kick logic">

void
DealAdminEvent::processAdminKickPlayer( Event* e )
{
    if (e->state() == Admin_WG_Req)
    {
        if (!e->has_adminkickplayer_req())
        {
            return;
        }
        Admin_KickPlayer_Req* req = e->mutable_adminkickplayer_req();
        //刷新玩家信息
        int64 nUserID = req->uid();
        GameDataHandler* dh = eh_->getDataHandler();
        int gid = dh->getGamedIdByUserId(nUserID);
        if (gid != eh_->GetSrvID())
        {
            return;
        }

        LoadStatus state;
        User* pUser = dh->getUser(nUserID, &state, true);
        if (pUser == NULL)
        {
            if (state == LOAD_WAITING)
            {
                eh_->postBackEvent(e);
            }
            return;
        }
        eh_->removeUserFdMap(pUser->fd(), pUser->id());
        pUser->setOnline(false);
        pUser->setFd(0);
        Admin_KickPlayer_Rsp* rsp = e->mutable_adminkickplayer_rsp();
        rsp->set_uid(req->uid());
        rsp->set_succ(true);
        rsp->set_fromweb(req->fromweb());
        rsp->set_adminfd(req->adminfd());
        e->clear_adminkickplayer_req();
        e->set_state(Admin_GW_Rsp);
        eh_->sendEventToWorld(e);
    }
    else
    {
        LOG4CXX_DEBUG(logger_, "Invalid admin command status:" << e->state());
    }
}
//</editor-fold>

/******************************************************************************/
/*    String Cmd                                                              */
/******************************************************************************/
//<editor-fold desc="String Cmd">

/**
 * 字符串GM指令
 * @param e
 */
void
DealAdminEvent::processAdminStr(Event* e)
{
    if (e->state() == Admin_WG_Req)
    {
        if (!e->has_adminstr_req())
        {
            return;
        }
        Admin_STR_Req* req = e->mutable_adminstr_req();
        vector<string> tokens;
        static string delims = ",";
        tokenize(req->str(), tokens, delims);
        std::string str_response = "Command Error , try help";
        //刷新玩家信息
        GameDataHandler* dh = eh_->getDataHandler();
        User * pUser = NULL;
        int gid = -1;
        int sid(0);
        safe_atoi(e->sid(), sid);
        LoadStatus state;
        if (req->has_openid())
        {
            std::string openid = req->openid();
            gid = dh->getGameIdfromPlatId(openid);
            pUser = dh->getUser(openid, &state, true);
        }
        else if (req->has_uid())
        {
            int64 nUserID = req->uid();
            GameDataHandler* dh = eh_->getDataHandler();
            gid = dh->getGamedIdByUserId(nUserID);
            pUser = dh->getUser(nUserID, &state, true);
        }

        if (sid != -1)
        {
            if (gid != eh_->GetSrvID())
            {
                return;
            }
        }
        Value objResponseJson(objectValue);
        bool bDealResult = false;
        if (pUser == NULL && tokens.size() >= 2)
        {
            //<editor-fold desc="get user"> 
            if (state == LOAD_WAITING)
            {
                eh_->postBackEvent(e);
                return;
            }
            if (state == LOAD_EMPTY && tokens[1] == "3" && req->has_openid())
            {
                GameDataHandler* dh = eh_->getDataHandler();
                vector<string> fpid;
                pUser = dh->createUser(req->openid() , "" , "", 0, (PLAT_TYPE) PLAT_QZONE ,
                        false, false, 0, fpid, eh_, "", 0, "QZONE", "PC-admin" );
                if (pUser != NULL)
                {
                    pUser->InitNewUser();
                }
            }
            else if (state == LOAD_MISS || pUser == NULL)
            {
                Value rsp_json1(objectValue);
                rsp_json1["cmd"]  = "User Load Miss!";
                rsp_json1["succ"] = -1;
                rsp_json1["sid"] = e->sid();
                str_response = rsp_json1.toStyledString();
                //str_response = "User Load miss!";
                Admin_STR_Rsp* rsp = e->mutable_adminstr_rsp();

                rsp->set_uid(req->uid());
                rsp->set_succ(false);
                rsp->set_adminfd(req->adminfd());
                rsp->set_str(str_response);
                e->set_state(Admin_GW_Rsp);
                eh_->sendEventToWorld(e);
                return;
            }
            //</editor-fold>

        }
        UserCtrl uc(pUser);
        bool succ  = false;
        do
        {
            if (pUser == NULL || pUser->GetPlayer() == NULL)
            {
                Value rsp_json1(objectValue), rsp_json2(objectValue);
                objResponseJson["cmd"]  =  tokens[0];
                objResponseJson["succ"] = 1;
                objResponseJson["sid"] = e->sid();
                //objResponseJson["result"] = rsp_json1;
                //               str_response = rsp_json1.toStyledString();
                break;
            }



            if (tokens.size() >= 3 && tokens[0] == "userinfoj")
            {
                //<editor-fold desc="userinfoj">
                Value rsp_json1(objectValue), rsp_json2(objectValue);
                pUser->GetPlayer()->FillToJson(rsp_json2);
                rsp_json1["cmd"]  = "str";
                rsp_json1["succ"] = 1;
                rsp_json1["detail"] = rsp_json2;
                rsp_json1["sid"] = e->sid();
                objResponseJson["result"] = rsp_json2;
                succ = true;
                //</editor-fold>
                bDealResult = true;
                break;
            }

            if (tokens.size() >= 5 && tokens[0] == "msg2qq") //msg2qq
            {
                //<editor-fold desc="userinfoj">
                int action_id(-1), succ(0);
                if (safe_atoi(tokens[3], action_id) == true)
                {
                    CMsg2QQ::GetInstance()->TellMsgSimple((MSG2QQType) action_id, pUser);
                }
                else
                {
                    succ = 1;
                }
                Value rsp_json1(objectValue);
                rsp_json1["cmd"]  = "msg2qq";
                rsp_json1["succ"] = succ;
                rsp_json1["sid"] = e->sid();
                objResponseJson["result"] = rsp_json1;
                //</editor-fold>
            }

            if (tokens.size() >= 4 && tokens[0] == "trans") //trans openid or uid
            {
                Value rsp_json1(objectValue);
                rsp_json1["cmd"]  = "trans";
                rsp_json1["succ"] = 0;
                rsp_json1["uid"]  = toString<int64 > (pUser->id());
                rsp_json1["openid"] = pUser->platform_id();
                rsp_json1["sid"] = e->sid();
                objResponseJson["result"] = rsp_json1;
            }

            if (tokens[0] == "UserInfoj")
            {
                Value jinfo(objectValue);
                pUser->GetPlayer()->FillToJson(jinfo) ;
                objResponseJson["result"] = jinfo;
                bDealResult = true;
            }

            if (tokens.size() >= 4 && (tokens[0] == "test" || tokens[0] == "t" || tokens[0] == "T") )
            {
                Admstr_FakeClientEvent(tokens, objResponseJson, pUser, e);
                return ;
            }

            if (tokens[0] == "giftfree")
            {
                pUser->GetDbUser().mutable_player()->mutable_friendgiftlivehistory()->set_checttime(0);
                pUser->GetPlayer()->GetMessageCtrl().Update();
                Value rsp_json1(objectValue);
                rsp_json1["cmd"]  = "giftfree";
                rsp_json1["succ"] = 0;
                rsp_json1["uid"]  = toString<int64 > (pUser->id());
                rsp_json1["sid"] = e->sid();
                objResponseJson["result"] = rsp_json1;
            }

            if (tokens.size() >= 4 && tokens[0] == "addcash")
            {
                int nCash = String2Int(tokens[3]);
                pUser->GetPlayer()->GetBag().AddCash(nCash);
                Value rsp_json1(objectValue);
                rsp_json1["cmd"]  = tokens[0];
                rsp_json1["num"] = toString<int64 > (pUser->GetDbUser().player().bag().cash());
                rsp_json1["sid"] = e->sid();
                objResponseJson["result"] = rsp_json1;
            }
            if (tokens.size() >= 4 && tokens[0] == "phpdecoder")
            {
                // Value rsp_json1(objectValue);
                std::string txt = "";
                for (int i = 3; i < tokens.size(); i++)
                {
                    txt += tokens[i];
                    if (i != tokens.size())
                    {
                        txt += ",";
                    }
                }
                std::string txt_out = "";
                phpdecoder::DecoderForm(txt.c_str(), 0, txt.size(), txt_out);
                const char* pTest = txt_out.c_str();
                int nlen = txt_out.size();
                if (pTest[nlen - 1] == ':')
                {
                    txt_out = "" + txt_out.substr(0, txt_out.size() - 1) + "";
                }
                Value testobj = parseJsonStr(txt_out);
                objResponseJson["result"] = testobj;
            }
            if (tokens.size() >= 5 && tokens[0] == "cmemread")
            {
                std::string mn = tokens[3];
                std::string id = tokens[4];
                std::string out = "";
                std::string out2 = "";
                static CMEMReader sgCReader;
                sgCReader.GetModel(mn, id, out);
                phpdecoder::DecoderForm(out.c_str(), 0, out.length(), out2);
                Value testobj = parseJsonStr(out2);
                objResponseJson["result"] = testobj;
            }
            if (tokens[0] == "reloadcmemUni")
            {
                static CMEMReader sgCReader;
                std::string json_universe = "";
                std::string json_universe_decoder  = "";
                sgCReader.GetModel("universe", pUser->platform_id(), json_universe);
                phpdecoder::DecoderForm(json_universe.c_str(), 0, json_universe.length(), json_universe_decoder);
                Value juni = parseJsonStr(json_universe_decoder);
                int i = pUser->ParseUniverseFromJson(juni["episodes"]);
                Value testobj(objectValue);
                testobj["uni"] = juni;
                testobj["str"] = json_universe_decoder;
                testobj["N"] = i;
                objResponseJson["result"] = testobj;
                bDealResult = true;

            }
            if (tokens[0] == "reloadcmem")
            {
                bool la = eh_->getDataHandler()->TryLoadUserFromCMem(pUser, 3);
                Value testobj(objectValue);
                testobj["n"] = pUser->GetPlayer()->GetUniverseCtrl().GetTotalLvl();
                testobj["loadact"] = la ? "true" : "false";
                objResponseJson["result"] = testobj;
            }
            if (tokens[0] == "db_freegift")
            {
                int n = 5;
                if (tokens.size() >= 4)
                    n = String2Int(tokens[3]);
                pUser->GetDbUser().set_total_login_days(n);
                BitCache bc(pUser->GetDbUser().mutable_player()->mutable_sign_history()->mutable_reward_cache());
                for (int i = 0; i < 8; i++)
                {
                    bc.SetBit(i, false);
                }
                pUser->GetDbUser().mutable_player()->mutable_sign_history()->set_last_reword_time_sec(0);
                pUser->GetDbUser().set_last_active_day(0);
                Value testobj(objectValue);
                testobj["n"] = n;
                objResponseJson["result"] = testobj;
                bDealResult = true;
            }
            if (tokens[0] == "activedays")
            {
                int n = 5;
                if (tokens.size() >= 4)
                    n = String2Int(tokens[3]);
                pUser->GetDbUser().set_active_days(n);
                int64 nToday = Clock::GetDay();
                pUser->GetDbUser().set_last_active_day(nToday - 1);

                time_t timep = Clock::GetSecond();
                struct tm *p;
                time(&timep);
                p = localtime(&timep);
                int m = p->tm_mon + 1;
                int d = p->tm_mday;
                pUser->GetPlayer()->GetActiveCtrl().SetSignMonth(24);
                pUser->GetPlayer()->GetActiveCtrl().SetSignMonth(m);
                for (int i = d; i >= d - n && i >= 1; i--)
                {
                    pUser->GetPlayer()->GetActiveCtrl().AddSignHistory(i, false);
                }

                Value testobj(objectValue);
                testobj["n"] = n;
                objResponseJson["result"] = testobj;
                bDealResult = true;
            }
            if (tokens[0] == "additem" && tokens.size() >= 5)
            {
                int tid = String2Int( tokens[3]);
                int num = String2Int(tokens[4]);

                ItemCfgUnit* pCfg =  (ItemCfgUnit*) ItemCfgMgr::GetInst()->GetUnit(tid);
                if (pCfg != NULL && pCfg->SubItemSet.ItemSize() > 0)
                {
                    for (int i = 0; i < num; i++)
                    {
                        pUser->GetPlayer()->GetBag().AddItemSet( &(pCfg->SubItemSet) ) ;
                    }

                }
                else
                {
                    pUser->GetPlayer()->GetBag().AddItem(tid, num);
                }
                pUser->GetPlayer()->GetLogCtrl().LogBegin()->
                        LogItem("", tid)->
                        LogItem("", num)->
                        LogEnd("AddItem");
                Value testobj(objectValue);
                objResponseJson["result"] = testobj;
                bDealResult = true;

            }
            if (tokens[0] == "openlevel" && tokens.size() >= 4)
            {
                int nLvlTo = String2Int(tokens[3]);
                int nLvlFrom = pUser->GetPlayer()->GetUniverseCtrl().GetTotalLvl();
                int nCnt = 0;
                for (int i = nLvlFrom; i <= nLvlTo; i++)
                {
                    UniverseCfgUnit* pUnit = (UniverseCfgUnit*) UniverseMgr::GetInst()->GetUnit(i);
                    if (pUnit != NULL)
                    {
                        Event fe;
                        std::string fake_attr = "";
                        fake_attr += toString<int>(i) + "," + toString<int>(pUnit->star_need[0]) + ",";
                        fake_attr += toString<int>(pUser->GetPlayer()->GetUniverseCtrl().AllocSeed()) + ",0,1,1";
                        fe.mutable_mce_mcegatewayfunction()->set_jsonattr(fake_attr);
                        PlayerService::gameEnd(&fe, pUser);
                        nCnt ++;
                    }
                }
                objResponseJson["level open cnt"] = nCnt;
                bDealResult = true;

            }
            if (tokens[0] == "direct_set_level_score")
            {
                if (tokens.size() >= 5)
                {
                    int nLvl = String2Int(tokens[3]);
                    int nSocre = String2Int(tokens[4]);
                    objResponseJson["action"] = "Not Set for Error lvl id";
                    if (nLvl >= 0 && nLvl < pUser->GetPlayer()->GetUniverseCtrl().GetTotalLvl())
                    {
                        pUser->GetDbUser().mutable_player()->mutable_universeinfo()->mutable_universe_set(nLvl)->set_value(nSocre);
                    }
                    objResponseJson["action"] = "Score set!";
                    bDealResult = true;
                }
                else
                {
                    objResponseJson["help info"] = "direct_set_level_score,x,id,lvl(0-n),score";
                }
            }
            if (tokens[0] == "reset_nusg")
            {
                BitCache bc(pUser->GetDbUser().mutable_player()->mutable_new_user_save_gift());
                pUser->GetDbUser().set_regist_time(Clock::GetSecond() - 28 * 60 * 60);
                pUser->GetDbUser().set_user_from(0);
                bc.ClearCache();
                bDealResult = true;
            }
            if (tokens[0] == "AddInviteCnt")
            {
                int nAddValue = 1;
                if (tokens.size() >= 4 )
                {
                    nAddValue = String2Int(tokens[3]);
                }
                pUser->GetDbUser().mutable_player()->set_invite_count(pUser->GetDbUser().player().invite_count() + nAddValue);
                bDealResult = true;
                objResponseJson["detail"] = "Add Invite Cnt Succ";
                objResponseJson["now_value"] = pUser->GetDbUser().player().invite_count();
            }
            if (tokens[0] == "set_nui")
            {
                BitCache bc(pUser->GetDbUser().mutable_player()->mutable_new_user_save_gift());
                bc.ClearCache();
                int v = 51;
                if (tokens.size() >= 4) v = String2Int(tokens[3]);
                pUser->GetPlayer()->GetDbPlayer()->set_invite_count(v);
                for (int i = 0; i < 20; i++)
                {
                    pUser->GetPlayer()->GetActiveCtrl().OnSendFriend(1, 55);
                }
                bDealResult = true;
                objResponseJson["detail"] = "set nui succ";
            }
            if (tokens[0] == "reset_alltime")
            {
                for (int i = 0; i < TimeSetCtrl::TST_MAX_; i++)
                {
                    pUser->GetPlayer()->GetTimeSetCtrl().Mark( (TimeSetCtrl::TimeSetType)i, llInvalidTime);
                }
                bDealResult = true;
                objResponseJson["detail"] = "all time reseted!";
            }
            if (tokens[0] == "reset_request")
            {
                pUser->GetPlayer()->GetDbPlayer()->mutable_friendgiftlivehistory()->Clear();
                bDealResult = true;
                objResponseJson["detail"] = "all send reseted!";
            }
            if (tokens[0] == "debug")
            {
                pUser->GetPlayer()->DebugLevel = 1;
                bDealResult = true;
                objResponseJson["detail"] = "set debug level : 1";
            }
            if (tokens[0] == "clear_zy_city")
            {
                bDealResult = true;
                pUser->GetDbUser().mutable_player()->set_from_zynga_city(false);
                BitCache bc(pUser->GetDbUser().mutable_player()->mutable_new_user_save_gift());
                bc.SetBit(ActiveCtrl::rewardbitZyCity, false);
                objResponseJson["detail"] = "set city mark & reward bit reseted !";
            }
            if (tokens[0] == "clear_qq_main_panel")
            {
                BitCache bc(pUser->GetDbUser().mutable_player()->mutable_new_user_save_gift());
                bc.SetBit(ActiveCtrl::rewardbitQQMainPanel, false);
                objResponseJson["detail"] = "set qq_main_panel reward bit reseted !";

            }
            if (tokens[0] == "AddCld")
            {
                pUser->GetPlayer()->GetCLDaysCtrl().AddLoginDay();
                objResponseJson["detail"] = "AddCld succ!";
                pUser->GetPlayer()->GetCLDaysCtrl().FillToJson( objResponseJson["data"]);

            }
            if (tokens[0] == "ClrCld")
            {
                pUser->GetPlayer()->GetCLDaysCtrl().ClearLoginDay();
                objResponseJson["detail"] = "ClrCld succ!!";
                pUser->GetPlayer()->GetCLDaysCtrl().FillToJson( objResponseJson["data"]);
            }
            if (tokens[0] == "ClrInviterOpenid" || tokens[0] == "CIO")
            {
                pUser->GetDbUser().mutable_player()->set_inviter_openid("");
                objResponseJson["detail"] = "Clr Inviter Openid succ!!";
            }
            if (tokens[0] == "SetFriendLvlsDB")
            {
                if (tokens.size() >= 6)
                {
                    pUser->GetPlayer()->GetActiveInvite().SetFriendLvlCnt(String2Int(tokens[3]),
                            String2Int(tokens[4]),
                            String2Int(tokens[5]));
                    objResponseJson["detail"] = "Set friend lvl succ!!";

                }
                else
                {
                    objResponseJson["detail"] = "usage: SetFriendLvlsDB,m,id,f1,f2,f3";
                }
            }
            if (tokens[0] == "ClrRoomReward")
            {
                pUser->GetDbUser().mutable_player()->mutable_room()->set_reward_time(0);
                objResponseJson["detail"] = "Clr RoomReward succ!!";
            }
            if (tokens[0] == "ClearFullbackHistory")
            {
                BitCache bc(pUser->GetDbUser().mutable_player()->mutable_sign_history()->mutable_reward_cache());
                bc.SetBit(ActiveCtrl::rewardbitFullbackLvl1, false);
                bc.SetBit(ActiveCtrl::rewardbitFullbackLvl2, false);
                bc.SetBit(ActiveCtrl::rewardbitFullbackLvl3, false);
                objResponseJson["detail"] = "Clr FullbackHistory succ!!";

            }
            if (tokens[0] == "SetDallySocre")
            {
                if (tokens.size() >= 4)
                {
                    int diw = String2Int(tokens[3]);
                    pUser->GetDbUser().mutable_player()->mutable_dally_universe()->mutable_uniset(diw)->mutable_uni()->set_value(0);
                    objResponseJson["detail"] = "Set Daily Score Succ!";
                }
            }
            if (tokens[0] == "SetLimit")
            {
                if (tokens.size() >= 5)
                {
                    int lid = String2Int(tokens[3]);
                    int num  = String2Int(tokens[4]);
                    pUser->GetPlayer()->GetLimit().SetLimit(lid, num);
                    objResponseJson["detail"] = "SetLimitSucc";
                }
                else
                {
                    objResponseJson["Usage"] = "SetLimit,0/1/3,uid/oid/oid,limit_id,num,sid";
                }
            }
            if (tokens[0] == "resetdailyreward")
            {
                pUser->GetPlayer()->GetDbPlayer()->mutable_dally_universe()->set_rewardpicked(false);
            }
            if (tokens[0] == "SetNoSend")
            {
                bDealResult = Admstr_SetLogNoSend(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "friend")
            {
                bDealResult = Admstr_friend(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "strtest")
            {
                bDealResult = Admstr_teststrbuild(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "GMClearDaily")
            {
                bDealResult = Admstr_removedailyvalue(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "GMSetCheatBit")
            {
                bDealResult = Admstr_SetCheatBit(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "SetDailyCounter")
            {
                bDealResult = Admstr_SetDailyCounter(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "AddDailyCounter")
            {
                bDealResult = Admstr_AddDailyCounter(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "ListDailyCounter")
            {
                Value list(arrayValue);
                for (int i = 0; i < pUser->GetDbUser().player().dailycounter().name_set_size(); i++)
                {
                    Value item(objResponseJson);
                    item["key"] = pUser->GetDbUser().player().dailycounter().name_set(i);
                    item["val"] = pUser->GetDbUser().player().dailycounter().num_set(i);
                    list.append(item);
                }
                objResponseJson["list"] = list;
                bDealResult = true;
            }
            if (tokens[0] == "FullSign")
            {
                int value = 31;
                if (tokens.size() >= 4)
                {
                    value = String2Int(tokens[3]);
                }
                pUser->GetDbUser().mutable_player()->mutable_sign_history()-> clear_days();
                for (int i = 0; i < value; i++)
                {
                    pUser->GetDbUser().mutable_player()->mutable_sign_history()->add_days(i + 1);
                }
                pUser->GetDbUser().mutable_player()->mutable_sign_history()->clear_reward_cache();
            }
            if (tokens[0] == "AddSignleSign")
            {
                pUser->GetDbUser().mutable_player()->mutable_sign_history()->add_days (pUser->GetDbUser().mutable_player()->mutable_sign_history()->days_size() + 1);
            }
            if (tokens[0] == "recall_send")
            {
                bDealResult = Admstr_WebCallbackSender(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "recall_take")
            {
                bDealResult = Admstr_WebCallbackTaker(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "SetPlayerLost")
            {
                bDealResult = Admstr_SetPlayerLost(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "SetCallbackHistorySender")
            {
                bDealResult = Admstr_SetCallbackHistorySender(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "SetCallbackHistoryTaker")
            {
                bDealResult = Admstr_WebCallbackSender(tokens, objResponseJson, pUser);
            }

            if (tokens[0] == "xjdsimport")
            {
                bDealResult = Admstr_SetZyCity(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "SetRegTime")
            {
                bDealResult = AdmStr_SetRegTime(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "AddZongzi")
            {
                pUser->GetPlayer()->GetDragboatFtv().addTotalRiceCnt(String2Int(tokens[3]));
                objResponseJson["detail"] = pUser->GetPlayer()->GetDragboatFtv().getTotalRiceCnt();
            }
            if (tokens[0] == "CLrecv")
            {
                pUser->GetPlayer()->GetActiveInvite().clearrecv();
                objResponseJson["detail"]  = "clear recv1 sucess";
            }
            if (tokens[0] == "fullback")
            {
                bDealResult = AdmStr_FullBack(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "restar")
            {
                bDealResult = Admstr_Restar(tokens, objResponseJson, pUser);
            }
            if (tokens[0] == "qq")
            {
                bDealResult = pUser->GetPlayer()->GetActiveCallManger().AddTotalDays(String2Int(tokens[3]));
            }
            if (tokens[0] == "clearcall")
            {
                bDealResult = pUser->GetPlayer()->GetActiveCallManger().Clear();
            }
            if (tokens[0] == "addusertype")
            {

                bDealResult = pUser->GetPlayer()->GetActiveCallManger().SetUserType(String2Int(tokens[3]));
                pUser->GetPlayer()->save();

                pUser->GetPlayer()->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->LogEnd("addusertype");
            }
            if (tokens[0] == "AddClCounter" && tokens.size() >= 4)
            {
                int v = 1;
                if (tokens.size() >= 5)
                {
                    v = String2Int(tokens[4]);
                }
                DealCenterLogicCounter::getInstance()->PushRequest(tokens[3], v);

            }
            if (tokens[0] == "kos"  )
            {
                if (tokens.size() >= 6)
                {
                    KingOfStar::GetInst()->OnPlayerLevelSucc(pUser->GetPlayer(), String2Int(tokens[3]), 0, String2Int(tokens[4]), 0, String2Int(tokens[5]), objResponseJson);
                }
                else
                {
                    objResponseJson["usage"] = "kos,x,id,lvl,star,score";
                }
            }
            if (tokens[0] == "addexp")
            {
                if (tokens.size() >= 4)
                {
                    pUser->GetPlayer()->GetExpLevelCtrl().AddExp( String2Int(tokens[3]));
                    objResponseJson["detial"] = "Add exp succ";
                }
                else
                {
                    objResponseJson["usage"] = "Add,x,id,exp";
                }
            }
            if (tokens[0] == "ListCenterCounter")
            {
                Value list(arrayValue);
                for (CLCounterMgr::SearchListSK::iterator itor =  CLCounterMgr::GetInst()->m_xKeyList.begin();
                        itor != CLCounterMgr::GetInst()->m_xKeyList.end(); itor++)
                {
                    Value item(objResponseJson);
                    item["key"] = itor->first;
                    item["val"] = ((CLCounterClinetUnit*) (itor->second))->value;
                    item["chg"] = ((CLCounterClinetUnit*) (itor->second))->change;
                    bool bSkip = (tokens.size() >= 4 && itor->first.find(tokens[3]) > 0);
                    if (!bSkip)
                    {
                        list.append(item);
                    }
                }
                objResponseJson["list"] = list;
                bDealResult = true;
            }
            objResponseJson["cmd"] = tokens[0];
            objResponseJson["sid"] = e->sid();
            objResponseJson["succ"] = bDealResult ? 0 : 1;
            str_response = objResponseJson.toStyledString();

        }
        while (0); // end of do-while;
        if (pUser != NULL)
        {
            if (e->sid() == "save")
            {
                eh_->getDataHandler()->markUserDirty(pUser, true, true);

            }
            else
            {
                eh_->getDataHandler()->markUserDirty(pUser);
            }
        }

        Admin_STR_Rsp* rsp = e->mutable_adminstr_rsp();
        rsp->set_uid(req->uid());
        rsp->set_succ(succ);
        rsp->set_adminfd(req->adminfd());
        rsp->set_str(str_response);
        e->set_state(Admin_GW_Rsp);
        eh_->sendEventToWorld(e);
    }
    else
    {
        LOG4CXX_DEBUG(logger_, "Invalid admin command status:" << e->state());
    }
}
//</editor-fold>


/******************************************************************************/
/*    Sub admin str functions                                                 */
/******************************************************************************/
//<editor-fold desc="Sub admin str functions">
static std::string _tmp_EmputStr = "";
#define GETARG(logicIndex) (tokens.size()>=g_nStrLogicArgStart+logicIndex)?tokens[g_nStrLogicArgStart+logicIndex-1]:_tmp_EmputStr
static const std::string strTrue = "1";
static const std::string strFalse = "0";

bool
DealAdminEvent::Admstr_SetLogNoSend(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    if (pUser == NULL)
    {
        return false;
    }

    bool bSetValue = StringToBool( GETARG(1));
    pUser->GetDbUser().mutable_player()->set_skiplog(bSetValue);
    objResponseJson["detail"] = GETARG(1);
    return true;

}

bool
DealAdminEvent::Admstr_friend(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    if (pUser == NULL || tokens.size() < 4)
    {
        return false;
    }
    vector<string> subtokens;
    static string subdelims = ";";
    tokenize(tokens[3], subtokens, subdelims);
    //std::string str_response = "Command Error , try help";
    int n = subtokens.size();
    int o = pUser->GetDbUser().mutable_player()->invite_send_count();
    pUser->GetDbUser().mutable_player()->set_invite_send_count(o + n);
    pUser->GetPlayer()->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
            LogItem("oldv", o)->
            LogItem("newv", o + n)->
            LogItem("ids", tokens[3].c_str())->
            LogEnd("friend");
    pUser->GetPlayer()->GetDailyCounter().AddCounter("daily_invite", n);
    return true;
}

bool
DealAdminEvent::Admstr_teststrbuild(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    int tid = 15;
    int num = 3;
    std::string itemstr = toString<int>(num) + Dictionary::GetStr("unit") + Dictionary::GetStr("item", tid);
    objResponseJson["detail"] = itemstr;
    return true;
}

bool
DealAdminEvent::Admstr_removedailyvalue(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    if (pUser == NULL)
    {
        return false;
    }
    pUser->GetDbUser().mutable_player()->mutable_dally_universe()->Clear();
    objResponseJson["detail"] = "daily history cleared";
    return true;

}

bool
DealAdminEvent::Admstr_SetCheatBit(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    if (pUser == NULL)
    {
        return false;
    }
    if (tokens.size() >= 4 && tokens[3] == "clear")
    {
        objResponseJson["detail"] = "CLEAR";

        pUser->GetDbUser().mutable_player()->mutable_dally_universe()->set_cheat_bit( false);
    }
    else
    {
        objResponseJson["detail"] = "SET";

        pUser->GetDbUser().mutable_player()->mutable_dally_universe()->set_cheat_bit( true);
    }
    return true;
}

bool
DealAdminEvent::Admstr_SetDailyCounter(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    if (pUser == NULL)
    {
        return false;
    }
    if (tokens.size() < 5)
    {
        objResponseJson["info"] = "SetDailyCounter,x,uid/oid,key,v[,sid]";
        return false;
    }
    int v = String2Int(tokens[4]);
    pUser->GetPlayer()->GetDailyCounter().SetCounter(tokens[3], v);

    objResponseJson["detail"] = "Set daily counter succ!";
    return true;
}

bool
DealAdminEvent::Admstr_AddDailyCounter(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    if (pUser == NULL)
    {
        return false;
    }
    if (tokens.size() < 4)
    {
        objResponseJson["info"] = "SetDailyCounter,x,uid/oid,key,v[,sid]";
        return false;
    }
    int v = pUser->GetPlayer()->GetDailyCounter().AddCounter(tokens[3]);
    objResponseJson["detail"] = "Set daily counter succ!";
    LogCtrl* pLog = pUser->GetPlayer()->GetLogCtrl().LogBegin(LT_LogTypeMustSend);
    for (int i = 3; i < tokens.size(); i++)
        pLog->LogItem("kesy", tokens[i].c_str());
    pLog->LogEnd("AddDailyCounter");
    return true;
}

bool
DealAdminEvent::Admstr_WebCallbackSender(admstr_args)
{//recall_send,0,%s,%s 用户召回
    if (tokens.size() < 4)
    {
        return false;
    }
    pUser->GetPlayer()->GetCallbackCtrl().MarkReward(tokens[3]);
    UserCtrl uc(pUser);
    uc.SendBalance();
    uc.SendPoll();
    pUser->GetPlayer()->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
            LogItem("taker", tokens[3].c_str())->
            LogEnd("CallbackSender");
    return true;
}

bool
DealAdminEvent::Admstr_WebCallbackTaker(admstr_args)
{
    //recall_take,0,%s,%s 成功召回
    if (tokens.size() < 4)
    {
        return false;
    }
    bool bSucc = pUser->GetPlayer()->GetCallbackCtrl().MarkSender(tokens[3]);
    if (bSucc)
    {
        UserCtrl uc(pUser);
        uc.SendBalance();
        uc.SendPoll();
        pUser->GetPlayer()->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
                LogItem("sender", tokens[3].c_str())->
                LogEnd("CallbackTaker");
    }
    return bSucc;

}

bool
DealAdminEvent::Admstr_SetCallbackHistorySender(admstr_args)
{
    if (pUser == NULL)
    {
        return false;
    }
    pUser->GetDbUser().mutable_player()->mutable_callback()->Clear();
    pUser->GetPlayer()->GetCallbackCtrl().Update();
    return true;
}

bool
DealAdminEvent::Admstr_SetCallbackHistoryTaker(admstr_args)
{
    if (pUser == NULL)
    {
        return false;
    }

    pUser->GetDbUser().mutable_player()->mutable_callback()->set_reward_cnt_as_taker(0);
    return true;
}

bool
DealAdminEvent::Admstr_SetPlayerLost(admstr_args)
{
    if (pUser == NULL)
    {
        return false;
    }
    FriendInfoLite info;
    pUser->FillAsFriendLite(&info, pUser->getPlatType());

    info.set_lastlogtime( time(NULL) - 36 * 24 * 3600);
    FriendInfoServerHandler::GetInst()->SafePushFriendUpdate(pUser->platform_id(), pUser->getPlatType(), &info);
    objResponseJson["detail"] = "Set Player stats As last login 36 days ago!";
    return true;
}

bool
DealAdminEvent::Admstr_SetZyCity(admstr_args)
{
    if (pUser == NULL)
    {
        return false;
    }
    if (!pUser->GetDbUser().player().from_zynga_city())
    {
        pUser->GetPlayer()->GetActiveCtrl().MarkZyCity();
        pUser->GetPlayer()->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
                LogEnd("ZyCityMark");
    }

    return true;
}

//</editor-fold>


/******************************************************************************/
/*    Fake client event                                                       */
/******************************************************************************/
//<editor-fold desc="Fake client event">

/**
 * 模拟客户端调用.
 * 同时提供给移动版本的数据支持
 * 
 * @param tokens
 * @param objResponseJson
 * @param pUser
 * @return 
 */
bool
DealAdminEvent::Admstr_FakeClientEvent(const std::vector<string>& tokens, Value& objResponseJson, User* pUser, Event* e)
{
    //<editor-fold desc="test">
    Event& fake_event = *e;
    std::string txt = "";

    if (!e->has_mce_mcegatewayfunction())
    {
        fake_event.mutable_mce_mcegatewayfunction()->set_action_idx(llInvalidId);
        fake_event.mutable_mce_mcegatewayfunction()->set_servername(tokens[3]);
        if (tokens[3] == "P" || tokens[3] == "p")
        {
            fake_event.mutable_mce_mcegatewayfunction()->set_servername("PlayerService");
        }
        if (tokens[3] == "m" || tokens[3] == "M")
        {
            fake_event.mutable_mce_mcegatewayfunction()->set_servername("Map");

        }
        fake_event.mutable_mce_mcegatewayfunction()->set_actionname(tokens[4]);
        fake_event.set_state(Status_Normal_Game);

        fake_event.set_cmd(C2S_MceGatewayFunction);
        fake_event.set_time(time(NULL));
        //fake_event.set_state()
        fake_event.set_uid(pUser->id());

        for (int i = 5; i < tokens.size(); i++)
        {
            txt += tokens[i];
            if (i != tokens.size())
            {
                txt += ",";
            }
        }
        fake_event.mutable_mce_mcegatewayfunction()->set_jsonattr(txt);
    }
    else
    {

    }
    DealMceGatewayFunction::getInstance()->handle_(&fake_event);

    //    Value rsp_json1(objectValue);
    //    rsp_json1["cmd"]  = "test";
    //    rsp_json1["ServerName"] = tokens[3];
    //    rsp_json1["ActName"] = tokens[4];
    //    rsp_json1["attlist"] = txt;
    //    rsp_json1["rlt"] = parseJsonStr(fake_event.mutable_mse_msegatewayfunction()->jsonresult());
    //    rsp_json1["succ"] = 0;
    //    rsp_json1["uid"]  = toString<int64 > (pUser->id());
    //    rsp_json1["openid"] = pUser->platform_id();
    //    rsp_json1["sid"] = e->sid();
    //    objResponseJson["result"] = rsp_json1;
    //</editor-fold>    
    return true;
}

//</editor-fold>

/******************************************************************************/
/*    修改注册日期                                                            */
/******************************************************************************/
//<editor-fold desc="修改注册日期* ">

bool
DealAdminEvent::AdmStr_SetRegTime(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{
    int nDay = 0;
    if (tokens.size() >= 4)
    {
        nDay = String2Int(tokens[3]);
    }
    int64 llToday =  Clock::GetDay();
    llToday -= nDay;
    pUser->GetDbUser().set_regist_time( llToday * 3600 * 24);
    objResponseJson["detail"] = "set reg time  succ!";

    return true;
}
//</editor-fold>

/******************************************************************************/
/*    首冲活动                                                                */
/******************************************************************************/
//<editor-fold desc="首冲活动 ">

bool
DealAdminEvent::AdmStr_FullBack(const std::vector<string>& tokens, Value& objResponseJson, User* pUser)
{

    if (tokens.size() >= 6)
    {
        int nToatlPrice = String2Int(tokens[5]);
        pUser->GetDbUser().mutable_player()->set_qpoint_usage_feedback(nToatlPrice);
        pUser->GetPlayer()->GetLogCtrl().LogBegin(LT_LogTypeMustSend)->
                LogItem("qpoint", nToatlPrice)->
                LogEnd("fullback_setprice");
    }
    return true;
}

bool
DealAdminEvent::Admstr_Restar(const std::vector<string>& tokens, Value& objResponseJson, User* pUser  )
{
    if (tokens.size() >= 4)
    {
        DB_UniverseInfo* pInfo = pUser->GetDbUser().mutable_player()->mutable_universeinfo();
        for (int i = 0 ; i < pInfo->universe_set_size(); i++)
        {
            DB_Universe* pUniverse  = pInfo->mutable_universe_set(i);
            UniverseCfgUnit* pCfg = UniverseMgr::GetInst()->GetUniByTotalLvl_QK( i + 1);
            int score = pUniverse->value();
            int stars = pUniverse->stars();
            int nMaxValue = 60 * 10000;
            if (tokens.size() >= 5)
            {
                nMaxValue = String2Int(tokens[4]) * 10000;
            }
            if (score  > nMaxValue)
            {
                int nNewStar = String2Int(tokens[3]) ;
                if (pCfg != NULL && nNewStar >= 1 && nNewStar <= 3)
                {
                    pUniverse->set_value(pCfg->star_need[nNewStar - 1]);
                    pUniverse->set_stars(nNewStar);
                }
                else
                {
                    pUniverse->set_value(3000);
                    pUniverse->set_stars(1);
                }
            }
        }
        objResponseJson["detail"] = "set star time  succ!";

    }
    return true;
}
//</editor-fold>
