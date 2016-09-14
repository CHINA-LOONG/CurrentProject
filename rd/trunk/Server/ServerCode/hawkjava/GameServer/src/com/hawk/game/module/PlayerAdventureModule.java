package com.hawk.game.module;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.AdventureCfg;
import com.hawk.game.config.AdventureTeamPriceCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.SociatyBaseCfg;
import com.hawk.game.entity.AdventureEntity;
import com.hawk.game.entity.AdventureTeamEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Adventure.HSAdventureBuyConditionRet;
import com.hawk.game.protocol.Adventure.HSAdventureBuyTeamRet;
import com.hawk.game.protocol.Adventure.HSAdventureConditionPush;
import com.hawk.game.protocol.Adventure.HSAdventureEnter;
import com.hawk.game.protocol.Adventure.HSAdventureEnterRet;
import com.hawk.game.protocol.Adventure.HSAdventureNewCondition;
import com.hawk.game.protocol.Adventure.HSAdventureNewConditionRet;
import com.hawk.game.protocol.Adventure.HSAdventureSettle;
import com.hawk.game.protocol.Adventure.HSAdventureSettleRet;
import com.hawk.game.protocol.Alliance.AllianceBaseMonster;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.AdventureUtil;
import com.hawk.game.util.AdventureUtil.AdventureCondition;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;

public class PlayerAdventureModule extends PlayerModule {

	public PlayerAdventureModule(Player player) {
		super(player);
	}

	/**
	 * 玩家升级，刷新大冒险条件
	 */
	@MessageHandler(code = GsConst.MsgType.PLAYER_LEVEL_UP)
	private boolean onPlayerLevelUp(HawkMsg msg) {
		HSAdventureConditionPush.Builder builder = HSAdventureConditionPush.newBuilder();

		List<Integer> busyAdventureList = new ArrayList<Integer> ();

		for (AdventureTeamEntity teamEntity : player.getPlayerData().getAdventureTeamEntityMap().values()) {
			if (0 != teamEntity.getAdventureId()) {
				busyAdventureList.add(teamEntity.getAdventureId());
			}
		}

		for (AdventureEntity advenEntity : player.getPlayerData().getAdventureEntityList()) {
			if (false == busyAdventureList.contains(advenEntity.getAdventureId())) {

				List<AdventureCondition> conditionList = AdventureUtil.genConditionList(player.getLevel());
				if (null == conditionList) {
					advenEntity.clearConditionList();
				} else {
					advenEntity.setConditionList(conditionList);
				}

				AdventureCfg advenCfg = AdventureUtil.getAdventureCfg(advenEntity.getType(), advenEntity.getGear(), player.getLevel());
				advenEntity.setAdventureId(advenCfg.getId());
				advenEntity.notifyUpdate(true);

				builder.addIdleAdventure(BuilderUtil.genAdventureBuilder(advenEntity));
			}
		}

		HawkProtocol protocol = HawkProtocol.valueOf(HS.code.ADVENTURE_CONDITION_PUSH_S, builder);
		player.sendProtocol(protocol);
		return true;
	}

	/**
	 * 开始冒险
	 */
	@ProtocolHandler(code = HS.code.ADVENTURE_ENTER_C_VALUE)
	private boolean onAdventureEnter(HawkProtocol cmd) {
		HSAdventureEnter protocol = cmd.parseProtocol(HSAdventureEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		int teamId = protocol.getTeamId();
		int type = protocol.getType();
		int gear = protocol.getGear();
		List<Integer> selfMonsterList = protocol.getSelfMonsterIdList();
		AllianceBaseMonster hireMonster = null;
		if (true == protocol.hasHireMonster()) {
			hireMonster = protocol.getHireMonster();
		}

		AdventureCfg adventureCfg = AdventureUtil.getAdventureCfg(type, gear, player.getLevel());
		if (null == adventureCfg) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return true;
		}

		// 队伍
		AdventureTeamEntity teamEntity = player.getPlayerData().getAdventureTeamEntity(teamId);
		if (null == teamEntity) {
			sendError(hsCode, Status.adventureError.ADVENTURE_TEAM_NOT_OPEN);
			return true;
		}
		if (0 != teamEntity.getAdventureId()) {
			sendError(hsCode, Status.adventureError.ADVENTURE_TEAM_BUSY);
			return true;
		}

		// 阵型
		int size = selfMonsterList.size();
		if ((size +  ((null == hireMonster) ? 0 : 1)) != GsConst.ADVENTURE_MONSTER_COUNT) {
			sendError(hsCode, Status.adventureError.ADVENTURE_MONSTER_COUNT);
			return true;
		}

		for (int i = 0; i < size; ++i) {
			int monsterId = selfMonsterList.get(i);
			// 去重，通用于arrayList和linkedList优化
			if (i != selfMonsterList.lastIndexOf(monsterId)) {
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return true;
			}

			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (monsterEntity == null) {
				sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE);
				return true;
			}
			if (monsterEntity.isStateSet(Const.MonsterState.IN_ALLIANCE_BASE_VALUE | Const.MonsterState.IN_ADVENTURE_VALUE)) {
				sendError(hsCode, Status.monsterError.MONSTER_BUSY_VALUE);
				return true;
			}
		}

