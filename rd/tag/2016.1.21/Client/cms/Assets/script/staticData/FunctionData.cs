using UnityEngine;
using System.Collections;

public class FunctionData 
{
    public string name;
    public string desc;
    public int needlevel;

    public string NameAttr
    {
        get
        {
            return StaticDataMgr.Instance.GetTextByID(name);
        }
    }

    public  string  DescAttr
    {
        get
        {
            return StaticDataMgr.Instance.GetTextByID(desc);
        }
    }
}
