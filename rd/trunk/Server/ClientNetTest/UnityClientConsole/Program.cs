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
        static int  TEST_USER = 1;

        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        } 

        static void Main(string[] args)
        {
//             FileStream aFile = new FileStream("test.txt", FileMode.Open);
//             StreamReader sr = new StreamReader(aFile);
//             string strLine = sr.ReadLine();
//             int start = int.Parse(strLine);
//             sr.Close();
//             aFile.Close();

            App[] test = new App[TEST_USER];

            for (int i = 0; i < TEST_USER; ++i)
            {
                string name = "shuadong2";// +(1 + i);
                //string name = "root" + (1 + 1 + i);
               // string name = "_chat" + (1 + i);
                //string name = "11171002";

                test[i] = new App();
                if (test[i].Init("127.0.0.1", 9595, name) == false)
                {
                    Console.Out.WriteLine(name);
                }

            }

            for (int i = 0; i < TEST_USER; ++i)
            {
                test[i].SendLoginProtocol();
            }

            System.Threading.Thread.Sleep(4000);

            //while (true)
            //{
            //    for (int i = 0; i < TEST_USER; ++i)
            //    {
            //        test[i].OnTick(GetTimeStamp());
            //    }

            //    //System.Threading.Thread.Sleep(10);
            //}

            Console.ReadLine();

            return;

        }
    }
}
