local string = string
local math = math
local pairs = pairs
local setmetatable = setmetatable
local table = table
local print = print
local _G = _G

module "Buff"

Buff = {
	id,
	category,
	disableValidators,
	buffReplaceList,
	buffCategoryReplaceList,
	buffExclusionList,
	buffCategoryExclusionList,
	coexist, --不同源共存
	
	duration,
	period,--多久触发一次
	periodCount,--触发次数
	--m_ValidatePos,
	maxTotalStack,
	maxPerStack,
	dispelLevel,
	
	extend,
	effectPeriodicID,
	effectExpireID, --timeout effect
	effectDispelID, --dispel effect
	effectRefreshID,--refresh effect
	effectInvalidateID,--removed by validator
	
	charge,
	noMaxLifeChange,
	
	--damage response
	damageResponseChance,
	damageResponseValidator,
	responseLocate, --{"attacker", "defender"}
	damageResponseEffectID,
	damageResponseCost, --{life, energy, cooldown, item,coin, ... charge?}
	damageResponseFlags, --{[1] = "critical"}
	
	--death response
	deathResponseChance,
	deathResponseEffectID,
	deathResponseCost,
	deathResponseLocate,
	
	--set when apply
	--spell releated
	spellEnableList,
	spellDisableList,
	spellEnableCategoryList,
	spellDisableCategoryList,
	--unit releated
	unitStateModify,
	--[[{
		disableSpell.
		forceTarget,
		suppressMovement,
		bury,
		generateThreat,
		invulnerable, --无敌
		passive,
		suppressAttack,
		suppressCombat,
		suppressEnergy,
		suppressEnergyRegenarate,
		suppressLife,
		suppressLifeRegenarate,
		suppressShield,
		suppressShieldRegenarate,
		suppressThreat,
		uncommandable,
		unselectable,
		untargetable,
	}]]
	unitAttributeModify,
	--[[{
			speed
			speedFactor
			criticalDamageLevel
			baseCriticalFactor
			criticalRate
			criticalLevel
			maxLife
			maxLifeFactor
			energyRate
			attack
			attackFactor
			voidDefense
			voidDefenseFactor
			elementDefence
			elementDefenseFactor
			physicalDefence
			physicalDefenseFactor
			hitRate
			hitLevel
			accurateLevel
			accurateFactor
			dodgeLevel
			dodgeRate
			defenseFactor
			injuryRate
			healRate
			leechFactor
	}]]
	--special flag
	shield,
	shieldFunction,
	shieldEffectID,
	shieldExtra,
	
	--to remove
	cacheAmount,
	cacheEffectID,
	
	--state data
	curCacheAmount,
	actualDuration,
	applyTime,
	endTime,
	caster,
	target,
	channelOwner,
	ownedSpell,
	curShield,
	totalShield,
	curCharge,
	curStack, -- = {}
	curTotalStack,
	curPeriodicCount,
	isFinish,
	removeType,
	modifiedAmount,
	nextValidateTime,
	enable,
	instanceID,
	cachedLife,--{}
	crazyDisable,
	finishMaker, -- the slot id who makes the buff finish
	owner,
}
---------------------------------------------------------------------------------------------------
function Buff:Init()
	self.curStack = {}
	self.curTotalStack = 0
end
---------------------------------------------------------------------------------------------------
function Buff:SetOwnedSpell(spell)
	self.ownedSpell = spell
end
---------------------------------------------------------------------------------------------------
function Buff:SetEnable(enable)
	if self.enable ~= enable then
		self.enable = enable
		self:ModifyUnit(not(enable))
	end
