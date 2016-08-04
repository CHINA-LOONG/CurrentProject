package com.hawk.game.module;

import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.manager.ImManager;
import com.hawk.game.module.alliance.AllianceCreateHandler;
import com.hawk.game.module.alliance.AllianceListHandler;
import com.hawk.game.module.alliance.AllianceSearchHandler;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;

/**
 * 公会模块
 * 
 * @author zs
 */
public class PlayerAllianceModule extends PlayerModule {
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerAllianceModule(Player player) {
		super(player);
		listenProto(HS.code.ALLIANCE_CREATE_C, new AllianceCreateHandler());
		listenProto(HS.code.ALLIANCE_LIST_C, new AllianceListHandler());
		listenProto(HS.code.ALLIANCE_SEARCH_C, new AllianceSearchHandler());
	}

	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	protected boolean onPlayerLogin() {
		// 加载公会数据
		PlayerAllianceEntity allianceEntity = player.getPlayerData().loadPlayerAlliance();
		int allianceId = allianceEntity.getAllianceId();
		if (allianceId != 0) {
			ImManager.getInstance().joinGuild(allianceId, player);
			
		}
		return true;
	}
	
	protected boolean onPlayerLogout() {
		int allianceId = player.getPlayerData().getPlayerAllianceEntity().getAllianceId();
		if (allianceId != 0) {
			ImManager.getInstance().quitGuild(allianceId, player.getId());
		}
		return true;
	}


	/**
	 * 更新
	 * 
	 * @return
	 */
	@Override
	public boolean onTick() {
		return super.onTick();
	}

	/**
	 * 消息响应
	 * 
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage(HawkMsg msg) {
		return super.onMessage(msg);
	}

	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol) {
		return super.onProtocol(protocol);
	}
}
