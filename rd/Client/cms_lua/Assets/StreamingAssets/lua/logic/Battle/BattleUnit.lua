BattleUnit = {
	guid,
	id,
	name,
	disposition,--性格
	curExp, --当前经验
	injuryRatio,--buff附加受伤比
	camp,--阵营
	
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
	o = o or {}    --初始化self，如果没有这句，那么类所建立的对象改变，其他对象都会改变
    setmetatable(o, BattleUnit)  --将self的元表设定为Class
    return o    --返回自身
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