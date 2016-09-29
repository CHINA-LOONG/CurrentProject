package com.hawk.game.module.alliance;

import org.hawk.app.HawkAppObj;
import org.hawk.msg.HawkMsg;
import org.hawk.msg.HawkMsgHandler;

import com.hawk.game.entity.AllianceBaseEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity.BaseMonsterInfo;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.util.AllianceUtil;

public class AllianceBaseHireHandler implements HawkMsgHandler{
	/**
	 * 消息处理器
	 * 
	 * @param appObj
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage(HawkAppObj appObj, HawkMsg msg)
	{
		Player player = (Player) msg.getParam(0);
		int monsterId = (int) msg.getParam(1);

		if(player.getAllianceId() == 0){
			return true;
		}
		
		AllianceEntity allianceEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianceEntity == null) {
			return true;
		}
		
		PlayerAllianceEntity playerAllianceEntity = allianceEntity.getMember(player.getId());
		if (playerAllianceEntity == null) {
			return true;
		}
		
		AllianceBaseEntity allianceBaseEntity = allianceEntity.getAllianceBaseEntityMap().get(monsterId);
		
		if (allianceBaseEntity != null) {		
			PlayerAllianceEntity targetAllianceEntity = allianceEntity.getMember(allianceBaseEntity.getPlayerId());
			if (targetAllianceEntity == null) {
				return true;
			}
			
			// 找到对应的怪
			for (BaseMonsterInfo baseMonsterInfo : targetAllianceEntity.getBaseMonsterInfo().values()) {
				if (baseMonsterInfo.getMonsterId() == monsterId) {
					baseMonsterInfo.addReward(AllianceUtil.getAllianceBaseConfig(allianceBaseEntity.getBp()).getCoinHireget());
					targetAllianceEntity.notifyUpdate(true);
					break;
				}
			}
		}
		
		return true;
	}
}
