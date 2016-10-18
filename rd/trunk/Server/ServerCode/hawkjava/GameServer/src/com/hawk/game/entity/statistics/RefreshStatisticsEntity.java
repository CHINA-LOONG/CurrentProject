package com.hawk.game.entity.statistics;

import java.util.Calendar;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;

/**
 * 刷新时间统计数据
 * 
 * @author walker
 */
@Entity
@Table(name = "statistics_refresh")
public class RefreshStatisticsEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "playerId", unique = true)
	protected int playerId = 0;

	@Column(name = "timePoint", nullable = false)
	protected int timePoint;

	// 时间点X上次刷新时间
	@Column(name = "refreshTime", nullable = false)
	protected Calendar refreshTime;

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	@Column(name = "updateTime")
	protected int updateTime = 0;
	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	// decode-------------------------------------------------------------------

	// method-------------------------------------------------------------------

	protected RefreshStatisticsEntity() {
		Calendar time = HawkTime.getCalendar();
		this.refreshTime = time;
	}

	protected RefreshStatisticsEntity(int playerId, int timePoint) {
		Calendar time = HawkTime.getCalendar();
		this.playerId = playerId;
		this.timePoint = timePoint;
		this.refreshTime = time;
	}

	@Override
	public boolean decode() {
		return true;
	}

	@Override
	public boolean encode() {
		return true;
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