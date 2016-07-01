using UnityEngine;
using System.Collections;

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
	[SerializeField]
	ModuleBase mPrevModule = null;
	[SerializeField]
	ModuleBase mNextModule = null;
	
	object mParam0 = null;
	public object Param0Attr
	{
		get
		{
			return mParam0;
		}
		set
		{
			mParam0 = value;
		}
	}

	static GameMain mInst = null;
	public static GameMain Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject go = new GameObject("GameMain");
				mInst = go.AddComponent<GameMain>();
			}
			return mInst;
		}
	}
	
	public void Init()
	{
		DontDestroyOnLoad(gameObject);
		//SkillMgr.Instance.Init();
		ChangeModule<LoginModule>();
	}
	
	void BindListener()
	{
	}
	
	void UnBindListener()
	{
		
	}

	public void ChangeModule<T>(object param0 = null) where T : ModuleBase
	{
		string moduleName = typeof(T).ToString();
		T t = this.gameObject.AddComponent<T>();
		t.ModuleNameAttr = moduleName;
		t.OnInit(mParam0);
		if (mCurModule != null)
		{
			mCurModule.OnExit(t);
		}
		mPrevModule = mCurModule;
		mCurModule = t;
		mCurModule.OnEnter(mPrevModule, param0);
		Destroy(mPrevModule);
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

	public TrainModule GetTrainModule()
	{
		if (mCurModule != null)
		{
			TrainModule tm = mCurModule as TrainModule;
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
}
