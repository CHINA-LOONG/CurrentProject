package com.hawk.game.item;

import java.util.List;

import org.hawk.log.HawkLog;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.google.protobuf.ProtocolMessageEnum;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerData;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.Consume.ConsumeItem;
import com.hawk.game.protocol.Consume.HSConsumeInfo;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Player.SynPlayerAttr;
import com.hawk.game.protocol.Status;
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

	public static ConsumeItems valueOf(ProtocolMessageEnum type, int count) {
		ConsumeItems consumeItems = new ConsumeItems();
		consumeItems.addChangeInfo(type, count);
		return consumeItems;
	}
	
	public static ConsumeItems valueOf(ProtocolMessageEnum type, int id, int itemId, int count) {
		ConsumeItems consumeItems = new ConsumeItems();
		consumeItems.addChangeInfo(type, id, itemId, count);
		return consumeItems;
	}

	public HSConsumeInfo.Builder getBuilder() {
		return consumeInfo;
	}

	/**
	 * type
	 * 
	 * @param type
	 */
	public void addChangeInfo(ProtocolMessageEnum type, int count) {
		SynPlayerAttr.Builder builder = consumeInfo.getPlayerAttrBuilder();
		switch (type.getNumber()) {
		case Const.changeType.CHANGE_GOLD_VALUE:
			builder.setGold(count);
			break;

		case Const.changeType.CHANGE_COIN_VALUE:
			builder.setCoin(count);
			break;

		case Const.changeType.CHANGE_PLAYER_EXP_VALUE:
			builder.setExp(count);
			break;

		case Const.changeType.CHANGE_PLAYER_LEVEL_VALUE:
			builder.setLevel(count);
			break;

		case Const.changeType.CHANGE_VIPLEVEL_VALUE:
			builder.setVipLevel(count);
			break;
			
		default:
			HawkLog.errPrintln("unsupport change info: " + type.getNumber());
			break;
		}
	}

	/**
	 * 添加修改信息
	 * 
	 * @param type
	 * @param id
	 * @param itemId
	 * @param count
	 */
	public void addChangeInfo(ProtocolMessageEnum type, long id, int itemId, int count) {
		ConsumeItem.Builder builder = ConsumeItem.newBuilder();
		builder.setType(type.getNumber());
		builder.setId(id);
		builder.setItemId(itemId);
		builder.setCount(count);
		consumeInfo.addConsumeItems(builder);
	}

	/**
	 * 同步改变信息
	 * 
	 * @param player
	 * @return
	 */
	public boolean pushChange(Player player) {
		HSConsumeInfo consumes = consumeInfo.build();
		SynPlayerAttr syncAttrInfo = consumes.getPlayerAttr();
		if(syncAttrInfo.getCoin() > 0) {
			consumeInfo.getPlayerAttrBuilder().setCoin(player.getCoin());
		}
		
		if(syncAttrInfo.getGold() > 0) {
			consumeInfo.getPlayerAttrBuilder().setGold(player.getGold());
		}
		
		if (syncAttrInfo.getExp() > 0) {
			consumeInfo.getPlayerAttrBuilder().setExp(player.getExp());
		}
		
		return player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_CONSUME_S, consumeInfo));
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
	 * 数据消耗
	 */
	public boolean consumeTakeAffectAndPush(Player player, Action action) {
		try {
			consumeTakeAffect(player, action);
			// 推送消耗信息
			pushChange(player);
		}catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}
		
		return true;
	}
	/**
	 * 数据消耗
	 */
	public boolean consumeTakeAffect(Player player, Action action) {
		HSConsumeInfo consumes = consumeInfo.build();
		SynPlayerAttr syncAttrInfo = consumes.getPlayerAttr();
		try{
			if(syncAttrInfo.getCoin() > 0) {
				player.consumeCoin(syncAttrInfo.getCoin(), action);
			}
			
			if(syncAttrInfo.getGold() > 0) {
				player.consumeGold(syncAttrInfo.getGold(), action);
			}
			
			// TODO 经验
			
			for(ConsumeItem consumeItem : consumes.getConsumeItemsList()) {
				if(consumeItem.getType() == changeType.CHANGE_EQUIP_VALUE) {
					//检测装备 
					long equipId = consumeItem.getId();
					player.consumeEquip(equipId, action);
				} else if(consumeItem.getType() == changeType.CHANGE_TOOLS_VALUE) {
					//检测道具
					int itemId = consumeItem.getItemId();
					player.consumeTools(itemId, consumeItem.getCount(), action);
				}
			}
		}catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}
		return true;
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
					case PlayerItemCheckResult.EQUIP_NOI_ENOUGH:
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
		HSConsumeInfo consumes = consumeInfo.build();
		SynPlayerAttr syncAttrInfo = consumes.getPlayerAttr();
		
		if(syncAttrInfo.getCoin() > 0) {
			if(player.getCoin() < syncAttrInfo.getCoin()) {
				return PlayerItemCheckResult.COINS_NOT_ENOUGH;
			}
		}
		
		if(syncAttrInfo.getGold() > 0) {
			if(player.getGold() < syncAttrInfo.getGold()) {
				return PlayerItemCheckResult.GOLD_NOT_ENOUGH;
			}
		}
		
		for(ConsumeItem consumeItem : consumes.getConsumeItemsList()) {
			if(consumeItem.getType() == changeType.CHANGE_EQUIP_VALUE) {
				//检测装备 
				long equipId = consumeItem.getId();
				EquipEntity equipEntity = player.getPlayerData().getEquipById(equipId);
				if(equipEntity == null) {
					return PlayerItemCheckResult.EQUIP_NOI_ENOUGH;
				}
			} else if(consumeItem.getType() == changeType.CHANGE_TOOLS_VALUE) {
				//检测道具
				int itemId = (int) consumeItem.getItemId();
				ItemEntity itemEntity = player.getPlayerData().getItemById(itemId);
				if(itemEntity == null || itemEntity.getCount() <= 0 || itemEntity.getCount() < consumeItem.getCount()) {
					return PlayerItemCheckResult.TOOLS_NOT_ENOUGH;
				}
			}
		}
		
		return 0;
	}

	public boolean addConsumeInfo(PlayerData playerData, List<ItemInfo> needItems) {
		SynPlayerAttr.Builder builder = consumeInfo.getPlayerAttrBuilder();
		for(ItemInfo itemInfo : needItems) {
			if(itemInfo.getType() == itemType.PLAYER_ATTR_VALUE) {
				switch (itemInfo.getItemId()) {
					case Const.playerAttr.GOLD_VALUE:
						builder.setGold(itemInfo.getCount());
						break;
					case Const.playerAttr.COIN_VALUE:	
						builder.setCoin(itemInfo.getCount());
						break;
					case Const.playerAttr.EXP_VALUE:
						builder.setExp(itemInfo.getCount());
						break;
					case Const.playerAttr.LEVEL_VALUE:
						builder.setLevel(itemInfo.getCount());
						break;
					case Const.playerAttr.VIPLEVEL_VALUE:
						builder.setVipLevel(itemInfo.getCount());
						break;
					default:
						break;
				}
			}
			else if(itemInfo.getType()  == itemType.ITEM_VALUE){
				ItemEntity itemEntity = playerData.getItemByItemId(itemInfo.getItemId());
				if(itemEntity == null || itemEntity.getCount() < itemInfo.getCount()) {
					//构造失败 道具不足
					return false;
				}
				addChangeInfo(changeType.CHANGE_TOOLS, itemEntity.getId(), itemEntity.getItemId(), itemInfo.getCount());
			}
			else if(itemInfo.getType()  == itemType.EQUIP_VALUE){
				ItemEntity itemEntity = playerData.getItemByItemId(itemInfo.getItemId());
				if(itemEntity == null) {
					//构造失败 装备不存在
					return false;
				}
				addChangeInfo(changeType.CHANGE_TOOLS, itemEntity.getId(), itemEntity.getItemId(), itemInfo.getCount());
			}
		}
		return true;
	}
}
