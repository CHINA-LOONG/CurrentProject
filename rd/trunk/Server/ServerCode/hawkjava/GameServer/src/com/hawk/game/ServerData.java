package com.hawk.game;

import java.util.Calendar;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicInteger;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.HoleCfg;
import com.hawk.game.entity.ServerDataEntity;
import com.hawk.game.util.GsConst;

/**
 * 服务器数据
 * 
 * @author hawk
 */
public class ServerData {
	/**
	 * 日志记录器
	 */
	private static final Logger logger = LoggerFactory.getLogger("Server");
	/**
	 * 注册玩家数
	 */
	private AtomicInteger registerPlayer;
	/**
	 * 在线玩家数
	 */
	private AtomicInteger onlinePlayer;
	/**
	 * 释放玩家数量
	 */
	private AtomicInteger releasePlayer;
	/**
	 * 回收玩家数量
	 */
	private AtomicInteger gcPlayer;
	/**
	 * puid和玩家id的映射表
	 */
	protected ConcurrentHashMap<String, Integer> puidMap;
	/**
	 * 玩家id和puid的映射表
	 */
	protected ConcurrentHashMap<Integer, String> idMap;
	/**
	 * 玩家名和玩家id的映射表
	 */
	protected ConcurrentHashMap<String, Integer> nameMap;
	/**
	 * 玩家id和语言映射表
	 */
	protected ConcurrentHashMap<Integer, String> langMap;
	/**
	 * 在线玩家列表
	 */
	protected ConcurrentHashMap<Integer, Integer> onlineMap;
	/**
	 * 无效设备
	 */
	protected ConcurrentHashMap<String, String> disablePhoneMap;
	/**
	 * 系统刷新时间的映射表
	 * 只有主线程使用
	 */
	protected HashMap<Integer, Calendar> refreshTimeMap;
	/**
	 * 刷新时间缓存（毫秒）
	 * 只有主线程使用
	 */
	private long[] refreshTimeCache = new long[GsConst.Refresh.SysTimePointArray.length];
	/**
	 * 已付费订单列表
	 */
	protected Set<String> rechargeList;
	/**
	 * 洞开启状态的映射表
	 */
	protected ConcurrentHashMap<Integer, Boolean> holeStateMap;

	/**
	 * 需要落地的数据
	 */
	protected ServerDataEntity serverDataEntity;

	/**
	 * 上次信息显示时间
	 */
	protected int lastShowTime = 0;

	/**
	 * 全局对象实例
	 */
	private static ServerData instance = null;

	/**
	 * 获取全局实例对象
	 * 
	 * @return
	 */
	public static ServerData getInstance() {
		if (instance == null) {
			instance = new ServerData();
		}
		return instance;
	}

	/**
	 * 构造
	 */
	private ServerData() {
		registerPlayer = new AtomicInteger();
		onlinePlayer = new AtomicInteger();
		gcPlayer = new AtomicInteger();
		releasePlayer = new AtomicInteger();
		puidMap = new ConcurrentHashMap<String, Integer>();
		idMap = new ConcurrentHashMap<Integer, String>();
		nameMap = new ConcurrentHashMap<String, Integer>();
		langMap = new ConcurrentHashMap<Integer, String>();
		onlineMap = new ConcurrentHashMap<Integer, Integer>();
		disablePhoneMap = new ConcurrentHashMap<String, String>();
		refreshTimeMap = new HashMap<Integer, Calendar>();
		rechargeList = Collections.synchronizedSet(new HashSet<String>());
		holeStateMap = new ConcurrentHashMap<Integer, Boolean>();
		lastShowTime = HawkTime.getSeconds();

		// 洞初始状态为关闭，启动后refresh
		Map<Object, HoleCfg> holeCfgMap = HawkConfigManager.getInstance().getConfigMap(HoleCfg.class);
		for (HoleCfg hole : holeCfgMap.values()) {
			holeStateMap.put(hole.getId(), false);
		}

		List<ServerDataEntity> resultList = HawkDBManager.getInstance().query("from ServerDataEntity where invalid = 0");
		if (resultList != null && resultList.size() > 0) {
			serverDataEntity = resultList.get(0);
		}

		if (serverDataEntity == null) {
			serverDataEntity = new ServerDataEntity();
			serverDataEntity.setPvpWeekRefreshTime(0);
			serverDataEntity.setPvpWeekRewardCount(0);
			serverDataEntity.notifyCreate();
		}

		if (serverDataEntity != null) {
			setLastRefreshTime(GsConst.PVP.PVP_WEEK_REFRESH_TIME_ID, HawkTime.getCalendar(serverDataEntity.getPvpWeekRefreshTime()));
		}
	}

