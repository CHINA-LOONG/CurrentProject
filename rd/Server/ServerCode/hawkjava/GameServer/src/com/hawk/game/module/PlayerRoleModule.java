package com.hawk.game.module;

import java.util.Map;

import javax.print.attribute.standard.Severity;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.GsConfig;
import com.hawk.game.ServerData;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.RoleEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Role.HSRoleDelete;
import com.hawk.game.protocol.Role.HSRoleDetail;
import com.hawk.game.protocol.Role.HSRoleSelectRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Role.HSRoleBrief;
import com.hawk.game.protocol.Role.HSRoleCreate;
import com.hawk.game.protocol.Role.HSRoleCreateRet;
import com.hawk.game.protocol.Role.HSRoleDeleteRet;
import com.hawk.game.protocol.Role.HSRoleSelect;
import com.hawk.game.protocol.Status.error;

public class PlayerRoleModule extends PlayerModule{

	private static final Logger logger = LoggerFactory.getLogger("Protocol");

	/**
	 * 构造
	 * 
	 * @param player
	 */
	public PlayerRoleModule(Player player) {
		super(player);
	}
	
	@ProtocolHandler(code = HS.code.ROLE_CREATE_C_VALUE)
	public boolean OnPlayerRegistRole(HawkProtocol cmd){	
		int count = (int)HawkDBManager.getInstance().count("select count(*) from RoleEntity where playerID = ?", player.getId());
		if (count >= GsConfig.getInstance().getRoleMaxSize()) {
			
			player.sendError(HS.code.ROLE_CREATE_C_VALUE, Status.roleError.ROLE_MAX_SIZE_VALUE, 1);
			return true;
		}
		
		HSRoleCreate protocol = cmd.parseProtocol(HSRoleCreate.getDefaultInstance());
		if (ServerData.getInstance().isExistName(protocol.getNickname())){
			player.sendError(HS.code.ROLE_CREATE_C_VALUE, Status.roleError.ROLE_NICKNAME_EXIST_VALUE, 1);
			return true;
		}
		
		RoleEntity roleEntity = new RoleEntity(protocol.getNickname(), protocol.getCareer(), protocol.getGender(), protocol.getEye(), protocol.getHair(), protocol.getHairColor(),count);
		if (!HawkDBManager.getInstance().create(roleEntity)) {
			logger.error("database error, create role entity fail");
			return false;
		}
		
		ServerData.getInstance().addNameAndPlayerId(protocol.getNickname(), roleEntity.getRoleID());
		
		HSRoleCreateRet.Builder response = HSRoleCreateRet.newBuilder();	
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		response.setRoleId(roleEntity.getRoleID());
		cmd.getSession().sendProtocol(HawkProtocol.valueOf(HS.code.ROLE_CREATE_S_VALUE,response));
		return true;
	}
	
	@ProtocolHandler(code = HS.code.ROLE_DELETE_C_VALUE)
	public boolean OnPlayerDeleteRole(HawkProtocol cmd){		
		HSRoleDelete protocol = cmd.parseProtocol(HSRoleDelete.getDefaultInstance());
		
		//TODO delete role
		
		HSRoleDeleteRet.Builder response = HSRoleDeleteRet.newBuilder();	
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		response.setRoleId(protocol.getRoleId());
		cmd.getSession().sendProtocol(HawkProtocol.valueOf(HS.code.ROLE_DELETE_S_VALUE,response));
		return true;
	}
	
	@ProtocolHandler(code = HS.code.ROLE_SELECT_C_VALUE)
	public boolean OnPlayerSelectRole(HawkProtocol cmd){
		HSRoleSelect protocol = cmd.parseProtocol(HSRoleSelect.getDefaultInstance());
		
		//保存之前的角色数据
		player.saveRoleData();
		RoleEntity entity =	player.getPlayerData().loadRoleAndMonster(protocol.getRoleId());
		if(entity != null){
			player.sendError(HS.code.ROLE_SELECT_C_VALUE, Status.roleError.ROLE_NOT_EXIST_VALUE, 1);
			return true;
		}
		
		HSRoleSelectRet.Builder response = HSRoleSelectRet.newBuilder();
		HSRoleDetail.Builder roleDetailData = HSRoleDetail.newBuilder();
		HSRoleBrief.Builder roleBriefData = HSRoleBrief.newBuilder();
		
		response.setStatus(error.NONE_ERROR_VALUE);
		response.setRoleData(roleDetailData);
		roleDetailData.setBriefData(roleBriefData);
		roleBriefData.setRoleId(entity.getRoleID());
		
		
		return true;
	}
	
	@Override
	protected boolean onPlayerLogout() {
		player.saveRoleData();
		return true;
	}

	
}
