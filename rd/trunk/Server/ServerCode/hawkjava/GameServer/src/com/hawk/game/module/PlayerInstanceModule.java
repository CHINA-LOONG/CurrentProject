package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import javax.net.ssl.SSLEngineResult.HandshakeStatus;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkRand;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.InstanceCfg;
import com.hawk.game.config.InstanceDropCfg;
import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.config.InstanceRewardCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.config.RewardGroupCfg;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.Const.BattleType;
import com.hawk.game.protocol.Const.itemType;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Instance.HSBattle;
import com.hawk.game.protocol.Instance.HSInstanceAssist;
import com.hawk.game.protocol.Instance.HSInstanceAssistRet;
import com.hawk.game.protocol.Instance.HSInstanceEnter;
import com.hawk.game.protocol.Instance.HSInstanceEnterRet;
import com.hawk.game.protocol.Instance.HSInstanceOpenCard;
import com.hawk.game.protocol.Instance.HSInstanceOpenCardRet;
import com.hawk.game.protocol.Instance.HSInstanceSettle;
import com.hawk.game.protocol.Instance.HSInstanceSettleRet;
import com.hawk.game.protocol.Reward.HSRewardInfo;
import com.hawk.game.protocol.Reward.RewardItem;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.InstanceUtil;
import com.hawk.game.util.ProtoUtil;
import com.hawk.game.util.InstanceUtil.InstanceChapter;

public class PlayerInstanceModule extends PlayerModule {

	private static final Logger logger = LoggerFactory.getLogger("Protocol");
	
	private String instanceId;
	// 本次对局列表
	private List<HSBattle> battleList;
	// 本次对局掉落列表
	private List<List<ItemInfo>> battleDropList;
	// 本次结算星级奖牌列表
	private List<ItemInfo> cardRewardList;

	public PlayerInstanceModule(Player player) {
		super(player);

		instanceId = "";
		battleList = new ArrayList<HSBattle>();
		battleDropList = new ArrayList<List<ItemInfo>>();
		cardRewardList = new ArrayList<ItemInfo>();
	}