	/**
	 * 初始化服务器数据
	 */
	public boolean init() {
		// 从db拉取玩家个数
		try {
			HawkLog.logPrintln("load player count from db......");
			long count = HawkDBManager.getInstance().count("select count(*) from PlayerEntity");
			registerPlayer.set((int) count);
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		// 从db拉取玩家puid和id的映射表
		try {
			HawkLog.logPrintln("load puid and playerId from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select puid, id from player");
			for (Object rowInfo : rowInfos) {
				Object[] colInfos = (Object[]) rowInfo;
				addPuidAndPlayerId((String) colInfos[0], (Integer) colInfos[1]);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		//
		try {
			HawkLog.logPrintln("load orderSerial from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select orderSerial from recharge");
			for (Object rowInfo : rowInfos) {
				addOrderSerial((String)rowInfo);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		// 从db拉取玩家name和id的映射表
		try {
			HawkLog.logPrintln("load nickname and playerId from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select nickname, id from player");
			for (Object rowInfo : rowInfos) {
				Object[] colInfos = (Object[]) rowInfo;
				addNameAndPlayerId((String) colInfos[0], (Integer) colInfos[1]);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		// 从db拉取玩家id和language的映射表
		try {
			HawkLog.logPrintln("load playerId and language from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select id, language from player");
			for (Object rowInfo : rowInfos) {
				Object[] colInfos = (Object[]) rowInfo;
				addPlayerIdAndLang((Integer) colInfos[0], (String) colInfos[1]);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		return true;
	}

	/**
	 * 增加注册玩家数
	 */
	public int addRegisterPlayer() {
		return registerPlayer.addAndGet(1);
	}

	/**
	 * 获取注册玩家数
	 */
	public int getRegisterPlayer() {
		return registerPlayer.get();
	}

	/**
	 * 增加在线玩家数
	 */
	public int addOnlinePlayer() {
		return onlinePlayer.addAndGet(1);
	}

	/**
	 * 获取在线玩家数
	 */
	public int getOnlinePlayer() {
		return onlinePlayer.get();
	}

	/**
	 * 增加释放玩家数
	 */
	public int addReleasePlayer() {
		return releasePlayer.addAndGet(1);
	}

	/**
	 * 获取释放玩家数
	 */
	public int getReleasePlayer() {
		return releasePlayer.get();
	}

	/**
	 * 增加回收玩家数
	 */
	public int addGCPlayer() {
		return gcPlayer.addAndGet(1);
	}

	/**
	 * 获取回收玩家数
	 */
	public int getGCPlayer() {
		return gcPlayer.get();
	}

	/**
	 * 通过puid获取玩家id
	 */
	public int getPlayerIdByPuid(String puid) {
		Integer id = puidMap.get(puid);
		if (id != null) {
			return id;
		}
		return 0;
	}

	/**
	 * 通过玩家id获取puid
	 */
	public String getPuidByPlayerId(int playerId) {
		return idMap.get(playerId);
	}

	/**
	 * 通过昵称获取playerID
	 */
	public int getPlayerIdByNickname(String nickname){
		Integer playerId = nameMap.get(nickname);
		if (playerId == null) {
			return 0;
		}

		return playerId;
	}

	/**
	 * 增加puid和玩家id的映射
	 */
	public void addPuidAndPlayerId(String puid, int playerId) {
		puidMap.put(puid, playerId);
		idMap.put(playerId, puid);
	}

	/**
	 * 增加name和玩家id的映射
	 */
	public void addNameAndPlayerId(String name, int playerId) {
		name = name.toLowerCase();
		if (name != GsConst.UNCOMPLETE_NICKNAME) {
			nameMap.put(name, playerId);
		}
	}

	public void replaceNameAndPlayerId(String oldName, String newName, int playerId) {
		nameMap.remove(oldName.toLowerCase());
		nameMap.put(newName.toLowerCase(), playerId);
	}

	/**
	 * 增加name和玩家id的映射
	 */
	public void addPlayerIdAndLang(int playerId, String lang) {
		langMap.put(playerId, lang);
	}

	/**
	 * 是否存在玩家
	 */
	public boolean isExistPlayer(int playerId) {
		return idMap.containsKey(playerId);
	}

	/**
	 * 获取所有玩家Id
	 */
	public Set<Integer> getAllPlayerIdSet() {
		return idMap.keySet();
	}

	/**
	 * 获取玩家语言
	 */
	public String getPlayerLang(int playerId) {
		return langMap.get(playerId);
	}

	/**
	 * 增加order
	 */
	public void addOrderSerial(String orderSerial) {
		rechargeList.add(orderSerial);
	}

	/**
	 * 是否order
	 */
	public boolean isExistOrder(String orderSerial) {
		return rechargeList.contains(orderSerial);
	}

	/**
	 * 是否存在名字
	 */
	public boolean isExistName(String name) {
		return nameMap.containsKey(name.toLowerCase());
	}

	/**
	 * 添加在线id
	 */
	public void addOnlinePlayerId(int playerId) {
		onlineMap.put(playerId, playerId);
	}

	/**
	 * 移除在线id
	 */
	public void removeOnlinePlayerId(int playerId) {
		try {
			onlineMap.remove(playerId);
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 玩家在线判断
	 */
	public boolean isPlayerOnline(int playerId) {
		return onlineMap.containsKey(playerId);
	}

	/**
	 * 添加禁用设备
	 */
	public void addDisablePhone(String phoneInfo) {
		disablePhoneMap.put(phoneInfo, phoneInfo);
	}

	/**
	 * 是否为禁用设备
	 */
	public boolean isDisablePhone(String phoneInfo) {
		return disablePhoneMap.containsKey(phoneInfo);
	}

	/**
	 * 清空无效设备信息
	 */
	public void clearDisablePhone() {
		disablePhoneMap.clear();
	}

	public Calendar getLastRefreshTime(int timeCfgId) {
		return refreshTimeMap.get(timeCfgId);
	}

	public void setLastRefreshTime(int timeCfgId, Calendar time) {
		this.refreshTimeMap.put(timeCfgId, time);
		if (timeCfgId == GsConst.PVP.PVP_WEEK_REFRESH_TIME_ID) {
			serverDataEntity.setPvpWeekRefreshTime(time.getTimeInMillis());
			serverDataEntity.notifyUpdate(false);
		}
	}

	public long getCacheRefreshTime(int refreshIndex) {
		return refreshTimeCache[refreshIndex];
	}

	public void setCacheRefreshTime(int refreshIndex, long refreshTime) {
		refreshTimeCache[refreshIndex] = refreshTime;
	}

	public Map<Integer, Boolean> getHoleStateMap() {
		return Collections.unmodifiableMap(holeStateMap);
	}

	public boolean isHoleOpen(int holeId) {
		return holeStateMap.get(holeId);
	}

	public void setHoleOpen(int holeId, boolean isOpen) {
		this.holeStateMap.put(holeId, isOpen);
	}

	public int getPVPWeekRewardCount(){
		return serverDataEntity.getPvpWeekRewardCount();
	}

	public void setPVPWeekRewardCoutn(int pvpWeekRewardCount) {
		this.serverDataEntity.setPvpWeekRewardCount(pvpWeekRewardCount);
		this.serverDataEntity.notifyUpdate(true);
	}

	/**
	 * 打印服务器状态信息
	 */
	public void showServerInfo() {
		// 每分钟显示一个服务器信息
		//if (HawkTime.getSeconds() - lastShowTime >= 60) {
			lastShowTime = HawkTime.getSeconds();
			// 记录信息
			logger.info("online user: {}", onlineMap.size());
			logger.info("release user: {}", getReleasePlayer());
			logger.info("gc user: {}", getGCPlayer());
		//}
	}

}
