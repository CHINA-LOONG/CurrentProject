local string = string
local math = math
local pairs = pairs
local setmetatable = setmetatable
local table = table
local Effect = Effect
local print = print

module "EffectSet"
---------------------------------------------------------------------------------------------------
EffectSet = {
	effectList,
	--minCount,
	--maxCount,
	--isRandom,
	--randomCount,
}
setmetatable(EffectSet, {__index = Effect.Effect})

---------------------------------------------------------------------------------------------------
--[[
function EffectSet:SetParam(...)

	self.m_Param1 = arg[1]
	self.m_Param2 = arg[2]
	
end
]]
---------------------------------------------------------------------------------------------------
function EffectSet:Apply(curTime)

	if self:ApplyBasic(curTime) == false then
		do return end
	end
	
	local info = string.format("set effect Apply id%s \n", self.id)
	print(info)
	do return end
	
	--self:CalculateEnergy()
	if self.effectList == nil then
		return
	end
	
	--no min/max count, no random in fact, think too much 
	--[[
	--calculate totalEffectcount first
	local effectTotalCount = 0
	for _, _ in pairs(self.effectList) do
		effectTotalCount = effectTotalCount + 1
	end
	
	local effectCount
	if self.maxCount then
	    effectCount = self.maxCount
    else
	    effectCount = #self.effectList
    end 
	 
	if self.randomCount then
	    if self.minCount then
		    effectCount = math.random(self.minCount, effectCount)
		else
		    effectCount = math.random(0, effectCount)
		end
	end
	
	local activeEffectIDList = {}
	if self.isRandom then
		for index = 1, effectCount do
			local randomNum = math.random(1, effectTotalCount)
			table.insert(activeEffectIDList, self.effectList[randomNum])
		end
	else
		local effectIndex = 1
		for index = 1, effectCount do
			if effectIndex > effectTotalCount then
				effectIndex = 1
			end
			table.insert(activeEffectIDList, self.effectList[effectIndex])
			effectIndex = effectIndex + 1
		end
	end
	]]

	for _, effectID in pairs(self.effectList) do
		local curEffect = self.owner:GetEffect(effectID)
		if curEffect ~= nil then
			curEffect:SetOwnedBuff(self.ownedBuff)
			curEffect:SetOwnedSpell(self.ownedSpell)
			--the target may changed by its parent effect, so reset target here
			curEffect.target = self.target
			curEffect:Apply(self.applyTime)
			--curEffect:SetSource(self.sourceID)
			--curEffect:SetParam(self.param1, self.param2)
			--self.m_Owner:AddActiveEffect(curEffect)
		end
	end
end
---------------------------------------------------------------------------------------------------

