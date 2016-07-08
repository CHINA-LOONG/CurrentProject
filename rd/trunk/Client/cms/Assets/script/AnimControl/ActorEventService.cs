using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

//---------------------------------------------------------------------------------------------
public class ActorEventGroupData
{
    public string groupID;
    public Dictionary<string, ActorEventData> actorEventList = new Dictionary<string,ActorEventData>();
}
//---------------------------------------------------------------------------------------------
public class ActorMotionData
{
    public float triggerTime;
    public string motionKey;
    public string motionID;
    public string motionValue;
}
//---------------------------------------------------------------------------------------------
public class ActorParticleData : ICloneable
{
    public float triggerTime;
    public string particleAsset;
    public string particleBundle;
    public string particleAni;
    public string particleParent;
    public string locky;
    public string attach;

    //state data
    public GameObject psObject;
    public float psDuration;
    
    public object Clone()
    {
        ActorParticleData pData = new ActorParticleData();
        pData.triggerTime = triggerTime;
        pData.particleAsset = particleAsset;
        pData.particleBundle = particleBundle;
        pData.particleAni = particleAni;
        pData.particleParent = particleParent;
        pData.locky = locky;
        pData.attach = attach;

        pData.psObject = null;
        pData.psDuration = 0.0f;

        return pData;
    }
}
//---------------------------------------------------------------------------------------------
public class ActorCameraData
{
    public float triggerTime;
    public string cameraAni;
    public string parent;
    public string isMover;
    public float duration;
    public int moverType;
}
//---------------------------------------------------------------------------------------------
public class ActorControllerData
{
    public float triggerTime;
    public string controllerName;
}
//---------------------------------------------------------------------------------------------
public class ActorAudioData : ICloneable
{
    public float triggerTime;
    public string audioName;
    //state data
    public AudioClip clip;

    public object Clone()
    {
        ActorAudioData aData = new ActorAudioData();
        aData.triggerTime = triggerTime;
        aData.audioName = audioName;

        aData.clip = null;

        return aData;
    }
}
//---------------------------------------------------------------------------------------------
public class ActorMeshData
{
    public float triggerTime;
    public string state;
    public string mesh;
}
//---------------------------------------------------------------------------------------------
public class ActorEventData
{
    public string id;
    public int state;
    public int actorDelay;
    public string finishEvent;

    public List<ActorMotionData> actorMotionSequence;// = new List<ActorMotionData>();
    public List<ActorParticleData> actorParticleSequence;// = new List<ActorParticleData>();
    public List<ActorCameraData> actorCameraSequence;// = new List<ActorCameraData>();
    public List<ActorControllerData> actorControllerSequence;// = new List<ActorControllerData>();
    public List<ActorAudioData> actorAudioSequence;// = new List<ActorAudioData>();
    public List<ActorMeshData> actorMeshSequence;// = new List<ActorMeshData>();

    //state data
    public float triggerTime;
    public string rootNode;
}
//---------------------------------------------------------------------------------------------
public class ActorEventService
{
    private Dictionary<string, ActorEventGroupData> actorEventGroupList = new Dictionary<string,ActorEventGroupData>();

