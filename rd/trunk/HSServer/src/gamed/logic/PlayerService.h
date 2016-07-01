/* 
 * File:   PlayerService.h
 * Author: Kidd
 *
 * Created on 2013年1月15日, 上午10:39
 */

#ifndef PLAYERSERVICE_H
#define	PLAYERSERVICE_H
#include "event/EventHandler.h"
class User;
class Player;

class PlayerService
{
public:
    PlayerService();
    ~PlayerService();
public:
    static void gameInit(Event* e, User* pUser);
    static void gameStart(Event* e, User* pUser);
    static void gameEnd(Event* e, User* pUser);
    static void gameEndMobile(Event* e, User* pUser);
    static void poll(Event* e, User* pUser);
    static void pollMobile(Event* e,User* pUser);
    static void getGameConfigurations(Event* e, User* pUser);
    static void getCandyProperties(Event* e, User* pUser);
    static void setCandyProperty(Event* e, User* pUser);
    static void getRecipes(Event* e, User* pUser);
    static void getMessages(Event* e, User* pUser);
    static void getLevelToplist(Event* e, User* pUser);
    static void openLevel(Event* e, User* pUser);
    static void reportFramerate(Event* e, User* pUser);
    static void setSoundFx (Event* e, User* pUser);
    static void setSoundMusic(Event* e, User* pUser);
    static void unlockItem(Event* e, User* pUser);
    static void handOutItemWinnings (Event* e, User* pUser);
    static void useItemsInGameMobile(Event* e, User* pUser);
    static void getItemAount(Event* e, User* pUser);
    static void useItemsInGame(Event* e, User* pUser);
    static void useTicket(Event* e, User* pUser);
    static void getLvlSocres(Event* e,User* pUser);

    static void request(Event* e, User* pUser);
    static void request_taker(Event*e , User* pUser);
    static void request_sender_after(Event*e , User* pUser);

    static void getGift(Event* e, User* pUser);
    static void getMyStarsRecord(Event* e, User* pUser);
    static void updateGuide(Event* e, User* pUser);
    static void buyGoods(Event* e, User* pUser);
    static void getFriendsData(Event* e, User* pUser);
    static void getBalance(Event* e, User* pUser);
    static void getSignin(Event* e, User* pUser);
    static void getFanhua(Event* e, User* pUser);
    static void getFanhuanReward(Event* e, User* pUser);
    static void getTodayCost(Event* e, User* pUser);
    static void stuffReward(Event* e, User* pUser);
    static void getCDkeyReward(Event* e, User* pUser);
    static void getSessionId(Event* e, User* pUser);
    static void action(Event* e, User* pUser);
    static void FlashAddDailyCounter(Event* e,User* pUser);

    static void TryLoad(Event* e, User* pUser);
    static void LoadFriendForMobile(Event* e, User* pUser);

    static void RemoteLog(Event* e, User* pUser);
    static void UnlockItemMobile(Event* e, User* pUser);
    static void AddMobileDevice(Event* e, User* pUser);


    static void Map_PutItem(Event* e, User* pUser);
    static void Map_PeekItem(Event* e, User* pUser);
    static void Map_MoveItem(Event* e, User* pUser);
    static void Map_MapDetail(Event* e, User* pUser);
    static void Map_Bag(Event* e , User* pUser);
    static void Map_SyncMapItem(Event* e, User* pUser);
    static void Map_SyncMaterial(Event* e, User* pUser);
    static void Map_GetEggItem(Event* e, User* pUser);
    static void Map_GetDailyRewardByLuxcy(Event* e, User* pUser);
    static void Map_GetDropLvl(Event* e, User* pUser);
    static void RegAllActions();
} ;

#endif	/* PLAYERSERVICE_H */

