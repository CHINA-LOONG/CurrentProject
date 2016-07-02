using UnityEngine;
using System.Collections;

public class GameEventList
{
    //Login
    public static string LoginClick = "LoginClick";//no param

    //Build
    public static string BattleBtnClick = "BattleBtnClick";

    //////////////////////////////////////////////////////////////////////////
    //Battle
    public static string StartBattle = "StartBattle";  //param: PbStartBattle  
	public static string LoadBattleObjectFinished = "LoadBattleObjectFinished";//param GameUnitData

	//Mirror in the Battle
	public static string SetMirrorModeState = "SetMirrorModeState"; //param bool 
	public static string FindWeakPoint = "FindWeakPoint";//param MirrorTarget ,MirrorTarget
	public static string FindFinishedWeakPoint = "FindFinishedWeakPoint";//param MIrrorTarget
	public static string MirrorOutWeakPoint = "MirrorOutWeakPoint";//param MIrrorTarget

    //process
    public static string SwitchPet = "SwitchPet";//param <int,int>
	public static string ShowSwitchPetUI = "ShowSwitchPetUI";//param  int
	public static string HideSwitchPetUI = "HideSwitchPetUI";//no param
    public static string ChangeTarget = "ChangeTarget";

    //UI in the battle
    public static string ShowBattleUI = "ShowBattleUI";

    //////////////////////////////////////////////////////////////////////////


    //Ui
    public static string RestartGame = "RestartGame";
    public static string RefreshBattleResultUI = "RefreshBattleResultUI";//UIData
    public static string ResetOperHabit = "ResetOperHabit"; //设置操作习惯

    //Net
    public static string NetRequestState = "NetRequestState";//object
    public static string OnLoginMsg = "OnLoginMsg";

    //Spell
    public static string SpellFire = "SpellFire";
    public static string SpellLifeChange = "SpellLifeChange";
    public static string SpellEnergyChange = "SpellEnergyChange";
    public static string SpellUnitDead = "SpellUnitDead";
    public static string SpellBuff = "SpellBuff";
    public static string SpellEffect = "SpellEffect";
    public static string SpellMiss = "SpellMiss";
    public static string SpellImmune = "SpellImmune";
    public static string SpellStun = "SpellStun";
}
