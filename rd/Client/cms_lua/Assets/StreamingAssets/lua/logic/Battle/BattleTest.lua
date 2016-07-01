require "logic.Battle.BattleController"

BattleTest = {}

local test = true

BattleData = {
}

local sampleUnit = {
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
	phyAttack = 100,
	magAttack = 100,
	defend = 10,
	
	spellList=
	{
		[1] = "s01",
	},
	battleSpellList = {},
	captainSpell = nil,
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
	shield = nil,
}



local BattleGroup = {
	Enemy = {
		Field = {},
		OffSite = {},
	},
	Player = {
		Field = {},
		OffSite = {},
	}
}

--------------------------Enemy
local enemy1 = BattleUnit:New(clone(sampleUnit))
enemy1.guid = 1
enemy1.name = "enemy1"
enemy1.speed = 50
enemy1.camp  = CAMP_ENEMY
BattleGroup.Enemy.Field[1] = enemy1

local enemy2 = BattleUnit:New(clone(sampleUnit))
enemy2.guid = 2
enemy2.name = "enemy2"
enemy2.speed = 51
enemy2.camp  = CAMP_ENEMY
BattleGroup.Enemy.Field[2] = enemy2

local enemy3 = BattleUnit:New(clone(sampleUnit))
enemy3.guid = 3
enemy3.name = "enemy3"
enemy3.speed = 52
enemy3.camp  = CAMP_ENEMY
BattleGroup.Enemy.Field[3] = enemy3

---------------------------Player
local player1 = BattleUnit:New(clone(sampleUnit))
player1.guid = 11
player1.name = "player1"
player1.speed = 53
player1.camp  = CAMP_PLAYER
BattleGroup.Player.Field[1] = player1

local player2 = BattleUnit:New(clone(sampleUnit))
player2.guid = 12
player2.name = "player2"
player2.speed = 54
player2.camp  = CAMP_PLAYER
BattleGroup.Player.Field[2] = player2

local player3 = BattleUnit:New(clone(sampleUnit))
player3.guid = 13
player3.name = "player3"
player3.speed = 55
player3.camp  = CAMP_PLAYER
BattleGroup.Player.Field[3] = player3

function BattleTest:Test()
	if not test then return end	
	
	BattleController:StartBattle(BattleGroup)
end