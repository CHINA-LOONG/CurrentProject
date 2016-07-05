package com.hawk.game.module;

import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.QuestCfg;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.QuestUtil;
import com.hawk.game.util.GsConst.Cycle;
import com.hawk.game.util.QuestUtil.QuestGroup;
import com.sun.java.swing.plaf.windows.WindowsBorders.ProgressBarBorder;

/**
 * 任务模块
 * 
 * @author walker
 */
public class PlayerQuestModule extends PlayerModule {
	
	private static final Logger logger = LoggerFactory.getLogger("Protocol");
	
	public PlayerQuestModule(Player player) {
		super(player);
	}
	
	/**
	 * 玩家统计数据更新
	 */
	@MessageHandler(code = GsConst.MsgType.STATISTICS_UPDATE)
	private boolean onStatisticsUpdate() {
		return true;
	}
	
	/**
	 * 任务交付
	 */
	@ProtocolHandler(code = HS.code.QUEST_SUBMIT_C_VALUE)
	private boolean onQuestSubmit(HawkProtocol cmd) {
		return true;
	}
	
	/**
	 * 当前任务初始化
	 */
	private boolean loadCurrentQuest() {
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		Set<Integer> questCompleteSet = statisticsEntity.getQuestCompleteSet();
		Set<Integer> questCompleteDailySet = statisticsEntity.getQuestCompleteDailySet();
		
		Map<Integer, QuestGroup> questGroupMap = QuestUtil.getQuestGroupMap();
		for (Entry<Integer, QuestGroup> entry : questGroupMap.entrySet()) {
			// 逆向查看任务组任务是否已完成，找到最后一个没完成的
			QuestGroup group = entry.getValue();
			QuestCfg lastUncompletedQuest = null;
			
			int index = group.questList.size() - 1;
			for (; index >= 0; --index) {
				QuestCfg questCfg = group.questList.get(index);
				if (questCfg.getCycle() == Cycle.NORMAL_CYCLE) {
					if (true == questCompleteSet.contains(questCfg.getId())) {
						break;
					}
				} else if (questCfg.getCycle() == Cycle.DAILY_CYCLE) {
					if (true == questCompleteDailySet.contains(questCfg.getId())) {
						break;
					}
				}
				lastUncompletedQuest = questCfg;
			}
			
			if (null == lastUncompletedQuest) {
				continue;
			}

			// 接取
			acceptQuest(lastUncompletedQuest);
			
			// 时效性任务组，后续任务都接取
			if (null != lastUncompletedQuest.getTimeBegin()) {
				for (index += 2; index < group.questList.size(); ++index) {
					acceptQuest(group.questList.get(index));
				}
			}
		}
		return true;
	}

	/**
	 * 接取任务
	 */
	private boolean acceptQuest(QuestCfg questCfg) {
		if (questCfg.getLevel() <= player.getLevel()) {
			HSQuest.Builder quest = HSQuest.newBuilder();
			quest.setQuestId(questCfg.getId());
			quest.setProgress(getQuestProgress(questCfg));
			player.getPlayerData().setQuest(quest.build());
			return true;
		}
		return false;
	}

	/**
	 * 计算任务进度
	 */
	private int getQuestProgress(QuestCfg quest) {
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
			progress = statisticsEntity.getMonsterCountOfStage(stage);
			break;
		}
		// 宠物等级
		case GsConst.QuestGoalType.MONSTER_LEVEL_GOAL: {
			int level = (int) quest.getGoalParam();
			progress = statisticsEntity.getMonsterCountOfLevel(level);
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
