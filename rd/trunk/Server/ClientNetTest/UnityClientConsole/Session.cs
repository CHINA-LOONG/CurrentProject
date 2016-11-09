using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ProtoBuf;
using ProtoBuf.Meta;
using PB;
using System.Threading;

namespace UnityClientConsole
{
    class Session
    {
        private static Session instance;
        private SocketClient socketClient;

        private int playerId;
        private string nickname;

        public string puid;
        private long lastBeatTime = 0;
        private long lastIMTime = 0;
        private long lastInstanceime = 0;
        private long lastShopRefreshTime = 0;
        static long  lastPrintStateTime = 0;

        static int IMINTERVAL = 60000;
        static int OTHERINTERVAL = 60000;

        public long startTime = 0;
        public int playerIndex = 0;

        Random random = new Random();
        int heartBeatSendCount = 0;
        int heartBeatReceivedCount = 0;
        int summonSendCount = 0;
        int summonReceivedCount = 0;

        int IMSendCount = 0;
        int IMReceivedCount = 0;

        int PVPSendCount = 0;
        int PVPReceivedCount = 0;

        int shopRefreshSendCount = 0;
        int shopRefreshReceivedCount = 0;
        int shopInitReceivedCount = 0;

        int instanceSendCount = 0;
        int instanceReceivedCount = 0;

        static int synCount = 0;
        static long delayTime = 0;
        static int delayCount = 0;

        public bool isAssemble = false;
        public bool isClosed = false;
        private bool hasConnected = false;

        bool firstTest = false;
        volatile bool settle = false;
        volatile bool pvpSettle = false;
        int[] monster = new int[10];
        string[] instanceID = {"yueguangsenlin11", "yueguangsenlin12", "yueguangsenlin14", "yueguangsenlin17", "yueguangsenlin18",
                               "minghe12", "minghe13", "minghe14", "minghe17", "minghe18"};

        public Session()
        {
            
        }

        public static Session GetInstance()
        {
            if (instance == null)
            {
                instance = new Session();
            }

            return instance;
        }

        public bool Init(string address, int port, string puid, int playerIndex, long timeStamp)
        {
            GetInstance();
            socketClient = new SocketClient(this);
            if (socketClient.Init(address, port) == false)
            {
                return false;
            }

            this.startTime = timeStamp - random.Next(1, 1000 * Program.onLineTime / 2);
            this.puid = puid;
            this.playerIndex = playerIndex;

            lastInstanceime = Program.GetTimeStamp() - random.Next(0, IMINTERVAL);
            lastIMTime = Program.GetTimeStamp() - random.Next(0, OTHERINTERVAL);
            lastShopRefreshTime = Program.GetTimeStamp() - random.Next(0, OTHERINTERVAL);

            return true;
        }

        public void Run()
        {

        }

