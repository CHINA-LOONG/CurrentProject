using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MirrorFindWpInfo : MonoBehaviour
{
	public Text wpInfoText;

	RectTransform rectTrans;
	// Use this for initialization
	void Start () 
	{
		rectTrans = transform as RectTransform;
	}
	
	public void Clear()
	{
		wpInfoText.text = "";
	}

	public	void Show(MirrorTarget wpTarget,int offsetX, int offsetY)
	{
		Vector3 tipPos =  BattleCamera.Instance.CameraAttr.WorldToScreenPoint (wpTarget.gameObject.transform.position);
		tipPos.x = tipPos.x / UIMgr.Instance.CanvasAttr.scaleFactor + offsetX;
		tipPos.y = tipPos.y / UIMgr.Instance.CanvasAttr.scaleFactor + offsetY; 

		rectTrans.anchoredPosition = tipPos;

		wpInfoText.text = wpTarget.WeakPointIDAttr;
	}
}
