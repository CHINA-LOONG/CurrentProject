UnitGradeDataList = 
{
	grade = {
		level=
		{
			strength,
			life,
			intelligence,
			speed,
			resistance,
			endurance,
			goldDropMin,
			goldDropMax,
			expDropMin,
			expDropMax,
			hitRate,
			criticalRate,
			criticalDamageFactor,
			recovery,
			nextLevelExp,
		}
	},
}

--the ratio of the base grade attribute for one unit\
UnitDataList = 
{
    id = 
	{
		grade,--Ʒ��
		obtainingRate,--�õ�����
		property,--��������
		--outputExp,--�Ե�����

		--if has these,criticalRateRatio,criticalDamageRatio,recoveryRatio will have no use
		fixedcriticalRate,
		fixedCriticalDamage,
		fixedRecovery,

		strengthRatio,
		lifeRatio,
		intelligenceRatio,
		speedRatio,
		resistanceRatio,
		enduranceRatio,
		goldDropRatio,
		expRatio,
		hitRatioRatio,
		criticalRateRatio,
		criticalDamageFactorRatio,
		recoveryRatio,
		
		spellList=
		{
		},
		captainSpell,
		evolutionType,
		evolutionList=
		{
			[1]={condition, id},
		},

		prefabName,
	}
    --public string equipID;
    --public string captainSkill;
    --public List<string> skillList;
}
--�˺���ʽ���
--����ϴ��
resisitanceK = 1
resisitanceAdjustRatio = 1
-----------------------------------------------------------------------------------------------
UnitData =
{
	guid,
	id,
	disposition,--�Ը�
	
	level,
	baseStrength,
	baseLife,
	baseIntelligence,
	baseSpeed,
	baseResistance,
	baseEndurance,
	baseHitRate,
	baseCriticalRate,
	baseCriticalDamageFactor,
	baseRecovery,
	
	phyAttack,
	magAttack,
	defend,
	strength,
	life,
	maxLife,
	intelligence,
	speed,
	resistance,
	endurance,
	hitRate,
	criticalRate,
	criticalDamageFactor,
	recovery,
	curExp, --��ǰ����
	injuryRatio,--buff�������˱�
	camp,--��Ӫ
	
	spellList=
	{
		
	},
	captainSpell,
	equipmentList=
	{
	},
	
	--not effectd by equipment
	goldDropMin,
	goldDropMax,
	expDropMin,
	expDropMax,
	
	--state data
	buffList={},
	stateFlags={},
}