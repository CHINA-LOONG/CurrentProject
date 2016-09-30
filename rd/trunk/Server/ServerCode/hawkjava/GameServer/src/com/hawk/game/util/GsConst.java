package com.hawk.game.util;

/**
 * 游戏常量定义
 * 
 * @author hawk
 */
public class GsConst {
	// BI版本
	public static final String BIVersion = "2.0";
	// 没有值，无效值，不可用
	public static final int UNUSABLE = -1;
	// 装备在背包时monsterId
	public static final int EQUIP_NOT_DRESS = -1;
	// 扫荡券Id
	public static final String SWEEP_TICKET = "10003";
	// 万能碎片Id
	public static final String COMMON_FRAGMENT = "20001";
	// 活力最大值
	public static final int MAX_FATIGUE_COUNT = 5000;
	// 技能点最大值
	public static final int MAX_SKILL_POINT = 10;
	// 大冒险条件刷新次数最大值
	public static final int MAX_ADVENTURE_CHANGE = 10;
	// 活力值恢复秒数
	public static final int FATIGUE_TIME = 10 * 60;
	// 技能点恢复秒数
	public static final int SKILL_POINT_TIME = 6 * 60;
	// 大冒险条件刷新次数恢复秒数
	public static final int ADVENTURE_CHANGE_TIME = 60 * 60;
	// 最大邮件数
	public static final int MAX_MAIL_COUNT = 300;
	// 最大品级数
	public static final int EQUIP_MAX_STAGE = 6;
	// 最大等级数
	public static final int GEM_MAX_STAGE = 6;
	// 最大级别数
	public static final int EQUIP_MAX_LEVEL = 9;
	// 宝石最大类别数
	public static final int GEM_MAX_TYPE = 9;
	// 宝石镶嵌占位
	public static final String EQUIP_GEM_NONE = "0";
	// 聊天最大长度
	public static final int MAX_IM_CHAT_LENGTH = 500;
	// 没有语言
	public static final String TRANSLATE_LANGUAGE_NULL = "";
	// 默认语言
	public static final String DEFAULT_LANGUAGE = "en";
	// 宠物消耗自身
	public static final String MONSTER_CONSUME_SELF = "self";
	// 月卡检测字段
	public static final String MONTH_CARD = "month";
	// 月卡时长
	public static final int MONTH_CARD_TIME = 30;
	// 钻石最大值
	public static final int MAX_GOLD_COUNT = 999999999;
	// 金币最大值
	public static final int MAX_COIN_COUNT = 999999999;
	// 副本最大星数
	public static final int MAX_INSTANCE_STAR = 3;
	// 副本复活次数
	public static final int INSTANCE_REVIVE_COUNT = 3;
	// 副本复活消耗
	public static final int[] INSTANCE_REVIVE_CONSUME = new int[] { 15, 30, 50 };
	// 上阵己方怪物最大数量
	public static final int MAX_BATTLE_MONSTER_COUNT = 5;
	// 钻石兑换金币索引
	public static final int GOLD_TO_COIN_INDEX = 1;
	// 最大屏蔽玩家数
	public static final int MAX_BLOCK_COUNT = 5;
	// 合成上一级宝石数量
	public static final int NEXT_LEVEL_GEM_COUNT = 1;
	// 合成上一级需要宝石数量
	public static final int GEM_COMPOSE_COUNT = 5;
	// 最大合成次数
	public static final int COMPOSE_MAX_COUNT = 10;
	// 购买大冒险条件刷新次数消耗钻石
	public static final int ADVENTURE_CHANGE_BUY_CONSUME = 50;
	// 大冒险上阵怪物数
	public static final int ADVENTURE_MONSTER_COUNT = 5;
	// 大冒险持续时间档位
	public static final int[] ADVENTURE_TIME_GEAR = new int[] { 5 * 3600, 8 * 3600, 12 * 3600 };
	// 趣加Funplus
	public static final String FUNPLUS_APP_ID = "1013";
	public static final String FUNPLUS_KEY = "aacbb2be28236338a3cb61d610a76f9e";

