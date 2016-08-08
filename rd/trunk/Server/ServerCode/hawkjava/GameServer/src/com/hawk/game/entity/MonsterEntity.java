package com.hawk.game.entity;

import java.util.HashMap;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;
import org.hibernate.annotations.GenericGenerator;

import com.google.gson.reflect.TypeToken;

/**
 * 怪物基础数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "monster")
public class MonsterEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "cfgId", nullable = false)
	private String cfgId = "";

	@Column(name = "playerId", nullable = false)
	protected int playerId = 0;

	@Column(name = "stage", nullable = false)
	protected byte stage = 0;

	@Column(name = "level", nullable = false)
	protected short level = 0;

	@Column(name = "exp", nullable = false)
	protected int exp = 0;

	@Column(name = "lazy", nullable = false)
	protected byte lazy = 0;
	
	@Column(name = "lazyExp", nullable = false)
	protected int lazyExp = 0;
	
	@Column(name = "disposition", nullable = false)
	protected byte disposition = 0;

	@Column(name = "skillList", nullable = false)
	private String skillJson = "";
	
	@Column(name = "locked", nullable = false)
	protected boolean locked = false;

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	@Transient
	protected Map<String, Integer> skillMap = new HashMap<String, Integer>();

	public MonsterEntity() {
		this.createTime = HawkTime.getSeconds();
	}

	public MonsterEntity(String cfgId, int playerId, byte stage, short level, int exp, byte lazy, int lazyExp, byte disposition) {
		this.cfgId = cfgId;
		this.playerId = playerId;
		this.stage = stage;
		this.level = level;
		this.exp = exp;
		this.lazy = lazy;
		this.lazyExp = lazyExp;
		this.disposition = disposition;
		this.createTime = HawkTime.getSeconds();
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getCfgId() {
		return cfgId;
	}

	public void setCfgId(String cfgId) {
		this.cfgId = cfgId;
	}

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public byte getStage() {
		return stage;
	}

	public void setStage(byte stage) {
		this.stage = stage;
	}

	public short getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = (short)level;
	}

	public int getExp() {
		return exp;
	}

	public void setExp(int exp) {
		this.exp = exp;
	}

	public byte getLazy() {
		return lazy;
	}

	public void setLazy(byte lazy) {
		this.lazy = lazy;
	}
	
	public int getLazyExp() {
		return lazyExp;
	}

	public void setLazyExp(int lazyExp) {
		this.lazyExp = lazyExp;
	}
	
	public byte getDisposition() {
		return disposition;
	}

	public void setDisposition(byte disposition) {
		this.disposition = disposition;
	}

	public Map<String, Integer> getSkillMap() {
		return skillMap;
	}

	public Integer getSkillLevel(String skillId) {
		return skillMap.get(skillId);
	}

	public void setSkillLevel(String skillId, int level) {
		skillMap.put(skillId, level);
	}

	public boolean isLocked() {
		return locked;
	}

	public void setLocked(boolean locked) {
		this.locked = locked;
	}

	@Override
	public boolean decode() {
		if (skillJson != null && false == "".equals(skillJson) && false == "null".equals(skillJson)) {
			skillMap = HawkJsonUtil.getJsonInstance().fromJson(skillJson, new TypeToken<HashMap<String, Integer>>() {}.getType());
		}
		return true;
	}

	@Override
	public boolean encode() {
		skillJson = HawkJsonUtil.getJsonInstance().toJson(skillMap);
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
