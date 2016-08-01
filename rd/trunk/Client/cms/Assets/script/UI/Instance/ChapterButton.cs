using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ChapterButton : MonoBehaviour
{
    public int chapterIndex = 0;
    public float touchScale = 1.15f;

    public Image chapterImage;
    public Text chapterName;
    public Material lockedMaterial;

    private RectTransform rectTrans;
   
	// Use this for initialization
	void Start ()
    {
        rectTrans = transform as RectTransform;

        EventTriggerListener.Get(this.gameObject).onDown = OnTouchEnter;
        EventTriggerListener.Get(this.gameObject).onEnter = OnTouchEnter;
        EventTriggerListener.Get(this.gameObject).onExit = OnTouchExit;
        EventTriggerListener.Get(this.gameObject).onUp = OnTouchExit;
        EventTriggerListener.Get(this.gameObject).onClick = OnClicked;
	}

    void    OnTouchEnter(GameObject go)
    {
        rectTrans.localScale = new Vector3(touchScale, touchScale, touchScale);
    }

    void    OnTouchExit(GameObject go)
    {
        rectTrans.localScale = new Vector3(1, 1, 1);
    }

    void OnClicked(GameObject go)
    {
        InstanceMap.Instance.OnChapterButtonOnClicked(this);
        if(InstanceMapService.Instance.IsChapterOpened(chapterIndex))
        {
            InstanceList.OpenWith(chapterIndex);
        }
    }

    public void SetLock(bool isLocked)
    {
        if(isLocked)
        {
            chapterImage.material = lockedMaterial;
        }
        else
        {
            chapterImage.material = null;
        } 
    }
}
