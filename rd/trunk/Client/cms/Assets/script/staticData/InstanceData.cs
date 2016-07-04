using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Condition
{
    public string func;
    public Dictionary<int, int> rets = new Dictionary<int, int>();

    public MethodInfo method;
}

public class ProcessData
{
    //从1开始
    public int index;
    public string processAnim;
    public string preAnim;
    public bool needClearBuff;
    public string func;
    public Dictionary<int, int> rets = new Dictionary<int, int>();

    public MethodInfo method = null;

    public void ParseCondition(string con)
    {
        if (string.IsNullOrEmpty(con))
            return;

        Hashtable table = MiniJSON.jsonDecode(con) as Hashtable;
        func = table["func"].ToString();
        var returnCodes = table["ret"] as ArrayList;
        foreach (var item in returnCodes)
        {
            var ret = item as Hashtable;
            var val = int.Parse(ret["val"].ToString());
            var gotoVal = int.Parse(ret["goto"].ToString());
            rets.Add(val, gotoVal);
        }
    }
}

public class InstanceData
{
    public string id;
    public string name;
    public int level;
    public float lifeCoef;
    public float attackCoef;
    public float expCoef;
    public float goldCoef;
    public string rewardList;
    public string sceneBattle;
    public string sceneAmount;
    public string monsterAmount;

	public int dazhaoGroup;
	public int dazhaoAdjust;

    public string monster1;
    public int monster1Amount;
    public string monster2;
    public int monster2Amount;
    public string monster3;
    public int monster3Amount;
    public string monster4;
    public int monster4Amount;
    public string monster5;
    public int monster5Amount;
    public string normalValiVic;
    public string bossID;
    public string bossStoryStartAnimation;
    public string bossStoryEndAnimation;
    public string pre1Animation;
    public string process1Animation;
    public byte is1ClearBuff;
    public string bossValiP1;
    public string pre2Animation;
    public string process2Animation;
    public byte is2ClearBuff;
    public string bossValiP2;
    public string pre3Animation;
    public string process3Animation;
    public byte is3ClearBuff;
    public string bossValiP3;
    public string pre4Animation;
    public string process4Animation;
    public byte is4ClearBuff;
    public string bossValiP4;
    public string pre5Animation;
    public string process5Animation;
    public byte is5ClearBuff;
    public string bossValiP5;
    public string bossValiVic;
    public string rareID;
    public float rareProbability;
    public string rareStoryStartAnimation;
    public string rareStoryEndAnimation;
    public string preRare1Animation;
    public string processRare1Animation;
    public byte isRare1ClearBuff;
    public string rareValiP1;
    public string preRare2Animation;
    public string processRare2Animation;
    public byte isRare2ClearBuff;
    public string rareValiP2;
    public string preRare3Animation;
    public string processRare3Animation;
    public byte isRare3ClearBuff;
    public string rareValiP3;
    public string preRare4Animation;
    public string processRare4Animation;
    public byte isRare4ClearBuff;
    public string rareValiP4;
    public string preRare5Animation;
    public string processRare5Animation;
    public byte isRare5ClearBuff;
    public string rareValiP5;
    public string rareValiVic;

    public List<ProcessData> bossProcess = new List<ProcessData>();
    public List<ProcessData> rareProcess = new List<ProcessData>();
    public MethodInfo normalValiVicMethod = null;
    public MethodInfo bossValiVicMethod = null;
    public MethodInfo rareValiVicMethod = null;
}