end
---------------------------------------------------------------------------------------------------
function Buff:Apply(curTime)
	local info = string.format("buff applyed id %s\n", self.id)
	print(info)
	do return end
	
	if self.target == nil then
		return
	end
	
	local activeBuffList = self.target.buffList
	self:ResetStateData(curTime)

	--self.m_InstanceID = self.m_OwnedSpell.m_InstanceID
	self:AddBuff(activeBuffList, curTime)
	self.applyTime = curTime
	--NOTE:if AddBuff() has same buff, should set OwnerBuff to that same buff?
	local effectInitial = self.owner:GetEffect(self.effectInitialID)
	if effectInitial ~= nil then
		effectInitial:SetOwnedSpell(self.ownedSpell)
		effectInitial:SetOwnedBuff(self)
		effectInitial.target = self.target
		effectInitial:Apply(curTime)
	end
	
	--self:ModifyUnit(false)
end
---------------------------------------------------------------------------------------------------
function Buff:GetRemainPos(curTime)
	local remainRound = -1
	if self.m_Duration > 0 then
		--local endPos = math.floor(self.m_EndTime/_Gen.POS_PER_ROUND) * _Gen.POS_PER_ROUND
		--local curPos = math.floor(curTime/_Gen.POS_PER_ROUND) * _Gen.POS_PER_ROUND
		remainRound = self.m_EndTime - curTime
	end
	return remainRound	
	
end
---------------------------------------------------------------------------------------------------
--jie suan
function Buff:Periodic(curTime)
	if self.isFinish then
		return
	end
	
	local periodTime = self.period
	if not(periodTime) then
	    periodTime = 1
	end
	local periodCount = self.periodCount
	if not(periodCount) then
	    periodCount = _G.MAX_ROUND
	end
	
	if self.enable and self.effectPeriodicID then
		local validateTime = self.applyTime + self.curPeriodicCount * periodTime
		while self.curPeriodicCount <= periodCount and 
			(validateTime- curTime)<= 0 
		do
			local periodicEffect = self.owner:GetEffect(self.effectPeriodicID)
			if periodicEffect then
				periodicEffect:SetOwnedSpell(self.ownedSpell)
				periodicEffect:SetOwnedBuff(self)
				
				periodicEffect.target = self.target
				periodicEffect.caster = self.caster
				periodicEffect.applyTime = validateTime
				self.owner:AddCalculateEffect(periodicEffect)
			end 
			self.curPeriodicCount = self.curPeriodicCount + 1
			validateTime = self.applyTime + self.curPeriodicCount * periodTime
		end	
	end

end
---------------------------------------------------------------------------------------------------
function Buff:AddBuff(activeBuffList, curTime)
	local successAdded = true

	--handle overlay
	local sameBuff 
	if activeBuffList then
		for _, buff in pairs(activeBuffList) do
			--the buff may finished but not removed from active buff list within one round
			if buff.isFinish == false and 
			   buff.id == self.id and
			   (buff.caster.guid == self.caster.guid or not(self.coexist))
			then
				sameBuff = buff
				break
			end
		end
	end
	if sameBuff ~= nil then
		--local curPerStack = sameBuff:GetCurStack(self.m_CasterID)
		--local curStack = sameBuff:GetCurStack()
		sameBuff:AddStack(self.caster.guid, nil, curTime)
		--refresh buff use new casterID
		sameBuff.caster = self.caster
		sameBuff.target = self.target
		sameBuff:Refresh(curTime)--trigger refresh event
		sameBuff:DealReplace(curTime)
		
		--&&&&
		sameBuff:Periodic(curTime)
		--sameBuff:UpdateBuffState(curTime)
	else
		self:DealReplace(curTime)
		self:CheckEnable()
		--if self.m_Enable then
		--	self:ModifyUnit(false)		
		--end
		successAdded = self.m_Owner:AddActiveBuff(self)
		if successAdded then
			self:AddStack(self.m_CasterID, 1, curTime)
			self:SendBuff(curTime, true)
			self:Periodic(curTime)
			self:UpdateBuffState(curTime)
		end
	end	
	
	return successAdded
	--Level:UpdateUnitAction()
	
