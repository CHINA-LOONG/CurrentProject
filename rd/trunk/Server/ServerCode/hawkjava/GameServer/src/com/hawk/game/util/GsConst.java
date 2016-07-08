package com.hawk.game.util;

/**
 * 游戏常量定义
 * 
 * @author hawk
 */
public class GsConst {
	// 刷新间隔帧
	public static final int REFRESH_PERIOD = 100;
	
	public static final int INSTANCE_CARD_COUNT = 4;

	// 装备在背包时monsterId
	public static final int EQUIPNOTDRESS = -1;
	
	// 扫荡券Id
	public static final int SWEEP_TICKET = 10003;

	/**
	 * 对象类型
	 * 
	 * @author hawk
	 */
	public static class ObjType {
		// 玩家对象
		public static final int PLAYER = 1;
		// 应用程序
		public static final int MANAGER = 100;
	}

	/**
	 * 系统对象id
	 * 
	 * @author hawk
	 */
	public static class ObjId {
		// 应用程序
		public static final int APP = 100;
	}

	/**
	 * 消息定义
	 */
	public static class MsgType {
		// 连接断开
		public static final int SESSION_CLOSED = 2;
		// 玩家上线
		public static final int PLAYER_LOGIN = 3;
		// 玩家初始化完成
		public static final int PLAYER_ASSEMBLE = 4;

		// 奖励宠物
		public static final int PRESENT_MONSTER = 1001;
		// 玩家统计数据更新
		public static final int STATISTICS_UPDATE = 1002;
	}

	/**
	 * 模块定义
	 */
	public static class ModuleType {
		// 登陆模块
		public static final int LOGIN_MODULE = 1;
		// 统计模块
		public static final int STATISTICS_MODULE = 2;
		// 怪物模块
		public static final int MONSTER_MODULE = 3;
		// 副本模块
		public static final int INSTANCE_MODULE = 4;
		// 装备模块
		public static final int ITEM_MODULE = 5;
		// 道具模块
		public static final int EQUIP_MODULE = 6;
		// 任务模块
		public static final int QUEST_MODULE = 7;
		
		// 空闲模块(保证在最后)
		public static final int IDLE_MODULE = 100;
	}

	/**
	 * 刷新类型定义
	 */
	public static class RefreshType {
		// 全局刷新----------------------------------------------------------
		public static final int GLOBAL_REFRESH_BEGIN = 0;
		public static final int QUEST_GLOBAL_REFRESH = 1;
		public static final int GLOBAL_REFRESH_END = 2;

		// 个人刷新----------------------------------------------------------
		public static final int PERS_REFRESH_BEGIN = 100;
		public static final int DAILY_PERS_REFRESH = 101;
//		public static final int SIGN_IN_PERS_REFRESH = 102;
//		public static final int INSTANCE_PERS_REFRESH = 103;
		public static final int PERS_REFRESH_END = 102;
	}

	/**
	 * 属性类型定义
	 */
	public static class PropertyType {
		// 金木水火土
		public static final int JIN_PROPERTY = 1;
		public static final int MU_PROPERTY = 2;
		public static final int SHUI_PROPERTY = 3;
		public static final int HUO_PROPERTY = 4;
		public static final int TU_PROPERTY = 5;
	}

	public static class PlayerItemCheckResult {
		/**
		 * 金币不足
		 */
		public static final int COINS_NOT_ENOUGH = 1;
		/**
		 * 钻石不足
		 */
		public static final int GOLD_NOT_ENOUGH = 2;

		public static final int LEVEL_NOT_ENOUGH = 4;

		public static final int EXP_NOT_ENOUGH = 5;

		public static final int VIPLEVEL_NOT_ENOUGH = 6;
		/**
		 * 道具不足
		 */
		public static final int TOOLS_NOT_ENOUGH = 7;
		/**
		 * 装备不足
		 */
		public static final int EQUIP_NOI_ENOUGH = 8;
	}

	// 循环性
	public static class Cycle {
		public static final int NORMAL_CYCLE = 1;
		public static final int DAILY_CYCLE = 2;
	}

	// 任务类型
	public static class QuestType {
		public static final int 	STORY_QUEST = 1;
		public static final int DAILY_QUEST = 2;
		public static final int BIOGRAPHY_QUEST = 3;
	}

	// 任务目标类型
	public static class QuestGoalType {
		public static final int DIFFICULTY_GOAL = 1;
		public static final int STAR_GOAL = 2;
		public static final int INSTANCE_NORMAL_GOAL = 3;
		public static final int INSTANCE_HARD_GOAL = 4;
		public static final int INSTANCE_ALL_GOAL = 5;
		public static final int LEVEL_GOAL = 6;
		public static final int MONSTER_STAGE_GOAL = 7;
		public static final int MONSTER_LEVEL_GOAL = 8;
		public static final int ARENA_GOAL = 9;
		public static final int TIME_HOLE_GOAL  = 10;
		public static final int MONSTER_MIX_GOAL  = 11;
		public static final int ADVENTURE_GOAL  = 12;
		public static final int BOSSRUSH_GOAL = 13;
		public static final int EXPLORE_GOAL = 14;
		public static final int SKILL_UP_GOAL = 15;
		public static final int EQUIP_UP_GOAL = 16;
		public static final int BUYCOIN_GOAL = 17;
	}

	// 统计数据类型
	public static class StatisticsType {
		public static final int LEVEL_STATISTICS = 1;
		public static final int OTHER_STATISTICS = 2;
	}

	// 副本难度
	public static class InstanceDifficulty {
		public static final int NORMAL_INSTANCE = 1;
		public static final int HARD_INSTANCE = 2;
	}
}
