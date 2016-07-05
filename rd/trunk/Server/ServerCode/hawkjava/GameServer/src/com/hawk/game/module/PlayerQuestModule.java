package com.hawk.game.module;

import java.util.ArrayList;
import java.util.HashSet;
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
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.QuestCfg;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.Quest.HSQuestAccept;
import com.hawk.game.protocol.Quest.HSQuest.Builder;
import com.hawk.game.protocol.Quest.HSQuestUpdate;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.GsConst.StatisticsType;
import com.hawk.game.util.QuestUtil;
import com.hawk.game.util.GsConst.Cycle;
import com.hawk.game.util.QuestUtil.QuestGroup;

/**
 * 任务模块
 * 
 * @author walker
 */
public class PlayerQuestModule extends PlayerModule {
	
	private static final Logger logger = LoggerFactory.getLogger("Protocol");
	private Set<Integer> inactiveQuestGroupSet = new HashSet<Integer>();
	
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
			updateBuilder.addAllQuestInfo(updateQuestList);
			HawkProtocol protocol = HawkProtocol.valueOf(HS.code.QUEST_UPDATE_S, updateBuilder);
			player.sendProtocol(protocol);
		}

		// 升级，检查未激活任务组，新接任务通知客户端
		if (statisticsType == StatisticsType.LEVEL_STATISTICS) {
			List<HSQuest> acceptQuestList = checkActiveQuest();
			if (false == acceptQuestList.isEmpty()) {
				HSQuestAccept.Builder acceptBuilder = HSQuestAccept.newBuilder();
				acceptBuilder.addAllQuestInfo(acceptQuestList);
				HawkProtocol protocol = HawkProtocol.valueOf(HS.code.QUEST_ACCEPT_S, acceptBuilder);
				player.sendProtocol(protocol);
			}
		}

		return true;
	}
	
	/**
	 * 任务交付
	 */
	@ProtocolHandler(code = HS.code.QUEST_SUBMIT_C_VALUE)
	private boolean onQuestSubmit(HawkProtocol cmd) {
		// TODO 接下一个任务
		return true;
	}

	// 内部函数------------------------------------------------------------------------------------------
	
	/**
	 * 初始化当前任务
	 */
	private boolean loadCurrentQuest() {
		Map<Integer, QuestGroup> questGroupMap = QuestUtil.getQuestGroupMap();
		for (Entry<Integer, QuestGroup> entry : questGroupMap.entrySet()) {
			QuestCfg newQuest = getGroupNewQuest(entry.getValue());	
			if (null == newQuest) {
				continue;
			}

			// 接取
			boolean active = acceptQuest(newQuest, null);
			if (false == active) {
				// 任务组未激活
				inactiveQuestGroupSet.add(entry.getKey());
				continue;
			}
		}
		return true;
	}

	/**
	 * 检查未激活任务组，如可接取则接取并激活
	 */
	private List<HSQuest> checkActiveQuest() {
		List<HSQuest> acceptQuestList = new ArrayList<HSQuest>();
		Map<Integer, QuestGroup> questGroupMap = QuestUtil.getQuestGroupMap();
		
		Iterator<Integer> iter = inactiveQuestGroupSet.iterator();
		while (iter.hasNext()) {
			int groupId = iter.next();
			QuestCfg newQuest = getGroupNewQuest(questGroupMap.get(groupId));
			if (null == newQuest) {
				continue;
			}

			// 接取
			boolean active = acceptQuest(newQuest, acceptQuestList);
			if (false == active) {
				continue;
			}

			// 激活任务组
			iter.remove();
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
		// 特定难度副本
		case GsConst.QuestGoalType.DIFFICULTY_GOAL: {
			String instanceId = (String) quest.getGoalParam();
			if (0 != statisticsEntity.getInstanceCompleteState(instanceId)) {
				progress = 1;
			}
			break;
		}
		//	特定星级副本
		case GsConst.QuestGoalType.STAR_GOAL: {
			String instanceId = (String) quest.getGoalParam();
			if (3 == statisticsEntity.getInstanceCompleteState(instanceId)) {
				progress = 1;
			}
			break;
		}
		// 普通难度副本
		case GsConst.QuestGoalType.INSTANCE_NORMAL_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getInstanceCount() - statisticsEntity.getHardCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getInstanceCountDaily() - statisticsEntity.getHardCountDaily();
			}
			break;
		}
		// 精英难度副本
		case GsConst.QuestGoalType.INSTANCE_HARD_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getHardCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getHardCountDaily();
			}
			break;
		}
		// 所有难度副本
		case GsConst.QuestGoalType.INSTANCE_ALL_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getInstanceCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getInstanceCountDaily();
			}
			break;
		}
		// 角色等级
		case GsConst.QuestGoalType.LEVEL_GOAL: {
			progress = player.getLevel();
			break;
		}
		// 宠物品级
		case GsConst.QuestGoalType.MONSTER_STAGE_GOAL: {
			int stage = (int) quest.getGoalParam();
			progress = statisticsEntity.getMonsterCountOverStage(stage);
			break;
		}
		// 宠物等级
		case GsConst.QuestGoalType.MONSTER_LEVEL_GOAL: {
			int level = (int) quest.getGoalParam();
			progress = statisticsEntity.getMonsterCountOverLevel(level);
			break;
		}
		// 竞技场次数
		case GsConst.QuestGoalType.ARENA_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getArenaCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getArenaCountDaily();
			}
			break;
		}
		//	时光之穴次数
		case GsConst.QuestGoalType.TIME_HOLE_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getTimeholeCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getTimeholeCountDaily();
			}
			break;
		}
		// 炼妖炉次数
		case GsConst.QuestGoalType.MONSTER_MIX_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getMonsterMixCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getMonsterMixCountDaily();
			}
			break;
		}
		// 大冒险次数
		case GsConst.QuestGoalType.ADVENTURE_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getAdventureCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getAdventureCountDaily();
			}
			break;
		}
		// bossrush次数
		case GsConst.QuestGoalType.BOSSRUSH_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getBossrushCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getBossrushCountDaily();
			}
			break;
		}
		// 稀有探索次数
		case GsConst.QuestGoalType.EXPLORE_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getExploreCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getExploreCountDaily();
			}
			break;
		}
		// 升级技能次数
		case GsConst.QuestGoalType.SKILL_UP_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getSkillUpCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getSkillUpCountDaily();
			}
			break;
		}
		// 升级装备次数
		case GsConst.QuestGoalType.EQUIP_UP_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getEquipUpCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getEquipUpCountDaily();
			}
			break;
		}
		// 购买金币次数
		case GsConst.QuestGoalType.BUYCOIN_GOAL: {
			if (quest.getCycle() == Cycle.NORMAL_CYCLE) {
				progress = statisticsEntity.getCoinOrderCount();
			} else if (quest.getCycle() == Cycle.DAILY_CYCLE) {
				progress = statisticsEntity.getCoinOrderCountDaily();
			}
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
		loadCurrentQuest();
		
		// 同步任务信息
		player.getPlayerData().syncQuestInfo();
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		return true;
	}
}
