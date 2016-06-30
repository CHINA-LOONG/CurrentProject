package com.hawk.game.module;

import java.util.Map;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.db.HawkDBManager;
import org.hawk.net.protocol.HawkProtocol;

import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Monster.HSMonsterBreak;
import com.hawk.game.util.ProtoUtil;

/**
 * 怪物模块
 * 
 * @author walker
 */
public class PlayerMonsterModule extends PlayerModule {

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
		
		boolean succ = monsterEntity.updateSync();
		if (succ == false) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.DATA_BASE_ERROR_VALUE, 1));
			return false;
		}
		
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
