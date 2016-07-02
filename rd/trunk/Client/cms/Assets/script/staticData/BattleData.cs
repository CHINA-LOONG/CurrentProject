using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleData
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
    public string preAnimation;
    public string process1Animation;
    public byte is1ClearBuff;
    public string bossValiP1;
    public string process2Animation;
    public byte is2ClearBuff;
    public string bossValiP2;
    public string process3Animation;
    public byte is3ClearBuff;
    public string bossValiP3;
    public string process4Animation;
    public byte is4ClearBuff;
    public string bossValiP4;
    public string process5Animation;
    public byte is5ClearBuff;
    public string bossValiVic;
    public string rareID;
    public float rareProbability;
    public string preRareAnimation;
    public string processRare1Animation;
    public byte isRare1ClearBuff;
    public string rareValiP1;
    public string processRare2Animation;
    public byte isRare2ClearBuff;
    public string rareValiP2;
    public string processRare3Animation;
    public byte isRare3ClearBuff;
    public string rareValiP3;
    public string processRare4Animation;
    public byte isRare4ClearBuff;
    public string rareValiP4;
    public string processRare5Animation;
    public byte isRare5ClearBuff;
    public string rareValiVic;

    public List<int> processList = new List<int>();

}
