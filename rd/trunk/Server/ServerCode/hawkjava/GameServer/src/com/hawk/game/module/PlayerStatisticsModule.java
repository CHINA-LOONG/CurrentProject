package com.hawk.game.module;

import java.util.Calendar;
import java.util.List;

import org.hawk.msg.HawkMsg;

import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
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

	@Override
	protected boolean onPlayerLogin() {
		// 加载统计数据
		player.getPlayerData().loadStatistics();

		// 登录时更新数据
		player.regainSkillPoint();
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
			int mask = GsConst.PlayerRefreshMask[index];

			if (0 != (mask & GsConst.RefreshMask.DAILY )) {
				statisticsEntity.clearAdventureTimesDaily();
				statisticsEntity.clearAllianceContriRewardDaily();
				statisticsEntity.clearAllianceFatigueTimesDaily();
				statisticsEntity.clearAlliancePrayTimesDaily();
				statisticsEntity.clearAllianceTaskCountDaily();
				statisticsEntity.clearArenaTimesDaily();
				statisticsEntity.clearBuyCoinTimesDaily();
				statisticsEntity.clearBuyGiftTimesDaily();
				statisticsEntity.clearCoinAllianceCountDaily();
				statisticsEntity.clearCoinArenaCountDaily();
				statisticsEntity.clearEggCoinTimesDaily();
				statisticsEntity.clearEquipPunchTimesDaily();
				statisticsEntity.clearEquipStageMaxCountDaily();
				statisticsEntity.clearHoleTimesDaily();
				statisticsEntity.clearInstanceEnterTimesDaily();
				statisticsEntity.clearInstanceHardTimesDaily();
				statisticsEntity.clearInstanceNormalTimesDaily();
				statisticsEntity.clearInstanceResetTimesDaily();
				statisticsEntity.clearPayDiamondCountDaily();
				statisticsEntity.clearQuestDailyComplete();
				statisticsEntity.clearSynAllTimesDaily();
				statisticsEntity.clearUpEquipTimesDaily();
				statisticsEntity.clearUpSkillTimesDaily();
				statisticsEntity.clearUseDiamondCountDaily();
				statisticsEntity.clearUseFatigueCountDaily();
				statisticsEntity.clearUseItemCountDaily();
				statisticsEntity.notifyUpdate(true);

				if (false == onLogin) {
					player.getPlayerData().syncDailyRefreshInfo();
				}

			} else if (0 != (mask & GsConst.RefreshMask.TOWER)) {
				statisticsEntity.clearTowerFloorMap();
				statisticsEntity.notifyUpdate(true);
				if (false == onLogin) {
					player.getPlayerData().syncMonthlyRefreshInfo();
				}
			}
		}

		return true;
	}

}
