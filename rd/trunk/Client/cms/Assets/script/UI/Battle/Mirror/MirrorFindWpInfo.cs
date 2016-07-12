using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MirrorFindWpInfo : MonoBehaviour
{
	public Text wpInfoText;
	public Image wpInfoImage;

	RectTransform rectTrans;
	// Use this for initialization
	void Start () 
	{
		rectTrans = transform as RectTransform;
	}
	
	public void Clear()
	{
		wpInfoText.text = "";
		wpInfoImage.enabled = false;
	}

	public	void Show(MirrorTarget wpTarget,int offsetX, int offsetY)
	{
		WeakPointRuntimeData wpRuntime = wpTarget.WpRuntimeData;
		if (null == wpRuntime)
			return;

		Vector2 wpPostion =  BattleCamera.Instance.CameraAttr.WorldToScreenPoint (wpTarget.gameObject.transform.position);
		wpPostion.x = wpPostion.x / UIMgr.Instance.CanvasAttr.scaleFactor ;
		wpPostion.y = wpPostion.y / UIMgr.Instance.CanvasAttr.scaleFactor ; 

		Vector2 tipPostion = new Vector2 (wpPostion.x + offsetX, wpPostion.y + offsetY);

		rectTrans.anchoredPosition = tipPostion;
		//wpInfoText.text = wpTarget.WeakPointIDAttr;

		int property = wpRuntime.property;
		var image = ResourceMgr.Instance.LoadAssetType<Sprite>("property_" + property) as Sprite;
		if(image != null)
		{
			wpInfoImage.enabled = true;
			wpInfoImage.sprite = image;
		}

		wpInfoText.text = wpRuntime.staticData.DescAttr;
	}
}
