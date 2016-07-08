using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireFocusEffect : MonoBehaviour
{
	RectTransform rectTrans = null;
	GameObject effectGo;
	Transform effectTargetTrans;
	float offsetH = 0.0f;

	void Start()
	{
		rectTrans = transform as RectTransform;
		BindListener ();
		effectGo = Util.FindChildByName (gameObject, "fireFocusImage");
		if (null == effectGo)
		{
			Logger.LogError ("Canpt Find SubGameobject: " + "fireFocusImage");
		} 
		else 
		{
			effectGo.SetActive (false);
		}

	}

	void OnDestroy()
	{
		UnBindListener();
	}

	void BindListener()
	{
        GameEventMgr.Instance.AddListener<BattleObject>(GameEventList.ShowFireFocus, OnShow);
		GameEventMgr.Instance.AddListener(GameEventList.HideFireFocus, OnHide);
	}
	
	void UnBindListener()
	{
        GameEventMgr.Instance.RemoveListener<BattleObject>(GameEventList.ShowFireFocus, OnShow);
		GameEventMgr.Instance.RemoveListener(GameEventList.HideFireFocus, OnHide);
	}


	void OnShow(BattleObject bo)
	{
		effectGo.SetActive (true);
		if (!gameObject.activeInHierarchy)
			return;

		try
		{
			StopCoroutine ("RefreshEffectCo");
		}
		catch
		{
		}

		offsetH = 0.5f;
        if (bo.unit.isBoss) 
		{
			offsetH = 1.1f;
		}

        Transform targetTrans = bo.gameObject.transform;
        if (bo.unit.attackWpName != null && bo.unit.attackWpName.Length > 0)
        {
			WeakPointRuntimeData wpRuntimeData = null;
			WeakPointData wp = null;

			if(bo.wpGroup.allWpDic.TryGetValue(bo.unit.attackWpName,out wpRuntimeData))
			{
				//wp = wpRuntimeData.staticData;
				targetTrans = wpRuntimeData.wpMirrorTarget.gameObject.transform;
				offsetH = 0.0f;

			}
        }

		effectTargetTrans = targetTrans;

		try
		{
			StartCoroutine ("RefreshEffectCo");
		}
		catch
		{
		}

	}

	IEnumerator RefreshEffectCo()
	{
		while (true) 
		{
			if(null != effectTargetTrans)
			{
				Vector3 unitPosition = effectTargetTrans.position + new Vector3 (0, offsetH, 0);
				var screenPos = BattleCamera.Instance.CameraAttr.WorldToScreenPoint (unitPosition);
				rectTrans.anchoredPosition = new Vector2 (screenPos.x / UIMgr.Instance.CanvasAttr.scaleFactor, screenPos.y / UIMgr.Instance.CanvasAttr.scaleFactor);
			}
			yield return new WaitForEndOfFrame();
		}
	}

	void OnHide()
	{
		try
		{
			StopCoroutine ("RefreshEffectCo");
		}
		catch
		{
		}
		effectGo.SetActive (false);
	}
}
