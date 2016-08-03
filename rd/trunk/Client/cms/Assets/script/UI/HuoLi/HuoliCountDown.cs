using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HuoliCountDown : MonoBehaviour
{
    public Text timeDescText;
    public Text timeCountDownText;

    private int leftTime = 0;
    bool isShowing = true;
	// Use this for initialization
	void Start ()
    {
        timeDescText.text = StaticDataMgr.Instance.GetTextByID("energy_ hint");
        SetShow(false);
	}
	
	// Update is called once per frame

    public void SetShow(bool isShow)
    {
        if (isShow == isShowing)
            return;
        if(!isShow)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            isShowing = false;
            return;
        }
        if (!GameDataMgr.Instance.HuoliRestoreAtrr.IsRestoring)
        {
            isShowing = false;
            return;
        }

        gameObject.SetActive(true);
        isShowing = true;
        StartCoroutine("TimeCountDownCo");
    }

    void CalcLeftTime()
    {
        PlayerData playerData = GameDataMgr.Instance.PlayerDataAttr;
        int lastRestore = playerData.HuoliBegintimeAttr;
        int curTime = GameTimeMgr.Instance.GetServerTimeStamp();
        int delta = curTime - lastRestore;
        leftTime = 360 - delta % 360 + 2;
    }
    int exitTip = 0;
   IEnumerator TimeCountDownCo()
    {
        exitTip = 0;
        int min = 0;
        int second = 0;
        while(true)
        {
            CalcLeftTime();
            min = leftTime / 60;
            second = leftTime % 60;
            timeCountDownText.text = string.Format("{0:D2}:{1:D2}",min,second);
            yield return new WaitForSeconds(1.0f);
            exitTip++;
            if(exitTip >= 10)
            {
                SetShow(false);
            }
        }
    }
}
