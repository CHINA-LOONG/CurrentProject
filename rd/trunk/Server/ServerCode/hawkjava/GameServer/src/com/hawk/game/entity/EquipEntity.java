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

import net.sf.json.JSON;
import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;

import com.hawk.game.attr.Attribute;

/**
 * 玩家基础数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "item")
@SuppressWarnings("serial")
public class EquipEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private long id = 0;

	@Column(name = "itemId")
	private int itemId = 0;

	@Column(name = "status")
	protected byte status = 0;
	
	@Column(name = "playerId")
	protected int playerId = 0;
	
	@Column(name = "monsterId")
	protected int monsterId = -1;
	
	@Column(name = "stage")
	protected short stage = 1;
	
	@Column(name = "level")
	protected byte level = 0;
	
	@Column(name = "gemDress")
	protected String gemDress = null;

	@Column(name = "additionAttr")
	protected String additionAttr = null;
	
	@Column(name = "expireTime")
	protected Date expireTime = null;

	@Column(name = "createTime", nullable = false)
	protected Date createTime = null;

	@Column(name = "updateTime")
	protected Date updateTime;

	@Column(name = "invalid")
	protected boolean invalid;

	@Transient
	protected Map<Integer, Integer> gemDressMap = new HashMap<Integer, Integer>();

	@Transient
	Attribute attr ;
	
	@Transient
	private boolean assembleFinish = false;
	
	public EquipEntity() {
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public EquipEntity(int itemId, byte status, short slot, short count, byte stage, byte level, Date expireTime) {
		this.status = status;
		this.stage = stage;
		this.level = level;
		this.expireTime = expireTime;
		this.createTime = HawkTime.getCalendar().getTime();
	}

	public long getId() {
		return id;
	}

	public void setId(long id) {
		this.id = id;
	}

	public int getItemId() {
		return itemId;
	}

	public void setItemId(int itemId) {
		this.itemId = itemId;
	}
	
	public byte getStatus() {
		return status;
	}

	public void setStatus(int status) {
		this.status = (byte)status;
	}

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}
	
	public int getMonsterId() {
		return monsterId;
	}

	public void setMonsterId(int monsterId) {
		this.monsterId = monsterId;
	}

	public short getStage() {
		return stage;
	}

	public void setStage(int stage) {
		this.stage = (short)stage;
	}

	public byte getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = (byte) level;
	}

	public Map<Integer, Integer> GetGemDressMap() {
		if (assembleFinish == false) {
			gemDressJsonToMap();
		}

		return gemDressMap;
	}
	
	private void gemDressJsonToMap(){		
		if (gemDress == null) {
			return;
		}
		
		JSONObject gemJson = JSONObject.fromObject(gemDress);
		if (gemJson == null) {
			HawkException.catchException(new HawkException("db gem data invalid: " + this.id));
		}
		else {
			for (Object k : gemJson.keySet()) {
				Object v = gemJson.get(k);
				gemDressMap.put((Integer)k, (Integer)v);
			}
		} 
		assembleFinish = true;
	}

	private void gemMapToJson() {
		//强制生成MAP
		GetGemDressMap();
		
		JSONObject gemJson = new JSONObject();
		for (Map.Entry<Integer, Integer> entry : gemDressMap.entrySet()) {
			gemJson.put(entry.getKey(), entry.getValue());
		}

		gemDress = gemJson.toString();
	}
	
	public void addGem(int index, int gemId)
	{
		// TODO valid of gemId
		gemDressMap.put(index, gemId);
	}
	
	public void remvoeGem(int index)
	{
		// TODO valid of gemId
		gemDressMap.remove(index);
	}
	
	public Attribute getAttr() {
		if (attr == null) {
			attr = Attribute.valueOf(additionAttr);
		}
		return attr;
	}
	
	public void seriAttr()	{
		// 强制生成Attr
		getAttr();
		additionAttr = attr.toString();
	}
	
	public Date getExpireTime() {
		return expireTime;
	}

	public void setExpireTime(Date expireTime) {
		this.expireTime = expireTime;
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
	
	@Override
	public void notifyUpdate(boolean async) {
		gemMapToJson();
		seriAttr();
		super.notifyUpdate(async);
	}
}
