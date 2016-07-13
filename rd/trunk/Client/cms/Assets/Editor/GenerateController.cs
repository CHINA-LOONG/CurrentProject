using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

public class GenerateAnimatorController
{
    static List<string> motionNameList = new List<string>();
    static List<string> paths = new List<string>();

    struct AnimatorData
    {
        public string aniKey;
        public AnimatorState aniState;
    }
    /// </summary>
    [MenuItem("Builder/Build Animator Controller")]
    public static void BuildAssetResource()
    {
		motionNameList.Clear ();
		paths.Clear ();

        var controllerDirPath = EditorUtility.OpenFolderPanel("选择读取路径", @"C:/Users/The Second Lock/Desktop/Project/rd/trunk/Client/cms/Assets/SourceAsset/importModels/MonsterModels", "");
        if (string.IsNullOrEmpty(controllerDirPath))
            return;

        string controllerDir = controllerDirPath;
        int subIndex = controllerDirPath.IndexOf("Assets");
        controllerDir = controllerDirPath.Substring(subIndex);
        Recursive(controllerDir);

        string controllerPath = EditorUtility.SaveFilePanel("文件名", "", ".controller", "controller");
        subIndex = controllerPath.IndexOf("Assets");
        controllerPath = controllerPath.Substring(subIndex);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        
        var rootStateMachine = controller.layers[0].stateMachine;

        AnimatorData daijiState = new AnimatorData();
        AnimatorData siwangState = new AnimatorData();
        AnimatorData shengliState = new AnimatorData();
        AnimatorData shoukongState = new AnimatorData();
        AnimatorData wugongState = new AnimatorData();
        AnimatorData fagongState = new AnimatorData();
        AnimatorData fangyuState = new AnimatorData();
        AnimatorData dazhaoxuanyaoState = new AnimatorData();
        AnimatorData dazhaoState = new AnimatorData();
        AnimatorData toulanState = new AnimatorData();
        AnimatorData shoujiState = new AnimatorData();
        AnimatorData paoluState = new AnimatorData();

        AnimationEvent newEvent = new AnimationEvent();
        string stateName;
        for (int index = 0; index < motionNameList.Count; ++index)
        {
            string name = motionNameList[index];
            AnimationClip newClip = AssetDatabase.LoadAssetAtPath(name, typeof(AnimationClip)) as AnimationClip;            
          
            if (newClip == null)
                continue;
            stateName = newClip.name;

            //newClip.AddEvent();
            
            //create state, param and trans 
            AnimatorState curState = rootStateMachine.AddState(stateName);
            if (stateName!="daiji")
            {
                controller.AddParameter(stateName, AnimatorControllerParameterType.Bool);
            }           

            if (name.Contains("daiji") == true)
            {
                daijiState.aniState = curState;
                daijiState.aniKey = stateName;
                daijiState.aniState.motion = newClip;
            }
            else if (name.Contains("siwang") == true)
            {
                siwangState.aniState = curState;
                siwangState.aniKey = stateName;
                siwangState.aniState.motion = newClip;
                newEvent.functionName = "OnSiwangEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("shengli") == true)
            {
                shengliState.aniState = curState;
                shengliState.aniKey = stateName;
                shengliState.aniState.motion = newClip;
                newEvent.functionName = "OnShengliEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("shoukong") == true)
            {
                shoukongState.aniState = curState;
                shoukongState.aniKey = stateName;
                shoukongState.aniState.motion = newClip;
                newEvent.functionName = "OnDaShoukongEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("wugong") == true)
            {
                wugongState.aniState = curState;
                wugongState.aniKey = stateName;
                wugongState.aniState.motion = newClip;
                newEvent.functionName = "OnWuGongEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("fagong") == true)
            {
                fagongState.aniState = curState;
                fagongState.aniKey = stateName;
                fagongState.aniState.motion = newClip;
                newEvent.functionName = "OnFaGongEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("fangyu") == true)
            {
                fangyuState.aniState = curState;
                fangyuState.aniKey = stateName;
                fangyuState.aniState.motion = newClip;
                newEvent.functionName = "OnFangyuEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("dazhaoxuanyao") == true)
            {
                dazhaoxuanyaoState.aniState = curState;
                dazhaoxuanyaoState.aniKey = stateName;
                dazhaoxuanyaoState.aniState.motion = newClip;
                newEvent.functionName = "OnDaZhaoxuanyaoEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("dazhao") == true && name.Contains("dazhaoxuanyao") == false)
            {
                dazhaoState.aniState = curState;
                dazhaoState.aniKey = stateName;
                dazhaoState.aniState.motion = newClip;
                newEvent.functionName = "OnDaZhaoEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[]{newEvent});
            }
            else if (name.Contains("toulan") == true)
            {
                toulanState.aniState = curState;
                toulanState.aniKey = stateName;
                toulanState.aniState.motion = newClip;
                newEvent.functionName = "OnToulanEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("shouji") == true)
            {
                shoujiState.aniState = curState;
                shoujiState.aniKey = stateName;
                shoujiState.aniState.motion = newClip;
                newEvent.functionName = "OnShoujiEnd";
                newEvent.time = newClip.length;
                AnimationUtility.SetAnimationEvents(newClip, new AnimationEvent[] { newEvent });
            }
            else if (name.Contains("paolu") == true)
            {
                paoluState.aniState = curState;
                paoluState.aniKey = stateName;
                paoluState.aniState.motion = newClip;
            }
        }

        //add trans
        
        //any->siwang        

        AnimatorStateTransition tosiwangTrans = rootStateMachine.AddAnyStateTransition(siwangState.aniState);        
        tosiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);


        AnimatorStateTransition toshengliTrans = rootStateMachine.AddAnyStateTransition(shengliState.aniState);
        toshengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shengliState.aniKey);
        toshengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        toshengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shoukongState.aniKey);

        AnimatorStateTransition toshoukong = rootStateMachine.AddAnyStateTransition(shoukongState.aniState);
        toshoukong.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        toshoukong.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey); 
       


        
        //daiji->?
        AnimatorStateTransition daijiToWugongTrans = daijiState.aniState.AddTransition(wugongState.aniState);
        daijiToWugongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, wugongState.aniKey);
        daijiToWugongTrans.hasExitTime = false;
        AnimatorStateTransition daijiToFagongTrans = daijiState.aniState.AddTransition(fagongState.aniState);
        daijiToFagongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, fagongState.aniKey);
        daijiToFagongTrans.hasExitTime = false;
        AnimatorStateTransition daijiToFangyuTrans = daijiState.aniState.AddTransition(fangyuState.aniState);
        daijiToFangyuTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, fangyuState.aniKey);
        daijiToFangyuTrans.hasExitTime = false;
        AnimatorStateTransition daijiToDazhaoxuanyaoTrans = daijiState.aniState.AddTransition(dazhaoxuanyaoState.aniState);
        daijiToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoxuanyaoState.aniKey);
        daijiToDazhaoxuanyaoTrans.hasExitTime = false;
        AnimatorStateTransition daijiToDazhaoTrans = daijiState.aniState.AddTransition(dazhaoState.aniState);
        daijiToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoState.aniKey);
        daijiToDazhaoTrans.hasExitTime = false;

