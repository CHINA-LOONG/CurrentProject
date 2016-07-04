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

	/// <summary>
	/// is 本体?
	/// </summary>
	public bool	isSelf = false;

	// Use this for initialization
	void Start () 
	{
	  
	}
}
