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

	MirrorRaycast m_MirrorRaycast;
	void Start()
	{
		m_MirrorRaycast = gameObject.AddComponent<MirrorRaycast> ();
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
	
	// 鼠标按下  
	public void OnPointerDown (PointerEventData data) 
	{  

	}  
	// 拖动  
	public void OnDrag (PointerEventData data)
	{  
		//GetComponent<RectTransform>().pivot.Set(0,0);
		transform.position = GetNewPosition (Input.mousePosition);
		m_MirrorRaycast.testRayCast (transform.position);
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

}
