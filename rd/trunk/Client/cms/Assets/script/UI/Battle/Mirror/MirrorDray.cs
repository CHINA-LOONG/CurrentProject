using UnityEngine;  
using UnityEngine.UI;  
using UnityEngine.EventSystems;  
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MirrorDray : MonoBehaviour,IPointerDownHandler, IPointerUpHandler, IDragHandler,IPointerClickHandler
{

	float	m_MinPosX = 0f;
	float 	m_MaxPosX = 0f;
	float 	m_MinPosY = 0f;
	float	m_MaxposY = 0f;

	RectTransform rectTrans;

	Tweener  mirrorTween = null;

	Dictionary<MirrorTarget, float> lastFindWeakpoint = new Dictionary<MirrorTarget, float> ();

	MirrorRaycast m_MirrorRaycast = null;
	List<MirrorTarget> newFindTargetList =new List<MirrorTarget>();
	List<MirrorTarget> finishFindTargett = new List<MirrorTarget>();
	List<MirrorTarget> outFindTarget = new List<MirrorTarget> ();
	//Image mirrorUi;
	Image MirrorDragImage;
	Animator mirrorUIAnimator;


	bool isDragging = false;

	int mirrorStateHash = -1;

	ParticleEffect mirrorExitEffect = null;

	public void Init(GameObject mirrorUi)
	{
		mirrorStateHash = Animator.StringToHash ("mirrorState");
		if (null == m_MirrorRaycast)
		{
			m_MirrorRaycast = gameObject.AddComponent<MirrorRaycast> ();
		}
		rectTrans = transform as RectTransform;

		mirrorUIAnimator = mirrorUi.GetComponent < Animator> ();

		MirrorDragImage = Util.FindChildByName (gameObject, "MirrorDragImage").GetComponent<Image> ();

		//baozha_fire
		mirrorExitEffect = ParticleEffect.CreateEffect (BattleController.Instance.GetUIBattle ().publicTopGroup, "baozha_fire");
		mirrorExitEffect.gameObject.SetActive (false);

		ResetMirror ();
	}

	public void ResetMirror()
	{
		//MirrorDragImage.gameObject.SetActive (false);
		mirrorTween = MirrorDragImage.DOFade (0, 1.5f).OnComplete (OnResetMiirror);
	//	mirrorUi.gameObject.SetActive (true);
		mirrorUIAnimator.SetInteger (mirrorStateHash, 2);
	}

	void OnResetMiirror()
	{
		RectTransform mirrorUiRt = mirrorUIAnimator.transform.parent.transform as RectTransform;
		
		Vector3 screenPos = UICamera.Instance.CameraAttr.WorldToScreenPoint (mirrorUiRt.position);
		screenPos /= UIMgr.Instance.CanvasAttr.scaleFactor ;
		
		screenPos.x += (rectTrans.pivot.x - 0.5f) * rectTrans.sizeDelta.x;
		screenPos.y += (rectTrans.pivot.y - 0.5f) * rectTrans.sizeDelta.y;
		rectTrans.anchoredPosition = screenPos ;
	}


	void Awake ()
	{  
		//RectTransform parentTransform = transform.parent as RectTransform;
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
        if (BattleController.Instance.processStart == false)
            return;

		if (mirrorTween != null &&
		    mirrorTween.IsPlaying())
		{
			mirrorTween.Kill(true);
		}

		isDragging = false;
		//MirrorDragImage.gameObject.SetActive (true);
		MirrorDragImage.DOFade (1.0f, 0.5f);

		//mirrorUi.gameObject.SetActive (false);
		mirrorUIAnimator.SetInteger (mirrorStateHash, 1);
		rectTrans.anchoredPosition = GetMirrorScreenPosition (Input.mousePosition);	
		OnSetMirrorModeState (true);
	}  

	//鼠标抬起
	public void OnPointerUp (PointerEventData eventData)
	{
		OnSetMirrorModeState (false);
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

		/*if (newPos.x < m_MinPosX) {
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
		*/
		return newPos;
	}

	public void	OnSetMirrorModeState(bool isMirror)
	{
		if (isMirror) 
		{
			mirrorExitEffect.gameObject.SetActive(false);
			StartRayCast();
		} 
		else 
		{
			ResetMirror();
			StopRayCast();
			mirrorExitEffect.gameObject.SetActive(true);
			RectTransform rt = mirrorExitEffect.transform as RectTransform;
			//rt.anchoredPosition = new Vector2(300,300);
			if(rt != null )
			{
				Vector3 mirrorScreenPos = UIUtil.GetSpacePos(transform as RectTransform,UIMgr.Instance.CanvasAttr,UICamera.Instance.CameraAttr);
				mirrorScreenPos.x -= (rectTrans.pivot.x -0.5f)*rectTrans.sizeDelta.x;
				mirrorScreenPos.y -= (rectTrans.pivot.y -0.5f)*rectTrans.sizeDelta.y;

				float fscale = UIMgr.Instance.CanvasAttr.scaleFactor;
				rt.anchoredPosition = new Vector2(mirrorScreenPos.x/fscale,mirrorScreenPos.y/fscale);
			}
			else
			{
				//test
				mirrorExitEffect.transform.position = Vector3.zero;
			}

		}
	}

	void StartRayCast()
	{
		lastFindWeakpoint.Clear ();


		StartCoroutine ("weakPointRayCastCo");
	}

	void StopRayCast()
	{
		StopCoroutine("weakPointRayCastCo");

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
			mirrorScreenPos.x -= (rectTrans.pivot.x -0.5f)*rectTrans.sizeDelta.x;
			mirrorScreenPos.y -= (rectTrans.pivot.y -0.5f)*rectTrans.sizeDelta.y;



			listFindTarget = m_MirrorRaycast.WeakpointRayCast (mirrorScreenPos);
			if(listFindTarget.Count > 0)
			{
				MirrorTarget subTarget = null;
				for(int i =0 ; i< listFindTarget.Count; ++i)
				{
					subTarget = listFindTarget[i];
					WeakPointRuntimeData wpRuntime = subTarget.WpRuntimeData;

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
						if(wpRuntime.IsFind)
						{
							lastFindWeakpoint.Add(subTarget,0.0f);
							finishFindTargett.Add(subTarget);
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
			yield return new WaitForFixedUpdate();
		}
	}

}
