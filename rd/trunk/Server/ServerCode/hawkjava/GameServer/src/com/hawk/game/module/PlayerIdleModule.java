package com.hawk.game.module;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.ServerData;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.log.BehaviorLogger.Source;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Player.HSAssembleFinish;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;

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
	 * 玩家上线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogin() {
		// 最后通知组装完成
		sendProtocol(HawkProtocol.valueOf(HS.code.ASSEMBLE_FINISH_S, HSAssembleFinish.newBuilder().setPlayerID(player.getPlayerData().getId())));
			
		// 通知玩家组装完成
 		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_ASSEMBLE, player.getXid());
		HawkApp.getInstance().postMsg(msg);
		
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
		
		// TEST------------------------------------------------------------------------------------------
		Map<Object, MailSysCfg> mailSysCfgMap = HawkConfigManager.getInstance().getConfigMap(MailSysCfg.class);
		if (mailSysCfgMap != null) {
			for (Entry<Object, MailSysCfg> entry : mailSysCfgMap.entrySet()) {
				MailSysCfg mailSysCfg = entry.getValue();
				List<Integer> receiverList = new ArrayList<>();
				int testerId = ServerData.getInstance().getPlayerIdByPuid("mail_test");
				if (testerId != 0 && true == ServerData.getInstance().isPlayerOnline(testerId)) {
					receiverList.add(testerId);
				}
				MailUtil.SendSysMail(mailSysCfg, receiverList, Source.SYS_OPERATION, Action.MAIL_REWARD);
			}
		}
		// TEST END-------------------------------------------------------------------------------------
		return true;
	}

	/**
	 * 玩家下线处理
	 * 
	 * @return
	 */
	@Override
	protected boolean onPlayerLogout() {
		// 移除玩家在线id
		ServerData.getInstance().removeOnlinePlayerId(player.getId());
		// 情况玩家会话
		player.setSession(null);
		// 设置组装状态
		player.setAssembleFinish(false);
		return true;
	}
}
