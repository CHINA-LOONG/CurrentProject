package com.hawk.game.util;

import java.util.Map;
import java.util.Map.Entry;

import org.hawk.os.HawkTime;
import org.hawk.util.services.HawkOrderService;

import com.hawk.game.ServerData;
import com.hawk.game.config.ShopCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.ShopEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.GemInfo;
import com.hawk.game.manager.ImManager.ImMsg;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Attribute.Attr;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Equip.EquipInfo;
import com.hawk.game.protocol.Equip.GemPunch;
import com.hawk.game.protocol.Im.HSImMsg;
import com.hawk.game.protocol.Item.ItemInfo;
import com.hawk.game.protocol.Mail.HSMail;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Shop.HSShopRefreshTimeSync;
import com.hawk.game.protocol.Statistics.ChapterState;
import com.hawk.game.protocol.Statistics.HSStatisticsInfoSync;
import com.hawk.game.protocol.Statistics.HSSyncDailyRefresh;
import com.hawk.game.protocol.Statistics.HSSyncExpLeftTimes;
import com.hawk.game.protocol.Statistics.HSSyncMonthlyRefresh;
import com.hawk.game.protocol.Statistics.HSSyncShopRefresh;
import com.hawk.game.protocol.Statistics.HoleState;
import com.hawk.game.protocol.Statistics.ItemState;
import com.hawk.game.protocol.Player.PlayerInfo;
import com.hawk.game.protocol.Skill.HSSkill;
import com.hawk.game.protocol.Statistics.InstanceState;
import com.hawk.game.protocol.Statistics.RechargeState;
import com.hawk.game.protocol.Statistics.TowerState;

public class BuilderUtil {

	/**
	 * 生成玩家协议同步信息
	 * 
	 * @return
	 */
	public static PlayerInfo.Builder genPlayerBuilder(PlayerEntity playerEntity) {
		PlayerInfo.Builder builder = PlayerInfo.newBuilder();
		builder.setPlayerId(playerEntity.getId());
		builder.setNickname(playerEntity.getNickname());
		builder.setGold(playerEntity.getBuyGold() + playerEntity.getFreeGold());
		builder.setCoin(playerEntity.getCoin());
		builder.setExp(playerEntity.getExp());
		builder.setCareer(playerEntity.getCareer());
		builder.setLevel(playerEntity.getLevel());
		builder.setGender(playerEntity.getLevel());
		builder.setEye(playerEntity.getExp());
		builder.setHair(playerEntity.getHair());
		builder.setHairColor(playerEntity.getHairColor());
		builder.addAllBattleMonster(playerEntity.getBattleMonsterList());
		builder.setRecharge(playerEntity.getRecharge());
		builder.setVipLevel(playerEntity.getVipLevel());
		return builder;
	}

