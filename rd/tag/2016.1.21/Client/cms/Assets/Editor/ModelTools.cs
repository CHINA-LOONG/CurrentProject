//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using System;
//using System.IO;

//enum AnimDataType
//{
//    Name = 0,
//    BeginTime,
//    EndTime,
//    IsLoop
//}

//public class ImportAnimData
//{
//    public string mAnimName;
//    public float mBegin;
//    public float mEnd;
//    public float mTime;
//    public bool mLoop;
//}

//public class ImportModelData
//{
//    public DirectoryInfo mModelDirectoryOnFileSystem;
//    public DirectoryInfo mModelPrefabDirectoryOnUnity;
//    public DirectoryInfo mModelSrcDirectoryOnUnity;
//    public List<FileInfo> mListFbx = new List<FileInfo>();
//    public List<FileInfo> mListPng = new List<FileInfo>();
//    public FileInfo mAnimFile = null;
//    public List<int> mListShaderType = new List<int>();
//    public List<ImportAnimData> mAnimDatas = new List<ImportAnimData>();
//    public Dictionary<string, List<FileInfo>> mSFI = new Dictionary<string, List<FileInfo>>();
//    public string mPrefabName;
//    public ModelImporterAnimationType mAnimType = ModelImporterAnimationType.Generic;
//    public static FileInfo GetTexFromName(ImportModelData imd, string name)
//    {
//        foreach (KeyValuePair<string, List<FileInfo>> kv in imd.mSFI)
//        {
//            if (kv.Key == name)
//            {
//                foreach (FileInfo fi in kv.Value)
//                {
//                    if (fi.Extension.ToLower() == ".png" || fi.Extension.ToLower() == ".tga" || fi.Extension.ToLower() == ".psd")
//                    {
//                        return fi;
//                    }
//                }
//            }
//        }
//        return null;
//    }
//}

//public class ClipListCreater
//{
//    private List<ModelImporterClipAnimation> clipList = new List<ModelImporterClipAnimation>();
//    public void addClip(string name, int firstFrame, int lastFrame, bool loop, WrapMode wrapMode)
//    {
//        ModelImporterClipAnimation tempClip = new ModelImporterClipAnimation();
//        tempClip.name = name;
//        tempClip.firstFrame = firstFrame;
//        tempClip.lastFrame = lastFrame;
//        tempClip.loop = loop;
//        tempClip.wrapMode = wrapMode;
//        tempClip.loopPose = loop;
//        tempClip.loopTime = loop;
//        clipList.Add(tempClip);            
//    }
	
//    public ModelImporterClipAnimation[] getArray()
//    {
//        return clipList.ToArray();
//    }
//}

//public class AnimModelSet : AssetPostprocessor
//{
//    void OnPreprocessModel()
//    {
//        ModelImporter mi = assetImporter as ModelImporter;
//        int slashPos = assetPath.LastIndexOf('/');
//        string modelName = assetPath.Substring(slashPos+1, assetPath.Length-slashPos-1);
//        Debug.Log("assetPath is " + modelName);
//        ImportModelData imd = ModelTools.GetImportModelDataByName(modelName);
//        if (imd != null)
//        {
//            ClipListCreater creater = new ClipListCreater();
//            foreach (ImportAnimData iad in imd.mAnimDatas)
//            {
//                WrapMode wm = WrapMode.Once;
//                if (iad.mLoop)
//                {
//                    wm = WrapMode.Loop;
//                }
//                else
//                {
//                    wm = WrapMode.Once;
//                }
//                creater.addClip(iad.mAnimName, (int)iad.mBegin, (int)iad.mEnd, iad.mLoop, wm);
//            }
//            mi.clipAnimations = creater.getArray();
//            mi.animationType = imd.mAnimType;
//        }
//    }
//}

//public class ModelTools
//{
//    public static ImportFolderConfig  mFolderConfig = new ImportFolderConfig();
//    public static List<ImportModelData> mImportModelDataList = new List<ImportModelData>();
//    public class ImportFolderConfig
//    {
//        public  string mSrcRootPathNameOnUnity = "ImportedModels";
//        public  string mDestRootPathNameOnUnity = "Resources/ModelPrefabs";
//        public  string mConfigRootPathNameOnFileSystem = "ShaderConfig";

