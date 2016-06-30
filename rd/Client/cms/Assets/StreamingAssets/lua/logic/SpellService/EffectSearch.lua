---------------------------------------------------------------------------------------------------
EffectSearch = {
	excludeTarget,
	includeTarget,
	maxCount,
	minCount,
	targetValidatorList,
	sort,
	compareFun,
	searchArea,--{range(one,all, row, column), searchCount, searchPortion, effectID}
	isRandom,
	isWanLing,
	
	--state data
	sortFunction,
	targetList,--{{target, effectid}}
}
_G.setmetatable(EffectSearch, {__index = Effect})
---------------------------------------------------------------------------------------------------
function EffectSearch:Init()
	self.targetList = {}
end
---------------------------------------------------------------------------------------------------
function EffectSearch:Apply(curTime)
	if self:ApplyBasic(curTime) == false then
		do return end
	end
	
	--self:SendEffect()
	--self:CalculateEnergy()
	self:GenerateSorFunction()
	if self.isWanLing then
		self:SearchWanLing()
	else
		self:Search()
		local targetCount = 0
		for _, _ in pairs(self.targetList) do
			targetCount = targetCount + 1	
		end
		
		for key, val in pairs(self.targetList) do
			local curEffect = SpellService:GetEffect(val.effectID)
			if curEffect ~= nil then
				curEffect:SetOwnedSpell(self.ownedSpell)
				curEffect:SetOwnedBuff(self.ownedBuff)
				curEffect:SetCaster(self.casterID)
				curEffect.target = val.target
				--curEffect:SetSource(self.targetID)
				--curEffect:SetParam(targetCount)
				curEffect:Apply(self.applyTime)
			end
		end
	end
end
---------------------------------------------------------------------------------------------------
function EffectSearch:IsTargetValidate(target, excludeList)
	--already searched before
	if excludeList[target.guid] then
		return false
	end
	
	local targetValidate = true
	if self.excludeTarget then
		if (target == self.target and self.excludeTarget.target) or
			(target == self.caster and self.excludeTarget.caster)
		then
			return false
		end
	end
	
	if self.targetValidatorList then
		for _, val in pairs(self.targetValidatorList) do
			targetValidate = val(self.caster, target)
			if not(targetValidate) then
				break
			end
		end
	end
	
	return targetValidate
end
---------------------------------------------------------------------------------------------------
function EffectSearch:Search()
	self.targetList = {}
	local excludeList = {}
	local targetList = {}
	local includeList = {}
	local totalCount = 0
	if self.includeTarget then
		for key, effectID in pairs(self.includeTarget) do
			if key == "target" then
				totalCount = totalCount + 1
				excludeList[self.target.guid] = true
				table.insert(includeList, {target = self.target.guid, effectID = effectID})
			elseif key == "caster" then
				totalCount = totalCount + 1
				excludeList[self.casterID.guid] = true
				table.insert(includeList, {target = self.caster.guid, effectID = effectID})
			end
		end
	end
	
	if self.searchArea then
		local playerList, enemyList = SpellService:GetAllUnits()
		--have sort
		if self.sortFunction or self.isRandom then
			for _, val in pairs(playerList) do
				if self:IsTargetValidate(val.guid, excludeList) then
					excludeList[val.guid] = true
					table.insert(targetList, {target = val.guid, effectID = self.searchArea.effectID})
					totalCount = totalCount + 1
				end
			end
			for _, val in pairs(enemyList) do
				if self:IsTargetValidate(val.guid, excludeList) then
					excludeList[guid] = true
					table.insert(targetList, {target = val.guid, effectID = searchArea.effectID})
					totalCount = totalCount + 1
				end
			end
		--no sort, break if reach max count of current area
		else 
			for _, val in pairs(playerList) do
				if self:IsTargetValidate(val.guid, excludeList) then
					if totalCount >= self.maxCount then
						break
					end
					excludeList[val.guid] = true
					table.insert(targetList, {target = val.guid, effectID = searchArea.effectID})
					totalCount = totalCount + 1
				end
			end
			if totalCount < self.maxCount then
				for _, val in pairs(enemyList) do
					if self:IsTargetValidate(val.guid, excludeList) then
						if totalCount >= self.maxCount then
							break
						end
						excludeList[val.guid] = true
						table.insert(targetList, {target = val.guid, effectID = searchArea.effectID})
						totalCount = totalCount + 1
					end
				end
			end
		end
	end
	
	--clear target list if not meet trigger condition
	if self.minCount and totalCount < self.minCount then
		targetList = {}
		includeList = {}
	end
	
	local curCount = 0
	local loseCount = 0
	--target in includelist always added into final targetlist
	for key, val in pairs(includeList) do
		table.insert(self.m_TargetList, val)
		curCount = curCount + 1
	end
	--trim
	if totalCount > self.m_MaxCount then
		local trimTotalCount = totalCount - curCount
		local trimMaxCount = self.m_MaxCount - curCount
		if self.m_SortFunction then
			table.sort(targetList, self.m_SortFunction)
			
			--check need random
			--{target = slotID, effectID = searchArea.effectID}
			--local unit1Attr = self.m_Owner:GetUnit(totalCount).curAttr
			--local unit2Attr = self.m_Owner:GetUnit(target2.target).curAttr
			--local val = self.m_Sort[1]
			
			--local startIndex = self.m_MaxCount
			--local endIndex = totalCount
			--local lastAttr = self.m_Owner:GetUnit(targetList[self.m_MaxCount].target).curAttr
			--local lastValue = lastAttr[self.m_Sort[1]]
			
			local startIndex = trimMaxCount
			local endIndex = trimTotalCount
			local lastAttr = self.m_Owner:GetUnit(targetList[trimMaxCount].target).curAttr
			local lastValue = lastAttr[self.m_Sort[1]]
			
			for index=1, trimMaxCount do
				local curAttr = self.m_Owner:GetUnit(targetList[index].target).curAttr
				local curVal = curAttr[self.m_Sort[1]]
				if curVal == lastValue then
					startIndex = index
					break
				end
			end
			for index=trimMaxCount+1, trimTotalCount do
				local curAttr = self.m_Owner:GetUnit(targetList[index].target).curAttr
				local curVal = curAttr[self.m_Sort[1]]
				if curVal ~= lastValue then
					endIndex = index - 1
					break
				end
			end
			
			if endIndex - trimMaxCount > 0 then
				for index = startIndex, endIndex do
					targetList[index].random = true
				end
			end
			
			for key, val in pairs(targetList) do
				if val.random then
					local dice = math.random(2)
					if curCount<self.m_MaxCount and (dice == 1 or loseCount >= (trimTotalCount - trimMaxCount)) then
						curCount = curCount + 1
					else
						loseCount = loseCount + 1
						targetList[key] = nil
					end
				else
					curCount = curCount + 1
					if curCount > self.m_MaxCount then
						targetList[key] = nil
					end
				end
			end
			
			--previous version
			--[[
			totalCount = 0
			for key, val in pairs(self.m_TargetList) do
				totalCount = totalCount + 1
				if totalCount > self.m_MaxCount then
					self.m_TargetList[key] = nil
				end
			end
			]]
		elseif self.m_Random then
			for key, val in pairs(targetList) do
				local dice = math.random(2)
				if curCount<self.m_MaxCount and (dice == 1 or loseCount >= (totalCount - self.m_MaxCount)) then
					curCount = curCount + 1
				else
					loseCount = loseCount + 1
					targetList[key] = nil
				end
			end
		end
	end
	
	for _, val in pairs(targetList) do
		table.insert(self.m_TargetList, val)
	end
	targetList = nil
