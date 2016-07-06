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
    public List<ActorEventData> activeEventList;
    public List<ActorEventData> waitEventList;

	public SimpleEffect shifaNodeEffect;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        actorEventService = ActorEventService.Instance;
        activeEventList = new List<ActorEventData>();
        waitEventList = new List<ActorEventData>();
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
                curEvent.cameraAni = srcEvent.cameraAni;
                curEvent.controllerName = srcEvent.controllerName;
                curEvent.ps = null;
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
        unit.State = UnitState.None;
        //BattleObject bo = ObjectDataMgr.Instance.GetBattleObject(guid);
        //if (bo != null)
        {
            gameObject.SetActive(true);
            GameObject slotNode = BattleController.Instance.GetSlotNode(camp, unit.pbUnit.slot, unit.isBoss);
            transform.position = slotNode.transform.position;
            transform.rotation = slotNode.transform.rotation;
            transform.localScale = slotNode.transform.localScale;
            gameObject.transform.SetParent(GameMain.Instance.transform);
            if (camp == UnitCamp.Enemy)
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
                    curEvent.psObject = GameObject.Instantiate(prefab);
                    Transform rootTransform = transform;
                    if (curEvent.rootNode != null && curEvent.rootNode.Length > 0)
                    {
                        GameObject rootParent = Util.FindChildByName(gameObject, curEvent.rootNode);
                        if (rootParent != null)
                        {
                            rootTransform = rootParent.transform;
                        }
                    }
                    curEvent.psObject.transform.parent = rootTransform;
                    curEvent.psObject.transform.localPosition = prefab.transform.position;
                    curEvent.psObject.transform.localRotation = prefab.transform.rotation;
                    curEvent.psObject.transform.localScale = prefab.transform.localScale;

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
				GameObject prefab = ResourceMgr.Instance.LoadAsset ("effect/battle",deadPrefabName);//
				GameObject effectObject = Instantiate (prefab) as GameObject;
				if (null != effectObject)
				{
					effectObject.transform.SetParent (effectGo.transform);
					effectObject.transform.localPosition = Vector3.zero;
				}
			}
		}
	}
}
