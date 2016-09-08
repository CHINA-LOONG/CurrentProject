package com.hawk.game.BILog;

public class BIBehaviorAction {

	public static enum BIType {
		/**
		 * 空类型
		 */
		NULL_TYPE,
		/**
		 * 钻石流水
		 */
		GOLD_EVENT,
		/**
		 * 金币流水
		 */
		COIN_EVENT,
		/**
		 * 道具流水
		 */
		ITEM_EVENT,
		/**
		 * 宝石流水
		 */
		GEM_FLOW_EVENT,
		/**
		 * 装备穿戴流水
		 */
		EQUIP_DRESS_EVENT,
		/**
		 * 装备强化流水
		 */
		EQUIP_INTENSITY_EVENT,
		/**
		 * 单据流水
		 */
		ROUND_FLOW_EVENT,
		/**
		 * 体力流水
		 */
		ENERGY_FLOW_EVENT,
		/**
		 * 闯关进度
		 */
		PVE_PROGRESS_EVENT,
		/**
		 * 宠物经验
		 */
		PET_LEVELUP_EVENT,
		/**
		 * 宠物强化经验
		 */
		PET_ENHANCE_EVENT,
		/**
		 * 宠物
		 */
		PET_FLOW_EVENT,
		/**
		 * 宠物技能
		 */
		PET_SKILL_EVENT,
		/**
		 * 任务流水
		 */
		MISSION_EVENT,
		/**
		 * 公会流水
		 */
		ALLIANCE_EVENT,
		/**
		 * 公会玩家流水
		 */
		ALLIANCE_MEMBER_EVENT,
		/**
		 * 角色经验流水
		 */
		PLAYER_LEVELUP_EVENT,
		/**
		 * 通天塔币流水
		 */
		TOWER_COIN_EVENT,
	}

	/**
	 * 行为定义
	 * 
	 * @author zs
	 * @date 2016-01-08
	 */
	public static enum Action {
		// 无明显Action操作的行为
		NULL(0),

		/**
		 * 正常战斗
		 */
		FIGHT(1001),
		/**
		 * 扫荡战斗
		 */
		RAID(1002),
		/**
		 * 复活
		 */
		REVIVE(1003),
		/**
		 * 开始普通副本
		 */
		NORMAL_INSTANCE(1004),
		/**
		 * 开始挑战副本
		 */
		HEROCI_INSTANCE(1005),
		/**
		 * 开始试炼副本
		 */
		HOLE_INSTANCE(1006),
		/**
		 * 开始通天塔
		 */
		TOWER_INSTANCE(1007),
		/**
		 * 开始公会任务副本
		 */
		GUILD_INSTANCE(1008),
		/**
		 * 结算普通副本
		 */
		NORMAL_INSTANCE_REWARD(1009),
		/**
		 * 结算挑战副本
		 */
		HEROCI_INSTANCE_REWARD(1010),
		/**
		 * 结算金钱试炼副本
		 */
		COIN_HOLE_INSTANCE_REWARD(1011),
		/**
		 * 结算经验试炼副本
		 */
		EXP_HOLE_INSTANCE_REWARD(1012),
		/**
		 * 结算通天塔
		 */
		TOWER_INSTANCE_REWARD(1013),
		/**
		 * 结算公会任务副本
		 */
		GUILD_INSTANCE_REWARD(1014),
		/**
		 * 重置副本次数
		 */
		INSTANCE_RESET(1015),
		/**
		 * 副本章节奖励
		 */
		INSTANCE_STAGE_REWARD(1016),

		/**
		 * 宠物升级
		 */
		MONSTER_LEVEL_UP(2001),
		/**
		 * 宠物强化进阶
		 */
		MONSTER_ADVANCE(2002),
		/**
		 * 宠物升级技能
		 */
		MONSTER_ABILITY_LEVEL_UP(2003),
		/**
		 * 宠物分解
		 */
		MONSTER_DISENCHANT(2004),
		/**
		 * 宠物合成图鉴
		 */
		MONSTER_SUMMON(2005),
		/**
		 * 宠物进化
		 */
		MONSTER_EVOLVE(2006),
		/**
		 * 穿装备
		 */
		MONSTER_ADD_EQUIP(2007),
		/**
		 * 脱装备
		 */
		MONSTER_REMOVE_EQUIP(2008),
		/**
		 * 技能点购买
		 */
		MONSTER_ABILITY_POINT(2009),

		/**
		 * 角色升级
		 */
		PLAYER_LEVEL_UP(3001),
		/**
		 * 自然体力恢复
		 */
		ENERGY_RECOVER(3002),

		/**
		 * 任务获得
		 */
		MISSION_REWARD(4001),

