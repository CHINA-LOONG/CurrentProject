package com.hawk.game.util;

import java.util.Map;
import java.util.Map.Entry;

import org.hawk.os.HawkTime;
import org.hawk.util.services.HawkOrderService;

import com.hawk.game.ServerData;
import com.hawk.game.config.ShopCfg;
import com.hawk.game.entity.AdventureEntity;
import com.hawk.game.entity.AdventureTeamEntity;
import com.hawk.game.entity.AllianceBaseEntity;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MailEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.ShopEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.GemInfo;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Adventure.HSAdventure;
import com.hawk.game.protocol.Adventure.HSAdventureCondition;
import com.hawk.game.protocol.Adventure.HSAdventureTeam;
import com.hawk.game.protocol.Alliance.AllianceBaseMonster;
import com.hawk.game.protocol.Attribute.Attr;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Equip.EquipInfo;
import com.hawk.game.protocol.Equip.GemPunch;
import com.hawk.game.protocol.Item.ItemInfo;
import com.hawk.game.protocol.Mail.HSMail;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Player.PlayerInfo;
import com.hawk.game.protocol.Shop.HSShopRefreshTimeSync;
import com.hawk.game.protocol.Skill.HSSkill;
import com.hawk.game.protocol.Statistics.ChapterState;
import com.hawk.game.protocol.Statistics.HSStatisticsSyncPart1;
import com.hawk.game.protocol.Statistics.HSStatisticsSyncPart2;
import com.hawk.game.protocol.Statistics.HSStatisticsSyncPart3;
import com.hawk.game.protocol.Statistics.HSSyncDailyRefresh;
import com.hawk.game.protocol.Statistics.HSSyncExpLeftTimes;
import com.hawk.game.protocol.Statistics.HSSyncMonthlyRefresh;
import com.hawk.game.protocol.Statistics.HSSyncShopRefresh;
import com.hawk.game.protocol.Statistics.HoleState;
import com.hawk.game.protocol.Statistics.InstanceState;
import com.hawk.game.protocol.Statistics.ItemState;
import com.hawk.game.protocol.Statistics.RechargeState;
import com.hawk.game.protocol.Statistics.TowerState;
import com.hawk.game.util.AdventureUtil.AdventureCondition;

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
		builder.setCareer(playerEntity.getCareer());
		builder.setLevel(playerEntity.getLevel());
		builder.setExp(playerEntity.getExp());
		builder.setGold(playerEntity.getBuyGold() + playerEntity.getFreeGold());
		builder.setCoin(playerEntity.getCoin());
		builder.setTowerCoin(playerEntity.getTowerCoin());
		builder.setHonor(playerEntity.getHonorPoint());
		builder.setGender(playerEntity.getLevel());
		builder.setEye(playerEntity.getExp());
		builder.setHair(playerEntity.getHair());
		builder.setHairColor(playerEntity.getHairColor());
		builder.setRecharge(playerEntity.getRecharge());
		builder.setVipLevel(playerEntity.getVipLevel());
		return builder;
	}

	public static HSStatisticsSyncPart1.Builder genStatisticsPart1Builder(StatisticsEntity statisticsEntity) {
		HSStatisticsSyncPart1.Builder builder = HSStatisticsSyncPart1.newBuilder();

		builder.setTimeStamp(HawkTime.getSeconds());

		for (Entry<String, int[]> entry : statisticsEntity.getInstanceStateMap().entrySet()) {
			int[] state = entry.getValue();
			if (state.length != GsConst.Instance.STATE_STORY_SIZE) {
				continue;
			}
			InstanceState.Builder instanceState = InstanceState.newBuilder();
			instanceState.setInstanceId(entry.getKey());
			instanceState.setStar(state[GsConst.Instance.STATE_STAR_INDEX]);
			instanceState.setCountDaily(state[GsConst.Instance.STATE_ENTER_INDEX]);

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

		for (Entry<Integer, Boolean> entry : ServerData.getInstance().getHoleStateMap().entrySet()) {
			HoleState.Builder holeState = HoleState.newBuilder();
			holeState.setHoleId(entry.getKey());
			holeState.setIsOpen(entry.getValue());
			holeState.setCountDaily(statisticsEntity.getHoleTimesDaily(entry.getKey()));

			builder.addHoleState(holeState);
		}

		for (Entry<Integer, Integer> entry : statisticsEntity.getTowerFloorMap().entrySet()) {
			TowerState.Builder towerState = TowerState.newBuilder();
			towerState.setTowerId(entry.getKey());
			towerState.setFloor(entry.getValue());

			builder.addTowerState(towerState);
		}

		builder.setInstanceResetCount(statisticsEntity.getInstanceResetTimesDaily());

		return builder;
	}

	public static HSStatisticsSyncPart2.Builder genStatisticsPart2Builder(StatisticsEntity statisticsEntity) {
		HSStatisticsSyncPart2.Builder builder = HSStatisticsSyncPart2.newBuilder();

		builder.addAllMonsterCollect(statisticsEntity.getMonsterCollectSet());

		for (Entry<String, Integer> entry : statisticsEntity.getUseItemCountDailyMap().entrySet()) {
			ItemState.Builder itemState = ItemState.newBuilder();
			itemState.setItemId(entry.getKey());
			itemState.setUseCountDaily(entry.getValue());

			builder.addItemState(itemState);
		}

		builder.setFatigue(statisticsEntity.getFatigue());
		builder.setFatigueBeginTime((int)(statisticsEntity.getFatigueBeginTime().getTimeInMillis() / 1000));
		builder.setSkillPoint(statisticsEntity.getSkillPoint());
		builder.setSkillPointBeginTime(statisticsEntity.getSkillPointBeginTime());
		builder.setPvpTimes(statisticsEntity.getPVPTime());
		builder.setPvpTimesBeginTime(statisticsEntity.getPVPTimeBeginTime());
		builder.setAdventureChange(statisticsEntity.getAdventureChange());
		builder.setAdventureChangeBeginTime((int)(statisticsEntity.getAdventureChangeBeginTime().getTimeInMillis() / 1000));
		builder.setSummonDiamondFreeBeginTime((int)(statisticsEntity.getEggDiamond1FreePointBeginTime().getTimeInMillis() / 1000));
		builder.setSummonCoinFreeTimesDaily(statisticsEntity.getEggCoin1FreeTimesDaily());
		builder.setSummonCoinFreeLastTime((int)(statisticsEntity.getEggCoin1FreeLastTime().getTimeInMillis() / 1000));
		builder.addAllHiredMonsterId(statisticsEntity.getHireMonsterDailySet());

		return builder;
	}

	public static HSStatisticsSyncPart3.Builder genStatisticsPart3Builder(StatisticsEntity statisticsEntity) {
		HSStatisticsSyncPart3.Builder builder = HSStatisticsSyncPart3.newBuilder();

		builder.setOrderServerKey(HawkOrderService.getInstance().getSuuid());

		for (Entry<String, Integer> entry : statisticsEntity.getRechargeRecordMap().entrySet()) {
			RechargeState.Builder rechargeState = RechargeState.newBuilder();
			rechargeState.setProductId(entry.getKey());
			rechargeState.setBuyTimes(entry.getValue());
			builder.addRechargeState(rechargeState);
		}

		if (statisticsEntity.getMonthCardEndTime() == null || HawkTime.getCalendar().compareTo(statisticsEntity.getMonthCardEndTime()) > 0) {
			builder.setMonthCardLeft(0);
		}
		else {
			builder.setMonthCardLeft(HawkTime.calendarDiff(statisticsEntity.getMonthCardEndTime(), HawkTime.getCalendar()));
		}

		builder.setGold2CoinTimes(statisticsEntity.getBuyCoinTimesDaily());
		builder.setExpLeftTimes(genSyncExpLeftTimesBuilder(statisticsEntity));
		builder.setSigninTimesMonthly(statisticsEntity.getSigninTimesMonthly());
		builder.setSigninFillTimesMonthly(statisticsEntity.getSigninFillTimesMonthly());
		builder.setIsSigninDaily(statisticsEntity.isSigninDaily());
		builder.setLoginTimesDaily(statisticsEntity.getLoginTimesDaily());
		builder.setDumpEndTime(statisticsEntity.getDumpTime());

		return builder;
	}

	public static HSSyncExpLeftTimes.Builder genSyncExpLeftTimesBuilder(StatisticsEntity statisticsEntity) {
		HSSyncExpLeftTimes.Builder builder = HSSyncExpLeftTimes.newBuilder();
		builder.setDoubleExpLeft(statisticsEntity.getDoubleExpLeft());
		builder.setTripleExpLeft(statisticsEntity.getTripleExpLeft());
		return builder;
	}

	public static HSShopRefreshTimeSync.Builder genShopRefreshTimeBuilder(Player player) {
		HSShopRefreshTimeSync.Builder builder = HSShopRefreshTimeSync.newBuilder();
		for (int i = Const.shopType.NORMALSHOP_VALUE; i <= Const.shopType.SHOPNUM_VALUE; ++i) {
			ShopCfg shopCfg = ShopCfg.getShopCfg(i, player.getLevel());
			ShopEntity shopEntity = player.getPlayerData().getShopEntity(i);
			if (i == Const.shopType.NORMALSHOP_VALUE) {
				builder.setNormalShopRefreshTime(shopCfg == null ? 0 : (shopCfg.getRefreshMaxNumByHand() - shopEntity.getRefreshNums()));
			}
			else if (i == Const.shopType.ALLIANCESHOP_VALUE) {
				builder.setAllianceShopRefreshTime(shopCfg == null ? 0 : (shopCfg.getRefreshMaxNumByHand() - shopEntity.getRefreshNums()));
			}
			else if (i == Const.shopType.TOWERSHOP_VALUE) {
				builder.setTowerShopRefreshTime(shopCfg == null ? 0 : (shopCfg.getRefreshMaxNumByHand() - shopEntity.getRefreshNums()));
			}
			else if (i == Const.shopType.PVPSHOP_VALUE) {
				builder.setTowerShopRefreshTime(shopCfg == null ? 0 : (shopCfg.getRefreshMaxNumByHand() - shopEntity.getRefreshNums()));
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

	public static HSMonster.Builder genMonsterBuilder(MonsterEntity monsterEntity, PlayerAllianceEntity playerAllianceEntity) {
		HSMonster.Builder builder = HSMonster.newBuilder();
		builder.setMonsterId(monsterEntity.getId());
		builder.setCfgId(monsterEntity.getCfgId());
		builder.setStage(monsterEntity.getStage());
		builder.setLevel(monsterEntity.getLevel());
		builder.setExp(monsterEntity.getExp());
		builder.setLazy(monsterEntity.getLazy());
		builder.setLazyExp(monsterEntity.getLazyExp());
		builder.setDisposition(monsterEntity.getDisposition());
		if (playerAllianceEntity != null && playerAllianceEntity.ismonsterSendtoBase(monsterEntity.getId())) {
			builder.setState(monsterEntity.getState() | Const.MonsterState.IN_ALLIANCE_BASE_VALUE);
		}
		else {
			builder.setState(monsterEntity.getState());
		}

		HSSkill.Builder skill = HSSkill.newBuilder();
		for (Entry<String, Integer> entry : monsterEntity.getSkillMap().entrySet()) {
			skill.setSkillId(entry.getKey());
			skill.setLevel(entry.getValue());
			builder.addSkill(skill);
		}
		return builder;
	}

	/**
	 * 生成宠物完整镜像 包括装备和装备上的宝石
	 * @param player
	 * @param monsterEntity
	 * @return
	 */
	public static HSMonster.Builder genCompleteMonsterBuilder(Player player, MonsterEntity monsterEntity){
		HSMonster.Builder builder = genMonsterBuilder(monsterEntity, null);
		// 组装装备
		Map<Integer, Long> equips = player.getPlayerData().getMonsterEquips(monsterEntity.getId());
		if (equips != null) {
			for (long equipId : equips.values()) {
				EquipEntity equipEntity = player.getPlayerData().getEquipById(equipId);
				if (equipEntity != null) {
					builder.addEquipInfos(genEquipBuilder(equipEntity));
				}
			}
		}
		return builder;
	}

	/**
	 * 生成基地驻兵信息
	 * @param baseEntity
	 * @return
	 */
	public static AllianceBaseMonster.Builder genAllianceBaseMonster(AllianceBaseEntity baseEntity, boolean myBase){
		AllianceBaseMonster.Builder builder = AllianceBaseMonster.newBuilder();
		builder.setId(baseEntity.getId());
		builder.setMonsterId(baseEntity.getMonsterBuilder().getMonsterId());
		builder.setCfgId(baseEntity.getMonsterBuilder().getCfgId());
		builder.setLevel(baseEntity.getMonsterBuilder().getLevel());
		builder.setStage(baseEntity.getMonsterBuilder().getStage());
		builder.setBp(baseEntity.getBp());
		builder.setNickname(baseEntity.getNickname());
		if (myBase == true) {
			builder.setSendTime(baseEntity.getSendTime());
			builder.setPosition(baseEntity.getPosition());
			builder.setReward(AllianceUtil.getAllianceBaseDefReward(baseEntity.getBp(), baseEntity.getSendTime()));
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

	public static HSAdventureTeam.Builder genAdventureTeamBuilder(AdventureTeamEntity teamEntity) {
		HSAdventureTeam.Builder builder = HSAdventureTeam.newBuilder();
		builder.setTeamId(teamEntity.getTeamId());
		builder.setAdventureId(teamEntity.getAdventureId());
		builder.setEndTime(teamEntity.getEndTime());
		builder.addAllSelfMonsterId(teamEntity.getSelfMonsterList());
		if (null != teamEntity.getHireMonster()) {
			builder.setHireMonster(teamEntity.getHireMonster());
		}
		return builder;
	}

	public static HSAdventure.Builder genAdventureBuilder(AdventureEntity adventureEntity) {
		HSAdventure.Builder builder = HSAdventure.newBuilder();
		builder.setAdventureId(adventureEntity.getAdventureId());

		for (AdventureCondition condition : adventureEntity.getConditionList()) {
			HSAdventureCondition.Builder conditionBuilder = HSAdventureCondition.newBuilder();
			conditionBuilder.setMonsterCount(condition.monsterCount);
			conditionBuilder.setConditionTypeCfgId(condition.conditionTypeCfgId);
			builder.addCondition(conditionBuilder);
		}

		return builder;
	}
}
