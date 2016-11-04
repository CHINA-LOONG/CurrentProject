package com.hawk.game.module;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.app.HawkApp;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.ServerData;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.HS.sys;
import com.hawk.game.protocol.Player.HSAssembleFinish;
import com.hawk.game.protocol.SysProtocol.HSDelayTest;
import com.hawk.game.util.GsConst;

/**
 * 空闲模块, 所有模块最后操作
 * 
 * @author hawk
 */
public class PlayerIdleModule extends PlayerModule {
	/**
	 * 构造
	 * 
	 * @param player
	 */
	public PlayerIdleModule(Player player) {
		super(player);
	}

	/**
	 * 测试延迟时间
	 * @param cmd
	 * @return
	 */
	@ProtocolHandler(code = sys.DELAY_TEST_VALUE)
	protected boolean onDelayTest(HawkProtocol cmd) {
		HSDelayTest.Builder delayTest = HSDelayTest.newBuilder();
		delayTest.setTimeStamp(cmd.parseProtocol(HSDelayTest.getDefaultInstance()).getTimeStamp());
		sendProtocol(HawkProtocol.valueOf(sys.DELAY_TEST_VALUE, delayTest));
		return true;
	}
	
	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogin() {
		// 最后通知组装完成
		HSAssembleFinish.Builder response = HSAssembleFinish.newBuilder();
		response.setPlayerId(player.getId());
		response.setAllianceId(player.getAllianceId());
		response.setContribution(player.getPlayerData().getPlayerAllianceEntity().getContribution());
		sendProtocol(HawkProtocol.valueOf(HS.code.ASSEMBLE_FINISH_S, response));
		
		// 通知玩家组装完成
 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_ASSEMBLE, player.getXid());
		HawkApp.getInstance().postMsg(msg);
		
		return true;
	}

	@Override
	protected boolean onPlayerReconnect(HawkMsg msg) {
		HSAssembleFinish.Builder response = HSAssembleFinish.newBuilder();
		response.setPlayerId(player.getId());
		response.setAllianceId(player.getAllianceId());
		sendProtocol(HawkProtocol.valueOf(HS.code.ASSEMBLE_FINISH_S, response));
		return true;
	}
	
	/**
	 * 组装完成
	 */
	@Override
	protected boolean onPlayerAssemble() {
		// 组装穿戴信息
		player.getPlayerData().initMonsterDressedEquip();
		// 设置组装状态
		player.setAssembleFinish(true);
		// 添加在线信息
		ServerData.getInstance().addOnlinePlayerId(player.getId());
		
		// 通知快照管理器删除离线快照数据
		//HawkXID targetXId = HawkXID.valueOf(GsConst.ObjType.MANAGER, GsConst.ObjId.SNAPSHOT);
		//HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.ONLINE_REMOVE_OFFLINE_SNAPSHOT, targetXId);
		//msg.pushParam(player.getId());
		//GsApp.getInstance().postMsg(msg);
		
		return true;
	}

	/**
	 * 玩家下线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogout() {
		// 保存玩家数据快照
		//SnapShotManager.getInstance().cacheSnapshot(player.getId(), player.getPlayerData().getOnlinePlayerSnapshot(true));
		// 移除玩家在线id
		ServerData.getInstance().removeOnlinePlayerId(player.getId());
		// 情况玩家会话
		player.setSession(null);
		// 设置组装状态
		player.setAssembleFinish(false);
		return true;
	}
}
