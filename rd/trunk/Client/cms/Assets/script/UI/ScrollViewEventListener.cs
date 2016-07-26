using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ScrollViewEventListener : MonoBehaviour, IPointerClickHandler,
                                                  IPointerDownHandler,
                                                  IPointerUpHandler,
                                                  IPointerExitHandler,
                                                  IPointerEnterHandler
                                                  
{
    public delegate void VoidDelegate(GameObject go);

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onUp;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onPressEnter;
    public VoidDelegate onPressExit;

    static public ScrollViewEventListener Get(GameObject go)
    {
        ScrollViewEventListener listener = go.GetComponent<ScrollViewEventListener>();
        if (listener == null) listener = go.AddComponent<ScrollViewEventListener>();
        return listener;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (needPressExit)  return;
        //Debug.Log("button: OnPointerClick");
        if (onClick != null && !press) onClick(gameObject);
    }


    void Update()
    {
        if (onPressEnter != null)
        {
            if (pressDown && !enterPress)
            {
                if (Time.time - pressTime > 1.0f)
                {
                    onPressEnter(gameObject);
                    press = true;
                    enterPress = true;
                    needPressExit = true;
                }
            }
            if (!pressDown && press)
            {
                press = false;
                enterPress = false;
                //Debug.Log("button: onPressExit");
                if (onPressExit != null) onPressExit(gameObject);
            }
        }
    }

    #region LongPress_used_member

    private float pressTime;
    private bool pressDown = false;     
    private bool press = false;         
    private bool enterPress = false;    
    private bool needPressExit = false;

    IEnumerator ExitPress()
    {
        yield return 2;
        needPressExit = false;
    }

    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("button: OnPointerDown");
        if (onDown != null) onDown(gameObject);

        pressDown = true;
        pressTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("button: OnPointerUp");
        if (onUp != null) onUp(gameObject);

        pressDown = false;
        if (needPressExit)
        {
            StartCoroutine(ExitPress());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExit!=null)
        {
            onExit(gameObject);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null)
        {
            onEnter(gameObject);
        }
    }
}
