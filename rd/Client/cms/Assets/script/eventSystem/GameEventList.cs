using UnityEngine;
using System.Collections;

public class GameEventList
{
	//Battle
	public static string EnterTerrain = "EnterTerrain";  //param: Collision

	public static string EatCoin = "EatCoin";			//param: Coin
	public static string EatItemMagnet = "EatItemMagnet";

	public static string HitedByObstacle = "HitedByObstacle";
	public static string HitedByPineNut = "HitedByPineNut";

	public static string EnterNewLevel = "EnterNewLevel";  //param: LevelFragment

	//Data
	public static string LifeDataChanged = "LifeDataChanged"; //float


	public static string StaminaRepair = "StaminaRepair";//object

	//Ui
	public static string RestartGame = "RestartGame";
	public static string RefreshBattleResultUI = "RefreshBattleResultUI";//UIData
	public static string ResetOperHabit = "ResetOperHabit"; //设置操作习惯

	//Net
	public static string NetRequestState = "NetRequestState";//object
	public static string OnLoginMsg = "OnLoginMsg";

}
