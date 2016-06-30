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

        public void OnProtocol(Protocol protocol)
        {
            
            if (protocol.checkType(code.LOGIN_C.GetHashCode()))
            {
                
            }
            else if (protocol.checkType(sys.HEART_BEAT.GetHashCode()))
            {
                HPHeartBeat response = protocol.GetProtocolBody<HPHeartBeat>();

            }
        }

    }
}
