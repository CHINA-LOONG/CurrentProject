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
}
