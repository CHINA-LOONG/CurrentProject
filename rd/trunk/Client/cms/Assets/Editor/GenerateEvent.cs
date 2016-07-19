using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GenerateEvent 
{
    [MenuItem("Builder/Build AnimationEvent")]
    static void Execute()
    {

        ModelImporterClipAnimation newAnimation = new ModelImporterClipAnimation();
        //List<string> lstAnimName = new List<string>();
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets))
        {
            if (!(o is GameObject))
                continue;
            //if (o.name.Contains("@"))
            //    continue;
            //if (o.name.Contains("anim") || o.name.Contains("ANIM"))
            //    continue;
            //if (o.name.Contains("meta"))
            //    continue;
            GameObject charFbx = (GameObject)o;
            //Animation anic = charFbx.GetComponent<Animation>();
            //anic.wrapMode = WrapMode.Loop;

            AnimationClip[] clips = AnimationUtility.GetAnimationClips(charFbx.gameObject);
            foreach (AnimationClip clip in clips)
            {
                if (clip.name.Contains("Take 0"))
                    continue;
                List<AnimationEvent> events = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(clip));
                AnimationEvent startEvent = new AnimationEvent();
                startEvent.time = 0.01f;
                if (clip.name=="dazhao")
                {
                    startEvent.functionName = "OnDaZhaoEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "dazhaoxuanyao")
                {
                    startEvent.functionName = "OnDaZhaoxuanyaoEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "fagong")
                {
                    startEvent.functionName = "OnFaGongEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "shouji")
                {
                    startEvent.functionName = "OnShoujiEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "siwang")
                {
                    startEvent.functionName = "OnDeadEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "toulan")
                {
                    startEvent.functionName = "OnLazyEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "wugong")
                {
                    startEvent.functionName = "OnWuGongEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "chuchang")
                {
                    startEvent.functionName = "OnChuChangEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }
                else if (clip.name == "fangyu")
                {
                    startEvent.functionName = "OnFangyuEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(clip, events.ToArray());
                }

            }
        }
        Logger.Log("Event ok!");
    }
}
