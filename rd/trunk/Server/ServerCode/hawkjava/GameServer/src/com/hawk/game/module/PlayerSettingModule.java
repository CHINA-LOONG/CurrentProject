package com.hawk.game.module;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.net.protocol.HawkProtocol;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.ServerData;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Setting.HSSettingBlockRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Setting.HSSettingBlock;
import com.hawk.game.protocol.Setting.HSSettingLanguage;
import com.hawk.game.util.ProtoUtil;

public class PlayerSettingModule extends PlayerModule {

	private static final Logger logger = LoggerFactory.getLogger("Protocol");

	public PlayerSettingModule(Player player) {
		super(player);
	}

	/**
	 * 设置语言
	 */
	@ProtocolHandler(code = HS.code.SETTING_LANGUAGE_C_VALUE)
	private boolean onSettingLanguage(HawkProtocol cmd) {
		HSSettingLanguage protocol = cmd.parseProtocol(HSSettingLanguage.getDefaultInstance());
		int hsCode = cmd.getType();
		String newLang = protocol.getLanguage();
		// TODO
		String oldLang = player.getLanguage();
		if (oldLang == newLang) {
			return true;
		}

		player.getEntity().setLanguage(newLang);
		ImManager.getInstance().changeLanguage(oldLang, newLang, player.getPlayerData().getPlayerAllianceEntity().getId());

		return true;
	}

	/**
	 * 屏蔽玩家
	 */
	@ProtocolHandler(code = HS.code.SETTING_BLOCK_C_VALUE)
	private boolean onSettingBlock(HawkProtocol cmd) {
		HSSettingBlock protocol = cmd.parseProtocol(HSSettingBlock.getDefaultInstance());
		int hsCode = cmd.getType();
		int targetId = protocol.getPlayerId();
		boolean isBlock = protocol.getIsBlock();

		if (false == ServerData.getInstance().isExistPlayer(targetId)) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.PlayerError.PLAYER_NOT_EXIST_VALUE, 1));
			return false;
		}

		if (isBlock == true) {
			player.getEntity().addBlockPlayer(targetId);
		} else {
			player.getEntity().removeBlockPlayer(targetId);
		}

		HSSettingBlockRet.Builder response = HSSettingBlockRet.newBuilder();
		response.setPlayerId(targetId);
		response.setIsBlock(isBlock);
		sendProtocol(HawkProtocol.valueOf(HS.code.SETTING_BLOCK_S, response));

		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 同步系统设置信息
		player.getPlayerData().syncSettingInfo();
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		return true;
	}
}
