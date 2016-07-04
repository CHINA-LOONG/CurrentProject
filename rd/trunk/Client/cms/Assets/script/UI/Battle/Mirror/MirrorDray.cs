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

	Dictionary<MirrorTarget, float> lastFindWeakpoint = new Dictionary<MirrorTarget, float> ();

	MirrorRaycast m_MirrorRaycast;
	List<MirrorTarget> newFindTargetList =new List<MirrorTarget>();
	List<MirrorTarget> finishFindTargett = new List<MirrorTarget>();
	List<MirrorTarget> outFindTarget = new List<MirrorTarget> ();

	bool isDragging = false;

	public void Init()
	{
		m_MirrorRaycast = gameObject.AddComponent<MirrorRaycast> ();
	}

	void Awake ()
	{  
		RectTransform parentTransform = transform.parent as RectTransform;
		float rootWidth =  parentTransform.rect.width;
		float rootHeight =  parentTransform.rect.height;
		float screenWith = Screen.width;
		float screenHeight = Screen.height;

		CanvasScaler cs =  UIMgr.Instance.GetComponent<CanvasScaler> ();
		float uiScaleFactor = cs.scaleFactor;

		RectTransform thisTransform = transform as RectTransform;
		float myWith = thisTransform.rect.width;
		float myHeigth = thisTransform.rect.height;

		m_MinPosX =  myWith / 2.0f;
		m_MaxPosX = rootWidth  - myWith / 2.0f;
		m_MinPosY = myHeigth / 2.0f;
		m_MaxposY = rootHeight  - myHeigth / 2.0f;
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
		transform.position = GetNewPosition (Input.mousePosition);
		//RectTransform rt = transform as RectTransform;
		//rt.anchoredPosition = GetNewPosition (Input.mousePosition);
		//rt.position = GetNewPosition (Input.mousePosition);
	}  
	//点击
	public void OnPointerClick (PointerEventData eventData)
	{
		if (!isDragging) 
		{
			GameEventMgr.Instance.FireEvent<Vector3>(GameEventList.MirrorClicked,Input.mousePosition);
		}
	}

	Vector3 GetNewPosition(Vector3 mousePosition)
	{
		Vector3 newPos = new Vector3 (mousePosition.x, mousePosition.y, mousePosition.z);
		if (newPos.x < m_MinPosX) {
			//newPos.x = m_MinPosX;
		}
		if (newPos.x > m_MaxPosX) {
			//newPos.x = m_MaxPosX;
		}
		if (newPos.y < m_MinPosY) {
			//newPos.y = m_MinPosY;
		}
		if (newPos.y > m_MaxposY) {
			//newPos.y = m_MaxposY;
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
	}

	IEnumerator weakPointRayCastCo()
	{
		List<MirrorTarget> listFindTarget = null;
		float findTimeCount = 0f;
		while (true)
		{
			newFindTargetList.Clear();
			outFindTarget.Clear();
			finishFindTargett.Clear();

			listFindTarget = m_MirrorRaycast.WeakpointRayCast (new Vector2(transform.position.x + GameConfig.Instance.MirrorCenterOffset.x, transform.position.y + GameConfig.Instance.MirrorCenterOffset.y));
			if(listFindTarget.Count > 0)
			{
				MirrorTarget subTarget = null;
				for(int i =0 ; i< listFindTarget.Count; ++i)
				{
					subTarget = listFindTarget[i];
					if(lastFindWeakpoint.ContainsKey(subTarget))
					{
						lastFindWeakpoint[subTarget] += Time.deltaTime;
						if(findTimeCount > GameConfig.Instance.FindWeakPointFinishedNeedTime)
						{
							finishFindTargett.Add(subTarget);
						}
					}
					else
					{
						lastFindWeakpoint.Add(subTarget,0.0f);
						newFindTargetList.Add(subTarget);

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
