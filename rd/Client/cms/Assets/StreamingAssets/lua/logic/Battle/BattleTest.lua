require "logic.Battle.BattleController"

BattleTest = {}

local test = true

BattleData = {
}

local BattleGroup = {
	Enemy = {
		Field = {
			{
				id = 1,
				name = "Enemy1",
				slot = 1,
				speed = 50,
				atk = 10,
				hp = 100,
			},
			{
				id = 2,
				name = "Enemy2",
				slot = 2,
				speed = 51,
				atk = 10,
				hp = 100,
				
			},
			{
				id = 3,
				name = "Enemy3",
				slot = 3,
				speed = 52,
				atk = 10,
				hp = 100,
			},
		},
		OffSite = {
			{
				id = 4,
				name = "Enemy4",
				speed = 53,
				atk = 10,
				hp = 100,
			},
			{
				id = 5,
				name = "Enemy5",
				speed = 54,
				atk = 10,
				hp = 100,
			},
		},
	},
	Player = {
		Field = {
			{
				id = 6,
				name = "Pet1",
				slot = 1,
				speed = 55,
				atk = 10,
				hp = 100,
			},
			{
				id = 7,
				name = "Pet2",
				slot = 2,
				speed = 56,
				atk = 10,
				hp = 100,
			},
			{
				id = 8,
				name = "Pet3",
				slot = 3,
				speed = 57,
				atk = 10,
				hp = 100,
			},
		},
		OffSite = {
			{
				id = 9,
				name = "Pet4",
				speed = 58,
				atk = 10,
				hp = 100,
			},
			{
				id = 10,
				name = "Pet5",
				speed = 59,
				atk = 10,
				hp = 100,
			},
			{
				id = 11,
				name = "Assistant",
				speed = 60,
				atk = 10,
				hp = 100,
			},
		},
	}
}

function BattleTest:Test()
	if not test then return end	
	
	BattleController:StartBattle(BattleGroup)
end