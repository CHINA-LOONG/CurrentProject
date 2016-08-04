package com.hawk.game.entity;

import java.util.Calendar;
import java.util.Date;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.log.BehaviorLogger.Action;
import com.sun.org.apache.bcel.internal.util.Objects;

/**
 * @author zs
 *系统公会实体
 */
@Entity
@Table(name = "alliance")
@SuppressWarnings("serial")
public class AllianceEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "playerId")
	private int playerId = 0;
	
	@Column(name = "playerName")
	private String playerName = "";
	
	@Column(name = "name")
	private String name;
	
	@Column(name = "level")
	private int level = 0;
	
	@Column(name = "exp")
	private int exp = 0;
	
	@Column(name = "joinLimit")
	private int joinLimit = 0;
	
	@Column(name = "notice")
	private String notice;
	
	@Column(name = "createAllianceTime")
	private long createAllianceTime = 0;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	
	@Column(name = "updateTime")
	protected int updateTime;
	
	/**
	 * 0:正常,1:为删除状态
	 */
	@Column(name = "invalid")
	protected boolean invalid;
	
	/**
	 * 成员列表
	 */
	@Transient
	private Set<Integer> memberList;
	
	public AllianceEntity() {
		memberList = new HashSet<Integer>();
	}
	
	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
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

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
	}

	public int getExp() {
		return exp;
	}

	public void setExp(int exp) {
		this.exp = exp;
	}

	public int getJoinLimit() {
		return joinLimit;
	}

	public void setJoinLimit(int joinLimit) {
		this.joinLimit = joinLimit;
	}

	public String getNotice() {
		return notice;
	}

	public void setNotice(String notice) {
		this.notice = notice;
	}

	public Set<Integer> getMemberList() {
		return memberList;
	}

	public void addMember(int memberId) {
		memberList.add(memberId);
	}

	public String getPlayerName() {
		return playerName;
	}

	public void setPlayerName(String playerName) {
		this.playerName = playerName;
	}

	public long getCreateAllianceTime() {
		return createAllianceTime;
	}

	public void setCreateAllianceTime(long createAllianceTime) {
		this.createAllianceTime = createAllianceTime;
	}

	@Override 
	public int hashCode() {
		return Objects.hashCode(name);
	};
	
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
}
