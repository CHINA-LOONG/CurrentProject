package com.hawk.game.entity;

import com.hawk.game.item.ShopItemInfo;
import com.hawk.game.protocol.Const;

import java.util.Calendar;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import net.sf.json.JSONArray;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;
/**
 * @author zs
 * 商店实体
 */
@Entity
@Table(name = "shop")
@SuppressWarnings("serial")
public class ShopEntity extends HawkDBEntity{
	@Id
	@Column(name = "playerId", unique = true)
	private int playerId;
	
	@Column(name = "normalShopId")
	protected int normalShopId = 0;
	
	@Column(name = "normalRefreshDate")
	protected Calendar normalRefreshDate = null;

	@Column(name = "normalRefreshNums")
	protected int normalRefreshNums = 0;
	
	@Column(name = "normalShopItems")
	protected String normalShopItems = null;
	
	@Column(name = "allianceShopId")
	protected int allianceShopId = 0;
	
	@Column(name = "allianceRefreshDate")
	protected Calendar allianceRefreshDate = null;

	@Column(name = "allianceRefreshNums")
	protected int allianceRefreshNums = 0;
	
	@Column(name = "allianceShopItems")
	protected String allianceShopItems = null;
	
	@Column(name = "otherShopId")
	protected int otherShopId = 0;
	
	@Column(name = "otherRefreshDate")
	protected Calendar otherRefreshDate = null;

	@Column(name = "otherRefreshNums")
	protected int otherRefreshNums = 0;
	
	@Column(name = "otherShopItems")
	protected String otherShopItems = null;
	
	@Column(name = "createTime", nullable = false)
	protected Calendar createTime = HawkTime.getCalendar();
	
	@Column(name = "updateTime")
	protected Calendar updateTime;
	
	@Column(name = "invalid")
	protected boolean invalid;
	
	@Transient
	private List<ShopItemInfo> normalShopItemsList = null;
	
	@Transient
	private List<ShopItemInfo> allianceShopItemsList = null;
	
