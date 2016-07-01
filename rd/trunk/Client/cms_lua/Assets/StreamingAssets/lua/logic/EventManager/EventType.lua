--game event
--spell releated Event
--RecordEvent = {
	EVENT_FIRE_SPELL = 1--EventArgs = {triggerTime,spellID,casterID,castResult}
	EVENT_LIFE_CHANGE = 2--EventArgs = {triggerTime, casterID, targetID, isCritical, lifeChange, lifeCurrent, lifeMax}
	EVENT_ENERGY_CHANGE = 3--EventArgs = {triggerTime, curEnergy, maxEnergy, targetID}
	EVENT_UNIT_DEAD = 4--EventArgs = {triggerTime, casterID, deathID}
	EVENT_BUFF = 5--EventArgs = {triggerTime, casterID, targetID, buffID, isAdd}
	EVENT_EFFECT = 6--EventArgs = {triggerTime, casterID, targetID, effectID, }
	EVENT_MISS = 7--EventArgs = {triggerTime, targetID}
	EVENT_IMMUNE = 8--EventArgs = {triggerTime, targetID}
--}