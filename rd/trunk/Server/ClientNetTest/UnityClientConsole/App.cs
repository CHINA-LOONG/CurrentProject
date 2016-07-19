using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProtoBuf;
using ProtoBuf.Meta;
using PB;

namespace UnityClientConsole
{
    class App
    {
        private static App instance;

        private int    playerID;
        private string puid;

        private App()
        {
           
        }
        public static App GetInstance()
        {
            if (instance == null)
            {
                instance = new App();
            }
            return instance;
        }

        public bool Init(string address, int port, string puid)
        {
            if (NetManager.GetInstance().Init(address, port) == false)
            {
                return false;
            }

            this.puid = puid;
            
            return true;
        }

        public void Run()
        {
        }

        public void SendLoginProtocol()
        {
            HSLogin login = new HSLogin();
            login.puid = puid;

            NetManager.GetInstance().SendProtocol(code.LOGIN_C.GetHashCode(), login);
        }


        public void OnProtocol(Protocol protocol)
        {
            
            if (protocol.checkType(sys.ERROR_CODE.GetHashCode()))
            {
                HSErrorCode hsError = protocol.GetProtocolBody<HSErrorCode>();
                Console.WriteLine("" + hsError.hpCode.GetHashCode() + " " + hsError.errCode.GetHashCode().ToString("X2"));
                Console.WriteLine(""); 
            }


            if (protocol.checkType(code.LOGIN_S.GetHashCode()))
            {
                HSLoginRet loginReturn = protocol.GetProtocolBody<HSLoginRet>();

                this.playerID = loginReturn.playerId;

                // 创建角色
                if (playerID == 0)
                {
                    Console.WriteLine("角色不存在，创角色");

                    HSPlayerCreate createRole = new HSPlayerCreate();
                    createRole.puid = puid;
                    createRole.nickname = "NuWa";
                    createRole.career = 1;
                    createRole.gender = 0;
                    createRole.eye = 1;
                    createRole.hair = 1;
                    createRole.hairColor = 1;
                    NetManager.GetInstance().SendProtocol(code.PLAYER_CREATE_C.GetHashCode(), createRole);
                }
                else
                {
                    Console.WriteLine("登录成功");
                    HSSyncInfo syncInfo = new HSSyncInfo();
                    NetManager.GetInstance().SendProtocol(code.SYNCINFO_C.GetHashCode(), syncInfo);
                }
            }
            else if (protocol.checkType(code.PLAYER_CREATE_S.GetHashCode()))
            {
                Console.WriteLine("创建角色成功 ");

                playerID = protocol.GetProtocolBody<HSPlayerCreateRet>().palyerID;
                HSSyncInfo syncInfo = new HSSyncInfo();
                NetManager.GetInstance().SendProtocol(code.SYNCINFO_C.GetHashCode(), syncInfo);
            }
            else if (protocol.checkType(code.SYNCINFO_S.GetHashCode()))
            {
                Console.WriteLine("开始同步信息");
            }
            // 同步----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.PLAYER_INFO_SYNC_S.GetHashCode()))
            {
                HSPlayerInfoSync playerInfo = protocol.GetProtocolBody<HSPlayerInfoSync>();
                Console.WriteLine("同步玩家信息");
            }
            else if (protocol.checkType(code.STATISTICS_INFO_SYNC_S.GetHashCode()))
            {
                HSStatisticsInfoSync statisticsInfo = protocol.GetProtocolBody<HSStatisticsInfoSync>();
                Console.WriteLine("同步统计信息");
            }
            else if (protocol.checkType(code.MONSTER_INFO_SYNC_S.GetHashCode()))
            {
                HSMonsterInfoSync monsterInfo = protocol.GetProtocolBody<HSMonsterInfoSync>();
                Console.WriteLine("同步宠物信息");
            }
            else if (protocol.checkType(code.ITEM_INFO_SYNC_S.GetHashCode()))
            {
                Console.WriteLine("同步道具信息");
            }
            else if (protocol.checkType(code.EQUIP_INFO_SYNC_S.GetHashCode()))
            {
                Console.WriteLine("同步装备信息");
            }
            else if (protocol.checkType(code.QUEST_INFO_SYNC_S.GetHashCode()))
            {
                HSQuestInfoSync questInfo = protocol.GetProtocolBody<HSQuestInfoSync>();
                Console.WriteLine("同步任务信息");
            }
            else if (protocol.checkType(code.MAIL_INFO_SYNC_S.GetHashCode()))
            {
                HSMailInfoSync mailInfo = protocol.GetProtocolBody<HSMailInfoSync>();
                Console.WriteLine("同步邮件信息");
            }
            else if (protocol.checkType(code.ASSEMBLE_FINISH_S.GetHashCode()))
            {
                Console.WriteLine("同步完成");

//                 GMGenTestAccount genAccount = new GMGenTestAccount();
//                 NetManager.GetInstance().SendProtocol(gm.GEN_TEST_ACCOUNT.GetHashCode(), genAccount);

//                 HSMailRead mailRead = new HSMailRead();
//                 mailRead.mailId = 2;
//                 NetManager.GetInstance().SendProtocol(code.MAIL_READ_C.GetHashCode(), mailRead);

//                 HSMailReceive mailReceive = new HSMailReceive();
//                 mailReceive.mailId = 2;
//                 NetManager.GetInstance().SendProtocol(code.MAIL_RECEIVE_C.GetHashCode(), mailReceive);

//                 HSMailReceiveAll mailReceiveAll = new HSMailReceiveAll();
//                 NetManager.GetInstance().SendProtocol(code.MAIL_RECEIVE_ALL_C.GetHashCode(), mailReceiveAll);

//                 HSMonsterStageUp stageUp = new HSMonsterStageUp();
//                 stageUp.monsterId;
//                 stageUp.consumeMonsterId;
//                 NetManager.GetInstance().SendProtocol(code.MONSTER_STAGE_UP_C.GetHashCode(), stageUp);

//                 HSMonsterSkillUp skillUp = new HSMonsterSkillUp();
//                 skillUp.monsterId = 240;
//                 skillUp.skillId = "buffMagic";
//                 NetManager.GetInstance().SendProtocol(code.MONSTER_SKILL_UP_C.GetHashCode(), skillUp);

//                 HSInstanceEnter instanceEnter = new HSInstanceEnter();
//                 instanceEnter.instanceId = "demo";
//                 NetManager.GetInstance().SendProtocol(code.INSTANCE_ENTER_C.GetHashCode(), instanceEnter);

//                 HSItemBuy itemBuy = new HSItemBuy();
//                 itemBuy.itemId = 40001;
//                 itemBuy.itemCount = 1;
//                 NetManager.GetInstance().SendProtocol(code.ITEM_BUY_C.GetHashCode(), itemBuy);
 
//                 HSItemUse itemUse = new HSItemUse();
//                 itemUse.itemId = 40001;
//                 NetManager.GetInstance().SendProtocol(code.ITEM_USE_C.GetHashCode(), itemUse);

//                 HSQuestSubmit questSubmit = new HSQuestSubmit();
//                 questSubmit.questId = 10003;
//                 NetManager.GetInstance().SendProtocol(code.QUEST_SUBMIT_C.GetHashCode(), questSubmit);
            }
            // 副本----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.INSTANCE_ENTER_S.GetHashCode()))
            {
                HSInstanceEnterRet enterReturn = protocol.GetProtocolBody<HSInstanceEnterRet>();
                Console.WriteLine("进入副本");

                HSInstanceSettle instanceSettle = new HSInstanceSettle();
                instanceSettle.passBattleIndex.Add(0);
                instanceSettle.passBattleIndex.Add(1);
                instanceSettle.passBattleIndex.Add(2);
                NetManager.GetInstance().SendProtocol(code.INSTANCE_SETTLE_C.GetHashCode(), instanceSettle);
            }
            else if (protocol.checkType(code.INSTANCE_SETTLE_S.GetHashCode()))
            {
                HSInstanceSettleRet settleReturn = protocol.GetProtocolBody<HSInstanceSettleRet>();
                Console.WriteLine("副本结算");

                HSInstanceOpenCard openCard = new HSInstanceOpenCard();
                openCard.openCount = 1;
                NetManager.GetInstance().SendProtocol(code.INSTANCE_OPEN_CARD_C.GetHashCode(), openCard);

                HSInstanceResetCount resetCount = new HSInstanceResetCount();
                resetCount.instanceId = "demo";
                NetManager.GetInstance().SendProtocol(code.INSTANCE_RESET_COUNT_C.GetHashCode(), resetCount);
            }
            else if (protocol.checkType(code.INSTANCE_OPEN_CARD_S.GetHashCode()))
            {
                HSInstanceOpenCardRet openCardReturn = protocol.GetProtocolBody<HSInstanceOpenCardRet>();
                Console.WriteLine("翻牌");
            }
            else if (protocol.checkType(code.INSTANCE_RESET_COUNT_S.GetHashCode()))
            {
                HSInstanceResetCountRet resetCountReturn = protocol.GetProtocolBody<HSInstanceResetCountRet>();
                Console.WriteLine("重置次数");
            }
            // 物品----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.PLAYER_REWARD_S.GetHashCode()))
            {
                HSRewardInfo rewardInfo = protocol.GetProtocolBody<HSRewardInfo>();
                Console.WriteLine("奖励");
            }
            else if (protocol.checkType(sys.HEART_BEAT.GetHashCode()))
            {
                // HSHeartBeat response = protocol.GetProtocolBody<HSHeartBeat>();

            }
            // 任务----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.QUEST_UPDATE_S.GetHashCode()))
            {
                HSQuestUpdate questUpdate = protocol.GetProtocolBody<HSQuestUpdate>();
                Console.WriteLine("任务更新");
            }
            else if (protocol.checkType(code.QUEST_ACCEPT_S.GetHashCode()))
            {
                HSQuestAccept questAccept = protocol.GetProtocolBody<HSQuestAccept>();
                Console.WriteLine("任务接取");
            }
            else if (protocol.checkType(code.QUEST_SUBMIT_S.GetHashCode()))
            {
                HSQuestSubmitRet questSubmitRet = protocol.GetProtocolBody<HSQuestSubmitRet>();
                Console.WriteLine("任务交付");
            }
            else if (protocol.checkType(code.QUEST_REMOVE_S.GetHashCode()))
            {
                HSQuestRemove questRemove = protocol.GetProtocolBody<HSQuestRemove>();
                Console.WriteLine("任务删除");
            }
            // 怪物----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.MONSTER_SKILL_UP_S.GetHashCode()))
            {
                HSMonsterSkillUpRet skillUp = protocol.GetProtocolBody<HSMonsterSkillUpRet>();
                Console.WriteLine("技能升级");
            }
            else if (protocol.checkType(code.MONSTER_STAGE_UP_S.GetHashCode()))
            {
                HSMonsterStageUpRet stageUp = protocol.GetProtocolBody<HSMonsterStageUpRet>();
                Console.WriteLine("进阶");
            }
            // 邮件----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.MAIL_RECEIVE_S.GetHashCode()))
            {
                HSMailReceiveRet mailReceive = protocol.GetProtocolBody<HSMailReceiveRet>();
                Console.WriteLine("邮件收取");
            }
            else if (protocol.checkType(code.MAIL_RECEIVE_ALL_S.GetHashCode()))
            {
                HSMailReceiveAllRet mailReceive = protocol.GetProtocolBody<HSMailReceiveAllRet>();
                Console.WriteLine("邮件全部收取");
            }
            else if (protocol.checkType(code.MAIL_NEW_S.GetHashCode()))
            {
                HSMailNew mailNew = protocol.GetProtocolBody<HSMailNew>();
                Console.WriteLine("新邮件");
            }
        }

    }
}