end
---------------------------------------------------------------------------------------------------
function Buff:DealReplace(curTime)
	--check replaceBuffList
	if self.buffReplaceList then
		for _, val in pairs(self.buffReplaceList) do
			self.owner:RemoveBuffID(self, val, curTime, _Gen.BUFF_REMOVED_DEFAULT)
		end
	end
	--check replacebuff category
	if self.buffCategoryReplaceList then
		for _, val in pairs(self.buffCategoryReplaceList) do
			self.owner:RemoveBuffCategory(self, val, curTime, _Gen.BUFF_REMOVED_DEFAULT)
		end
	end
end
---------------------------------------------------------------------------------------------------
function Buff:GetCurStack(casterID)
	local curStack = 0
	--get total stack if no casterID
	if casterID == nil then
	    for _, val in pairs(self.curStack) do
			curStack = curStack + val
		end
	else
	    curStack = self.m_CurStack[casterID]
	    if curStack == nil then
	        curStack = 0
		end
	end

	return curStack
end
---------------------------------------------------------------------------------------------------
function Buff:AddStack(casterID, buffCount, pos)

	if self.maxTotalStack == self.curTotalStack and buffCount and buffCount > 0 then
		return
	end
	
	local addStack = buffCount
	if addStack == nil then	
		addStack = 1
	end
	
	local previousTotalStack = self.curTotalStack
	if self.curStack[casterID] ~= nil then
		self.curStack[casterID]  = self.curStack[casterID] + addStack
	else
	    self.m_CurStack[casterID] = addStack
	end
	self.curTotalStack = self.curTotalStack + addStack
	local minusStack = 0
	minusStack = math.min((self.maxPerStack - self.curStack[casterID]),(self.maxTotalStack - self.curTotalStack))
	if minusStack < 0 then
		self.curStack[casterID] = self.curStack[casterID] + minusStack
		self.curTotalStack = self.curTotalStack + minusStack
	end
	
	if self.curTotalStack ~= previousTotalStack then
		if previousTotalStack ~= 0 then
			self:ModifyUnit(true, previousTotalStack, pos)
		end
		--if self.m_CurTotalStack ~= 1 then --TODO:check =1 already modifyunit in checkenable?
		self:ModifyUnit(false)
		--end
	end

end
---------------------------------------------------------------------------------------------------
function Buff:ResetStateData(curTime)
	self.actualDuration = self.duration
	self.applyTime = curTime
	self.endTime = curTime + self.duration
	self.curStack = {}
	self.curTotalStack = 0
	self.curPeriodicCount = 1
	self.isFinish = false
	self.removeType = buff_removeby_default
	if self.shieldFunction then
		self.shield = self.shieldFunction(self)
	end
	self.curShield = self.shield
	self.totalShield = self.curShield
	self.curCacheAmount = 0
end
---------------------------------------------------------------------------------------------------
function Buff:Refresh(curTime)
	local remainTime = curTime - self.applyTime
	self.applyTime = curTime
	if self.extend then
		self.actualDuration = self.duration + remainTime
	else
		self.actualDuration = self.duration
	end
	self.endTime = curTime + self.actualDuration
	self.curCharge = self.charge
	self.curPeriodicCount = 1
	--if self.m_NextValidateTime <= curTime then
		local effectRefresh = self.owner:GetEffect(self.effectRefreshID)
		if self.effectRefresh ~= nil then
			effectRefresh:SetOwnedSpell(self.ownedSpell)
			effectRefresh:SetOwnedBuff(self)
			effectRefresh.target = self.target
			effectRefresh:Apply(curTime)
			--self.m_Owner:AddActiveEffect(effectRefresh)
		end
	--end
	
	self.owner:RemoveEffectNotTriggered(self, curTime, "buff")
	if self.endTime - curTime <= 0 then
		self:Remove(_G.BUFF_REMOVE_EXPIRE)
	end
	
	--[[local curBeginPos = math.floor(curTime/_Gen.POS_PER_ROUND)*_Gen.POS_PER_ROUND
	if self.actualDuration > 0 and self.m_EndTime - curTime <= _Gen.POS_PER_ROUND then
		self.m_Owner:AddBuffToRemove(self, true)
	else
		self.m_Owner:ClearBuffToRemove(self)
	end]]
