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
        if (Time.time >= showTime)
        {
            GameObject hint = transform.FindChild("Image").gameObject;
            hint.SetActive(true);
        }

        if (Time.time >= fadeTime && beginFade == false)
        {
            beginFade = true;
            battleTitleTw = gameObject.transform.DOLocalMoveY(100, gradualTime);
            gameObject.transform.DOScale(0, gradualTime);
            bak.DOColor(new Color(0, 0, 0, 0), gradualTime);
            text.DOColor(new Color(0, 0, 0, 0), gradualTime);
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
