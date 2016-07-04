package com.hawk.game.module;

import java.util.Calendar;

import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;

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
}
