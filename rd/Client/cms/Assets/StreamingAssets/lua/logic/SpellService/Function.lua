---------------------------------------------------------------------------------------------------
--damage releated
function GetInjuryAdjustNum(level)
	print("get injury count 1\n")
	return 1
end
---------------------------------------------------------------------------------------------------
function GetHitRateAdjustNum(levelDiff)
	return 1
end
---------------------------------------------------------------------------------------------------
function GetPropertyInfluenceRatio(propAttacker, propDefender)
	return 1
end
---------------------------------------------------------------------------------------------------
--validators
function IsAlly(caster, target)
	return caster.camp == target.camp
end
---------------------------------------------------------------------------------------------------
function IsEnemy(caster, target)
	return not(IsAlly())
end
---------------------------------------------------------------------------------------------------
function GetChangeValue(baseValue, amountChange, factorChange)
	local finalValue = 0
	if amountChange then
		finalValue = amountChange
	end
	if factorChange then
		finalValue = finalValue + baseValue * factorChange
	end
	
	return finalValue
end
---------------------------------------------------------------------------------------------------