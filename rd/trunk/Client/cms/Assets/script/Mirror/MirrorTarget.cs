using UnityEngine;
using System.Collections;

public class MirrorTarget : MonoBehaviour 
{
	[SerializeField]
	string	m_monsterPoorID;
	public	string	MonsterPoorIDAttr
	{
		set
		{
			m_monsterPoorID = value;
		}
		get
		{
			return m_monsterPoorID;
		}
	}
	// Use this for initialization
	void Start () 
	{
	
	}
}
