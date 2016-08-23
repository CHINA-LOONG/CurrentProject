package com.hawk.game.player;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.app.HawkApp;
import org.hawk.app.HawkAppObj;
import org.hawk.app.HawkObjModule;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.service.HawkServiceProxy;
import org.hawk.util.services.HawkAccountService;
import org.hawk.xid.HawkXID;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.MonsterBaseCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.PlayerAttrCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.log.BehaviorLogger;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.log.BehaviorLogger.Params;
import com.hawk.game.log.BehaviorLogger.Source;
import com.hawk.game.module.PlayerAllianceModule;
import com.hawk.game.module.PlayerEquipModule;
import com.hawk.game.module.PlayerIdleModule;
import com.hawk.game.module.PlayerImModule;
import com.hawk.game.module.PlayerInstanceModule;
import com.hawk.game.module.PlayerItemModule;
import com.hawk.game.module.PlayerLoginModule;
import com.hawk.game.module.PlayerMailModule;
import com.hawk.game.module.PlayerMonsterModule;
import com.hawk.game.module.PlayerQuestModule;
import com.hawk.game.module.PlayerSettingModule;
import com.hawk.game.module.PlayerShopModule;
import com.hawk.game.module.PlayerStatisticsModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.HS.sys;
import com.hawk.game.protocol.SysProtocol.HSErrorCode;
import com.hawk.game.protocol.SysProtocol.HSKickPlayer;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.MailUtil.MailInfo;
import com.hawk.game.util.TimeUtil;

/**
 * 玩家对象
 * 
 * @author hawk
 * 
 */
public class Player extends HawkAppObj {
	/**
	 * 协议日志记录器
	 */
	private static final Logger logger = LoggerFactory.getLogger("Protocol");

	/**
	 * 挂载玩家数据管理集合
	 */
	private PlayerData playerData;

	/**
	 * 组装状态
	 */
	private boolean assembleFinish;

	/**
	 * 构造函数
	 * 
	 * @param xid
	 */
	public Player(HawkXID xid) {
		super(xid);

		initModules();

		playerData = new PlayerData(this);
	}

	/**
	 * 初始化模块
	 * 
	 */
	public void initModules() {
		registerModule(GsConst.ModuleType.LOGIN_MODULE, new PlayerLoginModule(this));
		registerModule(GsConst.ModuleType.STATISTICS_MODULE, new PlayerStatisticsModule(this));
		registerModule(GsConst.ModuleType.SETTING_MODULE, new PlayerSettingModule(this));
		registerModule(GsConst.ModuleType.MONSTER_MODULE, new PlayerMonsterModule(this));
		registerModule(GsConst.ModuleType.INSTANCE_MODULE, new PlayerInstanceModule(this));
		registerModule(GsConst.ModuleType.ITEM_MODULE, new PlayerItemModule(this));
		registerModule(GsConst.ModuleType.EQUIP_MODULE, new PlayerEquipModule(this));
		registerModule(GsConst.ModuleType.QUEST_MODULE, new PlayerQuestModule(this));
		registerModule(GsConst.ModuleType.MAIL_MODULE, new PlayerMailModule(this));
		registerModule(GsConst.ModuleType.IM_MODULE, new PlayerImModule(this));
		registerModule(GsConst.ModuleType.SHOP_MODULE, new PlayerShopModule(this));
		registerModule(GsConst.ModuleType.ALLIANCE_MODULE, new PlayerAllianceModule(this));

		// 最后注册空闲模块, 用来消息收尾处理
		registerModule(GsConst.ModuleType.IDLE_MODULE, new PlayerIdleModule(this));
	}

	/**
	 * 获取玩家数据
	 * 
	 * @return
	 */
	public PlayerData getPlayerData() {
		return playerData;
	}

	/**
	 * 获取玩家实体
	 * 
	 * @return
	 */
	public PlayerEntity getEntity() {
		return playerData.getPlayerEntity();
	}

	/**
	 * 是否组装完成
	 * 
	 * @return
	 */
	public boolean isAssembleFinish() {
		return assembleFinish;
	}

	/**
	 * 设置组装完成状态
	 * 
	 * @param assembleFinish
	 */
	public void setAssembleFinish(boolean assembleFinish) {
		this.assembleFinish = assembleFinish;
	}

	/**
	 * 通知错误码
	 * 
	 * @param errCode
	 */
	public void sendError(int hsCode, int errCode) {
		HSErrorCode.Builder builder = HSErrorCode.newBuilder();
		builder.setHsCode(hsCode);
		builder.setErrCode(errCode);
		sendProtocol(HawkProtocol.valueOf(HS.sys.ERROR_CODE, builder));
	}

	/**
	 * 通知错误码
	 * 
	 * @param errCode
	 */
	public void sendError(int hsCode, int errCode, int errFlag) {
		HSErrorCode.Builder builder = HSErrorCode.newBuilder();
		builder.setHsCode(hsCode);
		builder.setErrCode(errCode);
		builder.setErrFlag(errFlag);
		sendProtocol(HawkProtocol.valueOf(HS.sys.ERROR_CODE, builder));
	}

