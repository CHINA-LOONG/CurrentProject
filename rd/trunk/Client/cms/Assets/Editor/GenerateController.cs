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
            //AnimationEvent startEvent = new AnimationEvent();
            //List<AnimationEvent> events = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(newClip));
            //startEvent.time = 0.01f;
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
            }
            else if (name.Contains("siwang") == true)
            {
                siwangState.aniState = curState;
                siwangState.aniKey = stateName;
                siwangState.aniState.motion = newClip;
                //startEvent.functionName = "OnDeadEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("shengli") == true)
            {
                shengliState.aniState = curState;
                shengliState.aniKey = stateName;
                shengliState.aniState.motion = newClip;
            }
            else if (name.Contains("shoukong") == true)
            {
                shoukongState.aniState = curState;
                shoukongState.aniKey = stateName;
                shoukongState.aniState.motion = newClip;
            }
            else if (name.Contains("wugong") == true)
            {
                wugongState.aniState = curState;
                wugongState.aniKey = stateName;
                wugongState.aniState.motion = newClip;
                //startEvent.functionName = "OnWuGongEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("fagong") == true)
            {
                fagongState.aniState = curState;
                fagongState.aniKey = stateName;
                fagongState.aniState.motion = newClip;
                //startEvent.functionName = "OnFaGongEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("fangyu") == true)
            {
                fangyuState.aniState = curState;
                fangyuState.aniKey = stateName;
                fangyuState.aniState.motion = newClip;
                //startEvent.functionName = "OnFangyuEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("dazhaoxuanyao") == true)
            {
                dazhaoxuanyaoState.aniState = curState;
                dazhaoxuanyaoState.aniKey = stateName;
                dazhaoxuanyaoState.aniState.motion = newClip;
                //startEvent.functionName = "OnDaZhaoxuanyaoEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("dazhao") == true )
            {
                dazhaoState.aniState = curState;
                dazhaoState.aniKey = stateName;
                dazhaoState.aniState.motion = newClip;
                isdazhao = true;
                //startEvent.functionName = "OnDaZhaoEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("toulan") == true)
            {
                toulanState.aniState = curState;
                toulanState.aniKey = stateName;
                toulanState.aniState.motion = newClip;
                //startEvent.functionName = "OnLazyEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("shouji") == true)
            {
                shoujiState.aniState = curState;
                shoujiState.aniKey = stateName;
                shoujiState.aniState.motion = newClip;
                //startEvent.functionName = "OnShoujiEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
            }
            else if (name.Contains("paobu") == true)
            {
                paobuState.aniState = curState;
                paobuState.aniKey = stateName;
                paobuState.aniState.motion = newClip;
            }
            else if (name.Contains("chuchang") == true)
            {
                chuchangState.aniState = curState;
                chuchangState.aniKey = stateName;
                chuchangState.aniState.motion = newClip;
                //startEvent.functionName = "OnChuChangEnd";
                //events.Add(startEvent);
                //AnimationUtility.SetAnimationEvents(newClip, events.ToArray());
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
        rootStateMachine.defaultState = daijiState.aniState;
        
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
        if (isdazhao)//是否创建大招
        {
            AnimatorStateTransition daijiToDazhaoTrans = daijiState.aniState.AddTransition(dazhaoState.aniState);
            daijiToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoState.aniKey);
            daijiToDazhaoTrans.hasExitTime = false;

            AnimatorStateTransition dazhaoToDaijiTrans = dazhaoState.aniState.AddTransition(daijiState.aniState);
            dazhaoToDaijiTrans.hasExitTime = true;
            //AnimatorStateTransition dazhaoToDazhaoTrans = dazhaoState.aniState.AddTransition(dazhaoState.aniState);
            //dazhaoToDazhaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoState.aniKey);
            //dazhaoToDazhaoTrans.hasExitTime = true;
        }
        AnimatorStateTransition daijiToDazhaoxuanyaoTrans = daijiState.aniState.AddTransition(dazhaoxuanyaoState.aniState);
        daijiToDazhaoxuanyaoTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, dazhaoxuanyaoState.aniKey);
        daijiToDazhaoxuanyaoTrans.hasExitTime = false;      
        AnimatorStateTransition daijiToToulanTrans = daijiState.aniState.AddTransition(toulanState.aniState);
        daijiToToulanTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, toulanState.aniKey);
        daijiToToulanTrans.hasExitTime = false;
        AnimatorStateTransition daijiToShoujiTrans = daijiState.aniState.AddTransition(shoujiState.aniState);
        daijiToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        daijiToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition daijiToPaoluTrans = daijiState.aniState.AddTransition(paobuState.aniState);
        daijiToPaoluTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, paobuState.aniKey);
        daijiToPaoluTrans.duration = 0.0f;
        daijiToPaoluTrans.hasExitTime = false;
        AnimatorStateTransition daijiToChuchangTrans = daijiState.aniState.AddTransition(chuchangState.aniState);
        daijiToChuchangTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, chuchangState.aniKey);
        daijiToChuchangTrans.hasExitTime = false;
        
        //?->daiji
        AnimatorStateTransition wugongToDaijiTrans = wugongState.aniState.AddTransition(daijiState.aniState);
        wugongToDaijiTrans.exitTime = 0.99f;
        wugongToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition fagongToDaijiTrans = fagongState.aniState.AddTransition(daijiState.aniState);
        fagongToDaijiTrans.hasExitTime = true;
        fagongToDaijiTrans.exitTime = 0.99f;
        AnimatorStateTransition fangyuToDaijiTrans = fangyuState.aniState.AddTransition(daijiState.aniState);
        fangyuToDaijiTrans.exitTime = 0.99f;
        fangyuToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition dazhaoxuanyaoToDaijiTrans = dazhaoxuanyaoState.aniState.AddTransition(daijiState.aniState);
        dazhaoxuanyaoToDaijiTrans.exitTime = 0.99f;
        dazhaoxuanyaoToDaijiTrans.hasExitTime = true;   
        AnimatorStateTransition toulanToDaijiTrans = toulanState.aniState.AddTransition(daijiState.aniState);
        toulanToDaijiTrans.exitTime = 0.99f;
        toulanToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition shoujiToDaijiTrans = shoujiState.aniState.AddTransition(daijiState.aniState);
        shoujiToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shoukongState.aniKey);
        shoujiToDaijiTrans.exitTime = 0.99f;
        shoujiToDaijiTrans.hasExitTime = true;
        AnimatorStateTransition shoujiToShoukongTrans = shoujiState.aniState.AddTransition(shoukongState.aniState);
        shoujiToShoukongTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        shoujiToShoukongTrans.exitTime = 0.99f;
        shoujiToShoukongTrans.hasExitTime = true;
        //AnimatorStateTransition shoujiToShoujiTrans = shoujiState.aniState.AddTransition(shoujiState.aniState);
        //shoujiToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        //shoujiToShoujiTrans.hasExitTime = false;
        AnimatorStateTransition paobuToDaijiTrans = paobuState.aniState.AddTransition(daijiState.aniState);
        paobuToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, paobuState.aniKey);
        paobuToDaijiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        paobuToDaijiTrans.exitTime = 0.99f;
        paobuToDaijiTrans.hasExitTime = true;        
        AnimatorStateTransition siwangToDajiTrans = siwangState.aniState.AddTransition(daijiState.aniState);
        siwangToDajiTrans.exitTime = 0.99f;
        siwangToDajiTrans.hasExitTime = true;
        AnimatorStateTransition shengliToDajiTrans = shengliState.aniState.AddTransition(daijiState.aniState);
        shengliToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, siwangState.aniKey);
        shengliToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shengliState.aniKey); 
        shengliToDajiTrans.hasExitTime = true;
        AnimatorStateTransition shoukongToDajiTrans = shoukongState.aniState.AddTransition(daijiState.aniState);
        shoukongToDajiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.IfNot, 0, shoukongState.aniKey);
        shoukongToDajiTrans.exitTime = 0.99f;
        shoukongToDajiTrans.hasExitTime = true;
        AnimatorStateTransition shoukongToShoujiTrans = shoukongState.aniState.AddTransition(shoujiState.aniState);
        shoukongToShoujiTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoujiState.aniKey);
        shoukongToShoujiTrans.exitTime = 0.99f;
        AnimatorStateTransition chuchangToDaijiTrans = chuchangState.aniState.AddTransition(daijiState.aniState);
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