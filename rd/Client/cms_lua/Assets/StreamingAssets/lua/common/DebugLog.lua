DebugLog = {}
---------------------------------------------------------------------------------------------------
function DebugLog:Log(msg)
	if not(self.disableLog) then
		print("[Log]"..msg)
	end
end
---------------------------------------------------------------------------------------------------
function DebugLog:LogWarning(msg)
	if not(self.disableLog) then
		print("[Warning]"..msg)
	end
end
---------------------------------------------------------------------------------------------------
function DebugLog:LogError(msg)
	if not(self.disableLog) then
		print("[Error]"..msg)
	end
end
---------------------------------------------------------------------------------------------------