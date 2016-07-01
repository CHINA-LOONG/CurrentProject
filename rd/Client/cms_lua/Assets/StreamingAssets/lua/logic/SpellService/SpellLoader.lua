--test config
require "logic.SpellService.config.SpellConfigTest"
require "logic.SpellService.config.EffectConfigTest"
require "logic.SpellService.config.BuffConfigTest"

---------------------------------------------------------------------------------------------------
function AddOriginalDatas(spellService)
	--------------------------------------------------------------------------------------
	--≤‚ ‘≈‰÷√1
	--------------------------------------------------------------------------------------
	for key, spell in pairs(SpellConfigTest) do
		if key ~= "_NAME" and key ~= "_M" and key ~= "_PACKAGE" then
			spellService:AddOriginalSpell(spell)
		end
	end
	
	for key, effect in pairs(EffectConfigTest) do
		if key ~= "_NAME" and key ~= "_M" and key ~= "_PACKAGE" then
			spellService:AddOriginalEffect(effect)
		end
	end
	
	for key, buff in pairs(BuffConfigTest) do
		if key ~= "_NAME" and key ~= "_M" and key ~= "_PACKAGE" then
			spellService:AddOriginalBuff(buff)
		end
	end
end
--------------------------------------------------------------------------------------