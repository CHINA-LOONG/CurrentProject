package com.hawk.game.entity;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkException;
import org.hibernate.annotations.GenericGenerator;

import com.hawk.game.protocol.Monster.HSMonster;

@Entity
@Table(name = "allianceBase")
public class AllianceBaseEntity extends HawkDBEntity{
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "allianceId")
	private int allianceId = 0;
	
	@Column(name = "playerId")
	private int playerId = 0;

	@Column(name = "bp")
	private int bp = 0;
	
	@Column(name = "position")
	private int position = 0;
	
	@Column(name = "sendTime")
	private int sendTime = 0;
	
	@Column(name = "monsterInfo")
	private byte[] monsterInfo = null;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	
	@Column(name = "updateTime")
	protected int updateTime;

	/**
	 * 0:正常,1:为删除状态
	 */
	@Column(name = "invalid")
	protected boolean invalid;
	
	@Transient
	private HSMonster.Builder monsterBuilder;
	
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

	public int getBp() {
		return bp;
	}

	public void setBp(int bp) {
		this.bp = bp;
	}

	public int getPosition() {
		return position;
	}

	public void setPosition(int position) {
		this.position = position;
	}

	public int getSendTime() {
		return sendTime;
	}

	public void setSendTime(int sendTime) {
		this.sendTime = sendTime;
	}

	public byte[] getMonsterInfo() {
		return monsterInfo;
	}

	public void setMonsterInfo(HSMonster.Builder monsterBuilder) {
		if (monsterBuilder != null) {
			this.monsterInfo = monsterBuilder.build().toByteArray();
			this.monsterBuilder = monsterBuilder;
		}
	}
	
	public void convertMonsterBuilder() {
		try {
			HSMonster monster = HSMonster.parseFrom(monsterInfo);
			HSMonster.Builder builder = monster.toBuilder();
			this.monsterBuilder = builder;
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	public HSMonster.Builder getMonsterBuilder() {
		return monsterBuilder;
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
