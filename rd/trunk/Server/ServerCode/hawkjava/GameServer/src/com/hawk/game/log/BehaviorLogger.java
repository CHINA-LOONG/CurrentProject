package com.hawk.game.log;

import net.sf.json.JSONObject;

import org.apache.log4j.Logger;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;

import com.hawk.game.GsConfig;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.player.Player;

/**
 * 行为日志
 * 
 * @author hawk
 * @date 2014-7-24
 */
public class BehaviorLogger {
	/**
	 * 日志源
	 * 
	 * @author hawk
	 * @date 2014-7-25
	 */
	public static enum Source {
		/**
		 * 用户操作
		 */
		USER_OPERATION,
		/**
		 * 系统操作
		 */
		SYS_OPERATION,
		/**
		 * GM操作
		 */
		GM_OPERATION,
		/**
		 * 武将移除
		 */
		ROLE_REMOVE,
		/**
		 * 角色添加
		 */
		ROLE_ADD,
		/**
		 * 装备移除
		 */
		EQUIP_REMOVE,
		/**
		 * 装备增加
		 */
		EQUIP_ADD,
		/**
		 * 道具移除
		 */
		TOOLS_REMOVE,
		/**
		 * 道具增加
		 */
		TOOLS_ADD,
		/**
		 * 属性改变
		 */
		PLAYER_ATTR_CHANGE,
		/**
		 * 钻石消耗
		 */
		GOLD_REDUCE,
		/**
		 * 游戏奖励
		 */
		GAME_SYS_REWARD,
		/**
		 * 邮件添加
		 */
		EMAIL_ADD,
		/**
		 * 邮件读取
		 */
		EMAIL_REMOVE,
		
		/**
		 * 未知源
		 */
		UNKNOWN_SOURCE;
	}

	/**
	 * 行为定义
	 * 
	 * @author hawk
	 * @date 2014-2-24
	 */
	public static enum Action {
		/**
		 * 无明显Action操作的行为
		 */
		NULL,
		/**
		 * 初始化数据
		 */
		INIT,
		/**
		 * 系统行为
		 */
		SYSTEM,
		/**
		 * 登录游戏
		 */
		LOGIN_GAME,
		/**
		 * 登出游戏
		 */
		LOGOUT_GAME,
		/**
		 * 每日重置
		 */
		DAILY_RESET,
		/**
		 * 创建角色
		 */
		CREATE_ROLE,
		/**
		 * 修改姓名
		 */
		ROLE_RENAME,
		/**
		 * 角色升级
		 */
		ROLE_LEVEL_UP,
		/**
		 * 首次充值
		 */
		FIRST_RECHARGE,
		/**
		 * 充值
		 */
		RECHARGE,
		/**
		 * 购买道具
		 */
		BUY_TOOL,
		/**
		 * 使用道具
		 */
		TOOL_USE,
		/**
		 * 出售道具
		 */
		TOOL_SELL,
		/**
		 * 刷新神秘商店
		 */
		REFRESH_SHOP,
		/**
		 * 神秘商店购买
		 */
		SHOP_BUY,
		/**
		 * 装备锻造
		 */
		EQUIP_FORGE,
		/**
		 * 装备出售
		 */
		EQUIP_SELL,
		/**
		 * 装备洗炼
		 */
		EQUIP_WASH,
		/**
		 * 装备强化
		 */
		EQUIP_EHANCE,
		/**
		 * 扩充装备包囊
		 */
		EXT_EQUIP_BAG,
		/**
		 * 传书领取 邮件
		 */
		SYS_MSG,
		/**
		 * GM发放奖励
		 */
		GM_AWARD,
		/**
		 * CDK兑换
		 */
		CDK_REWARD,
		/**
		 * 每日首登
		 */
		DAILY_FIRST_LOGIN,
		/**
		 * 后台充值
		 */
		GM_RECHARGE,
		/**
		 * 踢出玩家
		 */
		GM_KICKOUT,
		/**
		 * GM封号处理
		 */
		GM_FORBIDEN,
		/**
		 * 钻石消耗
		 */
		GOLD_COST,
		/**
		 * 金币消耗
		 */
		COIN_COST,
		/**
		 * 使用cdk
		 */
		USE_CDK,
		/**
		 * 游戏奖励
		 */
		GAME_SYS_REWARD,

