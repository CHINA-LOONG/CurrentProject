package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkRand;
import org.hawk.os.HawkTime;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.SummonCfg;
import com.hawk.game.config.SummonPseudoCfg;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Summon.HSSummonOne;
import com.hawk.game.protocol.Summon.HSSummonOneRet;
import com.hawk.game.protocol.Summon.HSSummonTen;
import com.hawk.game.protocol.Summon.HSSummonTenRet;
import com.hawk.game.util.GsConst;

public class PlayerSummonModule extends PlayerModule {

	public PlayerSummonModule(Player player) {
		super(player);
	}

	/**
	 * 单抽
	 */
	@ProtocolHandler(code = HS.code.SUMMON_ONE_C_VALUE)
	private boolean onSummonOne(HawkProtocol cmd) {
		HSSummonOne protocol = cmd.parseProtocol(HSSummonOne.getDefaultInstance());
		int hsCode = cmd.getType();
		int type = protocol.getType();

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int recharge = statisticsEntity.getPayDiamondCount();

		// 抽蛋奖励配置
		SummonCfg summonCfg = SummonCfg.getSummonCfg(type, recharge);
		if (null == summonCfg) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		ConsumeItems consume = ConsumeItems.valueOf();
		AwardItems reward = AwardItems.valueOf();
		HSSummonOneRet.Builder response = HSSummonOneRet.newBuilder();

		switch (type) {
		case Const.SummonType.SUMMON_COIN_VALUE: {
			consume.addCoin(GsConst.summon.ONE_COIN_PRICE);
			if (false == consume.checkConsume(player, hsCode)) {
				return true;
			}
			consume.consumeTakeAffectAndPush(player, Action.NULL, hsCode);
			reward.addItemInfos(summonCfg.getReward().getRewardList());
			statisticsEntity.increaseEggCoinTimes();
			statisticsEntity.increaseEggCoinTimesDaily();
			break;
		}

		case Const.SummonType.SUMMON_COIN_FREE_VALUE: {
			if (statisticsEntity.getEggCoinFreeTimesDaily() >= GsConst.summon.MAX_COIN_FREE_TIMES_DAILY) {
				sendError(hsCode, Status.summonError.SUMMON_COIN_NO_FREE);
				return true;
			}
			Calendar curTime = HawkTime.getCalendar();
			if (curTime.getTimeInMillis() < statisticsEntity.getEggCoinFreeLastTime().getTimeInMillis() + GsConst.summon.COIN_FREE_CD * 1000) {
				sendError(hsCode, Status.summonError.SUMMON_COIN_FREE_CD);
				return true;
			}
			reward.addItemInfos(summonCfg.getReward().getRewardList());
			statisticsEntity.increaseEggCoinFreeTimesDaily();
			statisticsEntity.setEggCoinFreeLastTime(curTime);
			response.setFreeCoinLastTime((int)(curTime.getTimeInMillis() / 1000));
			break;
		}

		case Const.SummonType.SUMMON_DIAMOND_VALUE: {
			consume.addGold(GsConst.summon.ONE_DIAMOND_PRICE);
			if (false == consume.checkConsume(player, hsCode)) {
				return true;
			}
			consume.consumeTakeAffectAndPush(player, Action.NULL, hsCode);
			// 假奖励
			SummonPseudoCfg pseudoCfg = HawkConfigManager.getInstance().getConfigByKey(SummonPseudoCfg.class, GsConst.summon.FIRST_N_ONE_PSEUDO_REWARD);
			if (statisticsEntity.getEggDiamondOneTimes() <= pseudoCfg.getTimes()) {
				reward.addItemInfos(pseudoCfg.getReward().getRewardList());
			} else {
				reward.addItemInfos(summonCfg.getReward().getRewardList());
			}
			statisticsEntity.increaseEggDiamondOneTimes();
			break;
		}

		case Const.SummonType.SUMMON_DIAMOND_FREE_VALUE: {
			if (0 >= player.regainEggDiamondPoint()) {
				sendError(hsCode, Status.summonError.SUMMON_DIAMOND_NO_FREE);
				return true;
			}
			reward.addItemInfos(summonCfg.getReward().getRewardList());
			Calendar curTime = HawkTime.getCalendar();
			// 最大1点，直接重置
			statisticsEntity.setEggDiamondPoint(0);
			statisticsEntity.setEggPointBeginTime(curTime);
			statisticsEntity.increaseEggDiamondFreeTimes();
			response.setFreeDiamondBeginTime((int)(curTime.getTimeInMillis() / 1000));
			break;
		}

		default:
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		reward.rewardTakeAffectAndPush(player, Action.NULL, hsCode);
		statisticsEntity.notifyUpdate(true);

		response.setReward(reward.getBuilder());
		sendProtocol(HawkProtocol.valueOf(HS.code.SUMMON_ONE_S, response));
		return true;
	}

	/**
	 * 十连抽
	 */
	@ProtocolHandler(code = HS.code.SUMMON_TEN_C_VALUE)
	private boolean onSummonTen(HawkProtocol cmd) {
		HSSummonTen protocol = cmd.parseProtocol(HSSummonTen.getDefaultInstance());
		int hsCode = cmd.getType();
		int type = protocol.getType();

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		int recharge = statisticsEntity.getPayDiamondCount();

		// 抽蛋奖励配置
		SummonCfg summonCfg = SummonCfg.getSummonCfg(type, recharge);
		if (null == summonCfg) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		ConsumeItems consume = ConsumeItems.valueOf();
		AwardItems allReward = AwardItems.valueOf();
		List<AwardItems> tenRewardList = new ArrayList<AwardItems>();

		switch (type) {
		case Const.SummonType.SUMMON_COIN_VALUE: {
			consume.addCoin(GsConst.summon.TEN_COIN_PRICE);
			if (false == consume.checkConsume(player, hsCode)) {
				return true;
			}
			consume.consumeTakeAffectAndPush(player, Action.NULL, hsCode);
			// 1次保底奖励
			AwardItems guaranteeReward = AwardItems.valueOf();
			List<ItemInfo> guaranteeRewardList = summonCfg.getGuaranteeReward().getRewardList();
			allReward.addItemInfos(guaranteeRewardList);
			guaranteeReward.addItemInfos(guaranteeRewardList);
			tenRewardList.add(guaranteeReward);
			// 9次普通奖励
			for (int i = 0; i < 9; ++i) {
				AwardItems reward = AwardItems.valueOf();
				List<ItemInfo> rewardList = summonCfg.getReward().getRewardList();
				allReward.addItemInfos(rewardList);
				reward.addItemInfos(rewardList);
				tenRewardList.add(reward);
			}
			// 乱序
			HawkRand.randomOrder(tenRewardList);

			statisticsEntity.increaseEggCoinTimes();
			statisticsEntity.increaseEggCoinTimesDaily();
			break;
		}

		case Const.SummonType.SUMMON_DIAMOND_VALUE: {
			consume.addGold(GsConst.summon.TEN_DIAMOND_PRICE);
			if (false == consume.checkConsume(player, hsCode)) {
				return true;
			}
			consume.consumeTakeAffectAndPush(player, Action.NULL, hsCode);
			// 假奖励
			SummonPseudoCfg pseudoCfg = HawkConfigManager.getInstance().getConfigByKey(SummonPseudoCfg.class, GsConst.summon.FIRST_N_TEN_PSEUDO_REWARD);
			// TODO 统计数据
			if (statisticsEntity.getEggDiamondTenTimes() <= pseudoCfg.getTimes()) {
				for (int i = 0; i < 10; ++i) {
					AwardItems reward = AwardItems.valueOf();
					List<ItemInfo> rewardList = pseudoCfg.getReward().getRewardList();
					allReward.addItemInfos(rewardList);
					reward.addItemInfos(rewardList);
					tenRewardList.add(reward);
				}
			} else {
				// 1次保底奖励
				AwardItems guaranteeReward = AwardItems.valueOf();
				List<ItemInfo> guaranteeRewardList = summonCfg.getGuaranteeReward().getRewardList();
				allReward.addItemInfos(guaranteeRewardList);
				guaranteeReward.addItemInfos(guaranteeRewardList);
				tenRewardList.add(guaranteeReward);
				// 9次普通奖励
				for (int i = 0; i < 9; ++i) {
					AwardItems reward = AwardItems.valueOf();
					List<ItemInfo> rewardList = summonCfg.getReward().getRewardList();
					allReward.addItemInfos(rewardList);
					reward.addItemInfos(rewardList);
					tenRewardList.add(reward);
				}
				// 乱序
				HawkRand.randomOrder(tenRewardList);
			}

			statisticsEntity.increaseEggDiamondTenTimes();
			break;
		}

		default:
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}

		allReward.rewardTakeAffectAndPush(player, Action.NULL, hsCode);
		statisticsEntity.notifyUpdate(true);

		HSSummonTenRet.Builder response = HSSummonTenRet.newBuilder();
		for (AwardItems reward : tenRewardList) {
			response.addReward(reward.getBuilder());
		}
		sendProtocol(HawkProtocol.valueOf(HS.code.SUMMON_TEN_S, response));
		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		return true;
	}
}
