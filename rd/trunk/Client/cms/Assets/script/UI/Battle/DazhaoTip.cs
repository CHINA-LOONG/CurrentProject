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
	private bool isShow = false;
	// Use this for initialization
	public void SetTipInfo(int timeLeft,int useAttack,int allAttack)
	{
		timeText.text = timeLeft.ToString ();
		//attackText.text = useAttack.ToString () + "/" + allAttack.ToString ();
	}

	public void Show()
	{
		isShow = true;
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
	public bool IsShow()
	{
		return  isShow;
	}
}
