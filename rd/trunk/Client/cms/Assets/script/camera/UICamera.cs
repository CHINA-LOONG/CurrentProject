using UnityEngine;
using System.Collections;

public class UICamera : MonoBehaviour 
{
	[SerializeField]
	Camera	m_camera = null;
	public	Camera CameraAttr
	{
		set
		{
			m_camera = value;
		}
		get
		{
			return m_camera;
		}
	}

	static UICamera mInst = null;
	public static UICamera Instance
	{
		get
		{
			if (mInst != null)
				return mInst;
			GameObject go1 = GameObject.Find("UICamera");
			if (go1 != null)
			{
				mInst = go1.GetComponent<UICamera>();
				mInst.CameraAttr = go1.GetComponent<Camera>();
			}
			else
			{
				if (mInst == null)
				{
					GameObject go =  ResourceMgr.Instance.LoadAsset("camera","UICamera");
					//GameObject go = Instantiate(prefab) as GameObject;
					go.name = "UICamera";
					mInst = go.GetComponent<UICamera>();
					mInst.CameraAttr = go.GetComponent<Camera>();
				}
			}
			return mInst;
		}
	}
	
	public void Init()
	{
	}
	
	public static void DestroyCamera()
	{
        //Destroy(mInst.gameObject);
        ResourceMgr.Instance.DestroyAsset(mInst.gameObject);
		mInst = null;
	}
}
