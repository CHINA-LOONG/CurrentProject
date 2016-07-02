using UnityEngine;
using System.Collections;

public class BattleCamera : MonoBehaviour
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

	static BattleCamera mInst = null;
	public static BattleCamera Instance
	{
		get
		{
			if (mInst != null)
				return mInst;
			GameObject go1 = GameObject.Find("BattleCamera");
			if (go1 != null)
			{
				mInst = go1.GetComponent<BattleCamera>();
				mInst.CameraAttr = go1.GetComponent<Camera>();
			}
			else
			{
				if (mInst == null)
				{
					GameObject prefab =  ResourceMgr.Instance.LoadAsset("camera","BattleCamera");
					GameObject go = Instantiate(prefab) as GameObject;
					go.name = "BattleCamera";
					mInst = go.GetComponent<BattleCamera>();
					mInst.CameraAttr = go1.GetComponent<Camera>();
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
		Destroy(mInst.gameObject);
		mInst = null;
	}



}
