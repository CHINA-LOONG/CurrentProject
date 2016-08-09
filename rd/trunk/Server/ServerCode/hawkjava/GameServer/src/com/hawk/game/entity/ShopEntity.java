package com.hawk.game.entity;

import java.util.Calendar;
import java.util.LinkedList;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import net.sf.json.JSONArray;

import org.hawk.db.HawkDBEntity;

import com.hawk.game.item.ShopItemInfo;
import com.hawk.game.protocol.Const;

/**
 * @author zs
 * 商店实体
 */
@Entity
@Table(name = "shop")
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
	
	@Column(name = "towerShopId")
	protected int towerShopId = 0;

	@Column(name = "towerRefreshDate")
	protected Calendar towerRefreshDate = null;

	@Column(name = "towerRefreshNums")
	protected int towerRefreshNums = 0;

	@Column(name = "towerShopItems")
	protected String towerShopItems = null;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	
	@Column(name = "updateTime")
	protected int updateTime;
	
	@Column(name = "invalid")
	protected boolean invalid;
	
	@Transient
	private List<ShopItemInfo> normalShopItemsList = null;
	
	@Transient
	private List<ShopItemInfo> allianceShopItemsList = null;

	@Transient
	private List<ShopItemInfo> towerShopItemsList = null;
	
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

	public int getTowerShopId() {
		return towerShopId;
	}

	public void setTowerShopId(int towerShopId) {
		this.towerShopId = towerShopId;
	}

	public Calendar getTowerRefreshDate() {
		return towerRefreshDate;
	}

	public void setTowerRefreshDate(Calendar towerRefreshDate) {
		this.towerRefreshDate = towerRefreshDate;
	}

	public int getTowerRefreshNums() {
		return towerRefreshNums;
	}

	public void setTowerRefreshNums(int towerRefreshNums) {
		this.towerRefreshNums = towerRefreshNums;
	}

	public String getTowerShopItems() {
		return towerShopItems;
	}

	public void setTowerShopItems(String towerShopItems) {
		this.towerShopItems = towerShopItems;
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

	public List<ShopItemInfo> getShopItemsList(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			return normalShopItemsList;
		case Const.shopType.ALLIANCESHOP_VALUE:
			return allianceShopItemsList;
		case Const.shopType.TOWERSHOP_VALUE:
			return towerShopItemsList;
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
		case Const.shopType.TOWERSHOP_VALUE:
			towerShopItemsList = shopItemsList;
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
		case Const.shopType.TOWERSHOP_VALUE:
			return towerShopId;
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
		case Const.shopType.TOWERSHOP_VALUE:
			towerShopId += 1;
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
		case Const.shopType.TOWERSHOP_VALUE:
			return towerRefreshNums;
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
		case Const.shopType.TOWERSHOP_VALUE:
			towerRefreshNums += 1;
			break;
		default:
		}
	}
	
	public Calendar getRefreshDate(int type) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			return normalRefreshDate;
		case Const.shopType.ALLIANCESHOP_VALUE:
			return allianceRefreshDate;
		case Const.shopType.TOWERSHOP_VALUE:
			return towerRefreshDate;
		default:
			return null;
		}
	}
	
	public void setRefreshDate(int type, Calendar refreshData) {
		switch (type) {
		case Const.shopType.NORMALSHOP_VALUE:
			normalRefreshDate = refreshData;
			break;
		case Const.shopType.ALLIANCESHOP_VALUE:
			allianceRefreshDate = refreshData;
			break;
		case Const.shopType.TOWERSHOP_VALUE:
			towerRefreshDate = refreshData;
			break;
		default:
		}
	}
	
	@Override
	public boolean decode() {
		normalShopItemsList = new LinkedList<>();
		if (normalShopItems != null && normalShopItems.isEmpty() == false) {
			String[] items = normalShopItems.trim().split(" ");
			for (String item  : items) {
				ShopItemInfo itemInfo = ShopItemInfo.valueOf(item);
				if (itemInfo != null) {
					normalShopItemsList.add(itemInfo);
				}
			}
		}
		
		allianceShopItemsList = new LinkedList<>();
		if (allianceShopItems != null && allianceShopItems.isEmpty() == false) {
			String[] items = allianceShopItems.trim().split(" ");
			for (String item  : items) {
				ShopItemInfo itemInfo = ShopItemInfo.valueOf(item);
				if (itemInfo != null) {
					allianceShopItemsList.add(itemInfo);
				}
			}
		}
		
		towerShopItemsList = new LinkedList<>();
		if (towerShopItems != null && towerShopItems.isEmpty() == false) {
			String[] items = towerShopItems.trim().split(" ");
			for (String item  : items) {
				ShopItemInfo itemInfo = ShopItemInfo.valueOf(item);
				if (itemInfo != null) {
					towerShopItemsList.add(itemInfo);
				}
			}
		}
		
		return true;
	}
	
	@Override
	public boolean encode() {
		StringBuilder builder = new StringBuilder();
		if (normalShopItemsList == null || normalShopItemsList.isEmpty() == true) {
			this.normalShopItems = null;
		}
		else{
			for (ShopItemInfo shopItem : normalShopItemsList) {
				builder.append(shopItem.toString()).append(" ");
			}
			this.normalShopItems = builder.toString().trim();
		}
		
		builder.setLength(0);
		if (allianceShopItemsList == null || allianceShopItemsList.isEmpty() == true) {
			this.allianceShopItems = null;
		}
		else{
			for (ShopItemInfo shopItem : allianceShopItemsList) {
				builder.append(shopItem.toString()).append(" ");
			}
			this.allianceShopItems = builder.toString().trim();
		}
		
		builder.setLength(0);
		if (towerShopItemsList == null || towerShopItemsList.isEmpty() == true) {
			this.towerShopItems = null;
		}
		else{
			for (ShopItemInfo shopItem : towerShopItemsList) {
				builder.append(shopItem.toString()).append(" ");
			}
			this.towerShopItems = builder.toString().trim();
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
	
	@Override
	public void notifyUpdate(boolean async) {
		super.notifyUpdate(async);
	}
}
