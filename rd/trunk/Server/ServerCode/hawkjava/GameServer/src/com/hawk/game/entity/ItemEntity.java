package com.hawk.game.entity;

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
 * 玩家基础数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "item")
@SuppressWarnings("serial")
public class ItemEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private long id = 0;

	@Column(name = "itemId", nullable = false)
	private String itemId = null;

	@Column(name = "count", nullable = false)
	protected int count = 0;
	
	@Column(name = "playerId")
	protected int playerId = 0;
	
	@Column(name = "status", nullable = false)
	protected byte status = 0;

	@Column(name = "expireTime")
	protected Calendar expireTime = null;

	@Column(name = "createTime", nullable = false)
	protected Calendar createTime = null;

	@Column(name = "updateTime")
	protected Calendar updateTime;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid;

	public ItemEntity() {
		this.createTime = HawkTime.getCalendar();
	}

	public ItemEntity(int itemId, byte status, short slot, short count, Calendar expireTime) {
		this.status = status;
		this.count = count;
		this.expireTime = expireTime;
		this.createTime = HawkTime.getCalendar();
	}

	public long getId() {
		return id;
	}

	public void setId(long id) {
		this.id = id;
	}

	public String getItemId() {
		return itemId;
	}

	public void setItemId(String itemId) {
		this.itemId = itemId;
	}

	public int getCount() {
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}
	
	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}
	
	public byte getStatus() {
		return status;
	}

	public void setStatus(byte status) {
		this.status = status;
	}

	public Calendar getExpireTime() {
		return expireTime;
	}

	public void setExpireTime(Calendar expireTime) {
		this.expireTime = expireTime;
	}

	@Override
	public Calendar getCreateTime() {
		return createTime;
	}

	@Override
	public void setCreateTime(Calendar createTime) {
		this.createTime = createTime;
	}

	@Override
	public Calendar getUpdateTime() {
		return updateTime;
	}

	@Override
	public void setUpdateTime(Calendar updateTime) {
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
