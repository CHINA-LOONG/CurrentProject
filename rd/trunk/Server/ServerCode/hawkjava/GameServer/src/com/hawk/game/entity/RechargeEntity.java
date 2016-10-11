package com.hawk.game.entity;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;
@Entity
@Table(name = "recharge")
public class RechargeEntity extends HawkDBEntity{
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "orderSerial", unique = true)
	private String orderSerial = "";
	
	@Column(name = "puid", unique = true)
	private String puid = "";
	
	@Column(name = "playerId", unique = true)
	private int playerId = 0;
	
	@Column(name = "goodsId", unique = true)
	private String goodsId = "";
	
	@Column(name = "addGold", unique = true)
	private int addGold = 0;

	@Column(name = "giftGold", unique = true)
	private int giftGold = 0;
	
	@Column(name = "level", unique = true)
	private int level = 0;
	
	@Column(name = "platform", unique = true)
	private String platform = "";
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;
	
	public RechargeEntity() {
		
	}
	public RechargeEntity(String orderSerial, String puid, int playerId, String goodsId, int addGold, int giftGold, int level, String platform) {
		this.orderSerial = orderSerial;
		this.puid = puid;
		this.playerId = playerId;
		this.goodsId = goodsId;
		this.addGold = addGold;
		this.giftGold = giftGold;
		this.level = level;
		this.platform = platform;
	}
	
	public String getOrderSerial() {
		return orderSerial;
	}

	public void setOrderSerial(String orderSerial) {
		this.orderSerial = orderSerial;
	}

	public String getPuid() {
		return puid;
	}

	public void setPuid(String puid) {
		this.puid = puid;
	}

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public String getGoodsId() {
		return goodsId;
	}

	public void setGoodsId(String goodsId) {
		this.goodsId = goodsId;
	}

	public int getAddGold() {
		return addGold;
	}

	public void setAddGold(int addGold) {
		this.addGold = addGold;
	}

	public int getGiftGold() {
		return giftGold;
	}

	public void setGiftGold(int giftGold) {
		this.giftGold = giftGold;
	}

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
	}

	public String getPlatform() {
		return platform;
	}

	public void setPlatform(String platform) {
		this.platform = platform;
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
