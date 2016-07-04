package com.hawk.game.item;

import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.hawk.game.attr.Attribute;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
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
import com.hawk.game.protocol.Player.SynPlayerAttr;
import com.hawk.game.protocol.Reward;
import com.hawk.game.protocol.Reward.HSRewardInfo;
import com.hawk.game.protocol.Reward.RewardItem;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ItemUtil;

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

	/**
	 * 判断是否有奖励
	 */
	public void setRewardInfo(HSRewardInfo.Builder rewardInfo) {
		this.rewardInfo = rewardInfo;
	}
	
	/**
	 * 设置builder
	 */
	public boolean hasAwardItem() {
		return rewardInfo.getRewardItemsList().size() > 0;
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

	public AwardItems addItem(int itemId, int count) {
		RewardItem.Builder rewardItem = RewardItem.newBuilder();
		rewardItem.setType(itemType.ITEM_VALUE);
		rewardItem.setItemId(itemId);
		rewardItem.setCount(count);
		rewardInfo.addRewardItems(rewardItem);
		return this;
	}

	public AwardItems addEquip(int equipId, int stage, int level) {
		RewardItem.Builder rewardItem = RewardItem.newBuilder();
		rewardItem.setType(itemType.EQUIP_VALUE);
		rewardItem.setItemId(equipId);
		rewardItem.setStage(stage);
		rewardItem.setLevel(level);
		rewardInfo.addRewardItems(rewardItem);
		return this;
	}

	public AwardItems addAttr(int attrType, int count) {
		RewardItem.Builder rewardItem = null;
		
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.PLAYER_ATTR_VALUE && reward.getItemId() == attrType) {
				rewardItem = reward;
				break;
			}
		}
		
		if (rewardItem == null) {
			rewardItem = RewardItem.newBuilder();
			rewardItem.setType(itemType.PLAYER_ATTR_VALUE);
			rewardItem.setItemId(attrType);
			rewardItem.setCount(0);
			rewardInfo.addRewardItems(rewardItem);
		}
		
		rewardItem.setCount(rewardItem.getCount() + count);
		return this;
	}
	
	public AwardItems addItem(ItemInfo itemInfo) {
		RewardItem.Builder rewardItem = RewardItem.newBuilder();
		rewardItem.setType(itemInfo.getType());
		rewardItem.setItemId(itemInfo.getItemId());
		rewardItem.setStage(itemInfo.getStage());
		rewardItem.setLevel(itemInfo.getLevel());
		rewardInfo.addRewardItems(rewardItem);
		return this;
	}

	public AwardItems addItemInfos(List<ItemInfo> itemInfos) {
		for (ItemInfo itemInfo : itemInfos) {
			addItem(itemInfo);
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

	public AwardItems appendAward(AwardItems awards) {
		AwardItems newAward = new AwardItems();
		newAward.setRewardInfo(rewardInfo.clone());
		return this;
	}

	/**
	 * 功能发放奖励
	 * 
	 * @param player
	 * @param action
	 * @param async
	 * @return
	 */
	public void  rewardTakeAffect(Player player, Action action) {
		deliverItemAwards(player, action);
	}

	/**
	 * 发放奖励并且推送
	 * 
	 * @param player
	 * @param action
	 */
	public void rewardTakeAffectAndPush(Player player, Action action) {
		rewardTakeAffect(player, action);
		player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_REWARD_S_VALUE, rewardInfo));
	}

	/**
	 * 具体发放奖励
	 * 
	 * @param player
	 * @param action
	 * @param async
	 */
	public void deliverItemAwards(Player player, Action action) {	
		try {			
			for (int i = 0; i < rewardInfo.getRewardItemsBuilderList().size(); ) {
				RewardItem.Builder item = rewardInfo.getRewardItemsBuilder(i);
				SynPlayerAttr.Builder playerBuilder = rewardInfo.getPlayerAttrBuilder();
				if (item.getType() == Const.itemType.PLAYER_ATTR_VALUE) {
					// 玩家属性
					switch (item.getItemId()) {
					case playerAttr.COIN_VALUE:
						player.increaseCoin(item.getCount(), action);
						playerBuilder.setCoin(player.getCoin());
						break;

					case playerAttr.GOLD_VALUE:
						player.increaseGold(item.getCount(), action);
						playerBuilder.setGold(player.getGold());
						break;

					case playerAttr.LEVEL_VALUE:
						player.increaseLevel(item.getCount(), action);
						playerBuilder.setLevel(player.getLevel());
						break;

					case playerAttr.EXP_VALUE:
						player.increaseExp(item.getCount(), action);
						playerBuilder.setExp(player.getExp());
						//playerBuilder.setLevel(player.getLevel());
						break;

					case playerAttr.VIPLEVEL_VALUE:
						player.setVipLevel(item.getCount(), action);
						playerBuilder.setVipLevel(player.getVipLevel());
						break;
					
					default:
						break;
					}
				}
				else if(item.getType() == Const.itemType.ITEM_VALUE){
					ItemEntity itemEntity = player.increaseTools(item.getItemId(), item.getCount(), action);
					if (itemEntity != null) {
						
						try {
							BehaviorLogger.log4Platform(player, action, Params.valueOf("id", itemEntity.getId()), 
								Params.valueOf("itemId", itemEntity.getItemId()), 
								Params.valueOf("itemCount", item.getCount()));
						} catch (Exception e) {
							HawkException.catchException(e);
						}
					}
					else {
						rewardInfo.removeRewardItems(i);
						continue;
					}
				}
				else if(item.getType() == Const.itemType.EQUIP_VALUE){
					EquipEntity equipEntity = player.increaseEquip(item.getItemId(), action);
					if (equipEntity != null) {				
						try {
							BehaviorLogger.log4Platform(player, action, Params.valueOf("id", equipEntity.getId()), 
								Params.valueOf("itemId", equipEntity.getItemId()), 
								Params.valueOf("itemCount", item.getCount()));
						} catch (Exception e) {
							HawkException.catchException(e);
						}
					}
					else {
						rewardInfo.removeRewardItems(i);
						continue;
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
		}
		
	}
}