		// 雇佣
		if (null != hireMonster) {
			if (0 == player.getAllianceId()) {
				sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
				return true;
			}

			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			if (true == statisticsEntity.getHireMonsterDailySet().contains(hireMonster.getMonsterId())) {
				sendError(hsCode, Status.adventureError.ADVENTURE_HIRE_ALREADY);
				return true;
			}

			SociatyBaseCfg baseCfg = AllianceUtil.getAllianceBaseConfig(hireMonster.getBp());
			if (null == baseCfg) {
				sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
				return true;
			}

			ConsumeItems consume = ConsumeItems.valueOf();
			consume.addCoin(baseCfg.getCoinHire());
			if (false == consume.checkConsume(player, hsCode)) {
				return true;
			}
			consume.consumeTakeAffectAndPush(player, Action.ADVENTURE_HIRE_MONSTER, hsCode);

			statisticsEntity.addHireMonsterDaily(hireMonster.getMonsterId());
			statisticsEntity.notifyUpdate(true);

	 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ALLIANCE_HIRE_REWARD, HawkXID.valueOf( GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
	 		msg.pushParam(player);
	 		msg.pushParam(hireMonster.getMonsterId());
			HawkApp.getInstance().postMsg(msg);
		}

		int endTime = HawkTime.getSeconds() + GsConst.ADVENTURE_TIME_GEAR[gear - 1];

		teamEntity.setAdventureId(adventureCfg.getId());
		teamEntity.setEndTime(endTime);
		teamEntity.getSelfMonsterList().clear();
		teamEntity.getSelfMonsterList().addAll(selfMonsterList);
		// hireMonster只读，直接使用protobuf的数据即可
		teamEntity.setHireMonster(hireMonster);
		teamEntity.notifyUpdate(true);

		HSAdventureEnterRet.Builder response = HSAdventureEnterRet.newBuilder();
		response.setTeamId(teamId);
		response.setEndTime(endTime);
		sendProtocol(HawkProtocol.valueOf(HS.code.ADVENTURE_ENTER_S, response));
		return true;
	}

	/**
	 * 结束冒险并领奖
	 */
	@ProtocolHandler(code = HS.code.ADVENTURE_SETTLE_C_VALUE)
	private boolean onAdventureSettle(HawkProtocol cmd) {
		HSAdventureSettle protocol = cmd.parseProtocol(HSAdventureSettle.getDefaultInstance());
		int hsCode = cmd.getType();
		int teamId = protocol.getTeamId();
		boolean isPay = protocol.getPay();

		// 队伍
		AdventureTeamEntity teamEntity = player.getPlayerData().getAdventureTeamEntity(teamId);
		if (null == teamEntity) {
			sendError(hsCode, Status.adventureError.ADVENTURE_TEAM_NOT_OPEN);
			return true;
		}
		if (0 == teamEntity.getAdventureId()) {
			sendError(hsCode, Status.adventureError.ADVENTURE_TEAM_IDLE);
			return true;
		}

		// 提前结束
		ConsumeItems consume = null; 
		int curTime = HawkTime.getSeconds();
		if (curTime < teamEntity.getEndTime()) {
			if (false == isPay) {
				sendError(hsCode, Status.adventureError.ADVENTURE_NOT_END);
				return true;
			}

			consume = ConsumeItems.valueOf();
			consume.addGold((int) Math.ceil((teamEntity.getEndTime() - curTime) / 600) * 2);
			if (false == consume.checkConsume(player, hsCode)) {
				return true;
			}
		}

		AdventureCfg advenCfg = HawkConfigManager.getInstance().getConfigByKey(AdventureCfg.class, teamEntity.getAdventureId());
		AdventureEntity advenEntity = player.getPlayerData().getAdventureEntity(advenCfg.getType(), advenCfg.getGear());

		// self monster
		List<MonsterCfg> monsterList = new ArrayList<>(GsConst.ADVENTURE_MONSTER_COUNT);
		for (Integer monsterId : teamEntity.getSelfMonsterList()) {
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (null == monsterEntity) {
				HawkLog.errPrintln(String.format("entity invalid MonsterEntity: %d", monsterId));
			} else {
				MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());
				if (null == monsterCfg) {
					HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", monsterEntity.getCfgId()));
				} else {
					monsterList.add(monsterCfg);
				}
			}
		}

