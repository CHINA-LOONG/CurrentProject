local Log = DebugLog
local Effect = Effect
local pairs = pairs
local setmetatable = setmetatable

module "EffectPersistent"

---------------------------------------------------------------------------------------------------
EffectPersistent = 
{
	effectID,
	delayTime,
	
	periodEffectList,--{[time] = id,}
}
setmetatable(EffectPersistent, {__index = Effect.Effect})
---------------------------------------------------------------------------------------------------
function EffectPersistent:Apply(triggerTime)
	if self:ApplyBasic(triggerTime) == false then
		do return end
	end
	
	--start effect
	local delayTime = 0
	if self.delayTime then
		delayTime = self.delayTime
	end
	local rootEffect = self.owner:GetEffect(self.effectID)
	if rootEffect then
		rootEffect:SetOwnedSpell(self.ownedSpell)
		rootEffect:SetOwnedBuff(self.ownedBuff)
		rootEffect.target = self.target
		if delayTime > 0 then
			--self.m_Owner:AddCalculateEffect(rootEffect, isCrazy)
			rootEffect:Apply(self.applyTime + delayTime)
		else
			rootEffect:Apply(self.applyTime)
		end
	end
	
	if self.periodEffectList then
		for pTime, effectID in pairs(self.periodEffectList) do
			local effect = self.owner:GetEffect(effectID)
			delayTime = delayTime + pTime
			if effect then
				effect:SetOwnedBuff(self.ownedBuff)
				effect:SetOwnedSpell(self.ownedSpell)
				effect.target = self.target
				effect:Apply(self.applyTime + delayTime)
			end
		end
	end			
	self.ownedSpell.length = delayTime
end
---------------------------------------------------------------------------------------------------