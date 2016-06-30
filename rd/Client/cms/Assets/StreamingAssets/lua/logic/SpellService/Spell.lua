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
}
---------------------------------------------------------------------------------------------------
function Spell:Init()
	nextValidateRound = coolDownStart
end
---------------------------------------------------------------------------------------------------
function Spell:Apply(triggerTime)
	self.applyTime = triggerTime
	local rootEffect = SpellService:GetEffect(rootEffectID)
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
	local curState = SPELL_STATE_PREPARE
	if curTime >= self.applyTime + self.prepareTime then
		self.curState = SPELL_STATE_FINISH
	end
	return curState
end
---------------------------------------------------------------------------------------------------