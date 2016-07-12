using UnityEngine;
using System.Collections;

public class DropItems : MonoBehaviour {

    float endTime = -1.0f;
    Animator animator;//动画
    GameObject effectDrop = null;//掉落消失的特效
    GameObject item = null;//子物体

	void Start()
    {
        animator = GetComponent<Animator>();
        endTime = Time.time + Random.Range(5f, 5.5f);

    }
    
    void Update()
    {
        if (Time.time >= endTime && endTime > 0.0f)
        {
            OnHit();
        }
    }

    public void Destroy()//销毁自己
    {
        //ResourceMgr.Instance.DestroyAsset(gameObject);
        ItemDropManager.Instance.DestroyDropItem(gameObject);
    }

    public void OnHit()//被点击
    {
        item = transform.FindChild("Cube").gameObject;
        effectDrop= ResourceMgr.Instance.LoadAsset("jiuweihu_wugong");
        effectDrop.transform.parent = item.transform;
        effectDrop.transform.position = item.transform.position;

        endTime = -1.0f;
        animator.SetBool("FallBool", true);
    } 
}