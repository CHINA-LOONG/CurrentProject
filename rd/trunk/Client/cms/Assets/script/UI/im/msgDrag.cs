using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class msgDrag : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        UIIm.Instance.isDrag = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIIm.Instance.isDrag = false;
        UIIm.Instance.showNewMsg = true;
    }
}
