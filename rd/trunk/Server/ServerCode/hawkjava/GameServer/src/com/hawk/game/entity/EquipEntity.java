package com.hawk.game.entity;

import java.util.Calendar;
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
import org.hawk.util.HawkJsonUtil;
import org.hibernate.annotations.GenericGenerator;

import sun.security.x509.GeneralName;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.attr.Attribute;

/**
 * 玩家基础数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "equip")
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
	protected String additionAttr = "";
	
	@Column(name = "expireTime")
	protected Calendar expireTime = null;

	@Column(name = "createTime", nullable = false)
	protected Calendar createTime = null;

	@Column(name = "updateTime")
	protected Calendar updateTime;

	@Column(name = "invalid")
	protected boolean invalid;

	@Transient
	protected Map<Integer, Integer> gemDressMap = new HashMap<Integer, Integer>();

	@Transient
	Attribute attr = null;
	
	public EquipEntity() {
		this.createTime = HawkTime.getCalendar();
	}

	public EquipEntity(int itemId, byte status, short slot, short count, byte stage, byte level, Calendar expireTime) {
		this.status = status;
		this.stage = stage;
		this.level = level;
		this.expireTime = expireTime;
		this.createTime = HawkTime.getCalendar();
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
		return gemDressMap;
	}

	public Map<Integer, Integer> GetGemDressString() {
		gemMapToJson();
		return gemDressMap;
	}
	
	private void gemDressJsonToMap(){		
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
	}

	private void gemMapToJson() {
		if (gemDressMap.isEmpty() == false) {
			JSONObject gemJson = new JSONObject();
			for (Map.Entry<Integer, Integer> entry : gemDressMap.entrySet()) {
				gemJson.put(entry.getKey(), entry.getValue());
			}
			gemDress = gemJson.toString();
		}
		else {
			gemDress = null;
		}
	}
	
	public void addGem(int index, int gemId)
	{
		// TODO valid of gemId
		GetGemDressMap().put(index, gemId);
	}
	
	public void remvoeGem(int index)
	{
		// TODO valid of gemId
		GetGemDressMap().remove(index);
	}
	
	public Attribute getAttr() {
		return attr;
	}
	
	public void setAttr(Attribute attr){
		this.attr = attr;
	}
	
	public Calendar getExpireTime() {
		return expireTime;
	}

	public void setExpireTime(Calendar expireTime) {
		this.expireTime = expireTime;
	}

	@Override
	public boolean decode() {
		if (additionAttr != null && false == "".equals(additionAttr) && false == "null".equals(additionAttr)) {
			attr = Attribute.valueOf(additionAttr);
		}
		if (gemDress != null && false == "".equals(gemDress) && false == "null".equals(gemDress)) {
			gemDressJsonToMap();
		}
		return true;
	}

	@Override
	public boolean encode() {
		gemMapToJson();
		if(attr != null){
			this.additionAttr = attr.toString();
		}
		
		return true;
	}
	
	@Override
	public Calendar getCreateTime() {
		return createTime;
	}

	@Override
	public void setCreateTime(Calendar createTime) {
		this.createTime = createTime;
	}

	@Override
	public Calendar getUpdateTime() {
		return updateTime;
	}

	@Override
	public void setUpdateTime(Calendar updateTime) {
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
