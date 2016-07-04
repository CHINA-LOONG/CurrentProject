package com.hawk.game.module;

import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;

public class PlayerEquipModuel extends PlayerModule{
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerEquipModuel(Player player) {
		super(player);
	}
	
	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogin() {
		player.getPlayerData().loadEquipEntities();
		player.getPlayerData().syncEquipInfo();
		return true;
	}
}
