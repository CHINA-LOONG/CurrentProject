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
		GameEventMgr.Instance.AddListener<GameUnit>(GameEventList.ShowFireFocus, OnShow);
		GameEventMgr.Instance.AddListener(GameEventList.HideFireFocus, OnHide);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<GameUnit>(GameEventList.ShowFireFocus, OnShow);
		GameEventMgr.Instance.RemoveListener(GameEventList.HideFireFocus, OnHide);
	}


	void OnShow(GameUnit unit)
	{
		effectGo.SetActive (true);

		float offsetH = 0.5f;
		if (unit.isBoss) 
		{
			offsetH = 1.1f;
		}

		Vector3 unitPosition = unit.gameObject.transform.position + new Vector3 (0, offsetH, 0);
		var screenPos = BattleCamera.Instance.CameraAttr.WorldToScreenPoint (unitPosition);
		rectTrans.anchoredPosition = new Vector2 (screenPos.x/UIMgr.Instance.CanvasAttr.scaleFactor, screenPos.y/UIMgr.Instance.CanvasAttr.scaleFactor);
	}

	void OnHide()
	{
		effectGo.SetActive (false);
	}
}
