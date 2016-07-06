using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraAni : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		//transform.DOMove (testTarget.position, 3);
		//transform.DORotate (testTarget.rotation.eulerAngles, 3);
	}
	//---------------------------------------------------------------------------------------
	public	Tweener MotionTo(Transform targetTrans,float duration)
	{
		return	MotionTo (targetTrans, duration, true);
	}
	//---------------------------------------------------------------------------------------
	public Tweener MotionTo(Transform targetTrans,float duration,bool isAni)
	{
		if (isAni)
		{
			transform.DOMove (targetTrans.position, duration);
			return	transform.DORotate (targetTrans.rotation.eulerAngles, duration);
		}
		else
		{
			transform.position = targetTrans.position;
			transform.rotation = targetTrans.rotation;
			return null;
		}
	}
}
