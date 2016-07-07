package com.hawk.game.module;

import java.util.Calendar;
import java.util.List;

import com.hawk.game.entity.StatisticsEntity;
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
		StatisticsEntity statisticsEntity = player.getPlayerData().loadStatistics();

		statisticsEntity.addLoginCount();
		statisticsEntity.notifyUpdate(true);

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
		statisticsEntity.addTotalOnlineTime(onlineTime);

		statisticsEntity.notifyUpdate(false);
		return true;
	}

	@Override
	protected boolean onRefresh(List<Integer> refreshTypeList) {
		// 重置统计数据，保证其它模块刷新时数据时间一致
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();

		if (refreshTypeList.contains(GsConst.RefreshType.DAILY_PERS_REFRESH)) {
			statisticsEntity.clearAdventureCountDaily();
			statisticsEntity.clearArenaCountDaily();
			statisticsEntity.clearBossrushCountDaily();
			statisticsEntity.clearCoinOrderCountDaily();
			statisticsEntity.clearEquipUpCountDaily();
			statisticsEntity.clearExploreCountDaily();
			statisticsEntity.clearFatigueClaimCountDaily();
			statisticsEntity.clearHardCountDaily();
			statisticsEntity.clearInstanceCountDaily();
			statisticsEntity.clearMonsterMixCountDaily();
			statisticsEntity.clearQuestCompleteDaily();
			statisticsEntity.clearSkillUpCountDaily();
			statisticsEntity.clearTimeholeCountDaily();
			
			statisticsEntity.notifyUpdate(true);
		}
		return true;
	}
}
