using UnityEngine;
using System.Collections;

public class DissolveController : MonoBehaviour {

    public Material mDissolveMat;

    private float mDissolveDuration = 1.5f;
    private float mDissolveSpeed = -1.0f;
    private float mDissolveAmount = 0.0f;
    private Renderer mCurRenderer;
	// Update is called once per frame
    void Start()
    {
        mCurRenderer = gameObject.GetComponentInChildren<Renderer>();
    }
	void Update ()
    {
        if (mDissolveSpeed > 0)
        {
            mDissolveAmount += mDissolveSpeed * Time.deltaTime;
            if (mDissolveAmount > 1.0f)
            {
                mDissolveAmount = 1.0f;
                mDissolveSpeed = -1.0f;
            }
            mDissolveMat.SetFloat("_Amount", mDissolveAmount);
        }	    
	}

    public void StartDissolve()
    {
        mDissolveSpeed =  1.0f / mDissolveDuration;
        mDissolveAmount = 0.0f;
        mCurRenderer.material = mDissolveMat;
    }
}
