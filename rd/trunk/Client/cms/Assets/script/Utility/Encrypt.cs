using UnityEngine;
using System.Collections;
using System.IO;
public class Encrypt
{
    //public static bool  IsSpecialFileEncode()
    //{
    //    string filePath = Path.Combine(Util.ResPath, "staticData/donotmodify.csv");
    //    if(!File.Exists(filePath))
    //    {
    //        Logger.LogErrorFormat(" find file error,no file :{0}", filePath);
    //        return false;
    //    }
    //    string text = File.ReadAllText(filePath);
    //    if (string.IsNullOrEmpty(text))
    //    {
    //        return false;
    //    }
    //    if (text.Equals("no use"))
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return true;
    //    }
    //}
    public static byte[] encodeFile(string key,string filePath)
    {
        byte[] box = rc4_init(key);
        if (!File.Exists(filePath))
        {
            Logger.LogErrorFormat("encode file error,no file :{0}", filePath);
            return null;
        }
        string directory = Path.GetDirectoryName(filePath);
        string fileName = Path.GetFileName(filePath);
        byte[] buffer = File.ReadAllBytes(filePath);

        for (int i = 0, low = 0, high = 0, mid; i < buffer.Length; i++)
        {
            low = (low + 1) % 255;
            high = (high + box[i % 255]) % 255;


            byte b = box[low];
            box[low] = box[high];
            box[high] = b;


            mid = (box[low] + box[high]) % 255;
            buffer[i] ^= box[mid];
        }
        return buffer;
    }
    public static void encodeFileAndSave(string key,string filePath)
    {
        byte[] buffer = encodeFile(key, filePath);
        if(null != buffer)
        {
            FileStream fstream = File.OpenWrite(filePath);
            fstream.Write(buffer, 0, buffer.Length);
            fstream.Flush();
            fstream.Close();
        }
        
    }
    public static string encode(string key, string value)
    {
        byte[] box = rc4_init(key);
        char[] buffer = value.ToCharArray();
        for (int i = 0, low = 0, high = 0, mid; i < buffer.Length; i++)
        {
            low = (low + 1) % 255;
            high = (high + box[i % 255]) % 255;


            byte b = box[low];
            box[low] = box[high];
            box[high] = b;


            mid = (box[low] + box[high]) % 255;
            buffer[i] ^= (char)box[mid];
        }
        return new string(buffer);
    }

    static byte[] rc4_init(string key)
    {
        byte[] box = new byte[255];
        for (int i = 0; i < 255; i++)
            box[i] = (byte)i;
        for (int i = 0, j = 0; i < 255; i++)
        {
            j = (j + box[i] + key[i % key.Length]) % 255;
            byte b = box[i];
            box[i] = box[j];
            box[j] = b;
        }
        return box;
    }
}
