package com.hawk.game.BILog;

import com.hawk.game.player.Player;

public class BITowerCoinData extends BIData{

	public void  log(Player player){
		logBegin(player, "tc_transaction");
		logEnd();
	}
}
