package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map.Entry;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;
import org.hawk.xid.HawkXID;

import com.hawk.game.ServerData;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.HoleCfg;
import com.hawk.game.config.InstanceCfg;
import com.hawk.game.config.InstanceChapterCfg;
import com.hawk.game.config.InstanceDropCfg;
import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.config.InstanceResetCfg;
import com.hawk.game.config.InstanceRewardCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.config.TowerCfg;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Instance.HSBattle;
import com.hawk.game.protocol.Instance.HSChapterBox;
import com.hawk.game.protocol.Instance.HSChapterBoxRet;
import com.hawk.game.protocol.Instance.HSGuildInstanceEnter;
import com.hawk.game.protocol.Instance.HSHoleEnter;
import com.hawk.game.protocol.Instance.HSInstanceEnter;
import com.hawk.game.protocol.Instance.HSInstanceEnterRet;
import com.hawk.game.protocol.Instance.HSInstanceResetCount;
import com.hawk.game.protocol.Instance.HSInstanceResetCountRet;
import com.hawk.game.protocol.Instance.HSInstanceReviveRet;
import com.hawk.game.protocol.Instance.HSInstanceSettle;
import com.hawk.game.protocol.Instance.HSInstanceSettleRet;
import com.hawk.game.protocol.Instance.HSInstanceSweep;
import com.hawk.game.protocol.Instance.HSInstanceSweepRet;
import com.hawk.game.protocol.Instance.HSTowerEnter;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.InstanceUtil;
import com.hawk.game.util.InstanceUtil.InstanceChapter;

public class PlayerInstanceModule extends PlayerModule {

	// 本次副本类型
	private int curType;
	// 本次副本Id
	private String curInstanceId;
	// 本次对局列表
	private List<HSBattle> curBattleList;
	// 本次副本掉落列表
	private List<ItemInfo> curDropList;
	// 本次上阵怪物列表
	private List<Integer> curMonsterList;
	// 本次副本复活次数
	private int curReviveCount;
	// 本次副本难度
	private int curDifficulty;
	// 本次副本章节Id
	private int curChapterId;
	// 本次副本章节索引
	private int curIndex;
	// 本次洞id
	private int curHoleId;
	// 本次塔id
	private int curTowerId;
	// 刷新类型掩码，副本开始后如果发生刷新，旧周期开始的副本结算时不影响新周期的数据
	// 方法在单线程内调用，不需volatile
	private int refreshMask;

	public PlayerInstanceModule(Player player) {
		super(player);

		curBattleList = new ArrayList<HSBattle>();
		curDropList = new ArrayList<ItemInfo>();
		curMonsterList = new ArrayList<Integer>();

		clearCurData();
	}

