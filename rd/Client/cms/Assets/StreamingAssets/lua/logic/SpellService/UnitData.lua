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
		grade,--品质
		obtainingRate,--得到概率
		property,--五行属性
		--outputExp,--吃掉经验

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
--伤害公式相关
--抗性洗漱
resisitanceK = 1
resisitanceAdjustRatio = 1
-----------------------------------------------------------------------------------------------
UnitData =
{
	guid,
	id,
	disposition,--性格
	
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
	curExp, --当前经验
	injuryRatio,--buff附加受伤比
	camp,--阵营
	
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