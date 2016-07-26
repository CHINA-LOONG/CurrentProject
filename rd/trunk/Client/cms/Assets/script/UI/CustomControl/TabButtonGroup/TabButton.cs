using UnityEngine;
using System.Collections;
[RequireComponent(typeof(HomeButton))]
public class TabButton : MonoBehaviour 
{
	[HideInInspector]
	public TabButtonGroup   group = null;

	[HideInInspector]
	public	int	index = 0;
	HomeButton homeButton;
	// Use this for initialization

    private Animator anim;

	void Awake()
	{
		homeButton = GetComponent<HomeButton> ();
		homeButton.onClick = OnTabButtonClicked;
	}

	public void SetIsOn(bool isOn)
	{
		if(homeButton != null)
			homeButton.IsOn = isOn;
        if (anim==null)
        {
            anim = GetComponent<Animator>();
        }
        if (anim != null)
        {
            anim.SetTrigger(isOn?"select":"normal");
        }

	}
	
	public	void	OnTabButtonClicked(GameObject go)
	{
		if (!homeButton.IsOn) 
		{
			homeButton.IsOn = true;
			return;
		}
		if (group != null)
		{
			group.OnChangeItem(index);
		}
	}
}