	/**
	 * 发送协议
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean sendProtocol(HawkProtocol protocol) {
		if (protocol.getSize() >= 2048) {
			logger.info("send protocol size overflow, protocol: {}, size: {}", new Object[] { protocol.getType(), protocol.getSize() });
		}
		return super.sendProtocol(protocol);
	}

	/**
	 * 踢出玩家
	 * @param reason
	 */
	public void kickout(int reason) {
		HSKickPlayer.Builder builder = HSKickPlayer.newBuilder();
		builder.setReason(reason);
		HawkProtocol protocol = HawkProtocol.valueOf(sys.KICK_PLAYER_VALUE, builder);
		sendProtocol(protocol);
		session.setAppObject(null);
		session = null;
	}
	/**
	 * 玩家消息预处理
	 * 
	 * @param msg
	 * @return
	 */
	private boolean onPlayerMessage(HawkMsg msg) {
		// 优先服务拦截
		if (HawkServiceProxy.onMessage(this, msg)) {
			return true;
		}

		// 系统级消息, 所有模块都进行处理的消息
		if (msg.getMsg() == GsConst.MsgType.PLAYER_LOGIN) {
			onLogin();

			for (Entry<Integer, HawkObjModule> entry : objModules.entrySet()) {
				PlayerModule playerModule = (PlayerModule) entry.getValue();
				playerModule.onPlayerLogin();
			}
			return true;
		} else if (msg.getMsg() == GsConst.MsgType.PLAYER_ASSEMBLE) {
			for (Entry<Integer, HawkObjModule> entry : objModules.entrySet()) {
				PlayerModule playerModule = (PlayerModule) entry.getValue();
				playerModule.onPlayerAssemble();
			}
			return true;
		} else if (msg.getMsg() == GsConst.MsgType.SESSION_CLOSED) {
			if (isAssembleFinish()) {
				for (Entry<Integer, HawkObjModule> entry : objModules.entrySet()) {
					PlayerModule playerModule = (PlayerModule) entry.getValue();
					playerModule.onPlayerLogout();
				}
			}
			return true;
		} else if (msg.getMsg() == GsConst.MsgType.PLAYER_RECONNECT) {
			for (Entry<Integer, HawkObjModule> entry : objModules.entrySet()) {
				PlayerModule playerModule = (PlayerModule) entry.getValue();
				playerModule.onPlayerReconnect(msg);
			}
			return true;
		}
		return false;
	}

	/**
	 * 玩家协议预处理
	 * 
	 * @param protocol
	 * @return
	 */
	private boolean onPlayerProtocol(HawkProtocol protocol) {
		// 优先服务拦截
		if (HawkServiceProxy.onProtocol(this, protocol)) {
			return true;
		}

		// 玩家不在线而且不是登陆协议(非法协议时机)
		if (!isOnline() && !(protocol.checkType(HS.code.LOGIN_C) || protocol.checkType(HS.code.SYNCINFO_C))) {
			HawkLog.errPrintln(String.format("player is offline, session: %s, protocol: %d", protocol.getSession().getIpAddr(), protocol.getType()));
			return true;
		}

		// 玩家未组装完成
		if (!isAssembleFinish() && !(protocol.checkType(HS.code.LOGIN_C) || protocol.checkType(HS.code.SYNCINFO_C))) {
			//HawkLog.errPrintln(String.format("player assemble unfinish, session: %s, protocol: %d", protocol.getSession().getIpAddr(), protocol.getType()));
			//return true;
		}

		return false;
	}

	/**
	 * 帧更新
	 */
	@Override
	public boolean onTick(long tickTime) {
		// 玩家未组装完成直接不走时钟tick机制
		if (!isAssembleFinish()) {
			return true;
		}

		return super.onTick(tickTime);
	}

	public boolean onRefresh(long refreshTime) {
		// 玩家未组装完成直接不走刷新机制
		if (!isAssembleFinish()) {
			return true;
		}

		onPlayerRefresh(refreshTime, false);
		return super.onRefresh(refreshTime);
	}

	/**
	 * 消息响应
	 * 
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage(HawkMsg msg) {
		if (onPlayerMessage(msg)) {
			return true;
		}
		return super.onMessage(msg);
	}

	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol) {
		HawkLog.logPrintln(String.format("player: %d on protocol: %d", this.getXid().getId(), protocol.getType()));
		if (onPlayerProtocol(protocol)) {
			return true;
		}
		return super.onProtocol(protocol);
	}

	/**
	 * 获取玩家id
	 * 
	 * @return
	 */
	public int getId() {
		return playerData.getPlayerEntity().getId();
	}

	/**
	 * 获取puid
	 * 
	 * @return
	 */
	public String getPuid() {
		return playerData.getPlayerEntity().getPuid();
	}

	/**
	 * 获取设备
	 * 
	 * @return
	 */
	public String getDevice() {
		return playerData.getPlayerEntity().getDevice();
	}

	/**
	 * 获取平台
	 * 
	 * @return
	 */
	public String getPlatform() {
		return playerData.getPlayerEntity().getPlatform();
	}

	/**
	 * 获取手机信息
	 * @return
	 */
	public String getPhoneInfo() {
		return playerData.getPlayerEntity().getPhoneInfo();
	}

	/**
	 * 获取语言
	 */
	public String getLanguage() {
		return playerData.getPlayerEntity().getLanguage();
	}

	/**
	 * 获取钻石
	 * 
	 * @return
	 */
	public int getGold() {
		return playerData.getPlayerEntity().getBuyGold() + playerData.getPlayerEntity().getFreeGold();
	}

	/**
	 * 获取金币
	 * 
	 * @return
	 */
	public long getCoin() {
		return playerData.getPlayerEntity().getCoin();
	}
	
	/**
	 * 获取通天塔币
	 * 
	 * @return
	 */
	public int getTowerCoin() {
		return playerData.getPlayerEntity().getTowerCoin();
	}

