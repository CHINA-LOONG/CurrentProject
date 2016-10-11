using UnityEngine;
using System.Collections;

public class ShakeUi : MonoBehaviour
{
    public float shakeDeltaX = 0.02f;
    public float shakeDeltaY = 0.06f;
    //震动一次时长
    public float shakeOnceTime = 0.02f;
    //震动总时长
    public float shakeLong = 1.0f;

    private bool isShake = false;
    private bool shakeEnable = false;

    float startShakeTime = 0;
    float curTime;
    float shakeTime = 0;
    Vector3 oldPosition = Vector3.zero;

    void OnEnable()
    {
        GameEventMgr.Instance.AddListener<bool>(GameEventList.ShowBattleUI, OnBattleUIShow);
    }
    void OnDisable()
    {
        GameEventMgr.Instance.RemoveListener<bool>(GameEventList.ShowBattleUI, OnBattleUIShow);
    }
    void OnBattleUIShow(bool showDazhao)
    {
        shakeEnable = showDazhao;
        if (showDazhao == false && isShake)
        {
            isShake = false;
            transform.position = oldPosition;
        }
    }

    public void SetShake()
    {
        if (shakeEnable == true)
        {
            startShakeTime = Time.time;
            oldPosition.x = transform.position.x;
            oldPosition.y = transform.position.y;
            oldPosition.z = transform.position.z;
            shakeTime = shakeOnceTime;

            isShake = true;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (isShake)
        {
            curTime = Time.time;
            if(curTime - startShakeTime > shakeLong)
            {
                isShake = false;
                transform.position = oldPosition;
                return;
            }

            if (Mathf.Abs(shakeTime - shakeOnceTime) < 0.0001)
            {
                transform.position = new Vector3(-shakeDeltaX *( Random.value-0.5f) + oldPosition.x,
                                                   -shakeDeltaY * (Random.value-0.5f) + oldPosition.y,
                                                   oldPosition.z);
            }
            shakeTime -= Time.deltaTime;
            if (shakeTime < 0)
            {
                shakeTime = shakeOnceTime;
            }
        }
    }
}
