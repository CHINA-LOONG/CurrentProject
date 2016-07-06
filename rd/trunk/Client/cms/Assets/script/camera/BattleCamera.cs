using UnityEngine;
using System.Collections;

public class BattleCamera : MonoBehaviour
{
	public class AniControlParam
	{
		public static int  phyDazhao;
	}

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

	public Animator animator = null;
	public CameraAni cameraAni = null;

	static BattleCamera mInst = null;
	public static BattleCamera Instance
	{
		get
		{
			if (mInst != null)
				return mInst;

			GameObject go = GameObject.Find("BattleCamera");
			if(go == null)
			{
				GameObject prefab =  ResourceMgr.Instance.LoadAsset("camera","BattleCamera");
				go = Instantiate(prefab) as GameObject;
				go.name = "BattleCamera";
			}
			mInst = go.GetComponent<BattleCamera>();
			mInst.CameraAttr = go.GetComponent<Camera>();
			mInst.animator = go.GetComponent<Animator>();
			mInst.cameraAni = go.AddComponent<CameraAni>();

			return mInst;
		}
	}

	public void Init()
	{
		//AniControlParam.phyDazhao = Animator.StringToHash ("phyDazhao");
	}



	public static void DestroyCamera()
	{
		Destroy(mInst.gameObject);
		mInst = null;
	}
}
