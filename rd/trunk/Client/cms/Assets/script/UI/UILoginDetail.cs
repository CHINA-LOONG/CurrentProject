using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILoginDetail : UIBase {
    public static string ViewName = "UILoginDetail";
    public Button button;
    public InputField nicknameField;

	// Use this for initialization
	void Start () {
        EventTriggerListener.Get(button.gameObject).onClick = OnButtonClick;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnButtonClick(GameObject go)
    {

        if (string.IsNullOrEmpty(GameDataMgr.Instance.UserDataAttr.guid))
        {
            Debug.Log("重新登陆");
            return;
        }

        if (string.IsNullOrEmpty(nicknameField.text))
        {
            Debug.Log("请输入用户名");
            return;
        }

        GameEventMgr.Instance.FireEvent<string>(GameEventList.createPlayerClick, nicknameField.text);
        
    }
}
