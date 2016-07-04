package com.hawk.game.entity;

import java.util.Date;
import java.util.HashMap;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;
import org.hibernate.annotations.GenericGenerator;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.util.GsConst;

/**
 * 怪物基础数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "monster")
@SuppressWarnings("serial")
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
	
	@Column(name = "ai", nullable = false)
	protected byte ai = 0;

	@Column(name = "skillList", nullable = false)
	private String skillJson = "";

	@Column(name = "createTime", nullable = false)
	protected Date createTime = null;

	@Column(name = "updateTime")
	protected Date updateTime = null;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	@Transient
	protected Map<Integer, Integer> skillMap = new HashMap<Integer, Integer>();

	public MonsterEntity() {
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public MonsterEntity(String cfgId, int playerId, byte stage, short level, int exp, byte lazy, byte ai) {
		this.cfgId = cfgId;
		this.playerId = playerId;
		this.stage = stage;
		this.level = level;
		this.exp = exp;
		this.lazy = lazy;
		this.ai = ai;
		this.createTime = HawkTime.getCalendar().getTime();
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

	public void setLevel(short level) {
		this.level = level;
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
	
	public byte getAi() {
		return ai;
	}

	public void setAi(byte ai) {
		this.ai = ai;
	}

	public Map<Integer, Integer> getSkillMap() {
		return skillMap;
	}

	public Integer getSkillLevel(int skillId) {
		return skillMap.get(skillId);
	}

	public void setSkillLevel(int skillId, int level) {
		skillMap.put(skillId, level);
	}

	@Override
	public boolean assemble() {
		if (skillJson != null && false == "".equals(skillJson) && false == "null".equals(skillJson)) {
			skillMap = HawkJsonUtil.getJsonInstance().fromJson(skillJson, new TypeToken<HashMap<Integer, Integer>>() {}.getType());
		}
		return true;
	}

	@Override
	public boolean disassemble() {
		skillJson = HawkJsonUtil.getJsonInstance().toJson(skillMap);
		return true;
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
