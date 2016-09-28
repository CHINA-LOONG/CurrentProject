using UnityEngine; 
using System.Collections;  

public class moveTest : MonoBehaviour {  

    private bool pressed;
    // Use this for initialization  
    void Start () {  
       
        ScrollViewEventListener.Get(transform.gameObject).onPressEnter = gmBtnPressEnter;       
        ScrollViewEventListener.Get(transform.gameObject).onPressExit = gmBtnPressExit;
           
    }

    void gmBtnPressEnter(GameObject go){
       
        StartCoroutine(OnMouseDown());
                
    }
    void gmBtnPressExit(GameObject go){

        StopCoroutine(OnMouseDown());
    }

    //值得注意的是世界坐标系转化为屏幕坐标系，Z轴是不变的  
    IEnumerator OnMouseDown ()
    {
           
           Logger.Log("move "+transform.name);
           if(transform.gameObject){  
            //转换对象到当前屏幕位置
            Vector3 screenPosition = UICamera.Instance.CameraAttr.WorldToScreenPoint (transform.position);
            
            //鼠标屏幕坐标
            Vector3 mScreenPosition=new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
            //获得鼠标和对象之间的偏移量,拖拽时相机应该保持不动
            Vector3 offset = transform.position - UICamera.Instance.CameraAttr.ScreenToWorldPoint( mScreenPosition);
         //   print ("drag starting:"+transform.name);
            
            //若鼠标左键一直按着则循环继续
            while (Input.GetMouseButton (0)) {
            //while (Input.GetButtonDown("Fire1")) {   
                //鼠标屏幕上新位置
                mScreenPosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
                
                // 对象新坐标 
            transform.position=offset + UICamera.Instance.CameraAttr.ScreenToWorldPoint (mScreenPosition);
                
                //协同，等待下一帧继续
                yield return new WaitForFixedUpdate ();
               
            }               
        }
    }  
}  