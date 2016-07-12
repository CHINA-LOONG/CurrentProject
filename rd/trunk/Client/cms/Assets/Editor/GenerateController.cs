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
        var controllerDirPath = EditorUtility.OpenFolderPanel("选择读取路径", @"C:/Users/The Second Lock/Desktop/Project/rd/trunk/Client/cms/Assets", "");
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


        string stateName;
        for (int index = 0; index < motionNameList.Count; ++index)
        {
            string name = motionNameList[index];
            AnimationClip newClip = AssetDatabase.LoadAssetAtPath(name, typeof(AnimationClip)) as AnimationClip;
            if (newClip == null)
                continue;
            //name = Path.GetFileName(name);
            //string[] wholeName = name.Split('.');
            stateName = newClip.name;

            //create state, param and trans 
            AnimatorState curState = rootStateMachine.AddState(stateName);
            controller.AddParameter(stateName, AnimatorControllerParameterType.Bool);
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
            }
            else if (name.Contains("fagong") == true)
            {
                fagongState.aniState = curState;
                fagongState.aniKey = stateName;
                fagongState.aniState.motion = newClip;
            }
            else if (name.Contains("fangyu") == true)
            {
                fangyuState.aniState = curState;
                fangyuState.aniKey = stateName;
                fangyuState.aniState.motion = newClip;
            }
            else if (name.Contains("dazhaoxuanyao") == true)
            {
                dazhaoxuanyaoState.aniState = curState;
                dazhaoxuanyaoState.aniKey = stateName;
                dazhaoxuanyaoState.aniState.motion = newClip;
            }
            else if (name.Contains("dazhao") == true && name.Contains("dazhaoxuanyao") == false)
            {
                dazhaoState.aniState = curState;
                dazhaoState.aniKey = stateName;
                dazhaoState.aniState.motion = newClip;
            }
            else if (name.Contains("toulan") == true)
            {
                toulanState.aniState = curState;
                toulanState.aniKey = stateName;
                toulanState.aniState.motion = newClip;
            }
            else if (name.Contains("shouji") == true)
            {
                shoujiState.aniState = curState;
                shoujiState.aniKey = stateName;
                shoujiState.aniState.motion = newClip;
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
        AnimatorStateTransition toshoukong = rootStateMachine.AddAnyStateTransition(shoukongState.aniState);
        toshoukong.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shoukongState.aniKey);
        AnimatorStateTransition toshengliTrans = rootStateMachine.AddAnyStateTransition(shengliState.aniState);
        toshengliTrans.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, shengliState.aniKey);


        //daiji->?
        AnimatorStateTransition daijiToWugongTrans = daijiState.aniState.AddTransition(wugongState.aniState);
        AnimatorStateTransition daijiToFagongTrans = daijiState.aniState.AddTransition(fagongState.aniState);
        AnimatorStateTransition daijiToFangyuTrans = daijiState.aniState.AddTransition(fangyuState.aniState);
        AnimatorStateTransition daijiToDazhaoxuanyaoTrans = daijiState.aniState.AddTransition(dazhaoxuanyaoState.aniState);
        AnimatorStateTransition daijiToDazhaoTrans = daijiState.aniState.AddTransition(dazhaoState.aniState);
        AnimatorStateTransition daijiToToulanTrans = daijiState.aniState.AddTransition(toulanState.aniState);
        AnimatorStateTransition daijiToShoujiTrans = daijiState.aniState.AddTransition(shoujiState.aniState);
        AnimatorStateTransition daijiToPaoluTrans = daijiState.aniState.AddTransition(paoluState.aniState);

        //?->daiji
        AnimatorStateTransition wugongToDaijiTrans = wugongState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition fagongToDaijiTrans = fagongState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition fangyuToDaijiTrans = fangyuState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition dazhaoxuanyaoToDaijiTrans = dazhaoxuanyaoState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition dazhaoToDaijiTrans = dazhaoState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition toulanToDaijiTrans = toulanState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition shoujiToDaijiTrans = shoujiState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition paoluToDaijiTrans = paoluState.aniState.AddTransition(daijiState.aniState);

        AnimatorStateTransition siwangToDajiTrans = siwangState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition shengliToDajiTrans = shengliState.aniState.AddTransition(daijiState.aniState);
        AnimatorStateTransition shoukongToDajiTrans = shoukongState.aniState.AddTransition(daijiState.aniState);
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