	public static final int PVP_WEAK_REFRESH_TIME_ID = 110;	
	
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
		// 商店
		public static final int SHOP = 3;
		// 公会
		public static final int ALLIANCE = 4;
		// 快照
		public static final int SNAPSHOT = 5;
		// 快照
		public static final int PVP = 6;
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
		// 玩家重新连接
		public static final int PLAYER_RECONNECT = 5;
		// 玩家等级变化更新
		public static final int PLAYER_LEVEL_UP = 6;

		// 创建公会
		public static final int ALLIANCE_CREATE = 100;
		// 申请入会
		public static final int ALLIANCE_APPLY = 101;
		// 操作申请
		public static final int ALLIANCE_HANDLE_APPLY = 102;
		// 职位变更
		public static final int ALLIANCE_CHANGE_POS = 103;
		// 公会会主变更
		public static final int ALLIANCE_CHANGE_OWNER = 104;
		// 离开公会
		public static final int ALLIANCE_LEAVE = 105;
		// 踢人
		public static final int ALLIANCE_KICK = 106;
		// 升级
		public static final int ALLIANCE_LEVEL_UP = 107;
		// 祈福
		public static final int ALLIANCE_PRAY = 108;
		// 设置
		public static final int ALLIANCE_SETTING = 109;
		// 疲劳只赠送
		public static final int ALLIANCE_FATIGUE_GIVE = 110;
		// 取消申请
		public static final int ALLIANCE_CANCLE_APPLY = 111;
		// 创建队伍
		public static final int ALLIANCE_TEAM_CREATE = 112;
		// 加入队伍
		public static final int ALLIANCE_TEAM_JOIN = 113;
		// 接受任务
		public static final int ALLIANCE_TASK_ACCEPT = 114;
		// 提交任务
		public static final int ALLIANCE_TASK_COMMIT = 115;
		// 工会任务
		public static final int ALLIANCE_INSTANCE_TASK = 116;
		// 工会奖励
		public static final int ALLIANCE_REWARD_TASK = 117;
		// 解散工会
		public static final int ALLIANCE_DISSOLVE_TEAM = 118;
		// 公会贡献值奖励
		public static final int ALLIANCE_CONTRIBUTION_REWARD = 119;
		// 公会驻兵
		public static final int ALLIANCE_BASE_SEND = 120;
		// 公会撤兵
		public static final int ALLIANCE_BASE_RECALL = 121;
		// 雇用奖励
		public static final int ALLIANCE_HIRE_REWARD = 122;

		// 匹配对手
		public static final int PVP_MATCH_TARGET = 201;
		// 结算
		public static final int PVP_SETTLE = 202;
		// 防守记录
		public static final int PVP_RECORD = 203;
		// pvp排行榜
		public static final int PVP_RANK_LIST = 204;
		// pvp周结算
		public static final int PVP_WEAK_REWARD = 205;
		
		/**
		 * 快照管理器消息定义
		 */
		// 上线删除快照数据
		public static int ONLINE_REMOVE_OFFLINE_SNAPSHOT = 202;

		// 玩家统计数据更新
		public static final int STATISTICS_UPDATE = 1001;
		// 玩家收到新邮件
		public static final int MAIL_NEW = 1002;
		// 刷新商店
		public static final int REFRESH_SHOP = 1003;
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
		// 大冒险模块
		public static final int ADVENTURE_MODULE = 13;
		// 抽蛋模块
		public static final int SUMMON_MODULE = 14;
		// PVP
		public static final int PVP_MODULE = 15;
		// 签到
		public static final int SIGNIN_MODULE = 16;