		/**
		 * 角色商城刷新
		 */
		SHOP_REFASH,

		/**
		 * 角色商城金币购买
		 */
		SHOP_BUY_COIN,
		/**
		 * 角色商城钻石购买
		 */
		SHOP_BUY_GOLD,

		/**
		 * 角色商城购买道具
		 */
		SHOP_BUY_TOOLS,

		/**
		 * 邮件奖励道具
		 */
		MAIL_REWARD_TOOLS,

		/**
		 * 创建邮件
		 */
		EMAIL_CREATE,

		/**
		 * 用钻石购买金币
		 */
		SHOP_COIN_BUY,
		/**
		 * 公会签到得金币
		 */
		ALLIANCE_REPORT_COIN,
		/**
		 * 公会签到得贡献
		 */
		ALLIANCE_REPORT_CONTRIBUTION,
		/**
		 * 公会签到得钻石
		 */
		ALLIANCE_REPORT_GOLD,
		/**
		 * 公会操作
		 */
		ALLIANCE_REPORT_OPER,
		/**
		 * 公会开启BOSS
		 */
		ALLIANCE_OPEN_BOSS,
		/**
		 * 鼓舞扣除钻石
		 */
		ALLIANCE_CONSUME_ADD_PROP,
		/**
		 * 公会商店消耗贡献
		 */
		ALLIANCE_CONSUME_CONTRIBUTION,
		/**
		 * 公会创建消耗
		 */
		ALLIANCE_CREATE_CONSUME,
		/**
		 * 加入公会
		 */
		ALLIANCE_JOIN_ALLIANCE,
		/**
		 * 自动参加公会boss战扣钻
		 */
		ALLIANCE_BOSS_AUTO_JOIN,
		/**
		 * 加入公会boss战
		 */
		JOIN_ALLIANCE_BOSS,
		/**
		 * 装备神器吞噬
		 */
		EQUIP_SWALLOW,
		/**
		 * 装备打造刷新
		 */
		EQUIP_SMELT_REFRESH,
		/**
		 * 装备熔炼
		 */
		EQUIP_SMELT,

		/**
		 * 装备打孔
		 */
		EQUIP_PUNCH,

		/**
		 * 购买竞技场挑战次数
		 */
		BUY_ARENA_CHALLENGE_TIMES,
		/**
		 * 刷新竞技场对手列表
		 */
		ARENA_REFRESH_OPPONENT_LIST,
		/**
		 * 竞技场挑战
		 */
		ARENA_CHALLENGE,

		/**
		 * 装备打造
		 */
		EQUIP_CREATE,
		/**
		 * 神器传承
		 */
		EQUIP_EXTEND,

		/**
		 * 任务奖励
		 */
		MISSION_BONUS,

