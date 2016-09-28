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

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.BILog.BICoinData;
import com.hawk.game.BILog.BIEnergyFlowData;
import com.hawk.game.BILog.BIGoldData;
import com.hawk.game.BILog.BIGuildMemberFlowData;
import com.hawk.game.BILog.BIItemData;
import com.hawk.game.BILog.BIPetFlowData;
import com.hawk.game.BILog.BIPetLevelUpData;
import com.hawk.game.BILog.BIPlayerLevelData;
import com.hawk.game.BILog.BITowerCoinData;
import com.hawk.game.config.AdventureCfg;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.config.MonsterBaseCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.PlayerAttrCfg;
import com.hawk.game.entity.AdventureEntity;
import com.hawk.game.entity.AdventureTeamEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.log.BILogger;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.module.PlayerAdventureModule;
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
import com.hawk.game.module.PlayerSummonModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.HS.code;
import com.hawk.game.protocol.SysProtocol.HSErrorCode;
import com.hawk.game.protocol.SysProtocol.HSKickPlayer;
import com.hawk.game.util.AdventureUtil;
import com.hawk.game.util.AdventureUtil.AdventureCondition;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.MailUtil.MailInfo;
import com.hawk.game.util.QuestUtil;
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
		registerModule(GsConst.ModuleType.MAIL_MODULE, new PlayerMailModule(this));
		registerModule(GsConst.ModuleType.IM_MODULE, new PlayerImModule(this));
		registerModule(GsConst.ModuleType.SHOP_MODULE, new PlayerShopModule(this));
		registerModule(GsConst.ModuleType.ALLIANCE_MODULE, new PlayerAllianceModule(this));
		registerModule(GsConst.ModuleType.ADVENTURE_MODULE, new PlayerAdventureModule(this));
		registerModule(GsConst.ModuleType.SUMMON_MODULE, new PlayerSummonModule(this));
		// 任务模块放其它模块后，用到其它模块数据
		registerModule(GsConst.ModuleType.QUEST_MODULE, new PlayerQuestModule(this));
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
		HawkProtocol protocol = HawkProtocol.valueOf(code.KICKOUT_S_VALUE, builder);
		sendProtocol(protocol);
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
	 * 获取手机型号
	 * @return
	 */
	public String getPhoneType() {
		return playerData.getPlayerEntity().getPhoneType();
	}

	/**
	 * 获取手机系统
	 * @return
	 */
	public String getOsName() {
		return playerData.getPlayerEntity().getOsName();
	}

	/**
	 * 获取手机型号
	 * @return
	 */
	public String getOsVersion() {
		return playerData.getPlayerEntity().getOsVersion();
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
	 */
	public int getTowerCoin() {
		return playerData.getPlayerEntity().getTowerCoin();
	}

	/**
	 * 获取竞技场币
	 */
	public int getArenaCoin() {
		return playerData.getPlayerEntity().getArenaCoin();
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
		int goldIncrease = gold - (goldRemain > 0 ? goldRemain : 0);
		if (0 != goldIncrease) {
			playerData.getPlayerEntity().setFreeGold(playerData.getPlayerEntity().getFreeGold() + goldIncrease);
			playerData.getPlayerEntity().notifyUpdate(true);
		}

		BILogger.getBIData(BIGoldData.class).log(this, action, gold, 0, getGold());
		return goldIncrease;
	}

	/**
	 * 增加购买钻石
	 * 
	 * @param gold
	 * @param action
	 */
	public int increaseBuyGold(int gold, Action action) {
		if (gold <= 0) {
			throw new RuntimeException("increaseBuyGold");
		}

		int goldRemain = getGold() + gold - GsConst.MAX_GOLD_COUNT;
		int goldIncrease = gold - (goldRemain > 0 ? goldRemain : 0);
		if (0 != goldIncrease) {
			playerData.getPlayerEntity().setBuyGold(playerData.getPlayerEntity().getBuyGold() + goldIncrease);
			playerData.getPlayerEntity().notifyUpdate(true);

			if (action == Action.STORE_RECHARGE) {
				StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
				statisticsEntity.increasePayDiamondCount(goldIncrease);
				statisticsEntity.increasePayDiamondCountDaily(goldIncrease);
				statisticsEntity.notifyUpdate(true);
			}
		}

		BILogger.getBIData(BIGoldData.class).log(this, action, gold, 0, getGold());
		return goldIncrease;
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

		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		statisticsEntity.increaseUseDiamondCount(gold);
		statisticsEntity.increaseUseDiamondCountDaily(gold);
		statisticsEntity.notifyUpdate(true);

		BILogger.getBIData(BIGoldData.class).log(this, action, 0, gold, getGold());
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
		long coinIncrease = coin - (coinRemain > 0 ? coinRemain : 0);
		if (0 != coinIncrease) {
			playerData.getPlayerEntity().setCoin(playerData.getPlayerEntity().getCoin() + coinIncrease);
			playerData.getPlayerEntity().notifyUpdate(true);
		}

		if (action == Action.COIN_CHANGE) {
			StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
			statisticsEntity.increaseBuyCoinTimes();
			statisticsEntity.increaseBuyCoinTimesDaily();
			statisticsEntity.notifyUpdate(true);
		}

		BILogger.getBIData(BICoinData.class).log(this, action, coin, 0, getCoin());
		return coinIncrease;
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

		BILogger.getBIData(BICoinData.class).log(this, action, 0, coin, getCoin());
	}

	/**
	 * 增加通天塔币
	 */
	public int increaseTowerCoin(int towerCoin, Action action) {
		if (towerCoin <= 0) {
			throw new RuntimeException("increaseTowerCoin");
		}

		int coinRemain = getTowerCoin() + towerCoin - GsConst.MAX_COIN_COUNT;
		int coinIncrease = towerCoin - (coinRemain > 0 ? coinRemain : 0);
		if (0 != coinIncrease) {
			playerData.getPlayerEntity().setTowerCoin(getTowerCoin() + coinIncrease);
			playerData.getPlayerEntity().notifyUpdate(true);

			playerData.getStatisticsEntity().increaseCoinTowerCount(coinIncrease);
			playerData.getStatisticsEntity().notifyUpdate(true);
		}

		BILogger.getBIData(BITowerCoinData.class).log(this, action, towerCoin, 0, getTowerCoin());
		return coinIncrease;
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

		BILogger.getBIData(BITowerCoinData.class).log(this, action, 0, towerCoin, getTowerCoin());
	}

	/**
	 * 增加竞技场币
	 */
	public int increaseArenaCoin(int arenaCoin, Action action) {
		if (arenaCoin <= 0) {
			throw new RuntimeException("increaseArenaCoin");
		}

		int coinRemain = getArenaCoin() + arenaCoin - GsConst.MAX_COIN_COUNT;
		int coinIncrease = arenaCoin - (coinRemain > 0 ? coinRemain : 0);
		if (0 != coinIncrease) {
			playerData.getPlayerEntity().setArenaCoin(getArenaCoin() + coinIncrease);
			playerData.getPlayerEntity().notifyUpdate(true);
		}

		// TODO BI
		return coinIncrease;
	}

	/**
	 * 消费竞技场币
	 */
	public void consumeArenaCoin(int arenaCoin, Action action) {
		if (arenaCoin <= 0 || arenaCoin > getArenaCoin()) {
			throw new RuntimeException("consumeArenaCoin");
		}

		playerData.getPlayerEntity().setArenaCoin(getArenaCoin() - arenaCoin);
		playerData.getPlayerEntity().notifyUpdate(true);

		//TODO BI
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

		int oldLevel = playerEntity.getLevel();
		int oldExp = playerEntity.getExp();

		if (playerEntity.getExp() >= playAttrCfg.get(level).getExp()) {
			playerEntity.setExp(playAttrCfg.get(level).getExp() - 1);
		}

		playerEntity.setLevel(level);
		playerEntity.notifyUpdate(true);

		if (true == levelup) {
			QuestUtil.postQuestDataUpdateMsg(getXid());

			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LEVEL_UP, getXid());
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post level up message failed: " + getId());
			}

			msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LEVEL_UP, HawkXID.valueOf(GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
			msg.pushParam(this);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post level up message failed: " + getName());
			}
		}

		BILogger.getBIData(BIPlayerLevelData.class).log(this, action, oldLevel, level, 0, oldExp, playerEntity.getExp());

		HawkAccountService.getInstance().report(new HawkAccountService.LevelUpData(getPuid(), getId(), getLevel()));
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
		int oldLevel = getLevel();
		int oldExp = getExp();
		int targetLevelMaxExp = playAttrCfg.get(targetLevel).getExp();
		boolean levelup = false;
		while (targetLevel != playAttrCfg.size() && expRemain >= targetLevelMaxExp) {
			expRemain -= targetLevelMaxExp;
			targetLevel += 1;
			targetLevelMaxExp = playAttrCfg.get(targetLevel).getExp();
			levelup = true;
		}

		playerData.getPlayerEntity().setExp(targetLevel != playAttrCfg.size() ? expRemain : 0);
		playerData.getPlayerEntity().setLevel(targetLevel);
		playerData.getPlayerEntity().notifyUpdate(true);

		if (true == levelup) {
			QuestUtil.postQuestDataUpdateMsg(getXid());

			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LEVEL_UP, getXid());
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post level up message failed: " + getId());
			}

			msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LEVEL_UP, HawkXID.valueOf(GsConst.ObjType.MANAGER, GsConst.ObjId.ALLIANCE));
			msg.pushParam(this);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post level up message failed: " + getName());
			}

			HawkAccountService.getInstance().report(new HawkAccountService.LevelUpData(getPuid(), getId(), targetLevel));
		}

		BILogger.getBIData(BIPlayerLevelData.class).log(this, action, oldLevel, getLevel(), exp, oldExp, getExp());
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
			MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getCfgId());
			if (monsterCfg == null) {
				throw new RuntimeException("monster config not found " + monster.getCfgId());
			}

			Map<Object, MonsterBaseCfg> monsterBaseCfg = HawkConfigManager.getInstance().getConfigMap(MonsterBaseCfg.class);
			if (level > monsterBaseCfg.size()) {
				level = monsterBaseCfg.size();
			}

			// gm指令不受玩家等级限制
			if (action != Action.GM && level > this.getLevel()) {
				level = this.getLevel();
			}

			int oldLevel = monster.getLevel();
			boolean levelUp = level > oldLevel;
			// 最大等级需要把经验置0
			if (level == monsterBaseCfg.size()) {
				monster.setExp(0);
			}

			int oldExp = monster.getExp();
			if (monster.getExp() >= monsterBaseCfg.get(level).getNextExp()) {
				monster.setExp(monsterBaseCfg.get(level).getNextExp() - 1);
			}

			monster.setLevel(level);
			monster.notifyUpdate(true);

			if (levelUp == true) {
				StatisticsEntity statisticsEntity = playerData.getStatisticsEntity(); 
				for (int i = oldLevel + 1; i <= level; ++i) {
					statisticsEntity.increaseMonsterCountOverLevel(i);
				}
				statisticsEntity.notifyUpdate(true);
			}

			BILogger.getBIData(BIPetLevelUpData.class).log(this, monsterId, monsterCfg, oldLevel, monster.getLevel(), 0, oldExp, monster.getExp());
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
			MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getCfgId());
			if (monsterCfg == null) {
				throw new RuntimeException("monster config not found " + monster.getCfgId());
			}

			int oldLevel = monster.getLevel();
			int oldExp = monster.getExp();
			Map<Object, MonsterBaseCfg> monsterBaseCfg = HawkConfigManager.getInstance().getConfigMap(MonsterBaseCfg.class);
			if (monsterBaseCfg.size() == oldLevel) {
				return ;
			}

			float levelUpExpRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getCfgId()).getNextExpRate();
			int expRemain = monster.getExp() + exp;
			int targetLevel = oldLevel;
			float targetLevelMaxExp = monsterBaseCfg.get(targetLevel).getNextExp() * levelUpExpRate;
			boolean levelup = false;

			// 宠物等级不超过玩家等级，相等时经验保持0
			while (targetLevel != monsterBaseCfg.size()
					&& targetLevel < this.getLevel()
					&& expRemain >= targetLevelMaxExp) {
				expRemain -= targetLevelMaxExp;
				targetLevel += 1;
				targetLevelMaxExp = monsterBaseCfg.get(targetLevel).getNextExp() * levelUpExpRate;
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
				for (int i = oldLevel + 1; i <= targetLevel; ++i) {
					statisticsEntity.increaseMonsterCountOverLevel(i);
				}
				statisticsEntity.notifyUpdate(true);
			}

			BILogger.getBIData(BIPetLevelUpData.class).log(this, monsterId, monsterCfg, oldLevel, monster.getLazyExp(), exp, oldExp, monster.getExp());
		}
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
			if (action == Action.ENERGY_BUY 	 ||
				action == Action.KEY_BUY 		 ||
				action == Action.RAID_TICKEY_BUY ||
				action == Action.NORMAL_SHOP_BUY ||
				action == Action.GUILD_SHOP_BUY  ||
				action == Action.TOWER_SHOP_BUY  ||
				action == Action.STORE_BUY) 
			{
				StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
				statisticsEntity.increaseBuyItemTimes(itemId);
				statisticsEntity.notifyUpdate(true);
			}

			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemEntity.getItemId());
			if (itemCfg == null) {
				throw new RuntimeException("item config not found " + itemEntity.getItemId());
			}

			BILogger.getBIData(BIItemData.class).log(this, action, itemCfg, 0, itemCount, 0);

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

			if (action == Action.BOX_USE 			||
				action == Action.EXP_POTION_USE	    ||
				action == Action.ENERGY_COOKIES_USE ||
				action == Action.KEY_USE 			||
				action == Action.RAID_TICKET_USE)
			{
				StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
				statisticsEntity.increaseUseItemCount(itemId, itemCount);
				statisticsEntity.increaseUseItemCountDaily(itemId, itemCount);
				statisticsEntity.notifyUpdate(true);
			}

			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemEntity.getItemId());
			if (itemCfg == null) {
				throw new RuntimeException("item config not found " + itemEntity.getItemId());
			}

			BILogger.getBIData(BIItemData.class).log(this, action, itemCfg, 0, 0, itemCount);

			return itemEntity;
		}
		return null;
	}

	/**
	 * 增加装备
	 */
	public EquipEntity increaseEquip(String equipId, Action action) {
		return increaseEquip(equipId, 0, 0, action);
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

				StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
				if (action == Action.GUILD_SHOP_BUY     ||
					action == Action.NORMAL_SHOP_BUY 	||
					action == Action.TOWER_SHOP_BUY) {
					statisticsEntity.increaseBuyItemTimes(equipId);
					statisticsEntity.notifyUpdate(true);
				} else if (action == Action.EGG) {
					statisticsEntity.increaseCallEquipStageTimes(stage);
					statisticsEntity.notifyUpdate(true);
				}

				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipId);
				if (itemCfg == null) {
					throw new RuntimeException("item config not found " + equipId);
				}

				BILogger.getBIData(BIItemData.class).log(this, action, itemCfg, stage, 1, 0);
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

			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, equipEntity.getItemId());
			if (itemCfg == null) {
				throw new RuntimeException("item config not found " + equipEntity.getItemId());
			}

			BILogger.getBIData(BIItemData.class).log(this, action, itemCfg, equipEntity.getStage(), 0, 1);

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
			throw new RuntimeException("monster config not found " + monsterCfgId);
		}

		int playerId = getId();
		short level = 1;
		int exp = 0;
		byte lazy = 1;
		int lazyExp = 0;
		byte disposition = (byte)monster.getDisposition();
		String[] skillList = monster.getSpellIdList();

		MonsterEntity monsterEntity = new MonsterEntity(monsterCfgId, playerId, (byte)stage, level, exp, lazy, lazyExp, disposition);
		for (String skillId : skillList) {
			monsterEntity.setSkillLevel(skillId, 1);
		}

		if (true == monsterEntity.notifyCreate()) {
			playerData.setMonsterEntity(monsterEntity);
			onIncreaseMonster(monsterEntity);

			StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
			if (action == Action.MONSTER_COMPOSE) {
				statisticsEntity.increaseMonsterMixTimes();
				statisticsEntity.notifyUpdate(true);
			} else if (action == Action.EGG) {
				statisticsEntity.increaseCallMonsterStageTimes(stage);
				statisticsEntity.notifyUpdate(true);
			}

			BILogger.getBIData(BIPetFlowData.class).log(this, action, monsterEntity.getId(), monster, stage, 1, 0, 1, 0);
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
			monsterEntity.delete();
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

			MonsterCfg monster = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monsterEntity.getCfgId());
			if (null == monster) {
				throw new RuntimeException("monster config not found " + monsterEntity.getCfgId());
			}

			BILogger.getBIData(BIPetFlowData.class).log(this, action, monsterEntity.getId(), monster, monsterEntity.getStage(), monsterEntity.getLevel(), 0, 1, 0);

			return true;
		}

		return false;
	}

	/**
	 * 统计增加的怪物
	 */
	public boolean onIncreaseMonster(MonsterEntity monsterEntity) {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();

		// collection
		if (false == statisticsEntity.getMonsterCollectSet().contains(monsterEntity.getCfgId())) {
			statisticsEntity.addMonsterCollect(monsterEntity.getCfgId());
		}

		// level
		int level = monsterEntity.getLevel();
		for (int i = 1; i <= level; ++i) {
			statisticsEntity.increaseMonsterCountOverLevel(level);
		}

		// stage
		int stage = monsterEntity.getStage();
		for (int i = 0; i <= stage; ++i) {
			statisticsEntity.increaseMonsterCountOverStage(stage);
		}

		statisticsEntity.notifyUpdate(true);
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

		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(getAllianceId());
		if (allianceEntity == null) {
			throw new RuntimeException("not join guild");
		}

		playerData.getPlayerAllianceEntity().setContribution(playerData.getPlayerAllianceEntity().getContribution() - contribution);
		playerData.getPlayerAllianceEntity().notifyUpdate(true);

		BILogger.getBIData(BIGuildMemberFlowData.class).logContribution(
				this, 
				action, 
				allianceEntity, 
				playerData.getPlayerAllianceEntity(), 
				0, 
				contribution
				);

		return true;
	}

	/**
	 * 获取贡献值
	 */
	public boolean increaseContribution(int contribution, Action action){
		if (getAllianceId() == 0) {
			throw new RuntimeException("not in alliance");
		}
		if (contribution <= 0) {
			throw new RuntimeException("increaseContribution");
		}

		playerData.getPlayerAllianceEntity().setContribution(playerData.getPlayerAllianceEntity().getContribution() + contribution);
		playerData.getPlayerAllianceEntity().setTotalContribution(playerData.getPlayerAllianceEntity().getTotalContribution() + contribution);
		playerData.getPlayerAllianceEntity().notifyUpdate(true);

		playerData.getStatisticsEntity().increaseCoinAllianceCount(contribution);
		playerData.getStatisticsEntity().increaseCoinAllianceCountDaily(contribution);
		playerData.getStatisticsEntity().notifyUpdate(true);

		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(getAllianceId());
		if (allianceEntity == null) {
			throw new RuntimeException("not join guild");
		}

		BILogger.getBIData(BIGuildMemberFlowData.class).logContribution(
				this, 
				action, 
				allianceEntity, 
				playerData.getPlayerAllianceEntity(), 
				contribution, 
				0
				);

		return true;
	}

	/**
	 * 发送邮件
	 */
	public boolean SendMail(MailInfo mailInfo, int receiverId, Action action) {
		int mailId = MailUtil.SendMail(mailInfo, receiverId, getId(), getName());
		if (mailId > 0) {
			//TODO BI
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

			// TODO BI
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
		regainFatigue();

		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		int fatigueRemain = statisticsEntity.getFatigue() + fatigue - GsConst.MAX_FATIGUE_COUNT;
		int fatigueIncrease = fatigue - (fatigueRemain > 0 ? fatigueRemain : 0);
		if (0 != fatigueIncrease) {
			statisticsEntity.setFatigue(statisticsEntity.getFatigue() + fatigueIncrease);
			statisticsEntity.notifyUpdate(true);
		}

		// 除以下情况，其余情况增加体力在具体位置记录BI
		if (action == Action.MAIL_REWARD || action == Action.ENERGY_COOKIES_USE) {
			BILogger.getBIData(BIEnergyFlowData.class).log(this, action, "", "", "", fatigueIncrease, fatigue, statisticsEntity.getFatigue());
		}
		return fatigueIncrease;
	}

	/**
	 * 消耗活力值
	 */
	public void consumeFatigue(int fatigue, Action action) {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		if (fatigue <= 0 || fatigue > statisticsEntity.getFatigue()) {
			throw new RuntimeException(String.format("consumeFatigue: %d, curFatigue: %d", fatigue, statisticsEntity.getFatigue()));
		}

		int old = statisticsEntity.getFatigue();
		int cur = old - fatigue;

		// 从低于上限的时间开始恢复
		PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, getLevel());
		if (attrCfg != null) {
			int max = attrCfg.getFatigue();
			if (old >= max && cur < max) {
				statisticsEntity.setFatigueBeginTime(HawkTime.getCalendar());
			}
		}

		statisticsEntity.setFatigue(cur);
		statisticsEntity.increaseUseFatigueCount(fatigue);
		statisticsEntity.increaseUseFatigueCountDaily(fatigue);
		statisticsEntity.notifyUpdate(true);

		BILogger.getBIData(BIEnergyFlowData.class).log(this, action, "", "", "", 0, fatigue, cur);
	}

	/**
	 * 恢复活力值
	 */
	public int regainFatigue() {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		int old = statisticsEntity.getFatigue();
		if (old == GsConst.MAX_FATIGUE_COUNT) {
			return old;
		}

		Calendar curTime = HawkTime.getCalendar();
		Calendar beginTime = statisticsEntity.getFatigueBeginTime();

		int delta = (int)((curTime.getTimeInMillis() - beginTime.getTimeInMillis()) / 1000);
		int cur = old + delta / GsConst.FATIGUE_TIME;

		if (cur > GsConst.MAX_FATIGUE_COUNT) {
			cur = GsConst.MAX_FATIGUE_COUNT;
		}

		PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, getLevel());
		if (attrCfg != null) {
			// 只有自动恢复体力受等级体力上限影响
			int autoMaxFatigue = attrCfg.getFatigue();
			if (old >= autoMaxFatigue) {
				cur = old;
			} else if (cur > autoMaxFatigue) {
				cur = autoMaxFatigue;
			}
		}

		if (old == cur) {
			return cur;
		}

		beginTime.setTimeInMillis(curTime.getTimeInMillis() - delta % GsConst.FATIGUE_TIME  * 1000);
		statisticsEntity.setFatigue(cur);
		statisticsEntity.setFatigueBeginTime(beginTime);
		statisticsEntity.notifyUpdate(true);

		BILogger.getBIData(BIEnergyFlowData.class).log(this, Action.ENERGY_RECOVER, "", "", "", cur - old, 0, cur);
		return cur;
	}

	/**
	 * 消耗技能点
	 */
	public void consumeSkillPoint(int skillPoint, Action action) {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		if (skillPoint <= 0 || skillPoint > statisticsEntity.getSkillPoint()) {
			throw new RuntimeException("consumeSkillPoint");
		}

		int old = statisticsEntity.getSkillPoint();
		int cur = old - skillPoint;

		// 从低于上限的时间开始恢复
		if (old >= GsConst.MAX_SKILL_POINT && cur < GsConst.MAX_SKILL_POINT) {
			statisticsEntity.setSkillPointBeginTime(HawkTime.getCalendar());
		}

		statisticsEntity.setSkillPoint(cur);
		statisticsEntity.notifyUpdate(true);

		// TODO BI
	}

	/**
	 * 恢复技能点
	 */
	public int regainSkillPoint() {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		int old = statisticsEntity.getSkillPoint();
		if (old == GsConst.MAX_SKILL_POINT) {
			return old;
		}

		Calendar curTime = HawkTime.getCalendar();
		Calendar beginTime = statisticsEntity.getSkillPointBeginTime();

		int delta = (int)((curTime.getTimeInMillis() - beginTime.getTimeInMillis()) / 1000);
		int cur = old + delta / GsConst.SKILL_POINT_TIME;

		if (cur > GsConst.MAX_SKILL_POINT) {
			cur = GsConst.MAX_SKILL_POINT;
		}

		if (old == cur) {
			return cur;
		}

		beginTime.setTimeInMillis(curTime.getTimeInMillis() - delta % GsConst.SKILL_POINT_TIME  * 1000);
		statisticsEntity.setSkillPoint(cur);
		statisticsEntity.setSkillPointBeginTime(beginTime);

		statisticsEntity.notifyUpdate(true);
		return cur;
	}

	/**
	 * 增加大冒险条件刷新次数
	 */
	public int increaseAdventureChangeTimes(int times, Action action) {
		if (times <= 0) {
			throw new RuntimeException("increaseAdventureChangeTimes");
		}

		// 增加前先更新
		regainAdventureChangeTimes();

		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		int old = statisticsEntity.getAdventureChange();
		int remain = old + times - GsConst.MAX_ADVENTURE_CHANGE;
		int increase = times - (remain > 0 ? remain : 0);
		if (0 != increase) {
			statisticsEntity.setAdventureChange(old + increase);
			statisticsEntity.notifyUpdate(true);
		}

		//TODO BI
		return increase;
	}

	/**
	 * 消耗大冒险条件刷新次数
	 */
	public void consumeAdventureChangeTimes(int times, Action action) {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		if (times <= 0 || times > statisticsEntity.getAdventureChange()) {
			throw new RuntimeException("consumeAdventureChangeTimes");
		}

		int old = statisticsEntity.getAdventureChange();
		int cur = old - times;

		// 从低于上限的时间开始恢复
		if (old >= GsConst.MAX_ADVENTURE_CHANGE && cur < GsConst.MAX_ADVENTURE_CHANGE) {
			statisticsEntity.setAdventureChangeBeginTime(HawkTime.getCalendar());
		}

		statisticsEntity.setAdventureChange(cur);
		statisticsEntity.notifyUpdate(true);

		// TODO BI
	}

	/**
	 * 恢复大冒险条件刷新次数
	 */
	public int regainAdventureChangeTimes() {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		int old = statisticsEntity.getAdventureChange();
		if (old == GsConst.MAX_ADVENTURE_CHANGE) {
			return old;
		}

		Calendar curTime = HawkTime.getCalendar();
		Calendar beginTime = statisticsEntity.getAdventureChangeBeginTime();

		int delta = (int)((curTime.getTimeInMillis() - beginTime.getTimeInMillis()) / 1000);
		int cur = old + delta / GsConst.ADVENTURE_CHANGE_TIME;

		if (cur > GsConst.MAX_ADVENTURE_CHANGE) {
			cur = GsConst.MAX_ADVENTURE_CHANGE;
		}

		if (old == cur) {
			return cur;
		}

		beginTime.setTimeInMillis(curTime.getTimeInMillis() - delta % GsConst.ADVENTURE_CHANGE_TIME  * 1000);
		statisticsEntity.setAdventureChange(cur);
		statisticsEntity.setAdventureChangeBeginTime(beginTime);

		statisticsEntity.notifyUpdate(true);
		return cur;
	}

	/**
	 * 恢复免费钻石抽蛋点数
	 */
	public int regainEggDiamondFreePoint() {
		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		int old = statisticsEntity.getEggDiamond1FreePoint();
		if (old == GsConst.summon.MAX_DIAMOND_FREE_TIMES) {
			return old;
		}

		Calendar curTime = HawkTime.getCalendar();
		Calendar beginTime = statisticsEntity.getEggDiamond1FreePointBeginTime();

		int delta = (int)((curTime.getTimeInMillis() - beginTime.getTimeInMillis()) / 1000);
		int cur = old + delta / GsConst.summon.DIAMOND_FREE_TIME;

		if (cur > GsConst.summon.MAX_DIAMOND_FREE_TIMES) {
			cur = GsConst.summon.MAX_DIAMOND_FREE_TIMES;
		}

		if (old == cur) {
			return cur;
		}

		beginTime.setTimeInMillis(curTime.getTimeInMillis() - delta % GsConst.summon.DIAMOND_FREE_TIME  * 1000);
		statisticsEntity.setEggDiamond1FreePoint(cur);
		statisticsEntity.setEggDiamond1FreePointBeginTime(beginTime);

		statisticsEntity.notifyUpdate(true);
		return cur;
	}

	/**
	 * 登录
	 */
	private void onLogin() {
		StatisticsEntity statisticsEntity = playerData.loadStatistics();

		// 首次登陆，初始化数据
		if (statisticsEntity.getLoginTimes() == 0) {
			try {
			genBirthData();
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}

		statisticsEntity.increaseLoginTimes();
		statisticsEntity.notifyUpdate(true);

		// 登录时刷新
		onPlayerRefresh(HawkTime.getMillisecond(), true);

		// 同步玩家信息
		playerData.syncPlayerInfo();
	}

	/**
	 * 生成角色出生初始数据
	 */
	private void genBirthData() {
		playerData.loadPlayer();
		MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.WELCOME);
		if (mailCfg != null) {
			MailUtil.SendSysMail(mailCfg, getId());
		}

		increaseCoin(1000, Action.NULL);
		increaseFreeGold(100, Action.NULL);

		StatisticsEntity statisticsEntity = playerData.loadStatistics();
		PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, getLevel());
		if (attrCfg != null) {
			statisticsEntity.setFatigue(attrCfg.getFatigue());
		}
		statisticsEntity.setSkillPoint(GsConst.MAX_SKILL_POINT);
		statisticsEntity.setAdventureChange(GsConst.MAX_ADVENTURE_CHANGE);
		statisticsEntity.notifyUpdate(true);

		// 所有大冒险
		playerData.loadAllAdventure();
		for (Entry<Integer, Map<Integer, List<AdventureCfg>>> typeEntry : AdventureUtil.getAdventureMap().entrySet()) {
			int type = typeEntry.getKey();
			for (Entry<Integer, List<AdventureCfg>> gearEntity : typeEntry.getValue().entrySet()) {
				int gear = gearEntity.getKey();
				List<AdventureCfg> cfgList = gearEntity.getValue();

				AdventureEntity advenEntity = new AdventureEntity(getId(), type, gear);

				List<AdventureCondition> conditionList = AdventureUtil.genConditionList(getLevel());
				if (null == conditionList) {
					advenEntity.clearConditionList();
				} else {
					advenEntity.setConditionList(conditionList);
				}

				for (int i = 0; i < gearEntity.getValue().size(); ++i) {
					AdventureCfg cfg = cfgList.get(i);
					if (true == cfg.isInLevelRange(getLevel())) {
						advenEntity.setAdventureId(cfg.getId());
						break;
					}
				}

				advenEntity.notifyCreate();
				playerData.addAdventureEntity(advenEntity);
			}
		}

		// 初始2支大冒险队伍
		playerData.loadAllAdventureTeam();
		AdventureTeamEntity teamEntity = new AdventureTeamEntity(getId(), 1);
		teamEntity.notifyCreate();
		playerData.addAdventureTeamEntity(teamEntity);

		teamEntity = new AdventureTeamEntity(getId(), 2);
		teamEntity.notifyCreate();
		playerData.addAdventureTeamEntity(teamEntity);

		// TEST ----------------------------------------------------------------------------------------
		playerData.loadAllMonster();
		// default monster
		if (true == statisticsEntity.getMonsterCollectSet().isEmpty()) {
			increaseMonster("xgXiaochou3", 0, Action.NULL);
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
