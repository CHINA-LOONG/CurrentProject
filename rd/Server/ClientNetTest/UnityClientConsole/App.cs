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

        public void SendCreateRoleProtocol()
        {
            HSRoleCreate roleCreate = new HSRoleCreate();
            roleCreate.nickname = "test1";
            roleCreate.career = 1;
            roleCreate.eye = 1;
            roleCreate.hair = 1;
            roleCreate.hairColor = 1;
            roleCreate.gender = 1;

            NetManager.GetInstance().SendProtocol(code.ROLE_CREATE_C.GetHashCode(), roleCreate);
        }

        public void SendDeleteRoleProtocol(int roleID)
        {
            HSRoleDelete roleDelete = new HSRoleDelete();
            roleDelete.roleId = roleID;

            NetManager.GetInstance().SendProtocol(code.ROLE_DELETE_C.GetHashCode(), roleDelete);
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
               else if (hsError.hpCode == code.ROLE_CREATE_C.GetHashCode())
               {
                   if (hsError.errCode == roleError.ROLE_NICKNAME_EXIST.GetHashCode())
                   {
                       Console.WriteLine("昵称重复");
                   }
                   else if (hsError.errCode == roleError.ROLE_MAX_SIZE.GetHashCode())
                   {
                       Console.WriteLine("角色数量达到上限");
                   }
               }
               else if (hsError.hpCode == code.ROLE_DELETE_C.GetHashCode())
               {
                   if (hsError.errCode == roleError.ROLE_NOT_EXIST.GetHashCode())
                   {
                       Console.WriteLine("要删除的角色不存在");
                   }
               }
                    

            }


            if (protocol.checkType(code.LOGIN_S.GetHashCode()))
            {
                HSLoginRet loginReturn = protocol.GetProtocolBody<HSLoginRet>();

                this.playerID = loginReturn.playerId;
            }
            else if (protocol.checkType(code.ROLE_CREATE_S.GetHashCode()))
            {
                HSRoleCreateRet response = protocol.GetProtocolBody<HSRoleCreateRet>();
                int roleID = response.roleId;
                Console.WriteLine(roleID);
            }
            else if (protocol.checkType(code.ROLE_DELETE_C.GetHashCode()))
            {
                HSRoleDeleteRet response = protocol.GetProtocolBody<HSRoleDeleteRet>(); 
                if (response.status == error.NONE_ERROR.GetHashCode())
                {
                    Console.WriteLine("角色删除成功{}", response.roleId);
                }
            }
            else if (protocol.checkType(sys.HEART_BEAT.GetHashCode()))
            {
                HSHeartBeat response = protocol.GetProtocolBody<HSHeartBeat>();

            }
        }

    }
}
