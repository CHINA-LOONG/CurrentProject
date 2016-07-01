---------------------------------------------------------------------------------------------------
--damage releated
function GetInjuryAdjustNum(level)
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
function IsAlly(param)
	return param.caster.camp == param.target.camp
end
---------------------------------------------------------------------------------------------------
function IsEnemy(param)
	return not(IsAlly(param))
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