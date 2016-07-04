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

               if (hsError.hpCode == code.LOGIN_C.GetHashCode())
               {
                   if (hsError.errCode == error.ONLINE_MAX_LIMIT.GetHashCode())
                   {
                       Console.WriteLine("在线人数超限制");
                   }
               }     
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
                    createRole.puid = "zs";
                    createRole.nickname = "qq";
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
                Console.WriteLine("创建角色成功");

                playerID = protocol.GetProtocolBody<HSPlayerCreateRet>().palyerID;
                HSSyncInfo syncInfo = new HSSyncInfo();
                NetManager.GetInstance().SendProtocol(code.SYNCINFO_C.GetHashCode(), syncInfo);

            }
            else if (protocol.checkType(code.SYNCINFO_S.GetHashCode()))
            {

                Console.WriteLine("开始同步信息");

            }
            else if (protocol.checkType(code.PLAYER_INFO_SYNC_S.GetHashCode()))
            {

                Console.WriteLine("同步玩家信息");

            }
            else if (protocol.checkType(code.STATISTICS_INFO_SYNC_S.GetHashCode()))
            {

                Console.WriteLine("同步统计信息");

            }
            else if (protocol.checkType(code.MONSTER_INFO_SYNC_S.GetHashCode()))
            {
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

            }
            else if (protocol.checkType(sys.HEART_BEAT.GetHashCode()))
            {
               // HSHeartBeat response = protocol.GetProtocolBody<HSHeartBeat>();

            }

        }

    }
}
