using UnityEngine;
using System.Collections.Generic;

public class HoleStaticData
{
	public string id;
    public string time;
    public int count;
    public string openId;
    public string dropId;
    public string difficulty;
}

public class HoleData
{
    public string id;
    public string time;
    public int count;
    public string openId;
    public string dropId;
    public List<string> difficultyList = new List<string>();
}
