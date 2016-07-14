using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

//public class Condition
//{
//    public string func;
//    public Dictionary<int, int> rets = new Dictionary<int, int>();

//    public MethodInfo method;
//}

//public class ProcessData
//{
//    //从1开始
//    public int index;
//    public string processAnim;
//    public string preAnim;
//    public bool needClearBuff;
//    public string func;
//    public Dictionary<int, int> rets = new Dictionary<int, int>();

//    public MethodInfo method = null;

//    public void ParseCondition(string con)
//    {
//        if (string.IsNullOrEmpty(con))
//            return;

//        Hashtable table = MiniJSON.jsonDecode(con) as Hashtable;
//        func = table["func"].ToString();
//        var returnCodes = table["ret"] as ArrayList;
//        foreach (var item in returnCodes)
//        {
//            var ret = item as Hashtable;
//            var val = int.Parse(ret["val"].ToString());
//            var gotoVal = int.Parse(ret["goto"].ToString());
//            rets.Add(val, gotoVal);
//        }
//    }
//}

//副本
public class InstanceProtoData
{
    public string id;
    public string name;
    public int level;
    public float lifeCoef;
    public float attackCoef;
    //public float expCoef;
    //public float goldCoef;
    //public string sceneList;
    public string sceneID;
    public string monsterList;
    public string battleLevelList;
    public string battleBoss;
    public string backgroundmusic;

	public int dazhaoGroup;
	public int dazhaoAdjust;
}

public class InstanceData
{
    public InstanceProtoData instanceProtoData;
    //public List<string> sceneList = new List<string>();
    public List<string> battleLevelList = new List<string>();
    //public Dictionary<string, int> monsterList = new Dictionary<string, int>();
}

//对局数据
public class BattleLevelProtoData
{
    public string id;

    //public string bossID;
    //public string sceneID;
    public string preStartEvent;//开场前对话
    public string startEvent;//开场动画
    public string endStartEvent;//开场动画结束了对话
    public int appearType;
    public string winFunc;
    public string loseFunc;
    public string endEvent;//对局结束对话
    public string monsterList;
}

public class BattleLevelData
{
    public BattleLevelProtoData battleProtoData;
    public MethodInfo winFunc = null;
    public MethodInfo loseFunc = null;
    public Dictionary<string, int> monsterList = new Dictionary<string, int>();
}
