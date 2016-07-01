require "logic.Battle.BattleProcess"

local this;
BattleController = {}

function BattleController:StartBattle()
	this = BattleController;

	-- create all units of this battle
	this.CreateUnit();

	-- 前置剧情动画
	this.PlayPreStoryAnim();

	-- 显示对局UI
	this.ShowUI();

	-- 进程循环
	this.ProcessLoop();

	-- 胜利失败动画
	this.PlayBalanceAnim();

	-- 后置剧情动画
	this.PlayPostStoryAnim();

	-- 结算面板
	this.Balance();
end

function BattleController:CreateUnit()
	print("Creating Battle Units...");
end

function BattleController:PlayPreStoryAnim()
	print("Play Story Animation before any process...");
end

function BattleController:PlayPostStoryAnim()
	print("Play Story Animation after any process...");
end

function BattleController:ProcessLoop()
	print("Pre Process");
	local curProcess = this.GetNextProcess();
	while curProcess ~= nil do
		curProcess.Start();
	end
	print("End Process");
end

function BattleController:ShowUI( ... )
	print("Show Battle UI...")
end

function BattleController:GetNextProcess( ... )
	return BattleProcess.New();
end

function BattleController:Balance( ... )
	print("Show the balance UI of this battle..")
end

-----------------------------------------Event-----------------------------------------------------
function BattleController:OnSkipAnim(  )
	-- body
end