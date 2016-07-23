package com.hawk.game.player;

import java.awt.TexturePaint;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.app.HawkApp;
import org.hawk.app.HawkAppObj;
import org.hawk.app.HawkObjModule;
import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.service.HawkServiceProxy;
import org.hawk.util.services.HawkReportService;
import org.hawk.xid.HawkXID;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.MonsterBaseCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.PlayerAttrCfg;
import com.hawk.game.config.QuestCfg;
import com.hawk.game.config.TimeCfg;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
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
import com.hawk.game.protocol.Const.RewardReason;
import com.hawk.game.protocol.Const.playerAttr;
import com.hawk.game.protocol.Mail.HSMail;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Monster.HSMonsterAdd;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.Quest.HSQuestAccept;
import com.hawk.game.protocol.Quest.HSQuestRemove;
import com.hawk.game.protocol.Skill.HSSkill;
import com.hawk.game.protocol.SysProtocol.HSErrorCode;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.QuestUtil;
import com.hawk.game.util.TimeUtil;
import com.hawk.game.util.MailUtil.MailInfo;
import com.hawk.game.util.QuestUtil.QuestGroup;

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
	 * 帧计数
	 */
	private int tickIndex = 0;

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
	public void sendError(int hpCode, int errCode) {
		HSErrorCode.Builder builder = HSErrorCode.newBuilder();
		builder.setHpCode(hpCode);
		builder.setErrCode(errCode);
		sendProtocol(HawkProtocol.valueOf(HS.sys.ERROR_CODE, builder));
	}

	/**
	 * 通知错误码
	 * 
	 * @param errCode
	 */
	public void sendError(int hpCode, int errCode, int errFlag) {
		HSErrorCode.Builder builder = HSErrorCode.newBuilder();
		builder.setHpCode(hpCode);
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
	public boolean onTick() {
		// 玩家未组装完成直接不走时钟tick机制
		if (!isAssembleFinish()) {
			return true;
		}

		// 在线跨天刷新
		if (null == playerData.getPlayerEntity() || false == HawkTime.isToday(playerData.getPlayerEntity().getResetTime().getTime())) {
			onFirstLoginDaily(true);
		}
		// 刷新玩家数据
		if (++tickIndex % GsConst.REFRESH_PERIOD == 0) {
			tickIndex = 0;
			onRefresh();
		}

		return super.onTick();
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
		return playerData.getPlayerEntity().getGold();
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
		return playerData.getMonsterEntity(monsterId).getExp();
	}

	/**
	 * 获取怪物经验
	 * @return
	 */
	public int getMonsterExp(int monsterId) {
		return playerData.getMonsterEntity(monsterId).getLevel();
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
	 * 增加钻石
	 * 
	 * @param gold
	 * @param action
	 */
	public void increaseGold(int gold, Action action) {
		if (gold <= 0) {
			throw new RuntimeException("increaseGold");
		}

		playerData.getPlayerEntity().setGold(playerData.getPlayerEntity().getGold() + gold);
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_GOLD_VALUE), 
				Params.valueOf("add", gold), 
				Params.valueOf("after", getGold()));
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

		playerData.getPlayerEntity().setGold(playerData.getPlayerEntity().getGold() - gold);
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
	public void increaseCoin(int coin, Action action) {
		if (coin <= 0) {
			throw new RuntimeException("increaseCoin");
		}

		playerData.getPlayerEntity().setCoin(playerData.getPlayerEntity().getCoin() + coin);
		playerData.getPlayerEntity().notifyUpdate(true);

		BehaviorLogger.log4Service(this, Source.PLAYER_ATTR_CHANGE, action, 
				Params.valueOf("playerAttr", Const.changeType.CHANGE_COIN_VALUE), 
				Params.valueOf("add", coin), 
				Params.valueOf("after", getCoin()));
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
	 * 增加vip等级
	 * 
	 * @param level
	 */
	public void setVipLevel(int level, Action action) {
		if (level <= 0) {
			throw new RuntimeException("increaseVipLevel");
		}
	}


	/**
	 * 增加等级
	 * 
	 * @param level
	 * @return 实际添加的等级
	 */
	public int increaseMonsterExp(int monsterId, int exp, Action action) {
		if (exp <= 0) {
			throw new RuntimeException("increaseExp");
		}
		
		MonsterEntity monster = playerData.getMonsterEntity(monsterId);
		if (monster != null) {		
			Map<Object, MonsterBaseCfg> monsterBaseCfg = HawkConfigManager.getInstance().getConfigMap(MonsterBaseCfg.class);	
			if (monsterBaseCfg.size() == monster.getLevel()) {
				return 0;
			}
			
			float levelUpExpRate = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, monster.getCfgId()).getNextExpRate();
			int expRemain = monster.getExp() + exp;
			int targetLevel = monster.getLevel();
			boolean levelup = false;
			while (targetLevel != monsterBaseCfg.size() && expRemain >= monsterBaseCfg.get(targetLevel).getNextExp() * levelUpExpRate) {
				expRemain -= monsterBaseCfg.get(targetLevel).getNextExp() * levelUpExpRate;
				targetLevel += 1;
				levelup = true;
			}
			
			monster.setExp(targetLevel == monsterBaseCfg.size() ? 0 : expRemain);
			monster.setLevel(targetLevel);
			monster.notifyUpdate(true);
			
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
					Params.valueOf("after", getMonsterExp(monsterId)));
			
			return monsterBaseCfg.size() == monster.getLevel() ? exp - expRemain : exp ;
		}
		
		return 0;
	}

	/**
	 * 增加经验
	 * 
	 * @param exp
	 */
	public int increaseExp(int exp, Action action) {
		if (exp <= 0) {
			throw new RuntimeException("increaseExp");
		}

		Map<Object, PlayerAttrCfg> playAttrCfg = HawkConfigManager.getInstance().getConfigMap(PlayerAttrCfg.class);
		
		if (getLevel() == playAttrCfg.size()) {
			return 0;
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
		}

		BehaviorLogger.log4Service(this, Source.MONSTER_ATTR_CHANGE, action, 
				Params.valueOf("monsterAttr", Const.changeType.CHANGE_PLAYER_EXP), 
				Params.valueOf("add", exp), 
				Params.valueOf("after", getExp()));
		
		return targetLevel != playAttrCfg.size() ? exp : exp - expRemain;
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
			List<Integer> battleMonsterList = playerData.getPlayerEntity().getBattleMonsterList();
			battleMonsterList.remove(Integer.valueOf(monsterEntity.getId()));

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
	 * 发送邮件
	 */
	public boolean SendMail(MailInfo mailInfo, int receiverId, Source source, Action action) {
		int mailId = MailUtil.SendMail(mailInfo, receiverId, getId(), getName(), source, action);
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
	 * 首次登录
	 */
	public void onFirstLogin() {
		StatisticsEntity statisticsEntity = playerData.loadStatistics();
		// default skill point
		statisticsEntity.setSkillPoint(10);
		statisticsEntity.notifyUpdate(true);

		// TEST ----------------------------------------------------------------------------------------
		playerData.loadAllMonster();
		// default monster
		if (statisticsEntity.getMonsterMaxCount() == 0) {
			increaseMonster("xgXiyiren", 1, Action.SYSTEM);
			increaseMonster("xgHuapo", 1, Action.SYSTEM);
			increaseMonster("xgPanshen", 1, Action.SYSTEM);
			increaseMonster("xgMantuoluo", 1, Action.SYSTEM);
			increaseMonster("xgAmute", 1, Action.SYSTEM);
			increaseMonster("xgWagui", 1, Action.SYSTEM);
			increaseMonster("xgHetong", 1, Action.SYSTEM);
			increaseMonster("xgLonggongtongzi", 1, Action.SYSTEM);
			increaseMonster("xgLangren", 1, Action.SYSTEM);
			increaseMonster("xgAnpusha", 1, Action.SYSTEM);
			increaseMonster("xgKairuisi", 1, Action.SYSTEM);
			increaseMonster("xgLuoxinfu", 1, Action.SYSTEM);
			increaseMonster("xgMomo", 1, Action.SYSTEM);
			increaseMonster("xgHapi", 1, Action.SYSTEM);
		}
		// TEST END-------------------------------------------------------------------------------------
	}

	/**
	 * 每日首次登录
	 */
	public void onFirstLoginDaily(boolean sync) {
		StatisticsEntity statisticsEntity = playerData.loadStatistics();
		// 保存重置时间
		playerData.getPlayerEntity().setResetTime(HawkTime.getCalendar());
		playerData.getPlayerEntity().notifyUpdate(true);

		// 同步
		if (sync) {
			sendProtocol(HawkProtocol.valueOf(HS.code.STATISTICS_INFO_SYNC_S, BuilderUtil.genStatisticsBuilder(statisticsEntity)));
		}

//		// 登陆信息上报
//		HawkReportService.LoginData loginData = new HawkReportService.LoginData(getPuid(), getDevice(), getId(), HawkTime.getTimeString());
//		HawkReportService.getInstance().report(loginData);
	 }
	 
	/**
	 * 刷新数据
	 */
	private void onRefresh() {
		Calendar curTime = HawkTime.getCalendar();

		StatisticsEntity statisticsEntity = playerData.getStatisticsEntity();
		if (null == statisticsEntity) {
			return;
		}

		List<Integer> refreshList = new ArrayList<Integer>();

		for (int i = GsConst.RefreshType.PERS_REFRESH_BEGIN + 1; i < GsConst.RefreshType.PERS_REFRESH_END; ++i) {
			TimeCfg timeCfg = HawkConfigManager.getInstance().getConfigByKey(TimeCfg.class, i);
			if (null != timeCfg) {
				try {
					boolean  shouldRefresh = false;
					Calendar nextRefreshTime = HawkTime.getCalendar();
					Calendar lastRefreshTime = statisticsEntity.getLastRefreshTime(i);
					if (null == lastRefreshTime) {
						lastRefreshTime = HawkTime.getCalendar();
						lastRefreshTime.setTimeInMillis(0);
					}

					shouldRefresh = TimeUtil.getNextRefreshTime(timeCfg, curTime, lastRefreshTime, nextRefreshTime);
					if (true == shouldRefresh) {
						statisticsEntity.setRefreshTime(i, nextRefreshTime);
						refreshList.add(i);
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
		}

		if (false == refreshList.isEmpty()) {
			statisticsEntity.notifyUpdate(true);

			for (Entry<Integer, HawkObjModule> entry : objModules.entrySet()) {
				PlayerModule playerModule = (PlayerModule) entry.getValue();
				try {
					playerModule.onRefresh(refreshList);
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
		}
	}
}
