using UnityEngine;
using System.Collections;

public class MonsterIconBg : MonoBehaviour
{
	public	GameObject focusEffect;
    public GameObject plusObject;
    public GameObject lockObject;
    //TODO：标记是否修改 xiaolong 2015-10-21 15:44:05
	private	Transform	iconTranform;

	// Use this for initialization
	void Start () 
	{
	
	}

	public	void	AddIcon(Transform iconGo)
	{
	}

	public	void	ClearIcon()
	{
		if (null != iconTranform)
		{
		}
	}

	public	void	SetEffectShow(bool bshow)
	{
		focusEffect.SetActive (bshow);
	}

    public  void SetAsLocked(bool isLock)
    {
        if (lockObject != null)
        {
            lockObject.SetActive(isLock);
        }

        if (plusObject != null)
        {
            plusObject.SetActive(!isLock);
        }
    }

}
