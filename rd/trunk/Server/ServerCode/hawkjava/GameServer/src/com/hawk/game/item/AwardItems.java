package com.hawk.game.item;

import java.util.List;

import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;

import com.hawk.game.config.PlayerAttrCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Const.changeType;
import com.hawk.game.protocol.Const.playerAttr;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Monster.SynMonsterAttr;
import com.hawk.game.protocol.Player.SynPlayerAttr;
import com.hawk.game.protocol.Reward.HSRewardInfo;
import com.hawk.game.protocol.Reward.RewardItem;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.EquipUtil;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.GsConst.AwardCheckResult;

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
		return !rewardInfo.getRewardItemsList().isEmpty();
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

	public AwardItems  addMonster(String monsterId, int count, int stage, int level, int lazy, int disposition) {
		for (int i = 0; i < count; i++) {
			addMonster(monsterId, stage, level, lazy, disposition);
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

	public AwardItems addMonster(String monsterId, int stage, int level, int lazy, int disposition) {
		RewardItem.Builder rewardItem = RewardItem.newBuilder();
		rewardItem.setType(Const.itemType.MONSTER_VALUE);
		rewardItem.setItemId(monsterId);
		rewardItem.setStage(stage);
		rewardItem.setLevel(level);

		HSMonster.Builder monster = HSMonster.newBuilder();
		monster.setCfgId(monsterId);
		monster.setStage(stage);
		monster.setLevel(level);
		monster.setExp(0);
		monster.setLazy(lazy);
		monster.setLazyExp(0);
		monster.setDisposition(disposition);
		monster.setMonsterId(0);
		rewardItem.setMonster(monster);

		rewardItem.setMonster(monster);
		rewardInfo.addRewardItems(rewardItem);
		return this;
	}

	public AwardItems addAttr(String attrType, int count) {
		RewardItem.Builder rewardItem = null;
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.PLAYER_ATTR_VALUE && reward.getItemId().equals(String.valueOf(attrType))) {
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

	public AwardItems addMonsterAttr(int attrType, int count, int id) {
		RewardItem.Builder rewardItem = null;
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.MONSTER_ATTR_VALUE && reward.getItemId().equals(String.valueOf(attrType))  && reward.getId() == id) {
				rewardItem = reward;
				break;
			}
		}

		if (rewardItem == null) {
			rewardItem = RewardItem.newBuilder();
			rewardItem.setType(itemType.MONSTER_ATTR_VALUE);
			rewardItem.setItemId(String.valueOf(attrType));
			rewardItem.setCount(count);
			rewardItem.setId(id);
			rewardInfo.addRewardItems(rewardItem);
		}
		else
		{
			rewardItem.setCount(rewardItem.getCount() + count);
		}
		return this;
	}

	public AwardItems addMonsterAttr(int attrType, int count) {
		RewardItem.Builder rewardItem = null;
		for (RewardItem.Builder reward :  rewardInfo.getRewardItemsBuilderList()) {
			if (reward.getType() == itemType.MONSTER_ATTR_VALUE && reward.getItemId().equals(String.valueOf(attrType))  && reward.getId() == 0) {
				rewardItem = reward;
				break;
			}
		}

		if (rewardItem == null) {
			rewardItem = RewardItem.newBuilder();
			rewardItem.setType(itemType.MONSTER_ATTR_VALUE);
			rewardItem.setItemId(String.valueOf(attrType));
			rewardItem.setCount(count);
			rewardInfo.addRewardItems(rewardItem);
		}
		else {
			rewardItem.setCount(rewardItem.getCount() + count);
		}

		return this;
	}

	public AwardItems addItemInfo(ItemInfo itemInfo) {
		if (itemInfo != null) {
			if (itemInfo.getType() == itemType.PLAYER_ATTR_VALUE) {
				addAttr(itemInfo.getItemId(), itemInfo.getCount());
			}
			else if (itemInfo.getType() == itemType.MONSTER_ATTR_VALUE) {
				addMonsterAttr(Integer.valueOf(itemInfo.getItemId()), itemInfo.getCount());
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
					addMonster(itemInfo.getItemId(), itemInfo.getCount(), itemInfo.getStage(), 1, 1, 1);
				}
				else {
					addMonster(itemInfo.getItemId(), itemInfo.getStage());
				}
			}
		}

		return this;
	}

	public AwardItems addItemInfos(List<ItemInfo> itemInfos) {
		if (itemInfos != null) {
			for (ItemInfo itemInfo : itemInfos) {
				addItemInfo(itemInfo);
			}
		}
		return this;
	}

	public AwardItems addFreeGold(int gold) {
		if (gold <= 0 ) {
			return this;
		}
		addAttr(changeType.CHANGE_GOLD_VALUE, gold);
		return this;
	}

	public AwardItems addBuyGold(int gold) {
		if (gold <= 0 ) {
			return this;
		}
		addAttr(changeType.CHANGE_GOLD_BUY_VALUE, gold);
		return this;
	}

	public AwardItems addCoin(int coin) {
		if (coin <= 0 ) {
			return this;
		}
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

	public boolean checkLimit(Player player, int hsCode) {
		int result = checkLimitInternal(player);
		if (result > 0) {
			if (hsCode > 0) {
				switch (result) {
				case AwardCheckResult.COIN_LIMIT:
					player.sendError(hsCode, Status.PlayerError.COINS_LIMIT_VALUE);
					break;
				case AwardCheckResult.GOLD_LIMIT:
					player.sendError(hsCode, Status.PlayerError.GOLD_LIMIT_VALUE);
					break;
				case AwardCheckResult.FATIGUE_LIMIT:
					player.sendError(hsCode, Status.PlayerError.FATIGUE_LIMIT_VALUE);
					break;
				}
			}
			return false;
		}
		return true;
	}

	/**
	 * 检查是否会超过上限
	 * 假设奖励列表内每种item唯一
	 */
	private int checkLimitInternal(Player player) {
		for (RewardItem rewardItem : rewardInfo.getRewardItemsList()) {
			if(rewardItem.getType() == Const.itemType.PLAYER_ATTR_VALUE) {
				if (Integer.valueOf(rewardItem.getItemId()).intValue() == Const.changeType.CHANGE_COIN_VALUE) {
					if(player.getCoin() + rewardItem.getCount() > GsConst.MAX_COIN_COUNT) {
						return AwardCheckResult.COIN_LIMIT;
					}
				}
				else if (Integer.valueOf(rewardItem.getItemId()).intValue() == Const.changeType.CHANGE_GOLD_VALUE) {
					if (player.getGold() + rewardItem.getCount() > GsConst.MAX_GOLD_COUNT) {
						return AwardCheckResult.GOLD_LIMIT;
					}
				}
				else if (Integer.valueOf(rewardItem.getItemId()).intValue() == Const.changeType.CHANGE_FATIGUE_VALUE) {
					// 更新活力值
					player.updateFatigue();
					if (player.getPlayerData().getStatisticsEntity().getFatigue() +  rewardItem.getCount() > GsConst.MAX_FATIGUE_COUNT) {
						return AwardCheckResult.FATIGUE_LIMIT;
					}
				}
			}
		}

		return 0;
	}

	/**
	 * 功能发放奖励
	 */
	public boolean rewardTakeAffect(Player player, Action action) {
		try {
			// 先计算玩家属性奖励
			for (int i = 0; i < rewardInfo.getRewardItemsBuilderList().size(); ) {
				RewardItem.Builder item = rewardInfo.getRewardItemsBuilder(i);
				boolean invalidType = false;
				if (item.getType() == itemType.PLAYER_ATTR_VALUE) {
					switch (Integer.parseInt(item.getItemId())) {
					case changeType.CHANGE_COIN_VALUE:
						player.increaseCoin(item.getCount(), action);
						break;

					case changeType.CHANGE_GOLD_VALUE:
						player.increaseFreeGold(item.getCount(), action);
						break;

					case changeType.CHANGE_GOLD_BUY_VALUE:
						player.increaseBuyGold(item.getCount(), action);
						break;

					case changeType.CHANGE_PLAYER_EXP_VALUE:
						int oldLevel = player.getLevel();
						player.increaseExp(item.getCount(), action);
						int newLevel = player.getLevel();

						if (newLevel > oldLevel) {
							for (int lv = oldLevel + 1; lv <= newLevel; ++lv) {
								PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, lv);
								if (null != attrCfg) {
									RewardItem.Builder rewardItem = RewardItem.newBuilder();
									rewardItem.setType(itemType.PLAYER_ATTR_VALUE);
									rewardItem.setItemId(String.valueOf(changeType.CHANGE_FATIGUE_VALUE));
									rewardItem.setCount(attrCfg.getFatigueReward());
									rewardInfo.addRewardItems(rewardItem);
								}
							}
						}
						break;

					case changeType.CHANGE_FATIGUE_VALUE:
						player.increaseFatigue(item.getCount(), action);
						break;

					// GM命令 ,不会和CHANGE_PLAYER_EXP_VALUE同时出现
					case changeType.CHANGE_PLAYER_LEVEL_VALUE:
						oldLevel = player.getLevel();
						player.setLevel(item.getCount(), action);
						newLevel = player.getLevel();

						if (newLevel > oldLevel) {
							for (int lv = oldLevel + 1; lv <= newLevel; ++lv) {
								PlayerAttrCfg attrCfg = HawkConfigManager.getInstance().getConfigByKey(PlayerAttrCfg.class, lv);
								if (null != attrCfg) {
									RewardItem.Builder rewardItem = RewardItem.newBuilder();
									rewardItem.setType(itemType.PLAYER_ATTR_VALUE);
									rewardItem.setItemId(String.valueOf(changeType.CHANGE_FATIGUE_VALUE));
									rewardItem.setCount(attrCfg.getFatigueReward());
									rewardInfo.addRewardItems(rewardItem);
								}
							}
						}
						break;

					default:
						invalidType = true;
						break;
					}

					SynPlayerAttr.Builder playerBuilder = rewardInfo.getPlayerAttrBuilder();
					playerBuilder.setCoin(player.getCoin());
					playerBuilder.setGold(player.getGold());
					playerBuilder.setExp(player.getExp());
					playerBuilder.setLevel(player.getLevel());
					playerBuilder.setFatigue(player.getPlayerData().getStatisticsEntity().getFatigue());
					playerBuilder.setFatigueBeginTime((int)(player.getPlayerData().getStatisticsEntity().getFatigueBeginTime().getTimeInMillis() / 1000));
				}

				if (invalidType== true) {
					rewardInfo.removeRewardItems(i);
				}
				else {
					++i;
				}
			}

			// 计算其它奖励
			for (int i = 0; i < rewardInfo.getRewardItemsBuilderList().size(); ) {
				RewardItem.Builder item = rewardInfo.getRewardItemsBuilder(i);
				boolean invalidType = false;
				boolean rewardFail = false;
				switch (item.getType()) {
				case itemType.MONSTER_ATTR_VALUE:
					if (item.getId() == 0 || player.getPlayerData().getMonsterEntity((int)item.getId()) == null) {
						// 强制清除没有id的怪物奖励
						invalidType = true;
					} else {
						if (Integer.parseInt(item.getItemId()) == changeType.CHANGE_MONSTER_EXP_VALUE) {
							player.increaseMonsterExp((int)item.getId(), item.getCount(), action);
						} else if (Integer.parseInt(item.getItemId()) == changeType.CHANGE_MONSTER_LEVEL_VALUE) {
							player.setMonsterLevel((int)item.getId(), item.getCount(), action);
						}

						SynMonsterAttr.Builder monsterBuilder = null;
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
						} else {
							monsterBuilder.setExp(player.getMonsterExp((int)item.getId()));
							monsterBuilder.setLevel(player.getMonsterLevel((int)item.getId()));
						}
					}
					break;

				case itemType.ITEM_VALUE:
					ItemEntity itemEntity = player.increaseItem(item.getItemId(), item.getCount(), action);
					if (itemEntity == null) {
						rewardFail = true;
					}
					break;

				case itemType.EQUIP_VALUE:
					EquipEntity equipEntity = player.increaseEquip(item.getItemId(), item.getStage(), item.getLevel(), action);
					if (equipEntity == null) {
						rewardFail = true;
					} else {
						EquipUtil.generateAttr(equipEntity, item);
						item.setId(equipEntity.getId());
					}
					break;

				case itemType.MONSTER_VALUE:
					MonsterEntity monsterEntity = player.increaseMonster(item.getItemId(), item.getStage(), action);
					if (monsterEntity == null) {
						rewardFail = true;
					} else {
						HSMonster monster = item.getMonster();
						if (monster != null) {
							monsterEntity.setLevel(monster.getLevel());
							monsterEntity.setExp(monster.getExp());
							monsterEntity.setLazy((byte)monster.getLazy());
							monsterEntity.setLazyExp(monster.getLazyExp());
							monsterEntity.setDisposition((byte)monster.getDisposition());
							monsterEntity.notifyUpdate(false);
						}

						item.setId(monsterEntity.getId());
						item.setMonster(BuilderUtil.genMonsterBuilder(monsterEntity));
					}
					break;

				default:
					invalidType = true;
					break;
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
	 */
	public void rewardTakeAffectAndPush(Player player, Action action, int hsCode) {
		if (rewardTakeAffect(player, action) == true) {
			rewardInfo.setHsCode(hsCode);
			player.sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_REWARD_S_VALUE, rewardInfo));
		}
	}

}
