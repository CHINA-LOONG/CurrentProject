using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class HomeButton : MonoBehaviour 
{
	public delegate void HomeButtonDelegate (GameObject go);
	public	Image normalImage;
	public	Image checkedImage;
	public	Button button;

	public RectTransform  normalEffectPanel;
	public	RectTransform checkedEffectPanel;

	public HomeButtonDelegate onClick;

	[SerializeField]
	bool	isOn = false;
	public bool IsOn
	{
		set
		{
			isOn = value;
			UpdateButtonGraphic ();
			UpdateEffect ();
		}
		get
		{
			return isOn;
		}
	}

	// Use this for initialization
	void Start ()
	{
	
		EventTriggerListener.Get (button.gameObject).onClick = OnButtonClicked;
		UpdateButtonGraphic ();
		UpdateEffect ();
	}

	void OnButtonClicked(GameObject go)
	{
		isOn = !isOn;
		UpdateButtonGraphic ();
		UpdateEffect ();

		if (null != onClick) 
		{
			onClick(go);
		}
	}

	void UpdateButtonGraphic()
	{
		normalImage.gameObject.SetActive (!isOn);
		checkedImage.gameObject.SetActive (isOn);
		Image selImage = null;
		if (isOn)
		{
			selImage = checkedImage;
		} 
		else
		{
			selImage = normalImage;
		}
		button.targetGraphic = selImage;
	}

	void UpdateEffect()
	{
		if (null != normalEffectPanel) 
		{
			normalEffectPanel.gameObject.SetActive( !isOn);
		}
		if (null != checkedEffectPanel) 
		{
			checkedEffectPanel.gameObject.SetActive(isOn);
		}
	}

}
