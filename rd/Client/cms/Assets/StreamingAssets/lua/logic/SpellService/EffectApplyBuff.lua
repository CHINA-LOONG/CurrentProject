local string = string
local math = math
local pairs = pairs
local setmetatable = setmetatable
local table = table
local Effect = Effect
local print = print

module "EffectApplyBuff"
---------------------------------------------------------------------------------------------------
EffectApplyBuff = {
	buffID,
	--buffCount = 1,
}
setmetatable(EffectApplyBuff, {__index = Effect.Effect})

---------------------------------------------------------------------------------------------------
function EffectApplyBuff:Apply(curTime)
	if self:ApplyBasic(curTime) == false then
		do return end
	end
	
	local info = string.format("apply buff effect Apply id%s \n", self.id)
	print(info)
	do return end
	
	self:CalculateHit()
	
	local buff = self.owner:GetBuff(self.buffID)
	if self.hit == _G.HIT_SUCCESS then
		if buff ~= nil then
			buff.caster = self.caster
			buff.target = self.target
			buff:SetOwnedSpell(self.ownedSpell)
			buff:Apply(self.applyTime)
			--self:CalculateEnergy()
		end
	end
end
---------------------------------------------------------------------------------------------------
function EffectApplyBuff:CalculateHit()
	--check exclusion
	local activeBuffList = self.target.buffList
	if activeBuffList then
		for _, buff in pairs(activeBuffList) do
			if not(buff.isFinish) then
				if buff.buffExclusionList then
					for _, excludeID in pairs( buff.buffExclusionList) do
						if excludeID == self.buffID then
							self.hit = _G.HIT_IMMUNE
							return
						end
					end
				end
				if buff.buffCategoryExclusionList then
					local targetBuff = self.owner:GetBuff(self.buffID)
					if buff then
						for _, excludeCategory in pairs( buff.buffCategoryExclusionList) do
							if targetBuff.category[excludeCategory] then
								self.hit = _G.HIT_IMMUNE
								return
							end
						end
					end
				end
			end
		end
	end
	
	--not check ally team
	if IsAlly(self.caster.camp, self.target.camp) then
		self.hit = _G.HIT_SUCCESS
		return
	end
	
	self.hit = CalculateBasicHit(self.caster, self.target)
end
---------------------------------------------------------------------------------------------------
