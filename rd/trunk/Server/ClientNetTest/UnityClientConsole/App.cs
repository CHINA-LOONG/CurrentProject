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
                    createRole.nickname = "www";
                    createRole.nickname = "monster01";
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
            else if (protocol.checkType(code.ASSEMBLE_FINISH_S.GetHashCode()))
            {
                Console.WriteLine("同步完成");

                //HSInstanceEnter instanceEnter = new HSInstanceEnter();
                //instanceEnter.cfgId = "demo";
                //NetManager.GetInstance().SendProtocol(code.INSTANCE_ENTER_C.GetHashCode(), instanceEnter);

                HSItemUse itemUse = new HSItemUse();
                itemUse.itemId = 40001;
                NetManager.GetInstance().SendProtocol(code.ITEM_USE_C.GetHashCode(), itemUse);
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
            }
            else if (protocol.checkType(code.INSTANCE_OPEN_CARD_S.GetHashCode()))
            {
                HSInstanceOpenCardRet openCardReturn = protocol.GetProtocolBody<HSInstanceOpenCardRet>();
                Console.WriteLine("翻牌");
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

        }

    }
}
