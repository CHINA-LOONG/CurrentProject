package com.hawk.game.item;

import java.util.List;

import org.hawk.log.HawkLog;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.google.protobuf.ProtocolMessageEnum;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.log.BehaviorLogger;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.log.BehaviorLogger.Params;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerData;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.Const.playerAttr;
import com.hawk.game.protocol.Consume.ConsumeItem;
import com.hawk.game.protocol.Consume.HSConsumeInfo;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Player.SynPlayerAttr;
import com.hawk.game.protocol.Reward.HSRewardInfo;
import com.hawk.game.protocol.Reward.RewardItem;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.GsConst.PlayerItemCheckResult;

/**
 * @author hawk
 * 
 */
public class ConsumeItems {
	/**
	 * 消耗信息
	 */
	private HSConsumeInfo.Builder consumeInfo;

	public ConsumeItems() {
		consumeInfo = HSConsumeInfo.newBuilder();
		SynPlayerAttr.Builder playerBuilder = SynPlayerAttr.newBuilder();
		consumeInfo.setPlayerAttr(playerBuilder);
	}

	public static ConsumeItems valueOf() {
		return new ConsumeItems();
	}

	public static ConsumeItems valueOf(ProtocolMessageEnum changeType, int count) {
		ConsumeItems newConsumeItem = ConsumeItems.valueOf();
		newConsumeItem.addAttr(changeType.getNumber(), count);
		return newConsumeItem;
	}
	
	/**
	 * 设置builder
	 */
	public void setRewardInfo(HSConsumeInfo.Builder consumeInfo) {
		this.consumeInfo = consumeInfo;
	}
	
	public HSConsumeInfo.Builder getBuilder() {
		return consumeInfo;
	}
	
	/**
	 * 克隆消耗对象
	 */
	@Override
	public ConsumeItems clone() {
		ConsumeItems newConsume = new ConsumeItems();
		newConsume.setRewardInfo(consumeInfo.clone());
		return newConsume;
	}
	
	/**
	 * 判断是否有奖励
	 */
	public boolean hasConsumeItem() {
		return consumeInfo.getConsumeItemsList().size() > 0;
	}
	
	/**
	 * 生成存储字符串
	 * 
	 * @return
	 */
	public String toDbString() {
		return null;
	}

	@Override
	public String toString() {
		return null;
	}
	
	public ConsumeItems addItem(String itemId, int count) {		
		ConsumeItem.Builder consumeItem = null;
		for (ConsumeItem.Builder consume :  consumeInfo.getConsumeItemsBuilderList()) {
			if (consume.getType() == itemType.ITEM_VALUE && consume.getItemId().equals(itemId)) {
				consumeItem = consume;
				break;
			}
		}
		
		if (consumeItem == null) {
			consumeItem = ConsumeItem.newBuilder();
			consumeItem.setType(itemType.ITEM_VALUE);
			consumeItem.setItemId(itemId);
			consumeItem.setCount( count);
			consumeInfo.addConsumeItems(consumeItem);
		}
		else {
			consumeItem.setCount(consumeItem.getCount() + count);
		}
		return this;
	}

	public ConsumeItems addEquip(long id, String equipId) {
		ConsumeItem.Builder consumeItem = ConsumeItem.newBuilder();
		consumeItem.setType(Const.itemType.EQUIP_VALUE);
		consumeItem.setItemId(equipId);
		consumeItem.setId(id);
		consumeItem.setCount(1);
		consumeInfo.addConsumeItems(consumeItem);
		return this;
	}
	
	public ConsumeItems addAttr(String attrType, int count) {
		ConsumeItem.Builder consumeItem = null;
		for (ConsumeItem.Builder consume :  consumeInfo.getConsumeItemsBuilderList()) {
			if (consume.getType() == itemType.PLAYER_ATTR_VALUE && consume.getItemId().equals(attrType)) {
				consumeItem = consume;
				break;
			}
		}		
		if (consumeItem == null) {
			consumeItem = ConsumeItem.newBuilder();
			consumeItem.setType(itemType.PLAYER_ATTR_VALUE);
			consumeItem.setItemId(attrType);
			consumeItem.setCount(count);
			consumeInfo.addConsumeItems(consumeItem);
		}
		else
		{
			consumeItem.setCount(consumeItem.getCount() + count);
		}			
		return this;
	}
	
