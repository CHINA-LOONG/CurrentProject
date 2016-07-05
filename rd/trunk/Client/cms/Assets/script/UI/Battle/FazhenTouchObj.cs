using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FazhenTouchObj : MonoBehaviour ,IPointerExitHandler
{

	// Use this for initialization
	void Start () 
	{
	
	}
	private FazhenStyle  fazhenStyle;

	public void SetFazhenStyle(FazhenStyle style)
	{
		fazhenStyle = style;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		fazhenStyle.AddTouchedTransform (this.transform);
	}
}
