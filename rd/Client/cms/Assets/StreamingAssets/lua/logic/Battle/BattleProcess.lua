require "logic.Battle.BattleConst"

BattleProcess = {
	battleEndCB = nil,
	units = nil,
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

	self:InitData(units)
	
	coroutine.start(function ()
		self:Process()
	end)
end

function BattleProcess:InitData(units)
	self.units = units
	
	for k, v in pairs(self.units.Enemy.Field) do
		v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		v.order = SPEED_K/v.speedCount
		print(string.format("Enemy %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
	
	for k, v in pairs(self.units.Player.Field) do
		v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		v.order = SPEED_K/v.speedCount
		print(string.format("Player's Monster %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
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
	
	print("<Battle.Process>End process")
	self:battleEndCB()
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
	coroutine.wait(1)
	
	self:OnFightOver(unit)
end

-- 一个单位行动结束
function BattleProcess:OnFightOver(movedUnit)
	self:ReCalcSequence(movedUnit.id)
	
	if not self:IsProcessOver() then
		self:StartFight()
	end
end

-- 判断当前进程是否结束，检查是否有一方全部死光
function BattleProcess:IsProcessOver()
	if #self.units.Enemy.Field == 0 then
		return true
	end
	
	if #self.units.Player.Field == 0 then
		return true
	end
	
	return false
end

function BattleProcess:OnUnitDead( unit )
	
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
		if v.order < fastestOrder then
			fastestUnit = v
			fastestOrder = v.order
		end
	end
	
	for k, v in pairs(self.units.Player.Field) do
		if v.order < fastestOrder then
			fastestUnit = v
			fastestOrder = v.order
		end
	end
	
	return fastestUnit
end

function BattleProcess:ReCalcSequence(movedUnitId)
	for k, v in pairs(self.units.Enemy.Field) do
		if v.id == movedUnitId then
			v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		else
			v.speedCount = v.speedCount + v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		end		
		v.order = SPEED_K/v.speedCount
		print(string.format("Enemy %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
	
	for k, v in pairs(self.units.Player.Field) do
		if v.id == movedUnitId then
			v.speedCount = v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		else
			v.speedCount = v.speedCount + v.speed * math.Random(SPEED_FACTOR_MIN, SPEED_FACTOR_MAX)
		end		
		v.order = SPEED_K/v.speedCount
		print(string.format("Player's Monster %s current state: SpeedCount: %f, order: %f", v.name, v.speedCount, v.order))
	end
end