		/**
		 * 角色洗炼
		 */
		ROLE_BAPTIZE,
		/**
		 * 装备背包扩充
		 */
		EQUIP_BAG_EXTEND,
		/**
		 * 宝石镶嵌
		 */
		EQUIP_STONE_DRESS,
		/**
		 * 购买快速战斗
		 */
		BUY_FAST_FIGHT_TIMES,
		/**
		 * 购买boss挑战次数
		 */
		BUY_BOSS_FIGHT_TIMES,
		/**
		 * 购买精英副本挑战次数
		 */
		BUY_ELITE_MAP_TIMES,
		/**
		 * 离线结算
		 */
		OFFLINE_ACCOUNT,
		/**
		 * 快速战斗
		 */
		FAST_FIGHTING,
		/**
		 * 地图战斗
		 */
		MONSTER_FIGHTING,
		/**
		 * 地图Boss战斗
		 */
		BOSS_FIGHTING,
		/**
		 * 经验副本战斗
		 */
		ELITE_MAP_FIGHTING,
		/**
		 * 竞技场战斗
		 */
		ARENA_FIGHTING,
		/**
		 * 团战创建队伍
		 */
		TEAM_BATTLE_CREATE_TEAM,
		/**
		 * 团战T人
		 */
		TEAM_BATTLE_KICK_UP_MEMBER,
		/**
		 * 参加团战(报名＋创建队伍)
		 */
		TAKE_PART_IN_TEAM_BATTLE,
		/**
		 * 取消团战报名
		 */
		CANCEL_TEAM_BATTLE,
		/**
		 * 参加阵营战斗
		 */
		TAKE_PART_IN_CAMPWAR,
		/**
		 * 领取礼物
		 */
		FETCH_GIFT,
		/**
		 * 读取邮件
		 */
		EMAIL_READ,
		/**
		 * 购买月卡
		 */
		MONTH_CARD_BUY,
		/**
		 * 月卡领取奖励
		 */
		MONTH_CARD_REWARD,
		/**
		 * 累计充值奖励
		 */
		ACC_RECHARGE_AWARDS,
		/**
		 * 连续充值奖励
		 */
		CONTINUE_RECHARGE_AWARDS,
		/**
		 * 累计消费奖励
		 */
		ACC_CONSUME_AWARDS,
		/**
		 * 换装
		 */
		EQUIP_DRESS,
		/**
		 * 卸下
		 */
		EQUIP_UNDRESS,
		/**
		 * 装备技能
		 */
		SKILL_CARRAY,
		/**
		 * 道具出售
		 */
		ITEM_SELL,
		/**
		 * 中秋换字兑换
		 */
		WORDS_EXCHANGE,
		/**
		 * 公测字兑换
		 */
		WORDS_EXCHANGE_SPECIAL,
		/**
		 * 公测字回收
		 */
		WORDS_EXCHANGE_SPECIAL_CYCLE,
		/**
		 * boss扫荡
		 */
		BOSS_WIPE,
		/**
		 * 经验副本扫荡
		 */
		ELITE_MAP_WIPE,
		/**
		 * 战斗奖励
		 */
		BATTLE_REWARD,
		/**
		 * 荣誉商店刷新
		 */
		HONOR_SHOP_REFRESH,
		/**
		 * 荣誉商店购买
		 */
		HONOR_SHOP_BUY,
		/**
		 * 特殊神器打造
		 */
		EQUIP_SPECIAL_CREATE,
		/**
		 * 神器合成
		 */
		EQUIP_COMPOUND,
		/**
		 * 阵营战鼓舞
		 */
		CAMPWAR_INSPIRE,
		/**
		 * 开宝箱
		 */
		OPEN_TREASURE,
		/**
		 * 加入阵营战
		 */
		JOIN_CAMPWAR,
		/**
		 * 自动投资阵营战
		 */
		AUTO_CAMPWAR,
		/**
		 * 熔炼打造装备刷新
		 */
		SMELT_EQUIP_REFRESH,
		/**
		 * 勾选自动加入boss战
		 */
		AUTO_BOSS_FIGHTING,
		/**
		 * 公会经验增加, 公会Boss获胜
		 */
		ALLIANCE_EXP_ADD,
		/**
		 * 每日单笔充值返利
		 */
		SINGLE_RECHARGE,
		/**
		 * 每日单笔充值奖励
		 */
		SINGLE_RECHARGE_AWARDS,
		/**
		 * 充值返利活动每日返利
		 */
		RECHARGE_REBATE_EVERYDAY_AWARDS,
		/**
		 * 称号改变
		 */
		TITLE_CHANGE,
		/**
		 * 周卡奖励
		 */
		WEEK_CARD_REWARD,
		/**
		 * 周卡每日奖励
		 */
		WEEK_CARD_DAILY_REWARD,
		/**
		 * 领取vip福利
		 */
		VIP_WELFARE_REWARD,
		/**
		 * 增加升星经验
		 */
		INC_STAR_EXP,
		/**
		 * 光环激活
		 */
		ROLE_RING_ACTIVE,
		/**
		 * 远征物资道具使用
		 */
		EXPEDITION_ITEM_USE,
		/**
		 * 经验副本地图
		 */
		ELITE_MAP,
		/**
		 * 未知行为
		 */
		UNKONWN_ACTION;

