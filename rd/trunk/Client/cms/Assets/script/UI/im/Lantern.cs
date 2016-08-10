using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;
public class Lantern : MonoBehaviour
{
    public class LanternData
    {
        public string content;
        public float beginTime;
        public float nextTime;
        public Text lant;
        public bool isRoll;
        public Vector3 moveVec;
        public Tweener battleTitleTw = null;
    }
    public int speed = 50;
    [HideInInspector]
    List<LanternData> lanternMsg = new List<LanternData>();//走马灯    
    public bool moveTypeEnd;
    [HideInInspector]
    public bool roll;
    
    //void Awake()
    //{
    //    GameEventMgr.Instance.AddListener<EventArgs>(GameEventList.SpeedChangeEvent, OnSpeedChange);
    //}

    //void Destroy()
    //{
    //    GameEventMgr.Instance.RemoveListener<EventArgs>(GameEventList.SpeedChangeEvent, OnSpeedChange);
    //}

    public void AddMsg(string msg,int type)
    {
        LanternData lanData = new LanternData();
        lanData.content = msg;
        lanData.lant = ResourceMgr.Instance.LoadAsset("lanternText").GetComponent<Text>();
        if (type == (int)PB.ImType.LANTERN)
        {
            lanData.lant.fontSize = 24;
        }
        else if (type == (int)PB.ImType.NOTICE)
        {
            lanData.lant.color = Color.red;
        }
        lanData.lant.transform.parent = transform;
        lanData.lant.transform.localScale = transform.localScale;
        lanData.lant.text = lanData.content;
        lanData.lant.rectTransform.localPosition = new Vector3(transform.GetComponent<Image>().rectTransform.sizeDelta.x, 0, 0);//起始位置;
        lanData.moveVec = new Vector3(0 - lanData.lant.preferredWidth, 0, 0);//移动目标位置
        float duration = (lanData.lant.rectTransform.localPosition.x - lanData.moveVec.x) / speed;
        lanData.beginTime = Time.unscaledTime;
        lanData.nextTime = Time.unscaledTime + duration;
        if (lanternMsg.Count >= 1)
        {
            float interval = duration;
            LanternData lastLanter = lanternMsg[lanternMsg.Count - 1];
            if (moveTypeEnd == true)
            {
                //100 is the distance between two lanterns
                interval = (lastLanter.lant.preferredWidth + 100) / speed;
            }
            lanData.beginTime = lastLanter.beginTime + interval;
            lanData.nextTime = lanData.beginTime + duration;
        }

        lanternMsg.Add(lanData);
    }
    public void UpdateLantern()
    {
        if (lanternMsg.Count > 0)
        {
            gameObject.SetActive(true);
        }
        for (int i = 0; i < lanternMsg.Count; i++)
        {
            if (lanternMsg[i].beginTime <= Time.unscaledTime && lanternMsg[i].isRoll == false)
            {
                lanternMsg[i].isRoll = true;
                lanternMsg[i].battleTitleTw = lanternMsg[i].lant.transform.DOLocalMove(lanternMsg[i].moveVec, lanternMsg[i].nextTime - lanternMsg[i].beginTime, true);
                lanternMsg[i].battleTitleTw.SetUpdate(true);
                lanternMsg[i].battleTitleTw.SetEase(Ease.Linear);
                lanternMsg[i].battleTitleTw.OnComplete(LanternMoveEnd);
                if (Time.timeScale != 0.0f)
                {
                    lanternMsg[i].battleTitleTw.timeScale = 1.0f / Time.timeScale;
                }
            }
        }
        if (lanternMsg.Count > 0 && GameMain.Instance.IsCurModule<BattleModule>() && moveTypeEnd)
        {
            RemoveAll();
        }
    }

    void LanternMoveEnd()//走马灯结束
    {
        ResourceMgr.Instance.DestroyAsset(lanternMsg[0].lant.gameObject);
        lanternMsg.RemoveAt(0);        
        if (lanternMsg.Count <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    void RemoveAll()//删除全部
    {
        for (int i = 0; i < lanternMsg.Count; i++)
        {
            //TODO: need to delete battleTitleTw?
            ResourceMgr.Instance.DestroyAsset(lanternMsg[i].lant.gameObject);
        }
        lanternMsg.Clear();
        gameObject.SetActive(false);
    }
    //public void OnSpeedChange(System.EventArgs args)
    //{
    //    float curTimeScale = 1.0f;
    //    if (Time.timeScale != 0.0f)
    //    {
    //        curTimeScale = 1.0f / Time.timeScale;
    //    }

    //    for (int i = 0; i < lanternMsg.Count; i++)
    //    {
    //        if (lanternMsg[i].battleTitleTw != null)
    //        {
    //            lanternMsg[i].battleTitleTw.timeScale = curTimeScale;
    //        }
    //    }

    //}
}
