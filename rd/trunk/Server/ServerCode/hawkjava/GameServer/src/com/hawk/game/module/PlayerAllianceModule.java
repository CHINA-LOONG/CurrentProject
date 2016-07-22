package com.hawk.game.module;

import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;

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
	}

	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	protected boolean onPlayerLogin() {
		// 加载公会数据
		PlayerAllianceEntity allianceEntity = player.getPlayerData().loadPlayerAlliance();

		if (allianceEntity.getAllianceId() != 0) {
		
		}
		return true;
	}
	
	protected boolean onPlayerLogout() {
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
