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
        bool isdazhao = false;//是否有大招
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
        AnimatorData paobuState = new AnimatorData();
        AnimatorData chuchangState = new AnimatorData();

        string stateName;
        for (int index = 0; index < motionNameList.Count; ++index)
        {
            string name = motionNameList[index];
            AnimationClip newClip = AssetDatabase.LoadAssetAtPath(name, typeof(AnimationClip)) as AnimationClip;
            if (newClip == null)
                continue;
            AnimationEvent startEvent = new AnimationEvent();
            List<AnimationEvent> events = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(newClip));
            startEvent.time = 0.01f;
            stateName = newClip.name;
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
                AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(newClip);
                clipSetting.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(newClip, clipSetting);
            }
            else if (name.Contains("siwang") == true)
            {
                siwangState.aniState = curState;
                siwangState.aniKey = stateName;
                siwangState.aniState.motion = newClip;
            }
            else if (name.Contains("shengli") == true)
            {
                shengliState.aniState = curState;
                shengliState.aniKey = stateName;
                shengliState.aniState.motion = newClip;
                AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(newClip);
                clipSetting.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(newClip, clipSetting);
            }
            else if (name.Contains("shoukong") == true)
            {
                shoukongState.aniState = curState;
                shoukongState.aniKey = stateName;
                shoukongState.aniState.motion = newClip;
                AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(newClip);
                clipSetting.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(newClip, clipSetting);
            }
            else if (name.Contains("wugong") == true)
            {
                wugongState.aniState = curState;
                wugongState.aniKey = stateName;
                wugongState.aniState.motion = newClip;
                if (events.Count < 0)
                {
                    startEvent.functionName = "OnWuGongEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
                }                
            }
            else if (name.Contains("fagong") == true)
            {
                fagongState.aniState = curState;
                fagongState.aniKey = stateName;
                fagongState.aniState.motion = newClip;
                if (events.Count < 0)
                {
                    startEvent.functionName = "OnFaGongEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
                }
            }
            else if (name.Contains("fangyu") == true)
            {
                fangyuState.aniState = curState;
                fangyuState.aniKey = stateName;
                fangyuState.aniState.motion = newClip;
                AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(newClip);
                clipSetting.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(newClip, clipSetting);
            }
            else if (name.Contains("dazhaoxuanyao") == true)
            {
                dazhaoxuanyaoState.aniState = curState;
                dazhaoxuanyaoState.aniKey = stateName;
                dazhaoxuanyaoState.aniState.motion = newClip;
                if (events.Count < 0)
                {
                    startEvent.functionName = "OnDaZhaoxuanyaoEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
                }
            }
            else if (name.Contains("dazhao") == true)
            {
                dazhaoState.aniState = curState;
                dazhaoState.aniKey = stateName;
                dazhaoState.aniState.motion = newClip;
                isdazhao = true;
                if (events.Count < 0)
                {
                    startEvent.functionName = "OnDaZhaoEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
                }
            }
            else if (name.Contains("toulan") == true)
            {
                toulanState.aniState = curState;
                toulanState.aniKey = stateName;
                toulanState.aniState.motion = newClip;
                if (events.Count < 0)
                {
                    startEvent.functionName = "OnLazyEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
                }
            }
            else if (name.Contains("shouji") == true)
            {
                shoujiState.aniState = curState;
                shoujiState.aniKey = stateName;
                shoujiState.aniState.motion = newClip;
                if (events.Count < 0)
                {
                    startEvent.functionName = "OnShoujiEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
                }
            }
            else if (name.Contains("paobu") == true)
            {
                paobuState.aniState = curState;
                paobuState.aniKey = stateName;
                paobuState.aniState.motion = newClip;
                AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(newClip);
                clipSetting.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(newClip, clipSetting);
            }
            else if (name.Contains("chuchang") == true)
            {
                chuchangState.aniState = curState;
                chuchangState.aniKey = stateName;
                chuchangState.aniState.motion = newClip;
                if (events.Count < 0)
                {
                    startEvent.functionName = "OnChuChangEnd";
                    events.Add(startEvent);
                    AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
                }
            }
        }
        //any->siwang       
        AnimatorStateTransition tosiwangTrans = rootStateMachine.AddAnyStateTransition(siwangState.aniState);        
        tosiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        tosiwangTrans.canTransitionToSelf = false;
        AnimatorStateTransition toshengliTrans = rootStateMachine.AddAnyStateTransition(shengliState.aniState);
        toshengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shengliState.aniKey);
        toshengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        toshengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shoukongState.aniKey);
        toshengliTrans.canTransitionToSelf = false;
        AnimatorStateTransition toshoukong = rootStateMachine.AddAnyStateTransition(shoukongState.aniState);
        toshoukong.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        toshoukong.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        toshoukong.canTransitionToSelf = false;
        rootStateMachine.defaultState = daijiState.aniState;

        
        //daiji->?
        AnimatorStateTransition daijiToShengliTrans = daijiState.aniState.AddTransition(shengliState.aniState);
        daijiToShengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shengliState.aniKey);
        daijiToShengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToShengliTrans.hasExitTime = false;
        AnimatorStateTransition daijiToSiwangTrans = daijiState.aniState.AddTransition(siwangState.aniState);
        daijiToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        daijiToSiwangTrans.hasExitTime = false;
        AnimatorStateTransition daijiToShoukongTrans = daijiState.aniState.AddTransition(shoukongState.aniState);
        daijiToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        daijiToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToShoukongTrans.hasExitTime = false;
        AnimatorStateTransition daijiToWugongTrans = daijiState.aniState.AddTransition(wugongState.aniState);
        daijiToWugongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, wugongState.aniKey);
        daijiToWugongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToWugongTrans.hasExitTime = false;
        AnimatorStateTransition daijiToFagongTrans = daijiState.aniState.AddTransition(fagongState.aniState);
        daijiToFagongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, fagongState.aniKey);
        daijiToFagongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToFagongTrans.hasExitTime = false;
        AnimatorStateTransition daijiToFangyuTrans = daijiState.aniState.AddTransition(fangyuState.aniState);
        daijiToFangyuTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, fangyuState.aniKey);
        daijiToFangyuTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToFangyuTrans.hasExitTime = false;
        if (isdazhao)//是否创建大招
        {
            AnimatorStateTransition daijiToDazhaoTrans = daijiState.aniState.AddTransition(dazhaoState.aniState);
            daijiToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoState.aniKey);
            daijiToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            daijiToDazhaoTrans.hasExitTime = false;
            AnimatorStateTransition dazhaoToDaijiTrans = dazhaoState.aniState.AddTransition(daijiState.aniState);
            dazhaoToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            dazhaoToDaijiTrans.hasExitTime = true;
            AnimatorStateTransition dazhaoToDazhaoTrans = dazhaoState.aniState.AddTransition(dazhaoState.aniState);
            dazhaoToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoState.aniKey);
            dazhaoToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            dazhaoToDazhaoTrans.hasExitTime = false;
            AnimatorStateTransition dazhaoToSiwangTrans = dazhaoState.aniState.AddTransition(siwangState.aniState);
            dazhaoToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
            dazhaoToSiwangTrans.hasExitTime = false;
            AnimatorStateTransition dazhaoToShoujiTrans = dazhaoState.aniState.AddTransition(shoujiState.aniState);
            dazhaoToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
            dazhaoToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            dazhaoToShoujiTrans.hasExitTime = false;
            AnimatorStateTransition dazhaoToShoukongTrans = dazhaoState.aniState.AddTransition(shoukongState.aniState);
            dazhaoToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
            dazhaoToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            dazhaoToShoukongTrans.hasExitTime = false;
            AnimatorStateTransition fangyuToDazhaoTrans = fangyuState.aniState.AddTransition(dazhaoState.aniState);
            fangyuToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoState.aniKey);
            fangyuToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            fangyuToDazhaoTrans.hasExitTime = false;
            AnimatorStateTransition toulanToDazhaoTrans = toulanState.aniState.AddTransition(dazhaoState.aniState);
            toulanToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoState.aniKey);
            toulanToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            toulanToDazhaoTrans.hasExitTime = true;
            AnimatorStateTransition dazhaoxuanyaoToDazhaoTrans = dazhaoxuanyaoState.aniState.AddTransition(dazhaoState.aniState);
            dazhaoxuanyaoToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
            dazhaoxuanyaoToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, dazhaoState.aniKey);
            dazhaoxuanyaoToDazhaoTrans.hasExitTime = false; 
        }
        AnimatorStateTransition daijiToDazhaoxuanyaoTrans = daijiState.aniState.AddTransition(dazhaoxuanyaoState.aniState);
        daijiToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoxuanyaoState.aniKey);
        daijiToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToDazhaoxuanyaoTrans.hasExitTime = false;      
        AnimatorStateTransition daijiToToulanTrans = daijiState.aniState.AddTransition(toulanState.aniState);
        daijiToToulanTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, toulanState.aniKey);
        daijiToToulanTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToToulanTrans.hasExitTime = false;
        AnimatorStateTransition daijiToShoujiTrans = daijiState.aniState.AddTransition(shoujiState.aniState);
        daijiToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        daijiToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition daijiToPaoluTrans = daijiState.aniState.AddTransition(paobuState.aniState);
        daijiToPaoluTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, paobuState.aniKey);
        daijiToPaoluTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToPaoluTrans.duration = 0.0f;
        daijiToPaoluTrans.hasExitTime = false;
        AnimatorStateTransition daijiToChuchangTrans = daijiState.aniState.AddTransition(chuchangState.aniState);
        daijiToChuchangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, chuchangState.aniKey);
        daijiToChuchangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        daijiToChuchangTrans.hasExitTime = false;
        
        //?->daiji
        AnimatorStateTransition siwangToDaijiTrans = siwangState.aniState.AddTransition(daijiState.aniState);
        siwangToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        siwangToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition wugongToDaijiTrans = wugongState.aniState.AddTransition(daijiState.aniState);
        wugongToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        wugongToDaijiTrans.exitTime = 0.99f;
        wugongToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition wugongToSiwangTrans = wugongState.aniState.AddTransition(siwangState.aniState);
        wugongToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        wugongToSiwangTrans.hasExitTime = false;
        AnimatorStateTransition wugongToShoukongTrans = wugongState.aniState.AddTransition(shoukongState.aniState);
        wugongToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        wugongToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        wugongToShoukongTrans.hasExitTime = false;
        AnimatorStateTransition wugongToShoujiTrans = wugongState.aniState.AddTransition(shoujiState.aniState);
        wugongToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        wugongToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        wugongToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition fagongToDaijiTrans = fagongState.aniState.AddTransition(daijiState.aniState);
        fagongToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fagongToDaijiTrans.hasExitTime = true;
        fagongToDaijiTrans.exitTime = 0.99f;
        AnimatorStateTransition fagongToSiwangTrans = fagongState.aniState.AddTransition(siwangState.aniState);
        fagongToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        fagongToSiwangTrans.hasExitTime = false;
        AnimatorStateTransition fagongToShoujiTrans = fagongState.aniState.AddTransition(shoujiState.aniState);
        fagongToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        fagongToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fagongToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition fagongToShoukongTrans = fagongState.aniState.AddTransition(shoukongState.aniState);
        fagongToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        fagongToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fagongToShoukongTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToDaijiTrans = fangyuState.aniState.AddTransition(daijiState.aniState);
        fangyuToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, fangyuState.aniKey);
        fangyuToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToDaijiTrans.exitTime = 0.99f;
        fangyuToDaijiTrans.hasExitTime =  true;
        AnimatorStateTransition fangyuToFagongTrans = fangyuState.aniState.AddTransition(fagongState.aniState);
        fangyuToFagongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, fagongState.aniKey);
        fangyuToFagongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToFagongTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToShengliTrans = fangyuState.aniState.AddTransition(shengliState.aniState);
        fangyuToShengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shengliState.aniKey);
        fangyuToShengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToShengliTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToWugongTrans = fangyuState.aniState.AddTransition(wugongState.aniState);
        fangyuToWugongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, wugongState.aniKey);
        fangyuToWugongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToWugongTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToShoukongTrans = fangyuState.aniState.AddTransition(shoukongState.aniState);
        fangyuToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        fangyuToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToShoukongTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToShoujiTrans = fangyuState.aniState.AddTransition(shoujiState.aniState);
        fangyuToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        fangyuToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToToulanTrans = fangyuState.aniState.AddTransition(toulanState.aniState);
        fangyuToToulanTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, toulanState.aniKey);
        fangyuToToulanTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToToulanTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToSiwangTrans = fangyuState.aniState.AddTransition(siwangState.aniState);
        fangyuToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        fangyuToSiwangTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToDazhaoxuanyaoTrans = fangyuState.aniState.AddTransition(dazhaoxuanyaoState.aniState);
        fangyuToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoxuanyaoState.aniKey);
        fangyuToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToDazhaoxuanyaoTrans.hasExitTime = false;
        AnimatorStateTransition fangyuToPaobuTrans = fangyuState.aniState.AddTransition(paobuState.aniState);
        fangyuToPaobuTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, paobuState.aniKey);
        fangyuToPaobuTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        fangyuToPaobuTrans.hasExitTime = false;
        AnimatorStateTransition dazhaoxuanyaoToDaijiTrans = dazhaoxuanyaoState.aniState.AddTransition(daijiState.aniState);
        dazhaoxuanyaoToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        dazhaoxuanyaoToDaijiTrans.exitTime = 0.99f;
        dazhaoxuanyaoToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition dazhaoxuanyaoToSiwangTrans = dazhaoxuanyaoState.aniState.AddTransition(siwangState.aniState);
        dazhaoxuanyaoToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        dazhaoxuanyaoToSiwangTrans.hasExitTime = true;
        AnimatorStateTransition toulanToDaijiTrans = toulanState.aniState.AddTransition(daijiState.aniState);
        toulanToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        toulanToDaijiTrans.exitTime = 0.99f;
        toulanToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition toulanToShoujiTrans = toulanState.aniState.AddTransition(shoujiState.aniState);
        toulanToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        toulanToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        toulanToShoujiTrans.hasExitTime = true;
        AnimatorStateTransition toulanToDazhaoxuanyaoTrans = toulanState.aniState.AddTransition(dazhaoxuanyaoState.aniState);
        toulanToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoxuanyaoState.aniKey);
        toulanToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        toulanToDazhaoxuanyaoTrans.hasExitTime = true;
        AnimatorStateTransition toulanToSiwangTrans = toulanState.aniState.AddTransition(siwangState.aniState);
        toulanToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        toulanToSiwangTrans.hasExitTime = true;
        AnimatorStateTransition shoujiToDaijiTrans = shoujiState.aniState.AddTransition(daijiState.aniState);
        shoujiToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shoukongState.aniKey);
        shoujiToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        shoujiToDaijiTrans.exitTime = 0.99f;
        shoujiToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition shoujiToShoukongTrans = shoujiState.aniState.AddTransition(shoukongState.aniState);
        shoujiToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        shoujiToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        shoujiToShoukongTrans.exitTime = 0.99f;
        shoujiToShoukongTrans.hasExitTime = true;
        AnimatorStateTransition shoujiToShoujiTrans = shoujiState.aniState.AddTransition(shoujiState.aniState);
        shoujiToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        shoujiToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        shoujiToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition shoujiToSiwangTrans = shoujiState.aniState.AddTransition(siwangState.aniState);
        shoujiToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        shoujiToSiwangTrans.hasExitTime = false;
        AnimatorStateTransition paobuToDaijiTrans = paobuState.aniState.AddTransition(daijiState.aniState);
        paobuToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, paobuState.aniKey);
        paobuToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        paobuToDaijiTrans.exitTime = 0.99f;
        paobuToDaijiTrans.hasExitTime = true;        

        AnimatorStateTransition shengliToDajiTrans = shengliState.aniState.AddTransition(daijiState.aniState);
        shengliToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shengliState.aniKey); 
        shengliToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);        
        shengliToDajiTrans.hasExitTime = true;
        AnimatorStateTransition shoukongToDajiTrans = shoukongState.aniState.AddTransition(daijiState.aniState);
        shoukongToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shoukongState.aniKey);
        shoukongToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        shoukongToDajiTrans.exitTime = 0.99f;
        shoukongToDajiTrans.hasExitTime = true;
        AnimatorStateTransition shoukongToShoujiTrans = shoukongState.aniState.AddTransition(shoujiState.aniState);
        shoukongToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        shoukongToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        shoukongToShoujiTrans.exitTime = 0.99f;
        shoukongToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition shoukongToSiwangTrans = shoukongState.aniState.AddTransition(siwangState.aniState);
        shoukongToSiwangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, siwangState.aniKey);
        shoukongToSiwangTrans.hasExitTime = false;
        AnimatorStateTransition shoukongToShengliTrans = shoukongState.aniState.AddTransition(shengliState.aniState);
        shoukongToShengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        shoukongToShengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shengliState.aniKey);
        shoukongToShengliTrans.hasExitTime = false;
        AnimatorStateTransition shoukongToPaobuTrans = shoukongState.aniState.AddTransition(paobuState.aniState);
        shoukongToPaobuTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        shoukongToPaobuTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, paobuState.aniKey);
        shoukongToPaobuTrans.hasExitTime = false;
        AnimatorStateTransition chuchangToDaijiTrans = chuchangState.aniState.AddTransition(daijiState.aniState);
        chuchangToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        chuchangToDaijiTrans.exitTime = 0.99f;
        chuchangToDaijiTrans.hasExitTime = true;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".fbx") || ext.Equals(".FBX") || ext.Equals(".anim"))
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