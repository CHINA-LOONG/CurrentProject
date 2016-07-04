package com.hawk.game.util;

/**
 * 游戏常量定义
 * 
 * @author hawk
 */
public class GsConst {
	// 刷新间隔帧
	public static final int REFRESH_PERIOD = 100;

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
		public static int ITEM_MODULE = 5;
		// 道具模块
		public static int EQUIP_MODULE = 6;
		
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
		public static final int SIGN_IN_PERS_REFRESH = 101;
		public static final int INSTANCE_PERS_REFRESH = 102;
		public static final int PERS_REFRESH_END = 103;
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

}
