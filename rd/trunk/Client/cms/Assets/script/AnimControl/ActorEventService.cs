using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;

public class ActorEventData
{
    public string id;
    public int state;
    public int actorDelay;
    //motion
    public string motionKey;
    public string motionValue;
    //particle
    public string particleAsset;
    public string particleBundle;
    public string particleAni;
    public string particleParent;
    //camera
    public string cameraAni;
    //controller
    public string controllerName;

    //state data
    public GameObject psObject;
    public ParticleSystem ps;
    public float triggerTime;
    public string rootNode;
}

public class ActorEventService
{
    private Dictionary<string, ActorEventData> eventList;

    static ActorEventService mInst = null;
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

    public void Init()
    {
        eventList = new Dictionary<string, ActorEventData>();
        loadEvent("animation.xml");
    }

    public bool GetEvent(string id, out ActorEventData eventData)
    {
        if (eventList.TryGetValue(id, out eventData))
        {
            return true;
        }
        return false;
    }

    private void loadEvent(string filename)
    {
        string filepath = Path.Combine(Util.StaticDataPath, filename);

        if(File.Exists (filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList=xmlDoc.SelectSingleNode("AniList").ChildNodes;
            foreach (XmlElement xe in nodeList)
            {
                ActorEventData eventData = new ActorEventData();
                eventData.id = xe.GetAttribute("id");
                eventList.Add(eventData.id, eventData);

                eventData.state = int.Parse(xe.GetAttribute("state"));
                eventData.actorDelay = 0;
                string actorDelay = xe.GetAttribute("action_delay");
                if (actorDelay.Length > 0)
                    eventData.actorDelay = int.Parse(xe.GetAttribute("action_delay"));
                
                foreach (XmlElement x1 in xe.ChildNodes)
                {
                    if (x1.Name == "Motion")
                    {
                        eventData.motionKey = x1.GetAttribute("key");
                        eventData.motionValue = x1.GetAttribute("value");
                    }
                    else if (x1.Name == "Particle")
                    {
                        eventData.particleAsset = x1.GetAttribute("asset");
                        eventData.particleBundle = x1.GetAttribute("bundle");
                        eventData.particleAni = x1.GetAttribute("ani");
                        eventData.particleParent = x1.GetAttribute("parent");
                    }
                    else if (x1.Name == "Camera")
                    {
                        eventData.cameraAni = x1.GetAttribute("ani");
                    }
                    else if (x1.Name == "Controller")
                    {
                        eventData.controllerName = x1.GetAttribute("name");
                    }
                }

            }
        }
    }
}
