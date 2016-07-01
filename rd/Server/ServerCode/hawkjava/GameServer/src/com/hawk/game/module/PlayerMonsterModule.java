package com.hawk.game.module;

import java.util.Map;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.RoleEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Monster.HSMonsterAdd;
import com.hawk.game.protocol.Monster.HSMonsterBreakRet;
import com.hawk.game.protocol.Monster.HSMonsterFeed;
import com.hawk.game.protocol.Monster.HSMonsterFeedRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Monster.HSMonsterBreak;
import com.hawk.game.protocol.Role.HSRoleCreate;
import com.hawk.game.protocol.Role.HSRoleCreateRet;
import com.hawk.game.util.ProtoUtil;

/**
 * 怪物模块
 * 
 * @author walker
 */
public class PlayerMonsterModule extends PlayerModule {
	
	private static final Logger logger = LoggerFactory.getLogger("Protocol");

	public PlayerMonsterModule(Player player) {
		super(player);
	}

	@ProtocolHandler(code = HS.code.MONSTER_BREAK_C_VALUE)
	private boolean onMonsterBreak(HawkProtocol cmd) {
		HSMonsterBreak protocol = cmd.parseProtocol(HSMonsterBreak.getDefaultInstance());
		int hsCode = cmd.getType();
		int monsterId = protocol.getMonsterId();
		
		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE, 1));
			return false;
		}
		
		// TODO: break logic
		monsterEntity.setInvalid(true);
		
		boolean succ = monsterEntity.updateSync();
		if (succ == false) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.DATA_BASE_ERROR_VALUE, 1));
			return false;
		}
		
		HSMonsterBreakRet.Builder response = HSMonsterBreakRet.newBuilder();
		response.setStatus(1);
		response.setMonsterId(monsterId);
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_BREAK_S, response));
		return true;
	}
	
	@ProtocolHandler(code = HS.code.MONSTER_FEED_C_VALUE)
	private boolean onMonsterFeed(HawkProtocol cmd) {
		HSMonsterFeed protocol = cmd.parseProtocol(HSMonsterFeed.getDefaultInstance());
		int hsCode = cmd.getType();
		int monsterId = protocol.getMonsterId();
		int foodMonsterId = protocol.getFoodMonsterId();
		
		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		MonsterEntity foodMonsterEntity = player.getPlayerData().getMonsterEntity(foodMonsterId);
		if (monsterEntity == null || foodMonsterEntity == null) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE, 1));
			return false;
		}
		
		// TODO: feed logic
		int exp = 0;
		int level = 0;
		foodMonsterEntity.setInvalid(true);
		
		boolean succ = monsterEntity.updateSync();
		if (succ == false) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.DATA_BASE_ERROR_VALUE, 1));
			return false;
		}
		
		HSMonsterFeedRet.Builder response = HSMonsterFeedRet.newBuilder();
		response.setStatus(1);
		response.setMonsterId(monsterId);
		response.setFoodMonsterId(foodMonsterId);
		response.setExp(exp);
		response.setLevel(level);
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_FEED_S, response));
		return true;
	}
	
	private boolean addMonster(int cfgId, int grade, int level, int exp, byte disposition, int reason) {
		int roleId = player.getPlayerData().getRoleEntity().getRoleID();
		int equipId = 0;
		
		MonsterEntity monsterEntity = new MonsterEntity(cfgId, roleId, grade, level, exp, disposition);
		if (!HawkDBManager.getInstance().create(monsterEntity)) {
			logger.error("database error, create monster entity fail");
			return false;
		}
		player.getPlayerData().setMonsterEntity(monsterEntity);
		
		HSMonsterAdd.Builder response = HSMonsterAdd.newBuilder();
		HSMonster.Builder monsterData = HSMonster.newBuilder();
		monsterData.setMonsterId(monsterEntity.getId());
		monsterData.setCfgId(cfgId);
		monsterData.setGrade(grade);
		monsterData.setLevel(level);
		monsterData.setExp(exp);
		monsterData.setEquipId(equipId);
		monsterData.setDisposition(disposition);
		response.setMonster(monsterData);
		response.setReason(reason);
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_ADD_S, response));
		return true;
	}
	
	@Override
	protected boolean onPlayerLogout() {
		// 重要数据下线就存储
		for (Map.Entry<Integer, MonsterEntity> entry : player.getPlayerData().getMonsterEntity().entrySet()) {
			entry.getValue().notifyUpdate(false);
		}
		return true;
	}
}
