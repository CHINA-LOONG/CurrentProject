using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net;
using System.Net.Sockets;
using System.Threading;

using ProtoBuf;
using ProtoBuf.Meta;
using PB;

namespace UnityClientConsole
{
    class Program
    {
        private static byte[] result = new byte[1024];

        static void Main(string[] args)
        {

            App test = new App();
            test.Init("127.0.0.1", 9595, "star_1");
            test.SendLoginProtocol();

//             for (int i = 1; i <= 2000; ++i )
//             {
//                 string name = "root" + i;
//                 App test = new App();
//                 if (test.Init("192.168.199.122", 9597, name) == false)
//                 {
//                     continue; 
//                 }
// 
//                 test.SendLoginProtocol();
//             }


            Console.ReadLine();

            return;

        }
    }
}
