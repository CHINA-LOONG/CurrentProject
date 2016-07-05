using UnityEngine;  
using UnityEngine.UI;  
using UnityEngine.EventSystems;  
using System.Collections;
using System.Collections.Generic;

public class MirrorDray : MonoBehaviour,IPointerDownHandler, IDragHandler,IPointerClickHandler
{

	float	m_MinPosX = 0f;
	float 	m_MaxPosX = 0f;
	float 	m_MinPosY = 0f;
	float	m_MaxposY = 0f;

	RectTransform rectTrans;

	Dictionary<MirrorTarget, float> lastFindWeakpoint = new Dictionary<MirrorTarget, float> ();

	MirrorRaycast m_MirrorRaycast;
	List<MirrorTarget> newFindTargetList =new List<MirrorTarget>();
	List<MirrorTarget> finishFindTargett = new List<MirrorTarget>();
	List<MirrorTarget> outFindTarget = new List<MirrorTarget> ();

	bool isDragging = false;

	public void Init()
	{
		m_MirrorRaycast = gameObject.AddComponent<MirrorRaycast> ();
		rectTrans = transform as RectTransform;
	}

	void Awake ()
	{  
		RectTransform parentTransform = transform.parent as RectTransform;
		float rootWidth = Screen.width /UIMgr.Instance.CanvasAttr.scaleFactor ;
		float rootHeight =   Screen.height/UIMgr.Instance.CanvasAttr.scaleFactor;
		
		//Debug.LogError ("Screen.Width = " + Screen.width + "  Screen.height = " + Screen.height + "scaleFactor = " + UIMgr.Instance.CanvasAttr.scaleFactor );
		RectTransform thisTransform = transform as RectTransform;
		float myWith = thisTransform.rect.width;
		float myHeigth = thisTransform.rect.height;

		Vector2 pivot = thisTransform.pivot;
		m_MinPosX = myWith * pivot.x;
		m_MaxPosX = rootWidth - myWith*(1 - pivot.x)  ;
		m_MinPosY = myWith * pivot.y;
		m_MaxposY = rootHeight - myHeigth * (1-pivot.y);
	}  

	// 鼠标按下  
	public void OnPointerDown (PointerEventData data) 
	{  
		isDragging = false;
	}  
	// 拖动  
	public void OnDrag (PointerEventData data)
	{  
		isDragging = true;
		
		rectTrans.anchoredPosition = GetMirrorScreenPosition (Input.mousePosition);	
	}  
	//点击
	public void OnPointerClick (PointerEventData eventData)
	{
		if (!isDragging) 
		{
			GameEventMgr.Instance.FireEvent<Vector3>(GameEventList.MirrorClicked,Input.mousePosition);
		}
	}

	Vector2 GetMirrorScreenPosition(Vector3 mousePosition)
	{
		Vector2 newPos = new Vector2 (mousePosition.x / UIMgr.Instance.CanvasAttr.scaleFactor , mousePosition.y / UIMgr.Instance.CanvasAttr.scaleFactor);

		if (newPos.x < m_MinPosX) {
			newPos.x = m_MinPosX;
		}
		if (newPos.x > m_MaxPosX) {
			newPos.x = m_MaxPosX;
		}
		if (newPos.y < m_MinPosY) {
			newPos.y = m_MinPosY;
		}
		if (newPos.y > m_MaxposY) {
			newPos.y = m_MaxposY;
		}
		return newPos;
	}

	public void	OnSetMirrorModeState(bool isMirror)
	{
		if (isMirror) 
		{
			StartRayCast();
		} 
		else 
		{
			StopRayCast();
		}
	}

	void StartRayCast()
	{
		lastFindWeakpoint.Clear ();


		StartCoroutine (weakPointRayCastCo());
	}

	void StopRayCast()
	{
		StopCoroutine(weakPointRayCastCo());

		List<MirrorTarget> listTarget = new List<MirrorTarget> (lastFindWeakpoint.Keys);
		if (null !=listTarget && listTarget.Count > 0) 
		{
			GameEventMgr.Instance.FireEvent<List<MirrorTarget>>(GameEventList.MirrorOutWeakPoint,listTarget);
		}
		GameEventMgr.Instance.FireEvent(GameEventList.HideFindMonsterInfo);
	}

	IEnumerator weakPointRayCastCo()
	{
		List<MirrorTarget> listFindTarget = null;
		while (true)
		{
			newFindTargetList.Clear();
			outFindTarget.Clear();
			finishFindTargett.Clear();

			Vector3 mirrorScreenPos = UIUtil.GetSpacePos(transform as RectTransform,UIMgr.Instance.CanvasAttr,UICamera.Instance.CameraAttr);
			mirrorScreenPos.x += GameConfig.Instance.MirrorCenterOffset.x;
			mirrorScreenPos.y += GameConfig.Instance.MirrorCenterOffset.y;
			listFindTarget = m_MirrorRaycast.WeakpointRayCast (mirrorScreenPos);
			if(listFindTarget.Count > 0)
			{
				MirrorTarget subTarget = null;
				for(int i =0 ; i< listFindTarget.Count; ++i)
				{
					subTarget = listFindTarget[i];
					var battleObject = subTarget.GetComponentInParent<BattleObject> () as BattleObject;


					if(lastFindWeakpoint.ContainsKey(subTarget))
					{
						lastFindWeakpoint[subTarget] += Time.deltaTime;
						if(lastFindWeakpoint[subTarget] > GameConfig.Instance.FindWeakPointFinishedNeedTime)
						{
							finishFindTargett.Add(subTarget);
						}
					}
					else
					{
						if(subTarget.isSelf && battleObject.unit.isVisible)
						{
							finishFindTargett.Add(subTarget);
							lastFindWeakpoint.Add(subTarget,0.0f);
						}
						else
						{
							lastFindWeakpoint.Add(subTarget,0.0f);
							newFindTargetList.Add(subTarget);
						}

					}
				}
			}
			if(newFindTargetList.Count > 0)
			{
				GameEventMgr.Instance.FireEvent<List<MirrorTarget>>(GameEventList.FindWeakPoint,newFindTargetList);
			}
			if(finishFindTargett.Count > 0)
			{
				GameEventMgr.Instance.FireEvent<List<MirrorTarget>>(GameEventList.FindFinishedWeakPoint,finishFindTargett);
				GameEventMgr.Instance.FireEvent<List<MirrorTarget>>(GameEventList.ShowFindMonsterInfo, finishFindTargett);
			}
			else
			{
				GameEventMgr.Instance.FireEvent(GameEventList.HideFindMonsterInfo);
			}

			List<MirrorTarget> lastFindKeys =  new List<MirrorTarget>( lastFindWeakpoint.Keys);
			if(null!=lastFindKeys && lastFindKeys.Count > 0)
			{
				for(int i =0 ;i < lastFindKeys.Count;++i)
				{
					if(!listFindTarget.Contains(lastFindKeys[i]))
					{
						outFindTarget.Add(lastFindKeys[i]);
						lastFindWeakpoint.Remove(lastFindKeys[i]);
					}
				}
			}
			if(outFindTarget.Count> 0)
			{
				GameEventMgr.Instance.FireEvent<List<MirrorTarget>>(GameEventList.MirrorOutWeakPoint,outFindTarget);
			}

			//Debug.LogError("finding....."+ transform.position.x);
			yield return new WaitForSeconds(0.05f);
		}
	}

}