		/**
		 * 行为名
		 */
		private String actionName;

		/**
		 * 获取行为名
		 * 
		 * @return
		 */
		public String getActionName() {
			return actionName;
		}

		/**
		 * 构造函数
		 */
		private Action() {
			this("");
		}

		/**
		 * 构造函数
		 */
		private Action(String value) {
			this.actionName = value;
		}
	}

	/**
	 * 日志参数
	 * 
	 * @author hawk
	 * @date 2014-7-24
	 */
	public static class Params {
		private String name;
		private Object value;

		public static final String AFTER = "after";

		public static final String COST = "cost";

		public static Params valueOf(String name, Object value) {
			Params params = new Params();
			params.setName(name);
			params.setValue(value);
			return params;
		}

		public String getName() {
			return name;
		}

		public void setName(String name) {
			this.name = name;
		}

		public Object getValue() {
			return value;
		}

		public void setValue(Object value) {
			this.value = value;
		}
	}

	/**
	 * GM日志记录器
	 */
	private static final Logger GM_LOGGER = Logger.getLogger("GM");
	/**
	 * 行为日志，数据流变化日志记录器
	 */
	private static final Logger ACTION_LOGGER = Logger.getLogger("Action");
	/**
	 * 统计平台日志记录器
	 */
	private static final Logger PLATFORM_LOGGER = Logger.getLogger("Platform");