        public void OnProtocol(Protocol protocol)
        {
#region 错误
            if (protocol.checkType(sys.ERROR_CODE.GetHashCode()))
            {
                HSErrorCode hsError = protocol.GetProtocolBody<HSErrorCode>();
                Console.WriteLine("" + hsError.hsCode.GetHashCode() + " " + hsError.errCode.GetHashCode().ToString("X2"));
            }
#endregion
#region 心跳
            else if (protocol.checkType(sys.HEART_BEAT.GetHashCode()))
            {
                lock (instance)
                {
                    heartBeatReceivedCount++;
                }
            }
#endregion                       
#region 登陆
            else if (protocol.checkType(code.LOGIN_S.GetHashCode()))
            {
                HSLoginRet loginReturn = protocol.GetProtocolBody<HSLoginRet>();
                HSSyncInfo syncInfo = new HSSyncInfo();
                socketClient.SendProtocol(code.SYNCINFO_C.GetHashCode(), syncInfo);
                //Console.WriteLine("登录成功");
            }
#endregion           
#region  同步
            else if (protocol.checkType(code.SYNCINFO_S.GetHashCode()))
            {
                // Console.WriteLine("开始同步信息");
                HSSyncInfoRet response = new HSSyncInfoRet();
            }
            // 同步----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.PLAYER_INFO_SYNC_S.GetHashCode()))
            {
                HSPlayerInfoSync playerInfo = protocol.GetProtocolBody<HSPlayerInfoSync>();
                nickname = playerInfo.info.nickname;
                // Console.WriteLine("同步玩家信息");

            }
            else if (protocol.checkType(code.STATISTICS_SYNC_PART1_S.GetHashCode()))
            {
                HSStatisticsSyncPart1 statisticsInfo = protocol.GetProtocolBody<HSStatisticsSyncPart1>();
                // Console.WriteLine("同步统计信息1");
            }
            else if (protocol.checkType(code.STATISTICS_SYNC_PART2_S.GetHashCode()))
            {
                HSStatisticsSyncPart2 statisticsInfo = protocol.GetProtocolBody<HSStatisticsSyncPart2>();
                //Console.WriteLine("同步统计信息2");
            }
            else if (protocol.checkType(code.STATISTICS_SYNC_PART3_S.GetHashCode()))
            {
                HSStatisticsSyncPart3 statisticsInfo = protocol.GetProtocolBody<HSStatisticsSyncPart3>();
                //Console.WriteLine("同步统计信息3");
            }
            else if (protocol.checkType(code.SETTING_INFO_SYNC_S.GetHashCode()))
            {
                HSSettingInfoSync settingSyn = protocol.GetProtocolBody<HSSettingInfoSync>();
            }
            else if (protocol.checkType(code.MONSTER_INFO_SYNC_S.GetHashCode()))
            {
                HSMonsterInfoSync monsterInfoSyn = protocol.GetProtocolBody<HSMonsterInfoSync>();

                if (firstTest == false)
                {
                    for (int i = 0; i < monsterInfoSyn.monsterInfo.Count; ++i)
                    {
                        monster[i] = monsterInfoSyn.monsterInfo[i].monsterId;
                    }

                    if (monsterInfoSyn.monsterInfo.Count < 10)
                    {
                        firstTest = true;
                    }

                    firstTest = true;
                }

            }
            else if (protocol.checkType(code.ITEM_INFO_SYNC_S.GetHashCode()))
            {
                // Console.WriteLine("同步道具信息");
            }
            else if (protocol.checkType(code.EQUIP_INFO_SYNC_S.GetHashCode()))
            {
                // Console.WriteLine("同步装备信息");
            }
            else if (protocol.checkType(code.QUEST_INFO_SYNC_S.GetHashCode()))
            {
                HSQuestInfoSync questInfo = protocol.GetProtocolBody<HSQuestInfoSync>();
                //Console.WriteLine("同步任务信息");
            }
            else if (protocol.checkType(code.MAIL_INFO_SYNC_S.GetHashCode()))
            {
                HSMailInfoSync mailInfo = protocol.GetProtocolBody<HSMailInfoSync>();
                // Console.WriteLine("同步邮件信息");
            }

            else if (protocol.checkType(code.ASSEMBLE_FINISH_S.GetHashCode()))
            {
                isAssemble = true;

                // 补完角色
                if (nickname == "")
                {
                    Console.WriteLine("新角色，补完角色");

                    HSPlayerComplete complete = new HSPlayerComplete();
                    complete.nickname = puid;
                    socketClient.SendProtocol(code.PLAYER_COMPLETE_C.GetHashCode(), complete);
                }

                lock (instance)
                {
                    synCount++;
                    if (synCount == Program.TEST_USER)
                    {
                        Console.WriteLine("全部同步完成");
                    }
                }

                // HSSettingBlock settingBlock = new HSSettingBlock();
                //settingBlock.playerId = 731; //xiaozhen1
                // settingBlock.isBlock = true;
                // socketClient.SendProtocol(code.SETTING_BLOCK_C.GetHashCode(), settingBlock);
                // Console.WriteLine("屏蔽");
                // 
                //                 HSSettingLanguage settingLang = new HSSettingLanguage();
                //                 settingLang.language = "en";
                //                 socketClient.SendProtocol(code.SETTING_LANGUAGE_C.GetHashCode(), settingLang);
                //                 Console.WriteLine("设置语言");

                //                 HSImChatSend chatSend = new HSImChatSend();
                //                 chatSend.channel = ImChannel.WORLD.GetHashCode();
                //                 chatSend.text = "hello world!";
                //                 chatSend.expansion = "ha\0haha";
                //                 socketClient.SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //for (int i = 0; i < 1000; ++i)
                //{
                //    chatSend.text = "hello world!";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "你好世界~";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "这个服务器太给力了，性能超棒，不像腾讯的小霸王服务器";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "这个游戏真不错，和我这一身chanel很搭呢，有那晚和lucy一起在Hawaii的feel";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "操，怎么死机了，sbQQ，干死马化腾";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "法轮功是邪教，我们要听江泽民爷爷的话";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "his document specifies an Internet standards track protocol for the" +
                //        " Internet community, and requests discussion and suggestions for" +
                //        " improvements.  Please refer to the current edition of the \"Internet" +
                //        " Official Protocol Standards\" (STD 1) for the standardization state" +
                //        " and status of this protocol.  Distribution of this memo is unlimited.";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "你只需要记住，我叫叶良辰。";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "在本地我有一百种方法让你活不下去，如果你想试试，良辰不妨陪你玩玩儿！";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "兄台，别逼我动用在北京的势力，我本不想掀起一场腥风血雨！";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "我家三世三代都是军统做事，原子弹，我爷爷都参与研究。";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "不错，我就是叶良辰。你们的行为实在欺人太甚，你们若是感觉有实力跟我玩，良辰不介意奉陪到底。";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "呵呵，我会让你们明白，良辰从不说空话。别让我碰到你们，如果在我的地盘，我有一百种方法让你们待不下去，可你们，却无可奈何。";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "呵呵，良辰最喜欢对那些自认为能力出众的人出手，你们只需要记住，我叫叶良辰。";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "无妨，你们可以把所有认识的人全部叫出来，良辰不介意陪你们玩玩，若我赢了，你们给我乖乖滚出贴吧，别欺人太甚。";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

                //    chatSend.text = "当然，若是你们就此罢手，那良辰在此多谢了，他日，必有重谢。";
                //    NetManager.GetInstance().SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);
                //}

                //Console.WriteLine("聊天");

                //                 HSImPlayerGet imPlayer = new HSImPlayerGet();
                //                 imPlayer.playerId = 725;
                //                 NetManager.GetInstance().SendProtocol(code.IM_PLAYER_GET_C.GetHashCode(), imPlayer);
                //                 Console.WriteLine("获取im玩家信息");

                //                 GMGenTestAccount genAccount = new GMGenTestAccount();
                //                 socketClient.SendProtocol(gm.GEN_TEST_ACCOUNT.GetHashCode(), genAccount);
                //                 Console.WriteLine("生成测试账号");

                //                 GMOperation gmOperation = new GMOperation();
                //                 gmOperation.action = "setpass";
                //                 gmOperation.itemId = "minghe12";
                //                 gmOperation.value = 0;
                //                 socketClient.SendProtocol(gm.GMOPERATION_C.GetHashCode(), gmOperation);


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

                //                 HSMonsterLock monsterLock = new HSMonsterLock();
                //                 monsterLock.monsterId = 4622;
                //                 monsterLock.locked = false;
                //                 NetManager.GetInstance().SendProtocol(code.MONSTER_LOCK_C.GetHashCode(), monsterLock);

                //                 HSMonsterCompose monsterCompose = new HSMonsterCompose();
                //                 monsterCompose.cfgId = "xgXiyiren";
                //                 monsterCompose.useCommon = true;
                //                 socketClient.SendProtocol(code.MONSTER_COMPOSE_C.GetHashCode(), monsterCompose);

                //                 HSInstanceEnter instanceEnter = new HSInstanceEnter();
                //                 instanceEnter.instanceId = "yueguangsenlin11";
                //                 instanceEnter.battleMonsterId.Add(21478);
                //                 instanceEnter.battleMonsterId.Add(21479);
                //                 instanceEnter.battleMonsterId.Add(21480);
                //                 socketClient.SendProtocol(code.INSTANCE_ENTER_C.GetHashCode(), instanceEnter);

                //                 HSInstanceResetCount resetCount = new HSInstanceResetCount();
                //                 resetCount.instanceId = "minghe12";
                //                 socketClient.SendProtocol(code.INSTANCE_RESET_COUNT_C.GetHashCode(), resetCount);

                //                 HSHoleEnter hole = new HSHoleEnter();
                //                 hole.holeId = 1;
                //                 hole.instanceId = "minghe12";
                //                 hole.battleMonsterId.Add(44278);
                //                 socketClient.SendProtocol(code.HOLE_ENTER_C.GetHashCode(), hole);

                //                 HSTowerEnter tower = new HSTowerEnter();
                //                 tower.towerId = 1;
                //                 tower.battleMonsterId.Add(44278);
                //                 socketClient.SendProtocol(code.TOWER_ENTER_C.GetHashCode(), tower);

                //HSChapterBox chapterBox = new HSChapterBox();
                //chapterBox.chapterId = 1;
                //chapterBox.difficulty = 0;
                //socketClient.SendProtocol(code.CHAPTER_BOX_C.GetHashCode(), chapterBox);

                //                 HSItemBuy itemBuy = new HSItemBuy();
                //                 itemBuy.itemId = 40001;
                //                 itemBuy.itemCount = 1;
                //                 NetManager.GetInstance().SendProtocol(code.ITEM_BUY_C.GetHashCode(), itemBuy);

                //                 HSItemUse itemUse = new HSItemUse();
                //                 itemUse.itemId = "50005";
                //                 itemUse.itemCount = 1;
                //                 socketClient.SendProtocol(code.ITEM_USE_C.GetHashCode(), itemUse);

                //                 HSItemBuyAndUse buyUse = new HSItemBuyAndUse();
                //                 buyUse.itemId = "50005";
                //                 buyUse.itemCount = 1;
                //                 socketClient.SendProtocol(code.ITEM_BUY_AND_USE_C.GetHashCode(), buyUse);

                //                 HSQuestSubmit questSubmit = new HSQuestSubmit();
                //                 questSubmit.questId = 10003;
                //                 NetManager.GetInstance().SendProtocol(code.QUEST_SUBMIT_C.GetHashCode(), questSubmit);

                //                 HSAllianceJoinList allianceJoinList = new HSAllianceJoinList();
                //                 allianceJoinList.reqPage = 1;
                //                 socketClient.SendProtocol(code.ALLIANCE_JOINLIST_C.GetHashCode(), allianceJoinList);

                //                 HSAllianceFatigueGive fatigue = new HSAllianceFatigueGive();
                //                 fatigue.targetId = 1891;
                //                 socketClient.SendProtocol(code.ALLIANCE_FATIGUE_GIVE_C.GetHashCode(), fatigue);

                //                 HSAdventureEnter adven = new HSAdventureEnter();
                //                 adven.teamId = 1;
                //                 adven.type = 1;
                //                 adven.gear = 1;
                //                 adven.selfMonsterId.Add(48185);
                //                 adven.selfMonsterId.Add(48186);
                //                 adven.selfMonsterId.Add(48187);
                //                 adven.selfMonsterId.Add(48188);
                //                 adven.selfMonsterId.Add(48189);
                //                 socketClient.SendProtocol(code.ADVENTURE_ENTER_C.GetHashCode(), adven);

                //                 HSAdventureNewCondition newCondition = new HSAdventureNewCondition();
                //                 newCondition.type = 1;
                //                 newCondition.gear = 1;
                //                 socketClient.SendProtocol(code.ADVENTURE_NEW_CONDITION_C.GetHashCode(), newCondition);

                //                 HSAdventureBuyCondition buyCondition = new HSAdventureBuyCondition();
                //                 socketClient.SendProtocol(code.ADVENTURE_BUY_CONDITION_C.GetHashCode(), buyCondition);

                //                 HSAdventureBuyTeam buyTeam = new HSAdventureBuyTeam();
                //                 socketClient.SendProtocol(code.ADVENTURE_BUY_TEAM_C.GetHashCode(), buyTeam);

                //                 HSSummonOne summonOne = new HSSummonOne();
                //                 summonOne.type = 4;
                //                 socketClient.SendProtocol(code.SUMMON_ONE_C.GetHashCode(), summonOne);
                // 
                //                 HSSummonTen summonTen = new HSSummonTen();
                //                 summonTen.type = 4;
                //                 socketClient.SendProtocol(code.SUMMON_TEN_C.GetHashCode(), summonTen);



                HSAllianceCreate createAlliance = new HSAllianceCreate();
                createAlliance.name = puid;
                createAlliance.notice = puid;
                //socketClient.SendProtocol(code.ALLIANCE_CREATE_C.GetHashCode(), createAlliance);

                HSAllianceApply allianceApply = new HSAllianceApply();
                allianceApply.allianceId = random.Next(10085, 10184);
                // socketClient.SendProtocol(code.ALLIANCE_APPLY_C.GetHashCode(), allianceApply);

                HSAllianceHanleApply handleApply = new HSAllianceHanleApply();
                handleApply.accept = true;
                handleApply.isAll = true;
                //socketClient.SendProtocol(code.ALLIANCE_HANDLE_APPLY_C.GetHashCode(), handleApply);

                HSGetPVPDefenceMonster getmonsters = new HSGetPVPDefenceMonster();
                // socketClient.SendProtocol(code.PVP_GET_DEFENCE_MONSTERS_C.GetHashCode(), getmonsters);


                HSPVPMatchTarget matchTarget = new HSPVPMatchTarget();
                //socketClient.SendProtocol(code.PVP_MATCH_TARGET_C.GetHashCode(), matchTarget);

                HSPVPDefenceRecord defenceRecord = new HSPVPDefenceRecord();
                // socketClient.SendProtocol(code.PVP_DEFENCE_RECORD_C.GetHashCode(), defenceRecord);

                HSPVPRank rank = new HSPVPRank();
                //socketClient.SendProtocol(code.PVP_RANK_LIST_C.GetHashCode(), rank);

                HSPVPRankDefence rankDefence = new HSPVPRankDefence();
                rankDefence.playerId = 6660;
                //socketClient.SendProtocol(code.PVP_GET_RANK_DEFENCE_C.GetHashCode(), rankDefence);

                HSSignin signin = new HSSignin();
                signin.month = 2;
                // socketClient.SendProtocol(code.SIGNIN_C.GetHashCode(), signin);

                HSSigninFill signinFill = new HSSigninFill();
                signinFill.month = 2;
                // socketClient.SendProtocol(code.SIGNIN_FILL_C.GetHashCode(), signinFill);
            }
            // 补完角色------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.PLAYER_COMPLETE_S.GetHashCode()))
            {
                Console.WriteLine("补完角色成功 ");
            }
#endregion
#region 刷新
            // 刷新----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.SYNC_DAILY_REFRESH_S.GetHashCode()))
            {
                HSSyncDailyRefresh dailyRefresh = protocol.GetProtocolBody<HSSyncDailyRefresh>();
                Console.WriteLine("每日刷新");
                Console.WriteLine(dailyRefresh.holeState[0].holeId.ToString() + dailyRefresh.holeState[0].isOpen.ToString());
                Console.WriteLine(dailyRefresh.holeState[1].holeId.ToString() + dailyRefresh.holeState[1].isOpen.ToString());
            }
#endregion
#region 副本
            // 副本----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.INSTANCE_ENTER_S.GetHashCode()))
            {
                HSInstanceEnterRet enterReturn = protocol.GetProtocolBody<HSInstanceEnterRet>();
                // Console.WriteLine("进入副本");

                HSInstanceSettle instanceSettle = new HSInstanceSettle();
                instanceSettle.passBattleCount = 3;
                socketClient.SendProtocol(code.INSTANCE_SETTLE_C.GetHashCode(), instanceSettle);

                //HSInstanceRevive instanceRevive = new HSInstanceRevive();
                //NetManager.GetInstance().SendProtocol(code.INSTANCE_REVIVE_C.GetHashCode(), instanceRevive);
            }
            else if (protocol.checkType(code.INSTANCE_SETTLE_S.GetHashCode()))
            {
                HSInstanceSettleRet settleReturn = protocol.GetProtocolBody<HSInstanceSettleRet>();
                // Console.WriteLine("副本结算");
                settle = false;
                //HSInstanceOpenCard openCard = new HSInstanceOpenCard();
                //openCard.openCount = 1;
                //NetManager.GetInstance().SendProtocol(code.INSTANCE_OPEN_CARD_C.GetHashCode(), openCard);
                instanceReceivedCount++;
                HSInstanceResetCount resetCount = new HSInstanceResetCount();
                resetCount.instanceId = "dajie11";
                socketClient.SendProtocol(code.INSTANCE_RESET_COUNT_C.GetHashCode(), resetCount);
            }
            else if (protocol.checkType(code.INSTANCE_OPEN_CARD_S.GetHashCode()))
            {
                HSInstanceOpenCardRet openCardReturn = protocol.GetProtocolBody<HSInstanceOpenCardRet>();
                Console.WriteLine("翻牌");
            }
            else if (protocol.checkType(code.INSTANCE_RESET_COUNT_S.GetHashCode()))
            {
                HSInstanceResetCountRet resetCountReturn = protocol.GetProtocolBody<HSInstanceResetCountRet>();
                //Console.WriteLine("重置次数");
            }
            else if (protocol.checkType(code.INSTANCE_REVIVE_S.GetHashCode()))
            {
                HSInstanceReviveRet reviveReturn = protocol.GetProtocolBody<HSInstanceReviveRet>();
                Console.WriteLine("复活");
            }
            else if (protocol.checkType(code.CHAPTER_BOX_S.GetHashCode()))
            {
                HSChapterBoxRet chapterReturn = protocol.GetProtocolBody<HSChapterBoxRet>();
                Console.WriteLine("满星宝箱");
            }
#endregion
#region 奖励
            // ----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.PLAYER_REWARD_S.GetHashCode()))
            {
                HSRewardInfo rewardInfo = protocol.GetProtocolBody<HSRewardInfo>();
                // Console.WriteLine("奖励");
            }
            else if (protocol.checkType(code.PLAYER_CONSUME_S.GetHashCode()))
            {
                HSConsumeInfo consumeInfo = protocol.GetProtocolBody<HSConsumeInfo>();
                // Console.WriteLine("奖励");
            }
#endregion
#region 商店
            // 商店---------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.SHOP_REFRESH_S.GetHashCode()))
            {
                HSShopRefreshRet rewardInfo = protocol.GetProtocolBody<HSShopRefreshRet>();
                shopRefreshReceivedCount++;
            }
            else if (protocol.checkType(code.ITEM_USE_S.GetHashCode()))
            {
                HSItemUseRet useReturn = protocol.GetProtocolBody<HSItemUseRet>();
                Console.WriteLine("使用");
            }
#endregion
#region 任务
            // 任务----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.QUEST_UPDATE_S.GetHashCode()))
            {
                HSQuestUpdate questUpdate = protocol.GetProtocolBody<HSQuestUpdate>();
                //Console.WriteLine("任务更新");
            }
            else if (protocol.checkType(code.QUEST_ACCEPT_S.GetHashCode()))
            {
                HSQuestAccept questAccept = protocol.GetProtocolBody<HSQuestAccept>();
                //Console.WriteLine("任务接取");
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
#endregion
#region 怪物
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
            else if (protocol.checkType(code.MONSTER_LOCK_S.GetHashCode()))
            {
                HSMonsterLockRet monsterLock = protocol.GetProtocolBody<HSMonsterLockRet>();
                Console.WriteLine("锁定");
            }
            else if (protocol.checkType(code.MONSTER_COMPOSE_S.GetHashCode()))
            {
                HSMonsterComposeRet compose = protocol.GetProtocolBody<HSMonsterComposeRet>();
                Console.WriteLine("怪物合成");

            }
#endregion
#region 邮件
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
                // Console.WriteLine("新邮件");
            }
#endregion
#region IM
            // IM------------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.IM_PUSH_S.GetHashCode()))
            {
                HSImPush chatPush = protocol.GetProtocolBody<HSImPush>();
                //for (int i = 0; i < chatPush.imMsg.Count; ++i)
                //{
                //    Console.WriteLine("------------------");
                //    Console.WriteLine(chatPush.imMsg[i].origText);
                //    Console.WriteLine(chatPush.imMsg[i].transText);
                //}

                IMReceivedCount += chatPush.imMsg.Count;
                //Console.WriteLine("IM------------------");
            }
            else if (protocol.checkType(code.IM_PLAYER_GET_S.GetHashCode()))
            {
                HSImPlayerGetRet imPlayer = protocol.GetProtocolBody<HSImPlayerGetRet>();
                Console.WriteLine("im玩家信息");
            }
#endregion
#region 设置
            // SETTING-------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.SETTING_BLOCK_S.GetHashCode()))
            {
                HSSettingBlockRet block = protocol.GetProtocolBody<HSSettingBlockRet>();
                Console.WriteLine("屏蔽玩家");
            }
            else if (protocol.checkType(gm.GMOPERATION_S.GetHashCode()))
            {
                GMOperationRet imPlayer = protocol.GetProtocolBody<GMOperationRet>();
                Console.WriteLine("gm 操作成功");
            }
            else if (protocol.checkType(code.SHOP_DATA_INIT_S.GetHashCode()))
            {
                shopInitReceivedCount++;
            }
#endregion
#region 工会
            // ALLIANCE-------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.ALLIANCE_CREATE_S.GetHashCode()))
            {
                HSAllianceCreateRet response = protocol.GetProtocolBody<HSAllianceCreateRet>();
                Console.WriteLine("创建公会成功");
            }
            else if (protocol.checkType(code.ALLIANCE_APPLY_S.GetHashCode()))
            {
                //HSAllianceHanleApplyRet response = protocol.GetProtocolBody<HSAllianceHanleApplyRet>();
                Console.WriteLine("申请成功");
            }
            else if (protocol.checkType(code.ALLIANCE_HANDLE_APPLY_S.GetHashCode()))
            {
                //HSAllianceCreateRet response = protocol.GetProtocolBody<HSAllianceCreateRet>();
                Console.WriteLine("申请操作成功");
            }
#endregion
#region 大冒险
            // 大冒险----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.ADVENTURE_ENTER_S.GetHashCode()))
            {
                HSAdventureEnterRet enterReturn = protocol.GetProtocolBody<HSAdventureEnterRet>();
                Console.WriteLine("进入大冒险");

                HSAdventureSettle settle = new HSAdventureSettle();
                settle.teamId = 1;
                socketClient.SendProtocol(code.ADVENTURE_SETTLE_C.GetHashCode(), settle);

                //HSInstanceRevive instanceRevive = new HSInstanceRevive();
                //NetManager.GetInstance().SendProtocol(code.INSTANCE_REVIVE_C.GetHashCode(), instanceRevive);
            }
#endregion
#region 抽蛋
            // 抽蛋----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.SUMMON_ONE_S.GetHashCode()))
            {
                HSSummonOneRet one = protocol.GetProtocolBody<HSSummonOneRet>();
                Console.WriteLine("单抽");
            }
            else if (protocol.checkType(code.SUMMON_TEN_S.GetHashCode()))
            {
                HSSummonTenRet ten = protocol.GetProtocolBody<HSSummonTenRet>();
                //Console.WriteLine("十连抽");
                summonReceivedCount++;
            }
#endregion
#region PVP
            else if (protocol.checkType(code.PVP_SET_DEFENCE_MONSTERS_S.GetHashCode()))
            {
                HSPVPMatchTarget matchTarget = new HSPVPMatchTarget();
                matchTarget.changeTarget = false;
                socketClient.SendProtocol(code.PVP_MATCH_TARGET_C.GetHashCode(), matchTarget);
            }
            else if (protocol.checkType(code.PVP_MATCH_TARGET_S.GetHashCode()))
            {
                HSPVPMatchTargetRet tartget = protocol.GetProtocolBody<HSPVPMatchTargetRet>();
                //Console.WriteLine(tartget.playerId);
                HSPVPEnter enter = new HSPVPEnter();
                socketClient.SendProtocol(code.PVP_ENTER_ROOM_C.GetHashCode(), enter);
            }
            else if (protocol.checkType(code.PVP_ENTER_ROOM_S.GetHashCode()))
            {
                HSPVPSettle settle = new HSPVPSettle();
                settle.result = random.Next(0, 2);
                socketClient.SendProtocol(code.PVP_SETTLE_C.GetHashCode(), settle);
            }
            else if (protocol.checkType(code.PVP_SETTLE_S.GetHashCode()))
            {
                pvpSettle = false;
                PVPReceivedCount++;
            }
            else if (protocol.checkType(code.PVP_DEFENCE_RECORD_S.GetHashCode()))
            {
                HSPVPDefenceRecordRet defenceRecord = protocol.GetProtocolBody<HSPVPDefenceRecordRet>();
                foreach (PVPDefenceRecordData data in defenceRecord.pvpDefenceRecordList)
                {
                    Console.WriteLine(data.attacker + " " + data.result + " " + data.point);
                }
            }
            else if (protocol.checkType(code.PVP_RANK_LIST_S.GetHashCode()))
            {
                HSPVPRankRet rank = protocol.GetProtocolBody<HSPVPRankRet>();
                foreach (PVPRankData data in rank.pvpRankList)
                {
                    Console.WriteLine(data.name + " " + data.point + " " + data.rank);
                }
            }
            else if (protocol.checkType(code.PVP_GET_DEFENCE_MONSTERS_S.GetHashCode()))
            {
                HSGetPVPDefenceMonsterRet monsters = protocol.GetProtocolBody<HSGetPVPDefenceMonsterRet>();
                foreach (int monster in monsters.monsterId)
                {
                    Console.WriteLine("**********" + monster + "**********");
                }
            }
            else if (protocol.checkType(code.PVP_GET_RANK_DEFENCE_S.GetHashCode()))
            {
                HSPVPRankDefenceRet monsters = protocol.GetProtocolBody<HSPVPRankDefenceRet>();
                foreach (HSMonster monster in monsters.monsterDefence.monsterInfo)
                {
                    Console.WriteLine("**********" + monster.monsterId + "**********");
                }
            }
            else if (protocol.checkType(sys.DELAY_TEST.GetHashCode()))
            {
                HSDelayTest delayTest = protocol.GetProtocolBody<HSDelayTest>();

                if (delayTest.id % 100 == 0)
                {
                    delayTime = (Program.GetTimeStamp() - delayTest.timeStamp);
                }
                
            }
#endregion
#region 签到
            // 签到----------------------------------------------------------------------------------------------------------
            else if (protocol.checkType(code.SIGNIN_S.GetHashCode()))
            {
                HSSigninRet signin = protocol.GetProtocolBody<HSSigninRet>();
                Console.WriteLine("签到");
            }
#endregion
        }

