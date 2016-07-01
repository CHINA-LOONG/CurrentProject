package com.hawk.game.entity;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import net.sf.json.JSONObject;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;
@Entity
@Table(name = "role")
@SuppressWarnings("serial")
public class RoleEntity extends HawkDBEntity{
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "playerId")
	protected int playerId = 0;

	@Column(name = "roleIndex")
	protected byte roleIndex = 0;
	
	@Column(name = "career")
	protected byte career = 0;
	
	@Column(name = "level")
	protected short level = 0;
	
	@Column(name = "nickname", nullable = false)
	protected String nickname = "";

	@Column(name = "asset", nullable = false)
	protected String asset = "";

	@Column(name = "exp")
	protected int exp = 0;
	
	@Column(name = "gold")
	protected int gold = 0;

	@Column(name = "fatigue")
	protected int fatigue = 0;
	
	@Column(name = "bagCapacity")
	protected int recharge = 0;

	@Column(name = "createTime")
	protected Date createTime = null;

	@Column(name = "updateTime")
	protected Date updateTime = null;
	
	@Column(name = "invalid")
	protected boolean invalid;	
	
	public RoleEntity() {
		this.createTime = HawkTime.getCalendar().getTime();
		this.updateTime = HawkTime.getCalendar().getTime();
	}
	
	public RoleEntity(String nickname, int career, int gender, int eye, int hair, int hairColor, int index) {
		this.nickname = nickname;
		this.career = (byte)career;
		this.roleIndex = (byte)index;
		JSONObject assetJson = new JSONObject();
		assetJson.put("gender", gender);
		assetJson.put("eye", hair);
		assetJson.put("hair", hair);
		assetJson.put("hairColor", hairColor);
		this.asset = assetJson.toString();
		this.createTime = HawkTime.getCalendar().getTime();
		this.updateTime = HawkTime.getCalendar().getTime();
	}
	
	public int getRoleID() {
		return id;
	}
	
	public Date getCreateTime() {
		return createTime;
	}

	public void setCreateTime(Date createTime) {
		this.createTime = createTime;
	}

	public Date getUpdateTime() {
		return updateTime;
	}

	public void setUpdateTime(Date updateTime) {
		this.updateTime = updateTime;
	}

	public boolean isInvalid() {
		return invalid;
	}

	public void setInvalid(boolean invalid) {
		this.invalid = invalid;
	}
}
