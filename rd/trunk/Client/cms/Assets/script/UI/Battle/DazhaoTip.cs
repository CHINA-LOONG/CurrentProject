using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DazhaoTip : MonoBehaviour 
{
	public	Text lbDazhao;
	public	Text lbClick;
	public	Text lbShifang;

	public Text timeText;
	public Text attackText;
    public Image daojishiImg;
	private bool isShow = false;
    private int timeTotal;
    private float showTime = 0.0f;
	// Use this for initialization
	public void SetTipInfo(int timeLeft)
	{
		timeText.text = timeLeft.ToString ();
		//attackText.text = useAttack.ToString () + "/" + allAttack.ToString ();
	}

    void Update()
    {
        if (isShow == true)
        {
            float leftTime = timeTotal - Time.unscaledTime + showTime;
            if (leftTime < 0.0f)
                leftTime = 0.0f;

            if (leftTime + 1 > timeTotal)
                timeText.text = (timeTotal).ToString();
            else if (leftTime > 0.0f)
                timeText.text = ((int)(leftTime + 1)).ToString();
            else
                timeText.text = "0";
            
            daojishiImg.fillAmount = leftTime / timeTotal;
        }
    }

    public void SetTotalTime(int totalTime)
    {
        timeTotal = totalTime;
    }
	public void Show()
	{
		isShow = true;
        showTime = Time.unscaledTime;
		gameObject.SetActive (true);
		lbDazhao.text = StaticDataMgr.Instance.GetTextByID ("dazhao_jieduan");
		lbClick.text = StaticDataMgr.Instance.GetTextByID ("dazhao_click");
		lbShifang.text = StaticDataMgr.Instance.GetTextByID ("dazhao_shifang");
	}

	public void Hide()
	{
		isShow = false;
		gameObject.SetActive (false);
	}
}
