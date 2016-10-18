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
            animator.SetTrigger("playEffect1");
        }
        else
        {
            animator.SetTrigger("playEffect2");
        }
    }

    public void OnStop()
    {
        if (CallBack!=null)
        {
            CallBack();
        }
    }
}
