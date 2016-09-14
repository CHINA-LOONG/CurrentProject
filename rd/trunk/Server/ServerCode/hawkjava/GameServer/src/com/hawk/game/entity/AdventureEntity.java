package com.hawk.game.entity;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;

import com.hawk.game.config.AdventureConditionTypeCfg;
import com.hawk.game.util.AdventureUtil.AdventureCondition;

/**
 * 邮件数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "adventure")
public class AdventureEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "playerId", nullable = false)
	protected int playerId = 0;

	@Column(name = "type", nullable = false)
	protected int type = 0;

	@Column(name = "gear", nullable = false)
	protected int gear = 0;

	@Column(name = "adventureId", nullable = false)
	protected int advenId = 0;

	@Column(name = "count1", nullable = false)
	protected int count1 = 0;

	@Column(name = "typeCfg1", nullable = false)
	protected int typeCfg1 = 0;

	@Column(name = "count2", nullable = false)
	protected int count2 = 0;

	@Column(name = "typeCfg2", nullable = false)
	protected int typeCfg2 = 0;

	@Column(name = "count3", nullable = false)
	protected int count3 = 0;

	@Column(name = "typeCfg3", nullable = false)
	protected int typeCfg3 = 0;

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	@Transient
	List<AdventureCondition> conditionList = new ArrayList<AdventureCondition> ();

	public AdventureEntity() {

	}

	public AdventureEntity(int playerId, int type, int gear) {
		this.playerId = playerId;
		this.type = type;
		this.gear = gear;
	}

	public int getId() {
		return id;
	}

	public int getPlayerId() {
		return playerId;
	}

	public int getType() {
		return type;
	}

	public int getGear() {
		return gear;
	}

	public int getAdventureId() {
		return advenId;
	}

	public void setAdventureId(int advenId) {
		this.advenId = advenId;
	}

	public List<AdventureCondition> getConditionList() {
		return conditionList;
	}

	public void setConditionList(List<AdventureCondition> conditionList) {
		count1 = count2 = count3 = 0;
		typeCfg1 = typeCfg2 = typeCfg3 = 0;
		this.conditionList = conditionList;
	}

	public void clearConditionList() {
		count1 = count2 = count3 = 0;
		typeCfg1 = typeCfg2 = typeCfg3 = 0;
		this.conditionList.clear();
	}

	@Override
	public boolean decode() {
		AdventureConditionTypeCfg conditionTypeCfg;
		AdventureCondition condition;

		if (0 != count1) {
			conditionTypeCfg = HawkConfigManager.getInstance().getConfigByKey(AdventureConditionTypeCfg.class, typeCfg1);
			if (null != conditionTypeCfg) {
				condition = new AdventureCondition(count1, conditionTypeCfg);
				conditionList.add(condition);
			}
		}
		if (0 != count2) {
			conditionTypeCfg = HawkConfigManager.getInstance().getConfigByKey(AdventureConditionTypeCfg.class, typeCfg2);
			if (null != conditionTypeCfg) {
				condition = new AdventureCondition(count2, conditionTypeCfg);
				conditionList.add(condition);
			}
		}
		if (0 != count3) {
			conditionTypeCfg = HawkConfigManager.getInstance().getConfigByKey(AdventureConditionTypeCfg.class, typeCfg3);
			if (null != conditionTypeCfg) {
				condition = new AdventureCondition(count3, conditionTypeCfg);
				conditionList.add(condition);
			}
		}

		return true;
	}

	@Override
	public boolean encode() {
		for (int i = 0; i < this.conditionList.size(); ++i) {
			AdventureCondition condition = conditionList.get(i);
			if (i == 0) {
				count1 = condition.monsterCount;
				typeCfg1 = condition.conditionTypeCfgId;
			} else if (i == 1) {
				count2 = condition.monsterCount;
				typeCfg2 = condition.conditionTypeCfgId;
			} else if (i == 2) {
				count3 = condition.monsterCount;
				typeCfg3 = condition.conditionTypeCfgId;
			}
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