end
---------------------------------------------------------------------------------------------------
function Buff:CheckEnable()
	if self.isFinish then
		return false
	end
	
	local previousEnable = self.enable
	local curEnable = true
	if self.disableValidators then
		for _,val in pairs(self.disableValidators) do
			if val(self) == false then
				curEnable = false
				break
			end
		end
	end
	if previousEnable ~= curEnable then
		self:SetEnable(curEnable)
		return true
	end
	return false
end
---------------------------------------------------------------------------------------------------
function Buff:ModifyUnit(isRemove, curStack, curTime)
	--log(_G.LOG_CATEGORY_SPELL, "modify unit")
	--do return end
	if self.curTotalStack == 0 then
		return
	end
	
	local target = self.target
	if target == nil then
		return
	end
	if not(curStack) then
		curStack = self.curTotalStack
	end
	if not(curTime) then
		curTime = self.applyTime
	end
	
	local factor = 1
	if isRemove then
		factor = -1
	end
	--[[
	local targetSpellList = target.spellList
	--unit spell
	if self.spellEnableList and targetSpellList then
		for _, enableID in pairs(self.m_SpellEnableList) do
			if targetSpellList[enableID] then
				local previousEnable = targetSpellList[enableID].curEnable
				targetSpellList[enableID].curEnable = targetSpellList[enableID].curEnable + factor
				local curEnable = targetSpellList[enableID].curEnable
				if curEnable * previousEnable <= 0 then
					targetSpellList[enableID]:CheckActive(curTime, true, self.target)
				end
				
			end	
		end
	end
	]]
	
	--shield
	if self.curShield then
		target.shield = target.shield + self.shield * factor
		if target.shield < 0 then
			target.shield = 0
		end
	end
	
	--unit state
	if self.unitStateModify then
		local modifyList = self.unitStateModify 
		if modifyList.invulnerable then
			if target.flags.invulnerable then
				target.flags.invulnerable = target.flags.invulnerable + factor
				if target.flags.invulnerable < 0 then
					target.flags.invulnerable = 0
				end
			else
				target.flags.invulnerable = modifyList.invulnerable
			end
			--[[
			if factor > 0 then
			    self.m_Owner:RemoveQte(self.m_TargetID)
			end
			]]
		end
		
		if modifyList.disableSpell then
			if target.flags.disableSpell then
				target.flags.disableSpell = target.flags.disableSpell + factor
				if target.flags.disableSpell < 0 then
					target.flags.disableSpell = 0
				end
			else
				target.flags.disableSpell = modifyList.disableSpell
			end
			if target.flags.disableSpell == 1 then
				--self.owner:InterruptSpell(self.m_TargetID, curTime, true)
			end
		end
		
		if modifyList.forceTarget then
			if isRemove then
				target.flags.forceTarget = nil
			else
				target.flags.forceTarget = self.caster.guid
				--self.owner:ChangeSpellTarget(self.target.guid, self.caster.guid, curTime)
			end
		end
	end
	--local testTab = self.m_UnitAttributeModify
	if self.unitAttributeModify then
		local modifyList = self.unitAttributeModify
		--[[
		--modify unit attribute
		if modifyList.alignment then
			if isRemove then
				curAttr.curAlignment = baseAttr.baseAlignment
			else	
				curAttr.curAlignment = modifyList.alignment
			end
		end
		]]
		local changeValue
		--暴击率
		changeValue = GetChangeValue(target.baseCriticalRate, modifyList.criticalRate) * curStack
		target.criticalRate = target.criticalRate + changeValue * factor 
		--生命上限
		changeValue = GetChangeValue(target.baseLife, modifyList.maxLife, modifyList.maxLifeFactor) * curStack
		local tempMaxLife = target.maxLife + changeValue * factor
		if not(self.noMaxLifeChange) then
			curAttr.curMaxLife = tempMaxLife
		end
		if changeValue ~= 0 then
			--local needSend
			if changeValue < 0 then
				if factor < 0 then--remove buff
					if self.cachedLife then
						if self.temoveType ~= _G.BUFF_REMOVE_TARGET_DEAD then
							target.life = target.life + self.cachedLife
						end
						--needSend = true
					end
				else		--add buff
					--needSend = true
					if target.life > tempMaxLife then
						self.cachedLife = target.life - tempMaxLife
						target.life = tempMaxLife
					else
						self.cachedLife = nil
					end
				end
			else
				if factor < 0 then--remove buff
					if self.removeType ~= _G.BUFF_REMOVE_TARGET_DEAD then
						target.life = target.life - changeValue
						if target.life < 1 then
							target.life = 1
						end
					end
				else		--add buff
					target.life = target.life + changeValue
				end
			end
			if target.life > target.maxLife then
				target.life = target.maxLife
			end
			--[[
			--if needSend then
			local vitalSendData = {}
			vitalSendData.targetSlot = self.m_TargetID
			vitalSendData.sourceSlot = self.m_CasterID
			vitalSendData.pos = pos
			vitalSendData.vitalType = _Gen.VITAL_LIFE_BUFF
			vitalSendData.changeVal = 0
			vitalSendData.curVal = curAttr.curLife
			vitalSendData.maxVal = curAttr.curMaxLife
			vitalSendData.critical = false
			self.m_Owner:AddSendVitalChangeData(vitalSendData)
			--end
			]]
		end
		
		--力量
		changeValue = GetChangeValue(target.baseStrength, modifyList.strength, modifyList.strengthFactor) * curStack
		target.strength = target.strength + changeValue * factor
		--智力
		changeValue = GetChangeValue(target.baseIntelligence, modifyList.intelligence, modifyList.intelligenceFactor) * curStack
		target.intelligence = target.intelligence + changeValue * factor
		--速度
		changeValue = GetChangeValue(target.baseSpeed, modifyList.speed, modifyList.speedFactor) * curStack
		target.speed = target.speed + changeValue * factor
		--抗性
		changeValue = GetChangeValue(target.baseResistance, modifyList.resistance, modifyList.resistanceFactor) * curStack
		target.speed = target.speed + changeValue * factor
		--耐力
		changeValue = GetChangeValue(target.baseEndurance, modifyList.endurance, modifyList.enduranceFactor) * curStack
		target.endurance = target.endurance + changeValue * factor
		--命中率
		changeValue = GetChangeValue(target.baseHitRate, modifyList.hitRate) * curStack
		target.hitRate = target.hitRate + changeValue * factor
		--受伤比
		target.injuryRatio = target.injuryRatio + modifyList.injuryRatio
		
		target:OnToplevelAttrChanged()
	end
	
