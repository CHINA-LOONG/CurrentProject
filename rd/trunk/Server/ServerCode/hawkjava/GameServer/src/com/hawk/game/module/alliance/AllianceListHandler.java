package com.hawk.game.module.alliance;

import java.util.LinkedHashSet;
import java.util.Set;

import org.hawk.app.HawkAppObj;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.net.protocol.HawkProtocolHandler;

import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.HSAllianceList;
import com.hawk.game.protocol.Alliance.HSAllianceListRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;

public class AllianceListHandler implements HawkProtocolHandler {
	@Override
	public boolean onProtocol(HawkAppObj appObj, HawkProtocol protocol) {
		Player player = (Player) appObj;
		Set<AllianceEntity> levelSet = new LinkedHashSet<AllianceEntity>();
		
		if (AllianceManager.getInstance().getPlayerApplyList(player.getId()) != null) {
			for (int allianceId : AllianceManager.getInstance().getPlayerApplyList(player.getId())) {
				AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(allianceId);
				if (allianceEntity != null) {
					levelSet.add(allianceEntity);
				}
			}
		}
		
		levelSet.addAll(AllianceManager.getInstance().getAllianceLevelSet());
		
		// 计算最大页码
		int maxPageNum = (levelSet.size() + GsConst.Alliance.ONE_PAGE_SIZE - 1) / GsConst.Alliance.ONE_PAGE_SIZE;
		
		HSAllianceList requsest = protocol.parseProtocol(HSAllianceList.getDefaultInstance());
		int reqPageNum = requsest.getReqPage();
		// 校正请求页码
		reqPageNum = Math.min(reqPageNum, maxPageNum);
		int formIndex = Math.max((reqPageNum-1) * GsConst.Alliance.ONE_PAGE_SIZE, 0);
		int endIndex = Math.max(reqPageNum * GsConst.Alliance.ONE_PAGE_SIZE, 0);
		if(reqPageNum == maxPageNum){
			endIndex = Math.min(reqPageNum * GsConst.Alliance.ONE_PAGE_SIZE - 1, levelSet.size());
			endIndex = Math.max(endIndex, 0);
		}
		
		HSAllianceListRet.Builder builder = HSAllianceListRet.newBuilder();
		int index = 0;
		for (AllianceEntity allianceEntity : levelSet) {
			if (index >= formIndex) {
				builder.addAllianceList(AllianceUtil.getSimpleAllianceInfo(allianceEntity, AllianceManager.getInstance().isPlayerApply(player.getId(), allianceEntity.getId())));
			}
			
			if (index == endIndex - 1) {
				break;
			}
			
			index ++;
		}
	
		builder.setTotalPage(maxPageNum);
		player.sendProtocol(HawkProtocol.valueOf(HS.code.ALLIANCE_LIST_S_VALUE, builder));
		return true;
	}
}
