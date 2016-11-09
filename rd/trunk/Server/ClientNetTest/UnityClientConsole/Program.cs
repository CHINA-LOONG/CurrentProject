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
        public static int  onLineTime = 600;
        public static int  TEST_USER = 1500;
        public static int  TOTAL_USER = 20000;

        public static HashSet<int> playerIndex = new HashSet<int>();
        public static Random random = new Random();

        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        static void Main(string[] args)
        {
            FileStream aFile = new FileStream("test.txt", FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            string strLine = sr.ReadLine();
            int start = int.Parse(strLine);
            sr.Close();
            aFile.Close();

            Session[] sessions = new Session[TEST_USER];

            while (true)
            {

                for (int i = 0; i < TEST_USER; ++i)
                {

                    if ((sessions[i] == null || sessions[i].CloseFinish()))
                    {
                        if (sessions[i] != null)
                        {
                            playerIndex.Remove(sessions[i].playerIndex);
                        }

                        int nextIndex = 0;

                        while(true){
                            nextIndex = random.Next(start, start + TOTAL_USER / 2);
                            if (!playerIndex.Contains(nextIndex))
                            {
                                playerIndex.Add(nextIndex);
                                break;
                            }
                        }

                        string name = "test" + (nextIndex);

                        sessions[i] = new Session();
                        if (sessions[i].Init("192.168.199.189", 9595, name, nextIndex, GetTimeStamp()) == false)
                        {
                            Console.Out.WriteLine(name);
                        }
                        else
                        {
                            Console.Out.WriteLine(name);
                        }

                    }

                    sessions[i].OnTick(GetTimeStamp());
                }

            }

            Console.ReadLine();

            return;

        }
    }
}
