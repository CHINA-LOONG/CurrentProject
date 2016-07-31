package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.InstanceCfg;
import com.hawk.game.config.InstanceDropCfg;
import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.config.InstanceRewardCfg;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.Const.toolType;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Instance.HSBattle;
import com.hawk.game.protocol.Instance.HSInstanceAssist;
import com.hawk.game.protocol.Instance.HSInstanceAssistRet;
import com.hawk.game.protocol.Instance.HSInstanceEnter;
import com.hawk.game.protocol.Instance.HSInstanceEnterRet;
import com.hawk.game.protocol.Instance.HSInstanceResetCount;
import com.hawk.game.protocol.Instance.HSInstanceResetCountRet;
import com.hawk.game.protocol.Instance.HSInstanceReviveRet;
import com.hawk.game.protocol.Instance.HSInstanceSettle;
import com.hawk.game.protocol.Instance.HSInstanceSettleRet;
import com.hawk.game.protocol.Instance.HSInstanceSweep;
import com.hawk.game.protocol.Instance.HSInstanceSweepRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.InstanceUtil;
import com.hawk.game.util.InstanceUtil.InstanceChapter;

public class PlayerInstanceModule extends PlayerModule {

	private static final Logger logger = LoggerFactory.getLogger("Protocol");

	// 本次副本Id
	private String curInstanceId;
	// 本次对局列表
	private List<HSBattle> curBattleList;
	// 本次副本掉落列表
	private List<ItemInfo> curDropList;
	// 本次副本复活次数
	private int curReviveCount;

	public PlayerInstanceModule(Player player) {
		super(player);

		curInstanceId = "";
		curBattleList = new ArrayList<HSBattle>();
		curDropList = new ArrayList<ItemInfo>();
		curReviveCount = 0;
	}

