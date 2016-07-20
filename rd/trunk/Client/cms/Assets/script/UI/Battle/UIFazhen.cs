using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFazhen : UIBase
{
	public static string ViewName = "UIFazhen";
	
	public Text leftTimeText;
	public Text errorTip;

	//int maxSecend = 10;
    float leftSecend = 0.0f;

    private GameObject fazhenStyle;

	// Use this for initialization
	void Start()
	{
		errorTip.text = StaticDataMgr.Instance.GetTextByID ("dazhao_huazhenshibai");
	}

    public override void Init()
    {
        InitFazhen();
        leftSecend = 10.0f;
		//StartCoroutine ("updateLeftTimeCo");
		ShowErrorTip (false);
    }
    public override void Clean()
    {
        if (fazhenStyle != null)
        {
            ResourceMgr.Instance.DestroyAsset(fazhenStyle);
        }
    }

	void OnDestroy()
	{
		//StopUpdateLeftTime ();
	}

    void Update()
    {
        if (leftSecend > -100.0f)
        {
            leftSecend -= Time.unscaledDeltaTime;
            if (leftSecend > 0.0f)
            {
                leftTimeText.text = Mathf.CeilToInt(leftSecend).ToString();   //Math.Ceiling(Convert.ToDecimal(d)).ToString())
            }
            else
            {
                leftSecend = -100.0f;//invalidate time;
                GameEventMgr.Instance.FireEvent<int>(GameEventList.OverMagicShifaWithResult, 0);
            }
        }
    }

	void InitFazhen()
	{
		int fazhenCount = BattleConst.magicFazhencount;
		int index = Random.Range (0, fazhenCount);
		string prefabName = "fazhenStyle_" + index.ToString ();

        if (fazhenStyle != null)
        {
            ResourceMgr.Instance.DestroyAsset(fazhenStyle);
        }
        fazhenStyle = ResourceMgr.Instance.LoadAsset(prefabName);

        fazhenStyle.transform.SetParent(this.transform, false);

        RectTransform rectTrans = fazhenStyle.transform as RectTransform;
		Vector3 stylePos = rectTrans.anchoredPosition3D;
		stylePos.z = -10;
		rectTrans.anchoredPosition3D = stylePos;
	}

    //IEnumerator updateLeftTimeCo()
    //{
    //    float waitTime = 1.0f;
    //    if (Time.timeScale > 0)
    //    {
    //        waitTime = 1.0f * Time.timeScale;
    //    }
    //    while(leftSecend > 0)
    //    {
    //        leftTimeText.text = leftSecend.ToString();
    //        leftSecend--;
    //        yield return new WaitForSeconds(waitTime);
    //    }
    //    GameEventMgr.Instance.FireEvent<int>(GameEventList.OverMagicShifaWithResult,0);
    //}

	public void ShifaSucc()
	{
		//StopUpdateLeftTime ();
		GameEventMgr.Instance.FireEvent<int>(GameEventList.OverMagicShifaWithResult,1);
	}

    //void StopUpdateLeftTime()
    //{
    //    StopCoroutine ("updateLeftTimeCo");
    //}

	public void ShowErrorTip(bool bshow)
	{
		errorTip.gameObject.SetActive (bshow);
	}
}
