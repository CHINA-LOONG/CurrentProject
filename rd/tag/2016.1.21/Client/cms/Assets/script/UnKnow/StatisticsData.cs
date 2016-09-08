using UnityEngine;
using System.Collections;

//---------------------------------------------------------------------------------------------
public class SpellAttackStatisticsParam
{
	public Spell useSpell;
	public int casterID;
	public int targetID;
	
	public SpellAttackStatisticsParam(Spell spell, int casterID,int targetID)
	{
		useSpell = spell;
		this.casterID = casterID;
		this.targetID = targetID;
	}
}


//public class StatisticsData : MonoBehaviour {

//	// Use this for initialization
//	void Start () {
	
//	}
	
//	// Update is called once per frame
//	void Update () {
	
//	}
//}
