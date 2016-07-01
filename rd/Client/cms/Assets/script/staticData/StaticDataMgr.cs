using UnityEngine;
using System.Collections;

public class StaticDataMgr : MonoBehaviour {
	
	// Use this for initialization
	void Start () 
	{
		
	}
	static StaticDataMgr mInst = null;
	public static StaticDataMgr Instance
	{
		get
		{
			if (mInst == null)
			{
				GameObject go = new GameObject("StaticDataMgr");
				mInst = go.AddComponent<StaticDataMgr>();
			}
			return mInst;
		}
	}
	public void Init()
	{
		DontDestroyOnLoad(gameObject);
		InitData();
	}

	[SerializeField]
	MonsterAbility  m_MonsterAbility;
	public MonsterAbility MonsterAbilityAttr
	{
		get
		{
			return m_MonsterAbility;
		}
	}

	[SerializeField]
	MonsterGrade m_MonsterGradeE;
	public MonsterGrade MonsterGradeEAttr
	{
		get
		{
			return m_MonsterGradeE;
		}
	}

	[SerializeField]
	MonsterGrade m_MonsterGradeA;
	public MonsterGrade MonsterGradeAAttr
	{
		get
		{
			return m_MonsterGradeA;
		}
	}
	
	public void InitData()
	{

		GameObject monsterAbilityGo = new GameObject ("MonsterAbilityData");
		monsterAbilityGo.transform.parent = transform;
		m_MonsterAbility = monsterAbilityGo.AddComponent<MonsterAbility> ();
		m_MonsterAbility.InitWithTableFile (Util.ResPath + "/staticData/" + "monsterAbility.csv");

		GameObject monsterGradeEGo = new GameObject ("MonsterGradeEData");
		monsterGradeEGo.transform.parent = transform;
		m_MonsterGradeE = monsterGradeEGo.AddComponent<MonsterGrade> ();
		m_MonsterGradeE.InitWithTableFile (Util.ResPath + "/staticData/" + "monsterGradeE.csv");

		GameObject monsterGradeAGo = new GameObject ("MonsterGradeAData");
		monsterGradeAGo.transform.parent = transform;
		m_MonsterGradeA = monsterGradeAGo.AddComponent<MonsterGrade> ();
		m_MonsterGradeA.InitWithTableFile (Util.ResPath + "/staticData/" + "monsterGradeE.csv");


	}
	
}
