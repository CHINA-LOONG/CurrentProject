BattleProcess = {}

BattleProcess.__index = BattleProcess

function BattleProcess:New() 
    local self = {};    --初始化self，如果没有这句，那么类所建立的对象改变，其他对象都会改变
    setmetatable(self, BattleProcess);  --将self的元表设定为Class
    return self;    --返回自身
end

function BattleProcess:Start()
	print("Start process");
end

function BattleProcess:HasPreAnim( ... )
	-- body
end

function BattleProcess:PlayAnim( ... )
	-- body
end

function BattleProcess:CheckBuff( ... )
	-- body
end

function BattleProcess:RemoveBuff( ... )
	-- body
end

function BattleProcess:RefreshEnemyState( ... )
	-- body
end

function BattleProcess:Timeline( ... )
	-- body
end

function BattleProcess:OnUnitDead( unit )
	-- body
end

function BattleProcess:OnRespawnEnemy( ... )
	-- body
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

