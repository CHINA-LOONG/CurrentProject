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
    public int guid;
    public GameUnit unit;

    public ActorEventService actorEventService;
    public AnimControl aniControl;
    public List<ActorEventData> activeEventList;
    public List<ActorEventData> waitEventList;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        actorEventService = ActorEventService.Instance;
        activeEventList = new List<ActorEventData>();
        waitEventList = new List<ActorEventData>();
    }
    //---------------------------------------------------------------------------------------------
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
    //---------------------------------------------------------------------------------------------
    public void OnEnterField()
    {
        BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(guid);
        if (bo != null)
        {
            bo.gameObject.SetActive(true);
            GameObject slotNode = BattleController.Instance.GetSlotNode(camp, unit.pbUnit.slot, unit.isBoss);
            bo.transform.position = slotNode.transform.position;
            bo.transform.rotation = slotNode.transform.rotation;
            bo.transform.localScale = slotNode.transform.localScale;
            bo.gameObject.transform.SetParent(GameMain.Instance.transform);
            if (bo.camp == UnitCamp.Enemy)
            {
                GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.LoadBattleObjectFinished, this);
            }
        }

        unit.ReCalcSpeed();

        Logger.LogFormat("Unit {0} guid:{1} has entered field", name, guid);
    }
    //---------------------------------------------------------------------------------------------
    public void OnExitField()
    {
        //if (pbUnit.camp == UnitCamp.Enemy)
        //{
        //    GameObject.Destroy(unitObject);
        //    unitObject = null;

        //}
        BattleObject unit = ObjectDataMgr.Instance.GetBattleObject(guid);
        if (unit != null)
        {
            unit.gameObject.SetActive(false);
            Logger.LogFormat("Unit {0} guid:{1} has exited field", name, guid);
        }
    }
    //---------------------------------------------------------------------------------------------
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
    //---------------------------------------------------------------------------------------------
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
                if (aniControl != null && curEvent.motionKey != null && curEvent.motionKey.Length > 0)
                {
                    aniControl.SetBool(curEvent.motionKey, bool.Parse(curEvent.motionValue));
                }

                if (curEvent.particleAsset != null && curEvent.particleAsset.Length > 0)
                {
                    GameObject prefab = ResourceMgr.Instance.LoadAsset(curEvent.particleBundle, curEvent.particleAsset);
                    curEvent.psObject = GameObject.Instantiate(prefab);
                    curEvent.psObject.transform.parent = transform;
                    //curEvent.psObject.transform.localPosition = prefab.transform.position;
                    //curEvent.psObject.transform.localRotation = prefab.transform.rotation;
                    //curEvent.psObject.transform.localScale = prefab.transform.localScale;

                    if (curEvent.particleParent != null && curEvent.particleParent.Length > 0)
                    {
                        //Transform parentNode = transform.Find(curEvent.particleParent);
                        GameObject parentNode = Util.FindChildByName(gameObject, curEvent.particleParent);
                        if (parentNode != null)
                        {
                            curEvent.psObject.transform.parent = parentNode.transform;
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
    //---------------------------------------------------------------------------------------------
}
