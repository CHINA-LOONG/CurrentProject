using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  
using System.Collections;
using System.Collections.Generic;

public class FazhenStyle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler
{
	public RectTransform	touchParticleTrans;
	public List<Transform> touchList = new List<Transform> ();
	private List<Transform> userTouchedList = new List<Transform>();

	private Vector3 lastTouchPosition;

	UIFazhen fazhen;
	// Use this for initialization
	void Start () 
	{
		userTouchedList.Clear ();
		touchParticleTrans.gameObject.SetActive (false);
		fazhen = UIBattle.Instance.uiFazhen;

		Transform subTrans = null;
		for (int i =0; i< touchList.Count; ++i)
		{
			subTrans = touchList[i];

			//FazhenTouchObj touchObj = subTrans.gameObject.AddComponent<FazhenTouchObj>();
			//touchObj.SetFazhenStyle(this);

			Image touchImg = subTrans.GetComponent<Image>();
			Color a = Color.red;
			a.a = 0;
			touchImg.color = a;
		}
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		lastTouchPosition = Input.mousePosition;
		userTouchedList.Clear ();
		fazhen.ShowErrorTip (false);
		ResetParticlePosition (Input.mousePosition);
		touchParticleTrans.gameObject.SetActive (true);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		TouchItemIntersect (lastTouchPosition, Input.mousePosition);
		bool isSucc = CheckTouch ();
		userTouchedList.Clear ();

		if (isSucc)
		{
			fazhen.ShifaSucc();
		}
		else 
		{
			fazhen.ShowErrorTip(true);
		}
		touchParticleTrans.gameObject.SetActive (false);
	}

	public void OnDrag (PointerEventData eventData)
	{
		ResetParticlePosition (Input.mousePosition);
	}

	public	void AddTouchedTransform(Transform touchedItem)
	{
		userTouchedList.Add (touchedItem);
	}

	bool	CheckTouch()
	{
		int touchCount = userTouchedList.Count;
		if (touchList.Count != touchCount)
			return false;

		Transform subTouch;
		Transform tempTouch;

		for (int i =0; i < touchCount; ++i)
		{
			subTouch = userTouchedList[i];
			tempTouch = touchList[i];
			if(subTouch != tempTouch)
			{
				break;
			}
			if(i == touchCount -1)
			{
				return true;
			}
		}

		for(int i =0; i<touchCount; ++i)
		{
			subTouch = userTouchedList[i];
			tempTouch = touchList[touchCount- i -1];

			if(subTouch != tempTouch)
			{
				return false;
			}
		}

		return true;
	}

	void ResetParticlePosition(Vector3 mousePosition)
	{
		TouchItemIntersect (lastTouchPosition,mousePosition);
		Vector3 particlePos = new Vector3 (0, 0,-200);
		particlePos.x = mousePosition.x / UIMgr.Instance.CanvasAttr.scaleFactor;
		particlePos.y = mousePosition.y / UIMgr.Instance.CanvasAttr.scaleFactor;

		touchParticleTrans.anchoredPosition3D = particlePos;
	}

	void TouchItemIntersect(Vector3 lastMousePosition ,Vector3 mousePosition)
	{
		RectTransform subRt = null;
		Vector3[] corners = new Vector3[]{Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero};
		foreach (Transform touchItem in touchList)
		{
			subRt = touchItem as RectTransform;
			subRt.GetWorldCorners( corners);
			UIUtil.GetSpaceCorners(subRt,UIMgr.Instance.CanvasAttr, corners,UICamera.Instance.CameraAttr);

			if(intersect(lastMousePosition,mousePosition,corners[0],corners[1]) ||
			   intersect(lastMousePosition,mousePosition,corners[2],corners[3]))
			{
				Logger.LogError("touch a item...................");
				if(!userTouchedList.Contains(touchItem))
				{
					userTouchedList.Add(touchItem);
				}
			}
		}
		lastMousePosition = mousePosition;
	}
	
	bool intersect(Vector2 aa, Vector2 bb, Vector2 cc, Vector2 dd)  
	{  
		float delta = determinant(bb.x-aa.x, cc.x-dd.x, bb.y-aa.y, cc.y-dd.y);  
		if ( delta<=(1e-6) && delta>=-(1e-6) )  // delta=0，表示两线段重合或平行  
		{  
			return false;  
		}  
		float namenda = determinant(cc.x-aa.x, cc.x-dd.x, cc.y-aa.y, cc.y-dd.y) / delta;  
		if ( namenda>1 || namenda<0 )  
		{  
			return false;  
		}  
		float miu = determinant(bb.x-aa.x, cc.x-aa.x, bb.y-aa.y, cc.y-aa.y) / delta;  
		if ( miu>1 || miu<0 )  
		{  
			return false;  
		}  
		return true;  
	}


	float determinant(float v1, float v2, float v3, float v4)  // 行列式  
	{  
		return (v1*v3-v2*v4);  
	}  
}