        public bool CloseFinish()
        {
            return socketClient.hasClosed;
        }

        public void SendLoginProtocol()
        {
            HSLogin login = new HSLogin();
            login.puid = puid;

            socketClient.SendProtocol(code.LOGIN_C.GetHashCode(), login);
        }

        public void SendHeartBeatProtocol()
        {
            HSHeartBeat heartBeat = new HSHeartBeat();
            socketClient.SendProtocol(sys.HEART_BEAT.GetHashCode(), heartBeat);
        }

        public void SendDealyTeatProtocol()
        {
            HSDelayTest delayTest = new HSDelayTest();
            delayTest.timeStamp = Program.GetTimeStamp();
            delayTest.id = ++delayCount;
            socketClient.SendProtocol(sys.DELAY_TEST.GetHashCode(), delayTest);           
        }

        public void SendSummon()
        {
            HSSummonTen summonTen = new HSSummonTen();
            summonTen.type = 2;
            socketClient.SendProtocol(code.SUMMON_TEN_C.GetHashCode(), summonTen);
        }

        public void SendIMMessage()
        {
            HSImChatSend chatSend = new HSImChatSend();
            chatSend.channel = ImChannel.WORLD.GetHashCode();

            chatSend.text = puid + "This is English. Hello world!";
            socketClient.SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);