//        public  string mDogFbxRootNameOnUnity = "ImportedModels/Room/Dog";
//        public  string mDogPrefabRootNameOnUnity = "Resources/Prefabs/Room/Dog";
//        public  string mDogSkeletonFbxNameOnUnity = "ImportedModels/Room/Dog/DogSkeleton";
//        public  string mDogSkeletonPrefabNameOnUnity = "Resources/Prefabs/Room/Dog/DogSkeleton";
//        public  string mDogEquipmentFbxNameOnUnity = "ImportedModels/Room/Dog/DogEquipment";
//        public  string mDogEquipmentPrefabNameOnUnity = "Resources/Prefabs/Room/Dog/DogEquipment";
//        //public  string mDogSuitsFbxNameOnUnity = "ImportedModels/Room/Dog/DogSuits";
//        public string m_StaticFbxNameOnUnity = "cmsAsset/ImportModels/SceneModels";
//        public  string m_StaticModelPrefabNameOnUnity = "cmsAsset/prefabs/SceneModels";
		
//        public  DirectoryInfo mSrcRootPathOnFileSystem = null;
//        public  DirectoryInfo mSrcRootPathOnUnity = null;
//        public  DirectoryInfo mDestRootPathOnUnity = null;
//        public  DirectoryInfo mConfigRootPathOnFileSystem = null;

//        public  DirectoryInfo mDogRootFbxOnUnity  = null;
//        public  DirectoryInfo mDogRootPrefabOnUnity  = null;
//        public  DirectoryInfo mDogSkeletonFbxOnUnity  = null;
//        public  DirectoryInfo mDogSkeletonPrefabOnUnity  = null;
//        public  DirectoryInfo mDogEquipmentFbxOnUnity  = null;
//        public  DirectoryInfo mDogEquipmentPrefabOnUnity  = null;
//        //public  DirectoryInfo mDogSuitsFbxOnUnity  = null;
//        //public  DirectoryInfo mDogSuitsPrefabOnUnity  = null;

//        public DirectoryInfo m_StaticModelFbxOnUnity;
//        public DirectoryInfo m_StaticModelPrefabOnUnity = null;

//        public void	SetSelectedFolder(string folderPath)
//        {
//            mSrcRootPathOnFileSystem = new DirectoryInfo(folderPath);
//            mSrcRootPathOnUnity = new DirectoryInfo(Application.dataPath + "/" + mSrcRootPathNameOnUnity + "/");
//            //mSrcPersistRootPathOnUnity = new DirectoryInfo(Application.dataPath + "/" + mSrcPersistRootPathNameOnUnity + "/");
//            mDestRootPathOnUnity = new DirectoryInfo(Application.dataPath + "/" + mDestRootPathNameOnUnity + "/");

//            mDogRootFbxOnUnity = new DirectoryInfo(Application.dataPath + "/" + mDogFbxRootNameOnUnity + "/");
//            mDogRootPrefabOnUnity = new DirectoryInfo(Application.dataPath + "/" + mDogPrefabRootNameOnUnity + "/");

//            mDogSkeletonFbxOnUnity = new DirectoryInfo(Application.dataPath + "/" + mDogSkeletonFbxNameOnUnity + "/");
//            mDogSkeletonPrefabOnUnity = new DirectoryInfo(Application.dataPath + "/" + mDogSkeletonPrefabNameOnUnity + "/");
		
//            mDogEquipmentFbxOnUnity = new DirectoryInfo(Application.dataPath + "/" + mDogEquipmentFbxNameOnUnity + "/");
//            mDogEquipmentPrefabOnUnity = new DirectoryInfo(Application.dataPath + "/" + mDogEquipmentPrefabNameOnUnity + "/");

//            m_StaticModelFbxOnUnity = new DirectoryInfo(Application.dataPath + "/" + m_StaticFbxNameOnUnity + "/");
//            m_StaticModelPrefabOnUnity = new DirectoryInfo(Application.dataPath + "/" + m_StaticModelPrefabNameOnUnity + "/");
//        }
//    }

//    public static ImportModelData GetImportModelDataWithDirectoryInfo(DirectoryInfo di)
//    {
//        ImportModelData imd = new ImportModelData();
//        imd.mModelDirectoryOnFileSystem = di;
//        FileInfo[] fis = di.GetFiles();
//        foreach (FileInfo f in fis)
//        {
//            string withoutExt = f.Name;
//            withoutExt = withoutExt.Substring(0, withoutExt.LastIndexOf('.'));
//            if (f.Extension.ToLower() == ".fbx")
//            {
//                imd.mListFbx.Add(f);
//            }
//            else if (f.Extension.ToLower() == ".png" || f.Extension.ToLower() == ".tga" || f.Extension.ToLower() == ".psd")
//            {
//                imd.mListPng.Add(f);
//            }
//            else if (f.Extension.ToLower() == ".txt" && f.Name.ToLower().Contains("anim"))
//            {
//                imd.mAnimFile = f;
//            }
//            else if (f.Extension.ToLower() == ".txt" && f.Name.ToLower().Contains("shadertype"))
//            {
//                string t = File.ReadAllText(f.FullName);
//                imd.mListShaderType.Add(int.Parse(t));
//            }
//            if (!imd.mSFI.ContainsKey(withoutExt))
//            {
//                imd.mSFI.Add(withoutExt, new List<FileInfo>());
//            }
//            List<FileInfo> l = imd.mSFI[withoutExt];
//            l.Add(f);
//        }
//        return imd;
//    }
	