		// hire monster
		if (null != teamEntity.getHireMonster()) {
			MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, teamEntity.getHireMonster().getCfgId());
			if (null == monsterCfg) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", teamEntity.getHireMonster().getCfgId()));
			} else {
				monsterList.add(monsterCfg);
			}
		}

		// reward
		AwardItems basicReward = AwardItems.valueOf();
		AwardItems extraReward = null;
		AwardItems allReward = AwardItems.valueOf();

		if (true == AdventureUtil.isConditionMeet(advenEntity.getConditionList(), monsterList)) {
			extraReward = AwardItems.valueOf();
			List<ItemInfo> rewardList = advenCfg.getExtraReward().getRewardList();
			extraReward.addItemInfos(rewardList);
			allReward.addItemInfos(rewardList);
		}

		List<ItemInfo> rewardList = advenCfg.getBasicReward().getRewardList();
		basicReward.addItemInfos(rewardList);
		allReward.addItemInfos(rewardList);

		if (null != consume) {
			consume.consumeTakeAffectAndPush(player, Action.ADVENTURE_COMPLETE_NOW, hsCode);
		}
		allReward.rewardTakeAffectAndPush(player, Action.ADVENTURE_REWARD, hsCode);

		// clear
		teamEntity.clear();
		teamEntity.notifyUpdate(true);

		// refresh
		List<AdventureCondition> conditionList = AdventureUtil.genConditionList(player.getLevel());
		if (null == conditionList) {
			advenEntity.clearConditionList();
		} else {
			advenEntity.setConditionList(conditionList);
		}
		advenEntity.notifyUpdate(true);

		HSAdventureSettleRet.Builder response = HSAdventureSettleRet.newBuilder();
		response.setTeamId(teamId);
		response.setBasicReward(basicReward.getBuilder());
		if (null != extraReward) {
			response.setExtraReward(extraReward.getBuilder());
		}
		response.setAdventure(BuilderUtil.genAdventureBuilder(advenEntity));
		sendProtocol(HawkProtocol.valueOf(HS.code.ADVENTURE_SETTLE_S, response));
		return true;
	}

	/**
	 * 刷新额外条件
	 */
	@ProtocolHandler(code = HS.code.ADVENTURE_NEW_CONDITION_C_VALUE)
	private boolean onAdventureNewCondition(HawkProtocol cmd) {
		HSAdventureNewCondition protocol = cmd.parseProtocol(HSAdventureNewCondition.getDefaultInstance());
		int hsCode = cmd.getType();
		int type = protocol.getType();
		int gear = protocol.getGear();

		AdventureEntity advenEntity = player.getPlayerData().getAdventureEntity(type, gear);
		if (null == advenEntity) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		if (0 >= player.regainAdventureChangeTimes()) {
			sendError(hsCode, Status.adventureError.ADVENTURE_CHANGE_ZERO);
			return true;
		}

		List<AdventureCondition> conditionList = AdventureUtil.genConditionList(player.getLevel());
		if (null == conditionList) {
			advenEntity.clearConditionList();
		} else {
			advenEntity.setConditionList(conditionList);
		}
		advenEntity.notifyUpdate(true);

		player.consumeAdventureChangeTimes(1, Action.NULL);

		HSAdventureNewConditionRet.Builder response = HSAdventureNewConditionRet.newBuilder();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		response.setAdventure(BuilderUtil.genAdventureBuilder(advenEntity));
		response.setChangeCount(statisticsEntity.getAdventureChange());
		response.setChangeCountBeginTime((int)(statisticsEntity.getAdventureChangeBeginTime().getTimeInMillis() / 1000));
		sendProtocol(HawkProtocol.valueOf(HS.code.ADVENTURE_NEW_CONDITION_S, response));
		return true;
	}

	/**
	 * 购买刷新额外条件次数
	 */
	@ProtocolHandler(code = HS.code.ADVENTURE_BUY_CONDITION_C_VALUE)
	private boolean onAdventureBuyCondition(HawkProtocol cmd) {
		int hsCode = cmd.getType();

		if (0 != player.regainAdventureChangeTimes()) {
			sendError(hsCode, Status.adventureError.ADVENTURE_CHANGE_NOT_ZERO);
			return true;
		}

		ConsumeItems consume = ConsumeItems.valueOf();
		consume.addGold(GsConst.ADVENTURE_CHANGE_BUY_CONSUME);
		if (false == consume.checkConsume(player, hsCode)) {
			return true;
		}

		consume.consumeTakeAffectAndPush(player, Action.ADVENTURE_BUY_REFRESH, hsCode);
		player.increaseAdventureChangeTimes(1, Action.ADVENTURE_BUY_REFRESH);

		HSAdventureBuyConditionRet.Builder response = HSAdventureBuyConditionRet.newBuilder();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		response.setChangeCount(statisticsEntity.getAdventureChange());
		response.setChangeCountBeginTime((int)(statisticsEntity.getAdventureChangeBeginTime().getTimeInMillis() / 1000));
		sendProtocol(HawkProtocol.valueOf(HS.code.ADVENTURE_NEW_CONDITION_S, response));
		return true;
	}

	/**
	 * 购买队伍
	 */
	@ProtocolHandler(code = HS.code.ADVENTURE_BUY_TEAM_C_VALUE)
	private boolean onAdventureBuyTeam(HawkProtocol cmd) {
		int hsCode = cmd.getType();

		// teamId 从1开始
		Map<Object, AdventureTeamPriceCfg> priceCfgMap = HawkConfigManager.getInstance().getConfigMap(AdventureTeamPriceCfg.class);

		int teamCount = player.getPlayerData().getAdventureTeamEntityMap().size();
		int newTeamId = teamCount + 1;

		if (newTeamId > priceCfgMap.size()) {
			sendError(hsCode, Status.adventureError.ADVENTURE_TEAM_COUNT);
			return true;
		}

		ConsumeItems consume = ConsumeItems.valueOf();
		consume.addGold(priceCfgMap.get(newTeamId).getGold());
		if (false == consume.checkConsume(player, hsCode)) {
			return true;
		}

		AdventureTeamEntity teamEntity = new AdventureTeamEntity(player.getId(), newTeamId);
		if (false == teamEntity.notifyCreate()) {
			sendError(hsCode, Status.error.DATA_BASE_ERROR_VALUE);
			return true;
		}

		player.getPlayerData().addAdventureTeamEntity(teamEntity);
		consume.consumeTakeAffectAndPush(player, Action.ADVENTURE_ADD_TEAM, hsCode);

		HSAdventureBuyTeamRet.Builder response = HSAdventureBuyTeamRet.newBuilder();
		response.setTeamId(newTeamId);
		sendProtocol(HawkProtocol.valueOf(HS.code.ADVENTURE_BUY_TEAM_S, response));
		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 加载所有大冒险数据
		player.getPlayerData().loadAllAdventure();
		player.getPlayerData().loadAllAdventureTeam();

		// 同步大冒险信息
		player.getPlayerData().syncAdventureInfo();

		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// 重要数据下线就存储
		for (AdventureEntity entity : player.getPlayerData().getAdventureEntityList()) {
			entity.notifyUpdate(false);
		}
		for (AdventureTeamEntity entity : player.getPlayerData().getAdventureTeamEntityMap().values()) {
			entity.notifyUpdate(false);
		}
		return true;
	}
}
