package com.hawk.game.module;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;
import org.hawk.util.services.HawkAccountService;

import com.hawk.game.ServerData;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Login.HSSyncInfo;
import com.hawk.game.protocol.Login.HSSyncInfoRet;
import com.hawk.game.protocol.Player.HSPlayerComplete;
import com.hawk.game.protocol.Player.HSPlayerCompleteRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Status.error;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ProtoUtil;
import com.hawk.game.util.TextUtil;
/**
 * 玩家登录模块
 * 
 * @author hawk
 */
public class PlayerLoginModule extends PlayerModule {

	public PlayerLoginModule(Player player) {
		super(player);
	}

	/**
	 * 登录协议处理
	 */
	@ProtocolHandler(code = HS.code.SYNCINFO_C_VALUE)
	private boolean onPlayerLoginSync(HawkProtocol cmd) {
		HSSyncInfo protocol = cmd.parseProtocol(HSSyncInfo.getDefaultInstance());
		int hsCode = cmd.getType();
		 
		// 在线人数达到上限
		int sessionMaxSize = HawkApp.getInstance().getAppCfg().getSessionMaxSize();
		if (sessionMaxSize > 0 && ServerData.getInstance().getOnlinePlayer() >= sessionMaxSize) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.ONLINE_MAX_LIMIT_VALUE, 1));
			return true;
		}

		// 参数解析
		String device = protocol.getDeviceId().trim().toLowerCase();
		String platInfo = protocol.getPlatform().trim().toLowerCase();

		// 解析平台名称
		// phonetype#osversion#OS#platform#channel
		String[] platInfos = platInfo.split("#");
		String phoneType = "";
		String osVersion = "";
		String osName = "";
		String platName = "";
		String channel = "";
		for (int i = 0; i < platInfos.length; i++) {
			if (i == 0) {
				phoneType = platInfos[i];
			} else if (i == 1) {
				osVersion = platInfos[i];
			} else if (i == 2) {
				osName = platInfos[i];
			} else if (i == 3) {
				platName = platInfos[i];
			} else if (i == 4) {
				channel = platInfos[i];
			}
		}

		// 整理平台名字
		String platform = channel;
		if (!channel.startsWith(platName)) {
			platform = platName + "_" + channel;
		}

		// 加载玩家实体信息
		PlayerEntity playerEntity = player.getPlayerData().loadPlayer();

		// 更新玩家设备相关信息
		if (playerEntity != null) {
			if (device != null && device.length() >= 0) {
				playerEntity.setDevice(device);
			}

			if (platform != null && platform.length() >= 0) {
				playerEntity.setPlatform(platform);
			}

			if (phoneType != null && phoneType.length() >= 0) {
				playerEntity.setPhoneType(phoneType);
			}

			if (osName != null && osName.length() >= 0) {
				playerEntity.setOsName(osName);
			}

			if (osVersion != null && osVersion.length() >= 0) {
				playerEntity.setOsName(osVersion);
			}
		}

		// 玩家对象信息错误
		if (playerEntity == null || playerEntity.getId() <= 0)
		{
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.PlayerError.PLAYER_NOT_EXIST_VALUE, 1));
			return true;
		}

		if (playerEntity.getLockTime() > HawkTime.getSeconds()) {
			player.sendError(HS.code.SYNCINFO_C_VALUE, error.LOGIN_LOCK_VALUE);
			return true;
		}

		playerEntity.setLoginTime(HawkTime.getCalendar());
		playerEntity.notifyUpdate(true);

		// 回复登录完成协议
		HSSyncInfoRet.Builder response = HSSyncInfoRet.newBuilder();
		response.setStatus(error.NONE_ERROR_VALUE);
		sendProtocol(HawkProtocol.valueOf(HS.code.SYNCINFO_S, response));

		// 通知玩家其他模块玩家登录成功
		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LOGIN, player.getXid());
		if (!HawkApp.getInstance().postMsg(msg))
		{
			HawkLog.errPrintln("post player login message failed: " + playerEntity.getNickname());
		}

		return true;
	}

	/**
	 * 登录协议处理
	 */
	@ProtocolHandler(code = HS.code.PLAYER_COMPLETE_C_VALUE)
	private boolean onPlayerComplete(HawkProtocol cmd) {
		HSPlayerComplete protocol = cmd.parseProtocol(HSPlayerComplete.getDefaultInstance());
		int hsCode = cmd.getType();
		String nickname = protocol.getNickname();
		int portraitId = 0;
		if (true == protocol.hasPortraitId()) {
			portraitId = protocol.getPortraitId();
		}

		// 昵称合法性
		int error = TextUtil.checkNickname(nickname);
		if (error != Status.error.NONE_ERROR_VALUE) {
			sendError(hsCode, error);
		}

		// 昵称重复
		if (true == ServerData.getInstance().isExistName(nickname)) {
			sendError(hsCode, Status.PlayerError.NICKNAME_EXIST);
		}

		player.getEntity().setNickname(nickname);
		player.getEntity().setPortrait(portraitId);
		player.getEntity().notifyUpdate(true);

		ServerData.getInstance().addNameAndPlayerId(protocol.getNickname(), player.getId());

		HawkAccountService.getInstance().report(new HawkAccountService.RenameRoleData(player.getPuid(), player.getId(), nickname));

		HSPlayerCompleteRet.Builder response = HSPlayerCompleteRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.PLAYER_COMPLETE_S, response));
		 return true;
	}

	@Override
	protected boolean onPlayerReconnect(HawkMsg msg){
		player.getEntity().setLoginTime(HawkTime.getCalendar());
		// TODO 检测设备信息
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// 记录下线时间
		player.getEntity().setLogoutTime(HawkTime.getCalendar());
		// 重要数据下线就存储
		player.getEntity().notifyUpdate(false);

		return true;
	}


}