//    public static ImportModelData GetImportModelDataByName(string name)
//    {
//        foreach (ImportModelData imd in mImportModelDataList)
//        {
//            if (imd.mListFbx[0].Name == name)
//            {
//                return imd;
//            }
//        }
//        return null;
//    }

//    public static void UpdateModelAnimation(ImportModelData modelData)
//    {
//        if (modelData.mAnimFile == null) 
//        {
//            return;
//        }
//        string[] ss = File.ReadAllLines(modelData.mAnimFile.FullName);
//        Debug.Log("animation fullname:" + modelData.mAnimFile.FullName);
//        foreach (string s in ss)
//        {
//            string[] sss = s.Split(' ');
//            ImportAnimData iad = new ImportAnimData();
//            for (int index = 0; index < sss.Length; index++)
//            {
//                AnimDataType aniType = (AnimDataType)index;
//                string ssss = sss[index];
//                ssss.Trim();
//                if (aniType == AnimDataType.Name)
//                {
//                    iad.mAnimName = ssss;
//                }
//                else if (aniType ==  AnimDataType.BeginTime) 
//                {
//                    iad.mBegin = float.Parse(ssss);
//                }
//                else if (aniType == AnimDataType.EndTime)
//                {
//                    iad.mEnd = float.Parse(ssss);
//                }
//                else if (aniType == AnimDataType.IsLoop)
//                {
//                    if (ssss == "y")
//                    {
//                        iad.mLoop = true;
//                    }
//                    else 
//                    {
//                        iad.mLoop = false;
//                    }
//                }
//            }
//            modelData.mAnimDatas.Add(iad);
//        }
//    }
	
//    public static void CreateDogSkeletonWithImportModelData(ImportModelData modelData)
//    {
//        string withoutExt = modelData.mPrefabName;
//        string assetSrcPath = modelData.mModelSrcDirectoryOnUnity.FullName; 
//        int index = assetSrcPath.IndexOf ("Assets");
//        assetSrcPath = assetSrcPath.Substring (index, assetSrcPath.Length - index);
//        //assetSrcPath
//        string assetDestPath = modelData.mModelPrefabDirectoryOnUnity.FullName;
//        index = assetDestPath.IndexOf ("Assets");
//        assetDestPath = assetDestPath.Substring (index, assetDestPath.Length - index);
//        Debug.Log ("loadAssert path =" + assetSrcPath + modelData.mListFbx[0].Name);
//        UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(assetSrcPath + modelData.mListFbx[0].Name, typeof(UnityEngine.Object)) as UnityEngine.Object;
//        if (null == o) 
//        {
//            EditorUtility.DisplayDialog("Warning", "Create Prefab Error!", "Ok");
//            return;
//        }
//        GameObject go = GameObject.Instantiate(o) as GameObject;
//        go.name = withoutExt;
//        go.transform.localPosition = new Vector3(0,0,0);
//        go.transform.localEulerAngles = new Vector3(0,0,0);
//        go.transform.localScale = new Vector3(1,1,1);
//        Debug.Log ("dest path =" + assetDestPath);
//        PrefabUtility.CreatePrefab(assetDestPath + withoutExt + ".prefab", go);
//        GameObject.DestroyImmediate(go);
//    }

//    public static void CreateStaticModelWithImportModelData(ImportModelData modelData, string modelName)
//    {
//        string assetSrcPath = modelData.mModelSrcDirectoryOnUnity.FullName + modelName + "/"; 
//        int index = assetSrcPath.IndexOf ("Assets");
//        assetSrcPath = assetSrcPath.Substring (index, assetSrcPath.Length - index);
//        //assetSrcPath
//        string assetDestPath = modelData.mModelPrefabDirectoryOnUnity.FullName + modelName + "/";
//        index = assetDestPath.IndexOf ("Assets");
//        assetDestPath = assetDestPath.Substring (index, assetDestPath.Length - index);
//        Debug.Log ("loadAssert path =" + assetSrcPath + modelData.mListFbx[0].Name);
//        for(int i = 0; i < modelData.mListFbx.Count; i++)
//        {
//            UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(assetSrcPath + modelData.mListFbx[i].Name, typeof(UnityEngine.Object)) as UnityEngine.Object;
//            if (null == o) 
//            {
//                EditorUtility.DisplayDialog("Warning", "Create Prefab Error!", "Ok");
//                return;
//            }

