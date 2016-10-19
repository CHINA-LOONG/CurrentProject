package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.BILog.BIMissionFlowData;
import com.hawk.game.config.QuestCfg;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BILogger;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.Quest.HSQuestAccept;
import com.hawk.game.protocol.Quest.HSQuestRemove;
import com.hawk.game.protocol.Quest.HSQuestSubmit;
import com.hawk.game.protocol.Quest.HSQuestSubmitRet;
import com.hawk.game.protocol.Quest.HSQuestUpdate;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ProtoUtil;
import com.hawk.game.util.QuestUtil;
import com.hawk.game.util.ShopUtil;
import com.hawk.game.util.QuestUtil.QuestGroup;
import com.hawk.game.util.TimePointUtil;

/**
 * 任务模块
 * 
 * @author walker
 */
public class PlayerQuestModule extends PlayerModule {
	// 未激活的任务组
	private Map<Integer, QuestGroup> inactiveQuestGroupMap = new HashMap<Integer, QuestGroup>();

	// 上一次更新任务进度时任务接取条件记录
	// 接取条件：等级
	private int lastPlayerLevel = 0;

	public PlayerQuestModule(Player player) {
		super(player);
	}

	/**
	 * 任务相关数据有更新
	 */
	@MessageHandler(code = GsConst.MsgType.STATISTICS_UPDATE)
	private boolean onStatisticsUpdate(HawkMsg msg) {
		// 更新任务进度，通知客户端
		List<HSQuest> updateQuestList = new ArrayList<HSQuest>();
		Map<Integer, HSQuest> questMap = player.getPlayerData().getQuestMap();
		for (Entry<Integer, HSQuest> entry : questMap.entrySet()) {
			int newProgress = getQuestProgress(entry.getValue().getQuestId());
			if (newProgress != entry.getValue().getProgress()) {
				HSQuest.Builder quest = entry.getValue().toBuilder();
				quest.setProgress(newProgress);
				entry.setValue(quest.build());

				updateQuestList.add(entry.getValue());

				QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, quest.getQuestId());
				BILogger.getBIData(BIMissionFlowData.class).log(player, Action.MISSION_ONGOING, questCfg.getType(), questCfg.getId(), questCfg.getNameId(), (float)newProgress / questCfg.getGoalCount());
			}
		}
		if (false == updateQuestList.isEmpty()) {
			HSQuestUpdate.Builder updateBuilder = HSQuestUpdate.newBuilder();
			updateBuilder.addAllQuest(updateQuestList);
			sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_UPDATE_S, updateBuilder));
		}

		// 升级，检查未激活任务组，新接任务通知客户端
		if (player.getLevel() > lastPlayerLevel) {
			lastPlayerLevel = player.getLevel();

			List<HSQuest> acceptQuestList = loadQuest(inactiveQuestGroupMap);
			if (false == acceptQuestList.isEmpty()) {
				HSQuestAccept.Builder acceptBuilder = HSQuestAccept.newBuilder();
				acceptBuilder.addAllQuest(acceptQuestList);
				sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_ACCEPT_S, acceptBuilder));
			}
		}
		return true;
	}

	/**
	 * 任务交付
	 */
	@ProtocolHandler(code = HS.code.QUEST_SUBMIT_C_VALUE)
	private boolean onQuestSubmit(HawkProtocol cmd) {
		Calendar curTime = HawkTime.getCalendar();
		HSQuestSubmit protocol = cmd.parseProtocol(HSQuestSubmit.getDefaultInstance());
		int hsCode = cmd.getType();
		int questId = protocol.getQuestId();
		QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);

		// 验证
		Map<Integer, HSQuest> questMap = player.getPlayerData().getQuestMap();
		HSQuest quest = questMap.get(questId);
		if (null == quest) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.questError.QUEST_NOT_ACCEPT_VALUE, 1));
			return true;
		}

		if (quest.getProgress() < questCfg.getGoalCount()) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.questError.QUEST_NOT_COMPLETE_VALUE, 1));
			return true;
		}

		if (null != questCfg.getTimeBegin() && null != questCfg.getTimeEnd()) {
			if (false == TimePointUtil.isTimeInPeriod(curTime, questCfg.getTimeBegin().getType(), questCfg.getTimeEnd().getType())) {
				sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.questError.QUEST_NOT_OPEN_VALUE, 1));
				return true;
			}
		}

		// 发奖
		int exp = questCfg.getExpReward(player.getLevel());
		List<ItemInfo> list = questCfg.getReward().getRewardList();
		AwardItems awardItems = AwardItems.valueOf();
		awardItems.addExp(exp);
		awardItems.addItemInfos(list);
		awardItems.rewardTakeAffectAndPush(player, Action.MISSION_REWARD, HS.code.QUEST_SUBMIT_C_VALUE);

		// 记录
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		statisticsEntity.addQuestComplete(questId);
		statisticsEntity.notifyUpdate(true);

		questMap.remove(questId);

		HSQuestSubmitRet.Builder response = HSQuestSubmitRet.newBuilder();
		response.setQuestId(questId);
		sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_SUBMIT_S, response));

		// 如果不是时效性任务组，接取下一个
		if (null == questCfg.getTimeBegin()) {
			QuestCfg nextQuestCfg = QuestUtil.getNextQuest(questId);
			if (null != nextQuestCfg) {
				List<HSQuest> acceptList = new ArrayList<HSQuest>();
				boolean active = acceptQuest(nextQuestCfg, acceptList);
				if (false == active) {
					// 任务无法接取，将任务组设为未激活
					QuestGroup group = QuestUtil.getQuestGroupByQuest(questId);
					inactiveQuestGroupMap.put(group.groupId, group);
				} else {
					HSQuestAccept.Builder acceptBuilder = HSQuestAccept.newBuilder();
					acceptBuilder.addAllQuest(acceptList);
					sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_ACCEPT_S, acceptBuilder));
				}
			}
		}

		BILogger.getBIData(BIMissionFlowData.class).log(player, Action.MISSION_REWARD, questCfg.getType(), questId, questCfg.getNameId(), 1);
		return true;
	}

	// 内部函数------------------------------------------------------------------------------------------

	/**
	 * 从指定任务组集合中接取任务
	 */
	private List<HSQuest> loadQuest(Map<Integer, QuestGroup> questGroupMap) {
		List<HSQuest> acceptQuestList = new ArrayList<HSQuest>();

		Iterator<Entry<Integer, QuestGroup>>  iter = questGroupMap.entrySet().iterator();
		while (iter.hasNext()) {
			Entry<Integer, QuestGroup> entry = iter.next();
			QuestCfg newQuest = getGroupNewQuest(entry.getValue());
			if (null == newQuest) {
				continue;
			}

			// 接取
			boolean active = acceptQuest(newQuest, acceptQuestList);
			if (false == active) {
				// 任务组未激活
				if (questGroupMap != inactiveQuestGroupMap) {
					inactiveQuestGroupMap.put(entry.getKey(), entry.getValue());
				}
				continue;
			} else {
				// 激活任务组
				if (questGroupMap != inactiveQuestGroupMap) {
					inactiveQuestGroupMap.remove(entry.getKey());
				} else {
					iter.remove();
				}
			}
		}

		return acceptQuestList;
	}

	/**
	 * 获取任务组未完成的第一个任务
	 * @return 如果任务组完成，返回null
	 */
	private QuestCfg getGroupNewQuest(QuestGroup group) {
		QuestCfg newQuest = null;
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		// 逆向查看任务组任务是否已完成，找到最后一个没完成的
		int index = group.questList.size() - 1;
		for (; index >= 0; --index) {
			QuestCfg quest = group.questList.get(index);

			if (GsConst.Quest.NORMAL_CYCLE == quest.getCycle()) {
				if (true == statisticsEntity.isQuestComplete(quest.getId())) {
					break;
				}
			} else if (GsConst.Quest.DAILY_CYCLE == quest.getCycle()) {
				if (true == statisticsEntity.isQuestDailyComplete(quest.getId())) {
					break;
				}
			}
			newQuest = quest;
		}
		return newQuest;
	}

	/**
	 * 接取任务
	 */
	private boolean acceptQuest(QuestCfg questCfg, List<HSQuest> acceptList) {
		if (questCfg.getLevel() <= player.getLevel()) {
			int progress = getQuestProgress(questCfg.getId());

			HSQuest.Builder builder = HSQuest.newBuilder();
			builder.setQuestId(questCfg.getId());
			builder.setProgress(progress);
			HSQuest quest = builder.build();
			player.getPlayerData().addQuest(quest);

			if (null != acceptList) {
				acceptList.add(quest);
			}

			// 如果是时效性任务组，接取下一个
			if (null != questCfg.getTimeBegin()) {
				QuestCfg nextQuestCfg = QuestUtil.getNextQuest(questCfg.getId());
				if (null != nextQuestCfg) {
					acceptQuest(nextQuestCfg, acceptList);
				}
			}

			BILogger.getBIData(BIMissionFlowData.class).log(player, Action.MISSION_ACCEPT, questCfg.getType(), questCfg.getId(), questCfg.getNameId(), 0);
			return true;
		}
		return false;
	}

	/**
	 * 计算任务进度
	 */
	private int getQuestProgress(int questId) {
		QuestCfg quest = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int cycle = quest.getCycle();
		int goalCount = quest.getGoalCount();
		int progress = 0;

		switch (quest.getGoalType()) {
		// 免费领取
		case GsConst.Quest.FREE: {
			// 永远是完成状态
			progress = goalCount;
			break;
		}
		// 角色等级达到N级
		case GsConst.Quest.LEVEL: {
			progress = player.getLevel();
			break;
		}
		//	三星通关X副本
		case GsConst.Quest.INSTANCE_X_STAR_3: {
			String instanceId = (String) quest.getGoalParamList().get(0);
			if (3 == statisticsEntity.getInstanceStar(instanceId)) {
				progress = goalCount;
			}
			break;
		}
		// 通关X副本N次
		case GsConst.Quest.INSTANCE_X_TIMES: {
			for (Object instanceId : quest.getGoalParamList()) {
				progress += statisticsEntity.getInstanceWinTimes((String) instanceId);
			}
			break;
		}
		// 通关副本N次
		case GsConst.Quest.INSTANCE_ALL_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getInstanceAllTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getInstanceAllTimesDaily();
			}
			break;
		}
		// 通关普通副本N次
		case GsConst.Quest.INSTANCE_NORMAL_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getInstanceNormalTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getInstanceNormalTimesDaily();
			}
			break;
		}
		// 通关挑战副本N次
		case GsConst.Quest.INSTANCE_HARD_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getInstanceHardTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getInstanceHardTimesDaily();
			}
			break;
		}
		// 领取X普通章节的满星奖励
		case GsConst.Quest.CHAPTER_X_NORMAL: {
			Integer chapterId = (Integer) quest.getGoalParamList().get(0);
			if (Const.ChapterBoxState.OPEN_VALUE == statisticsEntity.getNormalChapterBoxState(chapterId)) {
				progress = goalCount;
			}
			break;
		}
		// 领取X困难章节的满星奖励
		case GsConst.Quest.CHAPTER_X_HARD: {
			Integer chapterId = (Integer) quest.getGoalParamList().get(0);
			if (Const.ChapterBoxState.OPEN_VALUE == statisticsEntity.getHardChapterBoxState(chapterId)) {
				progress = goalCount;
			}
			break;
		}
		// 领取大冒险奖励N次
		case GsConst.Quest.ADVENTURE_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getAdventureTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getAdventureTimesDaily();
			}
			break;
		}
		// 进行竞技场N次
		case GsConst.Quest.ARENA_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getArenaTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getArenaTimesDaily();
			}
			break;
		}
		// 进入金币试炼N次
		case GsConst.Quest.HOLE_COIN_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getHoleTimes(GsConst.Instance.COIN_HOLE);
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getHoleTimesDaily(GsConst.Instance.COIN_HOLE);
			}
			break;
		}
		// 进入经验试炼N次
		case GsConst.Quest.HOLE_EXP_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getHoleTimes(GsConst.Instance.EXP_HOLE);
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getHoleTimesDaily(GsConst.Instance.EXP_HOLE);
			}
			break;
		}
		// 拥有过≥X品级的宠物N个
		case GsConst.Quest.MONSTER_STAGE_X_COUNT: {
			Integer stage = (Integer) quest.getGoalParamList().get(0);
			progress = statisticsEntity.getMonsterCountOverStage(stage);
			break;
		}
		// 拥有过≥X等级的宠物N个
		case GsConst.Quest.MONSTER_LEVEL_X_COUNT: {
			Integer level = (Integer) quest.getGoalParamList().get(0);
			progress = statisticsEntity.getMonsterCountOverLevel(level);
			break;
		}
		// 完成宠物合成N次
		case GsConst.Quest.MONSTER_MIX_TIMES: {
			progress = statisticsEntity.getMonsterMixTimes();
			break;
		}
		// 同一只宠物穿过≥X品级的装备达到N个
		case GsConst.Quest.EQUIP_WEAR_STAGE_X_COUNT: {
			Integer stage = (Integer) quest.getGoalParamList().get(0);
			progress = statisticsEntity.getEquipMaxCountOverStage(stage);
			break;
		}
		// 装备打孔N次
		case GsConst.Quest.EQUIP_SLOT_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getEquipPunchTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getEquipPunchTimesDaily();
			}
			break;
		}
		// 升级技能N次
		case GsConst.Quest.UP_SKILL_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getUpSkillTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getUpSkillTimesDaily();
			}
			break;
		}
		// 升级装备N次
		case GsConst.Quest.UP_EQUIP_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getUpEquipTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getUpEquipTimesDaily();
			}
			break;
		}
		// 购买金币N次
		case GsConst.Quest.BUY_COIN_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getBuyCoinTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getBuyCoinTimesDaily();
			}
			break;
		}
		// 购买礼包N次
		case GsConst.Quest.BUY_GIFT_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getBuyGiftTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getBuyGiftTimesDaily();
			}
			break;
		}
		// 商店购买X道具N次
		case GsConst.Quest.BUY_ITEM_X_TIMES: {
			for (Object itemCfgId : quest.getGoalParamList()) {
				progress += statisticsEntity.getBuyItemTimes((String) itemCfgId);
			}
			break;
		}
		// 充值钻石N个
		case GsConst.Quest.PAY_DIAMOND_COUNT: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getPayDiamondCount();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getPayDiamondCountDaily();
			}
			break;
		}
		// 消耗活力值N个
		case GsConst.Quest.USE_FATIGUE_COUNT: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getUseFatigueCount();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getUseFatigueCountDaily();
			}
			break;
		}
		// 使用钻石N个
		case GsConst.Quest.USE_DIAMOND_COUNT: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getUseDiamondCount();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getUseDiamondCountDaily();
			}
			break;
		}
		// 使用X物品N个
		case GsConst.Quest.USE_ITEM_X_COUNT: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				for (Object itemCfgId : quest.getGoalParamList()) {
					progress += statisticsEntity.getUseItemCount((String) itemCfgId);
				}
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				for (Object itemCfgId : quest.getGoalParamList()) {
					progress += statisticsEntity.getUseItemCountDaily((String) itemCfgId);
				}
			}
			break;
		}
		// 镶嵌宝石N次
		case GsConst.Quest.INLAY_ALL_TIMES: {
			progress = statisticsEntity.getInlayAllTimes();
			break;
		}
		// 镶嵌X类型宝石N次
		case GsConst.Quest.INLAY_TYPE_X_TIMES: {
			for (Object type : quest.getGoalParamList()) {
				progress += statisticsEntity.getInlayTypeTimes((Integer) type);
			}
			break;
		}
		// 合成宝石N个
		case GsConst.Quest.SYN_ALL_COUNT: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getSynAllTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getSynAllTimesDaily();
			}
			break;
		}
		// 合成X类型宝石N个
		case GsConst.Quest.SYN_TYPE_X_COUNT: {
			for (Object type : quest.getGoalParamList()) {
				progress += statisticsEntity.getSynTypeTimes((Integer) type);
			}
			break;
		}
		// 抽蛋N次
		case GsConst.Quest.EGG_ALL_TIMES: {
			 progress = statisticsEntity.getEggCoin1Times()
					 + statisticsEntity.getEggDiamond1FreeTimes()
					 + statisticsEntity.getEggDiamond1PayTimes()
					 + statisticsEntity.getEggCoin10Times() * 10
					 + statisticsEntity.getEggDiamond10Times() * 10;
			break;
		}
		// 金币抽蛋N次
		case GsConst.Quest.EGG_COIN_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getEggCoin1Times()
						+ statisticsEntity.getEggCoin10Times() * 10;
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getEggCoin1FreeTimesDaily()
						+ statisticsEntity.getEggCoin1PayTimesDaily()
						+ statisticsEntity.getEggCoin10TimesDaily() * 10;
			}
			break;
		}
		// 金币十连抽N次
		case GsConst.Quest.EGG_COIN_10_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getEggCoin10Times();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getEggCoin10TimesDaily();
			}
			break;
		}
		// 钻石抽蛋N次
		case GsConst.Quest.EGG_DIAMOND_TIMES: {
			progress = statisticsEntity.getEggDiamond1FreeTimes()
					+ statisticsEntity.getEggDiamond1PayTimes()
					+ statisticsEntity.getEggDiamond10Times() * 10;
			break;
		}
		// 钻石十连抽N次
		case GsConst.Quest.EGG_DIAMOND_10_TIMES: {
			progress = statisticsEntity.getEggDiamond10Times();
			break;
		}
		// 抽到≥X品级的宠物N个
		case GsConst.Quest.CALL_PET_STAGE_X_COUNT: {
			Integer stage = (Integer) quest.getGoalParamList().get(0);
			progress = statisticsEntity.getCallMonsterStageTimes(stage);
			break;
		}
		// 抽到≥X品级的装备N个
		case GsConst.Quest.CALL_EQUIP_STAGE_X_COUNT: {
			Integer stage = (Integer) quest.getGoalParamList().get(0);
			progress = statisticsEntity.getCallEquipStageTimes(stage);
			break;
		}
		// 加入公会N次
		case GsConst.Quest.SOCIETY_JOIN_TIMES: {
			// (有公会) 或 (无公会但曾经加入过公会)
			PlayerAllianceEntity allianceEntity = player.getPlayerData().getPlayerAllianceEntity();
			if (0 != allianceEntity.getAllianceId() || 0 != allianceEntity.getPreAllianceId()) {
				progress = goalCount;
			}
			break;
		}
		// 公会祈福N次
		case GsConst.Quest.SOCIETY_PRAY_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getAlliancePrayTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getAlliancePrayTimesDaily();
			}
			break;
		}
		// 公会bossN次
		case GsConst.Quest.SOCIETY_BOSS_TIMES: {
			progress = statisticsEntity.getAllianceBossTimes();
			break;
		}
		// 公会体力赠送N次
		case GsConst.Quest.SOCIETY_FATIGUE_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getAllianceFatigueTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getAllianceFatigueTimesDaily();
			}
			break;
		}
		// 手动刷新商店N次
		case GsConst.Quest.SHOP_REFRESH_TIMES: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getShopRefreshTimes();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = ShopUtil.getAllRefreshTimes(player);
			}
			break;
		}
		// 获得公会币N个
		case GsConst.Quest.COIN_SOCIETY_COUNT: {
			if (GsConst.Quest.NORMAL_CYCLE == cycle) {
				progress = statisticsEntity.getCoinAllianceCount();
			} else if (GsConst.Quest.DAILY_CYCLE == cycle) {
				progress = statisticsEntity.getCoinAllianceCountDaily();
			}
			break;
		}
		// 获得通天塔币N个
		case GsConst.Quest.COIN_TOWER_COUNT: {
			progress = statisticsEntity.getCoinTowerCount();
			break;
		}
		// 完成X任务
		case GsConst.Quest.QUEST_X: {
			Integer goalQuestId = (Integer) quest.getGoalParamList().get(0);
			if (true == statisticsEntity.isQuestComplete(goalQuestId)
					|| true == statisticsEntity.isQuestDailyComplete(goalQuestId)) {
				progress = goalCount;
			}
			break;
		}
		// 完成X类任务N个
		case GsConst.Quest.QUEST_TYPE_X_COUNT: {
			Set<Integer> questCompleteSet = statisticsEntity.getQuestCompleteSet();
			Set<Integer> questDailyCompleteSet = statisticsEntity.getQuestDailyCompleteSet();

			// TODO 如果type和cycle确定1对1关系，可优化
			for (Object type : quest.getGoalParamList()) {
				for (Integer questCompleteId : questCompleteSet) {
					QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questCompleteId);
					if (questCfg.getType() == (Integer) type) {
						progress += 1;
					}
				}
				for (Integer questCompleteId : questDailyCompleteSet) {
					QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questCompleteId);
					if (questCfg.getType() == (Integer) type) {
						progress += 1;
					}
				}
			}
			break;
		}
		// 完成X循环类任务N个
		case GsConst.Quest.QUEST_CYCLE_X_COUNT: {
			Set<Integer> questCompleteSet = statisticsEntity.getQuestCompleteSet();
			Set<Integer> questDailyCompleteSet = statisticsEntity.getQuestDailyCompleteSet();

			for (Object cycleType : quest.getGoalParamList()) {
				if (GsConst.Quest.NORMAL_CYCLE == (Integer)cycleType) {
					for (Integer questCompleteId : questCompleteSet) {
						QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questCompleteId);
						if (questCfg.getCycle() == (Integer) cycleType) {
							progress += 1;
						}
					}
				} else if (GsConst.Quest.DAILY_CYCLE == (Integer)cycleType) {
					for (Integer questCompleteId : questDailyCompleteSet) {
						QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questCompleteId);
						if (questCfg.getCycle() == (Integer) cycleType) {
							progress += 1;
						}
					}
				}
			}
			break;
		}
		default:
			break;
		}

		return progress > goalCount ? goalCount : progress;
	}

	@Override
	protected boolean onPlayerLogin() {
		lastPlayerLevel = player.getLevel();

		// 从统计数据中计算当前任务
		loadQuest(QuestUtil.getQuestGroupMap());

		// 同步任务信息
		player.getPlayerData().syncQuestInfo();
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		return true;
	}

	@Override
	public boolean onPlayerRefresh(List<Integer> refreshIndexList, boolean onLogin) {
		for (int index : refreshIndexList) {
			if (0 != (GsConst.Refresh.PlayerMaskArray[index] & GsConst.Refresh.DAILY )) {
				// 忽略登录时刷新
				if (true == onLogin) {
					continue;
				}

				// 清理已过期任务
				List<Integer> removeList = new ArrayList<Integer>();
				Map<Integer, HSQuest> questMap = player.getPlayerData().getQuestMap();
				Iterator<Entry<Integer, HSQuest>> iter = questMap.entrySet().iterator();
				while (iter.hasNext()) {
					Entry<Integer, HSQuest> entry = iter.next();
					int questId = entry.getValue().getQuestId();

					QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
					if (questCfg.getCycle() == GsConst.Quest.DAILY_CYCLE) {
						removeList.add(questId);
						iter.remove();
					}
				}
				if (false == removeList.isEmpty()) {
					HSQuestRemove.Builder builder = HSQuestRemove.newBuilder();
					builder.addAllQuestId(removeList);
					sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_REMOVE_S, builder));
				}

				// 接取新任务
				Map<Integer, QuestGroup> groupMap = QuestUtil.getCycleQuestGroupMap(GsConst.Quest.DAILY_CYCLE);
				List<HSQuest> acceptQuestList = loadQuest(groupMap);
				if (false == acceptQuestList.isEmpty()) {
					HSQuestAccept.Builder acceptBuilder = HSQuestAccept.newBuilder();
					acceptBuilder.addAllQuest(acceptQuestList);
					sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_ACCEPT_S, acceptBuilder));
				}
			}
		}

		return true;
	}
}
