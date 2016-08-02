package com.hawk.game.entity;

import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;

import com.google.gson.reflect.TypeToken;

/**
 * @author zs
 *角色持有公会实体
 */

@Entity
@Table(name = "player_alliance")
@SuppressWarnings("serial")
public class PlayerAllianceEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "allianceId")
	private int allianceId = 0;
	
	@Column(name = "playerId")
	private int playerId = 0;
	
	@Column(name = "contribution")
	private int contribution = 0;
	
	/**
	 * 0:普通成员,1:副会长,2:会长
	 */
	@Column(name = "postion")
	private int postion;
	
	@Column(name = "exitTime")
	private long exitTime = 0;
	
	@Column(name = "joinTime")
	private long joinTime = 0;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	
	@Column(name = "updateTime")
	protected int updateTime;
	
	@Column(name = "invalid")
	protected boolean invalid;
	
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

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public int getContribution() {
		return contribution;
	}

	public void setContribution(int contribution) {
		this.contribution = contribution;
	}

	public int getPostion() {
		return postion;
	}

	public void setPostion(int postion) {
		this.postion = postion;
	}

	public long getExitTime() {
		return exitTime;
	}

	public void setExitTime(long exitTime) {
		this.exitTime = exitTime;
	}

	public long getJoinTime() {
		return joinTime;
	}

	public void setJoinTime(long joinTime) {
		this.joinTime = joinTime;
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
}
