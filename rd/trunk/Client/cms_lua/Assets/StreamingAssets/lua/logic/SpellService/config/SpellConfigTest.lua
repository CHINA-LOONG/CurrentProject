local setmetatable = setmetatable
local _G = _G
local Spell = Spell

module "SpellConfigTest"
---------------------------------------------------------------------------------------------------
--spell
spell_test01 = {
	id = "s01",
	category = "attack",
	rootEffectID = "e05",
	
	prepareTime = 24,
	validateTime = 24,
	
	costUse = {},
}
setmetatable(spell_test01, {__index = Spell.Spell})
---------------------------------------------------------------------------------------------------