	public ConsumeItems addAttr(int attrType, int count) {		
		return addAttr(String.valueOf(attrType), count);
	}
	
	//目前怪物的消耗都是通过指定Id的
	public ConsumeItems addMonster(int id, String monsterId) {
		ConsumeItem.Builder consumeItem = ConsumeItem.newBuilder();
		consumeItem.setType(Const.itemType.MONSTER_VALUE);
		consumeItem.setId(id);
		consumeItem.setItemId(monsterId);
		consumeInfo.addConsumeItems(consumeItem);
		return this;
	}
	
	/**
	 * 根据配置添加消耗
	 * 
	 * @return
	 */
	public ConsumeItems addItemInfo (ItemInfo itemInfo) {
			
		if (itemInfo.getType() == itemType.PLAYER_ATTR_VALUE) {
			addAttr(itemInfo.getItemId(), itemInfo.getCount());
		}
		else if (itemInfo.getType() == itemType.ITEM_VALUE) {
			addItem(itemInfo.getItemId(), itemInfo.getCount());
		}
		else if (itemInfo.getType() == itemType.MONSTER_VALUE) {
			throw new RuntimeException("unsupport config consume type");
		}
		//配置的消耗类型不包括装备
		else if (itemInfo.getType() == itemType.EQUIP_VALUE) {
			throw new RuntimeException("unsupport config consume type");
		}
		return this;
	}
	
	public ConsumeItems addItemInfos(List<ItemInfo> itemInfos) {
		if (itemInfos != null) {
			for (ItemInfo itemInfo : itemInfos) {
				addItemInfo(itemInfo);
			}
		}
		
		return this;
	}
	
	public ConsumeItems addGold(int gold) {
		addAttr(changeType.CHANGE_GOLD_VALUE, gold);
		return this;
	}

	public ConsumeItems addCoin(int coin) {
		addAttr(changeType.CHANGE_COIN_VALUE, coin);
		return this;
	}

	/**
	 * 检测是否可消耗
	 * 
	 * @return
	 */
	public boolean checkConsume(Player player) {
		return checkConsumeInternal(player) == 0;
	}
	
	/**
	 * 检测是否可消耗
	 * @param player
	 * @param hpCode
	 * @return
	 */
	public boolean checkConsume(Player player, int hpCode) {
		int result = checkConsumeInternal(player);
		if(result > 0) {
			if(hpCode > 0) {
				switch (result) {
					case PlayerItemCheckResult.COINS_NOT_ENOUGH:
						player.sendError(hpCode, Status.itemError.COINS_NOT_ENOUGH_VALUE);
						break;
					case PlayerItemCheckResult.GOLD_NOT_ENOUGH:
						player.sendError(hpCode, Status.itemError.GOLD_NOT_ENOUGH_VALUE);
						break;
					case PlayerItemCheckResult.EQUIP_NOT_ENOUGH:
						player.sendError(hpCode, Status.itemError.EQUIP_NOT_FOUND_VALUE);
						break;
					case PlayerItemCheckResult.TOOLS_NOT_ENOUGH:
						player.sendError(hpCode, Status.itemError.ITEM_NOT_ENOUGH_VALUE);
						break;
					
					default:
						break;
					}
			}
			return false;
		}
		return true;
	}
	
