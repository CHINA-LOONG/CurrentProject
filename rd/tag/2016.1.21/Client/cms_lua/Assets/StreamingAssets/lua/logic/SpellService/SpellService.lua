require "logic.EventManager.EventManager"
require "logic.EventManager.EventType"
require "logic.SpellService.Macros"
require "logic.SpellService.Function"
require "logic.SpellService.Spell"
require "logic.SpellService.Effect"
require "logic.SpellService.EffectDamage"
require "logic.SpellService.Buff"
require "logic.SpellService.EffectSet"
require "logic.SpellService.EffectSearch"
require "logic.SpellService.EffectApplyBuff"
require "logic.SpellService.EffectPersistent"
require "logic.SpellService.SpellLoader"

local EventManager = EventManager.EventManager
local Log = DebugLog

---------------------------------------------------------------------------------------------------
function AddData(dataList, data)
	if not(data) then
		do return end
	end
	
	local dataID = data.id
	local debugMsg = string.format("add data id=%q\n", dataID)
	Log:Log(debugMsg)
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
		Log:Log(debugMsg)
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
	Log:Log("SpellService New!")
	
	local instance = 
	{
		activeEffectList = {},
	}
	setmetatable(instance, {__index = self})
	EventManager:RegisterEvent(_G.EVENT_UNIT_DEAD, instance.HandleUnitDead, instance)
	--EventManager:RegisterEvent(_G.EVENT_FIRE_SPELL, instance.HandleFireSpell1,instance)
	--EventManager:UnRegisterEvent(_G.EVENT_FIRE_SPELL, instance.HandleFireSpell1,instance)
	--EventManager:RegisterEvent(_G.EVENT_FIRE_SPELL, instance.HandleFireSpell2)
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
function SpellService:Init(game)
	Log:Log("begin SpellService init\n")
	
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
	--test only
	return self.playerList,self.enemyList
	
	--return self.game:GetBattleUnits()
	
end
function SpellService:GetUnit(id)
end
---------------------------------------------------------------------------------------------------
function SpellService:AddActiveBuff(buff)
end
---------------------------------------------------------------------------------------------------
function SpellService:InterruptSpell()
end
---------------------------------------------------------------------------------------------------
function SpellService:SpellRequest(request)
	Log:Log(string.format("spell request caster=%d target=%d spell=%s \n",request.caster.guid, request.target.guid, request.spellID));
	local caster = request.caster
	if caster == nil then
		Log:LogError(string.format("error, casterID=%s can not faind \n", request.caster.guid))
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
function SpellService:CastSpell(request)
	Log:Log("SpellService=>CastSpell()\n")
	local caster = request.caster
	local target = request.target
	--check if target is locked
	if caster.flags.lockTarget then
		target = caster.flags.lockTarget
	end

	--no spell, may controlled
	local spell = caster.battleSpellList[request.spellID]
	if not(spell) then
		do return end
	end
	
	spell.caster = caster
	spell.target = target
	local checkResult = spell:CheckCast(request.triggerTime)
	if checkResult == SPELL_CAST_OK then
		spell:Apply(request.triggerTime)
		
	else
		Log:Log("check spell cast failed")
	end
end
---------------------------------------------------------------------------------------------------
function SpellService:InitUnit(unit)
	local spellList = unit.spellList
	for _, spellID in pairs(spellList) do
		local curSpell = self:GetSpell(spellID)
		if curSpell then
			unit.battleSpellList[spellID] = curSpell
		end
	end
end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerFireSpell(data)
	local eventArgs = {}
	eventArgs.triggerTime = data.triggerTime
	eventArgs.spellID = data.spellID
	eventArgs.casterID = data.casterID
	eventArgs.castResult = data.castResult
	EventManager:TriggerEvent(_G.EVENT_FIRE_SPELL, eventArgs)
	
	Log:Log(string.format("%d cast spell %s at time %d", eventArgs.casterID, eventArgs.spellID, eventArgs.triggerTime))
end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerLifeChange(data)
	local eventArgs = {}
	eventArgs.triggerTime = data.triggerTime
	eventArgs.casterID = data.casterID
	eventArgs.targetID = data.targetID
	eventArgs.isCritical = data.isCritical
	eventArgs.lifeChange = data.lifeChange
	eventArgs.lifeCurrent = data.lifeCurrent
	eventArgs.lifeMax = data.lifeMax
	EventManager:TriggerEvent(_G.EVENT_LIFE_CHANGE, eventArgs)
	
	
	Log:Log(string.format("%d make %d damage(heal) to %d at time %d",
								eventArgs.casterID, eventArgs.lifeChange, 
								eventArgs.targetID, eventArgs.triggerTime
								)
			)
end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerEnergyChange(data)
	local eventArgs = {}
	eventArgs.triggerTime = data.triggerTime
	eventArgs.curEnergy = data.curEnergy
	eventArgs.maxEnergy = data.maxEnergy
	eventArgs.targetID = data.targetID
	EventManager:TriggerEvent(_G.EVENT_ENERGY_CHANGE, eventArgs)
end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerUnitDead(data)
	local eventArgs = {}
	eventArgs.triggerTime = data.triggerTime
	eventArgs.casterID = data.casterID
	eventArgs.deathID = data.deathID
	EventManager:TriggerEvent(_G.EVENT_UNIT_DEAD, eventArgs)
	
	Log:Log(string.format("%d killed %d at time %d", eventArgs.casterID, eventArgs.deathID, eventArgs.triggerTime))
end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerBuff(data)
	local eventArgs = {}
	eventArgs.triggerTime = data.triggerTime
	eventArgs.casterID = data.casterID
	eventArgs.targetID = data.targetID
	eventArgs.buffID = data.buffID
	eventArgs.isAdd = data.isAdd
	EventManager:TriggerEvent(_G.EVENT_BUFF, eventArgs)
	
	if eventArgs.isAdd then
		Log:Log(string.format("%d apply buff(%s) to %d at time %d", eventArgs.casterID, eventArgs.buffID, eventArgs.targetID, eventArgs.triggerTime))
	else
		Log:Log(string.format("%d's buff(%s) is removed at time %d", eventArgs.targetID, eventArgs.buffID, eventArgs.triggerTime))
	end
end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerEffect(data)
	local eventArgs = {}
	eventArgs.triggerTime = data.triggerTime

end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerMiss(data)
end
---------------------------------------------------------------------------------------------------
function SpellService:TriggerImmune(data)
end
---------------------------------------------------------------------------------------------------
--test only
---------------------------------------------------------------------------------------------------
function SpellService:HandleUnitDead(...)
	do return end
	local eventArgs = ...
	local info = string.format("%d make %d dead at time %d", eventArgs.casterID, eventArgs.deathID, eventArgs.triggerTime)
	Log:Log(info)
	do return end
	
	--NOTEO: if unit get from level,and level register the event also
	--TODO:Remove unit Buff
	local deathUnit = self:GetUnit(eventArgs.deathID)
	if deathUnit and deathUnit.buffList then
		for _, buff in pairs(deathUnit.buffList) do
			buff:Remove(_G.BUFF_REMOVED_TARGET_DEAD, eventArgs.triggerTime)
		end	
	end
end
---------------------------------------------------------------------------------------------------
function SpellService:HandleFireSpell1(...)
	Log:Log("test fire spell1 callback")
end
function SpellService:HandleFireSpell2(...)
	Log:Log("test fire spell2 callback")
end
---------------------------------------------------------------------------------------------------
