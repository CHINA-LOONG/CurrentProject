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
	public GameObject shifaGo;
	public GameObject dazhaoPrepareEffect;

    public ActorEventService actorEventService;
    public AnimControl aniControl;
    public List<ActorEventData> activeEventList = new List<ActorEventData>();
    public List<ActorEventData> waitEventList = new List<ActorEventData>();

    public SimpleEffect shifaNodeEffect;
    private Quaternion targetRot;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        actorEventService = ActorEventService.Instance;
    }
    //---------------------------------------------------------------------------------------------
    public void TriggerEvent(string eventID, float triggerTime, string rootNode)
    {
        ActorEventData srcEvent = null;
        if (actorEventService.GetEvent(eventID, out srcEvent))
        {
            //trigger event
            if (srcEvent.state == 0)
            {
                ActorEventData curEvent = new ActorEventData();
                curEvent.triggerTime = triggerTime;
                curEvent.actorDelay = srcEvent.actorDelay;
                curEvent.id = srcEvent.id;
                curEvent.motionKey = srcEvent.motionKey;
                curEvent.motionValue = srcEvent.motionValue;
                curEvent.particleAsset = srcEvent.particleAsset;
                curEvent.particleBundle = srcEvent.particleBundle;
                curEvent.particleAni = srcEvent.particleAni;
                curEvent.particleParent = srcEvent.particleParent;
                curEvent.locky = srcEvent.locky;
                curEvent.cameraAni = srcEvent.cameraAni;
                curEvent.controllerName = srcEvent.controllerName;
                curEvent.psDuration = 0.0f;
                curEvent.attach = srcEvent.attach;
                curEvent.rootNode = rootNode;
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
                        ResourceMgr.Instance.DestroyAsset(curEventData.psObject);
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
        unit.State = UnitState.None;
        //BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(guid);
        //if (bo != null)
        {
            gameObject.SetActive(true);
            GameObject slotNode = BattleController.Instance.GetSlotNode(camp, unit.pbUnit.slot, unit.isBoss);
            transform.localPosition = slotNode.transform.position;
            transform.localRotation = slotNode.transform.rotation;
            transform.localScale = slotNode.transform.localScale;
            targetRot = gameObject.transform.localRotation;
            gameObject.transform.SetParent(GameMain.Instance.transform, false);
            if (camp == UnitCamp.Enemy)
            {
                GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.LoadBattleObjectFinished, this);
            }
        }

        unit.ReCalcSpeed();

        Logger.LogFormat("Unit {0} guid:{1} has entered field", name, guid);
    }
    //---------------------------------------------------------------------------------------------
    public void SetTargetRotate(Quaternion rot, bool reset)
    {
        if (type == BattleObjectType.Scene)
            return;

        if (reset == false)
        {
            targetRot = rot;
        }
        else
        {
            GameObject slotNode = BattleController.Instance.GetSlotNode(camp, unit.pbUnit.slot, unit.isBoss);
            targetRot = slotNode.transform.rotation;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnExitField()
    {
        //if (pbUnit.camp == UnitCamp.Enemy)
        //{
        //    GameObject.Destroy(unitObject);
        //    unitObject = null;

        //}
        //BattleObject unit = ObjectDataMgr.Instance.GetBattleObject(guid);
        //if (unit != null)
        {
            unit.State = UnitState.None;
            gameObject.SetActive(false);
            Logger.LogFormat("Unit {0} guid:{1} has exited field", name, guid);
        }
    }
    //---------------------------------------------------------------------------------------------
    void Update()
    {
        UpdateEventsInternal();

        ActorEventData curEventData;
        for (int i = activeEventList.Count - 1; i >= 0; --i)
        {
            curEventData = activeEventList[i];

            if (curEventData.psObject != null)
            {
                if (curEventData.psDuration >= 0.0f && Time.time - curEventData.triggerTime >= curEventData.psDuration)
                {
                    ResourceMgr.Instance.DestroyAsset(curEventData.psObject);
                    activeEventList.RemoveAt(i);
                }
            }
            else
            {
                activeEventList.RemoveAt(i);
            }
        }

        if (type == BattleObjectType.Unit && false)
        {
            float step = BattleConst.unitRotSpeed * Time.deltaTime;
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRot, step);
            transform.localRotation = Quaternion.Lerp(transform.rotation, targetRot, step);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ClearEvent()
    {
        waitEventList.Clear();
        ActorEventData curEventData;
        for (int i = activeEventList.Count - 1; i >= 0; --i)
        {
            curEventData = activeEventList[i];
            if (curEventData.psObject != null)
            {
                ResourceMgr.Instance.DestroyAsset(curEventData.psObject);
            }
        }
        activeEventList.Clear();
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
                if (curEvent.actorDelay > 0 && BattleController.Instance.Process != null)
                {
                    BattleController.Instance.Process.ActionDelayTime = curEvent.actorDelay;
                }
                if (aniControl != null)
                {
                    if (curEvent.controllerName != null && curEvent.controllerName.Length > 0)
                    {
                        aniControl.SetController(curEvent.controllerName);
                    }
                    //TODO: not only bool
                    if (curEvent.motionKey != null && curEvent.motionKey.Length > 0)
                    {
                        aniControl.SetBool(curEvent.motionKey, bool.Parse(curEvent.motionValue));
                    }
                }

                if (curEvent.particleAsset != null && curEvent.particleAsset.Length > 0)
                {
                    GameObject prefab = ResourceMgr.Instance.LoadAsset(curEvent.particleBundle, curEvent.particleAsset);
                    if (prefab != null)
                    {
                        curEvent.psObject = prefab;
                        Transform rootTransform = transform;
                        if (curEvent.rootNode != null && curEvent.rootNode.Length > 0)
                        {
                            GameObject rootParent = Util.FindChildByName(gameObject, curEvent.rootNode);
                            if (rootParent != null)
                            {
                                rootTransform = rootParent.transform;
                            }
                        }

                        if (curEvent.particleParent != null && curEvent.particleParent.Length > 0)
                        {
                            if (curEvent.rootNode != null)
                            {
                                Logger.LogWarning("weak point is ignored since event configs the parent node");
                            }
                            //Transform parentNode = transform.Find(curEvent.particleParent);
                            GameObject parentNode = Util.FindChildByName(gameObject, curEvent.particleParent);
                            if (parentNode != null)
                            {
                                rootTransform = parentNode.transform;
                            }
                        }

                        if (curEvent.attach == "true")
                        {
                            curEvent.psObject.transform.localPosition = prefab.transform.localPosition;
                            //curEvent.psObject.transform.localRotation = prefab.transform.localRotation;
                            curEvent.psObject.transform.localRotation = Quaternion.identity;
                            curEvent.psObject.transform.SetParent(rootTransform, false);
                            //NOTE: xw said if attach, ignore lock
                        }
                        else
                        {
                            //curEvent.psObject.transform.parent = transform.parent;
                            curEvent.psObject.transform.localPosition = rootTransform.position;
                            curEvent.psObject.transform.localRotation = Quaternion.identity;
                            //curEvent.psObject.transform.localRotation = rootTransform.rotation;
                            curEvent.psObject.transform.SetParent(transform.parent, false);
                            if (curEvent.locky == "true")
                            {
                                curEvent.psObject.transform.localRotation = Quaternion.identity;
                                curEvent.psObject.transform.localPosition = new Vector3(rootTransform.position.x, 0.0f, rootTransform.position.z);
                            }

                        }
                        curEvent.psObject.transform.localScale = prefab.transform.localScale;
                        curEvent.psDuration = Util.ParticleSystemLength(curEvent.psObject.transform);

                        if (curEvent.particleAni != null && curEvent.particleAni.Length > 0)
                        {
                            Animator animator = curEvent.psObject.GetComponent<Animator>();
                            int curStateHash = Animator.StringToHash(curEvent.particleAni);
                            if (animator != null && animator.HasState(0, curStateHash))
                            {
                                animator.Play(curStateHash);
                            }
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

	public void ShowWeakpointDeadEffect(string wp)
	{
		WeakPointData rowData = StaticDataMgr.Instance.GetWeakPointData(wp);
		if(null!=rowData)
		{
			string effectNodeName = rowData.node;
			GameObject effectGo = Util.FindChildByName (gameObject, effectNodeName);
			string deadPrefabName = rowData.deadEffect;
			if(!string.IsNullOrEmpty(deadPrefabName))
			{
                GameObject effectObject = ResourceMgr.Instance.LoadAsset("effect/battle", deadPrefabName);//
                //GameObject effectObject = Instantiate(prefab) as GameObject;
				if (null != effectObject)
				{
					effectObject.transform.SetParent (effectGo.transform);
					effectObject.transform.localPosition = Vector3.zero;
				}
			}
		}
	}
}