        AnimatorStateTransition daijiToToulanTrans = daijiState.aniState.AddTransition(toulanState.aniState);
        daijiToToulanTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, toulanState.aniKey);
        daijiToToulanTrans.hasExitTime = false;

        AnimatorStateTransition daijiToShoujiTrans = daijiState.aniState.AddTransition(shoujiState.aniState);
        daijiToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        daijiToShoujiTrans.hasExitTime = false;

        AnimatorStateTransition daijiToPaoluTrans = daijiState.aniState.AddTransition(paoluState.aniState);
        daijiToPaoluTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, paoluState.aniKey);
        daijiToPaoluTrans.duration = 0.0f;
        daijiToPaoluTrans.hasExitTime = false;
        
        //?->daiji
        AnimatorStateTransition wugongToDaijiTrans = wugongState.aniState.AddTransition(daijiState.aniState);
        wugongToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition fagongToDaijiTrans = fagongState.aniState.AddTransition(daijiState.aniState);
        fagongToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition fangyuToDaijiTrans = fangyuState.aniState.AddTransition(daijiState.aniState);
        fangyuToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition dazhaoxuanyaoToDaijiTrans = dazhaoxuanyaoState.aniState.AddTransition(daijiState.aniState);
        dazhaoxuanyaoToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition dazhaoToDaijiTrans = dazhaoState.aniState.AddTransition(daijiState.aniState);
        dazhaoToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition toulanToDaijiTrans = toulanState.aniState.AddTransition(daijiState.aniState);
        toulanToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition shoujiToDaijiTrans = shoujiState.aniState.AddTransition(daijiState.aniState);
        shoujiToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition paoluToDaijiTrans = paoluState.aniState.AddTransition(daijiState.aniState);
        paoluToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, paoluState.aniKey);
        paoluToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        paoluToDaijiTrans.hasExitTime = true;
        
        AnimatorStateTransition siwangToDajiTrans = siwangState.aniState.AddTransition(daijiState.aniState);         
        siwangToDajiTrans.hasExitTime = true;
        AnimatorStateTransition shengliToDajiTrans = shengliState.aniState.AddTransition(daijiState.aniState);
        shengliToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        shengliToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shengliState.aniKey); 
        shengliToDajiTrans.hasExitTime = true;
        AnimatorStateTransition shoukongToDajiTrans = shoukongState.aniState.AddTransition(daijiState.aniState);
        shoukongToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shoukongState.aniKey);
        shoukongToDajiTrans.hasExitTime = true;
    }

    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".fbx") || ext.Equals(".FBX"))
            {
                if (filename.Contains("moxing") == true)
                    continue;

                motionNameList.Add(filename.Replace('\\', '/'));
            }
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }
}