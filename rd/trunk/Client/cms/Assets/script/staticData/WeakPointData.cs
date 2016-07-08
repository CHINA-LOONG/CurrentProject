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

	//鉴定效果
	public string findWpEffect;
	public string appraisalStateEffect;

	//---------------对原始资源的处理，方便使用------------
	public string findWpEffectAsset;
	public string findWpEffectPrefab;

	public string appraisalStateEffectAsset;
	public string appraisalStateEffectPrefab;
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

		if (!string.IsNullOrEmpty (findWpEffect)) 
		{
			int index = findWpEffect.LastIndexOf('/');
			findWpEffectAsset = findWpEffect.Substring(0,index);
			findWpEffectPrefab = findWpEffect.Substring(index +1,findWpEffect.Length - index -1);
		}

		if (!string.IsNullOrEmpty (appraisalStateEffect)) 
		{
			int index = findWpEffect.LastIndexOf('/');
			appraisalStateEffectAsset = appraisalStateEffect.Substring(0,index);
			appraisalStateEffectPrefab = appraisalStateEffect.Substring(index +1,appraisalStateEffect.Length - index -1);
		}
	}
}



