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

import com.hawk.game.protocol.Monster.HSMonsterDefence;

@Entity
@Table(name = "pvp_defence")
public class PVPDefenceEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "playerId")
	private int playerId = 0;
	
	@Column(name = "name")
	private String name = null;
	
	@Column(name = "level")
	private int level = 0;
	
	@Column(name = "monsterInfo", nullable = false)
	private byte[] monsterInfo = null;
	
	@Column(name = "BP")
	private int BP = 0;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid;	
	
	@Transient
	HSMonsterDefence.Builder monsterDefenceBuilder = null;

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

	public void setMonsterDefenceBuilder(HSMonsterDefence.Builder monsterDefenceBuilder) {
		if (monsterDefenceBuilder != null) {
			this.monsterInfo = monsterDefenceBuilder.build().toByteArray();
			this.monsterDefenceBuilder = monsterDefenceBuilder;
		}
	}

	public HSMonsterDefence.Builder getMonsterDefenceBuilder() {
		return monsterDefenceBuilder;
	}

	public void convertMonsterDefenceBuilder(){
		try {
			if (monsterInfo == null) {
				this.monsterDefenceBuilder = null;
			}
			else {
				HSMonsterDefence monsterDefence = HSMonsterDefence.parseFrom(monsterInfo);
				HSMonsterDefence.Builder builder = monsterDefence.toBuilder();		
				this.monsterDefenceBuilder = builder;
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
	
	public boolean hasSetDefenceMonsters(){
		return monsterDefenceBuilder != null && monsterDefenceBuilder.getMonsterInfoBuilderList().size() > 0;
	}
	
	public int getBP() {
		return BP;
	}

	public void setBP(int bP) {
		BP = bP;
	}

	@Override
	public boolean decode() {
		this.convertMonsterDefenceBuilder();
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
