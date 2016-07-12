using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeakPointData
{
    public string id;
    public int type;
    public byte isDamagePoint;//是否关联boss减血
    public int health;
    public byte isTarget;

	public string node;
	public string boxColliders;

	//鉴定效果
	public string findWpEffect;
	public string appraisalStateEffect;
	//-----------------------------------------

    public int initialStatus;

	public int tipType;	
	public int tipOffsetX;
	public int tipOffsetY;

	public float damageRate1;//伤害系数
	public float damageRate2;//伤害系数

	public int property1;
	public int property2;

	public string stateparam0;
	public string stateparam1;
	public string stateparam2;
	public string stateparam3;
	public string stateparam4;

	//
	public Hashtable state0;
	public Hashtable state1;
	public Hashtable state2;
	public Hashtable state3;
	public Hashtable state4;

	public void AdaptData()
	{
		state0 = MiniJsonExtensions.hashtableFromJson (stateparam0);
		state1 = MiniJsonExtensions.hashtableFromJson (stateparam1);
		state2 = MiniJsonExtensions.hashtableFromJson (stateparam2);
		state3 = MiniJsonExtensions.hashtableFromJson (stateparam3);
		state4 = MiniJsonExtensions.hashtableFromJson (stateparam4);

		stateparam0 = null;
		stateparam1 = null;
		stateparam2 = null;
		stateparam3 = null;
		stateparam4 = null;
	}
}