//            string withoutExt = modelData.mListFbx[i].Name;
//            withoutExt = withoutExt.Substring(0, withoutExt.LastIndexOf('.'));

//            /*
//            FileInfo fi = ImportModelData.GetTexFromName(modelData, withoutExt);
//            if (fi == null)
//            {
//                Debug.LogError("FBX Error! " + withoutExt);
//                continue;
//            }
//            UnityEngine.Object tex = AssetDatabase.LoadAssetAtPath(assetSrcPath + fi.Name, typeof(UnityEngine.Object)) as UnityEngine.Object;
//            if (null == tex)
//            {
//                Debug.LogError("LoadAsset Texture Error!");
//            }
//            */
//            string prefabName = modelData.mModelDirectoryOnFileSystem.Name;

//            GameObject go = GameObject.Instantiate(o) as GameObject;
//            go.name = withoutExt;
//            go.transform.localPosition = new Vector3(0,0,0);
//            go.transform.localEulerAngles = new Vector3(0,0,0);
//            go.transform.localScale = new Vector3(1,1,1);
//            Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
//            /*foreach (Renderer r in rs)
//            {
//                r.sharedMaterial.mainTexture = (Texture)tex;
//                r.sharedMaterial.color = Color.white;
//                r.sharedMaterial.shader = Shader.Find("Mobile/Diffuse");
//            }*/
//            PrefabUtility.CreatePrefab(assetDestPath + withoutExt + ".prefab", go);
//            AssetDatabase.Refresh();
//            Debug.Log("create Prefab : " + assetDestPath + withoutExt + ".prefab");
//            GameObject.DestroyImmediate(go);
//            AssetDatabase.Refresh();
//        }
//    }

//    [MenuItem("GameTools/ModelTools/ImportSkeleton(Legacy)")]
//    static void ImportSkeletonLegacy()
//    {
//        EditorUtility.DisplayDialog ("Warning!", "Comming later", "OK");
//        return;
//        ImportSkeleton(ModelImporterAnimationType.Legacy);
//    }
//    [MenuItem("GameTools/ModelTools/ImportSkeleton(Generic)")]
//    static void ImportSkeletonGeneric()
//    {
//        EditorUtility.DisplayDialog ("Warning!", "Comming later", "OK");
//        return;
//        ImportSkeleton(ModelImporterAnimationType.Generic);
//    }
//    static void ImportSkeleton(ModelImporterAnimationType miat)
//    {
//        mImportModelDataList.Clear();
//        string folderPath = EditorUtility.OpenFolderPanel("Select Skeleton", "", "");
//        if (folderPath == "") 
//        {
//            return;
//        }
//        mFolderConfig.SetSelectedFolder(folderPath);
//        DirectoryInfo[] dis = mFolderConfig.mSrcRootPathOnFileSystem.GetDirectories();
//        if (dis.Length > 0)
//        {
//            EditorUtility.DisplayDialog("Warning!", "Useless folder existed!", "Ok");
//            return;
//        }
//        ImportModelData imd = ModelTools.GetImportModelDataWithDirectoryInfo(mFolderConfig.mSrcRootPathOnFileSystem);
//        imd.mPrefabName = "DogSkeleton";
//        imd.mAnimType = miat;
//        if (imd.mListFbx.Count != 1)
//        {
//            EditorUtility.DisplayDialog("Warning!", "Only 1 .fbx file should be included!", "Ok");
//            return;
//        }
//        if (imd.mAnimFile == null)
//        {
//            EditorUtility.DisplayDialog("Warning!", "No Animation File could be found!", "Ok");
//            return;
//        }
//        UpdateModelAnimation(imd);

