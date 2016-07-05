using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeakPointData
{
    public string id;
    public int type;
    public byte isDamagePoint;//是否关联boss减血
    public float damageRate;//伤害系数
    public int health;
    public byte isTarget;
    public int property;

    public string node;
    public string asset;
   // public string collider;
    public string mesh;
	public string deadMesh;
	public string deadEffect;
    public int initialStatus;

	public int isSelf;
	public int tipType;	
	public int tipOffsetX;
	public int tipOffsetY;

}

public class WeakPointRuntimeData
{
	public string id;
	public int maxHp;
	public int hp;
}