		/**
		 * 出售道具
		 */
		ITEM_SELL(4001),
		/**
		 * 使用宝箱
		 */
		BOX_USE(4002),
		/**
		 * 使用扫荡券
		 */
		RAID_TICKET_USE(4003),
		/**
		 * 使用钥匙 
		 */
		KEY_USE(4004),
		/**
		 * 使用双倍经验药
		 */
		EXP_SCROLL_USE(4005),
		/**
		 * 使用活力道具
		 */
		ENERGY_COOKIES_USE(4006),
		/**
		 * 宠物使用经验道具
		 */
		EXP_POTION_USE(4007),
		/**
		 * 宝石合成
		 */
		GEM_COMBINE(4008),
		/**
		 * 材料合成
		 */
		MATERIAL_COMBINE(4009),

		/**
		 * 装备强化
		 */
		EQUIP_FORGE(6001),
		/**
		 * 装备进阶
		 */
		EQUIP_ADVANCE(6002),
		/**
		 * 装备开孔
		 */
		EQUIP_OPEN_SLOTS(6003),
		/**
		 * 装备分解
		 */
		EQUIP_DISENCHANT(6004),
		/**
		 * 装备锁孔
		 */
		EQUIP_LOCK_SLOT(6005),
		/**
		 * 镶嵌宝石
		 */
		EQUIP_GEM(6006),

		/**
		 * 邮件获得
		 */
		MAIL_REWARD(7001),

		/**
		 * 商城充值
		 */
		STORE_RECHARGE(8001),
		/**
		 * 钻石兑换金币
		 */
		COIN_CHANGE(8002),
		/**
		 * 商城购买
		 */
		STORE_BUY(8003),
		/**
		 * 普通商店购买
		 */
		NORMAL_SHOP_BUY(8004),
		/**
		 * 公会商店购买
		 */
		GUILD_SHOP_BUY(8005),
		/**
		 * 通天塔商店购买
		 */
		TOWER_SHOP_BUY(8006),
		/**
		 * 刷新商店
		 */
		SHOP_REFRESH(8007),
		/**
		 * 购买活力
		 */
		ENERGY_BUY(8008),
		/**
		 * 购买钥匙
		 */
		KEY_BUY(8009),
		/**
		 * 购买扫荡劵
		 */
		RAID_TICKEY_BUY(8010),

		/**
		 * 大冒险奖励
		 */
		ADVENTURE_REWARD(9001),
		/**
		 * 大冒险加速
		 */
		ADVENTURE_COMPLETE_NOW(9002),
		/**
		 * 大冒险购买列队
		 */
		ADVENTURE_ADD_TEAM(9003),
		/**
		 * 大冒险雇佣宠物
		 */
		ADVENTURE_HIRE_MONSTER(9004),
		/**
		 * 大冒险购买刷新次数
		 */
		ADVENTURE_BUY_REFRESH(9005),

		/**
		 * 创建公会
		 */
		GUILD_CREATE(10001),
		/**
		 * 批准加入公会
		 */
		GUILD_JOIN_HANDLE(10002),
		/**
		 * 自动加入公会
		 */
		GUILD_JOIN_AUTO(10003),
		/**
		 * 退出公会
		 */
		GUILD_LEAVE(10004),
		/**
		 * 转让会长
		 */
		GUILD_MAIN_TRANSFER(10005),
		/**
		 * 任命副会长
		 */
		GUILD_COPY_MAIN_ADD(10006),
		/**
		 * 解除副会长
		 */
		GUILD_COPY_MAIN_REMOVE(10007),
		/**
		 * 请离公会
		 */
		GUILD_KICK(10008),
		/**
		 * 公会许愿
		 */
		GUILD_PRAY(10009),
		/**
		 * 公会任务奖励
		 */
		GUILD_TASK_REWARD(10010),
		/**
		 * 公会任务开启
		 */
		GUILD_TASK_OPEN(10011),
		/**
		 * 公会任务放弃返还
		 */
		GUILD_TASK_GIVEUP(10012),
		/**
		 * 公会科技
		 */
		GUILD_TECH_LEVELUP(10013),
		/**
		 * 公会每日贡献奖励
		 */
		GUILD_CONTRIBUTION_REWARD(10014),
		/**
		 * 公会基地助战
		 */
		GUILD_BASE_REWARD(10015),
		/**
		 * 公会子任务
		 */
		GUILD_SUB_MISSION(10016),
		/**
		 * 加入任务
		 */
		GUILD_JOIN_TEAM(10017),

		/**
		 * 抽蛋
		 */

		/**
		 * 签到
		 */
		DAILY_SIGN(12001),

		/**
		 * GM 命令
		 */
		GM(100000);

		/**
		 * 构造函数
		 */
		Action(int code){
			this.code = code;
		}

		/**
		 * BI code
		 */
		private int code;

		/**
		 * 获取BI code 值
		 * @return
		 */
		public int getBICode(){
			return code;
		}

	}
}
