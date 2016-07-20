package com.hawk.game.item;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hibernate.type.IntegerType;

import com.hawk.game.attr.Attribute;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.log.BehaviorLogger;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.log.BehaviorLogger.Params;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.Const.playerAttr;
import com.hawk.game.protocol.Consume.HSConsumeInfo;
import com.hawk.game.protocol.Monster.SynMonsterAttr;
import com.hawk.game.protocol.Player.SynPlayerAttr;
import com.hawk.game.protocol.Reward;
import com.hawk.game.protocol.Reward.HSRewardInfo;
import com.hawk.game.protocol.Reward.RewardItem;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ItemUtil;
import com.sun.org.apache.regexp.internal.recompile;

/**
 * 奖励信息内存数据 禁忌: 此对象不可重复复用, 避免奖励累加, 切记
 * 
 * @author hawk
 * 
 */
public class AwardItems {
	/**
	 * 奖励信息
	 */
	private HSRewardInfo.Builder rewardInfo;
	
	/**
	 * 构造函数
	 */
	public AwardItems() {
		rewardInfo = HSRewardInfo.newBuilder();
		SynPlayerAttr.Builder playerBuilder = SynPlayerAttr.newBuilder();
		rewardInfo.setPlayerAttr(playerBuilder);
	}

	public static AwardItems valueOf() {
		return new AwardItems();
	}

	public static AwardItems valueOf(List<ItemInfo> infos) {
		AwardItems awards = new AwardItems();
		awards.addItemInfos(infos);
		return awards;
	}
	
	/**
	 * 设置builder
	 */
	public void setRewardInfo(HSRewardInfo.Builder rewardInfo) {
		this.rewardInfo = rewardInfo;
	}
	
	public HSRewardInfo.Builder getBuilder() {
		return rewardInfo;
	}
	
	/**
	 * 克隆奖励对象
	 */
	@Override
	public AwardItems clone() {
		AwardItems newAward = new AwardItems();
		newAward.setRewardInfo(rewardInfo.clone());
		return newAward;
	}
	
	/**
	 * 判断是否有奖励
	 */
	public boolean hasAwardItem() {
		return rewardInfo.getRewardItemsList().size() > 0;
	}

	/**
	 * 生成存储字符串
	 * 
	 * @return
	 */
	public String toDbString() {
		return null;
	}

	/**
	 * 直接设置奖励列表
	 */
	public AwardItems setRewardItemList(List<RewardItem> rewardList) {
		rewardInfo.clearRewardItems();
		rewardInfo.addAllRewardItems(rewardList);
		return this;
	}
	
