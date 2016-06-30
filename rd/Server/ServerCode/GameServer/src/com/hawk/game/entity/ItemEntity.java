package com.hawk.game.entity;

import java.util.Date;

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

	@Column(name = "cfg", nullable = false)
	private int cfgId = 0;

	@Column(name = "currentRole", nullable = false)
	protected int roleId = 0;

	@Column(name = "currentCount", nullable = false)
	protected short count = 0;

	@Column(name = "initRole", nullable = false)
	protected int initRoleId = 0;

	@Column(name = "initCount", nullable = false)
	protected short initCount = 0;

	@Column(name = "status", nullable = false)
	protected short status = 0;

	@Column(name = "monster", nullable = false)
	protected int monsterId = 0;

	@Column(name = "slot", nullable = false)
	protected int slot = 0;

	@Column(name = "expireTime", nullable = false)
	protected Date expireTime = null;

	@Column(name = "createTime", nullable = false)
	protected Date createTime = null;

	@Column(name = "updateTime", nullable = false)
	protected Date updateTime;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid;

	public ItemEntity() {
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public ItemEntity(int cfgId, int roleId, int monsterId, short status, int slot, short count, Date expireTime) {
		this.cfgId = cfgId;
		this.roleId = roleId;
		this.monsterId = monsterId;
		this.status = status;
		this.slot = slot;
		this.count = count;
		this.expireTime = expireTime;

		this.initRoleId = roleId;
		this.initCount = count;
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public long getId() {
		return id;
	}

	public void setId(long id) {
		this.id = id;
	}

	public int getCfgId() {
		return cfgId;
	}

	public void setCfgId(int cfgId) {
		this.cfgId = cfgId;
	}

	public int getRoleId() {
		return roleId;
	}

	public void setRoleId(int roleId) {
		this.roleId = roleId;
	}

	public short getCount() {
		return count;
	}

	public void setCount(short count) {
		this.count = count;
	}

	public int getInitRoleId() {
		return initRoleId;
	}

	public void setInitRoleId(int initRoleId) {
		this.initRoleId = initRoleId;
	}

	public int getInitCount() {
		return initCount;
	}

	public void setInitCount(short initCount) {
		this.initCount = initCount;
	}

	public int getStatus() {
		return status;
	}

	public void setStatus(short status) {
		this.status = status;
	}

	public int getMonsterId() {
		return monsterId;
	}

	public void setMonsterId(int monsterId) {
		this.monsterId = monsterId;
	}

	public int getSlot() {
		return slot;
	}

	public void setSlot(int slot) {
		this.slot = slot;
	}

	public Date getExpireTime() {
		return expireTime;
	}

	public void setExpireTime(Date expireTime) {
		this.expireTime = expireTime;
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
