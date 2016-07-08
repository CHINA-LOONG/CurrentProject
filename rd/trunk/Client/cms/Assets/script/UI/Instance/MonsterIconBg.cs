using UnityEngine;
using System.Collections;

public class MonsterIconBg : MonoBehaviour
{
	public	GameObject focusEffect;
	private	Transform	iconTranform;

	// Use this for initialization
	void Start () 
	{
	
	}

	public	void	AddIcon(Transform iconGo)
	{
	}

	public	void	ClearIcon()
	{
		if (null != iconTranform)
		{
		}
	}

	public	void	SetEffectShow(bool bshow)
	{
		focusEffect.SetActive (bshow);
	}
}
