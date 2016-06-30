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

        private int   playerID;


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

        public bool Init(string address, int port)
        {
            if (NetManager.GetInstance().Init(address, port) == false)
            {
                return false;
            }

            return true;
        }

        public void Run()
        {
        }

        public void SendLoginProtocol(String puid)
        {
            HSLogin login = new HSLogin();
            login.puid = puid;

            NetManager.GetInstance().SendProtocol(code.LOGIN_C.GetHashCode(), login);
        }

        public void OnProtocol(Protocol protocol)
        {
            
            if (protocol.checkType(code.LOGIN_S.GetHashCode()))
            {
                HSLoginRet loginReturn = protocol.GetProtocolBody<HSLoginRet>();

                this.playerID = loginReturn.playerId;

                HSRoleCreate roleCreate = new HSRoleCreate();
                roleCreate.nickname = "test1";
                roleCreate.career = 1;
                roleCreate.eye = 1;
                roleCreate.hair = 1;
                roleCreate.hairColor = 1;
                roleCreate.gender = 1;


                NetManager.GetInstance().SendProtocol(code.ROLE_CREATE_C.GetHashCode(), roleCreate);





            }
            else if (protocol.checkType(sys.HEART_BEAT.GetHashCode()))
            {
                HSHeartBeat response = protocol.GetProtocolBody<HSHeartBeat>();

            }
            else if (protocol.checkType(code.ROLE_CREATE_S.GetHashCode()))
            {
                HSRoleCreateRet response = protocol.GetProtocolBody<HSRoleCreateRet>();
                int roleID = response.roleId;

                Console.WriteLine(roleID);


            }
        }

    }
}
