local setmetatable = setmetatable
local _G = _G
local Buff = Buff

module "BuffConfigTest"
---------------------------------------------------------------------------------------------------
--buff
buff_test01 = {
	id = "b01",
	category = "negative",
	
	duration = 4,
	period = 1,
	effectPeriodicID = "e01",
	periodCount = 1,
	maxTotalStack = 1,
	maxPerStack = 1,
	dispelLevel = 0,
	unitAttributeModify =
	{
		["attackFactor"] = 0.5,
		["injuryRate"] = 0.5,
	},
	--flags
	coexist = false,
}
setmetatable(buff_test01, {__index = Buff.Buff})
---------------------------------------------------------------------------------------------------