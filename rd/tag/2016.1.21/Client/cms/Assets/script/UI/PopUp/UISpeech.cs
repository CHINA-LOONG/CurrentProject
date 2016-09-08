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
            Logger.Log("缺少本地数据配置…………");
            return;
        }
        UISpeech speech = UIMgr.Instance.OpenUI_(UISpeech.ViewName) as UISpeech;
        speech.info = info;
        speech.endEvent = callBack;
        speech.ShowWithData(info);
    }


    public Button btnSkip;
    public Button btnNext;
    public Image imgNextTip;

    public GameObject panelCampA;
    public GameObject panelCampB;
    public Image imgCampA;
    public Image imgCampB;
    public GameObject nameCampA;
    public GameObject nameCampB;
    public Text textCampA;
    public Text textCampB;

    public Text textContent;


    private SpeechData info;
    private System.Action<float> endEvent;
    private int index = 0;

    private Image imgCurrent;
    public GameObject nameCurrent;
    private Text textCurrent;
    private string camp="";
    public string Camp
    {
        get { return info.speechList[index].campType; }
        set 
        {
            if (value!=camp)
            {
                SetCamp(value);
                camp = value;
            }
        }
    }
    void SetCamp(string camp)
    {

        if (camp=="a")
        {
            panelCampA.gameObject.SetActive(true);
            panelCampB.gameObject.SetActive(false);
            nameCurrent = nameCampA;
            imgCurrent = imgCampA;
            textCurrent = textCampA;
        }
        else if(camp=="b")
        {
            panelCampB.gameObject.SetActive(true);
            panelCampA.gameObject.SetActive(false);

            nameCurrent = nameCampB;
            imgCurrent = imgCampB;
            textCurrent = textCampB;
        }
        else
        {
            Logger.Log("error： 配置表阵营填写错误！！");
        }
    }


    void Start()
    {
        EventTriggerListener.Get(btnSkip.gameObject).onClick = OnClickSkip;
        EventTriggerListener.Get(btnNext.gameObject).onClick = OnClickNext;
    }

    public void ShowWithData(SpeechData info)
    {
        if (info.skip != "1")
            btnSkip.gameObject.SetActive(false);
        else
            btnSkip.gameObject.SetActive(true);
        index = 0;
        imgNextTip.gameObject.SetActive(true);
        SetSpeech(index);
    }

    void SetSpeech(int index)
    {
        if (index >= info.speechList.Count) { EndOfSpeech(); return; }
        if (index == (info.speechList.Count - 1)) { imgNextTip.gameObject.SetActive(false); }
        SpeechStaticData data = info.speechList[index];
        Camp = data.campType;

        imgCurrent.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(data.image);
        nameCurrent.SetActive(!string.IsNullOrEmpty(data.name));

        textCurrent.text = StaticDataMgr.Instance.GetTextByID(data.name);
        textContent.text = StaticDataMgr.Instance.GetTextByID(data.speakId);
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
