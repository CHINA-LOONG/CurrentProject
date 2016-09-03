package com.hawk.game.item;

import java.util.List;

import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.google.protobuf.ProtocolMessageEnum;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.Consume.ConsumeItem;
import com.hawk.game.protocol.Consume.HSConsumeInfo;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Player.SynPlayerAttr;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst.ConsumeCheckResult;

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

	public ConsumeItems addAttr(int attrType, int count) {
		return addAttr(String.valueOf(attrType), count);
	}

	public ConsumeItems addAttr(String attrType, int count) {
		ConsumeItem.Builder consumeItem = null;
		for (ConsumeItem.Builder consume :  consumeInfo.getConsumeItemsBuilderList()) {
			if (consume.getType() == itemType.PLAYER_ATTR_VALUE && consume.getItemId().equals(String.valueOf(attrType))) {
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
		if (itemInfo != null) {
			if (itemInfo.getType() == itemType.PLAYER_ATTR_VALUE) {
				addAttr(itemInfo.getItemId(), itemInfo.getCount());
			}
			else if (itemInfo.getType() == itemType.ITEM_VALUE) {
				addItem(itemInfo.getItemId(), itemInfo.getCount());
			}
			else if (itemInfo.getType() == itemType.MONSTER_VALUE) {
				throw new RuntimeException("unsupport config consume type");
			}
			else if (itemInfo.getType() == itemType.PLAYER_ATTR_VALUE) {
				addContribution(itemInfo.getCount());
			}
			//配置的消耗类型不包括装备
			else if (itemInfo.getType() == itemType.EQUIP_VALUE) {
				throw new RuntimeException("unsupport config consume type");
			}
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
		if (gold > 0 ) {
			addAttr(changeType.CHANGE_GOLD_VALUE, gold);
		}
		return this;
	}

	public ConsumeItems addCoin(int coin) {
		if (coin > 0 ) {
			addAttr(changeType.CHANGE_COIN_VALUE, coin);
		}
		return this;
	}

	public ConsumeItems addTowerCoin(int towerCoin) {
		if (towerCoin > 0) {
			addAttr(changeType.CHANGE_TOWER_COIN_VALUE, towerCoin);
		}
		return this;
	}

	public ConsumeItems addArenaCoin(int towerCoin) {
		if (towerCoin > 0) {
			addAttr(changeType.CHANGE_ARENA_COIN_VALUE, towerCoin);
		}
		return this;
	}

	public ConsumeItems addContribution(int contribution) {
		if (contribution > 0) {
			addAttr(changeType.CHANGE_PLAYER_CONTRIBUTION_VALUE, contribution);
		}
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
	 * @param hsCode
	 * @return
	 */
	public boolean checkConsume(Player player, int hsCode) {
		int result = checkConsumeInternal(player);
		if(result > 0) {
			if(hsCode > 0) {
				switch (result) {
					case ConsumeCheckResult.COIN_NOT_ENOUGH:
						player.sendError(hsCode, Status.PlayerError.COIN_NOT_ENOUGH_VALUE);
						break;
					case ConsumeCheckResult.GOLD_NOT_ENOUGH:
						player.sendError(hsCode, Status.PlayerError.GOLD_NOT_ENOUGH_VALUE);
						break;
					case ConsumeCheckResult.TOWER_COIN_NOT_ENOUGH:
						player.sendError(hsCode, Status.PlayerError.TOWER_COIN_NOT_ENOUGH_VALUE);
						break;
					case ConsumeCheckResult.ARENA_COIN_NOT_ENOUGH:
						player.sendError(hsCode, Status.PlayerError.ARENA_COIN_NOT_ENOUGH_VALUE);
						break;
					case ConsumeCheckResult.FATIGUE_NOT_ENOUGH:
						player.sendError(hsCode, Status.PlayerError.FATIGUE_NOT_ENOUGH_VALUE);
						break;
					case ConsumeCheckResult.EQUIP_NOT_ENOUGH:
						player.sendError(hsCode, Status.itemError.EQUIP_NOT_FOUND_VALUE);
						break;
					case ConsumeCheckResult.TOOLS_NOT_ENOUGH:
						player.sendError(hsCode, Status.itemError.ITEM_NOT_ENOUGH_VALUE);
						break;
					case ConsumeCheckResult.MONSTER_NOT_ENOUGH:
						player.sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE);
						break;
					case ConsumeCheckResult.MONSTER_LOCKED:
						player.sendError(hsCode, Status.monsterError.MONSTER_LOCKED_VALUE);
						break;
					case ConsumeCheckResult.NOT_IN_ALLIANCE:
						player.sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN_VALUE);
						break;
					case ConsumeCheckResult.CONTRIBUTION_NOT_ENOUGH:
						player.sendError(hsCode, Status.allianceError.ALLIANCE_CONTRI_NOT_ENOUGH_VALUE);
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
			switch (consumeItem.getType()) {
			case itemType.PLAYER_ATTR_VALUE:
				int changeType = Integer.valueOf(consumeItem.getItemId());
				if (changeType == Const.changeType.CHANGE_COIN_VALUE) {
					if(player.getCoin() < consumeItem.getCount()) {
						return ConsumeCheckResult.COIN_NOT_ENOUGH;
					}
				}
				else if (changeType == Const.changeType.CHANGE_GOLD_VALUE) {
					if (player.getGold() < consumeItem.getCount()) {
						return ConsumeCheckResult.GOLD_NOT_ENOUGH;
					}
				}
				else if (changeType == Const.changeType.CHANGE_TOWER_COIN_VALUE) {
					if (player.getTowerCoin() < consumeItem.getCount()) {
						return ConsumeCheckResult.TOWER_COIN_NOT_ENOUGH;
					}
				}
				else if (changeType == Const.changeType.CHANGE_ARENA_COIN_VALUE) {
					if (player.getArenaCoin() < consumeItem.getCount()) {
						return ConsumeCheckResult.ARENA_COIN_NOT_ENOUGH;
					}
				}
				else if (changeType == Const.changeType.CHANGE_FATIGUE_VALUE) {
					// 更新活力值
					player.regainFatigue();
					if (player.getPlayerData().getStatisticsEntity().getFatigue() < consumeItem.getCount()) {
						return ConsumeCheckResult.FATIGUE_NOT_ENOUGH;
					}
				}
				else if (changeType == Const.changeType.CHANGE_PLAYER_CONTRIBUTION_VALUE) {
					// 检测公会
					if (player.getAllianceId() == 0) {
						return ConsumeCheckResult.NOT_IN_ALLIANCE;
					}
					// 检测贡献值
					if (Integer.valueOf(consumeItem.getItemId()).intValue() == Const.changeType.CHANGE_PLAYER_CONTRIBUTION_VALUE) {
						if (player.getPlayerData().getPlayerAllianceEntity().getContribution() < consumeItem.getCount()) {
							return ConsumeCheckResult.CONTRIBUTION_NOT_ENOUGH;
						}
					}
				}
				break;

			case itemType.EQUIP_VALUE:
				EquipEntity equipEntity = player.getPlayerData().getEquipById(consumeItem.getId());
				if(equipEntity == null) {
					return ConsumeCheckResult.EQUIP_NOT_ENOUGH;
				}
				break;

			case itemType.MONSTER_VALUE:
				MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity((int)consumeItem.getId());
				if(monsterEntity == null) {
					return ConsumeCheckResult.MONSTER_NOT_ENOUGH;
				}
				else if (monsterEntity.isStateSet(Const.MonsterState.LOCKED_VALUE) ||
						 monsterEntity.isStateSet(Const.MonsterState.IN_ALLIANCE_BASE_VALUE)  ||
						 monsterEntity.isStateSet(Const.MonsterState.IN_ADVENTURE_VALUE)) 
				{
					return ConsumeCheckResult.MONSTER_LOCKED;
				}
				break;

			case itemType.ITEM_VALUE:
				ItemEntity itemEntity = player.getPlayerData().getItemByItemId(consumeItem.getItemId());
				if(itemEntity == null || itemEntity.getCount() <= 0 || itemEntity.getCount() < consumeItem.getCount()) {
					return ConsumeCheckResult.TOOLS_NOT_ENOUGH;
				}
				break;

			default:
				break;
			}
		}

		return 0;
	}

	/**
	 * 数据消耗
	 */
	public boolean consumeTakeAffect(Player player, Action action) {
		try {
			for (int i = 0; i < consumeInfo.getConsumeItemsBuilderList().size(); ++i) {
				ConsumeItem.Builder item = consumeInfo.getConsumeItemsBuilder(i);
				switch (item.getType()) {
				case itemType.PLAYER_ATTR_VALUE:
					// 玩家属性
					switch (Integer.valueOf(item.getItemId()).intValue()) {
					case changeType.CHANGE_COIN_VALUE:
						player.consumeCoin(item.getCount(), action);
						break;

					case changeType.CHANGE_GOLD_VALUE:
						player.consumeGold(item.getCount(), action);
						break;

					case changeType.CHANGE_TOWER_COIN_VALUE:
						player.consumeTowerCoin(item.getCount(), action);
						break;

					case changeType.CHANGE_ARENA_COIN_VALUE:
						player.consumeArenaCoin(item.getCount(), action);
						break;

					case changeType.CHANGE_FATIGUE_VALUE:
						player.consumeFatigue(item.getCount(), action);
						break;

					case changeType.CHANGE_PLAYER_CONTRIBUTION_VALUE:
						player.consumeContribution(item.getCount(), action);
						break;

					default:
						break;
					}
					SynPlayerAttr.Builder playerBuilder = consumeInfo.getPlayerAttrBuilder();
					playerBuilder.setGold(player.getGold());
					playerBuilder.setCoin(player.getCoin());
					playerBuilder.setTowerCoin(player.getTowerCoin());
					playerBuilder.setContribution(player.getAllianceId() != 0 ? player.getPlayerData().getPlayerAllianceEntity().getContribution() : 0);
					playerBuilder.setExp(player.getExp());
					playerBuilder.setLevel(player.getLevel());
					playerBuilder.setFatigue(player.getPlayerData().getStatisticsEntity().getFatigue());
					playerBuilder.setFatigueBeginTime((int)(player.getPlayerData().getStatisticsEntity().getFatigueBeginTime().getTimeInMillis() / 1000));
					break;

				case itemType.ITEM_VALUE:
					if (null == player.consumeItem(item.getItemId(), item.getCount(), action)) {
						consumeInfo.removeConsumeItems(i);
						continue;
					}
					break;

				case itemType.EQUIP_VALUE:
					if (false == player.consumeEquip(item.getId(), action)) {
						consumeInfo.removeConsumeItems(i);
						continue;
					}
					break;

				case itemType.MONSTER_VALUE:
					if (false == player.consumeMonster((int)item.getId(), action)) {
						consumeInfo.removeConsumeItems(i);
						continue;
					}
					break;

				default:
					throw new RuntimeException("unsupport item type");
				}
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
	public void consumeTakeAffectAndPush(Player player, Action action, int hsCode) {
		if (consumeTakeAffect(player, action) == true) {
			// 推送消耗信息
			consumeInfo.setHsCode(hsCode);
			player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_CONSUME_S, consumeInfo));
		}
	}
}
