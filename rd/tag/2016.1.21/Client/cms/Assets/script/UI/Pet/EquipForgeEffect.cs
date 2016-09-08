using UnityEngine;
using System.Collections;

public class EquipForgeEffect : MonoBehaviour
{
    public System.Action CallBack;
    public Animator animator;
    public void Play(int index,System.Action StopBack)
    {
        CallBack = StopBack;
        if (index==1)
        {
            animator.SetBool("PlayEffect1", true);
        }
        else
        {
            animator.SetBool("PlayEffect2", true);
        }
    }

    public void OnStop()
    {
        animator.SetBool("PlayEffect1", false);
        animator.SetBool("PlayEffect2", false);
        if (CallBack!=null)
        {
            CallBack();
        }
    }
}