	public AwardItems addItem(String itemId, int count) {
		RewardItem.Builder rewardItem = null;
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.ITEM_VALUE && reward.getItemId().equals(itemId)) {
				rewardItem = reward;
				break;
			}
		}
		
		if (rewardItem == null) {
			rewardItem = RewardItem.newBuilder();
			rewardItem.setType(itemType.ITEM_VALUE);
			rewardItem.setItemId(itemId);
			rewardItem.setCount(count);
			rewardInfo.addRewardItems(rewardItem);
		}
		else {
			rewardItem.setCount(rewardItem.getCount() + count);
		}
		
		return this;
	}

	public AwardItems addEquip(String equipId, int count, int stage, int level) {
		for (int i = 0; i < count; i++) {
			addEquip(equipId, stage, level);
		}
		return this;
	}

	public AwardItems addEquip(String equipId, int stage, int level) {
		RewardItem.Builder rewardItem = RewardItem.newBuilder();
		rewardItem.setType(Const.itemType.EQUIP_VALUE);
		rewardItem.setItemId(equipId);
		rewardItem.setStage(stage);
		rewardItem.setLevel(level);
		rewardInfo.addRewardItems(rewardItem);
		return this;
	}

	public AwardItems  addMonster(String monsterId, int count, int stage) {
		for (int i = 0; i < count; i++) {
			addMonster(monsterId, stage);
		}
		return this;
	}
	
	public AwardItems  addMonster(String monsterId, int stage) {
		RewardItem.Builder rewardItem = RewardItem.newBuilder();
		rewardItem.setType(Const.itemType.MONSTER_VALUE);
		rewardItem.setItemId(monsterId);
		rewardItem.setStage(stage);
		rewardInfo.addRewardItems(rewardItem);
		return this;
	}
	
	public AwardItems addAttr(String attrType, int count) {
		RewardItem.Builder rewardItem = null;
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.PLAYER_ATTR_VALUE && reward.getItemId().equals(attrType)) {
				rewardItem = reward;
				break;
			}
		}
		
		if (rewardItem == null) {
			rewardItem = RewardItem.newBuilder();
			rewardItem.setType(itemType.PLAYER_ATTR_VALUE);
			rewardItem.setItemId(attrType);
			rewardItem.setCount(count);
			rewardInfo.addRewardItems(rewardItem);
		}
		else {
			rewardItem.setCount(rewardItem.getCount() + count);
		}
		
		return this;
	}
	
	public AwardItems addAttr(int attrType, int count) {		
		return addAttr(String.valueOf(attrType), count);
	}
	
	public AwardItems addMonsterAttr(String attrType, int count, int id) {
		RewardItem.Builder rewardItem = null;
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.MONSTER_ATTR_VALUE && reward.getItemId().equals(attrType)  && reward.getId() == id) {
				rewardItem = reward;
				break;
			}
		}
		
		if (rewardItem == null) {
			rewardItem = RewardItem.newBuilder();
			rewardItem.setType(itemType.MONSTER_ATTR_VALUE);
			rewardItem.setItemId(attrType);
			rewardItem.setId(id);
			rewardInfo.addRewardItems(rewardItem);
		}
		else
		{
			rewardItem.setCount(rewardItem.getCount() + count);	
		}
		return this;
	}
	
	public AwardItems addMonsterAttr(String attrType, int count) {
		RewardItem.Builder rewardItem = null;
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.MONSTER_ATTR_VALUE && reward.getItemId().equals(attrType)  && reward.getId() == 0) {
				rewardItem = reward;
				break;
			}
		}
		
		if (rewardItem == null) {
			rewardItem = RewardItem.newBuilder();
			rewardItem.setType(itemType.MONSTER_ATTR_VALUE);
			rewardItem.setItemId(attrType);
			rewardItem.setCount(count);
			rewardInfo.addRewardItems(rewardItem);
		}
		else {
			rewardItem.setCount(rewardItem.getCount() + count);	
		}
				
		return this;
	}
	
	public AwardItems addItemInfo(ItemInfo itemInfo) {
		if (itemInfo.getType() == itemType.PLAYER_ATTR_VALUE) {
			addAttr(itemInfo.getItemId(), itemInfo.getCount());
		}
		else if (itemInfo.getType() == itemType.MONSTER_ATTR_VALUE) {
			addMonsterAttr(itemInfo.getItemId(), itemInfo.getCount());
		}
		else if (itemInfo.getType() == itemType.ITEM_VALUE) {
			addItem(itemInfo.getItemId(), itemInfo.getCount());
		}
		else if (itemInfo.getType() == itemType.EQUIP_VALUE) {
			if (itemInfo.getCount() > 0) {
				addEquip(itemInfo.getItemId(), itemInfo.getCount(), itemInfo.getStage(), itemInfo.getLevel());
			}
			else {
				addEquip(itemInfo.getItemId(), itemInfo.getStage(), itemInfo.getLevel());
			}
		}		
		else if (itemInfo.getType() == itemType.MONSTER_VALUE) {
			if (itemInfo.getCount() > 0) {
				addMonster(itemInfo.getItemId(), itemInfo.getCount(), itemInfo.getStage());
			}
			else {
				addMonster(itemInfo.getItemId(), itemInfo.getStage());
			}
		}
		return this;
	}

	public AwardItems addItemInfos(List<ItemInfo> itemInfos) {
		for (ItemInfo itemInfo : itemInfos) {
			addItemInfo(itemInfo);
		}
		return this;
	}

	public AwardItems addGold(int gold) {
		addAttr(changeType.CHANGE_GOLD_VALUE, gold);
		return this;
	}

	public AwardItems addCoin(int coin) {
		addAttr(changeType.CHANGE_COIN_VALUE, coin);
		return this;
	}

	public AwardItems addLevel(int level) {
		addAttr(changeType.CHANGE_PLAYER_LEVEL_VALUE, level);
		return this;
	}

	public AwardItems addExp(int exp) {
		addAttr(changeType.CHANGE_PLAYER_EXP_VALUE, exp);
		return this;
	}

	public AwardItems setVipLevel(int level) {
		addAttr(changeType.CHANGE_PLAYER_EXP_VALUE, level);
		return this;
	}
	
	public boolean initByString(String info) {
		if (info != null && info.length() > 0 && !info.equals("0") && !info.equals("none")) {

		}
		return false;
	}

	public static AwardItems valueOf(String info) {
		AwardItems awardItems = new AwardItems();
		if (awardItems.initByString(info)) {
			return awardItems;
		}
		return null;
	}
	
	/**
	 * 功能发放奖励
	 * 
	 * @param player
	 * @param action
	 * @param async
	 * @return
	 */
	public boolean  rewardTakeAffect(Player player, Action action) {
		try {			
			for (int i = 0; i < rewardInfo.getRewardItemsBuilderList().size(); ) {
				RewardItem.Builder item = rewardInfo.getRewardItemsBuilder(i);
				SynPlayerAttr.Builder playerBuilder = rewardInfo.getPlayerAttrBuilder();
				boolean invalidType = false;
				boolean rewardFail = false;
				if (item.getType() == Const.itemType.PLAYER_ATTR_VALUE) {
					// 玩家属性
					switch (Integer.parseInt(item.getItemId())) {
					case changeType.CHANGE_COIN_VALUE:
						player.increaseCoin(item.getCount(), action);
						playerBuilder.setCoin(player.getCoin());
						break;

					case changeType.CHANGE_GOLD_VALUE:
						player.increaseGold(item.getCount(), action);
						playerBuilder.setGold(player.getGold());
						break;

					case changeType.CHANGE_PLAYER_EXP_VALUE:
						player.increaseExp(item.getCount(), action);
						playerBuilder.setExp(player.getExp());
						playerBuilder.setLevel(player.getLevel());
						break;

					case changeType.CHANGE_VIPLEVEL_VALUE:
						player.setVipLevel(item.getCount(), action);
						playerBuilder.setVipLevel(player.getVipLevel());
						break;
					
					default:
						invalidType = true;
						break;
					}
				}
				else if (item.getType() == Const.itemType.MONSTER_ATTR_VALUE) {					
					if (Integer.parseInt(item.getItemId())== changeType.CHANGE_MONSTER_EXP_VALUE) {
						List<Integer> battleMonsters = player.getPlayerData().getPlayerEntity().getBattleMonsterList();
						if (item.getId() == 0) {
							for (Integer monsterId : battleMonsters) {
								if (player.getPlayerData().getMonsterEntity(monsterId) != null) {
									RewardItem.Builder builder = RewardItem.newBuilder();
									builder.setType(itemType.MONSTER_ATTR_VALUE);
									builder.setItemId(String.valueOf(changeType.CHANGE_MONSTER_EXP_VALUE));
									builder.setCount(item.getCount());
									builder.setId(monsterId);
									rewardInfo.addRewardItems(builder);
								}
							}
							
							// 强制清除没有id的怪物奖励,替换成
							invalidType = true;
						}
						else {
								if (player.getPlayerData().getMonsterEntity((int)item.getId()) != null) {
									SynMonsterAttr.Builder monsterBuilder = null;

									player.increaseMonsterExp((int)item.getId(), item.getCount(), action);
									
									for (SynMonsterAttr.Builder builder : rewardInfo.getMonstersAttrBuilderList()) {
										if (builder.getMonsterId() == (int)item.getId()) {
											monsterBuilder = builder;
											break;
										}
									}
									if (monsterBuilder == null) {
										monsterBuilder = SynMonsterAttr.newBuilder();
										monsterBuilder.setMonsterId((int)item.getId());
										monsterBuilder.setExp(player.getMonsterExp((int)item.getId()));
										monsterBuilder.setLevel(player.getMonsterLevel((int)item.getId()));
										rewardInfo.addMonstersAttr(monsterBuilder);
									}
									else {
										monsterBuilder.setExp(player.getMonsterExp((int)item.getId()));
										monsterBuilder.setLevel(player.getMonsterLevel((int)item.getId()));
									}						
								}
						}
					}
				}
				
				else if(item.getType() == Const.itemType.ITEM_VALUE){
					ItemEntity itemEntity = player.increaseItem(item.getItemId(), item.getCount(), action);
					if (itemEntity == null) {					
						rewardFail = true;
					}
				}
				else if(item.getType() == Const.itemType.EQUIP_VALUE){				
					EquipEntity equipEntity = player.increaseEquip(item.getItemId(), item.getStage(), item.getLevel(), action);			
					if (equipEntity == null) {				
						rewardFail = true;
					}
					else {
						EquipUtil.generateAttr(equipEntity, item);
					}
				}
				else if(item.getType() == Const.itemType.MONSTER_VALUE){				
				
					MonsterEntity monsterEntity = player.increaseMonster(item.getItemId(), item.getStage(), action);			
					if (monsterEntity == null) {				
						rewardFail = true;
					}
				}
				else {
					invalidType = true;
				}
				
				if (invalidType== true || rewardFail == true) {
					rewardInfo.removeRewardItems(i);
				}
				else {
					++i;
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
	 * 发放奖励并且推送
	 * 
	 * @param player
	 * @param action
	 */
	public void rewardTakeAffectAndPush(Player player, Action action) {
		if (rewardTakeAffect(player, action) == true) {
			player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_REWARD_S_VALUE, rewardInfo));
		}
	}

}
