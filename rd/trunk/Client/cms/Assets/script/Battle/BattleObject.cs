using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BattleObjectType
{
    Scene,
    Unit,
}

public class BattleObject : MonoBehaviour
{
    public BattleObjectType type = BattleObjectType.Unit ;
    public UnitCamp camp;
    public int id;
    public GameUnit unit;

    public ActorEventService actorEventService;
    public AnimControl aniControl;
    public List<ActorEventData> activeEventList;
    public List<ActorEventData> waitEventList;

    void Awake()
    {
        actorEventService = ActorEventService.Instance;
        activeEventList = new List<ActorEventData>();
        waitEventList = new List<ActorEventData>();
    }

    public void TriggerEvent(string eventID, float triggerTime)
    {
        ActorEventData srcEvent = null;
        if (actorEventService.GetEvent(eventID, out srcEvent))
        {
            //trigger event
            if (srcEvent.state == 0)
            {
                ActorEventData curEvent = new ActorEventData();
                curEvent.triggerTime = triggerTime;
                curEvent.id = srcEvent.id;
                curEvent.motionKey = srcEvent.motionKey;
                curEvent.motionValue = srcEvent.motionValue;
                curEvent.particleAsset = srcEvent.particleAsset;
                curEvent.particleBundle = srcEvent.particleBundle;
                curEvent.particleAni = srcEvent.particleAni;
                curEvent.particleParent = srcEvent.particleParent;
                curEvent.cameraAni = srcEvent.cameraAni;
                curEvent.ps = null;
                waitEventList.Add(curEvent);
            }
            //remove event
            else
            {
                ActorEventData curEventData;
                for (int i = activeEventList.Count - 1; i >= 0; --i)
                {
                    curEventData = activeEventList[i];
                    if (curEventData.particleAsset == srcEvent.particleAsset)
                    {
                        Destroy(curEventData.psObject);
                        activeEventList.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        UpdateEventsInternal();
        ActorEventData curEventData;
        for (int i = activeEventList.Count - 1; i >= 0; --i)
        {
            curEventData = activeEventList[i];
            if (curEventData.ps != null && curEventData.ps.isStopped)
            {
                Destroy(curEventData.psObject);
                activeEventList.RemoveAt(i);
            }
        }
    }

    private void UpdateEventsInternal()
    {
        float curTime = Time.time;
        ActorEventData curEvent;
        for (int i = waitEventList.Count - 1; i >= 0; --i)
        {
            curEvent = waitEventList[i];
            if (curEvent.triggerTime <= curTime)
            {
                activeEventList.Add(curEvent);
                //TODO: not only bool
                if (curEvent.motionKey != null && curEvent.motionKey.Length > 0)
                {
                    aniControl.SetBool(curEvent.motionKey, bool.Parse(curEvent.motionValue));
                }

                if (curEvent.particleAsset != null && curEvent.particleAsset.Length > 0)
                {
                    GameObject prefab = ResourceMgr.Instance.LoadAsset(curEvent.particleBundle, curEvent.particleAsset);
                    curEvent.psObject = GameObject.Instantiate(prefab);
                    curEvent.psObject.transform.parent = transform;
                    curEvent.psObject.transform.localPosition = prefab.transform.position;
                    curEvent.psObject.transform.localRotation = prefab.transform.rotation;
                    curEvent.psObject.transform.localScale = prefab.transform.localScale;

                    if (curEvent.particleParent != null && curEvent.particleParent.Length > 0)
                    {
                        Transform parentNode = transform.Find(curEvent.particleParent);
                        if (parentNode != null)
                        {
                            curEvent.psObject.transform.parent = parentNode;
                        }
                    }
                    curEvent.ps = curEvent.psObject.GetComponent<ParticleSystem>();
                    if (curEvent.ps != null)
                    {
                        curEvent.ps.Play();
                    }

                    if (curEvent.particleAni != null && curEvent.particleAni.Length > 0)
                    {
                        Animator animator = curEvent.psObject.GetComponentInChildren<Animator>();
                        int curStateHash = Animator.StringToHash(curEvent.particleAni);
                        if (animator != null && animator.HasState(0, curStateHash))
                        {
                            animator.Play(curStateHash);
                        }
                    }
                }

                if (curEvent.cameraAni != null && curEvent.cameraAni.Length > 0)
                {

                }

                waitEventList.RemoveAt(i);
            }
        }
    }
}
