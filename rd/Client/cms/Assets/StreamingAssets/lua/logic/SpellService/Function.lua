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
function IsAlly(caster, target)
	return caster.camp == target.camp
end
---------------------------------------------------------------------------------------------------
function IsEnemy(caster, target)
	return not(IsAlly())
end
---------------------------------------------------------------------------------------------------