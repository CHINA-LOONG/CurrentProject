package com.hawk.game.entity;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;
@Entity
@Table(name = "pvp_defence_record")
public class PVPDefenceRecordEntity extends HawkDBEntity{
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "playerId")
	private int playerId = 0;
	
	@Column(name = "point")
	private int point = 0;
	
	@Column(name = "result")
	private int result = 0;
	
	@Column(name = "attackerName")
	private String attackerName = null;
	
	@Column(name = "attackerLevel")
	private int attackerLevel = 0;
	
	@Column(name = "attackerGrade")
	private int attackerGrade = 0;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid;	
	
	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public int getPoint() {
		return point;
	}

	public void setPoint(int point) {
		this.point = point;
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
	}

	public String getAttackerName() {
		return attackerName;
	}

	public void setAttackerName(String attackerName) {
		this.attackerName = attackerName;
	}

	public int getAttackerLevel() {
		return attackerLevel;
	}

	public void setAttackerLevel(int attackerLevel) {
		this.attackerLevel = attackerLevel;
	}

	public int getAttackerGrade() {
		return attackerGrade;
	}

	public void setAttackerGrade(int attackerGrade) {
		this.attackerGrade = attackerGrade;
	}

	public int getId() {
		return id;
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
