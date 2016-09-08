package com.hawk.game.BILog;

import com.hawk.game.player.Player;

public class BIMissionFlowData extends BIData{

	public void log(Player player){
		logBegin(player, "Guild_Flow");
		
		logEnd();
	}
}
