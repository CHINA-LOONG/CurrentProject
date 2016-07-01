local string = string
local pairs = pairs
local table = table
local Event = Event

module "EventManager"

EventManager = {
	eventList = {},
	waitEventList = {},
}
---------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------
function EventManager:RegisterEvent(eventName, handle, obj)
	local curEvent = self.eventList[eventName]
	if not(curEvent) then
		curEvent = Event(eventName, true)
		self.eventList[eventName] = curEvent
	end
	
	curEvent:Add(handle, obj)	
end
---------------------------------------------------------------------------------------------------
function EventManager:UnRegisterEvent(eventName, handle, obj)
	local curEvent = self.eventList[eventName]
	curEvent:Remove(handle, obj)
	--[[
	if curEvent then
		for _, val in pairs(curEvent) do
			if val.func == handle then
				curEvent:Remove(handle, obj)
			end
		end
	end
	]]
	
	if curEvent.Count == 0 then
		self.eventList[eventName] = nil
	end	
end
---------------------------------------------------------------------------------------------------
--trigger event immediate
function EventManager:TriggerEvent(eventName, eventArgs)
	local curEvent = self.eventList[eventName]
	if curEvent then
		curEvent(eventArgs)
	end
end
---------------------------------------------------------------------------------------------------
--trigger event next update
function EventManager:InvokeEvent(eventName, eventArgs)
	local curEvent = self.eventList[eventName]
	if curEvent then
		table.insert(self.waitEventList, {event = curEvent, args = eventArgs})
	end
end
---------------------------------------------------------------------------------------------------
function EventManager:Update(delTime)
	for _, val in pairs(self.waitEventList) do
		if val.event then
			val.event(val.args)
		end
	end	
	
	self.waitEventList = {}
end
---------------------------------------------------------------------------------------------------