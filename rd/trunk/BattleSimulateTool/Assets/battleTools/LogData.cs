using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LogData
{
    public int logXhNumber;//第几次循环副本的数据
	public int[] logRoundNumber = new int[3]{0,0,0};//改对局一共的回合数
    public int[] logIsWin = new int[] { 0, 0, 0 };//该对局玩家的胜负1-胜0-负
	public FightData[] playerData = new FightData[3]{new FightData(), new FightData(), new FightData()};//攻击方输出数据 
	public FightData[] enemyData = new FightData[3]{new FightData(), new FightData(), new FightData()};//防守方输出数据
}
public class FightData
{
    public int[] attBloodNumber = new int[] { 0, 0, 0, 0, 0, };//怪物剩余血量
    public int monsterNumber = 0;//剩余怪物数
    public int monsterAttNumber;//怪物总共出手回合
    public int monsterHitNumber;//怪物总共命中回合数
    public int monsterCritNumber;//怪物总共暴击回合数
    public int monsterDazhaoNumber;//怪物总共使用大招的次数    
}
