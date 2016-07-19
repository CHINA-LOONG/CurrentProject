using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILoginDetail : UIBase {
    public static string ViewName = "UILoginDetail";
    public Button button;
    public Button clearButton;
    public InputField playerIDFileld;
    public InputField nicknameField;

	// Use this for initialization
	void Start () {
        EventTriggerListener.Get(button.gameObject).onClick = OnButtonClick;
        EventTriggerListener.Get(clearButton.gameObject).onClick = OnClearButtonClick;

        if (playerIDFileld != null && string.IsNullOrEmpty(PlayerPrefs.GetString("testGuid")) == false)
        {
            playerIDFileld.text = PlayerPrefs.GetString("testGuid");
        }
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnButtonClick(GameObject go)
    {

        if (string.IsNullOrEmpty(playerIDFileld.text) == true)
        {
            Debug.Log("请输入用户名");
            return;
        }

        GameDataMgr.Instance.UserDataAttr.guid = playerIDFileld.text; 

        if (string.IsNullOrEmpty(nicknameField.text) == true)
        {
            PB.HSLogin hsLogin = new PB.HSLogin();
            hsLogin.puid = playerIDFileld.text;

            hsLogin.token = GameDataMgr.Instance.UserDataAttr.token;
            GameApp.Instance.netManager.SendMessage(PB.code.LOGIN_C.GetHashCode(), hsLogin);
        }
        else
        {
            GameEventMgr.Instance.FireEvent<string>(GameEventList.createPlayerClick, nicknameField.text);
        }
        
    }

    void OnClearButtonClick(GameObject go)
    {
        playerIDFileld.text = "";
    }
}
