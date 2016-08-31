using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UICreatePlayer : UIBase
{
	public static	string	ViewName = "UICreatePlayer";

	public Button 		createButton;
	public InputField   nameInputFileld;

	// Use this for initialization
	void Start ()
	{
		EventTriggerListener.Get (createButton.gameObject).onClick = OnCreateButtonClick;
	}

	void OnCreateButtonClick(GameObject go)
	{
        if (string.IsNullOrEmpty(nameInputFileld.text))
        {
            Logger.LogError("input name is empty!");
            return;
        }
        GameEventMgr.Instance.FireEvent<string>(GameEventList.createPlayerClick, nameInputFileld.text);
	}

}
