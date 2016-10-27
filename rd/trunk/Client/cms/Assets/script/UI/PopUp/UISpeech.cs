using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISpeech : UIBase
{
    public static string ViewName = "UISpeech";
    public static void Open(string speechId, System.Action<float> callBack = null)
    {
        SpeechData info = StaticDataMgr.Instance.GetSpeechData(speechId);
        if (info == null)
        {
            Logger.LogError("立绘对话框， 缺少本地数据配置………… speechId  = " + speechId);
            return;
        }
        UISpeech speech = UIMgr.Instance.OpenUI_(UISpeech.ViewName) as UISpeech;
        speech.ShowWithData(info, callBack);
    }
    
    public Button btnSkip;
    public Button btnNext;

    [System.Serializable]
    public class SpeechCamp
    {
        public Image imgRole;
        public Image imgFace;
        public Text textName;
        public Text textContent;
        public GameObject objNextTips;

        string strRole;
        string strFace;

        public void SetSpeech(SpeechStaticData data,bool isLast)
        {
            if (string.IsNullOrEmpty(strRole)||!strRole.Equals(data.image))
            {
                strRole = data.image;
                imgRole.sprite= ResourceMgr.Instance.LoadAssetType<Sprite>(data.image);
                //imgRole.SetNativeSize();
            }
            if (string.IsNullOrEmpty(strFace) || !strFace.Equals(data.face))
            {
                strFace = data.face;
                imgFace.sprite= ResourceMgr.Instance.LoadAssetType<Sprite>(data.face);
                imgFace.transform.localPosition = data.FacePos;
                //imgFace.SetNativeSize();
            }
            textName.text = StaticDataMgr.Instance.GetTextByID(data.name);
            textContent.text = StaticDataMgr.Instance.GetTextByID(data.speakId);
            objNextTips.SetActive(!isLast);
        }
    }
    
    public Animator animatorA;
    public Animator animatorB;
    public SpeechCamp campA;
    public SpeechCamp campB;
    
    private SpeechData info;
    private System.Action<float> endEvent;
    private int index = 0;
    private string curCamp = "";


    void Start()
    {
        EventTriggerListener.Get(btnSkip.gameObject).onClick = OnClickSkip;
        EventTriggerListener.Get(btnNext.gameObject).onClick = OnClickNext;
    }

    public void ShowWithData(SpeechData info,System.Action<float> callBack)
    {
        this.info = info;
        this.endEvent = callBack;

        if (info.skip != "1")
            btnSkip.gameObject.SetActive(false);
        else
            btnSkip.gameObject.SetActive(true);

        index = 0;
        SetSpeech(index);
    }

    void SetSpeech(int index)
    {
        //对话结束
        if (index >= info.speechList.Count) { EndOfSpeech(); return; }

        SpeechStaticData data = info.speechList[index];

        if (data.campType == "a")
        {
            if (!string.Equals(curCamp,data.campType))
            {
                //TODO:播放动画
                if (index != 0)
                {
                    //animatorB.SetTrigger("exit");
                    //Vector2 temp = (animatorB.transform as RectTransform).anchoredPosition;
                    //temp.x = 1200;
                    //(animatorB.transform as RectTransform).anchoredPosition = temp;
                }
                animatorB.gameObject.SetActive(false);
                animatorA.gameObject.SetActive(true);
                animatorA.SetTrigger("enter");
            }
            campA.SetSpeech(data, index == (info.speechList.Count - 1));
        }
        else if (data.campType == "b")
        {
            if (!string.Equals(curCamp, data.campType))
            {
                //TODO:播放动画
                if (index != 0)
                {
                    //animatorA.SetTrigger("exit");
                    //Vector2 temp = (animatorA.transform as RectTransform).anchoredPosition;
                    //temp.x = 1200;
                    //(animatorA.transform as RectTransform).anchoredPosition = temp;
                }
                animatorA.gameObject.SetActive(false);
                animatorB.gameObject.SetActive(true);
                animatorB.SetTrigger("enter");
            }
            campB.SetSpeech(data, index == (info.speechList.Count - 1));
        }
        else
        {
            Logger.Log("error： 配置表阵营填写错误！！");
        }
        curCamp = data.campType;
    }
    
    void OnClickSkip(GameObject go)
    {
        EndOfSpeech();
    }

    void OnClickNext(GameObject go) 
    {
        SetSpeech(++index);
    }

    void EndOfSpeech()
    {
        if (endEvent!=null)
            endEvent(0.0f);
        UIMgr.Instance.DestroyUI(this);
    }

}
