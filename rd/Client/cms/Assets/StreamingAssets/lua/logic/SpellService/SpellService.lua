require "logic.SpellService.Macros"
require "logic.SpellService.Function"
require "logic.SpellService.Spell"
require "logic.SpellService.Effect"
require "logic.SpellService.EffectDamage"
require "logic.SpellService.Buff"
require "logic.SpellService.EffectSet"
require "logic.SpellService.EffectSearch"
require "logic.SpellService.EffectApplyBuff"
require "logic.SpellService.SpellLoader"

---------------------------------------------------------------------------------------------------
function AddData(dataList, data)
	if not(data) then
		do return end
	end
	
	local dataID = data.id
	local debugMsg = string.format("add data id=%q\n", dataID)
	print(debugMsg)
	--log(_G.LOG_CATEGORY_SPELL, debugMsg)
	
	if dataList[dataID] ~= nil then
		--log(_G.LOG_CATEGORY_SPELL, "duplicate")
	else
		dataList[dataID] = data
	end
end
---------------------------------------------------------------------------------------------------
function GetData(dataList, dataID, owner)
	if dataID == nil then
		return nil
	end
	
	local originalData = dataList[dataID]
	local curData
	if originalData ~= nil then
		curData = {}
		_G.setmetatable(curData, {__index = originalData})
		--curData:SetOwner(owner)
		curData:Init()
		curData.owner = owner
	else
		local debugMsg = string.format("get data id=%q failed\n", dataID)
		print(debugMsg)
		--log(_G.LOG_CATEGORY_SPELL, debugMsg)
	end

	return curData
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
	print("SpellService New!")
	
	local instance = 
	{
		activeEffectList = {},
	}
	setmetatable(instance, {__index = self})
	return instance
end
---------------------------------------------------------------------------------------------------
function SpellService:AddOriginalSpell(originalSpell)
	AddData(self.spellList, originalSpell)
end
---------------------------------------------------------------------------------------------------
function SpellService:GetSpell(spellID)
	return GetData(self.spellList, spellID, self)
end
---------------------------------------------------------------------------------------------------
function SpellService:AddOriginalEffect(originalEffect)
	AddData(self.effectList, originalEffect)
end
---------------------------------------------------------------------------------------------------
function SpellService:GetEffect(effectID)
	return GetData(self.effectList, effectID, self)
end
---------------------------------------------------------------------------------------------------
function SpellService:AddOriginalBuff(originalBuff)
	AddData(self.buffList, originalBuff)
end
---------------------------------------------------------------------------------------------------
function SpellService:GetBuff(buffID)
	return GetData(self.buffList, buffID, self)
end
---------------------------------------------------------------------------------------------------
function SpellService:CastSpell(request)
	print("SpellService=>CastSpell()\n")
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
	else
		print("check spell cast failed")
	end
end
---------------------------------------------------------------------------------------------------
function SpellService:Init(game)
	print("begin SpellService init\n")
	
	AddOriginalDatas(self)
	self.game = game
end
---------------------------------------------------------------------------------------------------
function SpellService:Destroy()
	self.activeEffectList = nil
end
---------------------------------------------------------------------------------------------------
function SpellService:Update(triggerTime)
end
---------------------------------------------------------------------------------------------------
function SpellService:OnUnitDead(target)
	
end
---------------------------------------------------------------------------------------------------
function SpellService:GetAllUnits()
end
---------------------------------------------------------------------------------------------------
function SpellService:InterruptSpell()
end
---------------------------------------------------------------------------------------------------
function SpellService:SpellRequest(request)
	print(string.format("spell request caster=%d target=%d spell=%s \n",request.caster.guid, request.target.guid, request.spell.id));
	local caster = request.caster
	if caster == nil then
		print(string.format("error, casterID=%s can not faind \n", request.caster.guid))
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
