using UnityEngine;
using System.Collections;

public class BattleEffectProxy : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
	
	}

	[SerializeField]
	int m_effectPositionID;
	public	int EffectPositionIDAttr
	{
		get
		{
			return m_effectPositionID;
		}

		set
		{
			m_effectPositionID = value;
		}
	}
}
