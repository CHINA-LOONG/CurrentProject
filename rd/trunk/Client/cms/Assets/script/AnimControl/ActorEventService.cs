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
    //public string particleBundle;
    public string particleAni;
    public string particleParent;
    public string locky;
    public string attach;
    public string ignoreRot;

    //state data
    public GameObject psObject;
    public float psDuration;
    
    public object Clone()
    {
        ActorParticleData pData = new ActorParticleData();
        pData.triggerTime = triggerTime;
        pData.particleAsset = particleAsset;
        //pData.particleBundle = particleBundle;
        pData.particleAni = particleAni;
        pData.particleParent = particleParent;
        pData.locky = locky;
        pData.attach = attach;
        pData.ignoreRot = ignoreRot;

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
public class ActorWpStateData
{
    public float triggerTime;
    public string wpName;
    public int state;
}
//---------------------------------------------------------------------------------------------
public class ActorSpeedData
{
    public float triggerTime;
    public float duration;
    public float speedRatio;
}
//---------------------------------------------------------------------------------------------
public class ActorEventData
{
    public string id;
    public int state;
    public int actorDelay;
    public string finishEvent;

    public List<ActorMotionData> actorMotionSequence = new List<ActorMotionData>();
    public List<ActorParticleData> actorParticleSequence = new List<ActorParticleData>();
    public List<ActorCameraData> actorCameraSequence = new List<ActorCameraData>();
    public List<ActorControllerData> actorControllerSequence = new List<ActorControllerData>();
    public List<ActorAudioData> actorAudioSequence = new List<ActorAudioData>();
    public List<ActorMeshData> actorMeshSequence = new List<ActorMeshData>();
    public List<ActorWpStateData> actorWpStateSequence = new List<ActorWpStateData>();
    public List<ActorSpeedData> actorSpeedSequence = new List<ActorSpeedData>();

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
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(filepath, settings);
            xmlDoc.Load(reader);
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
                    eventData.id = aniNode.GetAttribute("id");
                    eventData.finishEvent = aniNode.GetAttribute("finish_event");
                    eventData.state = int.Parse(aniNode.GetAttribute("state"));
                    eventData.actorDelay = 0;
                    string actorDelay = aniNode.GetAttribute("action_delay");
                    if (actorDelay.Length > 0)
                    {
                        eventData.actorDelay = int.Parse(aniNode.GetAttribute("action_delay"));
                    }
					try
					{

                    	eventGroupData.actorEventList.Add(eventData.id, eventData);
					}
					catch
					{
						Logger.LogError("animation.xml repeat key = " + eventData.id);
					}

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
                                //particleData.particleBundle = particleNode.GetAttribute("bundle");
                                particleData.particleAni = particleNode.GetAttribute("ani");
                                particleData.particleParent = particleNode.GetAttribute("parent");
                                particleData.locky = particleNode.GetAttribute("locky");
                                particleData.attach = particleNode.GetAttribute("attach");
                                particleData.ignoreRot = particleNode.GetAttribute("ignore_rot");
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
                                str = cameraNode.GetAttribute("duration");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    cameraData.duration = float.Parse(str);
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
                        else if (x1.Name == "WpStateList")
                        {
                            foreach (XmlElement wpNode in x1.ChildNodes)
                            {
                                ActorWpStateData wpStateData = new ActorWpStateData();
                                string str = wpNode.GetAttribute("time");
                                wpStateData.triggerTime = 0.0f;
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    wpStateData.triggerTime = float.Parse(str);
                                }
                                wpStateData.wpName = wpNode.GetAttribute("name");
                                wpStateData.state = 0;
                                str = wpNode.GetAttribute("state");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    wpStateData.state = int.Parse(str);
                                }
                                eventData.actorWpStateSequence.Add(wpStateData);
                            }
                        }
                        else if (x1.Name == "SpeedList")
                        {
                            foreach (XmlElement speedNode in x1.ChildNodes)
                            {
                                ActorSpeedData speedData = new ActorSpeedData();
                                string str = speedNode.GetAttribute("time");
                                speedData.triggerTime = 0.0f;
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    speedData.triggerTime = float.Parse(str);
                                }
                                speedData.speedRatio = 1.0f;
                                str = speedNode.GetAttribute("speed_ratio");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    speedData.speedRatio = float.Parse(str);
                                }
                                speedData.duration = -1.0f;
                                str = speedNode.GetAttribute("duration");
                                if (string.IsNullOrEmpty(str) == false)
                                {
                                    speedData.duration = float.Parse(str);
                                }
                                eventData.actorSpeedSequence.Add(speedData);
                            }
                        }
                    }
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void AddResourceGroup(string groupName)
    {
        ResourceMgr resMgr = ResourceMgr.Instance;

        ActorEventGroupData groupData;
        if (actorEventGroupList.TryGetValue(groupName, out groupData) == true)
        {
            var eventItr = groupData.actorEventList.GetEnumerator();
            while (eventItr.MoveNext())
            {
                ActorEventData curEventData = eventItr.Current.Value;
                int particleCount = curEventData.actorParticleSequence.Count;

                for (int i = 0; i < particleCount; ++i)
                {
                    if (string.IsNullOrEmpty(curEventData.actorParticleSequence[i].particleAsset) == false)
                        resMgr.AddAssetRequest(new AssetRequest(curEventData.actorParticleSequence[i].particleAsset));
                }
            }
        }
    }
    //---------------------------------------------------------------------------------------------
}
