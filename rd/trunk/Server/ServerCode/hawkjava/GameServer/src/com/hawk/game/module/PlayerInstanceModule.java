package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkRand;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.InstanceCfg;
import com.hawk.game.config.InstanceDropCfg;
import com.hawk.game.config.InstanceRewardCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.config.RewardGroupCfg;
import com.hawk.game.entity.MonsterEntity;
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
import com.hawk.game.util.ProtoUtil;

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
	 * 副本进入
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_ENTER_C_VALUE)
	private boolean onInstanceEnter(HawkProtocol cmd) {
		HSInstanceEnter protocol = cmd.parseProtocol(HSInstanceEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		String cfgId = protocol.getCfgId();

		// TODO: 检测条件
		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, cfgId);
		if(instanceCfg == null) {
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

		// 体力修改
//		PlayerEntity playerEntity = player.getPlayerData().getPlayerEntity();
		int fatigueChange = 0;
//		playerEntity.notifyUpdate(true);

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
			// TODO:
			//if (BattleType.BOSS == battle.getType()) {
				complete = true;
				// TODO: 评价
				starCount = 3;
			//}

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
		}

		// 发放掉落奖励和完成奖励
		dropCompleteReward.rewardTakeAffectAndPush(player,  Action.ELITE_MAP_FIGHTING);
		
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
		
		cardReward.rewardTakeAffectAndPush(player,  Action.ELITE_MAP_FIGHTING);
		
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