		// 空闲模块(保证在最后)
		public static final int IDLE_MODULE = 100;
	}

	/**
	 * 刷新时间掩码
	 */
	public static class RefreshMask {
		public static final int DAILY = 1 << 0;
		public static final int MONTHLY = 1 << 1;
		public static final int HOLE = 1 << 2;
		public static final int SHOP_NORMAL = 1 << 3;
		public static final int SHOP_ALLIANCE = 1 << 4;
		public static final int SHOP_TOWER = 1 << 5;
		public static final int PVP = 1 << 6;
	}

	public static int[] SysRefreshTime = { 103, 104, 105, 106, 107, 108, 109, PVP_WEAK_REFRESH_TIME_ID};
	public static int[] SysRefreshMask = { 4, 4, 4, 4, 4, 4, 4, 64};

	public static int[] PlayerRefreshTime = { 101, 102, 201, 202, 203, 204,
			205, 206, 207 };
	public static int[] PlayerRefreshMask = { 1, 2, 8, 8, 8, 16, 16, 16, 32 };

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
	public enum EquipStagePunch {
		NONE_STAGE(0), WHITE_STAGE(0), GREEN_STAGE(0), BLUE_STAGE(1), PURPLE_STAGE(
				2), ORANGE_STAGE(3), RED_STAGE(4);

		private int punchCount;

		private EquipStagePunch(int count) {
			this.punchCount = count;
		}

		public int GetCount() {
			return punchCount;
		}
	}

	/**
	 * 宝石类型权重
	 */
	public enum GemTypeWeight {
		NONE_GEM_TYPE(0), FIRST_GEM_TYPE(100), SECOND_GEM_TYPE(200), THIRD_GEM_TYPE(
				300), FORTH_GEM_TYPE(400), FIFTH_GEM_TYPE(500), SIXTH_GEM_TYPE(
				600), SEVENTH_GEM_TYPE(700), EIGHTI_GEM_TYPE(800), NINTH_GEM_TYPE(
				900);

		private int weight;

		private GemTypeWeight(int weight) {
			this.weight = weight;
		}

		public float GetWeight() {
			return weight;
		}
	}

	/**
	 * equip 品级对应的打孔数量
	 */
	public enum AllianceReward {
		FIRST_REWARD(500), SECOND_REWARD(1500), THIRD_REWARD(3000);

		private int rewardCount;

		private AllianceReward(int count) {
			this.rewardCount = count;
		}

		public int GetRewardCount() {
			return rewardCount;
		}
	}

	/**
	 * item 消耗检查内容
	 */
	public static class ConsumeCheckResult {
		public static final int COIN_NOT_ENOUGH = 1;
		public static final int GOLD_NOT_ENOUGH = 2;
		public static final int TOWER_COIN_NOT_ENOUGH = 3;
		public static final int ARENA_COIN_NOT_ENOUGH = 4;
		public static final int FATIGUE_NOT_ENOUGH = 5;
		public static final int LEVEL_NOT_ENOUGH = 6;
		public static final int EXP_NOT_ENOUGH = 7;
		// 道具不足
		public static final int TOOLS_NOT_ENOUGH = 8;
		// 装备不足
		public static final int EQUIP_NOT_ENOUGH = 9;
		// 宠物不足
		public static final int MONSTER_NOT_ENOUGH = 10;
		// 宠物锁定
		public static final int MONSTER_LOCKED = 11;
		// 未入公会
		public static final int NOT_IN_ALLIANCE = 12;
		// 贡献值不足
		public static final int CONTRIBUTION_NOT_ENOUGH = 13;


	}

	/**
	 * item 奖励检查内容
	 */
	public static class AwardCheckResult {
		public static final int COIN_LIMIT = 1;
		public static final int GOLD_LIMIT = 2;
		public static final int TOWER_COIN_LIMIT = 3;
		public static final int ARENA_COIN_LIMIT = 4;
		public static final int FATIGUE_LIMIT = 5;
		public static final int CONTRIBUTION_LIMIT = 6;
	}

	// 循环性
	public static class Cycle {
		public static final int NORMAL_CYCLE = 1;
		public static final int DAILY_CYCLE = 2;
	}

	// 任务类型
	public static class QuestType {
		public static final int STORY_QUEST = 1;
		public static final int DAILY_QUEST = 2;
		public static final int BIOGRAPHY_QUEST = 3;
	}

	// 目标类型
	public static class AllianceQuestType {
		public static final int COIN_QUEST = 1;
		public static final int ITEM_QUEST = 2;
		public static final int INSTANCE_QUEST = 3;
	}

	// 任务目标类型
	public static class QuestGoalType {
		public static final int FREE = 1;
		public static final int LEVEL = 2;
		public static final int INSTANCE_X_STAR_3 = 3;
		public static final int INSTANCE_X_TIMES = 4;
		public static final int INSTANCE_ALL_TIMES = 5;
		public static final int INSTANCE_NORMAL_TIMES = 6;
		public static final int INSTANCE_HARD_TIMES = 7;
		public static final int CHAPTER_X_NORMAL = 8;
		public static final int CHAPTER_X_HARD = 9;
		public static final int ADVENTURE_TIMES = 10;
		public static final int ARENA_TIMES = 11;
		public static final int HOLE_COIN_TIMES = 12;
		public static final int HOLE_EXP_TIMES = 13;
		public static final int MONSTER_STAGE_X_COUNT = 14;
		public static final int MONSTER_LEVEL_X_COUNT = 15;
		public static final int MONSTER_MIX_TIMES = 16;
		public static final int EQUIP_WEAR_STAGE_X_COUNT = 17;
		public static final int EQUIP_SLOT_TIMES = 18;
		public static final int UP_SKILL_TIMES = 19;
		public static final int UP_EQUIP_TIMES = 20;
		public static final int BUY_COIN_TIMES = 21;
		public static final int BUY_GIFT_TIMES = 22;
		public static final int BUY_ITEM_X_TIMES = 23;
		public static final int PAY_DIAMOND_COUNT = 24;
		public static final int USE_FATIGUE_COUNT = 25;
		public static final int USE_DIAMOND_COUNT = 26;
		public static final int USE_ITEM_X_COUNT = 27;
		public static final int INLAY_ALL_TIMES = 28;
		public static final int INLAY_TYPE_X_TIMES = 29;
		public static final int SYN_ALL_COUNT = 30;
		public static final int SYN_TYPE_X_COUNT = 31;
		public static final int EGG_ALL_TIMES = 32;
		public static final int EGG_COIN_TIMES = 33;
		public static final int EGG_COIN_10_TIMES = 34;
		public static final int EGG_DIAMOND_TIMES = 35;
		public static final int EGG_DIAMOND_10_TIMES = 36;
		public static final int CALL_PET_STAGE_X_COUNT = 37;
		public static final int CALL_EQUIP_STAGE_X_COUNT = 38;
		public static final int SOCIETY_JOIN_TIMES = 39;
		public static final int SOCIETY_PRAY_TIMES = 40;
		public static final int SOCIETY_BOSS_TIMES = 41;
		public static final int SOCIETY_FATIGUE_TIMES = 42;
		public static final int SHOP_REFRESH_TIMES = 43;
		public static final int COIN_SOCIETY_COUNT = 44;
		public static final int COIN_TOWER_COUNT = 45;
		public static final int QUEST_X = 46;
		public static final int QUEST_TYPE_X_COUNT = 47;
		public static final int QUEST_CYCLE_X_COUNT = 48;
	}

	// 副本难度
	public static class Instance {
		// 难度
		public static final int NORMAL = 0;
		public static final int HARD = 1;
		// 状态索引
		public static final int STATE_WIN_INDEX = 0;
		public static final int STATE_STAR_INDEX = 1;
		public static final int STATE_ENTER_INDEX = 2;
		// 状态长度
		public static final  int STATE_STORY_SIZE = 3;
		public static final  int STATE_OTHER_SIZE = 1;
	}

	public static class Alliance {
		/*
		 * 0:普通成员, 1:副会长, 2:会长
		 */
		public static final int ALLIANCE_POS_COMMON = 0;
		public static final int ALLIANCE_POS_COPYMAIN = 1;
		public static final int ALLIANCE_POS_MAIN = 2;

		public static final int ALLIANCE_TEC_LEVEL = 1;
		public static final int ALLIANCE_TEC_MEMBER = 2;
		public static final int ALLIANCE_TEC_COIN = 3;
		public static final int ALLIANCE_TEC_EXP = 4;

		public static final int ONE_PAGE_SIZE = 10;
		public static final int NAME_MAX_LENGTH = 12;
		public static final int NOTICE_MAX_LENGTH = 300;
		public static final int MAX_COPYMAIN_COUNT = 2;
		public static final int SEND_FATIGUE_COUNT = 1;

		public static final int BASE_MIN_TIME = 30;

		// 公会贡献值奖励1
		public static final String ALLIANCE_CONTRI_REWARD1 = "110001";
		// 公会贡献值奖励2
		public static final String ALLIANCE_CONTRI_REWARD2 = "110001";
		// 公会贡献值奖励3
		public static final String ALLIANCE_CONTRI_REWARD3 = "110001";
	}

	public static class PVP {
		public static final int PVP_DEFAULT_POINT = 1000;
		public static final int PVP_POOL_STAGE_MAX_SIZE = 100;
		public static final int PVP_POOL_STAGE_INIT_SIZE = 10;
		public static final int PVP_DEFENCE_RECORD_SIZE = 10;
		public static final int PVP_RANK_SIZE = 100;
		public static final float PVP_RERANK_K_VALUE = 5.0f;
	}
	
	// 翻译系统，敏感词选项
	public static class Profanity {
		public static final String OFF = "off";
		public static final String STOP = "stop";
		public static final String CENSOR = "censor";
	}

	// 翻译系统，文本类型
	public static class TextType {
		public static final String CHAT = "chat";
		public static final String MAIL = "mail";
	}

	public static class HoleType {
		public static final int EXP_HOLE = 1;
		public static final int COIN_HOLE = 2;
	}

	public static class SysMail {
		// 新手欢迎邮件
		public static final int WELCOME = 100;
		// 任命会长
		public static final int ALLIANCE_OWNER = 101;
		// 被赠送活力值
		public static final int ALLIANCE_FATIGUE = 102;
		// 被踢出公会
		public static final int ALLIANCE_KICK = 103;
		// 离开公会发放基地奖励
		public static final int ALLIANCE_LEAVE_BASE = 104;
		// 周结算 段位奖励
		public static final int PVP_WAEK_GRADE_REWARD = 105;
		// 月结算 排名奖励
		public static final int PVP_MONTH_RANK_REWARD = 106;
	}

	public static class SysIm {
		// 公会科技升级
		public static final int ALLIANCE_LEVEL_UP = 101;
		// 公会任命会长
		public static final int ALLIANCE_OWNER = 102;
		// 公会成员加入
		public static final int ALLIANCE_JOIN = 103;
		// 公会任命副会长
		public static final int ALLIANCE_ELDER_PROMOTE = 104;
		// 公会解除副会长
		public static final int ALLIANCE_ELDER_DEMOTE = 105;
	}

	public static class summon {
		public static final int FIRST_N_ONE_PSEUDO_REWARD = 1;
		public static final int FIRST_N_TEN_PSEUDO_REWARD = 2;

		public static final int ONE_COIN_PRICE= 100;
		public static final int TEN_COIN_PRICE = 900;
		public static final int ONE_DIAMOND_PRICE = 10;
		public static final int TEN_DIAMOND_PRICE = 90;

		public static final int MAX_COIN_FREE_TIMES_DAILY = 5;
		public static final int MAX_DIAMOND_FREE_TIMES = 1;
		// 免费钻石抽蛋恢复秒数
		public static final int DIAMOND_FREE_TIME = 60;
		// 免费金币抽蛋冷却秒数
		public static final int COIN_FREE_CD = 180;
	}
}
