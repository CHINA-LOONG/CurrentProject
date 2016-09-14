using UnityEngine;
using System.Collections;

public class UpdateHelpter
{
    public  static  bool IsResouceExtracted()
    {
        int clientInitCode = Const.resouceCode;
        int saveCode = GetResouceCode();
        return saveCode >= clientInitCode;
    }

    public static int GetResouceCode()
    {
        int savecode = PlayerPrefs.GetInt("resouceCode");
        return savecode;
    }
    public static void SetResouceCode(int resCode)
    {
        PlayerPrefs.SetInt("resouceCode", resCode);
    }

    
}
