package com.hawk.game.module;

import java.util.List;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.ServerData;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Setting.HSSettingBlock;
import com.hawk.game.protocol.Setting.HSSettingBlockRet;
import com.hawk.game.protocol.Setting.HSSettingLanguage;
import com.hawk.game.protocol.Setting.HSSettingLanguageRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

public class PlayerSettingModule extends PlayerModule {

	public PlayerSettingModule(Player player) {
		super(player);
	}

	/**
	 * 设置语言
	 */
	@ProtocolHandler(code = HS.code.SETTING_LANGUAGE_C_VALUE)
	private boolean onSettingLanguage(HawkProtocol cmd) {
		HSSettingLanguage protocol = cmd.parseProtocol(HSSettingLanguage.getDefaultInstance());

		String newLang = protocol.getLanguage();
		// TODO 检测合法
		String oldLang = player.getLanguage();
		if (oldLang == newLang) {
			return true;
		}

		player.getEntity().setLanguage(newLang);
		player.getEntity().notifyUpdate(true);
		ImManager.getInstance().changeLanguage(oldLang, newLang, player.getPlayerData().getPlayerAllianceEntity().getId());

		HSSettingLanguageRet.Builder response = HSSettingLanguageRet.newBuilder();
		response.setLanguage(newLang);
		sendProtocol(HawkProtocol.valueOf(HS.code.SETTING_LANGUAGE_S, response));

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

		if (targetId == player.getId()) {
			sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
			return true;
		}

		if (false == ServerData.getInstance().isExistPlayer(targetId)) {
			sendError(hsCode, Status.PlayerError.PLAYER_NOT_EXIST_VALUE);
			return true;
		}

		List<Integer> blockList = player.getEntity().getBlockPlayerList();
		boolean alreadyBlock = blockList.contains(Integer.valueOf(targetId));
		if (true == isBlock && false == alreadyBlock) {
			if (blockList.size() >= GsConst.MAX_BLOCK_COUNT) {
				sendError(hsCode, Status.settingError.SETTING_BLOCK_FULL_VALUE);
				return true;
			}
			player.getEntity().addBlockPlayer(targetId);
			player.getEntity().notifyUpdate(true);
		} else if (false == isBlock && true == alreadyBlock){
			player.getEntity().removeBlockPlayer(targetId);
			player.getEntity().notifyUpdate(true);
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
