--require "logic.SpellService.SpellLoader"
require "logic.SpellService.SpellService"

---------------------------------------------------------------------------------------------------
game = {
}
--temp unit just for test
Unit1 =
{
	guid = 1,
	id = "unit1",
	disposition = 0,--性格
	curExp = 0, --当前经验
	injuryRatio = 0,--buff附加受伤比
	camp = 0,--阵营
	
	--base attribute
	level = 1,
	baseStrength = 10,
	baseLife = 10,
	baseIntelligence = 10,
	baseSpeed = 10,
	baseResistance = 10,
	baseEndurance = 10,
	baseHitRate = 10,
	baseCriticalRate = 0.2,
	baseCriticalDamageFactor = 1.2,
	baseRecovery = 10,
	
	--current attribute
	strength = 10,
	life = 10,
	maxLife = 10,
	intelligence = 10,
	speed = 10,
	resistance = 10,
	endurance = 10,
	hitRate = 0.8,
	criticalRate = 0.2,
	criticalDamageFactor = 1.2,
	recovery = 10,
	
	--second level attribute
	phyAttack = 10,
	magAttack = 10,
	defend = 10,
	
	spellList=
	{
		["s01"] = 1,
	},
	captainSpell,
	equipmentList=
	{
	},
	
	--not effectd by equipment
	goldDropMin = 10,
	goldDropMax = 20,
	expDropMin = 10,
	expDropMax = 20,
	
	--state data
	buffList={},
	flags={},
	shield,
}
Unit2 =
{
	guid = 2,
	id = "unit2",
	disposition = 0,--性格
	curExp = 0, --当前经验
	injuryRatio = 0,--buff附加受伤比
	camp = 1,--阵营
	
	--base attribute
	level = 1,
	baseStrength = 10,
	baseLife = 10,
	baseIntelligence = 10,
	baseSpeed = 10,
	baseResistance = 10,
	baseEndurance = 10,
	baseHitRate = 10,
	baseCriticalRate = 0.2,
	baseCriticalDamageFactor = 1.2,
	baseRecovery = 10,
	
	--current attribute
	strength = 10,
	life = 10,
	maxLife = 10,
	intelligence = 10,
	speed = 10,
	resistance = 10,
	endurance = 10,
	hitRate = 0.8,
	criticalRate = 0.2,
	criticalDamageFactor = 1.2,
	recovery = 10,
	
	--second level attribute
	phyAttack = 10,
	magAttack = 10,
	defend = 10,
	
	spellList=
	{
		["s01"] = 1,
	},
	captainSpell,
	equipmentList=
	{
	},
	
	--not effectd by equipment
	goldDropMin = 10,
	goldDropMax = 20,
	expDropMin = 10,
	expDropMax = 20,
	
	--state data
	buffList={},
	flags={},
	shield,
}
---------------------------------------------------------------------------------------------------
local this
SpellTest = {}
---------------------------------------------------------------------------------------------------
function SpellTest.SpellInit()
	print("SpellTest->SpellInit()\n")
	this = SpellTest
	this.ss = SpellService:New()
	this.ss:Init(game)
	
	local s1 = this.ss:GetSpell("s01")
	local request = {
		triggerTime = 0,
		caster = Unit1,
		target = Unit2,
		spell = s1,
	}
	
	this.ss:SpellRequest(request)
end
---------------------------------------------------------------------------------------------------