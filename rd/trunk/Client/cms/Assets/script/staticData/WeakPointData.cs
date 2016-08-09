using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WeakPointData
{
    public string id;
    public string name;
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

	public float damageRate1;//伤害系数
	public float damageRate2;//伤害系数

	public int property1;
	public int property2;

    public int stat1WpType;
    public int state2WpType;

    public string stateparam0;
    public string state0Tips;
	public string stateparam1;
    public string state1Tips;
	public string stateparam2;
    public string state2Tips;

	public string stateparam3;
	public string stateparam4;
    public string state4Tips;

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



