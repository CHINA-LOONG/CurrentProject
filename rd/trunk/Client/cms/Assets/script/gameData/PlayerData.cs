using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	[SerializeField]
	int m_Diamond = 0;
	public int Diamond
	{
		get
		{
			return m_Diamond;
		}
		set
		{
			m_Diamond = value;
		}
	}

	[SerializeField]
	int m_Coin = 0;
	public int Coin
	{
		get
		{
			return m_Coin;
		}
		set
		{
			m_Coin = value;
		}
	}
	

	[SerializeField]
	string m_Heroid ;
	public string Heroid
	{
		get
		{
			return m_Heroid;
		}
		set
		{
			m_Heroid = value;
		}
	}

    //主角装备加成
    public int equipHealth;
    public int equipStrength;
    public int equipIntelligence;
    public int equipSpeed;
    public int equipDefense;
    public int equipEndurance;
    public float criticalRatio;
    public float hitRatio;

    public float equipHealthRatio;
    public float equipStrengthRatio;
    public float equipIntelligenceRatio;
    public float equipSpeedRatio;
    public float equipDefenseRatio;
    public float equipEnduranceRatio;
    //五行加成
    public float goldDamageRatio;
    public float woodDamageRatio;
    public float waterDamageRatio;
    public float fireDamageRatio;
    public float earthDamageRatio;
}
