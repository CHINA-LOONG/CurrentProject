using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;

public class MiniJsonData
{
	public object mData;
//	public static int mTempNewObj = 0;
	public MiniJsonData(object obj)
	{
		mData = obj;
	}
	public MiniJsonData this[string prop_name] 
	{
		get 
		{
			Hashtable ht = (Hashtable)mData;
			if (!ht.ContainsKey(prop_name))
				return null;
			object val = ht[prop_name];
			return new MiniJsonData(val);
		}
	}
	public MiniJsonData this[int index]
	{
		get
		{
			object val = ((ArrayList)mData)[index];
			return new MiniJsonData(val);
		}
	}
	static public MiniJsonData Wrap(object obj)
	{
		if (obj == null)
			return null;
		return new MiniJsonData(obj);
	}
	static public MiniJsonData Parse(string content)
	{
		object obj = MiniJSON.jsonDecode(content);
		return new MiniJsonData(obj);
	}
	public static explicit operator bool(MiniJsonData mjd)
    {
		return Convert.ToBoolean(mjd.mData);
    }
	public static explicit operator int(MiniJsonData mjd)
    {
		return Convert.ToInt32(mjd.mData);
    }
	public static explicit operator float(MiniJsonData mjd)
    {
		return Convert.ToSingle(mjd.mData);
    }
   	public static explicit operator double(MiniJsonData mjd)
    {
        return Convert.ToDouble(mjd.mData);
    }
   	public static explicit operator string(MiniJsonData mjd)
    {
        return Convert.ToString(mjd.mData);
    }
	public static explicit operator Hashtable(MiniJsonData mjd)
	{
		return (Hashtable)mjd.mData;
	}
	public static explicit operator ArrayList(MiniJsonData mjd)
	{
		return (ArrayList)mjd.mData;
	}
	
    public override string ToString()
   	{
     return Convert.ToString(mData);
   	}
}