            chatSend.text = puid + "这是中文。你好世界！";
            socketClient.SendProtocol(code.IM_CHAT_SEND_C.GetHashCode(), chatSend);
        }

        public void SendInstanceMessage()
        {
            if (settle == false)
            {
                settle = true;
                HSInstanceEnter instanceEnter = new HSInstanceEnter();
                for (int i = 0; i < 5; i++)
                {
                    instanceEnter.battleMonsterId.Add(monster[i]);
                }

                instanceEnter.instanceId = "dajie11";
                socketClient.SendProtocol(code.INSTANCE_ENTER_C.GetHashCode(), instanceEnter);
            }
        }

        public bool isConnected()
        {
            return socketClient.isConnected;
        }

        public void SendRefreshShop()
        {
            HSShopRefresh refreshShop = new HSShopRefresh();
            refreshShop.type = 2;

            socketClient.SendProtocol(code.SHOP_REFRESH_C.GetHashCode(), refreshShop);
        }

        public void SendGetShopData()
        {
            HSShopDataInit shopInit = new HSShopDataInit();

            socketClient.SendProtocol(code.SHOP_DATA_INIT_C.GetHashCode(), shopInit);
        }

        public void SendPVPMatch()
        {
            if (pvpSettle == false)
            {
                pvpSettle = true;
                HSSetPVPDefenceMonster monsters = new HSSetPVPDefenceMonster();
                try
                {
                    monsters.monsterId.Add(monster[5]);
                    monsters.monsterId.Add(monster[6]);
                    monsters.monsterId.Add(monster[7]);
                    monsters.monsterId.Add(monster[8]);
                    monsters.monsterId.Add(monster[9]);
                }
                catch (System.Exception ex)
                {
                    Console.Out.WriteLine(ex.Message);
                }

                socketClient.SendProtocol(code.PVP_SET_DEFENCE_MONSTERS_C.GetHashCode(), monsters);

                PVPSendCount++;
            }
        }

