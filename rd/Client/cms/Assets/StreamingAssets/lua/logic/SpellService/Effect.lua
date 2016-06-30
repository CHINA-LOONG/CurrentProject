---------------------------------------------------------------------------------------------------
Effect = 
{
	id,
	chance,
	validateTime,
	energy,
	targetType,
	casterType,
	
	--state data
	ownedBuff,
	ownedSpell,
	--source,
	caster,
	target,
	hit,
	applyTime,
	instanceID,
}
---------------------------------------------------------------------------------------------------
function Effect:Init()
end
---------------------------------------------------------------------------------------------------
function Effect:SetOwnedBuff(buff)
	self.ownedBuff = buff
	if buff ~= nil then
		self.caster = buff.caster
		self.target = buff:target
	end
end
---------------------------------------------------------------------------------------------------
function Effect:SetOwnedSpell(spell)
	self.ownedSpell = spell
	if spell ~= nil then
		self.caster = spell:GetCaster()
		self.target = spell:GetTarget()
	end
end
---------------------------------------------------------------------------------------------------
function Effect:ApplyBasic(curTime)
	self:GenerateTarget(self.caster, self.target)
	if self.chance then
		local rand = math.random(100)/100.0
		if rand > self.chance then
			do return false end
		end
	end
	
	if curTime then
		self.applyTime = curTime
	end
	
	return true
end
---------------------------------------------------------------------------------------------------
function Effect:GenerateTarget(caster, target)
	if self.targetType == TARGET_CASTER then
		self.target = caster
	else
		self.target = target
	end
	
	if self.casterType == TARGET_TARGET then
		self.caster = target
	else
		self.caster = caster
	end
end
---------------------------------------------------------------------------------------------------
function Effect:GenerateEnergy()
	if self.caster and self.energy then	
	end
end
---------------------------------------------------------------------------------------------------
function Effect:CalculateBasicHit(caster, target)
	--min((max(N+L(lv1-lv2)),60%)+装备附加命中+buff附加命中,100%)
	local hitRate = caster.baseHitRate + GetHitRateAdjustNum(caster.level - target.level)
	local hitRateExtra = caster.hitRate - caster.baseHitRate
	if hitRate < HIT_RATE_MIN then
		hitRate = HIT_RATE_MIN
	end
	hitRate = hitRate + hitRateExtra
	local hitRandNum = math.random()
	if hitRate < 0 or hitRandNum > hitRate then
		return HIT_MISS
	end
	
	return HIT_SUCCESS
end
---------------------------------------------------------------------------------------------------