	/**
	 * 获取助战列表
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_ASSIST_C_VALUE)
	private boolean onInstanceAssist(HawkProtocol cmd) {
		HSInstanceAssist protocol = cmd.parseProtocol(HSInstanceAssist.getDefaultInstance());
		int hsCode = cmd.getType();

		// TODO

		HSInstanceAssistRet.Builder response = HSInstanceAssistRet.newBuilder();
		//response.addAllAssist(values);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ASSIST_S, response));
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
			int friendId = protocol.getFriendId();
		}

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		int chapterId = entryCfg.getChapter();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		Map<Integer, InstanceChapter> chapterMap = InstanceUtil.getInstanceChapterMap();
		InstanceChapter chapter = chapterMap.get(chapterId);

		// 章节已开启，前置副本完成
		// 精英副本，必须通关普通章节
		if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
			int index = chapter.normalList.indexOf(entryCfg);
			int size = chapter.normalList.size();
			int curChapterId = statisticsEntity.getNormalInstanceChapter();
			int curIndex = statisticsEntity.getNormalInstanceIndex();

			if ((chapterId > curChapterId && (index != size - 1 || chapterId > curChapterId + 1)) ||
					(chapterId == curChapterId && index > curIndex + 1)) {
				sendError(hsCode, Status.instanceError.INSTANCE_NOT_OPEN);
				return true;
			}
		} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
			int normalSize = chapter.normalList.size();
			int normalCurChapterId = statisticsEntity.getNormalInstanceChapter();
			int normalCurIndex = statisticsEntity.getNormalInstanceIndex();
			int hardIndex = chapter.hardList.indexOf(entryCfg);
			int hardSize = chapter.hardList.size();
			int hardCurChapterId = statisticsEntity.getHardInstanceChapter();
			int hardCurIndex = statisticsEntity.getHardInstanceIndex();

			if ((chapterId > normalCurChapterId) ||
					(chapterId == normalCurChapterId && normalCurIndex != normalSize - 1) ||
					(chapterId > hardCurChapterId && (hardIndex != hardSize - 1 || chapterId > hardCurChapterId + 1)) ||
					(chapterId == hardCurChapterId && hardIndex > hardCurIndex + 1)) {
				sendError(hsCode, Status.instanceError.INSTANCE_NOT_OPEN);
				return true;
			}
		}

		// 副本等级
		if (player.getLevel() < entryCfg.getLevel()) {
			sendError(hsCode, Status.instanceError.INSTANCE_LEVEL);
			return true;
		}

		// 次数
		if (statisticsEntity.getInstanceCountDaily(instanceId) >= entryCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return true;
		}

		// 体力
		if (statisticsEntity.getFatigue() < entryCfg.getFatigue()) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return true;
		}

		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, instanceId);
		if (instanceCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return true;
		}

		// 阵型
		if (battleMonsterList.size() == 0 || battleMonsterList.size() > GsConst.MAX_BATTLE_MONSTER_COUNT) {
			sendError(hsCode, Status.monsterError.BATTLE_MONSTER_COUNT);
			return true;
		}

		List<Integer> newMonsterList = new LinkedList<Integer>();
		for(Integer monsterId : battleMonsterList) {
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (monsterEntity == null) {
				sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST);
				return true;
			}
			newMonsterList.add(monsterId);
		}

		player.getEntity().setBattleMonsterList(newMonsterList);
		player.getEntity().notifyUpdate(true);

		// 满足条件，进入副本，生成副本数据
		if (this.curInstanceId != ""
				|| this.curBattleList.isEmpty() == false
				|| this.curDropList.isEmpty() == false
				|| this.curReviveCount != 0) {
			logger.error("instance data is not empty when enter instance");
			clearCurData();
		}
		this.curInstanceId = instanceId;

		// 生成对局
		// normal
		List<String> orderedMonsterList = new ArrayList<String>();

		// 乱序
		for (Entry<String, Integer> entry : instanceCfg.getNormalBattleMonsterMap().entrySet()) {
			for (int i = entry.getValue(); i > 0; --i) {
				orderedMonsterList.add(entry.getKey());
			}
		}
		HawkRand.randomOrder(orderedMonsterList);
		int battleMonsterCount = orderedMonsterList.size() / instanceCfg.getNormalBattleCount();

		Iterator<String> iter = orderedMonsterList.iterator();
		for (int i = 0; i < instanceCfg.getNormalBattleCount(); ++i) {
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

		// boss
		HSBattle.Builder bossBattle = HSBattle.newBuilder();
		bossBattle.setBattleCfgId(instanceCfg.getBossBattleId());

		// boss对局不乱序
		orderedMonsterList.clear();
		for (Entry<String, Integer> entry : instanceCfg.getBossBattleMonsterMap().entrySet()) {
			for (int i = entry.getValue(); i > 0; --i) {
				orderedMonsterList.add(entry.getKey());
			}
		}

		iter = orderedMonsterList.iterator();
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

		// 体力和次数修改
		int fatigueChange = entryCfg.getFatigue();
		statisticsEntity.setFatigue(statisticsEntity.getFatigue() - fatigueChange);
		statisticsEntity.addInstanceCountDaily(instanceId, 1);
		statisticsEntity.notifyUpdate(true);

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
		boolean victory = protocol.getVictory();

		AwardItems completeReward = AwardItems.valueOf();

		int starCount = 3;

		if (true == victory) {
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			// 多倍怪物经验
			int multiple = 1;
			if (statisticsEntity.getDoubleExpLeftTimes() > 0) {
				multiple = 2;
			}
			else if (statisticsEntity.getTripleExpLeftTimes() > 0){
				multiple = 3;
			}

			// 通关奖励
			InstanceRewardCfg instanceRewardCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceRewardCfg.class, this.curInstanceId);
			if (instanceRewardCfg != null) {
				this.curDropList.addAll(instanceRewardCfg.getReward().getRewardList());
			}

			List<ItemInfo> monsterRewardList = new ArrayList<ItemInfo>();

			Iterator<ItemInfo> iter = this.curDropList.iterator();
			while (iter.hasNext()) {
				ItemInfo itemInfo = iter.next();
				if (itemInfo.getType() == Const.itemType.MONSTER_VALUE) {
					monsterRewardList.add(itemInfo);
				} else if (itemInfo.getType() == itemType.MONSTER_ATTR_VALUE
						&& Integer.parseInt(itemInfo.getItemId()) == changeType.CHANGE_MONSTER_EXP_VALUE) {
					completeReward.addMonsterAttr(changeType.CHANGE_MONSTER_EXP_VALUE, itemInfo.getCount() * multiple);
				} else {
					completeReward.addItemInfo(itemInfo);
				}
			}

			// 怪物奖励最多取一个
			if (false == monsterRewardList.isEmpty()) {
				try {
					int index = HawkRand.randInt(0, monsterRewardList.size() - 1);
					ItemInfo monster = monsterRewardList.get(index);
					int disposition = HawkRand.randInt(0, 5);
					completeReward.addMonster(monster.getItemId(), monster.getStage(), 1, 5, disposition);
				} catch (HawkException e) {
					HawkException.catchException(e);
				}		
			}

			// 发放掉落奖励和完成奖励
			completeReward.rewardTakeAffectAndPush(player,  Action.INSTACE_SETTLE, HS.code.INSTANCE_SETTLE_C_VALUE);

			// 记录副本进度
			int oldStar = statisticsEntity.getInstanceStar(this.curInstanceId);
			if (starCount > oldStar) {
				statisticsEntity.setInstanceStar(this.curInstanceId, starCount);
			}
			statisticsEntity.addInstanceAllCount();
			statisticsEntity.addInstanceAllCountDaily();
			if (statisticsEntity.getDoubleExpLeftTimes() > 0) {
				statisticsEntity.decreaseDoubleExpLeft(1);
				player.getPlayerData().syncStatisticsExpLeftInfo();
			}
			else if (statisticsEntity.getDoubleExpLeftTimes() > 0) {
				statisticsEntity.decreaseTripleExpLeft(1);
				player.getPlayerData().syncStatisticsExpLeftInfo();
			}

			InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, this.curInstanceId);
			if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
				statisticsEntity.addHardCount();
				statisticsEntity.addHardCountDaily();
			}

			statisticsEntity.notifyUpdate(true);
		}

		// TODO: 体力扣除

		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();
		if (true == victory) {
			response.setStarCount(starCount);
			response.setReward(completeReward.getBuilder());
		}
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));

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

		// 次数
		if (statisticsEntity.getInstanceCountDaily(instanceId) + count > entryCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return true;
		}

		// 体力
		int fatigueChange = count * entryCfg.getFatigue();
		ConsumeItems consumeFatigue = ConsumeItems.valueOf();
		consumeFatigue.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, fatigueChange);
		if (consumeFatigue.checkConsume(player, hsCode)) {
			return true;
		}

		// 扫荡券
		ConsumeItems consumeItem = ConsumeItems.valueOf();
		consumeItem.addItem(GsConst.SWEEP_TICKET, count);
		if (false == consumeItem.checkConsume(player, hsCode)) {
			return true;
		}
		consumeItem.consumeTakeAffectAndPush(player, Action.INSTANCE_SWEEP, HS.code.INSTANCE_SWEEP_C_VALUE);

		// 奖励
		List<AwardItems> completeRewardList = new ArrayList<AwardItems>();
		AwardItems sweepReward = AwardItems.valueOf();

		InstanceRewardCfg instanceRewardCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceRewardCfg.class, instanceId);
		if (instanceRewardCfg != null) {
			// 多倍怪物经验，给多倍经验药水
			int multiple = 1;
			if (statisticsEntity.getDoubleExpLeftTimes() > 0) {
				multiple = 2;
			}
			else if (statisticsEntity.getTripleExpLeftTimes() > 0){
				multiple = 3;
			}

			for (int i = 0; i < count; ++i) {
				AwardItems completeReward = AwardItems.valueOf();

				for (ItemInfo itemInfo : instanceRewardCfg.getReward().getRewardList()) {
					ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemInfo.getItemId());
					if (itemCfg != null
							&& itemCfg.getType() == toolType.USETOOL_VALUE
							&& itemCfg.getAddAttrType() == changeType.CHANGE_MONSTER_EXP_VALUE) {
						completeReward.addItem(itemInfo.getItemId(), itemInfo.getCount() * multiple);
					} else {
						completeReward.addItemInfo(itemInfo);
					}
				}

				completeRewardList.add(completeReward);
			}

			for (ItemInfo itemInfo : instanceRewardCfg.getSweepReward().getRewardList()) {
				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemInfo.getItemId());
				if (itemCfg != null
						&& itemCfg.getType() == toolType.USETOOL_VALUE
						&& itemCfg.getAddAttrType() == changeType.CHANGE_MONSTER_EXP_VALUE) {
					sweepReward.addItem(itemInfo.getItemId(), itemInfo.getCount() * multiple);
				} else {
					sweepReward.addItemInfo(itemInfo);
				}
			}
		}

		// 体力和次数修改
		consumeFatigue.consumeTakeAffectAndPush(player, Action.INSTANCE_SWEEP, HS.code.INSTANCE_SWEEP_C_VALUE);
		statisticsEntity.addInstanceCountDaily(instanceId, count);
		if (statisticsEntity.getDoubleExpLeftTimes() > 0) {
			statisticsEntity.decreaseDoubleExpLeft(1);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		}
		else if (statisticsEntity.getDoubleExpLeftTimes() > 0) {
			statisticsEntity.decreaseTripleExpLeft(1);
			player.getPlayerData().syncStatisticsExpLeftInfo();
		}

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

		// TODO
		statisticsEntity.setInstanceCountDaily(instanceId, 0);
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
		consume.consumeTakeAffectAndPush(player, Action.INSTANCE_REVIVE, HS.code.INSTANCE_REVIVE_C_VALUE);

		this.curReviveCount = reviveCount;

		HSInstanceReviveRet.Builder response = HSInstanceReviveRet.newBuilder();
		response.setReviveCount(this.curReviveCount);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_REVIVE_S, response));

		return true;
	}

	/**
	 * 清空副本数据
	 */
	private void clearCurData() {
		this.curInstanceId = "";
		this.curBattleList.clear();
		this.curDropList.clear();
		this.curReviveCount = 0;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 清空上次副本数据
		clearCurData();

		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// do nothing
		return true;
	}
}
