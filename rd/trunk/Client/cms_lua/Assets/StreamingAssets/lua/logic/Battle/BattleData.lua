-- 静态数据，即数据表导出数据
-- 怪物
MonsterData = {
	id,
	name,
	prefab,
	
	hp,
	patk,
	matk,
	pdef,
	mdef,
	speed,
}

-- 对局
BattleData = {
	id,
	-- 小怪、boss、稀有怪
	type,
	monsters = {
		{
			id,
			amount,
		}
	},
	processes = {},
}

-- 进程
ProcessData = {}