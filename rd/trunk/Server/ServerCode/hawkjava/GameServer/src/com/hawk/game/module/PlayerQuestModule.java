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

import com.hawk.game.config.QuestCfg;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.Quest.HSQuest.Builder;
import com.hawk.game.protocol.Quest.HSQuestAccept;
import com.hawk.game.protocol.Quest.HSQuestRemove;
import com.hawk.game.protocol.Quest.HSQuestSubmit;
import com.hawk.game.protocol.Quest.HSQuestSubmitRet;
import com.hawk.game.protocol.Quest.HSQuestUpdate;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.GsConst.Cycle;
import com.hawk.game.util.GsConst.StatisticsType;
import com.hawk.game.util.ProtoUtil;
import com.hawk.game.util.QuestUtil;
import com.hawk.game.util.QuestUtil.QuestGroup;
import com.hawk.game.util.TimeUtil;

/**
 * 任务模块
 * 
 * @author walker
 */
public class PlayerQuestModule extends PlayerModule {

	private Map<Integer, QuestGroup> inactiveQuestGroupMap = new HashMap<Integer, QuestGroup>();

	public PlayerQuestModule(Player player) {
		super(player);
	}

	/**
	 * 玩家统计数据更新
	 */
	@MessageHandler(code = GsConst.MsgType.STATISTICS_UPDATE)
	private boolean onStatisticsUpdate(HawkMsg msg) {
		int statisticsType = msg.getParam(0);

		// 更新任务进度，通知客户端
		List<HSQuest> updateQuestList = new ArrayList<HSQuest>();
		Map<Integer, HSQuest> questMap = player.getPlayerData().getQuestMap();
		for (Entry<Integer, HSQuest> entry : questMap.entrySet()) {
			int newProgress = getQuestProgress(entry.getValue().getQuestId());
			if (newProgress != entry.getValue().getProgress()) {
				Builder quest = entry.getValue().toBuilder();
				quest.setProgress(newProgress);
				entry.setValue(quest.build());

				updateQuestList.add(entry.getValue());
			}
		}
		if (false == updateQuestList.isEmpty()) {
			HSQuestUpdate.Builder updateBuilder = HSQuestUpdate.newBuilder();
			updateBuilder.addAllQuest(updateQuestList);
			sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_UPDATE_S, updateBuilder));
		}

		// 升级，检查未激活任务组，新接任务通知客户端
		if (statisticsType == StatisticsType.LEVEL_STATISTICS) {
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
			if (false == TimeUtil.isTimeInPeriod(curTime, questCfg.getTimeBegin(), questCfg.getTimeEnd())) {
				sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.questError.QUEST_NOT_OPEN_VALUE, 1));
				return true;
			}
		}

		// 发奖
		int exp = questCfg.getExpReward(player.getLevel());
		List<ItemInfo> list = questCfg.getReward().getRewardList();
		AwardItems awardItems = new AwardItems();
		awardItems.addExp(exp);
		awardItems.addItemInfos(list);
		awardItems.rewardTakeAffectAndPush(player, Action.QUEST_SUBMIT, HS.code.QUEST_SUBMIT_C_VALUE);

		// 记录
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		if (questCfg.getCycle() == Cycle.NORMAL_CYCLE) {
			statisticsEntity.getQuestCompleteSet().add(questId);
		} else if (questCfg.getCycle() == Cycle.DAILY_CYCLE) {
			statisticsEntity.getQuestCompleteDailySet().add(questId);
		}
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
				acceptQuest(nextQuestCfg, acceptList);
				if (false == acceptList.isEmpty()) {
					HSQuestAccept.Builder acceptBuilder = HSQuestAccept.newBuilder();
					acceptBuilder.addAllQuest(acceptList);
					sendProtocol(HawkProtocol.valueOf(HS.code.QUEST_ACCEPT_S, acceptBuilder));
				}
			}
		}

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
		Set<Integer> questCompleteSet = statisticsEntity.getQuestCompleteSet();
		Set<Integer> questCompleteDailySet = statisticsEntity.getQuestCompleteDailySet();

		// 逆向查看任务组任务是否已完成，找到最后一个没完成的
		int index = group.questList.size() - 1;
		for (; index >= 0; --index) {
			QuestCfg quest = group.questList.get(index);

			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				if (true == questCompleteSet.contains(quest.getId())) {
					break;
				}
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				if (true == questCompleteDailySet.contains(quest.getId())) {
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
			player.getPlayerData().setQuest(quest);

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

			return true;
		}
		return false;
	}

	/**
	 * 计算任务进度
	 */
	private int getQuestProgress(int questId) {
		QuestCfg quest = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		int progress = 0;
		int goalCount = quest.getGoalCount();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		switch (quest.getGoalType()) {
		// 通关X副本
		case GsConst.QuestGoalType.INSTANCE: {
//			String instanceId = (String) quest.getGoalParam();
//			if (0 != statisticsEntity.getInstanceStar(instanceId)) {
//				progress = 1;
//			}
			break;
		}
		//	满星通关X副本
		case GsConst.QuestGoalType.INSTANCE_STAR3: {
//			String instanceId = (String) quest.getGoalParam();
//			if (3 == statisticsEntity.getInstanceStar(instanceId)) {
//				progress = 1;
//			}
			break;
		}
		// 通关普通副本次数
		case GsConst.QuestGoalType.INSTANCE_NORMAL_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getInstanceAllCount() - statisticsEntity.getHardCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getInstanceAllCountDaily() - statisticsEntity.getHardCountDaily();
			}
			break;
		}
		// 通关精英副本次数
		case GsConst.QuestGoalType.INSTANCE_HARD_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getHardCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getHardCountDaily();
			}
			break;
		}
		// 通关副本次数
		case GsConst.QuestGoalType.INSTANCE_ALL_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getInstanceAllCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getInstanceAllCountDaily();
			}
			break;
		}
		// 满星通关X章节
		case GsConst.QuestGoalType.CHAPTER: {
			break;
		}
		// 达到X角色等级
		case GsConst.QuestGoalType.LEVEL: {
			progress = player.getLevel();
			break;
		}
		// 达到X品级宠物数量
		case GsConst.QuestGoalType.MONSTER_STAGE_COUNT: {
//			int stage = (int) quest.getGoalParam();
//			progress = statisticsEntity.getMonsterCountOverStage(stage);
			break;
		}
		// 达到X等级宠物数量
		case GsConst.QuestGoalType.MONSTER_LEVEL_COUNT: {
//			int level = (int) quest.getGoalParam();
//			progress = statisticsEntity.getMonsterCountOverLevel(level);
			break;
		}
		// 合成X宠物数量
		case GsConst.QuestGoalType.MONSTER_MIX_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getMonsterMixCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getMonsterMixCountDaily();
			}
			break;
		}
		// 完成竞技场次数
		case GsConst.QuestGoalType.ARENA_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getArenaCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getArenaCountDaily();
			}
			break;
		}
		//	完成金币试炼次数
		case GsConst.QuestGoalType.HOLE_COIN_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getHoleCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = 0;
				for (Entry<Integer, Integer> entry : statisticsEntity.getHoleCountDailyMap().entrySet()) {
					progress += entry.getValue();
				}
			}
			break;
		}
		// 完成经验试炼次数
		case GsConst.QuestGoalType.HOLE_EXP_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getHoleCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = 0;
				for (Entry<Integer, Integer> entry : statisticsEntity.getHoleCountDailyMap().entrySet()) {
					progress += entry.getValue();
				}
			}
			break;
		}
		// 完成通天塔次数
		case GsConst.QuestGoalType.TOWER_TIMES: {
			break;
		}
		// 完成大冒险次数
		case GsConst.QuestGoalType.ADVENTURE_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getAdventureCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getAdventureCountDaily();
			}
			break;
		}
		// 升级技能次数
		case GsConst.QuestGoalType.UP_SKILL_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getSkillUpCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getSkillUpCountDaily();
			}
			break;
		}
		// 升级装备次数
		case GsConst.QuestGoalType.UP_EQUIP_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getEquipUpCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getEquipUpCountDaily();
			}
			break;
		}
		// 购买金币次数
		case GsConst.QuestGoalType.BUY_COIN_TIMES: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getCoinOrderCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getCoinOrderCountDaily();
			}
			break;
		}
		// 购买礼包次数
		case GsConst.QuestGoalType.BUY_GIFT_TIMES: {
			break;
		}
		// 购买X物品次数
		case GsConst.QuestGoalType.BUY_ITEM_TIMES: {
			break;
		}
		// 购买钻石数量
		case GsConst.QuestGoalType.PAY_DIAMOND_COUNT: {
			break;
		}
		// 消耗活力值数量
		case GsConst.QuestGoalType.USE_FATIGUE_COUNT: {
			break;
		}
		// 使用X物品数量
		case GsConst.QuestGoalType.USE_ITEM_COUNT: {
			break;
		}
		// 使用钻石数量
		case GsConst.QuestGoalType.USE_DIAMOND_COUNT: {
			break;
		}
		// 镶嵌宝石次数
		case GsConst.QuestGoalType.INLAY_ALL_TIMES: {
			break;
		}
		// 镶嵌X类型宝石次数
		case GsConst.QuestGoalType.INLAY_TYPE_TIMES: {
			break;
		}
		// 合成宝石次数
		case GsConst.QuestGoalType.SYN_ALL_TIMES: {
			break;
		}
		// 合成X类型宝石次数
		case GsConst.QuestGoalType.SYN_TYPE_TIMES: {
			break;
		}
		// 金币抽蛋次数
		case GsConst.QuestGoalType.EGG_COIN_TIMES: {
			break;
		}
		// 钻石抽蛋次数
		case GsConst.QuestGoalType.EGG_DIAMOND_TIMES: {
			break;
		}
		// 抽蛋次数
		case GsConst.QuestGoalType.EGG_ALL_TIMES: {
			break;
		}
		// 抽到X品级宠物次数
		case GsConst.QuestGoalType.CALL_PET_STAGE_TIMES: {
			break;
		}
		// 抽到X品级装备次数
		case GsConst.QuestGoalType.CALL_EQUIP_STAGE_TIMES: {
			break;
		}
		// 抽到X物品次数
		case GsConst.QuestGoalType.CALL_ITEM_TIMES: {
			break;
		}
		// 加入公会次数
		case GsConst.QuestGoalType.SOCIETY_JOIN_TIMES: {
			break;
		}
		// 退出公会次数
		case GsConst.QuestGoalType.SOCIETY_LEAVE_TIMES: {
			break;
		}
		// 公会祈福次数
		case GsConst.QuestGoalType.SOCIETY_PRAY_TIMES: {
			break;
		}
		// 公会boss次数
		case GsConst.QuestGoalType.SOCIETY_BOSS_TIMES: {
			break;
		}
		// 公会体力赠送次数
		case GsConst.QuestGoalType.SOCIETY_FATIGUE_TIMES: {
			break;
		}
		// 刷新商店次数
		case GsConst.QuestGoalType.SHOP_REFRESH_TIMES: {
			break;
		}
		// 获得竞技场币数量
		case GsConst.QuestGoalType.COIN_ARENA_COUNT: {
			break;
		}
		// 获得公会币数量
		case GsConst.QuestGoalType.COIN_SOCIETY_COUNT: {
			break;
		}
		// 获得通天塔币数量
		case GsConst.QuestGoalType.COIN_TOWER_COUNT: {
			break;
		}
		// 完成X任务
		case GsConst.QuestGoalType.QUEST: {
			break;
		}
		// 完成X类型任务数量
		case GsConst.QuestGoalType.QUEST_TYPE_COUNT: {
			break;
		}
		// 完成X循环性任务数量
		case GsConst.QuestGoalType.QUEST_CYCLE_COUNT: {
			break;
		}
		// 携带过X品级装备数量
		case GsConst.QuestGoalType.EQUIP_STAGE_COUNT: {
			break;
		}
		// 打孔次数
		case GsConst.QuestGoalType.EQUIP_SLOT_TIMES: {
			break;
		}
		// 免费领取
		case GsConst.QuestGoalType.FREE_GOAL: {
			// 永远是完成状态
			progress = goalCount;
			break;
		}
		default:
			break;
		}

		return goalCount < progress ? goalCount : progress;
	}

	@Override
	protected boolean onPlayerLogin() {
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
			if (0 != (GsConst.PlayerRefreshMask[index] & GsConst.RefreshMask.DAILY )) {
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
					if (questCfg.getCycle() == GsConst.Cycle.DAILY_CYCLE) {
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
				Map<Integer, QuestGroup> groupMap = QuestUtil.getCycleQuestGroupMap(GsConst.Cycle.DAILY_CYCLE);
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
