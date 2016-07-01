local setmetatable = setmetatable
local _G = _G
local EffectSet = EffectSet
local EffectDamage = EffectDamage
local EffectApplyBuff = EffectApplyBuff
local EffectSearch = EffectSearch

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
}
setmetatable(effect_test02, {__index = EffectSet.EffectSet})
---------------------------------------------------------------------------------------------------
effect_test03 = {
	id = "e03",
	targetType = _G.TARGET_TARGET,
	damageType = 1,
	amount = 0,
	attackFactor = 0.8,
	isHeal = false,
}
setmetatable(effect_test03, {__index = EffectSearch.EffectSearch})
---------------------------------------------------------------------------------------------------
effect_test04 = {
	id = "e04",
	targetType = _G.TARGET_TARGET,
	damageType = 1,
	amount = 0,
	attackFactor = 0.8,
	isHeal = false,
}
setmetatable(effect_test04, {__index = EffectApplyBuff.EffectApplyBuff})
---------------------------------------------------------------------------------------------------