using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

    //public SimpleEffect shifaNodeEffect;
    private Quaternion targetRot = Quaternion.identity;
    private float lastUpdateTime;

	public WeakPointGroup wpGroup = null;

    //---------------------------------------------------------------------------------------------
    void Awake()
    {
        actorEventService = ActorEventService.Instance;
        lastUpdateTime = 0.0f;
    }
    //---------------------------------------------------------------------------------------------
    public void TriggerEvent(string eventID, float triggerTime, string rootNode)
    {
        ActorEventData srcEvent;
        if (actorEventService.GetEvent(eventID, out srcEvent))
        {
            //trigger event
            if (srcEvent.state == 0)
            {
                ActorEventData curEvent = new ActorEventData();
                curEvent.triggerTime = triggerTime;
                curEvent.rootNode = rootNode;
                curEvent.actorDelay = srcEvent.actorDelay;
                curEvent.id = srcEvent.id;
                curEvent.finishEvent = srcEvent.finishEvent;
                //NOTE: refrence
                curEvent.actorMotionSequence = srcEvent.actorMotionSequence;
                curEvent.actorCameraSequence = srcEvent.actorCameraSequence;
                curEvent.actorControllerSequence = srcEvent.actorControllerSequence;
                curEvent.actorMeshSequence = srcEvent.actorMeshSequence;
                curEvent.actorWpStateSequence = srcEvent.actorWpStateSequence;
                curEvent.actorSpeedSequence = srcEvent.actorSpeedSequence;
                //copy
                curEvent.actorParticleSequence = new List<ActorParticleData>();
                for (int i = 0; i < srcEvent.actorParticleSequence.Count; ++i)
                {
                    curEvent.actorParticleSequence.Add((ActorParticleData)srcEvent.actorParticleSequence[i].Clone());
                }

                curEvent.actorAudioSequence = srcEvent.actorAudioSequence;


                waitEventList.Add(curEvent);
            }
            //remove event
            else
            {
                if (string.IsNullOrEmpty(srcEvent.finishEvent) == false)
                {
                    bool findFinishEvent = false;
                    ActorEventData curEventData;
                    //find waitEventList first,since,may remove one event triggered in same frame
                    for (int i = waitEventList.Count - 1; i >= 0; --i)
                    {
                        curEventData = waitEventList[i];
                        if (curEventData.id == srcEvent.finishEvent)
                        {
                            findFinishEvent = true;
                            waitEventList.RemoveAt(i);
                            //TODO:not trigger or trigger and delete immediate
                            break;
                        }
                    }

                    if (findFinishEvent == false)
                    {
                        for (int i = activeEventList.Count - 1; i >= 0; --i)
                        {
                            curEventData = activeEventList[i];
                            if (curEventData.id == srcEvent.finishEvent)
                            {
                                int particleCount = curEventData.actorParticleSequence.Count;
                                for (int index = 0; index < particleCount; ++index)
                                {
                                    if (curEventData.actorParticleSequence[index].psObject != null)
                                    {
                                        ResourceMgr.Instance.DestroyAsset(curEventData.actorParticleSequence[index].psObject);
                                    }
                                }
                                //NOTE: xw said only need to destroy particle

                                activeEventList.RemoveAt(i);
                                //TODO:need break? or delete all same name event
                            }
                        }
                    }
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnEnterField()
    {
        unit.State = UnitState.None;
        unit.backUp = false;
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
           // if (camp == UnitCamp.Enemy)
           // {
               // GameEventMgr.Instance.FireEvent<BattleObject>(GameEventList.LoadBattleObjectFinished, this);
           // }
        }

        unit.RecalcCurActionOrder();

        Logger.LogFormat("Unit {0} guid:{1} has entered field", name, guid);
    }
    //---------------------------------------------------------------------------------------------
    public void SetTargetRotate(Quaternion rot, bool reset)
    {
        if (type == BattleObjectType.Scene || unit.stun > 0)
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
            //ClearEvent();
            if (unit.State != UnitState.Dead)
            {
                unit.State = UnitState.None;
            }

            gameObject.transform.localPosition = new Vector3(0.0f, 10000f, 0.0f);
            //gameObject.SetActive(false);
            Logger.LogFormat("Unit {0} guid:{1} has exited field", name, guid);
        }
    }
    //---------------------------------------------------------------------------------------------
    void LateUpdate()
    {
        //update event list
        float curTime = Time.time;
        ActorEventData curEventData;
        for (int i = waitEventList.Count - 1; i >= 0; --i)
        {
            curEventData = waitEventList[i];
            if (curEventData.triggerTime <= curTime)
            {
                activeEventList.Add(curEventData);
                if (curEventData.actorDelay > 0 && BattleController.Instance.Process != null)
                {
                    BattleController.Instance.Process.ActionDelayTime = curEventData.actorDelay;
                }
                waitEventList.RemoveAt(i);
            }
        }

        //update sequence
        int count = 0;
        bool finish = true;
        float sequenceTriggerTime = 0.0f;
        for (int i = activeEventList.Count - 1; i >= 0; --i)
        {
            finish = true;
            curEventData = activeEventList[i];
            if (aniControl != null)
            {
                //TODO: use template
                //update controller
                count = curEventData.actorControllerSequence.Count;
                ActorControllerData curControllerData;
                for (int index = 0; index < count; ++index)
                {
                    curControllerData = curEventData.actorControllerSequence[index];
                    sequenceTriggerTime = curEventData.triggerTime + curControllerData.triggerTime;
                    if (sequenceTriggerTime < lastUpdateTime)
                    {
                        continue;
                    }
                    if (sequenceTriggerTime >= curTime)
                    {
                        finish = false;
                        continue;
                    }
                    finish = false;

                    if (string.IsNullOrEmpty(curControllerData.controllerName) == false)
                    {
                        aniControl.SetController(curControllerData.controllerName);
                    }
                }

                //update motion 
                count = curEventData.actorMotionSequence.Count;
                ActorMotionData curMotionData;
                for (int index = 0; index < count; ++index)
                {
                    curMotionData = curEventData.actorMotionSequence[index];
                    sequenceTriggerTime = curEventData.triggerTime + curMotionData.triggerTime;
                    if (sequenceTriggerTime < lastUpdateTime)
                    {
                        continue;
                    }
                    if (sequenceTriggerTime >= curTime)
                    {
                        finish = false;
                        continue;
                    }
                    finish = false;

                    if (string.IsNullOrEmpty(curMotionData.motionKey) == false)
                    {
                        aniControl.SetBool(curMotionData.motionKey, bool.Parse(curMotionData.motionValue));
                    }
                }
            }

            //update particle
            count = curEventData.actorParticleSequence.Count;
            ActorParticleData curParticleData;
            for (int index = 0; index < count; ++index)
            {
                curParticleData = curEventData.actorParticleSequence[index];
                sequenceTriggerTime = curEventData.triggerTime + curParticleData.triggerTime;
                if (sequenceTriggerTime < lastUpdateTime)
                {
                    continue;
                }
                if (sequenceTriggerTime >= curTime)
                {
                    finish = false;
                    continue;
                }
                finish = false;

                if (string.IsNullOrEmpty(curParticleData.particleAsset) == false)
                {
                    curParticleData.psObject = ResourceMgr.Instance.LoadAsset(curParticleData.particleAsset);
                    if (curParticleData.psObject != null)
                    {
                        Transform rootTransform = transform;
                        if (string.IsNullOrEmpty(curEventData.rootNode) == false)
                        {
                            GameObject rootParent = Util.FindChildByName(gameObject, curEventData.rootNode);
                            if (rootParent != null)
                            {
                                rootTransform = rootParent.transform;
                            }
                        }

                        if (curParticleData.particleParent != null && curParticleData.particleParent.Length > 0)
                        {
                            if (curEventData.rootNode != null)
                            {
                                Logger.LogWarning("weak point is ignored since event configs the parent node");
                            }
                            //Transform parentNode = transform.Find(curEvent.particleParent);
                            GameObject parentNode = Util.FindChildByName(gameObject, curParticleData.particleParent);
                            if (parentNode != null)
                            {
                                rootTransform = parentNode.transform;
                            }
                        }

                        if (curParticleData.attach == "true")
                        {
                            //curParticleData.psObject.transform.localRotation = prefab.transform.localRotation;
                            curParticleData.psObject.transform.localRotation = Quaternion.identity;
                            curParticleData.psObject.transform.SetParent(rootTransform, false);
                            //NOTE: xw said if attach, ignore lock
                        }
                        else
                        {
                            //curEvent.psObject.transform.parent = transform.parent;
                            curParticleData.psObject.transform.localPosition = rootTransform.position;
                            curParticleData.psObject.transform.localRotation = Quaternion.identity;
                            //curParticleData.psObject.transform.localRotation = rootTransform.rotation;
                            curParticleData.psObject.transform.SetParent(transform.parent, false);
                            if (curParticleData.locky == "true")
                            {
                                curParticleData.psObject.transform.localRotation = Quaternion.identity;
                                curParticleData.psObject.transform.localPosition = new Vector3(rootTransform.position.x, BattleController.floorHeight + BattleConst.floorHeight, rootTransform.position.z);
                            }

                        }
                        curParticleData.psDuration = Util.ParticleSystemLength(curParticleData.psObject.transform);

                        if (curParticleData.particleAni != null && curParticleData.particleAni.Length > 0)
                        {
                            Animator animator = curParticleData.psObject.GetComponent<Animator>();
                            int curStateHash = Animator.StringToHash(curParticleData.particleAni);
                            if (animator != null && animator.HasState(0, curStateHash))
                            {
                                animator.Play(curStateHash);
                            }
                        }
                    }
                }
            }

            //update camera
            count = curEventData.actorCameraSequence.Count;
            ActorCameraData curCameraData;
            for (int index = 0; index < count; ++index)
            {
                curCameraData = curEventData.actorCameraSequence[index];
                sequenceTriggerTime = curEventData.triggerTime + curCameraData.triggerTime;
                if (sequenceTriggerTime < lastUpdateTime)
                {
                    continue;
                }
                if (sequenceTriggerTime >= curTime)
                {
                    finish = false;
                    continue;
                }
                finish = false;

                //TODO: animate battlecamera only?
                GameObject cameraRoot = BattleCamera.Instance.gameObject;
                if (string.IsNullOrEmpty(curCameraData.parent) == false)
                {
                    GameObject cameraParentTarget = null;
                    if (curCameraData.parent == "default")
                    {
                        cameraParentTarget = BattleController.Instance.GetDefaultCameraNode().gameObject;
                    }
                    else
                    {
                        cameraParentTarget = Util.FindChildByName(gameObject, curCameraData.parent);
                    }
                    if (cameraParentTarget != null)
                    {
                        if (string.IsNullOrEmpty(curCameraData.isMover) || curCameraData.isMover != "true")
                        {
                            Transform parentTransform = cameraParentTarget.transform;
                            cameraRoot.transform.localPosition = parentTransform.position;
                            cameraRoot.transform.localRotation = parentTransform.rotation;
                            cameraRoot.transform.SetParent(transform.parent, false);
                        }
                        else 
                        {
                            cameraRoot.transform.DOMove(cameraParentTarget.transform.position, curCameraData.duration);
                            cameraRoot.transform.DORotate(cameraParentTarget.transform.rotation.eulerAngles, curCameraData.duration);
                        }
                    }
                }

                if (string.IsNullOrEmpty(curCameraData.cameraAni) == false)
                {
                    Animator animator = cameraRoot.GetComponent<Animator>();
                    if (animator != null)
                    {
                        int curAniHash = 0;
                        for (int layerIndex = 0; layerIndex < 2; ++layerIndex)
                        {
                            curAniHash = Animator.StringToHash(curCameraData.cameraAni);
                            if (animator.HasState(layerIndex, curAniHash))
                            {
                                animator.Play(curAniHash);
                                break;
                            }
                        }
                    }
                }
            }

            //update mesh
            count = curEventData.actorMeshSequence.Count;
            ActorMeshData curActorMeshData;
            for (int index = 0; index < count; ++index)
            {
                curActorMeshData = curEventData.actorMeshSequence[index];
                sequenceTriggerTime = curEventData.triggerTime + curActorMeshData.triggerTime;
                if (sequenceTriggerTime < lastUpdateTime)
                {
                    continue;
                }
                if (sequenceTriggerTime >= curTime)
                {
                    finish = false;
                    continue;
                }
                finish = false;

                if (string.IsNullOrEmpty(curActorMeshData.mesh))
                {
                    continue;
                }
                GameObject mesh = Util.FindChildByName(transform.gameObject, curActorMeshData.mesh);
                mesh.SetActive(curActorMeshData.state == "show");
            }

            //update audio
            count = curEventData.actorAudioSequence.Count;
            ActorAudioData curAudioData;
            for (int index = 0; index < count; ++index)
            {
                curAudioData = curEventData.actorAudioSequence[index];
                sequenceTriggerTime = curEventData.triggerTime + curAudioData.triggerTime;
                if (sequenceTriggerTime < lastUpdateTime)
                {
                    continue;
                }
                if (sequenceTriggerTime >= curTime)
                {
                    finish = false;
                    continue;
                }
                finish = false;

                if (string.IsNullOrEmpty(curAudioData.audioName))
                {
                    continue;
                }
                //Debug.LogError("Play Sound:\t" + curAudioData.audioName);
                AudioSystemMgr.Instance.PlaySoundByName(curAudioData.audioName);
            }

            //update wpstate
            count = curEventData.actorWpStateSequence.Count;
            ActorWpStateData curWpData = null;
            for (int index = 0; index < count; ++index)
            {
                curWpData = curEventData.actorWpStateSequence[index];
                sequenceTriggerTime = curEventData.triggerTime + curWpData.triggerTime;
                if (sequenceTriggerTime < lastUpdateTime)
                {
                    continue;
                }
                if (sequenceTriggerTime >= curTime)
                {
                    finish = false;
                    continue;
                }
                finish = false;

                if (string.IsNullOrEmpty(curWpData.wpName))
                {
                    continue;
                }

                wpGroup.ChangeState(curWpData.wpName, (WeakpointState)curWpData.state);
            }

            //update speed
            count = curEventData.actorSpeedSequence.Count;
            ActorSpeedData curSpeedData = null;
            for (int index = 0; index < count; ++index)
            {
                curSpeedData = curEventData.actorSpeedSequence[index];
                sequenceTriggerTime = curEventData.triggerTime + curSpeedData.triggerTime;
                if (sequenceTriggerTime < lastUpdateTime)
                {
                    continue;
                }
                if (sequenceTriggerTime >= curTime)
                {
                    finish = false;
                    continue;
                }
                finish = false;

                GameSpeedService.Instance.SetTmpSpeed(curSpeedData.speedRatio, curSpeedData.duration);
            }


            //check particle
            count = curEventData.actorParticleSequence.Count;
            for (int index = 0; index < count; ++index)
            {
                curParticleData = curEventData.actorParticleSequence[index];
                if (curParticleData.psObject != null)
                {
                    //TODO: replace Time.time to battle time
                    if (curParticleData.psDuration >= 0.0f && curTime - curEventData.triggerTime - curParticleData.triggerTime >= curParticleData.psDuration)
                    {
                        ResourceMgr.Instance.DestroyAsset(curParticleData.psObject);
                        curParticleData.psObject = null;
                        //activeEventList.RemoveAt(i);
                    }
                    else 
                    {
                        finish = false;
                    }
                }
            }

            //check audio
            //count = curEventData.actorAudioSequence.Count;
            //for (int index = 0; index < count; ++index)
            //{
            //    curAudioData = curEventData.actorAudioSequence[index];
            //    if (curAudioData.clip != null && curAudioData.clip.)
            //    {
            //        //TODO: replace Time.time to battle time
            //        if (curParticleData.psDuration >= 0.0f && curTime - curEventData.triggerTime - curParticleData.triggerTime >= curParticleData.psDuration)
            //        {
            //            ResourceMgr.Instance.DestroyAsset(curParticleData.psObject);
            //            curParticleData.psObject = null;
            //            //activeEventList.RemoveAt(i);
            //        }
            //    }
            //}

            if (finish == true)
            {
                activeEventList.RemoveAt(i);
            }
        }

        lastUpdateTime = curTime;

        //rotate unit if necessary
        if (type == BattleObjectType.Unit && transform.localRotation != targetRot)
        {
            float step = BattleConst.unitRotSpeed * Time.deltaTime;
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRot, step);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, step);
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
            int count = curEventData.actorParticleSequence.Count;
            ActorParticleData curParticleData;
            for (int index = 0; index < count; ++index)
            {
                curParticleData = curEventData.actorParticleSequence[index];
                if (curParticleData.psObject != null)
                {
                    ResourceMgr.Instance.DestroyAsset(curParticleData.psObject);
                    curParticleData.psObject = null;
                }
            }
        }
        activeEventList.Clear();
    }
    //---------------------------------------------------------------------------------------------
}
