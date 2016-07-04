using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireFocusEffect : MonoBehaviour
{
	RectTransform rectTrans = null;
	GameObject effectGo;

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

		float offsetH = 0.5f;
        if (bo.unit.isBoss) 
		{
			offsetH = 1.1f;
		}

        Vector3 unitPosition = bo.gameObject.transform.position + new Vector3(0, offsetH, 0);
		var screenPos = BattleCamera.Instance.CameraAttr.WorldToScreenPoint (unitPosition);
		rectTrans.anchoredPosition = new Vector2 (screenPos.x/UIMgr.Instance.CanvasAttr.scaleFactor, screenPos.y/UIMgr.Instance.CanvasAttr.scaleFactor);
	}

	void OnHide()
	{
		effectGo.SetActive (false);
	}
}
