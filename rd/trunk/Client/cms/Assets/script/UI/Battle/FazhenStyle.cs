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

	UIFazhen fazhen;
	// Use this for initialization
	void Start () 
	{
		userTouchedList.Clear ();
		touchParticleTrans.gameObject.SetActive (false);
		fazhen = transform.parent.gameObject.GetComponent<UIFazhen>();

		Transform subTrans = null;
		for (int i =0; i< touchList.Count; ++i)
		{
			subTrans = touchList[i];

			FazhenTouchObj touchObj = subTrans.gameObject.AddComponent<FazhenTouchObj>();
			touchObj.SetFazhenStyle(this);

			Image touchImg = touchObj.GetComponent<Image>();
			Color a = Color.red;
			a.a = 0;
			touchImg.color = a;
		}
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		userTouchedList.Clear ();
		fazhen.ShowErrorTip (false);
		ResetParticlePosition (Input.mousePosition);
		touchParticleTrans.gameObject.SetActive (true);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
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
		Vector3 particlePos = new Vector3 (0, 0,-200);
		particlePos.x = mousePosition.x / UIMgr.Instance.CanvasAttr.scaleFactor;
		particlePos.y = mousePosition.y / UIMgr.Instance.CanvasAttr.scaleFactor;

		touchParticleTrans.anchoredPosition3D = particlePos;
	}
}
