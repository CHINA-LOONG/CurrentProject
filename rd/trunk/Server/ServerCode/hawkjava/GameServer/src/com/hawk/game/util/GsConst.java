package com.hawk.game.util;

/**
 * 游戏常量定义
 * 
 * @author hawk
 */
public class GsConst {
	// 刷新间隔帧
	public static final int REFRESH_PERIOD = 100;
	// 副本翻牌数量
	public static final int INSTANCE_CARD_COUNT = 4;
	// 装备在背包时monsterId
	public static final int EQUIPNOTDRESS = -1;
	// 扫荡券Id
	public static final String SWEEP_TICKET = "10003";
	// 最大技能点数
	public static final int MAX_SKILL_POINT = 10;
	// 技能点增长秒数
	public static final int SKILL_POINT_TIME = 6 * 60;
	// 最大邮件数
	public static final int MAX_MAIL_COUNT = 300;
	// 最大品级数
	public static final int EQUIP_MAX_STAGE = 6;
	// 最大级别数
	public static final int EQUIP_MAX_LEVEL = 9;
	// 宝石最大类别数
	public static final int GEM_MAX_TYPE = 3;
	// 宝石镶嵌占位
	public static final String EQUIP_GEM_NONE = "0";
	// 聊天最大长度
	public static final int MAX_IM_CHAT_LENGTH = 500;
	// 翻译系统appId
	//public static final String TRANSLATE_APP_ID = "hawk";
	// 翻译系统模式
	//public static final String TRANSLATE_MODE = "s2s";
	// 翻译系统cache更新时间间隔秒数
	//public static final int TRANSLATE_CACHE_TIME = 10 * 60;
	// 没有语言
	public static final String TRANSLATE_LANGUAGE_NULL = "";
	// 默认语言
	public static final String DEFAULT_LANGUAGE = "en";
	
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
		public static final int APP = 1;
		// 即时通讯
		public static final int IM = 2;
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
		// 玩家收到新邮件
		public static final int MAIL_NEW = 1003;
	}

	/**
	 * 模块定义
	 */
	public static class ModuleType {
		// 登陆模块
		public static final int LOGIN_MODULE = 1;
		// 统计模块
		public static final int STATISTICS_MODULE = 2;
		// 系统设置模块
		public static final int SETTING_MODULE = 3;
		// 怪物模块
		public static final int MONSTER_MODULE = 4;
		// 副本模块
		public static final int INSTANCE_MODULE = 5;
		// 装备模块
		public static final int ITEM_MODULE = 6;
		// 道具模块
		public static final int EQUIP_MODULE = 7;
		// 任务模块
		public static final int QUEST_MODULE = 8;
		// 邮件模块
		public static final int MAIL_MODULE = 9;
		// IM模块
		public static final int IM_MODULE = 10;
		// 商店模块
		public static final int SHOP_MODULE = 11;
		// 公会模块
		public static final int ALLIANCE_MODULE = 12;

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

		public static final int SHOP_REFRESH_TIME_FIRST = 104;
		public static final int SHOP_REFRESH_TIME_SECOND = 105;
		public static final int SHOP_REFRESH_TIME_THIRD = 106;

		public static final int ALLIANCE_REFRESH_TIME_FIRST = 107;
		public static final int ALLIANCE_REFRESH_TIME_SECOND = 108;
		public static final int ALLIANCE_REFRESH_TIME_THIRD = 109;

		public static final int PERS_REFRESH_END = 110;
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

	/**
	 * item 配置解析类型
	 */
	public static class ItemParseType {
		public static final int PARSE_DEFAULT = 0;
		public static final int PARSE_MONSTER_STAGE = 1;	
		public static final int PARSE_EQUIP_ATTR = 2;	
	}

	/**
	 * equip 品级对应的打孔数量
	 */
	public enum EquipStagePunch
	{
		NONE_STAGE(0),
		WHITE_STAGE(0),
		GREEN_STAGE(0),
		BLUE_STAGE(1),
		PURPLE_STAGE(2),
		ORANGE_STAGE(3),
		RED_STAGE(4);
		
		private int punchCount;
		
		private EquipStagePunch(int count) {
			this.punchCount = count;
		}
		
		public int GetCount(){
			return punchCount;
		}
	}

	/**
	 * item 消耗检查内容
	 */
	public static class PlayerItemCheckResult {
		// 金币不足
		public static final int COINS_NOT_ENOUGH = 1;
		// 钻石不足
		public static final int GOLD_NOT_ENOUGH = 2;
		public static final int LEVEL_NOT_ENOUGH = 4;
		public static final int EXP_NOT_ENOUGH = 5;
		public static final int VIPLEVEL_NOT_ENOUGH = 6;
		// 道具不足
		public static final int TOOLS_NOT_ENOUGH = 7;
		// 装备不足
		public static final int EQUIP_NOT_ENOUGH = 8;
		// 宠物不足
		public static final int MONSTER_NOT_ENOUGH = 8;
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
		public static final int TIMEHOLE_GOAL  = 10;
		public static final int MONSTER_MIX_GOAL  = 11;
		public static final int ADVENTURE_GOAL  = 12;
		public static final int BOSSRUSH_GOAL = 13;
		public static final int EXPLORE_GOAL = 14;
		public static final int SKILL_UP_GOAL = 15;
		public static final int EQUIP_UP_GOAL = 16;
		public static final int BUY_COIN_GOAL = 17;
		public static final int GET_FATIGUE_GOAL = 18;
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

	public static class Alliance {
		/*
		 * 0:普通成员, 1:副会长, 2:会长
		 */
		public static final int ALLIANCE_POS_COMMON = 0;
		public static final int ALLIANCE_POS_COPYMAIN = 1;
		public static final int ALLIANCE_POS_MAIN = 2;
	}
}
