local setmetatable = setmetatable
local _G = _G
local EffectSet = EffectSet
local EffectDamage = EffectDamage
local EffectApplyBuff = EffectApplyBuff
local EffectSearch = EffectSearch
local EffectPersistent = EffectPersistent

module "EffectConfigTest"

---------------------------------------------------------------------------------------------------
--effect
effect_test01 = {
	id = "e01",
	targetType = _G.TARGET_TARGET,
	damageType = 1,
	amount = 0,
	attackFactor = 0.8,
	isHeal = false,
}
setmetatable(effect_test01, {__index = EffectDamage.EffectDamage})
---------------------------------------------------------------------------------------------------
effect_test02 = {
	id = "e02",
	targetType = _G.TARGET_TARGET,
	damageType = 1,
	amount = 0,
	attackFactor = 0.8,
	isHeal = false,
	effectList ={
		[1] = "e03",
	},
}
setmetatable(effect_test02, {__index = EffectSet.EffectSet})
---------------------------------------------------------------------------------------------------
effect_test03 = {
	id = "e03",
	maxCount = 1,
	targetValidatorList = {
		[1] = _G.IsEnemy,
	},
	searchEffectID = "e04",
	sort,
	compareFun,
	isRandom,
	
	--state data
	sortFunction,
	targetList,--{{target, effectid}}
}
setmetatable(effect_test03, {__index = EffectSearch.EffectSearch})
---------------------------------------------------------------------------------------------------
effect_test04 = {
	id = "e04",
	buffID = "b01",
}
setmetatable(effect_test04, {__index = EffectApplyBuff.EffectApplyBuff})
---------------------------------------------------------------------------------------------------
effect_test05 = {
	id = "e05",
	effectID = "e01",
	delayTime = 100,
	
	periodEffectList = 
	{
		[1] = "e01",
		[10] = "e01",
	},
}
setmetatable(effect_test05, {__index = EffectPersistent.EffectPersistent})
---------------------------------------------------------------------------------------------------