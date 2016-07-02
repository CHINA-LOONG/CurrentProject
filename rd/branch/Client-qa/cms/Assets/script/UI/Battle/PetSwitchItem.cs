using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//IPointerClickHandler
public class PetSwitchItem : MonoBehaviour, IPointerClickHandler
{
    public Text text;

    public int targetId;
    public GameUnit unit;

	// Use this for initialization
	void Start ()
	{
		gameObject.AddComponent<Button> ();
	}

	public	void OnPointerClick (PointerEventData eventData)
	{
        GameEventMgr.Instance.FireEvent<int, int>(GameEventList.SwitchPet, targetId, unit.pbUnit.guid);
        GameEventMgr.Instance.FireEvent(GameEventList.HideSwitchPetUI);
	}
}
