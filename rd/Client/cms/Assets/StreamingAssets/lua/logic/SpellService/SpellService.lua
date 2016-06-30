---------------------------------------------------------------------------------------------------
--load original spell data (convert from excel)
function AddOriginalDatas(spellService)
	
	for key, spell in pairs(SpellFactory) do
		if key ~= "_NAME" and key ~= "_M" and key ~= "_PACKAGE" then
			spellService:AddOriginalSpell(spell)
		end
	end
	
	for key, effect in pairs(EffectFactory) do
		if key ~= "_NAME" and key ~= "_M" and key ~= "_PACKAGE" then
			spellService:AddOriginalEffect(effect)
		end
	end
	
	for key, buff in pairs(BuffFactory) do
		if key ~= "_NAME" and key ~= "_M" and key ~= "_PACKAGE" then
			spellService:AddOriginalBuff(buff)
		end
	end

end
---------------------------------------------------------------------------------------------------
--cache spell config data
SpellService = 
{
	--original data list
	spellList = {},
	buffList = {},
	effectList = {},
}
---------------------------------------------------------------------------------------------------
function SpellService:New()
	local instance = 
	{
		activeEffectList = {},
	}
	setmetatable(instance, {__index = self})
	return instance
end
---------------------------------------------------------------------------------------------------
function SpellService:Init()
	AddOriginalDatas(self)
end
---------------------------------------------------------------------------------------------------
function SpellService:Destroy()
	self.activeEffectList = nil
end
---------------------------------------------------------------------------------------------------
function SpellService:SpellRequest(request)
	print("SpellService=>SpellRequest()\n")
	do return end

	print(string.format("spell request unitID=%s spell=%s \n",request.caster.id, request.spell.id));
	local caster = request.caster
	if caster == nil then
		print(string.format("error, casterID=%s can not faind \n", request.caster.id))
		return
	end
	
	--TODO: if current spell disapel buff, which one first, dot or disapel
	--calculate buff
	--take periodic effect, and then check if buff finished
	local activeBuffList = caster.buffList;
	for key, buff in pairs(activeBuffList) do
		buff:Periodic(request.triggerTime)
		buff:Update(request.triggerTime)
		if buff.isFinish then
			activeBuffList[key] = nil
		end
	end
	
	--apply spell request
	self:CastSpell(request)
end
---------------------------------------------------------------------------------------------------
function SpellService:Update(triggerTime)
end
---------------------------------------------------------------------------------------------------
function SpellService:CastSpell(request)
	print("SpellService=>CastSpell()\n")
	do return end
	
	local caster = request.caster
	local target = request.target
	--check if target is locked
	if caster.flags.lockTarget then
		target = caster.flags.lockTarget
	end
	
	local spell = request.spell
	spell.caster = caster
	spell.target = target
	local checkResult = spell:CheckCast(request.triggerTime)
	if checkResult == SPELL_CAST_OK then
		spell:Apply(request.triggerTime)
	end
end
---------------------------------------------------------------------------------------------------
function SpellService:OnUnitDead(self.target)
	
end
---------------------------------------------------------------------------------------------------
function SpellService:GetAllUnits()
end
---------------------------------------------------------------------------------------------------
