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
                mInst.Lang = (Language)PlayerPrefs.GetInt(mInst.strKeyLanguage, (int)Language.Chinese);
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
            if (mInst.font!=null)
            {
                Resources.UnloadAsset(mInst.font);
            }
            mInst.font = setFontByLang(lang);
        }
    }

    private Font font;

    public Font Font
    {
        get { return font; }
        set 
        {
            font = value; 
        }
    }

    public void SetLanguageFont(GameObject go)
    {
        if (Font == null) return;
        System.Action<Transform> setFont = (label) =>
        {
            label.GetComponent<UnityEngine.UI.Text>().font = Font;
        };
        Util.SetChild_UsedT<UnityEngine.UI.Text>(go.transform, setFont);
    }

    Font setFontByLang(Language lang)
    {
        Font font=null;
        switch (Lang)
        {
            case Language.Chinese:
                font = Resources.Load("Font/SIMHEI") as Font;
                break;
            case Language.English:
                font = Resources.Load("Font/FZLTCXHJW") as Font;
                break;
            default:
                break;
        }
        return font;
    }
}
