using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CoinButton : MonoBehaviour
{
	// Use this for initialization
	void Start () 
	{
	
	}

	public	Image	coinImage = null;
	public	Text	coinCount = null;
	public	Image	plusImage = null;
	public	Button	addCoinButton;

	public	void	HideAddCoinButton()
	{
		plusImage.gameObject.SetActive (false);
		addCoinButton.gameObject.SetActive (false);
	}
}