        public void OnTick(long timeStamp)
        {
            if (socketClient.isConnected && hasConnected == false)
            {
                SendLoginProtocol();
                hasConnected = true;
            }

            if (isAssemble == false || isClosed == true)
            {
                return;
            }

            if (startTime + 1000 * Program.onLineTime < timeStamp)
            {
                socketClient.Close();
                isClosed = true;
                return;
            }

            if (lastBeatTime + 3000 < timeStamp)
            {
                SendHeartBeatProtocol();
                SendDealyTeatProtocol();
                heartBeatSendCount++;
                lastBeatTime = timeStamp;
                lastBeatTime += random.Next(0, 3000);
            }

            if (lastIMTime + IMINTERVAL < timeStamp)
            {
                SendIMMessage();
                IMSendCount++;
                lastIMTime = timeStamp - random.Next(0, IMINTERVAL);
            }

            if (lastInstanceime + OTHERINTERVAL < timeStamp)
            {
                SendInstanceMessage();
                instanceSendCount++;
                lastInstanceime = timeStamp - random.Next(0, OTHERINTERVAL);
            }

            if (lastShopRefreshTime + IMINTERVAL < timeStamp)
            {
                SendRefreshShop();
                SendGetShopData();
                SendSummon();
                SendPVPMatch();

                summonSendCount++;
                shopRefreshSendCount++;
                lastShopRefreshTime = timeStamp - random.Next(0, IMINTERVAL);
            }

            if (lastPrintStateTime + 1000 < timeStamp)
            {
                Console.Out.WriteLine("延迟时间:" + delayTime);
           
                lastPrintStateTime = timeStamp;
            }
        }
    }
}
