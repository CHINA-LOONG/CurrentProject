using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class AnimControl : MonoBehaviour 
{
    private AnimatorOverrideController actualControler;

    private Animator animator;
    //param hash
    private int hashDead;
    private int hashRun;
    private int hashWin;
    private int hashDazhao;
    private int hashWugong;
    private int hashLazy;
    private int hashStun;
    //state hash
    private int hashDeadState;
    private int hashStunState;
    private int hashWinState;

    //public delegate void AnimEvent(string clipName);
    //public static event AnimEvent OnAnimationBegin;
    //public static event AnimEvent OnAnimationEnd;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        RuntimeAnimatorController curController = animator.runtimeAnimatorController;
        actualControler = new AnimatorOverrideController();

        actualControler.runtimeAnimatorController = curController;
        animator.runtimeAnimatorController = actualControler;
        
        hashDead = Animator.StringToHash("dead");
        hashRun = Animator.StringToHash("run");
        hashWin = Animator.StringToHash("win");
        hashDazhao = Animator.StringToHash("dazhao");
        hashWugong = Animator.StringToHash("wugong");
        hashLazy = Animator.StringToHash("lazy");
        hashStun = Animator.StringToHash("stun");

        hashDeadState = Animator.StringToHash("Base Layer.siwang");
        hashStunState = Animator.StringToHash("Base Layer.shoukong");
        hashWinState = Animator.StringToHash("Base Layer.shengli");
    }
    //---------------------------------------------------------------------------------------------
    public void SetController(string controllerName)
    {
        RuntimeAnimatorController curController = (RuntimeAnimatorController)Resources.Load(controllerName);
        if (curController != null)
        {
            actualControler.runtimeAnimatorController = curController;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Pause()
    {
        animator.speed = 0;
    }
    //---------------------------------------------------------------------------------------------
    public void SetSpeed(float speed)
    {
        animator.speed = speed;
    }
    //---------------------------------------------------------------------------------------------
    public void SetFloat(string name, float value)
    {
        //TODO: use hash
        animator.SetFloat(name, value);
    }
    //---------------------------------------------------------------------------------------------
    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
    //---------------------------------------------------------------------------------------------
    public void SetInt(string name, int value)
    {
        animator.SetInteger(name, value);
    }
    //---------------------------------------------------------------------------------------------
    //call back functions
    //---------------------------------------------------------------------------------------------
    public void OnDeadBegin()
    {
    }
    //---------------------------------------------------------------------------------------------
    public void OnDeadEnd()
    {
    }
    //---------------------------------------------------------------------------------------------
    public void OnRunBegin()
    {
    }
    //---------------------------------------------------------------------------------------------
    public void OnRunEnd()
    {
        animator.SetBool(hashRun, false);
    }
    //---------------------------------------------------------------------------------------------
    public void OnWinBegin()
    {
    
    }
    //---------------------------------------------------------------------------------------------
    public void OnWinEnd()
    {
        animator.SetBool(hashWin, false);
    }
    //---------------------------------------------------------------------------------------------
    public void OnDaZhaoBegin()
    {

    }
    //---------------------------------------------------------------------------------------------
    public void OnDaZhaoEnd()
    {
        animator.SetBool(hashDazhao, false);
    }
    //---------------------------------------------------------------------------------------------
    public void OnWuGongBegin()
    {
    
    }
    //---------------------------------------------------------------------------------------------
    public void OnWuGongEnd()
    {
        animator.SetBool(hashWugong, false);
    }
    //---------------------------------------------------------------------------------------------
    public void OnLazyBegin()
    {

    }
    //---------------------------------------------------------------------------------------------
    public void OnLazyEnd()
    {
        animator.SetBool(hashLazy, false);
    }
    //---------------------------------------------------------------------------------------------
    public void OnStunBegin()
    {

    }
    //---------------------------------------------------------------------------------------------
    public void OnStunEnd()
    {
        animator.SetBool(hashStun, false);
    }
    //---------------------------------------------------------------------------------------------
}