	/**
	 * 检测消耗物品或者属性数量是否充足
	 * @param player
	 * @return
	 */
	private int checkConsumeInternal(Player player) {
		for(ConsumeItem consumeItem : consumeInfo.getConsumeItemsList()) {
			if(consumeItem.getType() == Const.itemType.PLAYER_ATTR_VALUE) {
				if (Integer.valueOf(consumeItem.getItemId()).intValue() == Const.changeType.CHANGE_COIN_VALUE) {
					if(player.getCoin() < consumeItem.getCount()) {
						return PlayerItemCheckResult.COINS_NOT_ENOUGH;
					} 
				}
				else if (Integer.valueOf(consumeItem.getItemId()).intValue() == Const.changeType.CHANGE_GOLD_VALUE) {
					if (player.getGold() < consumeItem.getCount()) {
						return PlayerItemCheckResult.GOLD_NOT_ENOUGH;
					}
				}
			}
			else if(consumeItem.getType() == Const.itemType.EQUIP_VALUE) {
				//检测装备 
				EquipEntity equipEntity = player.getPlayerData().getEquipById(consumeItem.getId());
				if(equipEntity == null) {
					return PlayerItemCheckResult.EQUIP_NOT_ENOUGH;
				}
			} 
			else if(consumeItem.getType() == Const.itemType.MONSTER_VALUE) {
				//检测装备 
				MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity((int)consumeItem.getId());
				if(monsterEntity == null) {
					return PlayerItemCheckResult.MONSTER_NOT_ENOUGH;
				}
			}
			else if(consumeItem.getType() == Const.itemType.ITEM_VALUE) {
				//检测道具
				String itemId = consumeItem.getItemId();
				ItemEntity itemEntity = player.getPlayerData().getItemByItemId(itemId);
				if(itemEntity == null || itemEntity.getCount() <= 0 || itemEntity.getCount() < consumeItem.getCount()) {
					return PlayerItemCheckResult.TOOLS_NOT_ENOUGH;
				}
			}
		}
		
		return 0;
	}
	
	/**
	 * 数据消耗
	 */
	public boolean consumeTakeAffect(Player player, Action action) {
		try {	
			for (int i = 0; i < consumeInfo.getConsumeItemsBuilderList().size(); ) {
				ConsumeItem.Builder item = consumeInfo.getConsumeItemsBuilder(i);
				SynPlayerAttr.Builder playerBuilder = consumeInfo.getPlayerAttrBuilder();
				if (item.getType() == Const.itemType.PLAYER_ATTR_VALUE) {
					// 玩家属性
					switch (Integer.valueOf(item.getItemId()).intValue()) {
					case changeType.CHANGE_COIN_VALUE:
						player.consumeCoin(item.getCount(), action);
						playerBuilder.setCoin(player.getCoin());
						break;

					case changeType.CHANGE_GOLD_VALUE:
						player.consumeGold(item.getCount(), action);
						playerBuilder.setGold(player.getGold());
						break;
					
					default:
						break;
					}
				}
				else if(item.getType() == Const.itemType.ITEM_VALUE){
					ItemEntity itemEntity = player.consumeItem(item.getItemId(), item.getCount(), action);
					if (itemEntity == null) {					
						consumeInfo.removeConsumeItems(i);
						continue;
					}
				}
				else if(item.getType() == Const.itemType.EQUIP_VALUE){
					boolean result = player.consumeEquip(item.getId(), action);
					if (result == false) {				
						consumeInfo.removeConsumeItems(i);
						continue;
					}
				}
				else if(item.getType() == Const.itemType.MONSTER_VALUE ){
					boolean result = player.consumeMonster((int)item.getId(), action);
					if (result == false) {				
						consumeInfo.removeConsumeItems(i);
						continue;
					}
					else {
						// 脱装备
						
					}
				}
				else {
					throw new RuntimeException("unsupport item type");
				}
				
				++i;
			}
		}
		catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}
		return true;
	}

	/**
	 * 数据消耗
	 */
	public void consumeTakeAffectAndPush(Player player, Action action) {
		if (consumeTakeAffect(player, action) == true) {
			// 推送消耗信息
			player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_CONSUME_S, consumeInfo));
		}
	}
}