    static ActorEventService mInst = null;
    //---------------------------------------------------------------------------------------------
    public static ActorEventService Instance
    {
        get
        {
            if (mInst == null)
            {
                //GameObject go = new GameObject("ActorEventService");
                //mInst = go.AddComponent<ActorEventService>();
                mInst = new ActorEventService();
            }
            return mInst;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void Init()
    {
        loadEvent("animation.xml");
    }
    //---------------------------------------------------------------------------------------------
    public bool GetEvent(string id, out ActorEventData eventData)
    {
        var itor = actorEventGroupList.GetEnumerator();
        while (itor.MoveNext())
        {
            if (itor.Current.Value.actorEventList.TryGetValue(id, out eventData))
            {
                return true;
            }
        }
        eventData = new ActorEventData();

        return false;
    }
    //---------------------------------------------------------------------------------------------
    private void loadEvent(string filename)
    {
        string filepath = Path.Combine(Util.StaticDataPath, filename);

        if(File.Exists (filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList=xmlDoc.SelectSingleNode("AniGroupList").ChildNodes;
            foreach (XmlElement groupNode in nodeList)
            {
                ActorEventGroupData eventGroupData = new ActorEventGroupData();
                eventGroupData.groupID = groupNode.GetAttribute("group");
                actorEventGroupList.Add(eventGroupData.groupID, eventGroupData);
                //parse ani data
                foreach (XmlElement aniNode in groupNode.ChildNodes)
                {
                    ActorEventData eventData = new ActorEventData();
                    eventData.actorMotionSequence = new List<ActorMotionData>();
                    eventData.actorParticleSequence = new List<ActorParticleData>();
                    eventData.actorCameraSequence = new List<ActorCameraData>();
                    eventData.actorControllerSequence = new List<ActorControllerData>();
                    eventData.actorAudioSequence = new List<ActorAudioData>();
                    eventData.actorMeshSequence = new List<ActorMeshData>();
                    eventData.id = aniNode.GetAttribute("id");
                    eventData.finishEvent = aniNode.GetAttribute("finish_event");
                    eventData.state = int.Parse(aniNode.GetAttribute("state"));
                    eventData.actorDelay = 0;
                    string actorDelay = aniNode.GetAttribute("action_delay");
                    if (actorDelay.Length > 0)
                    {
                        eventData.actorDelay = int.Parse(aniNode.GetAttribute("action_delay"));
                    }
                    eventGroupData.actorEventList.Add(eventData.id, eventData);

                    foreach (XmlElement x1 in aniNode.ChildNodes)
                    {
                        if (x1.Name == "MotionList")
                        {
                            foreach (XmlElement motionNode in x1.ChildNodes)
                            {
                                ActorMotionData motionData = new ActorMotionData();
                                motionData.triggerTime = 0;
                                string str = motionNode.GetAttribute("time");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    motionData.triggerTime = float.Parse(str);
                                }
                                motionData.motionKey = motionNode.GetAttribute("key");
                                motionData.motionValue = motionNode.GetAttribute("value");
                                motionData.motionID = motionNode.GetAttribute("ani");

                                eventData.actorMotionSequence.Add(motionData);
                            }
                        }
                        else if (x1.Name == "ParticleList")
                        {
                            foreach (XmlElement particleNode in x1.ChildNodes)
                            {
                                ActorParticleData particleData = new ActorParticleData();
                                particleData.triggerTime = 0;
                                string str = particleNode.GetAttribute("time");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    particleData.triggerTime = float.Parse(str);
                                }
                                particleData.particleAsset = particleNode.GetAttribute("asset");
                                particleData.particleBundle = particleNode.GetAttribute("bundle");
                                particleData.particleAni = particleNode.GetAttribute("ani");
                                particleData.particleParent = particleNode.GetAttribute("parent");
                                particleData.locky = particleNode.GetAttribute("locky");
                                particleData.attach = particleNode.GetAttribute("attach");
                                particleData.psObject = null;
                                particleData.psDuration = 0.0f;

                                eventData.actorParticleSequence.Add(particleData);
                            }
                        }
                        else if (x1.Name == "CameraMotionList")
                        {
                            foreach (XmlElement cameraNode in x1.ChildNodes)
                            {
                                ActorCameraData cameraData = new ActorCameraData();
                                cameraData.triggerTime = 0;
                                string str = cameraNode.GetAttribute("time");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    cameraData.triggerTime = float.Parse(str);
                                }
                                cameraData.cameraAni = cameraNode.GetAttribute("ani");
                                cameraData.parent = cameraNode.GetAttribute("parent");
                                cameraData.isMover = cameraNode.GetAttribute("is_mover");
                                cameraData.duration = 0.0f;
                                str = x1.GetAttribute("duration");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    cameraData.duration = float.Parse("duration");
                                }
                                cameraData.moverType = 0;
                                str = cameraNode.GetAttribute("mover_type");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    cameraData.moverType = int.Parse(str);
                                }

                                eventData.actorCameraSequence.Add(cameraData);
                            }
                        }
                        else if (x1.Name == "ControllerList")
                        {
                            foreach (XmlElement controllerNode in x1.ChildNodes)
                            {
                                ActorControllerData controllerData = new ActorControllerData();
                                string str = controllerNode.GetAttribute("time");
                                controllerData.triggerTime = 0.0f;
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    controllerData.triggerTime = float.Parse(str);
                                }
                                controllerData.controllerName = controllerNode.GetAttribute("name");
                                eventData.actorControllerSequence.Add(controllerData);
                            }
                        }
                        else if (x1.Name == "AudioList")
                        {
                            foreach (XmlElement audioNode in x1.ChildNodes)
                            {
                                ActorAudioData audioData = new ActorAudioData();
                                string str = audioNode.GetAttribute("time");
                                audioData.triggerTime = 0.0f;
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    audioData.triggerTime = float.Parse(str);
                                }
                                audioData.clip = null;
                                audioData.audioName = audioNode.GetAttribute("name");
                                eventData.actorAudioSequence.Add(audioData);
                            }
                        }
                        else if (x1.Name == "MeshStateList")
                        {
                            foreach (XmlElement audioNode in x1.ChildNodes)
                            {
                                ActorMeshData meshData = new ActorMeshData();
                                string str = audioNode.GetAttribute("time");
                                meshData.triggerTime = 0.0f;
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    meshData.triggerTime = float.Parse(str);
                                }
                                meshData.mesh = audioNode.GetAttribute("name");
                                meshData.state = audioNode.GetAttribute("state");
                                eventData.actorMeshSequence.Add(meshData);
                            }
                        }
                    }
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
