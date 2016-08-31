package com.hawk.game.entity;

import java.util.HashMap;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;

/**
 * @author zs
 *角色持有公会实体
 */

@Entity
@Table(name = "player_alliance")
public class PlayerAllianceEntity extends HawkDBEntity {
	
	/**
	 * 奖励信息
	 *
	 */
	public static class BaseMonsterInfo{
		private int monsterId;
		private int reward;
		
		public BaseMonsterInfo(int monsterId, int reward) {
			super();
			this.monsterId = monsterId;
			this.reward = reward;
		}
		public int getMonsterId() {
			return monsterId;
		}
		
		public int getReward() {
			return reward;
		}

		public void addReward(int reward) {
			this.reward += reward;
		}
		
	}
	
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "allianceId")
	private int allianceId = 0;
	
	@Column(name = "preAllianceId")
	private int preAllianceId = 0;
	
	@Column(name = "playerId")
	private int playerId = 0;
	
	@Column(name = "name")
	private String name = null;
	
	@Column(name = "allianceBase")
	private String allianceBase = null;

	@Column(name = "level")
	private int level = 0;
	
	@Column(name = "fatigueCount")
	private int fatigueCount = 0;
	
	@Column(name = "fatigue", nullable = false)
	private String fatigue = "";
	
	@Column(name = "contribution")
	private int contribution = 0;
	
	@Column(name = "totalContribution")
	private int totalContribution = 0;

	/**
	 * 0:普通成员,1:副会长,2:会长
	 */
	@Column(name = "postion")
	private int postion;
	
	@Column(name = "exitTime")
	private int exitTime = 0;
	
	@Column(name = "logoutTime")
	private int logoutTime = 0;
	
	@Column(name = "loginTime")
	private int loginTime = 0;
	
	@Column(name = "joinTime")
	private int joinTime = 0;

	@Column(name = "refreshTime")
	private int refreshTime = 0;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	
	@Column(name = "updateTime")
	protected int updateTime;
	
	@Column(name = "invalid")
	protected boolean invalid;
	
	@Transient
	protected Set<Integer> fatigueSet = new HashSet<Integer>();

	@Transient
	protected Map<Integer, BaseMonsterInfo> allianceBaseMap = new HashMap<Integer, BaseMonsterInfo>();
	
	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getAllianceId() {
		return allianceId;
	}

	public void setAllianceId(int allianceId) {
		this.allianceId = allianceId;
	}

	public int getPreAllianceId() {
		return preAllianceId;
	}

	public void setPreAllianceId(int preAllianceId) {
		this.preAllianceId = preAllianceId;
	}

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}
	
	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public int getFatigueCount() {
		return fatigueCount;
	}

	public void addFatigueCount() {
		++fatigueCount;
	}
	
	public void clearFatigueCount() {
		fatigueCount = 0;
	}
	
	public Set<Integer> getFatigueSet() {
		return fatigueSet;
	}

	public synchronized void addFatigueSet(int playerId) {
		fatigueSet.add(playerId);
	}
	
	public synchronized void clearFatigueSet() {
		fatigueSet.clear();
	}

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
	}

	public int getContribution() {
		return contribution;
	}

	public void setContribution(int contribution) {
		this.contribution = contribution;
	}

	public int getTotalContribution() {
		return totalContribution;
	}

	public void setTotalContribution(int totalContribution) {
		this.totalContribution = totalContribution;
	}
	
	public int getPostion() {
		return postion;
	}

	public void setPostion(int postion) {
		this.postion = postion;
	}

	public int getLoginTime() {
		return loginTime;
	}

	public void setLoginTime(int loginTime) {
		this.loginTime = loginTime;
	}

	public int getLogoutTime() {
		return logoutTime;
	}

	public void setLogoutTime(int logoutTime) {
		this.logoutTime = logoutTime;
	}

	public long getExitTime() {
		return exitTime;
	}

	public void setExitTime(int exitTime) {
		this.exitTime = exitTime;
	}

	public long getJoinTime() {
		return joinTime;
	}

	public void setJoinTime(int joinTime) {
		this.joinTime = joinTime;
	}

	public long getRefreshTime() {
		return refreshTime;
	}

	public void setRefreshTime(int refreshTime) {
		this.refreshTime = refreshTime;
	}
	
	public void addAllianceBase(int position, AllianceBaseEntity baseEntity){
		BaseMonsterInfo rewardInfo = new BaseMonsterInfo(baseEntity.getMonsterBuilder().getMonsterId(), 0);
		allianceBaseMap.put(position, rewardInfo);
	}
	
	public void removeAllianceBase(int position){
		allianceBaseMap.remove(position);
	}
	
	public boolean isBasePositionHasMonster(int position){
		return allianceBaseMap.containsKey(position);
	}
	
	public BaseMonsterInfo getBaseMonsterInfo(int position){
		return allianceBaseMap.get(position);
	}
	
	public Map<Integer, BaseMonsterInfo> getBaseMonsterInfo(){
		return allianceBaseMap;
	}
	
	@Override
	public int getCreateTime() {
		return createTime;
	}
	
	@Override
	public void setCreateTime(int createTime) {
		this.createTime = createTime;
	}
	
	@Override
	public int getUpdateTime() {
		return updateTime;
	}

	@Override
	public void setUpdateTime(int updateTime) {
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
	
	@Override
	public void notifyUpdate(boolean async) {
		super.notifyUpdate(async);
	}
	
	@Override
	public boolean decode() {
		fatigueSet.clear();		
		if (fatigue != null && false == "".equals(fatigue) && false == "null".equals(fatigue)) {
			String[] result = fatigue.trim().split(" ");
			for (String element : result) {
				fatigueSet.add(Integer.parseInt(element));
			}
		}
		
		allianceBaseMap.clear();
		if (allianceBase != null && false == "".equals(allianceBase) && false == "null".equals(allianceBase)) {
			String[] result = allianceBase.trim().split(" ");
			for (String element : result) {
				String[] monsterInfo = element.split(",");
				if (monsterInfo.length == 3) {
					BaseMonsterInfo rewardInfo = new BaseMonsterInfo(Integer.valueOf(monsterInfo[1]), Integer.valueOf(monsterInfo[2]));
					allianceBaseMap.put(Integer.valueOf(monsterInfo[0]), rewardInfo);
				}
			}
		}
		
		return true;
	}
	
	@Override
	public boolean encode() {
		fatigue = "";
		for (int element : fatigueSet) {
			fatigue += String.valueOf(element) + " ";
		}
		fatigue.trim();
		
		allianceBase = "";
		for (Map.Entry<Integer, BaseMonsterInfo> entry : allianceBaseMap.entrySet()) {
			allianceBase += entry.getKey() + "," + entry.getValue().getMonsterId() + "," + entry.getValue().getReward() + " ";
		}
		allianceBase.trim();
		
		return true;
	}
}
