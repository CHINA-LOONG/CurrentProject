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
import org.hibernate.annotations.GenericGenerator;

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

	@Column(name = "cfg", nullable = false)
	private int cfgId = 0;

	@Column(name = "role", nullable = false)
	protected int roleId = 0;

	@Column(name = "grade", nullable = false)
	protected int grade = 0;

	@Column(name = "level", nullable = false)
	protected int level = 0;

	@Column(name = "exp", nullable = false)
	protected int exp = 0;

	@Column(name = "hasEquip", nullable = false)
	protected boolean hasEquip = false;

	@Column(name = "disposition", nullable = false)
	protected byte disposition = 0;

	@Column(name = "skillList", nullable = false)
	private String skillJsonStr = "";

	@Column(name = "createTime", nullable = false)
	protected Date createTime = null;

	@Column(name = "updateTime", nullable = false)
	protected Date updateTime = null;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	@Transient
	protected Map<Integer, Integer> skillMap = new HashMap<Integer, Integer>();

	public MonsterEntity() {
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public MonsterEntity(int cfgId, int roleId, int grade, int level, int exp, boolean hasEquip, byte disposition) {
		this.cfgId = cfgId;
		this.roleId = roleId;
		this.grade = grade;
		this.level = level;
		this.exp = exp;
		this.hasEquip = hasEquip;
		this.disposition = disposition;
		this.createTime = HawkTime.getCalendar().getTime();
	}

	private void SkillJsonToMap(){
		JSONArray jsonArray = JSONArray.fromObject(skillJsonStr);
		JSONObject skill = null;
		int skillId = 0;
		int skillLevel = 0;
		skillMap.clear();

		for (int i = 0; jsonArray != null && jsonArray.isArray() && i < jsonArray.size(); ++i) {
			skill = jsonArray.getJSONObject(i);

			try {
				if (skill.containsKey("id") == false || skill.containsKey("level") == false) {
					throw new Exception();
				}

				skillId = skill.getInt("id");
				skillLevel = skill.getInt("level");
			} catch (Exception e) {
				HawkException.catchException(new HawkException("db monster skill data invalid: " + this.id));
			}

			skillMap.put(skillId, skillLevel);
		}
	}

	private void SkillMapToJson() {
		JSONArray jsonArray = new JSONArray();
		JSONObject skill = new JSONObject();

		for (Map.Entry<Integer, Integer> entry : skillMap.entrySet()) {
			skill.put("id", entry.getKey());
			skill.put("level", entry.getValue());

			jsonArray.add(skill);
		}

		skillJsonStr = jsonArray.toString();
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
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

	public int getGrade() {
		return grade;
	}

	public void setGrade(int grade) {
		this.grade = grade;
	}

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
	}

	public int getExp() {
		return exp;
	}

	public void setExp(int exp) {
		this.exp = exp;
	}

	public boolean getHasEquip() {
		return hasEquip;
	}

	public void setHasEquip(boolean hasEquip) {
		this.hasEquip = hasEquip;
	}

	public byte getDisposition() {
		return disposition;
	}

	public void setDisposition(byte disposition) {
		this.disposition = disposition;
	}

	public Map<Integer, Integer> getSkillMap() {
		SkillJsonToMap();
		return skillMap;
	}

	public Integer getSkillLevel(int skillId) {
		return skillMap.get(skillId);
	}

	public void setSkillLevel(int skillId, int level) {
		skillMap.put(skillId, level);
		SkillMapToJson();
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
