package com.hawk.game.module;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Im.HSImChatSend;

public class PlayerImModule extends PlayerModule {

	private static final Logger logger = LoggerFactory.getLogger("Protocol");

	public PlayerImModule(Player player) {
		super(player);
	}

	/**
	 * 发送聊天
	 */
	@ProtocolHandler(code = HS.code.IM_CHAT_SEND_C_VALUE)
	private boolean onImChatSend(HawkProtocol cmd) {
		HSImChatSend protocol = cmd.parseProtocol(HSImChatSend.getDefaultInstance());
		int hsCode = cmd.getType();
		int channel = protocol.getChannel();
		String chatText = protocol.getText();

		// TODO: 
		// 禁言判断
		// 长度限制

		ImManager.getInstance().postChat(player, chatText, channel);

		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 加入世界聊天频道
		ImManager.getInstance().joinWorld(player.getId(), player.getLanguage(), player.getSession());
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// 退出世界聊天频道
		ImManager.getInstance().quitWorld(player.getId());
		return true;
	}
}