	/**
	 * 获取助战列表
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_ASSIST_C_VALUE)
	private boolean onInstanceAssist(HawkProtocol cmd) {
//		HSInstanceAssist protocol = cmd.parseProtocol(HSInstanceAssist.getDefaultInstance());
//		int hsCode = cmd.getType();
//
//		HSInstanceAssistRet.Builder response = HSInstanceAssistRet.newBuilder();
//		//response.addAllAssist(values);
//		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ASSIST_S, response));
		return true;
	}

	/**
	 * 副本进入
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_ENTER_C_VALUE)
	private boolean onInstanceEnter(HawkProtocol cmd) {
		HSInstanceEnter protocol = cmd.parseProtocol(HSInstanceEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		String instanceId = protocol.getInstanceId();
		List<Integer> battleMonsterList = protocol.getBattleMonsterIdList();
		if (true == protocol.hasFriendId()) {
//			int friendId = protocol.getFriendId();
		}

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}
		InstanceChapterCfg chapterCfg = entryCfg.getChapterCfg();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// 副本开启
		if (false == isInstanceOpen(entryCfg)) {
			sendError(hsCode, Status.instanceError.INSTANCE_NOT_OPEN);
			return true;
		}

		// 副本等级
		if (player.getLevel() < chapterCfg.getLevelByDifficulty(entryCfg.getDifficult())) {
			sendError(hsCode, Status.instanceError.INSTANCE_LEVEL);
			return true;
		}

		// 次数
		if (statisticsEntity.getInstanceEnterTimesDaily(instanceId) >= entryCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return true;
		}

		// 体力
		if (player.regainFatigue() < entryCfg.getFatigue()) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return true;
		}

		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, instanceId);
		if (instanceCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		// 阵型
		int battleMonsterCode = checkBattleMonsterList(battleMonsterList);
		if (Status.error.NONE_ERROR_VALUE != battleMonsterCode) {
			sendError(hsCode, battleMonsterCode);
			return true;
		}

		// 满足条件，进入副本
		if (false == isCurDataClear()) {
			HawkLog.errPrintln("instance data is not empty when enter instance");
			clearCurData();
		}
		this.curType = Const.InstanceType.INSTANCE_STORY_VALUE;
		this.curInstanceId = instanceId;
		this.curDifficulty = entryCfg.getDifficult();
		this.curChapterId = entryCfg.getChapter();
		this.curIndex = entryCfg.getIndex();
		this.curMonsterList.addAll(battleMonsterList);

		// 生成副本
		genInstance(instanceId);

		// 进副本消耗1点体力
		ConsumeItems consumeFatigue = ConsumeItems.valueOf();
		consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, 1);
		consumeFatigue.consumeTakeAffectAndPush(player, Action.NORMAL_INSTANCE, hsCode);

		// 次数修改
		statisticsEntity.increaseInstanceEnterTimesDaily(instanceId, 1);
		statisticsEntity.notifyUpdate(true);

		HSInstanceEnterRet.Builder response = HSInstanceEnterRet.newBuilder();
		response.setInstanceId(instanceId);
		response.addAllBattle(this.curBattleList);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ENTER_S, response));
		return true;
	}

	/**
	 * 洞进入
	 */
	@ProtocolHandler(code = HS.code.HOLE_ENTER_C_VALUE)
	private boolean onHoleEnter(HawkProtocol cmd) {
		HSHoleEnter protocol = cmd.parseProtocol(HSHoleEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		int holeId = protocol.getHoleId();
		String instanceId = protocol.getInstanceId();
		List<Integer> battleMonsterList = protocol.getBattleMonsterIdList();

		HoleCfg holeCfg = HawkConfigManager.getInstance().getConfigByKey(HoleCfg.class, holeId);
		if (holeCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		String[] instanceList = holeCfg.getInstanceList();
		boolean contain = false;
		for (String id : instanceList) {
			if (id.equals(instanceId)) {
				contain = true;
			}
		}
		if (false == contain) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, instanceId);
		if (instanceCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// 洞开启
		if (false == ServerData.getInstance().isHoleOpen(holeId)) {
			sendError(hsCode, Status.instanceError.INSTANCE_NOT_OPEN);
			return true;
		}

		// 次数
		if (statisticsEntity.getHoleTimesDaily(holeId) >= holeCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return true;
		}

		// 等级，使用副本怪物等级判断
		if (player.getLevel() < instanceCfg.getMonsterLevel()) {
			sendError(hsCode, Status.instanceError.INSTANCE_LEVEL);
			return true;
		}

		// 体力
		if (player.regainFatigue() < entryCfg.getFatigue()) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return true;
		}

		// 阵型
		int battleMonsterCode = checkBattleMonsterList(battleMonsterList);
		if (Status.error.NONE_ERROR_VALUE != battleMonsterCode) {
			sendError(hsCode, battleMonsterCode);
			return true;
		}

		// 满足条件，进入副本
		if (false == isCurDataClear()) {
			HawkLog.errPrintln("instance data is not empty when enter instance");
			clearCurData();
		}
		this.curType = Const.InstanceType.INSTANCE_HOLE_VALUE;
		this.curInstanceId = instanceId;
		this.curMonsterList.addAll(battleMonsterList);
		this.curHoleId = holeId;

		// 生成副本
		genInstance(instanceId);

		// 进副本消耗全部体力
		ConsumeItems consumeFatigue = ConsumeItems.valueOf();
		consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, entryCfg.getFatigue());
		consumeFatigue.consumeTakeAffectAndPush(player, Action.HOLE_INSTANCE, hsCode);

		// 次数修改
		statisticsEntity.increaseHoleTimes(holeId);
		statisticsEntity.increaseHoleTimesDaily(holeId);
		statisticsEntity.notifyUpdate(true);

		HSInstanceEnterRet.Builder response = HSInstanceEnterRet.newBuilder();
		response.setInstanceId(instanceId);
		response.addAllBattle(this.curBattleList);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ENTER_S, response));

		return true;
	}

	/**
	 * 塔进入
	 */
	@ProtocolHandler(code = HS.code.TOWER_ENTER_C_VALUE)
	private boolean onTowerEnter(HawkProtocol cmd) {
		HSTowerEnter protocol = cmd.parseProtocol(HSTowerEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		int towerId = protocol.getTowerId();
		int floor = protocol.getFloor();
		List<Integer> battleMonsterList = protocol.getBattleMonsterIdList();

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		TowerCfg towerCfg = HawkConfigManager.getInstance().getConfigByKey(TowerCfg.class, towerId);
		if (towerCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		if (floor -1 != statisticsEntity.getTowerFloor(towerId)) {
			sendError(hsCode, Status.instanceError.TOWER_FLOOR);
			return true;
		}

		String[] instanceList = towerCfg.getInstanceList();
		if (floor > instanceList.length) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return true;
		}
		String instanceId = instanceList[floor - 1];

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		// 等级
		if (player.getLevel() < towerCfg.getLevel()) {
			sendError(hsCode, Status.instanceError.INSTANCE_LEVEL);
			return true;
		}

		// 体力
		if (player.regainFatigue() < entryCfg.getFatigue()) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return true;
		}

		// 阵型
		int battleMonsterCode = checkBattleMonsterList(battleMonsterList);
		if (Status.error.NONE_ERROR_VALUE != battleMonsterCode) {
			sendError(hsCode, battleMonsterCode);
			return true;
		}

		// 满足条件，进入副本
		if (false == isCurDataClear()) {
			HawkLog.errPrintln("instance data is not empty when enter instance");
			clearCurData();
		}
		this.curType = Const.InstanceType.INSTANCE_TOWER_VALUE;
		this.curInstanceId = instanceId;
		this.curMonsterList.addAll(battleMonsterList);
		this.curTowerId = towerId;

		// 生成副本
		genInstance(instanceId);

		// 进副本消耗全部体力
		ConsumeItems consumeFatigue = ConsumeItems.valueOf();
		consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, entryCfg.getFatigue());
		consumeFatigue.consumeTakeAffectAndPush(player, Action.TOWER_INSTANCE, hsCode);

		HSInstanceEnterRet.Builder response = HSInstanceEnterRet.newBuilder();
		response.setInstanceId(instanceId);
		response.addAllBattle(this.curBattleList);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ENTER_S, response));

		return true;
	}

	/**
	 * 公会副本进入
	 */
	@ProtocolHandler(code = HS.code.GUILD_INSTANCE_ENTER_C_VALUE)
	private boolean onGuildInstanceEnter(HawkProtocol cmd) {
		HSGuildInstanceEnter protocol = cmd.parseProtocol(HSGuildInstanceEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		String instanceId = protocol.getInstanceId();
		List<Integer> battleMonsterList = protocol.getBattleMonsterIdList();

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, instanceId);
		if (instanceCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		// 体力
		if (player.regainFatigue() < entryCfg.getFatigue()) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return true;
		}

		// 阵型
		int battleMonsterCode = checkBattleMonsterList(battleMonsterList);
		if (Status.error.NONE_ERROR_VALUE != battleMonsterCode) {
			sendError(hsCode, battleMonsterCode);
			return true;
		}

		// 满足条件，进入副本
		if (false == isCurDataClear()) {
			HawkLog.errPrintln("instance data is not empty when enter instance");
			clearCurData();
		}
		this.curType = Const.InstanceType.INSTANCE_GUILD_VALUE;
		this.curInstanceId = instanceId;
		this.curMonsterList.addAll(battleMonsterList);

		// 生成副本
		genInstance(instanceId);

		// 进副本消耗1点体力
		ConsumeItems consumeFatigue = ConsumeItems.valueOf();
		consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, 1);
		consumeFatigue.consumeTakeAffectAndPush(player, Action.GUILD_INSTANCE, hsCode);

		HSInstanceEnterRet.Builder response = HSInstanceEnterRet.newBuilder();
		response.setInstanceId(instanceId);
		response.addAllBattle(this.curBattleList);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ENTER_S, response));
		return true;
	}

	/**
	 * 副本结算
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_SETTLE_C_VALUE)
	private boolean onInstanceSettle(HawkProtocol cmd) {
		HSInstanceSettle protocol = cmd.parseProtocol(HSInstanceSettle.getDefaultInstance());
		int hsCode = cmd.getType();
		int passBattleCount = protocol.getPassBattleCount();
		int deadMonsterCount = protocol.getDeadMonsterCount();

		if (this.curInstanceId == ""){
			sendError(hsCode, Status.instanceError.INSTANCE_NOT_ENTER_VALUE);
			HawkException.catchException(new RuntimeException("instanceSettle"));
			return true;
		}

		switch (this.curType) {
		case Const.InstanceType.INSTANCE_STORY_VALUE:
			settleStory(passBattleCount, deadMonsterCount, hsCode);
			break;

		case Const.InstanceType.INSTANCE_HOLE_VALUE:
			settleHole(passBattleCount, hsCode);
			break;

		case Const.InstanceType.INSTANCE_TOWER_VALUE:
			settleTower(passBattleCount, hsCode);
			break;

		case Const.InstanceType.INSTANCE_GUILD_VALUE:
			settleGuild(passBattleCount, hsCode);
			break;

		default:
			sendError(hsCode, Status.instanceError.INSTANCE_NOT_ENTER_VALUE);
			HawkException.catchException(new RuntimeException("instanceSettle"));
			return true;
		}

		// 清空副本数据
		clearCurData();
		return true;
	}

	/**
	 * 扫荡
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_SWEEP_C_VALUE)
	private boolean onInstanceSweep(HawkProtocol cmd) {
		HSInstanceSweep protocol = cmd.parseProtocol(HSInstanceSweep.getDefaultInstance());
		int hsCode = cmd.getType();
		String instanceId = protocol.getInstanceId();
		int count = protocol.getCount();

		if (count < 1) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// 体力
		int fatigueChange = count * entryCfg.getFatigue();
		ConsumeItems consumeFatigue = ConsumeItems.valueOf();
		consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, fatigueChange);
		if (false == consumeFatigue.checkConsume(player, hsCode)) {
			return true;
		}

		// 次数
		if (statisticsEntity.getInstanceEnterTimesDaily(instanceId) + count > entryCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return true;
		}

		// 扫荡券
		ConsumeItems consumeItem = ConsumeItems.valueOf();
		consumeItem.addItem(GsConst.SWEEP_TICKET, count);
		if (false == consumeItem.checkConsume(player, hsCode)) {
			return true;
		}

		// 奖励
		List<AwardItems> completeRewardList = new ArrayList<AwardItems>();
		AwardItems sweepReward = AwardItems.valueOf();
		// TODO 是否可优化
		AwardItems allReward = AwardItems.valueOf();

		InstanceRewardCfg instanceRewardCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceRewardCfg.class, instanceId);
		if (instanceRewardCfg != null) {
			// 扫荡不给宠物经验，配置保证
			for (int i = 0; i < count; ++i) {
				AwardItems completeReward = AwardItems.valueOf();
				List<ItemInfo> rewardList = instanceRewardCfg.getReward().getRewardList();
				allReward.addItemInfos(rewardList);
				completeReward.addItemInfos(rewardList);
				completeRewardList.add(completeReward);

				rewardList = instanceRewardCfg.getSweepReward().getRewardList();
				allReward.addItemInfos(rewardList);
				sweepReward.addItemInfos(rewardList);
			}
		}

		consumeItem.consumeTakeAffectAndPush(player, Action.RAID, hsCode);
		consumeFatigue.consumeTakeAffectAndPush(player, Action.RAID, hsCode);
		allReward.rewardTakeAffectAndPush(player, Action.RAID, hsCode);

		statisticsEntity.increaseInstanceEnterTimesDaily(instanceId, count);
		statisticsEntity.notifyUpdate(true);

		HSInstanceSweepRet.Builder response = HSInstanceSweepRet.newBuilder();
		for (AwardItems award : completeRewardList) {
			response.addCompleteReward(award.getBuilder());
		}
		response.setSweepReward(sweepReward.getBuilder());
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SWEEP_S, response));
		return true;
	}

	/**
	 * 重置次数
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_RESET_COUNT_C_VALUE)
	private boolean onInstanceResetCount(HawkProtocol cmd) {
		HSInstanceResetCount protocol = cmd.parseProtocol(HSInstanceResetCount.getDefaultInstance());
		int hsCode = cmd.getType();
		String instanceId = protocol.getInstanceId();

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		if (0 == statisticsEntity.getInstanceEnterTimesDaily(instanceId)) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		InstanceResetCfg resetCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceResetCfg.class, GsConst.INSTANCE_RESET_ID);
		if (resetCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR_VALUE);
			return true;
		}

		int resetTimes = player.getPlayerData().getStatisticsEntity().getInstanceResetTimesDaily();
//		// 如有重置次数上限
//		if (resetCfg.getMaxTimes() != GsConst.UNUSABLE && resetTimes >= resetCfg.getMaxTimes()) {
//			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
//			return true;
//		}

		ConsumeItems consume = ConsumeItems.valueOf();
		int mutiple = (int)Math.pow(2, resetTimes / resetCfg.getDoubleTimes());
		int goldCost = mutiple * (resetCfg.getConsume() + resetTimes * resetCfg.getConsumeAdd());
		consume.addGold(goldCost);

		if (false == consume.checkConsume(player, hsCode)) {
			return true;
		}

		consume.consumeTakeAffectAndPush(player, Action.INSTANCE_RESET, hsCode);

		statisticsEntity.setInstanceEnterTimesDaily(instanceId, 0);
		statisticsEntity.increaseInstanceResetTimesDaily();
		statisticsEntity.notifyUpdate(true);

		HSInstanceResetCountRet.Builder response = HSInstanceResetCountRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_RESET_COUNT_S, response));

		return true;
	}

	/**
	 * 复活
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_REVIVE_C_VALUE)
	private boolean onInstanceRevive(HawkProtocol cmd) {
		int hsCode = cmd.getType();

		int reviveCount = this.curReviveCount + 1;
		if (reviveCount > GsConst.INSTANCE_REVIVE_COUNT) {
			sendError(hsCode, Status.instanceError.INSTANCE_REVIVE_COUNT);
			return true;
		}

		if(player.getGold() < GsConst.INSTANCE_REVIVE_CONSUME[reviveCount - 1]){
			sendError(hsCode, Status.PlayerError.GOLD_NOT_ENOUGH_VALUE);
			return true;
		}

		ConsumeItems consume = ConsumeItems.valueOf();
		consume.addGold(GsConst.INSTANCE_REVIVE_CONSUME[reviveCount - 1]);
		consume.consumeTakeAffectAndPush(player, Action.REVIVE, HS.code.INSTANCE_REVIVE_C_VALUE);

		this.curReviveCount = reviveCount;

		HSInstanceReviveRet.Builder response = HSInstanceReviveRet.newBuilder();
		response.setReviveCount(this.curReviveCount);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_REVIVE_S, response));

		return true;
	}

	/**
	 * 开满星宝箱
	 */
	@ProtocolHandler(code = HS.code.CHAPTER_BOX_C_VALUE)
	private boolean onInstanceBox(HawkProtocol cmd) {
		HSChapterBox protocol = cmd.parseProtocol(HSChapterBox.getDefaultInstance());
		int hsCode = cmd.getType();
		int chapterId = protocol.getChapterId();
		int difficulty = protocol.getDifficulty();

		if (difficulty != GsConst.InstanceDifficulty.NORMAL_INSTANCE && difficulty != GsConst.InstanceDifficulty.HARD_INSTANCE) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return true;
		}

		InstanceChapterCfg chapterCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceChapterCfg.class, chapterId);
		if (chapterCfg == null) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return true;
		}

		RewardCfg rewardCfg = chapterCfg.getRewardByDifficulty(difficulty);
		if (rewardCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR_VALUE);
			return true;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int boxState = Const.ChapterBoxState.INVALID_VALUE;

		switch (difficulty) {
		case GsConst.InstanceDifficulty.NORMAL_INSTANCE:
			boxState = statisticsEntity.getNormalChapterBoxState(chapterId);
			break;
		case GsConst.InstanceDifficulty.HARD_INSTANCE:
			boxState = statisticsEntity.getHardChapterBoxState(chapterId);
			break;
		}

		if (boxState == Const.ChapterBoxState.INVALID_VALUE) {
			sendError(hsCode, Status.instanceError.CHAPTER_BOX_STAR_COUNT_VALUE);
			return true;
		} else if (boxState == Const.ChapterBoxState.OPEN_VALUE) {
			sendError(hsCode, Status.instanceError.CHAPTER_BOX_ALREADY_OPEN_VALUE);
			return true;
		} 

		boxState = Const.ChapterBoxState.OPEN_VALUE;

		AwardItems awardItems = new AwardItems();
		awardItems.addItemInfos(rewardCfg.getRewardList());
		awardItems.rewardTakeAffectAndPush(player,  Action.INSTANCE_STAGE_REWARD, hsCode);

		switch (difficulty) {
		case GsConst.InstanceDifficulty.NORMAL_INSTANCE:
			statisticsEntity.setNormalChapterBoxState(chapterId, boxState);
			break;
		case GsConst.InstanceDifficulty.HARD_INSTANCE:
			statisticsEntity.setHardChapterBoxState(chapterId, boxState);
			break;
		}
		statisticsEntity.notifyUpdate(true);

		HSChapterBoxRet.Builder response = HSChapterBoxRet.newBuilder();
		response.setChapterId(chapterId);
		response.setDifficulty(difficulty);
		response.setBoxState(boxState);
		sendProtocol(HawkProtocol.valueOf(HS.code.CHAPTER_BOX_S, response));

		return true;
	}

	// 内部函数--------------------------------------------------------------------------------------

	/**
	 * 章节是否完成
	 */
	private boolean isChapterComplete(int chapterId, int difficulty) {
		// 第0章特殊，表示还未完成任何副本
		if (chapterId == 0) {
			return true;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int topChapterId = 0;
		int topIndex = 0;
		int topMaxIndex = 0;

		switch (difficulty) {
		case GsConst.InstanceDifficulty.NORMAL_INSTANCE:
			topChapterId = statisticsEntity.getNormalTopChapter();
			if (topChapterId == 0) {
				return false;
			} else if (chapterId < topChapterId) {
				return true;
			}

			topIndex = statisticsEntity.getNormalTopIndex();
			topMaxIndex = InstanceUtil.getInstanceChapter(topChapterId).normalList.size() - 1;
			break;
		case GsConst.InstanceDifficulty.HARD_INSTANCE:
			topChapterId = statisticsEntity.getHardTopChapter();
			if (topChapterId == 0) {
				return false;
			} else if (chapterId < topChapterId) {
				return true;
			}

			topIndex = statisticsEntity.getHardTopIndex();
			topMaxIndex = InstanceUtil.getInstanceChapter(topChapterId).hardList.size() - 1;
			break;
		default:
			return false;
		}

		if (chapterId == topChapterId && topIndex == topMaxIndex) {
			return true;
		}
		return false;
	}

	/**
	 * 章节是否满星
	 */
	private boolean isChapterFullStar(int chapterId, int difficulty) {
		InstanceChapter chapter = InstanceUtil.getInstanceChapter(chapterId);
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int boxState = Const.ChapterBoxState.INVALID_VALUE;
		List<InstanceEntryCfg> instanceList = null;

		switch(difficulty) {
		case GsConst.InstanceDifficulty.NORMAL_INSTANCE:
			boxState = statisticsEntity.getNormalChapterBoxState(chapterId);
			instanceList = chapter.normalList;
			break;
		case GsConst.InstanceDifficulty.HARD_INSTANCE:
			boxState = statisticsEntity.getHardChapterBoxState(chapterId);
			instanceList = chapter.hardList;
			break;
		default:
			return false;
		}

		if (boxState == Const.ChapterBoxState.INVALID_VALUE) {
			for (InstanceEntryCfg entry : instanceList) {
				if (statisticsEntity.getInstanceStar(entry.getInstanceId()) != GsConst.MAX_INSTANCE_STAR) {
					return false;
				}
			}
		}
		return true;
	}

	/**
	 * 副本是否开启
	 */
	private boolean isInstanceOpen(InstanceEntryCfg entryCfg) {
		int chapterId = entryCfg.getChapter();
		int difficulty = entryCfg.getDifficult();
		int index = entryCfg.getIndex();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// 必须通关前置副本、前置章节
		// 精英副本必须通关普通章节
		if (difficulty == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
			int topIndex = statisticsEntity.getNormalTopIndex();
			int topChapterId = statisticsEntity.getNormalTopChapter();

			if (chapterId > topChapterId + 1
					|| (chapterId == topChapterId + 1
						&& (index > 0 || false == isChapterComplete(topChapterId, difficulty))
					|| (chapterId == topChapterId && index > topIndex + 1))) {
				return false;
			}
		} else if (difficulty == GsConst.InstanceDifficulty.HARD_INSTANCE) {
			int topIndex = statisticsEntity.getHardTopIndex();
			int topChapterId = statisticsEntity.getHardTopChapter();
			int topNormalChapterId = statisticsEntity.getNormalTopChapter();

			if (chapterId > topNormalChapterId
					|| (chapterId == topNormalChapterId && false == isChapterComplete(topNormalChapterId, GsConst.InstanceDifficulty.NORMAL_INSTANCE))
					|| chapterId > topChapterId + 1
					|| (chapterId == topChapterId + 1
						&& (index > 0 || false == isChapterComplete(topChapterId, difficulty))
					|| (chapterId == topChapterId && index > topIndex + 1))) {
						return false;
			}
		}

		return true;
	}

	/**
	 * 检查阵容是否合法
	 */
	private int checkBattleMonsterList(List<Integer> battleMonsterList) {
		// 阵型
		int size = battleMonsterList.size();
		if (size == 0 || size > GsConst.MAX_BATTLE_MONSTER_COUNT) {
			return Status.monsterError.BATTLE_MONSTER_COUNT_VALUE;
		}

		for (int i = 0; i < size; ++i) {
			int monsterId = battleMonsterList.get(i);
			// 去重，通用于arrayList和linkedList优化
			if (i != battleMonsterList.lastIndexOf(monsterId)) {
				return Status.error.PARAMS_INVALID_VALUE;
			}

			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (monsterEntity == null) {
				return Status.monsterError.MONSTER_NOT_EXIST_VALUE;
			}
			if (true == player.isMonsterBusy(monsterId)) {
				return Status.monsterError.MONSTER_BUSY_VALUE;
			}
		}

		return Status.error.NONE_ERROR_VALUE;
	}

	/**
	 * 生成副本数据
	 */
	private boolean genInstance(String instanceId) {
		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, instanceId);
		if (instanceCfg == null) {
			return false;
		}

		List<String> orderedMonsterList = new ArrayList<String>();

		// 生成normal对局-------------------------------------------------------------------------------
		int normalBattleCount = instanceCfg.getNormalBattleCount();
		if (normalBattleCount > 0) {
			// 乱序
			for (Entry<String, Integer> entry : instanceCfg.getNormalBattleMonsterMap().entrySet()) {
				for (int i = entry.getValue(); i > 0; --i) {
					orderedMonsterList.add(entry.getKey());
				}
			}
			HawkRand.randomOrder(orderedMonsterList);
			int battleMonsterCount = orderedMonsterList.size() / normalBattleCount;

			Iterator<String> iter = orderedMonsterList.iterator();
			for (int i = 0; i < normalBattleCount; ++i) {
				HSBattle.Builder battle = HSBattle.newBuilder();
				battle.setBattleCfgId(instanceCfg.getNormalBattleIdList().get(i));

				for (int j = 0; j < battleMonsterCount; ++j) {
					String monsterCfgId = iter.next();
					String instanceMonsterId = instanceId + "_" + monsterCfgId;
					List<ItemInfo> monsterDropList = null;

					InstanceDropCfg dropCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceDropCfg.class, instanceMonsterId);
					if (dropCfg != null) {
						monsterDropList = dropCfg.getReward().getRewardList();
						this.curDropList.addAll(monsterDropList);
					} else {
						monsterDropList = new ArrayList<ItemInfo>();
					}

					battle.addMonsterCfgId(monsterCfgId);
					battle.addMonsterDrop(AwardItems.valueOf(monsterDropList).getBuilder());
				}

				this.curBattleList.add(battle.build());
			}
		}

		// 生成boss对局-----------------------------------------------------------------------------------------
		HSBattle.Builder bossBattle = HSBattle.newBuilder();
		bossBattle.setBattleCfgId(instanceCfg.getBossBattleId());

		// boss对局不乱序
		orderedMonsterList.clear();
		for (Entry<String, Integer> entry : instanceCfg.getBossBattleMonsterMap().entrySet()) {
			for (int i = entry.getValue(); i > 0; --i) {
				orderedMonsterList.add(entry.getKey());
			}
		}

		for (int i = 0; i < orderedMonsterList.size(); ++i) {
			String monsterCfgId = orderedMonsterList.get(i);
			String instanceMonsterId = instanceId + "_" + monsterCfgId;
			List<ItemInfo> monsterDropList = null;

			InstanceDropCfg dropCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceDropCfg.class, instanceMonsterId);
			if (dropCfg != null) {
				monsterDropList = dropCfg.getReward().getRewardList();
				this.curDropList.addAll(monsterDropList);
			} else {
				monsterDropList = new ArrayList<ItemInfo>();
			}

			bossBattle.addMonsterCfgId(monsterCfgId);
			bossBattle.addMonsterDrop(AwardItems.valueOf(monsterDropList).getBuilder());
		}

		this.curBattleList.add(bossBattle.build());

		return true;
	}

	/**
	 * 结算故事副本
	 */
	private void settleStory(int passBattleCount, int deadMonsterCount, int hsCode) {
		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();

		// 胜利
		if (passBattleCount == this.curBattleList.size()) {
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

			// 根据复活次数和死亡宠物次数决定星级
			int starCount = 0;
			if (this.curReviveCount > 0) {
				starCount = 1;
			} else {
				if (deadMonsterCount == 0) {
					starCount = 3;
				} else if (deadMonsterCount == 1) {
					starCount = 2;
				} else {
					starCount = 1;
				}
			}

			// 记录副本进度
			int oldStar = statisticsEntity.getInstanceStar(this.curInstanceId);
			if (oldStar < starCount) {
				statisticsEntity.setInstanceStar(this.curInstanceId, starCount);

				if (true == isChapterFullStar(this.curChapterId, this.curDifficulty)) {
					if (this.curDifficulty == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
						statisticsEntity.setNormalChapterBoxState(this.curChapterId, Const.ChapterBoxState.VALID_VALUE);
					} else if (this.curDifficulty == GsConst.InstanceDifficulty.HARD_INSTANCE) {
						statisticsEntity.setHardChapterBoxState(this.curChapterId, Const.ChapterBoxState.VALID_VALUE);
					}
				}
			}

			if (this.curDifficulty == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
				int oldChapter = statisticsEntity.getNormalTopChapter();
				int oldIndex = statisticsEntity.getNormalTopIndex();
				if (oldChapter < this.curChapterId) {
					statisticsEntity.setNormalTopChapter(this.curChapterId);
					statisticsEntity.setNormalTopIndex(this.curIndex);
				} else if (oldChapter == this.curChapterId && oldIndex < this.curIndex) {
					statisticsEntity.setNormalTopIndex(this.curIndex);
				}

				if (0 == (this.refreshMask & GsConst.RefreshMask.DAILY)) {
					statisticsEntity.increaseInstanceNormalTimesDaily();
				}
				statisticsEntity.increaseInstanceNormalTimes();

			} else if (this.curDifficulty == GsConst.InstanceDifficulty.HARD_INSTANCE) {
				int oldChapter = statisticsEntity.getHardTopChapter();
				int oldIndex = statisticsEntity.getHardTopIndex();
				if (oldChapter < this.curChapterId) {
					statisticsEntity.setHardTopChapter(this.curChapterId);
					statisticsEntity.setHardTopIndex(this.curIndex);
				} else if (oldChapter == this.curChapterId && oldIndex < this.curIndex) {
					statisticsEntity.setHardTopIndex(this.curIndex);
				}

				if (0 == (this.refreshMask & GsConst.RefreshMask.DAILY)) {
					statisticsEntity.increaseInstanceHardTimesDaily();
				}
				statisticsEntity.increaseInstanceHardTimes();
			}

			statisticsEntity.notifyUpdate(true);

			response.setStarCount(starCount);

			// 奖励
			genSettleReward(0, null);
		}

		// 消耗剩余体力，向上取整
		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class,this.curInstanceId);
		int fatigueChange = (int) Math.ceil((double)passBattleCount / this.curBattleList.size() * (entryCfg.getFatigue() - 1));
		if (fatigueChange > 0) {
			ConsumeItems consumeFatigue = ConsumeItems.valueOf();
			consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, fatigueChange);
			consumeFatigue.consumeTakeAffectAndPush(player, Action.NORMAL_INSTANCE_REWARD, hsCode);
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));
	}

	/**
	 * 结算洞
	 */
	private void settleHole(int passBattleCount, int hsCode) {
		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();

		// 胜利
		if (passBattleCount == this.curBattleList.size()) {
			float moreCoinRatio = 0;
			List<ItemInfo> moreItemList = null;

			if (this.curHoleId == GsConst.HoleType.EXP_HOLE) {
				ItemInfo itemInfo = AllianceUtil.getAllianceExp(player);
				if (itemInfo != null) {
					moreItemList = new ArrayList<ItemInfo>();
					moreItemList.add(itemInfo);
				}
			} else if (this.curHoleId == GsConst.HoleType.COIN_HOLE) {
				moreCoinRatio = AllianceUtil.getAllianceCoinRatio(player);
			}

			// 奖励
			genSettleReward(moreCoinRatio, moreItemList);
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));
	}

	/**
	 * 结算塔
	 */
	private void settleTower(int passBattleCount, int hsCode) {
		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();

		// 胜利
		if (passBattleCount == this.curBattleList.size()) {
			if (0 == (this.refreshMask & GsConst.RefreshMask.TOWER)) {
				StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
				statisticsEntity.increaseTowerFloor(this.curTowerId);
				statisticsEntity.notifyUpdate(true);
			}

			// 奖励
			genSettleReward(0, null);
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));
	}

	/**
	 * 结算公会本
	 */
	private void settleGuild(int passBattleCount, int hsCode) {
		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();

		// 胜利
		if (passBattleCount == this.curBattleList.size()) {
			// 公会消息
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_INSTANCE_TASK, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
			msg.pushParam(player);
			msg.pushParam(this.curInstanceId);
			HawkApp.getInstance().postMsg(msg);

			// 奖励
			genSettleReward(0, null);
		}

		// 消耗剩余体力，向上取整
		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class,this.curInstanceId);
		int fatigueChange = (int) Math.ceil((double)passBattleCount / this.curBattleList.size() * (entryCfg.getFatigue() - 1));
		if (fatigueChange > 0) {
			ConsumeItems consumeFatigue = ConsumeItems.valueOf();
			consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, fatigueChange);
			consumeFatigue.consumeTakeAffectAndPush(player, Action.GUILD_INSTANCE_REWARD, hsCode);
		}

		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));
	}

	/**
	 * 发放结算奖励
	 */
	private AwardItems genSettleReward(float moreCoinRatio, List<ItemInfo> moreItemInfoList) {
		AwardItems completeReward = AwardItems.valueOf();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// 多倍怪物经验
		int multiple = 1;
		if (statisticsEntity.getDoubleExpLeft() > 0) {
			multiple = 2;
		} else if (statisticsEntity.getTripleExpLeft() > 0){
			multiple = 3;
		}

		// 通关奖励
		InstanceRewardCfg instanceRewardCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceRewardCfg.class, this.curInstanceId);
		if (instanceRewardCfg != null) {
			this.curDropList.addAll(instanceRewardCfg.getReward().getRewardList());
		}

		// 额外奖励
		if (moreItemInfoList != null) {
			this.curDropList.addAll(moreItemInfoList);
		}

		List<ItemInfo> monsterRewardList = new ArrayList<ItemInfo>();

		Iterator<ItemInfo> iter = this.curDropList.iterator();
		while (iter.hasNext()) {
			ItemInfo itemInfo = iter.next();

			switch (itemInfo.getType()) {
			case itemType.MONSTER_VALUE: {
				monsterRewardList.add(itemInfo);
				break;
			}
			case itemType.MONSTER_ATTR_VALUE: {
				int count = itemInfo.getCount();
				int type = Integer.parseInt(itemInfo.getItemId());

				if (type == changeType.CHANGE_MONSTER_EXP_VALUE) {
					count *= multiple;

					// 把奖励平分给上阵怪物
					if (false == this.curMonsterList.isEmpty()) {
						count /= this.curMonsterList.size();
					}
				}

				for (int monsterId : this.curMonsterList) {
					completeReward.addMonsterAttr(type, count, monsterId);
				}
				break;
			}
			case itemType.PLAYER_ATTR_VALUE: {
				int count = itemInfo.getCount();
				int type = Integer.parseInt(itemInfo.getItemId());

				if (type == changeType.CHANGE_COIN_VALUE) {
					count *= 1 + moreCoinRatio;
				}

				completeReward.addAttr(type, count);
				break;
			}
			default:
				completeReward.addItemInfo(itemInfo);
				break;
			}
		}

		// 怪物奖励最多取一个
		if (false == monsterRewardList.isEmpty()) {
			try {
				int index = HawkRand.randInt(0, monsterRewardList.size() - 1);
				ItemInfo monster = monsterRewardList.get(index);
				MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getItemId());

				completeReward.addMonster(monster.getItemId(), monster.getStage(), 1, 1, monsterCfg.getDisposition());
			} catch (HawkException e) {
				HawkException.catchException(e);
			}
		}

		// 发放掉落奖励和完成奖励
		completeReward.rewardTakeAffectAndPush(player,  Action.NORMAL_INSTANCE_REWARD, HS.code.INSTANCE_SETTLE_C_VALUE);

		// 多倍经验次数
		if (statisticsEntity.getDoubleExpLeft() > 0) {
			statisticsEntity.decreaseDoubleExpLeft(1);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		} else if (statisticsEntity.getTripleExpLeft() > 0) {
			statisticsEntity.decreaseTripleExpLeft(1);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		}

		statisticsEntity.notifyUpdate(true);
		return completeReward;
	}

	/**
	 * 副本数据是否处于初始状态
	 */
	private boolean isCurDataClear() {
		if (this.curType != 0
				|| this.curInstanceId != ""
				|| this.curBattleList.isEmpty() == false
				|| this.curDropList.isEmpty() == false
				|| this.curMonsterList.isEmpty() == false
				|| this.curReviveCount != 0
				|| this.curDifficulty != 0 || this.curChapterId != 0 || this.curIndex != 0
				|| this.curTowerId != 0
				|| this.curHoleId != 0
				|| this.refreshMask != 0) {
			return false;
		}
		return true;
	}

	/**
	 * 清空副本数据
	 */
	private void clearCurData() {
		this.curType = 0;
		this.curInstanceId = "";
		this.curBattleList.clear();
		this.curDropList.clear();
		this.curMonsterList.clear();
		this.curReviveCount = 0;
		this.curDifficulty = 0;
		this.curChapterId = 0;
		this.curIndex = 0;
		this.curHoleId = 0;
		this.curTowerId = 0;
		this.refreshMask = 0;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 清空上次副本数据
		clearCurData();

		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// TODO 如掉线，是否按失败结算待定
		// do nothing
		return true;
	}

	@Override
	public boolean onPlayerRefresh(List<Integer> refreshIndexList, boolean onLogin) {
		for (int index : refreshIndexList) {
			this.refreshMask |= GsConst.PlayerRefreshMask[index];
		}
		return true;
	}
}
