using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class uiHintMsg : MonoBehaviour {
    public GameObject noticeUI;//系统公告
    Lantern noticeMove;
    public GameObject lanternUI;//走马灯
    Lantern lanternMove;//走马灯文本
    List<GameObject> hintMsg = new List<GameObject>();//系统提示
    static uiHintMsg mInst = null;
    public static uiHintMsg Instance
    {
        get
        {
            return mInst;
        }
    }
    public void NoticeAdd(string msg)//系统公告
    {
        noticeMove.AddMsg(msg, (int)PB.ImType.NOTICE);
    }
    public void LanternAdd(string msg)//走马灯
    {
        lanternMove.AddMsg(msg, (int)PB.ImType.LANTERN);
    }
    public void HintShow(string hintText)//系统提示
    {
        GameObject hintBox = ResourceMgr.Instance.LoadAsset("hintMessage");
        hintBox.transform.SetParent(transform);
        hintBox.transform.localPosition = new Vector3(0, 152, 0);
        hintBox.transform.localScale = gameObject.transform.localScale;
        Hint hintComponent = hintBox.GetComponent<Hint>();
        hintComponent.ownedList = hintMsg;
        GameObject hint = hintBox.transform.FindChild("Image").gameObject;
        Text hintBoxText = hint.transform.FindChild("Text").GetComponent<Text>();
        hint.SetActive(false);
        hintBoxText.text = hintText;
        float hintWidth = hintBoxText.preferredWidth + BattleConst.hintImageLength;
        if (hintWidth >= hintBoxText.rectTransform.sizeDelta.x)
        {
            hint.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(hintBoxText.rectTransform.sizeDelta.x + BattleConst.hintImageLength, hintBoxText.preferredHeight);
        }
        else
        {
            hint.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(hintWidth, hintBoxText.preferredHeight);
        }

        hintComponent.showTime = Time.unscaledTime;
        hintComponent.SetFadeTime(hintComponent.stopTime + Time.unscaledTime);
        if (hintMsg.Count == 1)
        {
            Hint lastHit = hintMsg[hintMsg.Count - 1].GetComponent<Hint>();
            lastHit.SetFadeTime(Time.unscaledTime);
            hintComponent.SetFadeTime(hintComponent.stopTime + lastHit.endTime);
            hintComponent.showTime = lastHit.endTime;
        }
        else if (hintMsg.Count > 1)
        {
            Hint lastHit = hintMsg[hintMsg.Count - 1].GetComponent<Hint>();
            lastHit.SetFadeTime(lastHit.fadeTime - lastHit.stopTime);
            //lastHit.SetFadeTime(Time.time);
            hintComponent.SetFadeTime(hintComponent.stopTime + lastHit.endTime);
            hintComponent.showTime = lastHit.endTime;
        }
        hintMsg.Add(hintBox);
    }
	// Use this for initialization
	void Start () {
        mInst = this;
        noticeMove = noticeUI.GetComponent<Lantern>();
        lanternMove = lanternUI.GetComponent<Lantern>();
	}	
	// Update is called once per frame
	void Update () {
        lanternMove.UpdateLantern();
        noticeMove.UpdateLantern();
	}
}
