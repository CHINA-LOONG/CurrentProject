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

	@Column(name = "pvpWeekRefreshTime")
	protected long pvpWeekRefreshTime = 0;

	@Column(name = "pvpWeekRewardCount")
	protected int pvpWeekRewardCount = 0;

	@Column(name = "createTime")
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;

	@Column(name = "invalid")
	protected boolean invalid;

	public long getPvpWeekRefreshTime() {
		return pvpWeekRefreshTime;
	}

	public void setPvpWeekRefreshTime(long pvpWeekRefreshTime) {
		this.pvpWeekRefreshTime = pvpWeekRefreshTime;
	}

	public int getPvpWeekRewardCount() {
		return pvpWeekRewardCount;
	}

	public void setPvpWeekRewardCount(int pvpWeekRewardCount) {
		this.pvpWeekRewardCount = pvpWeekRewardCount;
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
