package com.hawk.game.module;

import org.hawk.app.HawkApp;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.HawkSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.ServerData;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Login.HSSyncInfo;
import com.hawk.game.protocol.Login.HSSyncInfoRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Status.error;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.ProtoUtil;

/**
 * 玩家登陆模块
 * 
 * @author hawk
 */
public class PlayerLoginModule extends PlayerModule {
	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerLoginModule(Player player) {
		super(player);
		listenProto(HS.code.SYNCINFO_C_VALUE);
	}

	/**
	 * 协议响应
	 * 
	 * @param protocol
	 * @return
	 */
	@Override
	public boolean onProtocol(HawkProtocol protocol)
	{
		if (protocol.checkType(HS.code.SYNCINFO_C_VALUE))
		{
			// 处理本会话的玩家登陆协议
			onPlayerLogin(protocol.getSession(), protocol.getType(), protocol.parseProtocol(HSSyncInfo.getDefaultInstance()));
			return true;
		}
		return super.onProtocol(protocol);
	}

	/**
	 * 登陆协议处理
	 * 
	 * @param session
	 * @param protocol
	 */
	private boolean onPlayerLogin(HawkSession session, int hsCode, HSSyncInfo protocol)
	{
		// 在线人数达到上限
		int sessionMaxSize = HawkApp.getInstance().getAppCfg().getSessionMaxSize();
		if (sessionMaxSize > 0 && ServerData.getInstance().getOnlinePlayer() >= sessionMaxSize) {
			session.sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.ONLINE_MAX_LIMIT_VALUE, 1));
			return false;
		}

		// 参数解析
		String device = protocol.getDeviceId().trim().toLowerCase();
		String platInfo = protocol.getPlatform().trim().toLowerCase();

		// 解析平台名称
		// phonetype#osversion#OS#platform#channel#id#mac#rmac&rip&route
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

		// 机器信息
		String phoneInfo = osName + "#" + osVersion + "#" + phoneType;
		// 整理平台名字
		String platform = channel;
		if (!channel.startsWith(platName)) {
			platform = platName + "_" + channel;
		}

		// 加载玩家实体信息
		PlayerEntity playerEntity = player.getPlayerData().loadPlayer();

		// 更新玩家设备相关信息
		if (playerEntity != null) {
			boolean needUpdate = false;
			if (playerEntity.getDevice() == null || playerEntity.getDevice().length() <= 0) {
				playerEntity.setDevice(device);
				needUpdate = true;
			}

			if (playerEntity.getPlatform() == null || playerEntity.getPlatform().length() <= 0) {
				playerEntity.setPlatform(platform);
				needUpdate = true;
			}

			if (playerEntity.getPhoneInfo() == null || playerEntity.getPhoneInfo().length() <= 0) {
				playerEntity.setPhoneInfo(phoneInfo);
				needUpdate = true;
			}

			// 回写设备信息
			if (needUpdate) {
				playerEntity.notifyUpdate(true);
			}
		}

		// 玩家对象信息错误
		if (playerEntity == null || playerEntity.getId() <= 0)
		{
			session.sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.PlayerError.PLAYER_NOT_EXIST_VALUE, 1));
			return false;
		}

		// 记录上线时间
		playerEntity.setLoginTime(HawkTime.getCalendar());

		// 登陆回复协议
		HSSyncInfoRet.Builder response = HSSyncInfoRet.newBuilder();
		response.setStatus(error.NONE_ERROR_VALUE);
		// 设置时间戳
		response.setTimeStamp(HawkTime.getSeconds());
		
		// 绑定会话
		player.setSession(session);
		playerEntity.setLoginTime(HawkTime.getCalendar());
		playerEntity.notifyUpdate(true);

		// 发送登陆成功协议
		sendProtocol(HawkProtocol.valueOf(HS.code.SYNCINFO_S, response));

		// 跨天重置
		if (null == player.getEntity().getResetTime() || false == HawkTime.isToday(player.getEntity().getResetTime().getTime())) {
			player.onFirstLoginDaily(false);
		}

		// 同步玩家信息
		player.getPlayerData().syncPlayerInfo();

		// 通知玩家其他模块玩家登陆成功
		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.PLAYER_LOGIN, player.getXid());
		if (!HawkApp.getInstance().postMsg(msg))
		{
			HawkLog.errPrintln("post player login message failed: " + playerEntity.getNickname());
		}

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
