using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildButton : MonoBehaviour
{
    public Image iconImage;
    public Image tipImage;
    public Text nameText;

	// Use this for initialization
	void Start ()
    {
        tipImage.gameObject.SetActive(false);

        EventTriggerListener.Get(this.gameObject).onEnter = OnTouchEnter;
        EventTriggerListener.Get(this.gameObject).onExit = OnTouchExit;
        EventTriggerListener.Get(this.gameObject).onUp = OnTouchExit;

    }

    public  void    SetRemind(bool isRemind)
    {
        tipImage.gameObject.SetActive(isRemind);
    }

    void OnTouchEnter(GameObject go)
    {
        iconImage.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

    }

    void OnTouchExit(GameObject go)
    {
        iconImage.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

}
