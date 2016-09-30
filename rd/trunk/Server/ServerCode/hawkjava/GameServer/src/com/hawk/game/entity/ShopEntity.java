package com.hawk.game.entity;

import java.util.LinkedList;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;

import com.hawk.game.item.ShopItemInfo;

/**
 * @author zs
 * 商店实体
 */
@Entity
@Table(name = "shop")
public class ShopEntity extends HawkDBEntity{
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;
	
	@Column(name = "playerId", unique = true)
	private int playerId;
	
	@Column(name = "type")
	protected int type = 0;
	
	@Column(name = "shopId")
	protected int shopId = 0;
	
	@Column(name = "refreshDate")
	protected int refreshDate = 0;

	@Column(name = "refreshNums")
	protected int refreshNums = 0;
	
	@Column(name = "shopItems")
	protected String shopItems = null;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	
	@Column(name = "updateTime")
	protected int updateTime;
	
	@Column(name = "invalid")
	protected boolean invalid;
	
	@Transient
	private List<ShopItemInfo> shopItemsList = null;
	
	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int id) {
		this.playerId = id;
	}
	
	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}

	public int getShopId() {
		return shopId;
	}

	public void setShopId(int shopId) {
		this.shopId = shopId;
	}

	public int getRefreshDate() {
		return refreshDate;
	}

	public void setRefreshDate(int refreshDate) {
		this.refreshDate = refreshDate;
	}

	public int getRefreshNums() {
		return refreshNums;
	}

	public void setRefreshNums(int refreshNums) {
		this.refreshNums = refreshNums;
	}

	public String getShopItems() {
		return shopItems;
	}

	public void setShopItems(String shopItems) {
		this.shopItems = shopItems;
	}

	public List<ShopItemInfo> getShopItemsList() {
		return shopItemsList;
	}

	public void setShopItemsList(List<ShopItemInfo> shopItemsList) {
		this.shopItemsList = shopItemsList;
	}
	
	public void increaseShopId() {
		shopId++;
	}
	
	public void increaseShopRefreshNum() {
		refreshNums++;
	}
	
	@Override
	public boolean decode() {
		shopItemsList = new LinkedList<>();
		if (shopItems != null && !shopItems.equals("")) {
			String[] items = shopItems.trim().split(" ");
			for (String item  : items) {
				ShopItemInfo itemInfo = ShopItemInfo.generateFromDB(item);
				if (itemInfo != null) {
					shopItemsList.add(itemInfo);
				}
			}
		}
		return true;
	}
	
	@Override
	public boolean encode() {
		StringBuilder builder = new StringBuilder();
		if (shopItemsList == null || shopItemsList.isEmpty() == true) {
			this.shopItems = null;
		}
		else{
			for (ShopItemInfo shopItem : shopItemsList) {
				builder.append(shopItem.toString()).append(" ");
			}
			this.shopItems = builder.toString().trim();
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
