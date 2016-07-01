require "logic.Battle.BattleProcess"

BattleController = {
	battleGroup = nil,
}

testProcess = 5
processCount = 0

function BattleController:StartBattle(group)
	self.battleGroup = group

	-- create all units of this battle
	self:CreateUnit()

	-- 前置剧情动画
	self:PlayPreStoryAnim()

	-- 显示对局UI
	self:ShowUI()

	-- 开始进程
	self:StartProcess()

end

function BattleController:CreateUnit()
	print("<Battle>Creating Battle Units...")
end

function BattleController:PlayPreStoryAnim()
	print("<Battle>Play Story Animation before any process...")
end

function BattleController:PlayPostStoryAnim()
	print("<Battle>Play Story Animation after any process...")
end

function BattleController:StartProcess()
	local curProcess = self:GetNextProcess()
	if curProcess ~= nil then
		curProcess:Start(self.battleGroup, function ()
			self:OnEndProcess()
		end)
	else
		self:OnEndAllProcess();
	end
end

function BattleController:OnEndProcess()
	print("<Battle>OnEndProcess Callback")
	self:StartProcess();
end

function BattleController:OnEndAllProcess()
	print("<Battle>OnEndAllProcess")
	-- 胜利失败动画
	self:PlayBalanceAnim()

	-- 后置剧情动画
	self:PlayPostStoryAnim()

	-- 结算面板
	self:ShowBalanceUI()
end

function BattleController:ShowUI( ... )
	print("<Battle>Show Battle UI...")
end

function BattleController:GetNextProcess( ... )
	if processCount < testProcess then	
		processCount = processCount+1
		return BattleProcess:New()
	else
		return nil
	end
end

function BattleController:PlayBalanceAnim( ... )
	print("<Battle>Playing Balance Animation...")
end

function BattleController:ShowBalanceUI( ... )
	print("<Battle>Show the balance UI of this battle...")
end

-----------------------------------------Event-----------------------------------------------------
function BattleController:OnSkipAnim(  )
	-- body
end