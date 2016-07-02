using UnityEngine;
using System.Collections;

public class MirrorTarget : MonoBehaviour 
{
	[SerializeField]
	string	m_monsterWeakPointID;

	public	string	WeakPointIDAttr
	{
		set
		{
			m_monsterWeakPointID = value;
		}
		get
		{
			return m_monsterWeakPointID;
		}
	}

	public float DistanceToMirror {
		set;
		get;
	}

	// Use this for initialization
	void Start () 
	{
	  
	}
}
