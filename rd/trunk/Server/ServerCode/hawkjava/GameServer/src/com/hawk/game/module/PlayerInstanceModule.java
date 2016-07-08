package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkRand;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.InstanceCfg;
import com.hawk.game.config.InstanceDropCfg;
import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.config.InstanceRewardCfg;
import com.hawk.game.config.RewardGroupCfg;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Instance.HSBattle;
import com.hawk.game.protocol.Instance.HSInstanceAssist;
import com.hawk.game.protocol.Instance.HSInstanceAssistRet;
import com.hawk.game.protocol.Instance.HSInstanceEnter;
import com.hawk.game.protocol.Instance.HSInstanceEnterRet;
import com.hawk.game.protocol.Instance.HSInstanceOpenCard;
import com.hawk.game.protocol.Instance.HSInstanceOpenCardRet;
import com.hawk.game.protocol.Instance.HSInstanceResetCount;
import com.hawk.game.protocol.Instance.HSInstanceResetCountRet;
import com.hawk.game.protocol.Instance.HSInstanceSettle;
import com.hawk.game.protocol.Instance.HSInstanceSettleRet;
import com.hawk.game.protocol.Instance.HSInstanceSweep;
import com.hawk.game.protocol.Instance.HSInstanceSweepRet;
import com.hawk.game.protocol.Reward.RewardItem;
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
	// 本次对局掉落列表
	private List<List<ItemInfo>> curBattleDropList;
	// 本次结算星级奖牌列表
	private List<ItemInfo> curCardRewardList;

	public PlayerInstanceModule(Player player) {
		super(player);

		curInstanceId = "";
		curBattleList = new ArrayList<HSBattle>();
		curBattleDropList = new ArrayList<List<ItemInfo>>();
		curCardRewardList = new ArrayList<ItemInfo>();
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
		if (true == protocol.hasFriendId()) {
			int friendId = protocol.getFriendId();
		}

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return false;
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
				return false;
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
				return false;
			}
		}

		// 副本等级
		if (player.getLevel() < entryCfg.getLevel()) {
			sendError(hsCode, Status.instanceError.INSTANCE_LEVEL);
			return false;
		}

		// 次数
		if (statisticsEntity.getInstanceCountDaily(instanceId) >= entryCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return false;
		}

		// 体力
		if (statisticsEntity.getFatigue() < entryCfg.getFatigue()) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return false;
		}

		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, instanceId);
		if (instanceCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return false;
		}

		this.curInstanceId = instanceId;
		if (false == this.curBattleList.isEmpty() || false == this.curBattleDropList.isEmpty() || false == this.curCardRewardList.isEmpty()) {
			logger.error("instance data is not empty when enter instance");
			this.curBattleList.clear();
			this.curBattleDropList.clear();
			this.curCardRewardList.clear();
		}

		// 生成对局
		// normal
		List<String> randMonsterList = new ArrayList<String>();

		// 乱序
		for (Entry<String, Integer> entry : instanceCfg.getNormalBattleMonsterMap().entrySet()) {
			for (int i = entry.getValue(); i > 0; --i) {
				randMonsterList.add(entry.getKey());
			}
		}
		HawkRand.randomOrder(randMonsterList);
		int battleMonsterCount = randMonsterList.size() / instanceCfg.getNormalBattleCount();

		Iterator<String> iter = randMonsterList.iterator();
		for (int i = 0; i < instanceCfg.getNormalBattleCount(); ++i) {
			HSBattle.Builder battle = HSBattle.newBuilder();
			battle.setBattleCfgId(instanceCfg.getNormalBattleIdList().get(i));
			List<ItemInfo> battleDropList = new ArrayList<ItemInfo>();

			for (int j = 0; j < battleMonsterCount; ++j) {
				String monsterCfgId = iter.next();
				String instanceMonsterId = instanceId + "_" + monsterCfgId;
				List<ItemInfo> monsterDropList = null;

				InstanceDropCfg dropCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceDropCfg.class, instanceMonsterId);
				if (dropCfg != null) {
					monsterDropList = dropCfg.getReward().getRewardList();
					battleDropList.addAll(monsterDropList);
				} else {
					monsterDropList = new ArrayList<ItemInfo>();
				}

				battle.addMonsterCfgId(monsterCfgId);
				battle.addMonsterDrop(AwardItems.valueOf(monsterDropList).getBuilder());
			}

			this.curBattleList.add(battle.build());
			this.curBattleDropList.add(battleDropList);
		}

		// boss
		HSBattle.Builder bossBattle = HSBattle.newBuilder();
		bossBattle.setBattleCfgId(instanceCfg.getBossBattleId());

		// 乱序
		randMonsterList.clear();
		for (Entry<String, Integer> entry : instanceCfg.getBossBattleMonsterMap().entrySet()) {
			for (int i = entry.getValue(); i > 0; --i) {
				randMonsterList.add(entry.getKey());
			}
		}
		HawkRand.randomOrder(randMonsterList);

		iter = randMonsterList.iterator();
		List<ItemInfo> battleDropList = new ArrayList<ItemInfo>();
		for (int i = 0; i < randMonsterList.size(); ++i) {
			String monsterCfgId = randMonsterList.get(i);
			String instanceMonsterId = instanceId + "_" + monsterCfgId;
			List<ItemInfo> monsterDropList = null;
			
			InstanceDropCfg dropCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceDropCfg.class, instanceMonsterId);
			if (dropCfg != null) {
				monsterDropList = dropCfg.getReward().getRewardList();
				battleDropList.addAll(monsterDropList);
			} else {
				monsterDropList = new ArrayList<ItemInfo>();
			}

			bossBattle.addMonsterCfgId(monsterCfgId);
			bossBattle.addMonsterDrop(AwardItems.valueOf(monsterDropList).getBuilder());
		}
		
		this.curBattleList.add(bossBattle.build());
		this.curBattleDropList.add(battleDropList);

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
		List<Integer> passBattleList = protocol.getPassBattleIndexList();
		List<Integer> passBoxList = protocol.getPassBoxIndexList();

		AwardItems dropCompleteReward = AwardItems.valueOf();
		AwardItems completeReward = AwardItems.valueOf();
		List<RewardItem> cardList = new ArrayList<RewardItem>();

		boolean complete = false;
		int starCount = 0;

		// 验证
		for (Integer i : passBattleList) {
			if (i < 0 || i > this.curBattleList.size()) {
				sendError(hsCode, Status.error.PARAMS_INVALID);
				return false;
			}
			HSBattle battle = this.curBattleList.get(i);
			//if (BattleType.BOSS == battle.getType()) {
				complete = true;
				// TODO: 评价
				starCount = 3;
			//}

			// 掉落奖励
			dropCompleteReward.addItemInfos(this.curBattleDropList.get(i));
		}

		if (true == complete) {
			// 通关奖励
			InstanceRewardCfg instanceRewardCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceRewardCfg.class, this.curInstanceId);
			if (instanceRewardCfg != null) {
				List<ItemInfo> list = instanceRewardCfg.getReward().getRewardList();
				dropCompleteReward.addItemInfos(list);
				completeReward.addItemInfos(list);

				// 生成翻牌奖励
				RewardGroupCfg starRewardCfg = instanceRewardCfg.getStarRewardGroup(starCount);
				for (int i = 0; i  < GsConst.INSTANCE_CARD_COUNT; ++i) {
					ItemInfo cardReward = starRewardCfg.getRewardItem();
					if (cardReward.getType() != itemType.NONE_ITEM_VALUE) {
						this.curCardRewardList.add(cardReward);

						AwardItems convertor = AwardItems.valueOf();
						convertor.addItemInfo(cardReward);
						cardList.add(convertor.getBuilder().getRewardItemsList().get(0));
					}
				}
			}

			// 记录副本进度
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			int oldStar = statisticsEntity.getInstanceStar(this.curInstanceId);
			if (starCount > oldStar) {
				statisticsEntity.setInstanceStar(this.curInstanceId, starCount);
			}

			statisticsEntity.addInstanceAllCount();
			statisticsEntity.addInstanceAllCountDaily();

			InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, this.curInstanceId);
			if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
				statisticsEntity.addHardCount();
				statisticsEntity.addHardCountDaily();
			}

			statisticsEntity.notifyUpdate(true);
		}

		// TODO: 体力扣除

		// 发放掉落奖励和完成奖励
		dropCompleteReward.rewardTakeAffectAndPush(player,  Action.INSTACE_SETTLE);

		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();
		if (true == complete) {
			response.setStarCount(starCount);
			response.setCompleteReward(completeReward.getBuilder());
			response.addAllCardReward(cardList);
		}
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));

		// 清空副本数据
		this.curBattleList.clear();
		this.curBattleDropList.clear();

		return true;
	}

	/**
	 * 翻牌
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_OPEN_CARD_C_VALUE)
	private boolean onInstanceOpenCard(HawkProtocol cmd) {
		HSInstanceOpenCard protocol = cmd.parseProtocol(HSInstanceOpenCard.getDefaultInstance());
		int hsCode = cmd.getType();
		int openCount = protocol.getOpenCount();

		if (openCount > this.curCardRewardList.size()) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return false;
		}

		ConsumeItems cardConsume = ConsumeItems.valueOf();
		AwardItems cardReward = AwardItems.valueOf();

		for (int i = 0; i < openCount; ++i) {
			cardReward.addItemInfo(this.curCardRewardList.get(i));
		}
		// TODO: 消耗

		cardReward.rewardTakeAffectAndPush(player,  Action.INSTACE_SETTLE);

		HSInstanceOpenCardRet.Builder response = HSInstanceOpenCardRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_OPEN_CARD_S, response));

		// 清空翻牌数据
		this.curInstanceId = "";
		this.curCardRewardList.clear();

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
			return false;
		}

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return false;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// 次数
		if (statisticsEntity.getInstanceCountDaily(instanceId) + count > entryCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return false;
		}

		// 体力
		int fatigueChange = count * entryCfg.getFatigue();
		if (statisticsEntity.getFatigue() < fatigueChange) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return false;
		}

		// 扫荡券
		ConsumeItems consume = ConsumeItems.valueOf();
		consume.addItem(GsConst.SWEEP_TICKET, count);
		if (false == consume.checkConsume(player, hsCode)) {
			return false;
		}
		consume.consumeTakeAffectAndPush(player, Action.INSTANCE_SWEEP);

		// 奖励
		List<AwardItems> completeRewardList = new ArrayList<AwardItems>();
		AwardItems sweepReward = AwardItems.valueOf();

		InstanceRewardCfg instanceRewardCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceRewardCfg.class, instanceId);
		if (instanceRewardCfg != null) {
			for (int i = 0; i < count; ++i) {
				List<ItemInfo> list = instanceRewardCfg.getReward().getRewardList();
				AwardItems completeReward = AwardItems.valueOf();
				completeReward.addItemInfos(list);
				completeRewardList.add(completeReward);
			}

			List<ItemInfo> list = instanceRewardCfg.getSweepReward().getRewardList();
			sweepReward.addItemInfos(list);
		}

		// 体力和次数修改
		statisticsEntity.setFatigue(statisticsEntity.getFatigue() - fatigueChange);
		statisticsEntity.addInstanceCountDaily(instanceId, count);
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
			return false;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// TODO
		statisticsEntity.setInstanceCountDaily(instanceId, 0);
		statisticsEntity.notifyUpdate(true);

		HSInstanceResetCountRet.Builder response = HSInstanceResetCountRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_RESET_COUNT_S, response));

		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 清空上次副本数据
		this.curInstanceId = "";
		this.curBattleList.clear();
		this.curBattleDropList.clear();
		this.curCardRewardList.clear();

		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// do nothing
		return true;
	}
}
