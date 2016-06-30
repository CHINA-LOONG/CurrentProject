---------------------------------------------------------------------------------------------------
EffectApplyBuff = {
	buffID,
	--buffCount = 1,
}
_G.setmetatable(EffectApplyBuff, {__index = Effect})

---------------------------------------------------------------------------------------------------
function EffectApplyBuff:Apply(curTime)
	if self:ApplyBasic(curTime) == false then
		do return end
	end
	
	self:CalculateHit()
	
	local buff = SpellService:GetBuff(self.buffID)
	if self.hit == HIT_SUCCESS then
		if buff ~= nil then
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
							self.hit = HIT_IMMUNE
							return
						end
					end
				end
				if buff.buffCategoryExclusionList then
					local targetBuff = SpellService:GetBuff(self.buffID)
					if buff then
						for _, excludeCategory in pairs( buff.buffCategoryExclusionList) do
							if targetBuff.category[excludeCategory] then
								self.hit = HIT_IMMUNE
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
		self.hit = HIT_SUCCESS
		return
	end
	
	self.hit = CalculateBasicHit(self.caster, self.target)
end
---------------------------------------------------------------------------------------------------
