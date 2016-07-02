package com.hawk.game.util;

/**
 * 游戏常量定义
 * 
 * @author hawk
 */
public class GsConst {
	/**
	 * 对象类型
	 * 
	 * @author hawk
	 */
	public static class ObjType {
		// 玩家对象
		public static int PLAYER = 1;
		// 应用程序
		public static int MANAGER = 100;
	}

	/**
	 * 系统对象id
	 * 
	 * @author hawk
	 */
	public static class ObjId {
		// 应用程序
		public static int APP = 100;
	}

	/**
	 * 消息定义
	 */
	public static class MsgType {
		// 连接断开
		public static int SESSION_CLOSED = 2;
		// 玩家上线
		public static int PLAYER_LOGIN = 3;
		// 玩家初始化完成
		public static int PLAYER_ASSEMBLE = 4;
	}

	/**
	 * 模块定义
	 */
	public static class ModuleType {
		// 登陆模块
		public static int LOGIN_MODULE = 1;
		// 怪物模块
		public static int MONSTER_MODULE = 2;
		
		// 空闲模块(保证在最后)
		public static int IDLE_MODULE = 100;
	}

	/**
	 * 属性类型定义
	 */
	public static class PropertyType {
		// 金木水火土
		public static int JIN_PROPERTY = 1;
		public static int MU_PROPERTY = 2;
		public static int SHUI_PROPERTY = 3;
		public static int HUO_PROPERTY = 4;
		public static int TU_PROPERTY = 5;
	}
	
}
