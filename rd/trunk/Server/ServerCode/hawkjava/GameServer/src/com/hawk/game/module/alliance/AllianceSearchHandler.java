package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.net.protocol.HawkProtocolHandler;

import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Alliance.HSAllianceSearch;
import com.hawk.game.protocol.Alliance.HSAllianceSearchRet;
import com.hawk.game.util.AllianceUtil;

public class AllianceSearchHandler implements HawkProtocolHandler{

	@Override
	public boolean onProtocol(HawkAppObj appObj, HawkProtocol protocol) {
		Player player = (Player) appObj;
		
		HSAllianceSearch requsest = protocol.parseProtocol(HSAllianceSearch.getDefaultInstance());
		if (requsest.getNameOrId() == null || "".equals(requsest.getNameOrId())) {
			player.sendError(protocol.getType(), Status.error.PARAMS_INVALID_VALUE);
			return true;
		}

		HSAllianceSearchRet.Builder response = HSAllianceSearchRet.newBuilder();
		boolean found = false;
		for (AllianceEntity allianceEntity : AllianceManager.getInstance().getAllianceMap().values()) {
			if (allianceEntity.getName().equals(requsest.getNameOrId())) {
				response.addResult(AllianceUtil.getSimpleAllianceInfo(allianceEntity, AllianceManager.getInstance().isPlayerApply(player.getId(), allianceEntity.getId())));
				found = true;
				break;
			}
		}
		
		if (found == false && AllianceUtil.isAllianceId(requsest.getNameOrId())) {
			int allianceId = Integer.valueOf(requsest.getNameOrId());
			AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(allianceId);
			if (allianceEntity != null) {
				response.addResult(AllianceUtil.getSimpleAllianceInfo(allianceEntity, AllianceManager.getInstance().isPlayerApply(player.getId(), allianceEntity.getId())));
			}
		}
		
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_SEARCH_S_VALUE, response));
		return true;
	}
}
