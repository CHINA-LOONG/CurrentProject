package com.hawk.game.entity;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.util.HawkJsonUtil;
import org.hibernate.annotations.GenericGenerator;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.protocol.Alliance.AllianceBaseMonster;

/**
 * 邮件数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "adventureTeam")
public class AdventureTeamEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "playerId", nullable = false)
	protected int playerId = 0;

	@Column(name = "teamId", nullable = false)
	protected int teamId = 0;

	@Column(name = "adventureId")
	protected int advenId = 0;

	@Column(name = "endTime")
	protected int endTime = 0;

	@Column(name = "selfMonster")
	protected String monsterJson = "";

	@Column(name = "hireMonsterId")
	protected int hireId = 0;

	@Column(name = "hireMonsterCfgId")
	protected String hireCfgId = "";

	@Column(name = "hireMonsterStage")
	protected int hireStage = 0;

	@Column(name = "hireMonsterLevel")
	protected int hireLevel = 0;

	@Column(name = "hireMonsterBp")
	protected int hireBp = 0;

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	@Transient
	protected List<Integer> selfMonsterIdList = new ArrayList<Integer>();

	@Transient
	protected AllianceBaseMonster hireMonster = null;

	public AdventureTeamEntity() {

	}

	public AdventureTeamEntity(int playerId, int teamId) {
		this.playerId = playerId;
		this.teamId = teamId;
	}

	public int getId() {
		return id;
	}

	public int getPlayerId() {
		return playerId;
	}

	public int getTeamId() {
		return teamId;
	}

	public int getAdventureId() {
		return advenId;
	}

	public void setAdventureId(int advenId) {
		this.advenId = advenId;
	}

	public int getEndTime() {
		return endTime;
	}

	public void setEndTime(int endTime) {
		this.endTime = endTime;
	}

	public List<Integer> getSelfMonsterList() {
		return selfMonsterIdList;
	}

	public void setSelfMonsterList(List<Integer> monsterList) {
		this.selfMonsterIdList = monsterList;
	}

	public AllianceBaseMonster getHireMonster() {
		return hireMonster;
	}

	public void setHireMonster(AllianceBaseMonster hireMonster) {
		this.hireMonster = hireMonster;
	}

	public void clear() {
		this.advenId = 0;
		this.endTime = 0;
		this.hireId = 0;
		this.hireCfgId = "";
		this.hireStage = 0;
		this.hireLevel = 0;
		this.hireBp = 0;
		this.selfMonsterIdList.clear();
		this.hireMonster = null;
	}

	@Override
	public boolean decode() {
		if (monsterJson != null && false == "".equals(monsterJson) && false == "null".equals(monsterJson)) {
			selfMonsterIdList = HawkJsonUtil.getJsonInstance().fromJson(monsterJson, new TypeToken<List<Integer>>() {}.getType());
		}

		if (0 !=advenId) {
			AllianceBaseMonster.Builder builder = AllianceBaseMonster.newBuilder();
			builder.setMonsterId(hireId);
			builder.setCfgId(hireCfgId);
			builder.setStage(hireStage);
			builder.setLevel(hireLevel);
			builder.setBp(hireBp);
			// 无用数据
			builder.setId(0);
			builder.setNickname("");
			hireMonster = builder.build();
		}

		return true;
	}

	@Override
	public boolean encode() {
		monsterJson = HawkJsonUtil.getJsonInstance().toJson(selfMonsterIdList);

		if (null != hireMonster) {
			hireId = hireMonster.getMonsterId();
			hireCfgId = hireMonster.getCfgId();
			hireStage = hireMonster.getStage();
			hireLevel = hireMonster.getLevel();
			hireBp = hireMonster.getBp();
		}

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
