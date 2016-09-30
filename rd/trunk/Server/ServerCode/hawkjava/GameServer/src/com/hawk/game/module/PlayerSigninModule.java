package com.hawk.game.module;

import java.util.Calendar;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.config.SigninCfg;
import com.hawk.game.config.SigninFillPriceCfg;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Signin.HSSignin;
import com.hawk.game.protocol.Signin.HSSigninFill;
import com.hawk.game.protocol.Signin.HSSigninRet;
import com.hawk.game.protocol.Status;

public class PlayerSigninModule extends PlayerModule {

	public PlayerSigninModule(Player player) {
		super(player);
	}

	/**
	 * 签到
	 */
	@ProtocolHandler(code = HS.code.SIGNIN_C_VALUE)
	private boolean onSignin(HawkProtocol cmd) {
		HSSignin protocol = cmd.parseProtocol(HSSignin.getDefaultInstance());
		int hsCode = cmd.getType();
		int month = protocol.getMonth();

		// 防止签到时跨月的情况
		if (month != HawkTime.getCalendar().get(Calendar.MONTH) + 1) {
			sendError(hsCode, Status.PlayerError.SIGNIN_STOP);
			return true;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		if (true == statisticsEntity.isSigninDaily()) {
			sendError(hsCode, Status.PlayerError.SIGNIN_ALREADY);
			return true;
		}

		RewardCfg rewardCfg = SigninCfg.getReward(month, statisticsEntity.getSigninTimesMonthly() + 1);
		if (null == rewardCfg) {
			sendError(hsCode, Status.PlayerError.SIGNIN_FINISH);
			return true;
		}

		AwardItems reward = AwardItems.valueOf();
		reward.addItemInfos(rewardCfg.getRewardList());
		reward.rewardTakeAffectAndPush(player, Action.DAILY_SIGN, hsCode);

		statisticsEntity.increaseSigninTimesMonthly();
		statisticsEntity.setSigninDaily(true);
		statisticsEntity.notifyUpdate(true);

		HSSigninRet.Builder response = HSSigninRet.newBuilder();
		response.setSigninTimes(statisticsEntity.getSigninTimesMonthly());
		response.setReward(reward.getBuilder().getRewardItems(0));
		sendProtocol(HawkProtocol.valueOf(HS.code.SIGNIN_S, response));
		return true;
	}

	/**
	 * 补签
	 */
	@ProtocolHandler(code = HS.code.SIGNIN_FILL_C_VALUE)
	private boolean onSigninFill(HawkProtocol cmd) {
		HSSigninFill protocol = cmd.parseProtocol(HSSigninFill.getDefaultInstance());
		int hsCode = cmd.getType();
		int month = protocol.getMonth();

		Calendar curTime = HawkTime.getCalendar();

		// 防止签到时跨月的情况
		if (month != curTime.get(Calendar.MONTH) + 1) {
			sendError(hsCode, Status.PlayerError.SIGNIN_STOP);
			return true;
		}

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		if (false == statisticsEntity.isSigninDaily()) {
			sendError(hsCode, Status.PlayerError.SIGNIN_NOT_YET);
			return true;
		}

		if (statisticsEntity.getSigninTimesMonthly() >= curTime.get(Calendar.DAY_OF_MONTH)) {
			sendError(hsCode, Status.PlayerError.SIGNIN_FILL_ZERO);
			return true;
		}

		RewardCfg rewardCfg = SigninCfg.getReward(month, statisticsEntity.getSigninTimesMonthly() + 1);
		if (null == rewardCfg) {
			sendError(hsCode, Status.PlayerError.SIGNIN_FINISH);
			return true;
		}

		SigninFillPriceCfg priceCfg = SigninFillPriceCfg.getSingleton();
		if (null == priceCfg) {
			sendError(hsCode, Status.error.CONFIG_ERROR_VALUE);
			return true;
		}

		int fillTimes = statisticsEntity.getSigninFillTimesMonthly();
		ConsumeItems consume = ConsumeItems.valueOf();
		int mutiple = (int)Math.pow(2, fillTimes / priceCfg.getDoubleTimes());
		int goldCost = mutiple * (priceCfg.getConsume() + fillTimes * priceCfg.getConsumeAdd());
		if (goldCost > priceCfg.getCeiling()) {
			goldCost = priceCfg.getCeiling();
		}
		consume.addGold(goldCost);

		if (false == consume.checkConsume(player, hsCode)) {
			return true;
		}

		consume.consumeTakeAffectAndPush(player, Action.DAILY_SIGN, hsCode);

		AwardItems reward = AwardItems.valueOf();
		reward.addItemInfos(rewardCfg.getRewardList());
		reward.rewardTakeAffectAndPush(player, Action.DAILY_SIGN, hsCode);

		statisticsEntity.increaseSigninTimesMonthly();
		statisticsEntity.increaseSigninFillTimesMonthly();
		statisticsEntity.notifyUpdate(true);

		HSSigninRet.Builder response = HSSigninRet.newBuilder();
		response.setSigninTimes(statisticsEntity.getSigninTimesMonthly());
		response.setReward(reward.getBuilder().getRewardItems(0));
		sendProtocol(HawkProtocol.valueOf(HS.code.SIGNIN_S, response));
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