	/**
	 * 获取助战列表
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_ASSIST_C_VALUE)
	private boolean onInstanceAssist(HawkProtocol cmd) {
		HSInstanceAssist protocol = cmd.parseProtocol(HSInstanceAssist.getDefaultInstance());
		int hsCode = cmd.getType();
		
		// TODO
		
		HSInstanceAssistRet.Builder response = HSInstanceAssistRet.newBuilder();
		//response.addAllAssist(values);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ASSIST_S, response));
		return true;
	}

	/**
	 * 副本进入
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_ENTER_C_VALUE)
	private boolean onInstanceEnter(HawkProtocol cmd) {
		HSInstanceEnter protocol = cmd.parseProtocol(HSInstanceEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		String cfgId = protocol.getCfgId();

		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, cfgId);
		if (entryCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return false;
		}
		
		int chapterId = entryCfg.getChapter();
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		Map<Integer, InstanceChapter> chapterMap = InstanceUtil.getInstanceChapterMap();
		InstanceChapter chapter = chapterMap.get(chapterId);
		
		// 章节已开启，前置副本完成
		// 精英副本，必须通关普通章节
		if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
			int index = chapter.normalList.indexOf(entryCfg);
			int size = chapter.normalList.size();
			int curChapterId = statisticsEntity.getNormalInstanceChapter();
			int curIndex = statisticsEntity.getNormalInstanceIndex();
			
			if ((chapterId > curChapterId && (index != size - 1 || chapterId > curChapterId + 1)) ||
					(chapterId == curChapterId && index > curIndex + 1)) {
				sendError(hsCode, Status.instanceError.INSTANCE_NOT_OPEN);
				return false;
			}
		} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
			int normalSize = chapter.normalList.size();
			int normalCurChapterId = statisticsEntity.getNormalInstanceChapter();
			int normalCurIndex = statisticsEntity.getNormalInstanceIndex();
			int hardIndex = chapter.hardList.indexOf(entryCfg);
			int hardSize = chapter.hardList.size();
			int hardCurChapterId = statisticsEntity.getHardInstanceChapter();
			int hardCurIndex = statisticsEntity.getHardInstanceIndex();

			if ((chapterId > normalCurChapterId) ||
					(chapterId == normalCurChapterId && normalCurIndex != normalSize - 1) ||
					(chapterId > hardCurChapterId && (hardIndex != hardSize - 1 || chapterId > hardCurChapterId + 1)) ||
					(chapterId == hardCurChapterId && hardIndex > hardCurIndex + 1)) {
				sendError(hsCode, Status.instanceError.INSTANCE_NOT_OPEN);
				return false;
			}
		}

		// 副本等级
		if (player.getLevel() < entryCfg.getLevel()) {
			sendError(hsCode, Status.instanceError.INSTANCE_LEVEL);
			return false;
		}
		
		// 次数
		if (statisticsEntity.getInstanceCountDaily(cfgId) >= entryCfg.getCount()) {
			sendError(hsCode, Status.instanceError.INSTANCE_COUNT);
			return false;
		}
		
		// 体力
		if (statisticsEntity.getFatigue() < entryCfg.getFatigue()) {
			sendError(hsCode, Status.instanceError.INSTANCE_FATIGUE);
			return false;
		}
		
		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, cfgId);
		if (instanceCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return false;
		}
		
		this.instanceId = cfgId;
		if (false == this.battleList.isEmpty() || false == this.battleDropList.isEmpty() || false == this.cardRewardList.isEmpty()) {
			logger.error("instance data is not empty when enter instance");
			this.battleList.clear();
			this.battleDropList.clear();
			this.cardRewardList.clear();
		}

		// 生成对局
		// normal
		List<String> randMonsterList = new ArrayList<String>();
		for (Entry<String, Integer> entry : instanceCfg.getMonsterAmountList().entrySet()) {
			for (int i = entry.getValue(); i > 0; --i) {
				randMonsterList.add(entry.getKey());
			}
		}
		HawkRand.randomOrder(randMonsterList);
		
		Iterator<String> iter = randMonsterList.iterator();
		for (int i = 0; i < instanceCfg.getBattleAmount(); ++i) {
			HSBattle.Builder battle = HSBattle.newBuilder();
			List<ItemInfo> dropList = new ArrayList<ItemInfo>();
			
			for (int j = 0; j < instanceCfg.getBattleMonsterAmount(); ++j) {
				String monsterCfgId = iter.next();
				battle.addMonsterCfgId(monsterCfgId);
				
				String instanceMonsterId = instanceId + "_" + monsterCfgId;
				InstanceDropCfg dropCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceDropCfg.class, instanceMonsterId);
				if (dropCfg != null) {
					dropList.addAll(dropCfg.getReward().getRewardList());
				}
			}

			battle.setDropReward(AwardItems.valueOf(dropList).getBuilder());
			this.battleList.add(battle.build());
			this.battleDropList.add(dropList);
		}
		
		// boss
		HSBattle.Builder bossBattle = HSBattle.newBuilder();
		bossBattle.setType(BattleType.BOSS);
		bossBattle.addMonsterCfgId(instanceCfg.getBossId());
		String instanceMonsterId = instanceId + "_" + instanceCfg.getBossId();
		InstanceDropCfg dropCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceDropCfg.class, instanceMonsterId);
		if (dropCfg != null) {
			 List<ItemInfo> dropList = dropCfg.getReward().getRewardList();
			bossBattle.setDropReward(AwardItems.valueOf(dropList).getBuilder());
			this.battleDropList.add(dropList);
		}
		else {
			this.battleDropList.add(new ArrayList<ItemInfo>());
		}
		this.battleList.add(bossBattle.build());
		
		
		// rare
		if (true == HawkRand.randPercentRate((int) (instanceCfg.getRareProbability() * 100))) {
			HSBattle.Builder rareBattle = HSBattle.newBuilder();
			rareBattle.setType(BattleType.RARE);
			rareBattle.addMonsterCfgId(instanceCfg.getRareId());
			instanceMonsterId = instanceId + "_" + instanceCfg.getRareId();
			dropCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceDropCfg.class, instanceMonsterId);
			if (dropCfg != null) {
				 List<ItemInfo> dropList = dropCfg.getReward().getRewardList();
				 rareBattle.setDropReward(AwardItems.valueOf(dropList).getBuilder());
				this.battleDropList.add(dropList);
			}
			else {
				this.battleDropList.add(new ArrayList<ItemInfo>());
			}
			this.battleList.add(rareBattle.build());
		}		

		// 体力和次数修改
		int fatigueChange = entryCfg.getFatigue();
		statisticsEntity.setFatigue(statisticsEntity.getFatigue() - fatigueChange);
		statisticsEntity.addInstanceCountDaily(cfgId, statisticsEntity.getInstanceCountDaily(cfgId) + 1);
		statisticsEntity.notifyUpdate(true);

		HSInstanceEnterRet.Builder response = HSInstanceEnterRet.newBuilder();
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		response.setCfgId(cfgId);
		response.addAllBattle(this.battleList);
		response.setFatigueChange(fatigueChange);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ENTER_S, response));
		return true;
	}
	
	/**
	 * 副本结算
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_SETTLE_C_VALUE)
	private boolean onInstanceSettle(HawkProtocol cmd) {
		HSInstanceSettle protocol = cmd.parseProtocol(HSInstanceSettle.getDefaultInstance());
		int hsCode = cmd.getType();
		List<Integer> passBattleList = protocol.getPassBattleIndexList();
		List<Integer> passBoxList = protocol.getPassBoxIndexList();
		
		AwardItems dropCompleteReward = AwardItems.valueOf();
		AwardItems completeReward = AwardItems.valueOf();
		List<RewardItem> cardList = new ArrayList<RewardItem>();
		
		boolean complete = false;
		int starCount = 0;
		
		// 验证
		for (Integer i : passBattleList) {
			if (i < 0 || i > battleList.size()) {
				sendError(hsCode, Status.error.PARAMS_INVALID);
				return false;
			}
			HSBattle battle = battleList.get(i);
			if (BattleType.BOSS == battle.getType()) {
				complete = true;
				// TODO: 评价
				starCount = 3;
			}

			// 掉落奖励
			dropCompleteReward.addItemInfos(this.battleDropList.get(i));
		}
		
		if (true == complete) {
			// 通关奖励
			InstanceRewardCfg instanceRewardCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceRewardCfg.class, instanceId);
			if (instanceRewardCfg != null) {
				List<ItemInfo> list = instanceRewardCfg.getReward().getRewardList();
				dropCompleteReward.addItemInfos(list);
				completeReward.addItemInfos(list);
			}

			// 生成翻牌奖励
			RewardGroupCfg starRewardCfg = instanceRewardCfg.getStarRewardGroup(starCount);
			for (int i = 0; i  < GsConst.INSTANCE_CARD_COUNT; ++i) {
				ItemInfo cardReward = starRewardCfg.getRewardItem();
				if (cardReward.getType() != itemType.NONE_ITEM_VALUE) {
					this.cardRewardList.add(cardReward);
					
					AwardItems convertor = AwardItems.valueOf();
					convertor.addItemInfo(cardReward);
					cardList.add(convertor.getBuilder().getRewardItemsList().get(0));
				}
			}

			// 记录副本进度
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			int oldStar = statisticsEntity.getInstanceStar(instanceId);
			if (starCount > oldStar) {
				statisticsEntity.addInstanceStar(instanceId, starCount);
			}

			statisticsEntity.addInstanceAllCount();
			statisticsEntity.addInstanceAllCountDaily();

			InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
			if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
				statisticsEntity.addHardCount();
				statisticsEntity.addHardCountDaily();
			}

			statisticsEntity.notifyUpdate(true);	
		}

		// TODO: 体力扣除

		// 发放掉落奖励和完成奖励
		dropCompleteReward.rewardTakeAffectAndPush(player,  Action.INSTACE_SETTLE);
		
		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		if (true == complete) {
			response.setStarCount(starCount);
			response.setCompleteReward(completeReward.getBuilder());
			response.addAllCardReward(cardList);
		}
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));
		
		// 清空副本数据
		this.battleList.clear();
		this.battleDropList.clear();
		
		return true;		
	}
	
	/**
	 * 翻牌
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_OPEN_CARD_C_VALUE)
	private boolean onInstanceOpenCard(HawkProtocol cmd) {
		HSInstanceOpenCard protocol = cmd.parseProtocol(HSInstanceOpenCard.getDefaultInstance());
		int hsCode = cmd.getType();
		int openCount = protocol.getOpenCount();
		
		if (openCount > cardRewardList.size()) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return false;
		}

		ConsumeItems cardConsume = ConsumeItems.valueOf();
		AwardItems cardReward = AwardItems.valueOf();
		
		for (int i = 0; i < openCount; ++i) {
			cardReward.addItemInfo(cardRewardList.get(i));
		}
		// TODO: 消耗
		
		cardReward.rewardTakeAffectAndPush(player,  Action.INSTACE_SETTLE);
		
		HSInstanceOpenCardRet.Builder response = HSInstanceOpenCardRet.newBuilder();
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_OPEN_CARD_S, response));
		
		// 清空翻牌数据
		cardRewardList.clear();
		
		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 清空上次副本数据
		this.battleList.clear();
		this.battleDropList.clear();
		this.cardRewardList.clear();
		
		return true;
	}
	
	@Override
	protected boolean onPlayerLogout() {
		// do nothing
		return true;
	}
}
