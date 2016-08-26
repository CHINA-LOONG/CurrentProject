using UnityEngine;
using System.Collections;

public class BaoxiangState : MonoBehaviour
{
    public enum State:int
    {
        BukeLingqu = 0,
        Kelingqu,
        YiLingqu
    }

    Animator animatorController = null;
    int aniStatehash = 0;
	// Use this for initialization
	void Awake ()
    {
        aniStatehash = Animator.StringToHash("state");
    }
	
	public void SetState(State bState)
    {
        if(null == animatorController)
        {
            animatorController = GetComponent<Animator>();
        }
        animatorController.SetInteger(aniStatehash, (int)bState);
    }
}
