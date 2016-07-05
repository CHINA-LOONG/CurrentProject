using UnityEngine;
using System.Collections;

public class DazhaoFocusCamera : MonoBehaviour 
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
	
	static DazhaoFocusCamera mInst = null;

	public static DazhaoFocusCamera Instance
	{
		get
		{
			return mInst;
		}
	}

	// Use this for initialization
	void Start ()
	{
		mInst = this;

		mInst.CameraAttr = gameObject.GetComponent<Camera>();
	}

}