end
---------------------------------------------------------------------------------------------------
function Buff:Remove(removeType, curTime)

	if self.isFinish then
		return false
	end

	self.isFinish = true
	self.removeType = removeType
	self.actualDuration = 0
	if curTime == nil then
		curTime = self.endTime
	end
	
	--deal remove
	local removeEffect
	if removeType == _G.BUFF_REMOVE_EXPIRE then
		removeEffect = self.owner:GetEffect(self.effectExpireID)
	elseif removeType == _G.BUFF_REMOVE_DISPEL then
		removeEffect = self.owner:GetEffect(self.effectDispelID)
	--[[
	elseif self.removeType == _G.BUFF_REMOVE_CASTER_DEAD then
	elseif self.removeType == _G.BUFF_REMOVE_TARGET_DEAD then
		self:DeathResponse(curTime)
	elseif self.m_RemoveType ==  _Gen.BUFF_REMOVED_SHIELD_EMPTY then
		log(
			_G.LOG_CATEGORY_SPELL,
			string.format(
				"buff id=%s ended,target=%d",
				self.m_ID, self.m_TargetID
				)
			)		
		if self.m_Shield or self.m_ShieldFunction then
			if self.m_CurShield <= 0 then
				local shieldEffect = self.m_Owner:GetEffect(self.m_ShieldEffectID)
				if shieldEffect then
					shieldEffect:SetOwnedSpell(self.m_OwnedSpell)
					shieldEffect:SetOwnedBuff(self)
					shieldEffect:SetTarget(self.m_TargetID)
					shieldEffect:Apply(curTime)				
				end
			end	
		end
        if self.m_CacheAmount then
            local cacheEffect = self.m_Owner:GetEffect(self.m_CacheEffectID)
            if cacheEffect then
                shieldEffect:SetOwnedSpell(self.m_OwnedSpell)
                shieldEffect:SetOwnedBuff(self)
                shieldEffect:SetTarget(self.m_TargetID)
                shieldEffect:SetParam(self.m_CurCacheAmount)
                shieldEffect:Apply(curTime)		
            end
        end	
	elseif self.m_RemoveType ==  _Gen.BUFF_REMOVED_CHARGE_EMPTY then
		log(
			_G.LOG_CATEGORY_SPELL,
			string.format(
				"buff id=%s ended,target=%d",
				self.m_ID, self.m_TargetID
				)
			)		
	elseif self.m_RemoveType ==  _Gen.BUFF_REMOVED_DEFAULT then
		log(
			_G.LOG_CATEGORY_SPELL,
			string.format(
				"buff id=%s ended,target=%d",
				self.m_ID, self.m_TargetID
				)
			)		
	elseif self.m_RemoveType == _Gen.BUFF_REMOVED_INVALIDATE then
	    if self.m_EffectInvalidateID then
		    removeEffect = self.m_Owner:GetEffect(self.m_EffectInvalidateID)
		end
		
		log(
			_G.LOG_CATEGORY_SPELL,
			string.format(
				"buff id=%s removed,target=%d, validator check failed or channel failed",
				self.m_ID, self.m_TargetID
				)
			)		
			]]
	end
	if removeEffect then
		removeEffect:SetOwnedSpell(self.ownedSpell)
		removeEffect:SetOwnedBuff(self)
		removeEffect.target = self.target
		removeEffect:Apply(curTime)
	end
	
	if self.enable then
		self:ModifyUnit(true, nil, curTime)
	end
	self.curTotalStack = 0
	
	--deal channel
	--[[
	--if caster channel buff is removed, clear all target channel buff
	if self.m_Category["channel_caster"] then
		--TODO: optimize
		local playerList, enemyList = self.m_Owner:GetAllUnits()
		for slotID, _ in pairs(playerList) do
			if slotID ~= self.m_TargetID then
				local activeBuffList = self.m_Owner:GetActiveBuffList(slotID)
				if activeBuffList then
					for _, buff in pairs(activeBuffList) do
						--the buff may finished but not removed from active buff list within one round
						if buff.isFinish == false and 
						   buff.category["channel_target"] and
						   buff.m_CasterID == self.m_TargetID
						then
							buff:Remove(_Gen.BUFF_REMOVED_INVALIDATE, curTime)
							log(
								_G.LOG_CATEGORY_SPELL,
								string.format(
									"buff id=%s removed since caster id=%d channel buff id=%s is removed",
									buff.m_ID, self.m_CasterID, self.m_ID
									)
								)		
							break
						end
					end
				end
			end
		end
		for slotID, _ in pairs(enemyList) do
			if slotID ~= self.m_TargetID then
				local activeBuffList = self.m_Owner:GetActiveBuffList(slotID)
				if activeBuffList then
					for _, buff in pairs(activeBuffList) do
						--the buff may finished but not removed from active buff list within one round
						if buff.m_IsFinish == false and 
						   buff.m_Category["channel_target"] and
						   buff.m_CasterID == self.m_TargetID
						then
							buff:Remove(_Gen.BUFF_REMOVED_INVALIDATE, curTime)
							log(
								_G.LOG_CATEGORY_SPELL,
								string.format(
									"buff id=%s removed since caster id=%d channel buff id=%s is removed",
									buff.m_ID, self.m_CasterID, self.m_ID
									)
								)		
							break
						end
					end
				end
			end
		end
	end
			
	if self.m_Category["channel_target"] then
		--if target channel buff is removed, minus one stack of caster channel buff
		local activeBuffList = self.m_Owner:GetActiveBuffList(self.m_CasterID)
		if activeBuffList then
			for _, buff in pairs(activeBuffList) do
				--the buff may finished but not removed from active buff list within one round
				if buff.m_IsFinish == false and buff.m_Category["channel_caster"] then
					buff:AddStack(self.m_CasterID, -1, curTime)
					log(
						_G.LOG_CATEGORY_SPELL,
						string.format(
							"buff id=%s removed one stack since target id=%d channel buff id=%s is removed",
							buff.m_ID, self.m_TargetID, self.m_ID
							)
						)	
					if buff.m_CurTotalStack == 0 then
						buff:Remove(_Gen.BUFF_REMOVED_INVALIDATE, curTime)
						self.m_Owner:InterruptSpell(buff.m_TargetID, curTime, true)
					end
					
					break
				end
			end
		end
	end
	]]
	
	--self:SendBuff(curTime, true)
	self.finishMaker = nil
	return true
