package com.hawk.game.entity;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;
@Entity
@Table(name = "serverData")
public class ServerDataEntity extends HawkDBEntity{	
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "pvpWeakRefreshTime")
	protected long pvpWeakRefreshTime = 0;
	
	@Column(name = "pvpWeakRewardCount")
	protected int pvpWeakRewardCount = 0;	
	
	@Column(name = "createTime")
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;
	
	@Column(name = "invalid")
	protected boolean invalid;	
	
	public long getPvpWeakRefreshTime() {
		return pvpWeakRefreshTime;
	}

	public void setPvpWeakRefreshTime(long pvpWeakRefreshTime) {
		this.pvpWeakRefreshTime = pvpWeakRefreshTime;
	}

	public int getPvpWeakRewardCount() {
		return pvpWeakRewardCount;
	}

	public void setPvpWeakRewardCount(int pvpWeakRewardCount) {
		this.pvpWeakRewardCount = pvpWeakRewardCount;
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
}
