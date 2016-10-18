using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
public class Hint : MonoBehaviour 
{
    public Image bak;
    public Text text;
    public float showTime;
    public float fadeTime;
    public float endTime;
    public float gradualTime = 0.5f;
    public float stopTime = 1.0f;
    bool beginFade = false;
    Tweener battleTitleTw;
    [HideInInspector]
    public List<GameObject> ownedList;
    void Update()
    {
        if (Time.unscaledTime >= showTime)
        {
            GameObject hint = transform.FindChild("Image").gameObject;
            hint.SetActive(true);
        }

        if (Time.unscaledTime >= fadeTime && beginFade == false)
        {
            beginFade = true;
            battleTitleTw = gameObject.transform.DOLocalMoveY(230, gradualTime);
            battleTitleTw.SetUpdate(true);
            //gameObject.transform.DOScale(0, gradualTime).SetUpdate(true);
            bak.gameObject.GetComponent<Graphic>().CrossFadeAlpha(0, gradualTime, true);
            text.gameObject.GetComponent<Graphic>().CrossFadeAlpha(0, gradualTime, true);
            battleTitleTw.OnComplete(End);
        }
    }
    public void SetFadeTime(float curTime)
    {
        if (beginFade == false)
        {
            fadeTime = curTime;
            endTime = fadeTime + gradualTime;
        }
    }
    void End()
    {
        ownedList.Remove(gameObject);
        ResourceMgr.Instance.DestroyAsset(gameObject);
    }
}