	@Transient
	private List<ShopItemInfo> otherShopItemsList = null;
	
	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int id) {
		this.playerId = id;
	}
	
	public int getNormalShopId() {
		return normalShopId;
	}

	public void setNormalShopId(int normalShopId) {
		this.normalShopId = normalShopId;
	}

	public Calendar getNormalRefreshDate() {
		return normalRefreshDate;
	}

	public void setNormalRefreshDate(Calendar normalRefreshDate) {
		this.normalRefreshDate = normalRefreshDate;
	}

	public int getNormalRefreshNums() {
		return normalRefreshNums;
	}

	public void setNormalRefreshNums(int normalRefreshNums) {
		this.normalRefreshNums = normalRefreshNums;
	}

	public String getNormalShopItems() {
		return normalShopItems;
	}

	public void setNormalShopItems(String normalShopItems) {
		this.normalShopItems = normalShopItems;
	}

	public int getAllianceShopId() {
		return allianceShopId;
	}

	public void setAllianceShopId(int allianceShopId) {
		this.allianceShopId = allianceShopId;
	}

	public Calendar getAllianceRefreshDate() {
		return allianceRefreshDate;
	}

	public void setAllianceRefreshDate(Calendar allianceRefreshDate) {
		this.allianceRefreshDate = allianceRefreshDate;
	}

	public int getAllianceRefreshNums() {
		return allianceRefreshNums;
	}

	public void setAllianceRefreshNums(int allianceRefreshNums) {
		this.allianceRefreshNums = allianceRefreshNums;
	}

	public String getAllianceShopItems() {
		return allianceShopItems;
	}

	public void setAllianceShopItems(String allianceShopItems) {
		this.allianceShopItems = allianceShopItems;
	}

	public int getOtherShopId() {
		return otherShopId;
	}

	public void setOtherShopId(int otherShopId) {
		this.otherShopId = otherShopId;
	}

	public Calendar getOtherRefreshDate() {
		return otherRefreshDate;
	}

	public void setOtherRefreshDate(Calendar otherRefreshDate) {
		this.otherRefreshDate = otherRefreshDate;
	}

	public int getOtherRefreshNums() {
		return otherRefreshNums;
	}

	public void setOtherRefreshNums(int otherRefreshNums) {
		this.otherRefreshNums = otherRefreshNums;
	}

	public String getOtherShopItems() {
		return otherShopItems;
	}

	public void setOtherShopItems(String otherShopItems) {
		this.otherShopItems = otherShopItems;
	}

	public List<ShopItemInfo> getNormalShopItemsList() {
		return normalShopItemsList;
	}

	public void setNormalShopItemsList(List<ShopItemInfo> normalShopItemsList) {
		this.normalShopItemsList = normalShopItemsList;
	}

	public List<ShopItemInfo> getAllianceShopItemsList() {
		return allianceShopItemsList;
	}

	public void setAllianceShopItemsList(List<ShopItemInfo> allianceShopItemsList) {
		this.allianceShopItemsList = allianceShopItemsList;
	}

	public List<ShopItemInfo> getOtherShopItemsList() {
		return otherShopItemsList;
	}

	public void setOtherShopItemsList(List<ShopItemInfo> otherShopItemsList) {
		this.otherShopItemsList = otherShopItemsList;
	}

	public List<ShopItemInfo> getShopItemsList(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			return normalShopItemsList;
		case Const.shopType.ALLIANCESHOP_VALUE:
			return allianceShopItemsList;
		case Const.shopType.OTHERSHOP_VALUE:
			return otherShopItemsList;
		default:
			return null;
		}
	}

	public void setShopItemsList(int type, List<ShopItemInfo> shopItemsList) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			normalShopItemsList = shopItemsList;
			break;
		case Const.shopType.ALLIANCESHOP_VALUE:
			allianceShopItemsList = shopItemsList;
			break;
		case Const.shopType.OTHERSHOP_VALUE:
			otherShopItemsList = shopItemsList;
			break;
		default:
			
		}
	}
	
	public int getShopId(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			return normalShopId;
		case Const.shopType.ALLIANCESHOP_VALUE:
			return allianceShopId;
		case Const.shopType.OTHERSHOP_VALUE:
			return otherShopId;
		default:
			return 0;
		}
	}
	
	public void increaseShopId(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			normalShopId += 1;
			break;
		case Const.shopType.ALLIANCESHOP_VALUE:
			allianceShopId += 1;
			break;
		case Const.shopType.OTHERSHOP_VALUE:
			otherShopId += 1;
			break;
		default:
		}
	}

	public int getShopRefreshNum(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			return normalRefreshNums;
		case Const.shopType.ALLIANCESHOP_VALUE:
			return allianceRefreshNums;
		case Const.shopType.OTHERSHOP_VALUE:
			return otherRefreshNums;
		default:
			return 0;
		}
	}
	
	public void increaseShopRefreshNum(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			normalRefreshNums += 1;
			break;
		case Const.shopType.ALLIANCESHOP_VALUE:
			allianceRefreshNums += 1;
			break;
		case Const.shopType.OTHERSHOP_VALUE:
			otherRefreshNums += 1;
			break;
		default:
		}
	}
	
	public Calendar getRefreshData(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			return normalRefreshDate;
		case Const.shopType.ALLIANCESHOP_VALUE:
			return allianceRefreshDate;
		case Const.shopType.OTHERSHOP_VALUE:
			return otherRefreshDate;
		default:
			return null;
		}
	}
	
	public void setRefreshData(int type, Calendar refreshData) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			normalRefreshDate = refreshData;
			break;
		case Const.shopType.ALLIANCESHOP_VALUE:
			allianceRefreshDate = refreshData;
			break;
		case Const.shopType.OTHERSHOP_VALUE:
			otherRefreshDate = refreshData;
			break;
		default:
		}
	}
	
	@Override
	@SuppressWarnings("unchecked")
	public boolean decode() {
		JSONArray array = null;
		if (normalShopItems != null && normalShopItems.isEmpty() == false) {
			array = JSONArray.fromObject(normalShopItems);
		}
		else
		{
			array = new JSONArray();
		}
		normalShopItemsList = ((List<ShopItemInfo>) JSONArray.toCollection(array, ShopItemInfo.class));
		
		if (allianceShopItems != null && allianceShopItems.isEmpty() == false) {
			array = JSONArray.fromObject(allianceShopItems);
		}
		else
		{
			array = new JSONArray();
		}
		allianceShopItemsList = ((List<ShopItemInfo>) JSONArray.toCollection(array, ShopItemInfo.class));
		
		if (otherShopItems != null && otherShopItems.isEmpty() == false) {
			array = JSONArray.fromObject(otherShopItems);
		}
		else
		{
			array = new JSONArray();
		}
		otherShopItemsList = ((List<ShopItemInfo>) JSONArray.toCollection(array, ShopItemInfo.class));
				
		return true;
	}
	
	@Override
	public boolean encode() {
		if (normalShopItemsList == null) {
			this.normalShopItems = null;
		}
		else
		{
			JSONArray array = JSONArray.fromObject(normalShopItemsList);
			this.normalShopItems = array.toString();
		}
		
		if (allianceShopItemsList == null) {
			this.allianceShopItems = null;
		}
		else
		{
			JSONArray array = JSONArray.fromObject(allianceShopItemsList);
			this.allianceShopItems = array.toString();
		}
		
		if (otherShopItemsList == null) {
			this.otherShopItems = null;
		}
		else
		{
			JSONArray array = JSONArray.fromObject(otherShopItemsList);
			this.otherShopItems = array.toString();
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
	
	@Override
	public void notifyUpdate(boolean async) {
		super.notifyUpdate(async);
	}
}
