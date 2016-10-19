package com.hawk.game.module;

import java.util.Calendar;
import java.util.List;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Guide.HSGuideFinish;
import com.hawk.game.protocol.Guide.HSGuideFinishRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.util.GsConst;

/**
 * 统计模块
 * 
 * @author walker
 */
public class PlayerStatisticsModule  extends PlayerModule {

	public PlayerStatisticsModule(Player player) {
		super(player);
	}

	/**
	 * 新手引导
	 */
	@ProtocolHandler(code = HS.code.GUIDE_FINISH_C_VALUE)
	private boolean onGuideFinsish(HawkProtocol cmd) {
		HSGuideFinish protocol = cmd.parseProtocol(HSGuideFinish.getDefaultInstance());
		List<Integer> guideIdList = protocol.getGuideIdList();

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		statisticsEntity.addGuideFinishList(guideIdList);
		statisticsEntity.notifyUpdate(true);

		HSGuideFinishRet.Builder response = HSGuideFinishRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.GUIDE_FINISH_S, response));
		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 加载统计数据
		player.getPlayerData().loadStatistics();

		// 登录时更新数据
		player.regainSkillPoint();
		player.regainPVPTime();
		player.regainFatigue();
		player.regainAdventureChangeTimes();

		// 同步统计信息
		player.getPlayerData().syncStatisticsInfo();
		return true;
	}

	@Override
	protected boolean onPlayerReconnect(HawkMsg msg) {
		// 登录时更新数据
		player.regainSkillPoint();
		player.regainPVPTime();
		player.regainFatigue();
		player.regainAdventureChangeTimes();

		// 同步统计信息
		player.getPlayerData().syncStatisticsInfo();
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		Calendar loginTime = player.getEntity().getLoginTime();
		Calendar logoutTime = player.getEntity().getLogoutTime();
		long onlineTime = logoutTime.getTimeInMillis() - loginTime.getTimeInMillis();
		statisticsEntity.increaseTotalOnlineTime(onlineTime);

		statisticsEntity.notifyUpdate(false);
		return true;
	}

	@Override
	public boolean onPlayerRefresh(List<Integer> refreshIndexList, boolean onLogin) {
		// 刷新统计数据，保证其它模块刷新时数据时间一致
		StatisticsEntity statisticsEntity = player.getPlayerData().loadStatistics();

		for (int index : refreshIndexList) {
			int mask = GsConst.Refresh.PlayerMaskArray[index];

			if (0 != (mask & GsConst.Refresh.DAILY )) {
				statisticsEntity.clearAdventureTimesDaily();
				statisticsEntity.clearAllianceContriRewardDaily();
				statisticsEntity.clearAllianceFatigueTimesDaily();
				statisticsEntity.clearAlliancePrayTimesDaily();
				statisticsEntity.clearAllianceTaskCountDaily();
				statisticsEntity.clearArenaTimesDaily();
				statisticsEntity.clearBuyCoinTimesDaily();
				statisticsEntity.clearBuyGiftTimesDaily();
				statisticsEntity.clearCoinAllianceCountDaily();
				statisticsEntity.clearEggCoin1FreeTimesDaily();
				statisticsEntity.clearEggCoin1PayTimesDaily();
				statisticsEntity.clearEggCoin10TimesDaily();
				statisticsEntity.clearEquipPunchTimesDaily();
				statisticsEntity.clearEquipStageMaxCountDaily();
				statisticsEntity.clearHireMonsterDaily();
				statisticsEntity.clearHoleTimesDaily();
				statisticsEntity.clearInstanceEnterTimesDaily();
				statisticsEntity.clearInstanceHardTimesDaily();
				statisticsEntity.clearInstanceNormalTimesDaily();
				statisticsEntity.clearInstanceResetTimesDaily();
				statisticsEntity.clearLoginTimesDaily();
				statisticsEntity.clearPayDiamondCountDaily();
				statisticsEntity.clearQuestDailyComplete();
				statisticsEntity.clearSynAllTimesDaily();
				statisticsEntity.clearUpEquipTimesDaily();
				statisticsEntity.clearUpSkillTimesDaily();
				statisticsEntity.clearUseDiamondCountDaily();
				statisticsEntity.clearUseFatigueCountDaily();
				statisticsEntity.clearUseItemCountDaily();
				statisticsEntity.setSigninDaily(false);
				statisticsEntity.notifyUpdate(true);

				if (false == onLogin) {
					player.getPlayerData().syncDailyRefreshInfo();
				}

			} else if (0 != (mask & GsConst.Refresh.MONTHLY)) {
				statisticsEntity.clearTowerFloorMap();
				statisticsEntity.clearSigninTimesMonthly();
				statisticsEntity.clearSigninFillTimesMonthly();
				statisticsEntity.notifyUpdate(true);

				if (false == onLogin) {
					player.getPlayerData().syncMonthlyRefreshInfo();
				}
			}
		}

		return true;
	}

}
