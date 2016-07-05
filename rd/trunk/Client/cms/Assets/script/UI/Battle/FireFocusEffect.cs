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
		StopCoroutine (RefreshEffectCo ());
		effectGo.SetActive (true);

		offsetH = 0.5f;
        if (bo.unit.isBoss) 
		{
			offsetH = 1.1f;
		}

        Transform targetTrans = bo.gameObject.transform;
        if (bo.unit.attackWpName != null && bo.unit.attackWpName.Length > 0)
        {
            WeakPointData wp = StaticDataMgr.Instance.GetWeakPointData(bo.unit.attackWpName);
            if (wp != null && wp.node != null)
            {
                GameObject targetNode = Util.FindChildByName(bo.gameObject, wp.node);
                if (targetNode != null)
                {
                    targetTrans = targetNode.transform;
                    offsetH = 0.0f;
                }
            }
        }

		effectTargetTrans = targetTrans;

		StartCoroutine (RefreshEffectCo ());
	}

	IEnumerator RefreshEffectCo()
	{
		while (true) 
		{
			Vector3 unitPosition = effectTargetTrans.position + new Vector3 (0, offsetH, 0);
			var screenPos = BattleCamera.Instance.CameraAttr.WorldToScreenPoint (unitPosition);
			rectTrans.anchoredPosition = new Vector2 (screenPos.x / UIMgr.Instance.CanvasAttr.scaleFactor, screenPos.y / UIMgr.Instance.CanvasAttr.scaleFactor);
			yield return new WaitForEndOfFrame();
		}
	}

	void OnHide()
	{
		StopCoroutine (RefreshEffectCo ());
		effectGo.SetActive (false);
	}
}
