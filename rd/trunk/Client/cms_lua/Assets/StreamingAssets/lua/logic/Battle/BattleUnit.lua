BattleUnit = {
	guid,
	id,
	name,
	disposition,--�Ը�
	curExp, --��ǰ����
	injuryRatio,--buff�������˱�
	camp,--��Ӫ
	
	--base attribute
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
	
	--current attribute
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
	
	--second level attribute
	phyAttack,
	magAttack,
	defend,
	
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
	flags={},
	shield,
	
	speedCount = 0,
	order = 0,
}

BattleUnit.__index = BattleUnit

function BattleUnit:New(o)
	o = o or {}    --��ʼ��self�����û����䣬��ô���������Ķ���ı䣬�������󶼻�ı�
    setmetatable(o, BattleUnit)  --��self��Ԫ���趨ΪClass
    return o    --��������
end

function BattleUnit:Move( ... )
	-- body
end

function BattleUnit:CanCastSuperSpell( ... )
	-- body
end

function BattleUnit:CastSpell( ... )
	-- body
end

function BattleUnit:GetTarget( ... )
	-- body
end

function BattleUnit:IsPlayer( ... )
	-- body
end