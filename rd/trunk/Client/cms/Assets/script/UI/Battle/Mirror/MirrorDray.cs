using UnityEngine;  
using UnityEngine.UI;  
using UnityEngine.EventSystems;  
using System.Collections;

public class MirrorDray : MonoBehaviour,IPointerDownHandler, IDragHandler
{

	float	m_MinPosX = 0f;
	float 	m_MaxPosX = 0f;
	float 	m_MinPosY = 0f;
	float	m_MaxposY = 0f;
	
	MirrorTarget m_curFindTarget;

	MirrorRaycast m_MirrorRaycast;

	public void Init()
	{
		m_MirrorRaycast = gameObject.AddComponent<MirrorRaycast> ();
		BindListener ();
	}

	void Awake ()
	{  
		RectTransform parentTransform = UIMgr.Instance.RootRectTransform;
		float rootWidth = parentTransform.rect.width;
		float rootHeight = parentTransform.rect.height;

		RectTransform thisTransform = transform as RectTransform;
		float myWith = thisTransform.rect.width;
		float myHeigth = thisTransform.rect.height;

		m_MinPosX =  myWith / 2.0f;
		m_MaxPosX = rootWidth  - myWith / 2.0f;
		m_MinPosY = myHeigth / 2.0f;
		m_MaxposY = rootHeight  - myHeigth / 2.0f;
	}  
	void OnDestory()
	{
		UnBindListener();
	}
	
	void BindListener()
	{
		GameEventMgr.Instance.AddListener<bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
	}
	
	void UnBindListener()
	{
		GameEventMgr.Instance.RemoveListener<bool>(GameEventList.SetMirrorModeState, OnSetMirrorModeState);
	}
	// 鼠标按下  
	public void OnPointerDown (PointerEventData data) 
	{  

	}  
	// 拖动  
	public void OnDrag (PointerEventData data)
	{  
		transform.position = GetNewPosition (Input.mousePosition);

	}  

	Vector3 GetNewPosition(Vector3 mousePosition)
	{
		Vector3 newPos = new Vector3 (mousePosition.x, mousePosition.y, mousePosition.z);
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

	void	OnSetMirrorModeState(bool isMirror)
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
		m_curFindTarget = null;
		StartCoroutine (weakPointRayCastCo());
	}

	void StopRayCast()
	{
		StopCoroutine(weakPointRayCastCo());
		if (null != m_curFindTarget) 
		{
			GameEventMgr.Instance.FireEvent<MirrorTarget>(GameEventList.MirrorOutWeakPoint,m_curFindTarget);
		}
	}

	IEnumerator weakPointRayCastCo()
	{
		MirrorTarget findTarget = null;
		float findTimeCount = 0f;
		while (true)
		{
			findTarget = m_MirrorRaycast.WeakpointRayCast (transform.position);
			if(findTarget)
			{
				if(null == m_curFindTarget)
				{
					m_curFindTarget = findTarget;
					GameEventMgr.Instance.FireEvent<MirrorTarget,MirrorTarget>(GameEventList.FindWeakPoint,m_curFindTarget,null);
					findTimeCount = 0f;
				}
				else
				{
					if(findTarget == m_curFindTarget)
					{
						findTimeCount += Time.deltaTime;
						//Debug.LogError("findTimeCount = " + findTimeCount + " needtime = " + GameConfig.Instance.FindWeakPointFinishedNeedTime );
						if(findTimeCount > GameConfig.Instance.FindWeakPointFinishedNeedTime)
						{
							GameEventMgr.Instance.FireEvent<MirrorTarget>(GameEventList.FindFinishedWeakPoint,m_curFindTarget);
						}
					}
					else
					{
						Debug.LogError("hello World!!");
						GameEventMgr.Instance.FireEvent<MirrorTarget,MirrorTarget>(GameEventList.FindWeakPoint,findTarget,m_curFindTarget);
						findTimeCount = 0f;
						m_curFindTarget = findTarget;
					}

				}
			}
			else
			{
				if(null != m_curFindTarget)
				{
					GameEventMgr.Instance.FireEvent<MirrorTarget>(GameEventList.MirrorOutWeakPoint,m_curFindTarget);
					m_curFindTarget = null;
				}
			}
			//Debug.LogError("finding....."+ transform.position.x);
			yield return new WaitForSeconds(0.05f);
		}
	}

}
