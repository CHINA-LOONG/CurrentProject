using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MirrorFindWpInfo : MonoBehaviour
{
	public Text wpInfoText;
	public Image wpInfoImage;
	public Image fiexdLine;
	public Image rotationLine;

	public float propertyWidth = 50;

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
		fiexdLine.enabled = false;
		rotationLine.enabled = false;
	}

	public	void Show(MirrorTarget wpTarget,int offsetX, int offsetY)
	{
		Vector2 wpPostion =  BattleCamera.Instance.CameraAttr.WorldToScreenPoint (wpTarget.gameObject.transform.position);
		wpPostion.x = wpPostion.x / UIMgr.Instance.CanvasAttr.scaleFactor ;
		wpPostion.y = wpPostion.y / UIMgr.Instance.CanvasAttr.scaleFactor ; 

		Vector2 tipPostion = new Vector2 (wpPostion.x + offsetX, wpPostion.y + offsetY);

		rectTrans.anchoredPosition = tipPostion;
		wpInfoText.text = wpTarget.WeakPointIDAttr;

		int property = WeakPointController.Instance.GetProperty(wpTarget);
		var image = ResourceMgr.Instance.LoadAssetType<Sprite>("ui/property", "property_" + property) as Sprite;
		if(image != null)
		{
			wpInfoImage.enabled = true;
			wpInfoImage.sprite = image;
		}

		fiexdLine.enabled = true;
		rotationLine.enabled = true;

		Vector2 fiexdLinePos = Vector2.zero;
		Vector2 rotationLinePos = Vector2.zero;
		fiexdLinePos.y = wpInfoImage.rectTransform.anchoredPosition.y;
		rotationLinePos.y = fiexdLinePos.y;
		if (offsetX > 0) 
		{
			fiexdLinePos.x = wpInfoImage.rectTransform.anchoredPosition.x - propertyWidth/2 - fiexdLine.rectTransform.sizeDelta.x ;
			rotationLinePos.x = fiexdLinePos.x;
		}
		else
		{
			fiexdLinePos.x = wpInfoImage.rectTransform.anchoredPosition.x + propertyWidth/2;
			rotationLinePos.x = fiexdLinePos.x + fiexdLine.rectTransform.sizeDelta.x;
		}
		fiexdLine.rectTransform.anchoredPosition = fiexdLinePos;
		rotationLine.rectTransform.anchoredPosition = rotationLinePos;

		{
			Vector2 lineInspce =  new Vector2(rotationLine.rectTransform.anchoredPosition.x, rotationLine.rectTransform.anchoredPosition.y);	
			lineInspce.x =  lineInspce.x  + rectTrans.anchoredPosition.x - rectTrans.sizeDelta.x/2;
			lineInspce.y = lineInspce.y + rectTrans.anchoredPosition.y - rectTrans.sizeDelta.y/2;
			//Vector2 lineInspce = UIUtil.GetSpacePos(rotationLine.rectTransform,UIMgr.Instance.CanvasAttr,UICamera.Instance.CameraAttr);
			float distance = Vector2.Distance (wpPostion, lineInspce) - 30;
			if(distance < 0)
			{
				distance = 0;
			}
			Vector2 tempDelta = rotationLine.rectTransform.sizeDelta;
			tempDelta.x= distance;
			rotationLine.rectTransform.sizeDelta = tempDelta;

			Vector2 vv = (wpPostion - lineInspce);
			float cosAngle = Vector2.Dot ( new Vector2(0,1) , vv.normalized );
			float angle = Mathf.Acos ( cosAngle) * Mathf.Rad2Deg ;

			if(vv.x  < 0)
			{
				angle *= -1;
			}
			else
			{
				angle += 180;
			}

			rotationLine.rectTransform.localEulerAngles = new Vector3(0,0,  angle );
			//rotationLine.rectTransform.look
		}


	}
}
