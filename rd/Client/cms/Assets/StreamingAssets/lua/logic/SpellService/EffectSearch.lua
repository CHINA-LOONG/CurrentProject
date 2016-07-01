local string = string
local math = math
local pairs = pairs
local setmetatable = setmetatable
local table = table
local Effect = Effect
local print = print

module "EffectSearch"
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
	
	--state data
	sortFunction,
	targetList,--{{target, effectid}}
}
setmetatable(EffectSearch, {__index = Effect.Effect})
---------------------------------------------------------------------------------------------------
function EffectSearch:Init()
	self.targetList = {}
end
---------------------------------------------------------------------------------------------------
function EffectSearch:Apply(curTime)
	if self:ApplyBasic(curTime) == false then
		do return end
	end
	
	local info = string.format("search effect Apply id%s \n", self.id)
	print(info)
	do return end
	
	--self:SendEffect()
	--self:CalculateEnergy()
	self:GenerateSorFunction()
	self:Search()
	local targetCount = 0
	for _, _ in pairs(self.targetList) do
		targetCount = targetCount + 1	
	end
	
	for key, val in pairs(self.targetList) do
		local curEffect = self.owner:GetEffect(val.effectID)
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
		local playerList, enemyList = self.owner:GetAllUnits()
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
		table.insert(self.targetList, val)
		curCount = curCount + 1
	end
	--trim
	if totalCount > self.maxCount then
		local trimTotalCount = totalCount - curCount
		local trimMaxCount = self.maxCount - curCount
		if self.sortFunction then
			table.sort(targetList, self.sortFunction)
			
			local startIndex = trimMaxCount
			local endIndex = trimTotalCount
			local targetUnit = self.owner:GetUnit(targetList[trimMaxCount].target)
			local lastValue = targetUnit[self.sort[1]]
			
			for index=1, trimMaxCount do
				local curUnit = self.owner:GetUnit(targetList[index].target)
				local curVal = curUnit[self.sort[1]]
				if curVal == lastValue then
					startIndex = index
					break
				end
			end
			for index=trimMaxCount+1, trimTotalCount do
				local curUnit = self.owner:GetUnit(targetList[index].target)
				local curVal = curUnit[self.sort[1]]
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
					if curCount<self.maxCount and (dice == 1 or loseCount >= (trimTotalCount - trimMaxCount)) then
						curCount = curCount + 1
					else
						loseCount = loseCount + 1
						targetList[key] = nil
					end
				else
					curCount = curCount + 1
					if curCount > self.maxCount then
						targetList[key] = nil
					end
				end
			end
		elseif self.isRandom then
			for key, val in pairs(targetList) do
				local dice = math.random(2)
				if curCount<self.maxCount and (dice == 1 or loseCount >= (totalCount - self.maxCount)) then
					curCount = curCount + 1
				else
					loseCount = loseCount + 1
					targetList[key] = nil
				end
			end
		end
	end
	
	for _, val in pairs(targetList) do
		table.insert(self.targetList, val)
	end
	targetList = nil
end
--*************************************************************************************************
function EffectSearch:GenerateSorFunction()

	if self.sort then
		self.sortFunction = 
		function (target1, target2)
			local unit1 = self.owner:GetUnit(target1.guid)
			local unit2 = self.owner:GetUnit(target2.guid)
			local result = false
			--for key, val in pairs(self.m_Sort) do
				local val = self.sort[1]
				local val1 = unit1[val]
				local val2 = unit2[val]
				if val1 == nil or val2 == nil then
					return false
				end
				if val1 ~= val2 then
					if self.compareFun then
						result = self.compareFun(val1, val2)
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


