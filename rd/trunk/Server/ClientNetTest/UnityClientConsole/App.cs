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
                    HSPlayerCreate createRole = new HSPlayerCreate();
                    createRole.puid = "zhengshuai";
                    createRole.nickname = "郑帅";
                    createRole.career = 1;
                    createRole.gender = 0;
                    createRole.eye = 1;
                    createRole.hair = 1;
                    createRole.hairColor = 1;
                    NetManager.GetInstance().SendProtocol(code.PLAYER_CREATE_C.GetHashCode(), createRole);
                }
            }
            else if (protocol.checkType(code.PLAYER_CREATE_S.GetHashCode()))
            {

                playerID = protocol.GetProtocolBody<HSPlayerCreateRet>().palyerID;

                SendLoginProtocol();

            }
            else if (protocol.checkType(sys.HEART_BEAT.GetHashCode()))
            {
               // HSHeartBeat response = protocol.GetProtocolBody<HSHeartBeat>();

            }

        }

    }
}