	/**
	 * 用于数据平台统计的日志输出
	 * 
	 * @param player
	 * @param action
	 * @param params
	 */
	public static void log4Platform(Player player, Action action, Params... params) {
		try {
			JSONObject jsonObject = new JSONObject();
			// 行为时间
			jsonObject.put("ts", HawkTime.getTimeString());
			// 平台用户Id
			jsonObject.put("puid", player.getPuid());
			// 设备Id
			jsonObject.put("device_id", player.getDevice());
			// 角色Id
			jsonObject.put("player_id", player.getId());
			// 场景，先留空
			jsonObject.put("scene", "scene");
			// 等级
			jsonObject.put("level", player.getLevel());
			// vip等级
			jsonObject.put("vip_level", player.getVipLevel());
			// 动作编号
			jsonObject.put("action", action.name());
			// 填充参数V1
			JSONObject paramsJsonObject = new JSONObject();
			for (Params param : params) {
				paramsJsonObject.put(param.getName(), param.getValue().toString());
			}
			jsonObject.put("v1", paramsJsonObject.toString());
			jsonObject.put("v2", "");
			jsonObject.put("v3", "");
			jsonObject.put("v4", "");

			// 渠道信息
			jsonObject.put("ip", player.getIp());
			jsonObject.put("gameid", GsConfig.getInstance().getGameId());
			jsonObject.put("server", GsConfig.getInstance().getPlatform() + "-" + GsConfig.getInstance().getServerId());
			jsonObject.put("store", player.getPlatform());
			// 手机信息
			jsonObject.put("phoneinfo", player.getPhoneInfo());

			PLATFORM_LOGGER.info(jsonObject.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 用于数据平台统计的日志输出
	 * 
	 * @param playerEntity
	 * @param action
	 * @param params
	 */
	public static void log4Platform(PlayerEntity playerEntity, Action action, Params... params) {
		try {
			JSONObject jsonObject = new JSONObject();
			// 行为时间
			jsonObject.put("ts", HawkTime.getTimeString());
			// 平台用户Id
			jsonObject.put("puid", playerEntity.getPuid());
			// 设备Id
			jsonObject.put("device_id", playerEntity.getDevice());
			// 角色Id
			jsonObject.put("player_id", playerEntity.getId());
			// 场景，先留空
			jsonObject.put("scene", "scene");
			// 等级
			jsonObject.put("level", playerEntity.getLevel());
			// vip等级
			jsonObject.put("vip_level", playerEntity.getVipLevel());
			// 动作编号
			jsonObject.put("action", action.name());
			// 填充参数V1
			JSONObject paramsJsonObject = new JSONObject();
			for (Params param : params) {
				paramsJsonObject.put(param.getName(), param.getValue().toString());
			}
			jsonObject.put("v1", paramsJsonObject.toString());
			jsonObject.put("v2", "");
			jsonObject.put("v3", "");
			jsonObject.put("v4", "");

			// 渠道信息
			jsonObject.put("ip", "");
			jsonObject.put("gameid", GsConfig.getInstance().getGameId());
			jsonObject.put("server", GsConfig.getInstance().getPlatform() + "-" + GsConfig.getInstance().getServerId());
			jsonObject.put("store", playerEntity.getPlatform());
			// 手机信息
			jsonObject.put("phoneinfo", playerEntity.getPhoneInfo());

			PLATFORM_LOGGER.info(jsonObject.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 数据流统计，主要用户客服问题查询
	 * 
	 * @param player
	 * @param source
	 * @param action
	 */
	public static void log4Service(Player player, Source source, Action action, Params... params) {
		try {
			JSONObject jsonObject = new JSONObject();
			// 行为时间
			jsonObject.put("puid", player.getPuid());
			jsonObject.put("playerId", player.getId());
			jsonObject.put("playerName", player.getName());
			jsonObject.put("source", source.name());
			jsonObject.put("action", action.name());

			JSONObject paramsJsonObject = new JSONObject();
			for (Params param : params) {
				paramsJsonObject.put(param.getName(), param.getValue().toString());
			}
			jsonObject.put("data", paramsJsonObject);

			ACTION_LOGGER.info(jsonObject.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	/**
	 * 数据流统计，主要用户客服问题查询
	 * 
	 * @param player
	 * @param source
	 * @param action
	 */
	public static void log4Service(PlayerEntity playerEntity, Source source, Action action, Params... params) {
		try {
			JSONObject jsonObject = new JSONObject();
			// 行为时间
			jsonObject.put("puid", playerEntity.getPuid());
			jsonObject.put("playerId", playerEntity.getId());
			jsonObject.put("playerName", playerEntity.getNickname());
			jsonObject.put("source", source.name());
			jsonObject.put("action", action.name());

			JSONObject paramsJsonObject = new JSONObject();
			for (Params param : params) {
				paramsJsonObject.put(param.getName(), param.getValue().toString());
			}
			jsonObject.put("data", paramsJsonObject);

			ACTION_LOGGER.info(jsonObject.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 以玩家id为key统计
	 * 
	 * @param playerId
	 * @param source
	 * @param action
	 * @param params
	 */
	public static void log4Service(int playerId, Source source, Action action, Params... params) {
		try {
			JSONObject jsonObject = new JSONObject();
			// 行为时间
			jsonObject.put("playerId", playerId);
			jsonObject.put("source", source.name());
			jsonObject.put("action", action.name());

			JSONObject paramsJsonObject = new JSONObject();
			for (Params param : params) {
				paramsJsonObject.put(param.getName(), param.getValue().toString());
			}
			jsonObject.put("data", paramsJsonObject);

			ACTION_LOGGER.info(jsonObject.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * GM统计，主要记录GM操作
	 * 
	 * @param user
	 * @param source
	 * @param action
	 */
	public static void log4GM(String user, Source source, Action action, Params... params) {
		try {
			JSONObject jsonObject = new JSONObject();
			// 行为时间
			jsonObject.put("user", user);
			jsonObject.put("source", source.name());
			jsonObject.put("action", action.name());

			JSONObject paramsJsonObject = new JSONObject();
			for (Params param : params) {
				paramsJsonObject.put(param.getName(), param.getValue().toString());
			}
			jsonObject.put("data", paramsJsonObject);

			GM_LOGGER.info(jsonObject.toString());
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

}
