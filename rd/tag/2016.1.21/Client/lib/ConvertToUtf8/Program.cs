using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConvertToUtf8
{
    class Program
    {
        static void Main(string[] args)
        {
            //获取当前路径下所有文件
            string curDir = Environment.CurrentDirectory;
            var allFiles = Directory.GetFiles(curDir, "*.csv");

            //转换成utf8编码并保存
            foreach (var item in allFiles)
            {
                string fileStr;
                using(StreamReader reader = new StreamReader(item, Encoding.GetEncoding("GB2312")))
                {
                    fileStr = reader.ReadToEnd();
                }

                //是否有BOM
                System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding(true);
                using (var writer = new StreamWriter(item, false, utf8))
                {
                    writer.Write(fileStr);
                }

                Console.WriteLine("已转换文件"+item);
            }

            Console.WriteLine("转换完毕！按任意键退出...");
            Console.ReadKey();
        }
    }
}
