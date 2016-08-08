package com.hawk.game.module;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.app.HawkAppObj;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkException;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.ServerData;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Im.HSImChatSend;
import com.hawk.game.protocol.Im.HSImPlayer;
import com.hawk.game.protocol.Im.HSImPlayerGet;
import com.hawk.game.protocol.Im.HSImPlayerGetRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

public class PlayerImModule extends PlayerModule {

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
		// 屏蔽
		// 长度限制
		if (chatText.length() > GsConst.MAX_IM_CHAT_LENGTH) {
			sendError(hsCode, Status.imError.IM_CHAT_LENGTH_VALUE);
			return true;
		}

		if (channel == Const.ImChannel.GUILD_VALUE) {
			if (0 == player.getPlayerData().getPlayerAllianceEntity().getAllianceId()) {
				sendError(hsCode, Status.allianceError.ALLIANCE_NOT_JOIN);
				return true;
			}
		}

		ImManager.getInstance().postChat(player, chatText, channel);

		return true;
	}

	/**
	 * 获取其他玩家信息
	 */
	@ProtocolHandler(code = HS.code.IM_PLAYER_GET_C_VALUE)
	private boolean onImPlayerGet(HawkProtocol cmd) {
		HSImPlayerGet protocol = cmd.parseProtocol(HSImPlayerGet.getDefaultInstance());
		int hsCode = cmd.getType();
		int playerId = protocol.getPlayerId();

		String puid = ServerData.getInstance().getPuidByPlayerId(playerId);
		if (null == puid) {
			sendError(hsCode, Status.PlayerError.PLAYER_NOT_EXIST);
		}

		HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, playerId);
		HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
		try {
			// 对象不存在即创建
			if (objBase == null || false == objBase.isObjValid()) {
				objBase = GsApp.getInstance().createObj(xid);
				if (objBase != null) {
					objBase.lockObj();
				}
			}

			if (objBase != null) {
				Player player = (Player) objBase.getImpl();

				// 设置玩家puid
				player.getPlayerData().setPuid(puid);

				// 加载数据
				player.getPlayerData().loadPlayer();
				player.getPlayerData().loadPlayerAlliance();

				HSImPlayer.Builder imPlayer = HSImPlayer.newBuilder();
				imPlayer.setPlayerId(playerId);
				imPlayer.setNickname(player.getName());
				imPlayer.setLevel(player.getLevel());

				int a = player.getAllianceId();
				AllianceEntity alliance = AllianceManager.getInstance().getAlliance(a);
				if (alliance != null) {
					imPlayer.setGuildId(alliance.getId());
					imPlayer.setGuildName(alliance.getName());
				}

				HSImPlayerGetRet.Builder response = HSImPlayerGetRet.newBuilder();
				response.setImPlayer(imPlayer);
				sendProtocol(HawkProtocol.valueOf(HS.code.IM_PLAYER_GET_S, response));
			} else {
				sendError(hsCode, Status.error.SERVER_ERROR);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			sendError(hsCode, Status.error.SERVER_ERROR);
		} finally {
			if (objBase != null) {
				objBase.unlockObj();
			} 
		}

		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 加入世界聊天频道
		ImManager.getInstance().joinWorld(player);
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// 退出世界聊天频道
		ImManager.getInstance().quitWorld(player.getId());
		return true;
	}
}
