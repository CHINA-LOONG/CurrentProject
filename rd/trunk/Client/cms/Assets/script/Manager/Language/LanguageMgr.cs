using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Language
{
    Chinese,
    English
}

public class LanguageMgr
{

    static LanguageMgr mInst = null;
    public static LanguageMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                mInst = new LanguageMgr();
                mInst.lang = (Language)PlayerPrefs.GetInt(mInst.strKeyLanguage, (int)Language.Chinese);
            }
            return mInst;
        }
    }

    private string strKeyLanguage = "language";
    private Language lang;

    public Language Lang
    {
        get
        {
            //lang = (Language)PlayerPrefs.GetInt(strKeyLanguage, (int)Language.Chinese);
            return lang;
        }
        set
        {
            lang = value;
            PlayerPrefs.SetInt(strKeyLanguage, (int)lang);
        }
    }
}
