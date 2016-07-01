using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
//IPointerClickHandler
public class PetSwitchItem : MonoBehaviour, IPointerClickHandler
{

	[SerializeField]
	int	m_PetPositionIndex;
	public	int	PetPositionIndex
	{
		get
		{
			return m_PetPositionIndex;
		}
	}

	// Use this for initialization
	void Start ()
	{
		gameObject.AddComponent<Button> ();
	}
	public	void OnPointerClick (PointerEventData eventData)
	{
		Debug.LogError ("-----OnPointerClick At Index " + m_PetPositionIndex );
	}

}