end
---------------------------------------------------------------------------------------------------
function Buff:ReBoundResponse(respEffect, amount, respTime, flags)
	return amount
end
---------------------------------------------------------------------------------------------------
function Buff:DamageResponse(respEffect, amount, respTime, flags)
	
	if self.isFinish or not(self.enable) then
		return
	end
	
	if self.cacheAmount and self.target.guid == respEffect.target.guid then
		self.curCacheAmount = self.curCacheAmount + amount
	end
	
	if not(self.damageResponseEffectID) then
		return
	end
	
	local respCaster = respEffect.caster
	local respTarget = respEffect.target
	local respEffectID = respEffect.id
	
	if (self.nextValidateTime and self.nextValidateTime > respTime) then
		return
	end
	--[[
	if self.m_DamageResponseChance then
		local chance = math.random()
		if chance > self.m_DamageResponseChance then
			return
		end
	end
	]]
	
	if self.damageResponseValidator then
		if not(self.damageResponseValidator(self)) then
			return
		end
	end
	
	if not(self.responseLocate) or self.responseLocate == "defender" then
		if self.target.guid ~= respTarget.guid then
			return
		end
	else
		if self.target.guid ~= respCaster.guid then
			return
		end
	end
	
	if self.damageResponseFlags then
		for _, flag in pairs(self.damageResponseFlags) do
			if not(flag) or not(flags[flag]) then
				return
			end	
		end	
	end
	
	if self:CheckCost(self.damageResponseCost, respTime) == false then
		return
	end
	
	local damageResponseEffect = self.owner:GetEffect(self.damageResponseEffectID)
	if damageResponseEffect then
		damageResponseEffect:SetOwnedSpell(self.ownedSpell)
		damageResponseEffect:SetOwnedBuff(self)
		damageResponseEffect.caster = respCaster
		damageResponseEffect.target = respTarget
		--damageResponseEffect:SetParam(amount, respEffect)
		damageResponseEffect:Apply(respTime)
		self:TakeCost(self.damageResponseCost, respTime)
		--TODO:set filter?
	end
	return
