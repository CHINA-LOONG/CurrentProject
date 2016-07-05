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
		int fazhenCount = 1;
		int index = Random.Range (0, fazhenCount);
		string prefabName = "fazhenStyle_" + index.ToString ();

		GameObject prefab = ResourceMgr.Instance.LoadAsset ("battle/fazhen", prefabName) as GameObject;
		GameObject fazhenGo = Instantiate (prefab) as GameObject;

		fazhenGo.transform.SetParent (this.transform, false);
	}

	IEnumerator updateLeftTimeCo()
	{
		while(leftSecend > 0)
		{
			leftTimeText.text = leftSecend.ToString();
			leftSecend--;
			yield return new WaitForSeconds(1.0f);
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
