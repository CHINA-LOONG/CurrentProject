package com.hawk.game.BILog;

import net.sf.json.JSONObject;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
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
	public void logOperation(Player player, Action action, AllianceEntity allianceEntity, int contributionIn, int contributionOut, int techType){
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

			jsonPropertyData.put("guild_skill", skillObject);
		}

		logEnd();
	}

	/**
	 * 
	 * @param player
	 * @param action
	 * @param allianceEntity
	 * @param teamEntity
	 * @param contributionIn
	 * @param contributionOut
	 */
	public void logTask(Player player, Action action, AllianceEntity allianceEntity, AllianceTeamEntity teamEntity, int subTaskId, int contributionIn, int contributionOut){

		logBegin(player, "Guild_Flow");
		jsonPropertyData.put("guild_id", allianceEntity.getId());
		jsonPropertyData.put("guild_name", allianceEntity.getName());
		jsonPropertyData.put("guildcontribution_in", contributionIn);
		jsonPropertyData.put("guildcontribution_out", contributionOut);
		jsonPropertyData.put("guildcontribution_bal", allianceEntity.getContribution());
		jsonPropertyData.put("guild_level", allianceEntity.getLevel());

		JSONObject taskObject = new JSONObject();
		taskObject.put("task_id", teamEntity.getTaskId());
		taskObject.put("subtask_id", "");
		taskObject.put("subtask_complete", "");
		taskObject.put("giveup_time", teamEntity.getCreateTime() + teamEntity.getOverTime());

		if (action == Action.GUILD_TASK_OPEN) {
			StringBuilder questList = new StringBuilder();
			questList.append(teamEntity.getCoinQuest1()).append(",").append(teamEntity.getCoinQuest2()).append(",");
			questList.append(teamEntity.getItemQuest1()).append(",").append(teamEntity.getItemQuest2()).append(",").append(teamEntity.getItemQuest3()).append(",");
			questList.append(teamEntity.getInstanceQuest1());
			taskObject.put("subtask_id", questList.toString());
		}
		else if (action == Action.GUILD_SUB_MISSION) {
			taskObject.put("subtask_complete", subTaskId);
		}

		jsonPropertyData.put("guild_task", taskObject);

		logEnd();
	}
}
