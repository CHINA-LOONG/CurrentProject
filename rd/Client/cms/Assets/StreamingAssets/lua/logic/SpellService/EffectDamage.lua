local string = string
local math = math
local pairs = pairs
local setmetatable = setmetatable
local table = table
local Effect = Effect
local Log = DebugLog
local _G = _G

module "EffectDamage"
---------------------------------------------------------------------------------------------------
damageTypePhy = 1
damageTypeMag = 2
---------------------------------------------------------------------------------------------------
EffectDamage = 
{
	--basic data
	amount,
	amountFunction,
	attackFactor,
	damageType,
	isHeal,
	fixedCriticalRatio,
	
	--flag
	kill,
	live,
	noMiss,
	absoluteCritical,
	noCritical,
	
	--state data
	isCritical,
}
setmetatable(EffectDamage, {__index = Effect.Effect})
---------------------------------------------------------------------------------------------------
function EffectDamage:Apply(curTime)
	if self:ApplyBasic(curTime) == false then
		return 
	end
	
	local info = string.format("damage effect Apply id%s \n", self.id)
	Log:Log(info)
	
	self:CalculateHit()
	if self.hit == _G.HIT_SUCCESS then
		self:CalculateEnergy()
		if self.kill then
			--kill
		else
			self:CalculateCritical()
			self:CalculateDamage(self.applyTime)
		end
	end
end
---------------------------------------------------------------------------------------------------
function EffectDamage:CalculateHit()
	local info = string.format("calculate hit in effect %s", self.id)
	Log:Log(info)
	
	self.hit = _G.HIT_SUCCESS
	if not(self.isHeal) then
		local invulnerable = self.target.flags.invulnerable
		if invulnerable and invulnerable > 0 then
			self.hit = _G.HIT_IMMUNE
			return
		end
	end
	
	if not(self.noMiss) then
		self.hit = self:CalculateBasicHit(self.caster, self.target)
	end
end
---------------------------------------------------------------------------------------------------
function EffectDamage:CalculateCritical()
	local info = string.format("calculate critical in effect %s", self.id)
	Log:Log(info)
	self.isCritical = true
	if self.absoluteCritical then
		self.isCritical = true
		return
	end
	if self.noCritical then
		self.isCritical = nil
		return
	end
	
	local criticalRandNum = math.random()
	if criticalRandNum > self.caster.criticalRate then
		self.isCritical = nil
	end
end
---------------------------------------------------------------------------------------------------
function EffectDamage:CalculateDamage(curTime)
	local info = string.format("calculate damage in effect %s", self.id)
	Log:Log(info)
	local lifeAmount
	if self.amountFunction then
		lifeAmount = self.amountFunction(self)
	elseif self.attackFactor then
		local attack = self.caster.phyAttack
		if self.damageType == damageTypeMag then
			attack = self.caster.magAttack
		end
		lifeAmount = self.attackFactor * attack
		if self.amount then
			lifeAmount = self.amount + lifeAmount
		end
	end
	
	--暴击系数
	if self.isCritical then
		lifeAmount  = lifeAmount * _G.CRITICAL_DMG_RATIO
	end
	
	local damageConvertRatio = 1
	if not(self.isHeal) then
		damageConvertRatio = -1
		--受伤比
		--1/(1+(B防御力)/I(lv2))
		local injuryRatio = 1.0/(1.0 + (self.target.defend)/_G.GetInjuryAdjustNum(math.min(self.caster.level, self.target.level)))
		if injuryRatio > 0.25 then
			injuryRatio = 0.25
		end
		injuryRatio = injuryRatio + self.target.injuryRatio
		lifeAmount = lifeAmount * injuryRatio
		
		--属性相克系数
		if self.damageType == damageTypeMag then
			lifeAmount = lifeAmount * _G.GetPropertyInfluenceRatio(self.caster.property, self.target.property)
		end
		
		--副本系数？什么玩意
		--lifeAmount = lifeAmount * self.owner.level.injuryRatio
	end
	lifeAmount = math.ceil(lifeAmount)
	lifeAmount = lifeAmount * damageConvertRatio
	
	--damage response
	--suppose only target has damage response for simply
	local activeBuffList = self.target.buffList
	if activeBuffList then
		for _, buff in pairs(activeBuffList) do
			buff:DamageResponse(lifeAmount, self)
		end
	end
	
	--change unit attribute
	if self.isHeal then
		self.target.life = self.target.life + lifeAmount
	end
	if self.target.life < 0 then
		self.target.life = 0
		self.owner:TriggerUnitDead({triggerTime = curTime, casterID = self.caster.guid, deathID = self.target.guid})
	elseif self.target.life > self.target.maxLife then
		self.target.life = self.target.maxLife
	end
	
	self.owner:TriggerLifeChange(
		{ triggerTime = curTime, casterID = self.caster.guid, targetID = self.target.guid,
			isCritical = self.isCritical, lifeChange = lifeAmount, 
			lifeCurrent = self.target.life,lifeMax = self.target.maxLife
		}
	)
end
---------------------------------------------------------------------------------------------------