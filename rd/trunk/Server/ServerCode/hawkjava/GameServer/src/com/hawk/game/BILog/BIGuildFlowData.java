package com.hawk.game.BILog;

import net.sf.json.JSONObject;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.player.Player;
import com.hawk.game.util.GsConst;

public class BIGuildFlowData extends BIData{
	/**
	 * 
	 * @param player
	 * @param action
	 * @param allianceEntity
	 * @param contributionIn
	 * @param contributionOut
	 * @param techType
	 */
	public void log(Player player, Action action, AllianceEntity allianceEntity, int contributionIn, int contributionOut, int techType){
		logBegin(player, "Guild_Flow");
		jsonPropertyData.put("guild_id", allianceEntity.getId());
		jsonPropertyData.put("guild_name", allianceEntity.getName());
		jsonPropertyData.put("guildcontribution_in", contributionIn);
		jsonPropertyData.put("guildcontribution_out", contributionOut);
		jsonPropertyData.put("guildcontribution_bal", allianceEntity.getContribution());
		jsonPropertyData.put("guild_level", allianceEntity.getLevel());	
		if (action == Action.GUILD_TECH_LEVELUP) {
			JSONObject skillObject = new JSONObject();
			skillObject.put("skill_id", techType);
			switch (techType) {
			case GsConst.Alliance.ALLIANCE_TEC_LEVEL:
				skillObject.put("skill_name", "Guild Lvl");
				skillObject.put("skill_level", allianceEntity.getLevel() - 1);
				skillObject.put("after_skill_level", allianceEntity.getLevel());
				break;
			case GsConst.Alliance.ALLIANCE_TEC_MEMBER:
				skillObject.put("skill_name", "Member Count");
				skillObject.put("skill_level", allianceEntity.getMemLevel() - 1);
				skillObject.put("after_skill_level", allianceEntity.getMemLevel());
				break;
			case GsConst.Alliance.ALLIANCE_TEC_EXP:
				skillObject.put("skill_name", "Palace of Midas Gains");
				skillObject.put("skill_level", allianceEntity.getExpLevel() - 1);
				skillObject.put("after_skill_level", allianceEntity.getExpLevel());
				break;
			case GsConst.Alliance.ALLIANCE_TEC_COIN:
				skillObject.put("skill_name", "Fields of Elysium Gains");
				skillObject.put("skill_level", allianceEntity.getCoinLevel() - 1);
				skillObject.put("after_skill_level", allianceEntity.getCoinLevel());
				break;
			default:
				break;
			}
		}
		
		logEnd();
	}
}
