using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFazhen : UIBase
{
	public static string ViewName = "UIFazhen";
	public static string AssertName = "ui/fazhen";
	
	public Text leftTimeText;
	public Text errorTip;

	int maxSecend = 10;
	int leftSecend = 0;

	// Use this for initialization
	void Start()
	{
		InitFazhen ();
		leftSecend = maxSecend;
		StartCoroutine (updateLeftTimeCo ());
		ShowErrorTip (false);
	}

	void OnDestroy()
	{
		StopUpdateLeftTime ();
	}

	void InitFazhen()
	{
		int fazhenCount = BattleConst.magicFazhencount;
		int index = Random.Range (0, fazhenCount);
		string prefabName = "fazhenStyle_" + index.ToString ();

		GameObject prefab = ResourceMgr.Instance.LoadAsset ("battle/fazhen", prefabName) as GameObject;
		GameObject fazhenGo = Instantiate (prefab) as GameObject;

		fazhenGo.transform.SetParent (this.transform, false);

		RectTransform rectTrans = fazhenGo.transform as RectTransform;
		Vector3 stylePos = rectTrans.anchoredPosition3D;
		stylePos.z = -10;
		rectTrans.anchoredPosition3D = stylePos;
	}

	IEnumerator updateLeftTimeCo()
	{
		float waitTime = 1.0f;
		if (Time.timeScale > 0)
		{
			waitTime = 1.0f * Time.timeScale;
		}
		while(leftSecend > 0)
		{
			leftTimeText.text = leftSecend.ToString();
			leftSecend--;
			yield return new WaitForSeconds(waitTime);
		}
		GameEventMgr.Instance.FireEvent<int>(GameEventList.OverMagicShifaWithResult,0);
	}

	public void ShifaSucc()
	{
		StopUpdateLeftTime ();
		GameEventMgr.Instance.FireEvent<int>(GameEventList.OverMagicShifaWithResult,1);
	}

	void StopUpdateLeftTime()
	{
		StopCoroutine (updateLeftTimeCo ());
	}

	public void ShowErrorTip(bool bshow)
	{
		errorTip.gameObject.SetActive (bshow);
	}
}