end
---------------------------------------------------------------------------------------------------
function Buff:CheckCost(costTab, curTime)
	local result = true
	if costTab then
		if costTab.life then
			local unit = self.target
			if not(unit) or unit.life <= costTab.life then
				return false
			end
		end
		if costTab.coolDown then
			if self.nextValidateTime >= curTime then
			    return false
			end
		end
		--[[
		if costTab.charge then
			if self.curCharge and self.m_CurCharge < costTab.charge then
				return false
			end
		end
		]]
	end
	return true
end
---------------------------------------------------------------------------------------------------
function Buff:TakeCost(costTab, curTime)

	if costTab then
			
		if costTab.life then
			local unit = self.target
			if not(unit) then
				unit.life = unit.life - costTab.life 
			end		
		end
		if costTab.coolDown then
			self.nextValidateTime = curTime + costTab.coolDown
		end
		--[[
		if costTab.charge then
			self.curCharge = self.m_CurCharge - costTab.charge
			if self.m_CurCharge <= 0 then
				self:Remove(_Gen.BUFF_REMOVED_CHARGE_EMPTY, curTime)
				--self.m_Owner:RemoveBuffID(self.m_TargetID, self.m_ID, curTime, _Gen.BUFF_REMOVED_DEFAULT)
			end
		end
		]]
	end
	