	public static HSStatisticsInfoSync.Builder genStatisticsBuilder(StatisticsEntity statisticsEntity) {
		HSStatisticsInfoSync.Builder builder = HSStatisticsInfoSync.newBuilder();

		for (Entry<String, Integer> entry : statisticsEntity.getInstanceStarMap().entrySet()) {
			InstanceState.Builder instanceState = InstanceState.newBuilder();
			instanceState.setInstanceId(entry.getKey());
			instanceState.setStar(entry.getValue());
			instanceState.setCountDaily(statisticsEntity.getInstanceCountDaily(entry.getKey()));

			builder.addInstanceState(instanceState);
		}

		ChapterState.Builder chapterState = ChapterState.newBuilder();
		chapterState.setNormalTopChapter(statisticsEntity.getNormalTopChapter());
		chapterState.setNormalTopIndex(statisticsEntity.getNormalTopIndex());
		chapterState.setHardTopChapter(statisticsEntity.getHardTopChapter());
		chapterState.setHardTopIndex(statisticsEntity.getHardTopIndex());
		chapterState.addAllNormalBoxState(statisticsEntity.getNormalChapterBoxStateList());
		chapterState.addAllHardBoxState(statisticsEntity.getHardChapterBoxStateList());
		builder.setChapterState(chapterState);

		builder.addAllMonsterCollect(statisticsEntity.getMonsterCollectSet());
		builder.setFatigue(statisticsEntity.getFatigue());
		builder.setFatigueBeginTime((int)(statisticsEntity.getFatigueBeginTime().getTimeInMillis() / 1000));
		builder.setSkillPoint(statisticsEntity.getSkillPoint());
		builder.setSkillPointBeginTime((int)(statisticsEntity.getSkillPointBeginTime().getTimeInMillis() / 1000));
		builder.setTimeStamp(HawkTime.getSeconds());

		// 设置订单服务器key
		builder.setOrderServerKey(HawkOrderService.getInstance().getSuuid());
		if (statisticsEntity.getMonthCardEndTime() == null || HawkTime.getCalendar().compareTo(statisticsEntity.getMonthCardEndTime()) > 0) {
			builder.setMonthCardLeft(0);
		}
		else {		
			builder.setMonthCardLeft(HawkTime.calendarDiff(statisticsEntity.getMonthCardEndTime(), HawkTime.getCalendar()));
		}

		for (Entry<String, Integer> entry : statisticsEntity.getRechargeRecordMap().entrySet()) {
			RechargeState.Builder rechargeState = RechargeState.newBuilder();
			rechargeState.setProductId(entry.getKey());
			rechargeState.setBuyTimes(entry.getValue());
			builder.addRechargeState(rechargeState);
		}

		builder.setGold2CoinTimes(statisticsEntity.getCoinOrderCountDaily());
		builder.setExpLeftTimes(genSyncExpLeftTimesBuilder(statisticsEntity));
		builder.setInstanceResetCount(statisticsEntity.getInstanceResetCountDaily());

		for (Entry<String, Integer> entry : statisticsEntity.getItemUseCountDailyMap().entrySet()) {
			ItemState.Builder itemState = ItemState.newBuilder();
			itemState.setItemId(entry.getKey());
			itemState.setUseCountDaily(entry.getValue());

			builder.addItemState(itemState);
		}

		for (Entry<Integer, Boolean> entry : ServerData.getInstance().getHoleStateMap().entrySet()) {
			HoleState.Builder holeState = HoleState.newBuilder();
			holeState.setHoleId(entry.getKey());
			holeState.setIsOpen(entry.getValue());
			holeState.setCountDaily(statisticsEntity.getHoleCountDaily(entry.getKey()));

			builder.addHoleState(holeState);
		}

		for (Entry<Integer, Integer> entry : statisticsEntity.getTowerIndexMap().entrySet()) {
			TowerState.Builder towerState = TowerState.newBuilder();
			towerState.setTowerId(entry.getKey());
			towerState.setIndex(entry.getValue());

			builder.addTowerState(towerState);
		}

		return builder;
	}

	public static HSSyncExpLeftTimes.Builder genSyncExpLeftTimesBuilder(StatisticsEntity statisticsEntity) {
		HSSyncExpLeftTimes.Builder builder = HSSyncExpLeftTimes.newBuilder();
		builder.setDoubleExpLeft(statisticsEntity.getDoubleExpLeftTimes());
		builder.setTripleExpLeft(statisticsEntity.getTripleExpLeftTimes());
		return builder;
	}

	public static HSShopRefreshTimeSync.Builder genShopRefreshTimeBuilder(Player player, ShopEntity shopEntity) {
		HSShopRefreshTimeSync.Builder builder = HSShopRefreshTimeSync.newBuilder();
		for (int i = Const.shopType.NORMALSHOP_VALUE; i <= Const.shopType.OTHERSHOP_VALUE; ++i) {
			ShopCfg shopCfg = ShopCfg.getShopCfg(i, player.getLevel());
			if (i == Const.shopType.NORMALSHOP_VALUE) {
				builder.setNormalShopRefreshTime(shopCfg == null ? 0 : (shopCfg.getRefreshMaxNumByHand() - shopEntity.getShopRefreshNum(i)));
			}
			else if (i == Const.shopType.ALLIANCESHOP_VALUE) {
				builder.setAllianceShopRefreshTime(shopCfg == null ? 0 : (shopCfg.getRefreshMaxNumByHand() - shopEntity.getShopRefreshNum(i)));
			}
			else if (i == Const.shopType.OTHERSHOP_VALUE) {
				builder.setOtherShopRefreshTime(shopCfg == null ? 0 : (shopCfg.getRefreshMaxNumByHand() - shopEntity.getShopRefreshNum(i)));
			}
		}
		return builder;
	}

	public static HSSyncShopRefresh.Builder genSyncShopRefreshBuilder(int shopType) {
		HSSyncShopRefresh.Builder builder = HSSyncShopRefresh.newBuilder();
		builder.setShopType(shopType);
		return builder;
	}

	public static HSSyncDailyRefresh.Builder genSyncDailyRefreshBuilder() {
		HSSyncDailyRefresh.Builder builder = HSSyncDailyRefresh.newBuilder();

		for (Entry<Integer, Boolean> entry : ServerData.getInstance().getHoleStateMap().entrySet()) {
			HoleState.Builder holeState = HoleState.newBuilder();
			holeState.setHoleId(entry.getKey());
			holeState.setIsOpen(entry.getValue());

			builder.addHoleState(holeState);
		}
		return builder;
	}

