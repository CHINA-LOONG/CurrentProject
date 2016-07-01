require "logic.Battle.BattleConst"
require "logic.Battle.BattleUnit"
require "logic.EventManager.EventManager"
require "logic.EventManager.EventType"

local EventManager = EventManager.EventManager
local Debug = DebugLog

BattleProcess = {
	battleEndCB = nil,
	units = nil,
	targetUnit = nil,
	startTime,
}

BattleProcess.__index = BattleProcess

function BattleProcess:New(o) 
    o = o or {}    --初始化self，如果没有这句，那么类所建立的对象改变，其他对象都会改变
    setmetatable(o, BattleProcess)  --将self的元表设定为Class
    return o    --返回自身
end

function BattleProcess:Start(units, OnEndProcess)
	self.battleEndCB = OnEndProcess;
	print("<Battle.Process>Start process")
	
	SpellService:Init(self)	

	self:InitData(units)
	
	self:RegEvent()	
	
	coroutine.start(function ()
		self:Process()
	end)
end

function BattleProcess:InitData(units)
	self.units = units
	
	for k, v in pairs(self.units.Enemy.Field) do
		v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		v.order = SPEED_K/v.speedCount
		
		SpellService:InitUnit(v)
		print(string.format("Enemy %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
	
	for k, v in pairs(self.units.Player.Field) do
		v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		v.order = SPEED_K/v.speedCount
		
		SpellService:InitUnit(v)
		print(string.format("Player's Monster %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
	
	self.targetUnit = self.units.Enemy.Field[1]
	self.startTime = Time.timeSinceLevelLoad
end

function BattleProcess:RegEvent()
	print("<Battle.Process>Registering events")
	EventManager:RegisterEvent(EVENT_FIRE_SPELL, self.OnFireSpell, self)

	EventManager:RegisterEvent(EVENT_LIFE_CHANGE, self.OnLifeChange, self)

	EventManager:RegisterEvent(EVENT_ENERGY_CHANGE, self.OnEnergyChange, self)

	EventManager:RegisterEvent(EVENT_UNIT_DEAD, self.OnUnitDead, self)

	EventManager:RegisterEvent(EVENT_BUFF, self.OnEventBuff, self)
end

function BattleProcess:Process()
	if self:HasProcessAnim() then
		self:PlayProcessAnim()
	end
	
	if self:HasPreAnim() then
		self:PlayPreAnim()
	end
	
	if self:IsClearBuff() then
		self:ClearBuff()
	end
	
	self:PlayStartFightAnim()
	
	self:RefreshEnemyState()
	
	self:StartFight()

end

function BattleProcess:HasProcessAnim()
	return true
end

function BattleProcess:PlayProcessAnim()
	print("<Battle.Process>Playing Process Animation...Waiting for 3s...")
	coroutine.wait(3)
	print("<Battle.Process>Playing Process Animation Finished...")
end

function BattleProcess:HasPreAnim( ... )
	return true
end

function BattleProcess:PlayPreAnim( ... )
	print("<Battle.Process>Playing Pre Animation...Waiting for 3s...")
	coroutine.wait(3)
	print("<Battle.Process>Playing Process Animation Finished...")
end

function BattleProcess:IsClearBuff( ... ) 
	return true
end

function BattleProcess:ClearBuff( ... )
	print("<Battle.Process>Clearing buff...")
	coroutine.wait(2)
end

function BattleProcess:PlayStartFightAnim()
	print("<Battle.Process>3...")
	coroutine.wait(1)
	print("<Battle.Process>2...")
	coroutine.wait(1)
	print("<Battle.Process>1...")
	coroutine.wait(1)
	print("<Battle.Process>Fight!!!")
end

function BattleProcess:RefreshEnemyState( ... )
	print("Refresh Enemy state")
end

-- 一个单位开始行动
function BattleProcess:StartFight( ... )
	local unit = self:GetNextMoveUnit()
	print("<Battle.Process>Unit "..unit.name.." is moving...")
	local request = {caster=unit, target=self.targetUnit, spellID=unit.spellList[1], triggerTime=Time.timeSinceLevelLoad}
	SpellService:SpellRequest(request)	
end

-- 一个单位行动结束
function BattleProcess:OnFightOver(movedUnit)
	self:ReCalcSequence(movedUnit.guid)
	
	if not self:IsProcessOver() then
		self:StartFight()
	else			
		warn("<Battle.Process>End process")
		self:battleEndCB()
	end
end

-- 判断当前进程是否结束，检查是否有一方全部死光
function BattleProcess:IsProcessOver()
	if self:IsEnemyAllDead() or self:IsPlayerAllDead() then
		return true
	end
	
	return false
end

function BattleProcess:IsEnemyAllDead()
	for k, v in pairs(self.units.Enemy.Field) do
		if v then
			return false
		end
	end

	return true
end

function BattleProcess:IsPlayerAllDead()
	for k, v in pairs(self.units.Player.Field) do
		if v then
			return false
		end
	end

	return true
end

function BattleProcess:OnRespawnEnemy( ... )
	
end

function BattleProcess:HasAviliablePet( ... )
	-- body
end

function BattleProcess:SwitchPet(id)
	-- body
end

function BattleProcess:SwitchRandomPet()
	-- body
end

-- check if there are enemy monsters to spawn
function BattleProcess:HasEnemyToSpawn( ... )
	-- body
end

function BattleProcess:CreateMonster( ... )
	-- body
end

function BattleProcess:GetNextMoveUnit()
	local fastestUnit;
	local fastestOrder = 10000;
	for k, v in pairs(self.units.Enemy.Field) do
		if v and v.order < fastestOrder then
			fastestUnit = v
			fastestOrder = v.order
		end
	end
	
	for k, v in pairs(self.units.Player.Field) do
		if v and v.order < fastestOrder then
			fastestUnit = v
			fastestOrder = v.order
		end
	end
	
	return fastestUnit
end

function BattleProcess:ReCalcSequence(movedUnitId)
	for k, v in pairs(self.units.Enemy.Field) do
		if v and v.guid == movedUnitId then
			v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		else
			v.speedCount = v.speedCount + v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		end		
		v.order = SPEED_K/v.speedCount
		print(string.format("Enemy %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
	
	for k, v in pairs(self.units.Player.Field) do
		if v and v.guid == movedUnitId then
			v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		else
			v.speedCount = v.speedCount + v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		end		
		v.order = SPEED_K/v.speedCount
		print(string.format("Player's Monster %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
end

function BattleProcess:GetBattleUnits()
	return self.units.Player.Field, self.units.Enemy.Field
end

function BattleProcess:GetUnitById(id)
	for k, v in pairs(self.units.Enemy.Field) do
		if v.guid == id then
			return v
		end
	end
	
	for k, v in pairs(self.units.Player.Field) do
		if v.guid == id then
			return v
		end
	end

	return nil
end

function BattleProcess:GetUnitSlot(unit)
	for k, v in pairs(self.units.Enemy.Field) do
		if v.guid == unit.guid then
			return k
		end
	end
	
	for k, v in pairs(self.units.Player.Field) do
		if v.guid == unit.guid then
			return k
		end
	end

	return 0
end

function BattleProcess:GetLiveEnemyUnit( ... )
	for k, v in pairs(self.units.Enemy.Field) do
		if v then
			return v
		end
	end
end

function BattleProcess:ChangeTarget(unit)
	self.targetUnit = unit
	Debug:Log(string.format("<BattleProcess>ChangeTarget to %s", unit.name))
end

---------------------------Event--------------------------
function BattleProcess:OnFireSpell(...)
	local args = ...
	local movedUnitId = args.casterID
	local movedUnit = self:GetUnitById(movedUnitId)
	Debug:Log("OnFireSpell")
	
	coroutine.start(function ()
		coroutine.wait(3)
		self:OnFightOver(movedUnit)
	end)
end

function BattleProcess:OnLifeChange(...)
	local args = ...
	Debug:Log("OnLifeChange")
end

function BattleProcess:OnEnergyChange(...)
	local args = ...
	Debug:Log("OnEnergyChange")	
end

function BattleProcess:OnUnitDead(...)
	local args = ...
	local deathID = args.deathID
	local deathUnit = self:GetUnitById(deathID)
	warn(string.format("<BattleProcess>OnUnitDead %s", deathUnit.name))

	local slot = self:GetUnitSlot(deathUnit)
	if deathUnit.camp == CAMP_ENEMY then
		self.units.Enemy.Field[slot] = nil
	else
		self.units.Player.Field[slot] = nil
	end

	-- 查找是否有可上场怪物


	-- 判断进程是否结束
	if self:IsProcessOver() then
		warn("<Battle.Process>End process")
		self:battleEndCB()
		return
	end

	-- 当前目标是否需要切换
	if deathUnit.guid == self.targetUnit.guid then
		self:ChangeTarget(self:GetLiveEnemyUnit())
	end

end

function BattleProcess:OnEventBuff(...)
	local args = ...
	Debug:Log("OnEventBuff")
end