package com.hawk.game.entity;

import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.util.GsConst;

/**
 * 玩家统计数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "statistics")
@SuppressWarnings("serial")
public class StatisticsEntity  extends HawkDBEntity {
	@Id
	@Column(name = "playerId", unique = true)
	private int playerId = 0;

	@Column(name = "signInCount", nullable = false)
	private byte signInCount = 0;

	@Column(name = "signInRt")
	private Date signInRt = new Date();

	@Column(name = "instanceState", nullable = false)
	private String instanceStateJson = "";

	@Column(name = "instanceRt")
	private Date instanceRt = null;
	
	@Column(name = "monsterCollect", nullable = false)
	private String monsterCollectJson = "";
	
	@Column(name = "loginCount", nullable = false)
	private int loginCount = 0;
	
	@Column(name = "totalOnlineTime", nullable = false)
	private long totalOnlineTime = 0;

	@Column(name = "createTime", nullable = false)
	protected Date createTime = null;

	@Column(name = "updateTime")
	protected Date updateTime = null;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	/** 每一项刷新时间 */
	@Transient
	protected Map<Integer, Date> refreshTimeMap = new HashMap<Integer, Date>();

	@Transient
	protected Map<String, Integer> instanceStateMap = new HashMap<String, Integer> ();
	
	@Transient
	protected Set<String> monsterCollectSet = new HashSet<String> ();
	
	public StatisticsEntity() {
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public StatisticsEntity(int playerId) {
		this.playerId = playerId;
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public Date getLastRefreshTime(int type) {
		return refreshTimeMap.get(type);
	}

	public void setRefreshTime(int type, Date time) {
		this.refreshTimeMap.put(type, time);
		
		switch (type) {
		case GsConst.RefreshType.SIGN_IN_PERS_REFRESH:
			this.signInRt = time;
			break;
		case GsConst.RefreshType.INSTANCE_PERS_REFRESH:
			this.instanceRt = time;
			break;
		default:
			break;
		}
	}
	
	public byte getSignInCount() {
		return signInCount;
	}

	public void setSignInCount(byte count) {
		this.signInCount = count;
	}

	public Map<String, Integer> getInstanceStateMap() {
		return instanceStateMap;
	}

	public Integer getInstanceState(String instanceId) {
		return instanceStateMap.get(instanceId);
	}
	
	public void setInstanceState(String instanceId, int state) {
		instanceStateMap.put(instanceId, state);
	}
	
	public Set<String> getMonsterCollectSet() {
		return monsterCollectSet;
	}
	
	public void addMonsterCollect(String monsterCfgId) {
		monsterCollectSet.add(monsterCfgId);
	}
	
	public int getLoginCount() {
		return loginCount;
	}
	
	public void addLoginCount() {
		++loginCount;
	}
	
	public long getTotalOnlineTime() {
		return totalOnlineTime;
	}
	
	public void addTotalOnlineTime(long onlineTime) {
		totalOnlineTime += onlineTime;
	}
	
	@Override
	public boolean assemble() {
		refreshTimeMap.put(GsConst.RefreshType.SIGN_IN_PERS_REFRESH, signInRt);
		refreshTimeMap.put(GsConst.RefreshType.INSTANCE_PERS_REFRESH, instanceRt);
		
		if (instanceStateJson != null && false == "".equals(instanceStateJson) && false == "null".equals(instanceStateJson)) {
			instanceStateMap = HawkJsonUtil.getJsonInstance().fromJson(instanceStateJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		
		if (monsterCollectJson != null && false == "".equals(monsterCollectJson) && false == "null".equals(monsterCollectJson)) {
			monsterCollectSet = HawkJsonUtil.getJsonInstance().fromJson(monsterCollectJson, new TypeToken<HashSet<String>>() {}.getType());
		}
		
		return true;
	}

	@Override
	public boolean disassemble() {
		instanceStateJson = HawkJsonUtil.getJsonInstance().toJson(instanceStateMap);
		monsterCollectJson = HawkJsonUtil.getJsonInstance().toJson(monsterCollectSet);
		return true;
	}
	
	@Override
	public Date getCreateTime() {
		return createTime;
	}

	@Override
	public void setCreateTime(Date createTime) {
		this.createTime = createTime;
	}

	@Override
	public Date getUpdateTime() {
		return updateTime;
	}

	@Override
	public void setUpdateTime(Date updateTime) {
		this.updateTime = updateTime;
	}

	@Override
	public boolean isInvalid() {
		return invalid;
	}

	@Override
	public void setInvalid(boolean invalid) {
		this.invalid = invalid;
	}

}
