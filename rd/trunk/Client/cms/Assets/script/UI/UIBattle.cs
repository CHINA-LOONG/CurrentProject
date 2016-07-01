using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBattle : UIBase
{
	public static	string	ViewName = "UIBattle";

	public Image  m_MirrorImage = null;
	private	MirrorDray	m_MirrorDray = null;
	// Use this for initialization
	void Start () 
	{
		if (null != m_MirrorImage) 
		{
			m_MirrorDray = m_MirrorImage.gameObject.AddComponent<MirrorDray> ();
		}
		else
		{
			Debug.LogError("You Should set MirrorImage in the UIBattle prefab!");
		}

	}

}