	/**
	 * 获取玩家vip等级
	 * 
	 * @return
	 */
	public int getVipLevel() {
		return playerData.getPlayerEntity().getVipLevel();
	}

	/**
	 * 获取玩家名字
	 * 
	 * @return
	 */
	public String getName() {
		return playerData.getPlayerEntity().getNickname();
	}

	/**
	 * 获取玩家等级
	 * 
	 * @return
	 */
	public int getLevel() {
		return playerData.getPlayerEntity().getLevel();
	}

	/**
	 * 获取公会Id
	 * @return
	 */
	public int getAllianceId() {
		return playerData.getPlayerAllianceEntity().getAllianceId();
	}

	/**
	 * 获取经验
	 * @return
	 */
	public int getExp() {
		return playerData.getPlayerEntity().getExp();
	}

	/**
	 * 获取怪物等级
	 * 
	 * @return
	 */
	public int getMonsterLevel(int monsterId) {
		return playerData.getMonsterEntity(monsterId).getLevel();
	}

	/**
	 * 获取怪物经验
	 * @return
	 */
	public int getMonsterExp(int monsterId) {
		return playerData.getMonsterEntity(monsterId).getExp();
	}

	/**
	 * 获取会话ip地址
	 * 
	 * @return
	 */
	public String getIp() {
		if (session != null) {
			return session.getIpAddr();
		}
		return null;
	}