end
---------------------------------------------------------------------------------------------------
function Buff:DeathResponse(curTime)
	if not(self.enable) then
		return
	end
	
	if self:CheckCost(self.deathResponseCost, curTime) then
		local deathResponseEffect = self.owner:GetEffect(self.deathResponseEffectID)
		if deathResponseEffect then
			deathResponseEffect:SetOwnedSpell(self.ownedSpell)
			deathResponseEffect:SetOwnedBuff(self)
			if not(self.deathResponseLocate) or self.deathResponseLocate == "target" then
				deathResponseEffect.target = self.target
			elseif self.deathResponseLocate == "caster" then
				deathResponseEffect.target = self.caster
			else
				deathResponseEffect.target = self.finishMaker
			end
			deathResponseEffect.noMiss = true
			deathResponseEffect:Apply(curTime)
		end
		self:TakeCost(self.deathResponseCost, curTime)
	end
end
---------------------------------------------------------------------------------------------------
function Buff:SendBuff(curTime, changeState)
--[[
	local buffSendData = {}
	buffSendData.id = self.m_ID
	buffSendData.instanceID = self.m_OwnedSpell.m_InstanceID
	if self.m_InstanceID then
		buffSendData.instanceID = self.m_InstanceID
	end
	buffSendData.pos = curTime
	buffSendData.length = self.m_ActualDuration
	buffSendData.targetSlot = self.m_TargetID
	buffSendData.sourceSlot = self.m_OwnedSpell.m_CasterID
	buffSendData.stack = self.m_CurTotalStack
	buffSendData.removeType = self.m_RemoveType
	
	if buffSendData.stack == 0 then
		if buffSendData.removeType == _Gen.BUFF_REMOVED_DISPEL then
			buffSendData.id = string.format("%s_interrupt", self.m_ID)
		elseif buffSendData.removeType == _Gen.BUFF_REMOVED_TARGET_DEAD then
			buffSendData.id = string.format("%s_target_dead", self.m_ID)
		else
			buffSendData.id = string.format("%s_finish", self.m_ID)
		end
		if changeState then
			self.m_Owner:GenerateControlEvent(buffSendData, self, true)
		end
	else
		if changeState then
			self.m_Owner:GenerateControlEvent(buffSendData, self, false)
		end
	end
	
	self.m_Owner:AddSendBuffData(buffSendData)
	]]
end
---------------------------------------------------------------------------------------------------
function Buff:OnDamageShield(amount, curTime)
	if amount == nil then
	    return
    end
	
	if self.isFinish or not(self.enable) then
		return amount
	end
	
	if self:CheckCost(self.damageResponseCost, curTime) == false then
		return
	end
	
	if self.curShield and self.curShield > 0 then
		local leftAmount = amount
		local unit = self.target
		if leftAmount >= self.curShield then
			unit.shield = unit.shield - self.curShield
			leftAmount = amount - self.curShield
			self.curShield = 0
			self.owner:RemoveBuffID(self, self.id, curTime, _G.BUFF_REMOVE_SHIELD_EMPTY)
		else
			unit.curShield = unit.curShield - amount
			self.curShield = self.curShield - amount
			leftAmount = 0	
		end
		self:TakeCost(self.damageResponseCost, curTime)
		return leftAmount
	end
	return amount
end
---------------------------------------------------------------------------------------------------
