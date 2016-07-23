package com.hawk.game.entity;

import java.util.Arrays;
import java.util.Calendar;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;







import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;

import com.hawk.game.attr.Attribute;
import com.hawk.game.item.GemInfo;
import com.hawk.game.util.GsConst;

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
	private String itemId = null;

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
	
	@Column(name = "punchCount")
	protected byte punchCount = 0;
	
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
	protected Map<Integer, GemInfo> gemDressList = new LinkedHashMap<Integer, GemInfo>();

	@Transient
	Attribute attr = null;
	
	public EquipEntity() {
		this.createTime = HawkTime.getCalendar();
	}

	public EquipEntity(String itemId, byte status, short slot, short count, byte stage, byte level, Calendar expireTime) {
		this.itemId = itemId;
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

	public String getItemId() {
		return itemId;
	}

	public void setItemId(String itemId) {
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
	
	public void setPunchCount(int punchCount) {
		this.punchCount = (byte) punchCount;
	}

	public byte getPunchCount() {
		return punchCount;
	}
	
	public Map<Integer, GemInfo> GetGemDressList() {
		return gemDressList;
	}
	
	public String gemListToString() {
		gemDress = "";
		if (gemDressList.isEmpty() == false) {
			for (Map.Entry<Integer, GemInfo> entry : gemDressList.entrySet()) {
				gemDress = gemDress + entry.getKey() + "," + entry.getValue().getType() + "," + entry.getValue().getGemId() + "_";
			}
		}
		
		return gemDress;
	}

	private void gemDressToList(){		
		gemDressList.clear();
		if (gemDress != null && !"".equals(gemDress)) {
			for (String element : Arrays.asList(gemDress.split("_"))) {
				if (element != null && !"".equals(element)) {
					String[] result = element.split(",");
					if (result.length == 3) {
						gemDressList.put(Integer.valueOf(result[0]), new GemInfo(Integer.valueOf(result[1]), result[2]));
					}
				}
			}
		}
	}
	
	public void addGem(int slot, int type) {
		gemDressList.put(slot, new GemInfo(type, GsConst.EQUIP_GEM_NONE));
	}
	
	public void addGem(int slot, int type, String gemId)
	{
		if (gemDressList.get(slot) != null && gemDressList.get(slot).equals(GsConst.EQUIP_GEM_NONE)) {
			gemDressList.get(slot).setGemId(gemId);
		}
	}
	
	public void removeGem(int slot, String gemId)
	{
		if (gemDressList.get(slot) != null && gemDressList.get(slot).equals(gemId)) {
			gemDressList.get(slot).setGemId(GsConst.EQUIP_GEM_NONE);
		}
	}
	
	public void replaceGem(int slot, String oldGemId, String newgemId)
	{
		if (gemDressList.get(slot) != null && gemDressList.get(slot).equals(oldGemId)) {
			gemDressList.get(slot).setGemId(newgemId);
		}
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
			gemDressToList();
		}
		return true;
	}

	@Override
	public boolean encode() {
		gemListToString();
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
