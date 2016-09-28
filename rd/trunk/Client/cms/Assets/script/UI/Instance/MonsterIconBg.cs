using UnityEngine;
using System.Collections;

public class MonsterIconBg : MonoBehaviour
{
    public delegate void LockClickDelegate();
	public	GameObject focusEffect;
    public GameObject plusObject;
    public GameObject lockObject;
    //TODO：标记是否修改 xiaolong 2015-10-21 15:44:05
	private	Transform	iconTranform;

    private LockClickDelegate callBack = null;

    // Use this for initialization
    void Start () 
	{
        EventTriggerListener.Get(gameObject).onClick = OnBgClick;
	}

	public	void	SetEffectShow(bool bshow)
	{
		focusEffect.SetActive (bshow);
	}

    public  void SetAsLocked(bool isLock,LockClickDelegate callBack = null)
    {
        this.callBack = callBack;
        if (lockObject != null)
        {
            lockObject.SetActive(isLock);
        }

        if (plusObject != null)
        {
            plusObject.SetActive(!isLock);
        }
        if(isLock)
        {
            SetEffectShow(false);
        }
    }

    void OnBgClick(GameObject go)
    {
        if(null != callBack)
        {
            callBack();
        }
    }
}
