using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMain : MonoBehaviour 
{
	[SerializeField]
	ModuleBase mCurModule = null;
	public ModuleBase CurModuleAttr
	{
		get
		{
			return mCurModule;
		}
	}
	//[SerializeField]
	//ModuleBase mPrevModule = null;
	/*[SerializeField]
	ModuleBase mNextModule = null;*/
	
    //object mParam0 = null;
    //public object Param0Attr
    //{
    //    get
    //    {
    //        return mParam0;
    //    }
    //    set
    //    {
    //        mParam0 = value;
    //    }
    //}

	static GameMain mInst = null;
	public static GameMain Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject go = new GameObject("GameMain");
                mInst = go.AddComponent<GameMain>();
                DontDestroyOnLoad(go);
			}
			return mInst;
		}
	}
	
	public void Init()
	{
		//DontDestroyOnLoad(gameObject);
		//SkillMgr.Instance.Init();
		ChangeModule<LoginModule>();
	}

	void OnDestory()
	{
		
	}

	public void ChangeModule<T>(object param0 = null) where T : ModuleBase
	{
		if (mCurModule != null)
		{
			mCurModule.OnExit();
            Destroy(mCurModule);
            UIMgr.Instance.ClearUIOnSceneChange(false);
            ResourceMgr.Instance.UnloadCachedBundles(true);
            //UIMgr.Instance.DestroyAllPopup();
        }
        string moduleName = typeof(T).ToString();
        T t = this.gameObject.AddComponent<T>();
        t.ModuleNameAttr = moduleName;
        t.OnInit(param0);
		//mPrevModule = mCurModule;
		mCurModule = t;
		mCurModule.OnEnter(param0);
		//Destroy(mPrevModule);

        GameSpeedService.Instance.OnModuleChange();
	}

	public bool IsCurModule<T>() where T : ModuleBase
	{
		if (mCurModule == null)
			return false;
		string moduleName = typeof(T).ToString();
		if (moduleName == mCurModule.ModuleNameAttr)
		{
			return true;
		}
		return false;
	}

	public BuildModule GetBuildModule()
	{
		if (mCurModule != null)
		{
			BuildModule tm = mCurModule as BuildModule;
			if (tm != null)
			{
				return tm;
			}
		}
		return null;
	}

	public BattleModule GetBattleModule()
	{
		if (mCurModule != null)
		{
			BattleModule bm = mCurModule as BattleModule;
			if (bm != null)
			{
				return bm;
			}
		}
		return null;
	}

    //---------------------------------------------------------------------------------------------
    //TODO: put where?
    public void LoadBattleLevel(EnterInstanceParam enterInstanceParam)
    {
        //ClearModule();
        InstanceData instanceData = StaticDataMgr.Instance.GetInstanceData(enterInstanceParam.instanceData.instanceId);
        SwitchLevelEventArgs args = new SwitchLevelEventArgs();
        args.enterParam = enterInstanceParam;
        ResourceMgr.Instance.LoadLevelAsyn(instanceData.instanceProtoData.sceneID, false, OnSceneLoaded, args);
        UIMgr.Instance.ClearUILayerList();
    }
    //---------------------------------------------------------------------------------------------
    public void LoadBattleLevelPvp(PvpFightParam pvpParam)
    {
        SwitchLevelEventArgs args = new SwitchLevelEventArgs();
        args.enterParam = null;
        args.pvpParam = pvpParam;

        InstanceData instanceData = StaticDataMgr.Instance.GetInstanceData("pvp_scene");
        ResourceMgr.Instance.LoadLevelAsyn(instanceData.instanceProtoData.sceneID, false, OnSceneLoaded, args);
        UIMgr.Instance.ClearUILayerList();
    }
    //---------------------------------------------------------------------------------------------
    public void LoadBattleLevelGuide(GuideLevelParam guideParam)
    {
        SwitchLevelEventArgs args = new SwitchLevelEventArgs();
        //TODO: need config files or just set it in codes
        args.guideParam = guideParam;
        args.enterParam = null;
        args.pvpParam = null;

        InstanceData guideInstance = StaticDataMgr.Instance.GetInstanceData(guideParam.instanceId);
        ResourceMgr.Instance.LoadLevelAsyn(guideInstance.instanceProtoData.sceneID, false, OnSceneLoaded, args);
        UIMgr.Instance.ClearUILayerList();
    }
    //---------------------------------------------------------------------------------------------
    public void OnSceneLoaded(GameObject instance, System.EventArgs args)
    {
        ClearModule();
        SwitchLevelEventArgs slArgs = args as SwitchLevelEventArgs;
        if (slArgs.enterParam != null)
        {
            ChangeModuleDirect<BattleModule>(slArgs.enterParam);
        }
        else if (slArgs.pvpParam != null)
        {
            ChangeModuleDirect<BattleModule>(slArgs.pvpParam);
        }
        else if (slArgs.guideParam != null)
        {
            ChangeModuleDirect<BattleModule>(slArgs.guideParam);
        }
        //StartCoroutine(FuckingU3d(args));
    }
    //---------------------------------------------------------------------------------------------
    //public IEnumerator FuckingU3d(System.EventArgs args)
    //{
    //    SwitchLevelEventArgs slArgs = args as SwitchLevelEventArgs;
    //    //fucking u3d 0.5s, or it will crash
    //    yield return new WaitForSeconds(0.5f);
    //    ChangeModuleDirect<BattleModule>(slArgs.enterParam);
    //}
    //---------------------------------------------------------------------------------------------
    //public void LoadBattleLevelFinish()
    //{

    //}
    //---------------------------------------------------------------------------------------------
    public void ClearModule()
    {
        if (mCurModule != null)
        {
            mCurModule.OnExit();
            Destroy(mCurModule);
            UIMgr.Instance.ClearUIOnSceneChange(false);
            ResourceMgr.Instance.UnloadCachedBundles(true);
            //UIMgr.Instance.DestroyAllPopup();
            mCurModule = null;
        }
    }
    //---------------------------------------------------------------------------------------------
    public void ChangeModuleDirect<T>(object param0 = null) where T : ModuleBase
    {
        string moduleName = typeof(T).ToString();
        T t = this.gameObject.AddComponent<T>();
        t.ModuleNameAttr = moduleName;
        t.OnInit(param0);
        //mPrevModule = mCurModule;
        mCurModule = t;
        mCurModule.OnEnter(param0);
        //Destroy(mPrevModule);

        GameSpeedService.Instance.OnModuleChange();
    }
    //---------------------------------------------------------------------------------------------
}