end
--*************************************************************************************************
function EffectSearch:SearchWanLing()

	local effectIndex = 1
	local maxSearchArea = #self.m_SearchArea
	while true do
		self:Search()
		local targetCount = #self.m_TargetList
		if (targetCount <= 0) then
		    break
		end
		local repeatCount = math.floor(self.m_MaxCount/targetCount)
		local remainCount = self.m_MaxCount - targetCount * repeatCount
		
		if repeatCount >= 1 then
			for index = 1, targetCount do
				local curEffect = self.m_Owner:GetEffect(self.m_SearchArea[effectIndex].effectID)
				effectIndex = effectIndex + 1
				if effectIndex > maxSearchArea then
				    effectIndex = 1
				end
				if curEffect ~= nil then
					curEffect:SetOwnedSpell(self.m_OwnedSpell)
					curEffect:SetOwnedBuff(self.m_OwnedBuff)
					curEffect:SetCaster(self.m_CasterID)
					curEffect:SetTarget(self.m_TargetList[index].target)
					curEffect:SetSource(self.m_TargetID)
					curEffect:SetParam(targetCount)
					curEffect:Apply(self.m_ApplyTime, isCrazy)
				end
			end
			
			self.m_MaxCount = self.m_MaxCount - targetCount
		else
			for index = 1, remainCount do
				local curEffect = self.m_Owner:GetEffect(self.m_SearchArea[effectIndex].effectID)
				effectIndex = effectIndex + 1
				if effectIndex > maxSearchArea then
				    effectIndex = 1
				end
				if curEffect ~= nil then
					curEffect:SetOwnedSpell(self.m_OwnedSpell)
					curEffect:SetOwnedBuff(self.m_OwnedBuff)
					curEffect:SetCaster(self.m_CasterID)
					curEffect:SetTarget(self.m_TargetList[index].target)
					curEffect:SetSource(self.m_TargetID)
					curEffect:SetParam(targetCount)
					curEffect:Apply(self.m_ApplyTime, isCrazy)
				end
			end
		end
		
		if repeatCount == 0 or (repeatCount == 1 and remainCount == 0) then
			break
		end
	end
	
end
--*************************************************************************************************
function EffectSearch:GenerateSorFunction()

	if self.m_Sort then
		self.m_SortFunction = 
		function (target1, target2)
			local unit1Attr = self.m_Owner:GetUnit(target1.target).curAttr
			local unit2Attr = self.m_Owner:GetUnit(target2.target).curAttr
			local result = false
			--for key, val in pairs(self.m_Sort) do
				local val = self.m_Sort[1]
				local val1 = unit1Attr[val]
				local val2 = unit2Attr[val]
				if val1 == nil or val2 == nil then
					return false
				end
				if val1 ~= val2 then
					if self.m_CompareFun then
						result = self.m_CompareFun(val1, val2)
					else
						result = val1 < val2
					end
					--break
				else
					--result = (math.random(2) == 1)
					result = false
				end
			--end
			return result
		end
	end
	
end
--*************************************************************************************************


