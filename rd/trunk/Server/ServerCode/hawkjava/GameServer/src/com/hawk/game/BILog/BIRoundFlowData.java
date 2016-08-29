package com.hawk.game.BILog;

import com.hawk.game.player.Player;

public class BIRoundFlowData extends BIData{

	public void log(Player player){
		logBegin(player, "Round_Flow");
		logEnd();
	}
}