	/**
	 * 增加奖励钻石
	 * 
	 * @param gold
	 * @param action
	 */
	public int increaseFreeGold(int gold, Action action) {
		if (gold <= 0) {
			throw new RuntimeException("increaseFreeGold");
		}

		int goldRemain = getGold() + gold - GsConst.MAX_GOLD_COUNT;
		playerData.getPlayerEntity().setFreeGold(playerData.getPlayerEntity().getFreeGold() + gold - (goldRemain > 0 ? goldRemain : 0));
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_GOLD_VALUE), 
				Params.valueOf("add", gold), 
				Params.valueOf("after", playerData.getPlayerEntity().getFreeGold()));

		return goldRemain > 0 ? gold - goldRemain : gold;
	}

	/**
	 * 增加奖励钻石
	 * 
	 * @param gold
	 * @param action
	 */
	public int increaseBuyGold(int gold, Action action) {
		if (gold <= 0) {
			throw new RuntimeException("increaseBuyGold");
		}

		int goldRemain = getGold() + gold - GsConst.MAX_GOLD_COUNT;
		playerData.getPlayerEntity().setBuyGold(playerData.getPlayerEntity().getBuyGold() + gold - (goldRemain > 0 ? goldRemain : 0));
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_GOLD_BUY_VALUE), 
				Params.valueOf("add", gold), 
				Params.valueOf("after", playerData.getPlayerEntity().getBuyGold()));

		return goldRemain > 0 ? gold - goldRemain : gold;
	}

	/**
	 * 消耗钻石
	 * 
	 * @param gold
	 * @param action
	 */
	public void consumeGold(int gold, Action action) {
		if (gold <= 0 || gold > getGold()) {
			throw new RuntimeException("consumeGold");
		}

		if (playerData.getPlayerEntity().getBuyGold() >= gold) {
			playerData.getPlayerEntity().setBuyGold(playerData.getPlayerEntity().getBuyGold() - gold);
		}
		else {
			playerData.getPlayerEntity().setFreeGold(playerData.getPlayerEntity().getFreeGold() + playerData.getPlayerEntity().getBuyGold() - gold);
			playerData.getPlayerEntity().setBuyGold(0);
		}

		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_GOLD_VALUE), 
				Params.valueOf("sub", gold), 
				Params.valueOf("after", getGold()));
	}

	/**
	 * 增加金币
	 * 
	 * @param coin
	 * @param action
	 */
	public long increaseCoin(long coin, Action action) {
		if (coin <= 0) {
			throw new RuntimeException("increaseCoin");
		}

		long coinRemain = playerData.getPlayerEntity().getCoin() + coin - GsConst.MAX_COIN_COUNT;
		playerData.getPlayerEntity().setCoin(playerData.getPlayerEntity().getCoin() + coin - (coinRemain > 0 ? coinRemain : 0));
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_COIN_VALUE), 
				Params.valueOf("add", coin), 
				Params.valueOf("after", getCoin()));

		return coinRemain > 0 ? coin - coinRemain : coin;
	}

	/**
	 * 消费金币
	 * 
	 * @param coin
	 * @param action
	 */
	public void consumeCoin(long coin, Action action) {
		if (coin <= 0 || coin > getCoin()) {
			throw new RuntimeException("consumeCoin");
		}

		playerData.getPlayerEntity().setCoin(playerData.getPlayerEntity().getCoin() - coin);
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_COIN_VALUE), 
				Params.valueOf("sub", coin), 
				Params.valueOf("after", getCoin()));
	}

	/**
	 * 增加通天塔币
	 */
	public int increaseTowerCoin(int towerCoin, Action action) {
		if (towerCoin <= 0) {
			throw new RuntimeException("increaseTowerCoin");
		}

		int coinRemain = getTowerCoin() + towerCoin - GsConst.MAX_COIN_COUNT;
		playerData.getPlayerEntity().setTowerCoin(getTowerCoin() + towerCoin - (coinRemain > 0 ? coinRemain : 0));
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_TOWER_COIN_VALUE), 
				Params.valueOf("add", towerCoin), 
				Params.valueOf("after", getTowerCoin()));

		return coinRemain > 0 ? towerCoin - coinRemain : towerCoin;
	}

	/**
	 * 消费通天塔币
	 */
	public void consumeTowerCoin(int towerCoin, Action action) {
		if (towerCoin <= 0 || towerCoin > getTowerCoin()) {
			throw new RuntimeException("consumeTowerCoin");
		}

		playerData.getPlayerEntity().setTowerCoin(getTowerCoin() - towerCoin);
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_TOWER_COIN_VALUE), 
				Params.valueOf("sub", towerCoin), 
				Params.valueOf("after", getTowerCoin()));
	}

	/**
	 * 设置玩家等级
	 * 
	 * @param level
	 */
	public void setLevel(int level, Action action) {
		if (level <= 0) {
			throw new RuntimeException("setLevel");
		}

		if (level == getLevel()) {
			return ;
		}

		Map<Object, PlayerAttrCfg> playAttrCfg = HawkConfigManager.getInstance().getConfigMap(PlayerAttrCfg.class);
		if (level > playAttrCfg.size()) {
			level = playAttrCfg.size();
		}

		PlayerEntity playerEntity = getPlayerData().getPlayerEntity();
		boolean levelup = level > getLevel();
		if (level == playAttrCfg.size()) {
			playerEntity.setExp(0);
		}

		if (playerEntity.getExp() >= playAttrCfg.get(level).getExp()) {
			playerEntity.setExp(playAttrCfg.get(level).getExp() - 1);
		}

		playerEntity.setLevel(level);
		playerEntity.notifyUpdate(true);

		if (true == levelup) {
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.STATISTICS_UPDATE, getXid());
			msg.pushParam(GsConst.StatisticsType.LEVEL_STATISTICS);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post statistics update message failed: " + getName());
			}
			
			msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LEVEL_CHANGE, HawkXID.valueOf(GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
			msg.pushParam(this);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post level update message failed: " + getName());
			}
			
		}

		BehaviorLogger.log4Service(this, Source.MONSTER_ATTR_CHANGE, action, 
				Params.valueOf("monsterAttr", Const.changeType.CHANGE_PLAYER_EXP), 
				Params.valueOf("level", level), 
				Params.valueOf("after_exp", getExp()),
				Params.valueOf("after_level", getLevel()));
	}

	/**
	 * 设置怪物等级
	 * 
	 * @param level
	 */
	public void setMonsterLevel(int monsterId, int level, Action action) {
		if (level <= 0) {
			throw new RuntimeException("setMonsterLevel");
		}

		MonsterEntity monster = playerData.getMonsterEntity(monsterId);
		if (monster != null && monster.getLevel() != level) {
			Map<Object, MonsterBaseCfg> monsterBaseCfg = HawkConfigManager.getInstance().getConfigMap(MonsterBaseCfg.class);
			if (level > monsterBaseCfg.size()) {
				level = monsterBaseCfg.size();
			}

			// gm指令不受玩家等级限制
			if (action != Action.GM_ACTION && level > this.getLevel()) {
				level = this.getLevel();
			}

			boolean levelUp = level > monster.getLevel();
			// 最大等级需要把经验置0
			if (level == monsterBaseCfg.size()) {
				monster.setExp(0);
			}

			if (monster.getExp() >= monsterBaseCfg.get(level).getNextExp()) {
				monster.setExp(monsterBaseCfg.get(level).getNextExp() - 1);
			}

			monster.setLevel(level);
			monster.notifyUpdate(true);

			if (levelUp == true) {
				StatisticsEntity statisticsEntity = playerData.getStatisticsEntity(); 
				boolean update = false;
				int history = statisticsEntity.getMonsterCountOverLevel(level);
				int cur = playerData.getMonsterCountOverLevel(level);
				if (cur > history) {
					statisticsEntity.setMonsterCountOverLevel(level, cur);
					update = true;
				}

				history = statisticsEntity.getMonsterMaxLevel();
				if (level > history) {
					statisticsEntity.setMonsterMaxLevel(level);
					update = true;
				}

				if (true == update) {
					statisticsEntity.notifyUpdate(true);

					HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.STATISTICS_UPDATE, getXid());
					msg.pushParam(GsConst.StatisticsType.OTHER_STATISTICS);
					if (false == HawkApp.getInstance().postMsg(msg)) {
						HawkLog.errPrintln("post statistics update message failed: " + getName());
					}
				}
			}

			BehaviorLogger.log4Service(this, Source.MONSTER_ATTR_CHANGE, action, 
					Params.valueOf("monsterAttr", Const.changeType.CHANGE_MONSTER_EXP), 
					Params.valueOf("level", level),  
					Params.valueOf("after_exp", getMonsterExp(monsterId)),
					Params.valueOf("after_level", getMonsterLevel(monsterId)));
		}
	}

	/**
	 * 增加怪物经验
	 */
	public void increaseMonsterExp(int monsterId, int exp, Action action) {
		if (exp <= 0) {
			throw new RuntimeException(String.format("increaseExp: %d", exp));
		}

		MonsterEntity monster = playerData.getMonsterEntity(monsterId);
		if (monster != null) {
			Map<Object, MonsterBaseCfg> monsterBaseCfg = HawkConfigManager.getInstance().getConfigMap(MonsterBaseCfg.class);
			if (monsterBaseCfg.size() == monster.getLevel()) {
				return ;
			}

			float levelUpExpRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getCfgId()).getNextExpRate();
			int expRemain = monster.getExp() + exp;
			int targetLevel = monster.getLevel();
			float targetLevelMaxExp = monsterBaseCfg.get(targetLevel).getNextExp() * levelUpExpRate;
			boolean levelup = false;

			// 宠物等级不超过玩家等级，相等时经验保持0
			while (targetLevel != monsterBaseCfg.size()
					&& targetLevel < this.getLevel()
					&& expRemain >= targetLevelMaxExp) {
				expRemain -= targetLevelMaxExp;
				targetLevel += 1;
				levelup = true;
			}

			if (targetLevel == monsterBaseCfg.size() || targetLevel >= this.getLevel()) {
				expRemain = 0;
			}

			if (expRemain != monster.getExp() || true == levelup) {
				monster.setExp(expRemain);
				monster.setLevel(targetLevel);
				monster.notifyUpdate(true);
			}

			if (true == levelup) {
				StatisticsEntity statisticsEntity = playerData.getStatisticsEntity(); 
				boolean update = false;
				int history = statisticsEntity.getMonsterCountOverLevel(targetLevel);
				int cur = playerData.getMonsterCountOverLevel(targetLevel);
				if (cur > history) {
					statisticsEntity.setMonsterCountOverLevel(targetLevel, cur);
					update = true;
				}

				history = statisticsEntity.getMonsterMaxLevel();
				if (targetLevel > history) {
					statisticsEntity.setMonsterMaxLevel(targetLevel);
					update = true;
				}

				if (true == update) {
					statisticsEntity.notifyUpdate(true);

					HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.STATISTICS_UPDATE, getXid());
					msg.pushParam(GsConst.StatisticsType.OTHER_STATISTICS);
					if (false == HawkApp.getInstance().postMsg(msg)) {
						HawkLog.errPrintln("post statistics update message failed: " + getName());
					}
				}
			}

			BehaviorLogger.log4Service(this, Source.MONSTER_ATTR_CHANGE, action, 
					Params.valueOf("monsterAttr", Const.changeType.CHANGE_MONSTER_EXP), 
					Params.valueOf("add", exp),  
					Params.valueOf("after_exp", getMonsterExp(monsterId)),
					Params.valueOf("after_level", getMonsterLevel(monsterId)));
		}
	}

	/**
	 * 增加经验
	 * 
	 * @param exp
	 */
	public void increaseExp(int exp, Action action) {
		if (exp <= 0) {
			throw new RuntimeException("increaseExp");
		}

		Map<Object, PlayerAttrCfg> playAttrCfg = HawkConfigManager.getInstance().getConfigMap(PlayerAttrCfg.class);
		if (getLevel() == playAttrCfg.size()) {
			return ;
		}

		int expRemain = getExp() + exp;
		int targetLevel = getLevel();
		boolean levelup = false;
		while (targetLevel != playAttrCfg.size() && expRemain >= playAttrCfg.get(targetLevel).getExp()) {
			expRemain -= playAttrCfg.get(targetLevel).getExp();
			targetLevel += 1;
			levelup = true;
		}

		playerData.getPlayerEntity().setExp(targetLevel != playAttrCfg.size() ? expRemain : 0);
		playerData.getPlayerEntity().setLevel(targetLevel);
		playerData.getPlayerEntity().notifyUpdate(true);

		if (true == levelup) {
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.STATISTICS_UPDATE, getXid());
			msg.pushParam(GsConst.StatisticsType.LEVEL_STATISTICS);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post statistics update message failed: " + getName());
			}

			msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LEVEL_CHANGE, HawkXID.valueOf(GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
			msg.pushParam(this);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post level update message failed: " + getName());
			}
			
			HawkAccountService.getInstance().report(new HawkAccountService.LevelUpData(getPuid(), getId(), targetLevel));
		}

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("attr", Const.changeType.CHANGE_PLAYER_EXP), 
				Params.valueOf("add", exp), 
				Params.valueOf("after", getExp()));
	}


	/**
	 * 增加物品
	 */
	public ItemEntity increaseItem(String itemId, int itemCount, Action action) {
		if(!ConfigUtil.check(Const.itemType.ITEM_VALUE, itemId)) {
			return null;
		}

		ItemEntity itemEntity = playerData.getItemByItemId(itemId);
		if (itemEntity == null) {
			itemEntity = new ItemEntity();
			itemEntity.setItemId(itemId);
			itemEntity.setCount(itemCount);
			itemEntity.setPlayerId(getId());
			if (itemEntity.notifyCreate()) {
				playerData.addItemEntity(itemEntity);
			}
		} else {
			itemEntity.setCount(itemEntity.getCount() + itemCount);
			itemEntity.notifyUpdate(true);
		}

		if (itemEntity.getId() > 0) {

			BehaviorLogger.log4Service(this, Source.ITEM_ADD, action, 
					Params.valueOf("itemId", itemId), 
					Params.valueOf("id", itemEntity.getId()), 
					Params.valueOf("add", itemCount), 
					Params.valueOf("after", itemEntity.getCount()));

			return itemEntity;
		}
		return null;
	}

	/**
	 * 消耗物品
	 */
	public ItemEntity consumeItem(String itemId, int itemCount, Action action) {
		ItemEntity itemEntity = playerData.getItemByItemId(itemId);
		if (itemEntity != null && itemEntity.getCount() >= itemCount) {
			itemEntity.setCount(itemEntity.getCount() - itemCount);
			itemEntity.notifyUpdate(true);

			BehaviorLogger.log4Service(this, Source.ITEM_REMOVE, action, 
					Params.valueOf("itemId", itemId), 
					Params.valueOf("id", itemEntity.getId()), 
					Params.valueOf("sub", itemCount), 
					Params.valueOf("after", itemEntity.getCount()));

			return itemEntity;
		}
		return null;
	}

	/**
	 * 增加装备
	 */
	public EquipEntity increaseEquip(String equipId, Action action) {
		return increaseEquip(equipId,0,0,action);
	}

	/**
	 * 增加装备
	 */
	public EquipEntity increaseEquip(String equipId, int stage, int level, Action action) {
		if(!ConfigUtil.check(Const.itemType.EQUIP_VALUE, equipId)) {
			return null;
		}

		EquipEntity equipEntity = EquipUtil.generateEquip(this, equipId, stage, level);
		if (equipEntity != null) {
			if (equipEntity.notifyCreate()) {
				playerData.addEquipEntity(equipEntity);

				BehaviorLogger.log4Service(this, Source.EQUIP_ADD, action, 
						Params.valueOf("equipId", equipId), 
						Params.valueOf("id", equipEntity.getId()));
				return equipEntity;
			}
		}
		return null;
	}

	/**
	 * 消耗装备
	 */
	public boolean consumeEquip(long id, Action action) {
		EquipEntity equipEntity = playerData.getEquipById(id);
		if (equipEntity != null) {
			playerData.removeEquipEntity(equipEntity);
			equipEntity.setInvalid(true);
			equipEntity.notifyUpdate(true);

			BehaviorLogger.log4Service(this, Source.EQUIP_REMOVE, action, 
					Params.valueOf("equipId", equipEntity.getItemId()), 
					Params.valueOf("id", equipEntity.getId()));

			return true;
		}
		return false;
	}

	/**
	 * 批量消耗装备
	 * 
	 * @return 消耗失败的装备Id
	 */
	public List<Integer> consumeEquip(List<Integer> ids, Action action) {
		List<Integer> removeFailEquipIds = new LinkedList<>();
		for (Integer id : ids) {
			if (!consumeEquip(id, action)) {
				removeFailEquipIds.add(id);
			}
		}
		return removeFailEquipIds;
	}

	/**
	 * 增加怪物
	 */
	public MonsterEntity increaseMonster(String monsterCfgId, int stage, Action action) {
		MonsterCfg monster = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterCfgId);
		if (null == monster) {
			return null;
		}

		int playerId = getId();
		short level = 1;
		int exp = 0;
		byte lazy = 1;
		int lazyExp = 0;
		byte disposition = 1;
		String[] skillList = monster.getSpellIdList();

		MonsterEntity monsterEntity = new MonsterEntity(monsterCfgId, playerId, (byte)stage, level, exp, lazy, lazyExp, disposition);
		for (String skillId : skillList) {
			monsterEntity.setSkillLevel(skillId, 1);
		}
		monsterEntity.setLocked(false);

		if (true == monsterEntity.notifyCreate()) {
			playerData.setMonsterEntity(monsterEntity);
			onIncreaseMonster(monsterEntity);

			BehaviorLogger.log4Service(this, Source.MONSTER_ADD, action, 
					Params.valueOf("monsterCfgId", monsterCfgId), 
					Params.valueOf("monsterId", monsterEntity.getId()));

			return monsterEntity;
		}
		return null;
	}

	/**
	 * 消耗怪物
	 */
	public boolean consumeMonster(int id, Action action) {
		MonsterEntity monsterEntity = playerData.getMonsterEntity(id);
		if (null != monsterEntity) {
			monsterEntity.setInvalid(true);
			monsterEntity.notifyUpdate(true);
			playerData.removeMonsterEntity(id);

			// 脱装备
			Map<Integer, Long> equips = playerData.getMonsterEquips(id);
			if (equips != null) {
				for (Map.Entry<Integer, Long> entry : equips.entrySet()) {
					EquipEntity equipEntity = playerData.getEquipById(entry.getValue());
					equipEntity.setMonsterId(GsConst.EQUIP_NOT_DRESS);
					equipEntity.notifyUpdate(true);
					playerData.removeMonsterEquip(equipEntity, entry.getKey());
				}
			}

			BehaviorLogger.log4Service(this, Source.MONSTER_REMOVE, action, 
					Params.valueOf("monsterId", id));

			return true;
		}

		return false;
	}

	/**
	 * 统计增加怪物
	 */
	public boolean onIncreaseMonster(MonsterEntity monsterEntity) {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity(); 
		boolean update = false;

		// collection
		if (false == statisticsEntity.getMonsterCollectSet().contains(monsterEntity.getCfgId())) {
			statisticsEntity.addMonsterCollect(monsterEntity.getCfgId());
			update = true;
		}

		// level
		int level = monsterEntity.getLevel();
		int history = statisticsEntity.getMonsterCountOverLevel(level);
		int cur = playerData.getMonsterCountOverLevel(level);
		if (cur > history) {
			statisticsEntity.setMonsterCountOverLevel(level, cur);
			update = true;
		}

		history = statisticsEntity.getMonsterMaxLevel();
		if (level > history) {
			statisticsEntity.setMonsterMaxLevel(level);
			update = true;
		}

		// stage
		int stage = monsterEntity.getStage();
		history = statisticsEntity.getMonsterCountOverStage(stage);
		cur = playerData.getMonsterCountOverStage(stage);
		if (cur > history) {
			statisticsEntity.setMonsterCountOverStage(stage, cur);
			update = true;
		}

		history = statisticsEntity.getMonsterMaxStage();
		if (stage > history) {
			statisticsEntity.setMonsterMaxStage(stage);
			update = true;
		}

		// count
		history = statisticsEntity.getMonsterMaxCount();
		cur = playerData.getMonsterEntityMap().size();
		if (cur > history) {
			statisticsEntity.setMonsterMaxCount(cur);
			update = true;
		}

		if (true == update) {
			statisticsEntity.notifyUpdate(true);

			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.STATISTICS_UPDATE, getXid());
			msg.pushParam(GsConst.StatisticsType.OTHER_STATISTICS);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post statistics update message failed: " + getName());
			}
		}

		return true;
	}

	/**
	 * 消耗贡献值
	 */
	public boolean consumeContribution(int contribution, Action action){
		
		if (getAllianceId() == 0) {
			throw new RuntimeException("not in alliance");
		}
		
		if (contribution <= 0 || contribution > getCoin()) {
			throw new RuntimeException("consumeContribution");
		}

		playerData.getPlayerAllianceEntity().setContribution(playerData.getPlayerAllianceEntity().getContribution() - contribution);
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.ALLIANCE_ATTR_CHANGE, action, 
				Params.valueOf("alliance", Const.changeType.CHANGE_PLAYER_CONTRIBUTION), 
				Params.valueOf("sub", contribution), 
				Params.valueOf("after", playerData.getPlayerAllianceEntity().getContribution()));
		return true;
	}

	/**
	 * 获取贡献值
	 */
	public boolean increaseContribution(int contribution, Action action){
		
		if (getAllianceId() == 0) {
			throw new RuntimeException("not in alliance");
		}

		playerData.getPlayerAllianceEntity().setContribution(playerData.getPlayerAllianceEntity().getContribution() + contribution);
		playerData.getPlayerAllianceEntity().setTotalContribution(playerData.getPlayerAllianceEntity().getTotalContribution() + contribution);
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.ALLIANCE_ATTR_CHANGE, action, 
				Params.valueOf("alliance", Const.changeType.CHANGE_PLAYER_CONTRIBUTION), 
				Params.valueOf("add", contribution), 
				Params.valueOf("after", playerData.getPlayerAllianceEntity().getContribution()));
		return true;
	}

	/**
	 * 发送邮件
	 */
	public boolean SendMail(MailInfo mailInfo, int receiverId, Source source, Action action) {
		int mailId = MailUtil.SendMail(mailInfo, receiverId, getId(), getName());
		if (mailId > 0) {
			BehaviorLogger.log4Service(this, source, action, 
					Params.valueOf("id", mailId));
					Params.valueOf("receiverId", receiverId);
			return true;
		}
		return false;
	}

	/**
	 * 删除多余邮件，从队列头开始删除
	 * @return 删除的邮件Id列表
	 */
	public List<Integer> RemoveOverflowMail() {
		List<Integer> overflowList = new ArrayList<>();

		List<MailEntity> mailList = playerData.getMailEntityList();
		while (mailList.size() > GsConst.MAX_MAIL_COUNT) {
			MailEntity mailEntity = mailList.get(0);
			mailEntity.setState((byte)Const.mailState.OVERFLOW_VALUE);
			mailEntity.setInvalid(true);
			mailEntity.notifyUpdate(true);
			overflowList.add(mailEntity.getId());
			mailList.remove(0);

			BehaviorLogger.log4Service(this, Source.SYS_OPERATION, Action.MAIL_OVERFLOW, 
					Params.valueOf("id", mailEntity.getId()));
		}

		return overflowList;
	}

	/**
	 * 增加活力值
	 */
	public int increaseFatigue(int fatigue, Action action) {
		if (fatigue <= 0) {
			throw new RuntimeException("increaseFatigue");
		}

		// 增加前先更新
		updateFatigue();

		int fatigueRemain = playerData.getStatisticsEntity().getFatigue() + fatigue - GsConst.MAX_FATIGUE_COUNT;
		playerData.getStatisticsEntity().setFatigue(playerData.getStatisticsEntity().getFatigue() + fatigue - (fatigueRemain > 0 ? fatigueRemain : 0));
		playerData.getStatisticsEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_FATIGUE_VALUE), 
				Params.valueOf("add", fatigue), 
				Params.valueOf("after", getPlayerData().getStatisticsEntity().getFatigue()));

		return fatigueRemain > 0 ? fatigue - fatigueRemain : fatigue;
	}

	/**
	 * 消耗活力值
	 */
	public void consumeFatigue(int fatigue, Action action) {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		if (fatigue <= 0 || fatigue > statisticsEntity.getFatigue()) {
			throw new RuntimeException(String.format("consumeFatigue: %d, curFatigue: %d", fatigue, statisticsEntity.getFatigue()));
		}

		int oldFatigue = statisticsEntity.getFatigue();
		int newFatigue = oldFatigue - fatigue;

		// 从低于上限的时间开始增长
		PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, getLevel());
		if (attrCfg != null) {
			int maxFatigue = attrCfg.getFatigue();
			if (oldFatigue >= maxFatigue && newFatigue < maxFatigue) {
				statisticsEntity.setFatigueBeginTime(HawkTime.getCalendar());
			}
		}

		statisticsEntity.setFatigue(newFatigue);
		statisticsEntity.notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_FATIGUE_VALUE), 
				Params.valueOf("sub", fatigue), 
				Params.valueOf("after", newFatigue));
	}

	/**
	 * 消耗技能点
	 */
	public void consumeSkillPoint(int skillPoint, Action action) {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		if (skillPoint <= 0 || skillPoint > statisticsEntity.getSkillPoint()) {
			throw new RuntimeException("consumeSkillPoint");
		}

		int oldSkillPoint = statisticsEntity.getSkillPoint();
		int newSkillPoint = oldSkillPoint - skillPoint;

		// 从低于上限的时间开始增长
		if (oldSkillPoint >= GsConst.MAX_SKILL_POINT && newSkillPoint < GsConst.MAX_SKILL_POINT) {
			statisticsEntity.setSkillPointBeginTime(HawkTime.getCalendar());
		}

		statisticsEntity.setSkillPoint(newSkillPoint);
		statisticsEntity.notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("skillPoint", oldSkillPoint), 
				Params.valueOf("sub", skillPoint), 
				Params.valueOf("after", newSkillPoint));
	}

	/**
	 * 更新技能点
	 */
	public int updateSkillPoint() {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		Calendar curTime = HawkTime.getCalendar();
		Calendar beginTime = statisticsEntity.getSkillPointBeginTime();

		int delta = (int)((curTime.getTimeInMillis() - beginTime.getTimeInMillis()) / 1000);
		int curSkillPoint = statisticsEntity.getSkillPoint() + delta / GsConst.SKILL_POINT_TIME;
		if (curSkillPoint > GsConst.MAX_SKILL_POINT) {
			curSkillPoint = GsConst.MAX_SKILL_POINT;
		}

		beginTime.setTimeInMillis(curTime.getTimeInMillis() - delta % GsConst.SKILL_POINT_TIME  * 1000);
		statisticsEntity.setSkillPoint(curSkillPoint);
		statisticsEntity.setSkillPointBeginTime(beginTime);

		statisticsEntity.notifyUpdate(true);
		return curSkillPoint;
	}

	/**
	 * 更新活力值
	 */
	public int updateFatigue() {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		Calendar curTime = HawkTime.getCalendar();
		Calendar beginTime = statisticsEntity.getFatigueBeginTime();

		int delta = (int)((curTime.getTimeInMillis() - beginTime.getTimeInMillis()) / 1000);
		int oldFatigue = statisticsEntity.getFatigue();
		int curFatigue = oldFatigue + delta / GsConst.FATIGUE_TIME;

		PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, getLevel());
		if (attrCfg != null) {
			// 只有自动恢复体力受等级体力上限影响
			int autoMaxFatigue = attrCfg.getFatigue();
			if (oldFatigue >= autoMaxFatigue) {
				curFatigue = oldFatigue;
			} else if (curFatigue > autoMaxFatigue) {
				curFatigue = autoMaxFatigue;
			}
		}

		if (curFatigue > GsConst.MAX_FATIGUE_COUNT) {
			curFatigue = GsConst.MAX_FATIGUE_COUNT;
		}

		beginTime.setTimeInMillis(curTime.getTimeInMillis() - delta % GsConst.FATIGUE_TIME  * 1000);
		statisticsEntity.setFatigue(curFatigue);
		statisticsEntity.setFatigueBeginTime(beginTime);

		statisticsEntity.notifyUpdate(true);
		return curFatigue;
	}

	/**
	 * 宠物是否可上阵
	 */
	public boolean isMonsterBusy(int monsterId) {
		// TODO
		return false;
	}

	/**
	 * 登录
	 */
	private void onLogin() {
		StatisticsEntity statisticsEntity = playerData.loadStatistics();

		// 首次登陆，初始化数据
		if (statisticsEntity.getLoginCount() == 0) {
			genBirthData();
		}

		// 登录时刷新
		onPlayerRefresh(HawkTime.getMillisecond(), true);
	}

	/**
	 * 生成角色出生初始数据
	 */
	private void genBirthData() {
		StatisticsEntity statisticsEntity = playerData.loadStatistics();

		// default statistics
		PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, getLevel());
		if (attrCfg != null) {
			statisticsEntity.setFatigue(attrCfg.getFatigue());
		}
		statisticsEntity.setSkillPoint(10);
		statisticsEntity.notifyUpdate(true);

		// TEST ----------------------------------------------------------------------------------------
		playerData.loadAllMonster();
		// default monster
		if (statisticsEntity.getMonsterMaxCount() == 0) {
			increaseMonster("xgXiyiren2", 1, Action.SYSTEM);
			increaseMonster("xgHuapo2", 1, Action.SYSTEM);
			increaseMonster("xgPanshen2", 1, Action.SYSTEM);
		}
		// TEST END-------------------------------------------------------------------------------------
	}

	/**
	 * 个人刷新
	 */
	private void onPlayerRefresh(long refreshTime, boolean onLogin) {
		Calendar curTime = HawkTime.getCalendar(refreshTime);
		StatisticsEntity statisticsEntity = playerData.loadStatistics();

		// 刷新时间点
		List<Integer> refreshIndexList = new ArrayList<Integer>();
		for (int index = 0; index < GsConst.PlayerRefreshTime.length; ++index) {
			int timeCfgId = GsConst.PlayerRefreshTime[index];

			Calendar lastRefreshTime = playerData.getStatisticsEntity().getLastRefreshTime(timeCfgId);
			Calendar expectedRefreshTime = TimeUtil.getExpectedRefreshTime(timeCfgId, curTime, lastRefreshTime);
			if (expectedRefreshTime != null) {
				statisticsEntity.setRefreshTime(timeCfgId, expectedRefreshTime);
				refreshIndexList.add(index);
			}
		}

		// 刷新数据
		if (false == refreshIndexList.isEmpty()) {
			statisticsEntity.notifyUpdate(true);

			for (Entry<Integer, HawkObjModule> entry : objModules.entrySet()) {
				PlayerModule playerModule = (PlayerModule) entry.getValue();
				try {
					playerModule.onPlayerRefresh(refreshIndexList, onLogin);
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
		}
	}

}
