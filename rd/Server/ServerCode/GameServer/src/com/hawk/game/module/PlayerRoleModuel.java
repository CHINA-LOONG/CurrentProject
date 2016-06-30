package com.hawk.game.module;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.GsConfig;
import com.hawk.game.entity.RoleEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Role.HSRoleDelete;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Role.HSRoleCreate;
import com.hawk.game.protocol.Role.HSRoleCreateRet;
import com.hawk.game.protocol.Role.HSRoleDeleteRet;

public class PlayerRoleModuel extends PlayerModule{

	private static final Logger logger = LoggerFactory.getLogger("Protocol");

	/**
	 * 构造
	 * 
	 * @param player
	 */
	public PlayerRoleModuel(Player player) {
		super(player);
	}
	
	@ProtocolHandler(code = HS.code.ROLE_CREATE_C_VALUE)
	public boolean OnPlayerRegistRole(HawkProtocol cmd){	
		int count = (int)HawkDBManager.getInstance().count("select count(*) from RoleEntity where playerID = ?", player.getId());
		if (count >= GsConfig.getInstance().getRoleMaxSize()) {
			
			player.sendError(HS.code.ROLE_CREATE_C_VALUE, Status.RoleError.ROLE_MAX_SIZE_VALUE, 1);
			return true;
		}
		
		HSRoleCreate protocol = cmd.parseProtocol(HSRoleCreate.getDefaultInstance());
		RoleEntity roleEntity = new RoleEntity(protocol.getNickname(), protocol.getCareer(), protocol.getGender(), protocol.getEye(), protocol.getHair(), protocol.getHairColor(),count);
		if (!HawkDBManager.getInstance().create(roleEntity)) {
			logger.error("database error, create role entity fail");
			return false;
		}
		
		HSRoleCreateRet.Builder response = HSRoleCreateRet.newBuilder();	
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		response.setRoleId(roleEntity.getRoleID());
		cmd.getSession().sendProtocol(HawkProtocol.valueOf(HS.code.ROLE_CREATE_S_VALUE,response));
		return true;
	}
	
	@ProtocolHandler(code = HS.code.ROLE_DELETE_C_VALUE)
	public boolean OnPlayerDeleteRole(HawkProtocol cmd){		
		HSRoleDelete protocol = cmd.parseProtocol(HSRoleDelete.getDefaultInstance());
		RoleEntity roleEntity = player.getPlayerData().loadRole(protocol.getRoleId());
		if (roleEntity == null) {
			player.sendError(HS.code.ROLE_DELETE_C_VALUE, Status.RoleError.ROLE_NOT_EXIST_VALUE, 1);
			return true;
		}
		
		if (!HawkDBManager.getInstance().delete(roleEntity)) {
			logger.error("database error, create role entity fail");
			return false;
		}
		
		HSRoleDeleteRet.Builder response = HSRoleDeleteRet.newBuilder();	
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		cmd.getSession().sendProtocol(HawkProtocol.valueOf(HS.code.ROLE_DELETE_S_VALUE,response));
		return true;
	}
	
}