	public static HSSyncMonthlyRefresh.Builder genSyncMonthlyRefreshBuilder() {
		HSSyncMonthlyRefresh.Builder builder = HSSyncMonthlyRefresh.newBuilder();
		return builder;
	}

	public static HSMonster.Builder genMonsterBuilder(MonsterEntity monsterEntity) {
		HSMonster.Builder builder = HSMonster.newBuilder();
		builder.setMonsterId(monsterEntity.getId());
		builder.setCfgId(monsterEntity.getCfgId());
		builder.setStage(monsterEntity.getStage());
		builder.setLevel(monsterEntity.getLevel());
		builder.setExp(monsterEntity.getExp());
		builder.setLazy(monsterEntity.getLazy());
		builder.setLazyExp(monsterEntity.getLazyExp());
		builder.setDisposition(monsterEntity.getDisposition());
		builder.setLocked(monsterEntity.isLocked());

		HSSkill.Builder skill = HSSkill.newBuilder();
		for (Entry<String, Integer> entry : monsterEntity.getSkillMap().entrySet()) {
			skill.setSkillId(entry.getKey());
			skill.setLevel(entry.getValue());
			builder.addSkill(skill);
		}
		return builder;
	}

	/**
	 * 生成物品实体的builder信息
	 * 
	 * @return
	 */
	public static ItemInfo.Builder genItemBuilder(ItemEntity itemEntity) {
		ItemInfo.Builder builder = ItemInfo.newBuilder();
		builder.setId(itemEntity.getId());
		builder.setItemId(itemEntity.getItemId());
		builder.setCount(itemEntity.getCount());
		builder.setStatus(itemEntity.getStatus());
		return builder;
	}

	/**
	 * 生成装备实体的builder信息
	 * 
	 * @return
	 */
	public static EquipInfo.Builder genEquipBuilder(EquipEntity equipEntity) {
		EquipInfo.Builder builder = EquipInfo.newBuilder();
		//组装基本数据
		builder.setId(equipEntity.getId());
		builder.setEquipId(equipEntity.getItemId());
		builder.setStage(equipEntity.getStage());
		builder.setLevel(equipEntity.getLevel());
		builder.setMonsterId(equipEntity.getMonsterId());
		builder.setStatus(0);
		if (equipEntity.getExpireTime() != 0) {
			builder.setExpireTime(equipEntity.getExpireTime());
		}

		//组装镶嵌宝石数据
		for (Map.Entry<Integer, GemInfo> entry : equipEntity.GetGemDressList().entrySet()) {
			GemPunch.Builder punchInfo = GemPunch.newBuilder();
			punchInfo.setSlot(entry.getKey());
			punchInfo.setType(entry.getValue().getType());
			punchInfo.setGemItemId(entry.getValue().getGemId());
			builder.addGemItems(punchInfo.build());
		}

		if (equipEntity.getAttr() != null) {
			//组装生成附加属性列表
			for (Map.Entry<Const.attr, Float> entry : equipEntity.getAttr().getAttrMap().entrySet()) {
				Attr.Builder attrInfo = Attr.newBuilder();
				attrInfo.setAttrId(entry.getKey().getNumber());
				attrInfo.setAttrValue(entry.getValue());
				builder.addAttrDatas(attrInfo);
			}
		}
		return builder;
	}

	public static HSMail.Builder genMailBuilder(MailEntity mailEntity) {
		HSMail.Builder builder = HSMail.newBuilder();
		builder.setMailId(mailEntity.getId());
		builder.setState(mailEntity.getState());
		builder.setSendTimeStamp(mailEntity.getCreateTime());
		builder.setSenderId(mailEntity.getSenderId());
		builder.setSenderName(mailEntity.getSenderName());
		builder.setSubject(mailEntity.getSubject());
		builder.setContent(mailEntity.getContent());
		AwardItems convetor = AwardItems.valueOf(mailEntity.getRewardList());
		builder.addAllReward(convetor.getBuilder().getRewardItemsList());
		return builder;
	}

	public static HSImMsg.Builder genImMsgBuilder(ImMsg imMsg, String language) {
		HSImMsg.Builder builder = HSImMsg.newBuilder();
		builder.setType(imMsg.type);
		builder.setChannel(imMsg.channel);
		builder.setSenderId(imMsg.senderId);
		builder.setOrigText(imMsg.origText);

		if (imMsg.senderName != null) {
			builder.setSenderName(imMsg.senderName);
		}

		if (imMsg.transText != null) {
			String transMsg = imMsg.transText.get(language);
			if (transMsg != null) {
				builder.setTransText(transMsg);
			}
		}
		return builder;
	}
}
