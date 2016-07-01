local string = string
local math = math
local pairs = pairs
local setmetatable = setmetatable
local table = table
local Log = DebugLog
local _G = _G

module "Spell"
---------------------------------------------------------------------------------------------------
Spell =
{
	id,
	prepareTime,
	validateTime,
	coolDownStart,
	costUse,
	rootEffectID,
	generateEnergy,
	
	--state data
	applyTime,
	caster,
	target,
	instanceID,
	nextValidateRound,
	owner,
}
---------------------------------------------------------------------------------------------------
function Spell:Init()
	local info = string.format("init spell %s\n", self.id)
	Log:Log(info)
	
	nextValidateRound = coolDownStart
end
---------------------------------------------------------------------------------------------------
function Spell:Apply(triggerTime)
	self.owner:TriggerFireSpell({triggerTime = triggerTime, spellID = self.id, casterID = self.caster.guid, castResult = _G.SPELL_CAST_OK})
	self.applyTime = triggerTime
	local rootEffect = self.owner:GetEffect(self.rootEffectID)
	if rootEffect then
		rootEffect.applyTime = triggerTime
		rootEffect:SetOwnedSpell(self)
		rootEffect:Apply(triggerTime)
	end
end
---------------------------------------------------------------------------------------------------
function Spell:TakeCost(curTime)
	if self.costUse == nil then
		return
	end
	--check cost
	if self.costUse.cd and self.costUse.cd > 0 then
		
	end
	--generateEnergy
	if self.caster then
		
	end
end
---------------------------------------------------------------------------------------------------
function Spell:GetCurState(curTime)
	local curState = _G.SPELL_STATE_PREPARE
	if curTime >= self.applyTime + self.prepareTime then
		self.curState = _G.SPELL_STATE_FINISH
	end
	return curState
end
---------------------------------------------------------------------------------------------------
function Spell:CheckActive()
	return true
end
---------------------------------------------------------------------------------------------------
function Spell:CheckCast(triggerTime)
	return _G.SPELL_CAST_OK
end
---------------------------------------------------------------------------------------------------