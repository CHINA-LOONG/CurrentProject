using UnityEngine;
using System.Collections.Generic;

public class HoleStaticData
{
    public int id;
	public string textId;
    public string time;
    public int count;
    public string openId;
    public string dropId;
    public string difficulty;
}

public class HoleData
{
    public int id;
    public string time;
    public int count;
    public string openId;
    public string dropId;
    public List<string> difficultyList = new List<string>();
}
