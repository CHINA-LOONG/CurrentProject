using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowButton : MonoBehaviour
{
	public	Image	enableImage;
	public	Image	disableImage;

	private	bool	isEnable = true;
	public	bool	IsEnable
	{
		get{ return	isEnable;}
		set
		{
			isEnable = value;

			enableImage.gameObject.SetActive(isEnable);
			disableImage.gameObject.SetActive(!isEnable);
		}
	}	
}