//        if (!mFolderConfig.mDogSkeletonFbxOnUnity.Exists)
//        {
//            Directory.CreateDirectory(mFolderConfig.mDogSkeletonFbxOnUnity.FullName);
//        }
//        if (!mFolderConfig.mDogSkeletonPrefabOnUnity.Exists)
//        {
//            Directory.CreateDirectory(mFolderConfig.mDogSkeletonPrefabOnUnity.FullName);
//        }
//        imd.mModelSrcDirectoryOnUnity = mFolderConfig.mDogSkeletonFbxOnUnity;
//        imd.mModelPrefabDirectoryOnUnity = mFolderConfig.mDogSkeletonPrefabOnUnity;
//        mImportModelDataList.Add(imd);
//        foreach (FileInfo ff in mFolderConfig.mDogSkeletonFbxOnUnity.GetFiles()) 
//        {
//            ff.Delete();
//        }
//        foreach (FileInfo ff in mFolderConfig.mDogSkeletonPrefabOnUnity.GetFiles()) 
//        {
//            ff.Delete();
//        }
//        AssetDatabase.Refresh();
//        File.Copy(imd.mModelDirectoryOnFileSystem.FullName+"/"+imd.mListFbx[0].Name, imd.mModelSrcDirectoryOnUnity.FullName+ "/"+ imd.mListFbx[0].Name);
//        AssetDatabase.Refresh();
//        CreateDogSkeletonWithImportModelData(imd);
//    }

//    [MenuItem("GameTools/ModelTools/ImportStaticModel")]
//    static void ImportStaticModel()
//    {
//        mImportModelDataList.Clear();
//        string folderPath = EditorUtility.OpenFolderPanel("Select Static Model", "", "");
//        if (folderPath == "") 
//        {
//            return;
//        }
//        mFolderConfig.SetSelectedFolder(folderPath);
//        DirectoryInfo[] dis = mFolderConfig.mSrcRootPathOnFileSystem.GetDirectories();
//        if (dis.Length > 0)
//        {
//            EditorUtility.DisplayDialog("Warning!", "Useless folder existed!", "Ok");
//            return;
//        }
//        ImportModelData imd = ModelTools.GetImportModelDataWithDirectoryInfo(mFolderConfig.mSrcRootPathOnFileSystem);
//        if (imd.mListFbx.Count < 1)
//        {
//            EditorUtility.DisplayDialog("Warning!", "Can't find any .Fbx file", "Ok");
//            return;
//        }
//        if (!mFolderConfig.m_StaticModelFbxOnUnity.Exists)
//        {
//            Directory.CreateDirectory(mFolderConfig.m_StaticModelFbxOnUnity.FullName);
//        }

//        string modelName = mFolderConfig.mSrcRootPathOnFileSystem.Name;
//        string modelFbxPath = mFolderConfig.m_StaticModelFbxOnUnity.FullName + modelName + "/";
//        string modelePrefabPath = mFolderConfig.m_StaticModelPrefabOnUnity.FullName + modelName + "/";
	

//        DirectoryInfo diss = new DirectoryInfo(modelFbxPath);
//        DirectoryInfo dipp = new DirectoryInfo(modelePrefabPath);
//        if (diss.Exists)
//        {
//            foreach (DirectoryInfo dd in diss.GetDirectories())
//            {
//                foreach (FileInfo ff in dd.GetFiles()) 
//                {
//                    ff.Delete();
//                }
//                dd.Delete();
//            }
//            foreach (FileInfo ff in diss.GetFiles()) 
//            {
//                ff.Delete();
//            }
//            diss.Delete();
//        }
//        if (dipp.Exists)
//        {
//            foreach (DirectoryInfo dd in dipp.GetDirectories())
//            {
//                foreach (FileInfo ff in dd.GetFiles()) 
//                {
//                    ff.Delete();
//                }
//                dd.Delete();
//            }
//            foreach (FileInfo ff in dipp.GetFiles()) 
//            {
//                ff.Delete();
//            }
//            dipp.Delete();
//        }
//        AssetDatabase.Refresh();
//        diss = Directory.CreateDirectory(modelFbxPath);
//        dipp = Directory.CreateDirectory(modelePrefabPath);
//        imd.mModelSrcDirectoryOnUnity = mFolderConfig.m_StaticModelFbxOnUnity;
//        imd.mModelPrefabDirectoryOnUnity = mFolderConfig.m_StaticModelPrefabOnUnity;
//        for(int index = 0; index < imd.mListFbx.Count; index++)
//        {
//            File.Copy(imd.mModelDirectoryOnFileSystem.FullName+"/"+imd.mListFbx[index].Name, diss.FullName + "/"+ imd.mListFbx[index].Name);
//        }
//        for(int index = 0; index < imd.mListPng.Count; index++)
//        {
//            File.Copy(imd.mModelDirectoryOnFileSystem.FullName+"/"+imd.mListPng[index].Name, diss.FullName + "/"+ imd.mListPng[index].Name);
//        }
//        mImportModelDataList.Add(imd);
//        AssetDatabase.Refresh();
//        CreateStaticModelWithImportModelData(imd, modelName);
//    }
//}

