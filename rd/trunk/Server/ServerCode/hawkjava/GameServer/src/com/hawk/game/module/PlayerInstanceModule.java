package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map.Entry;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkRand;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.InstanceCfg;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Instance.HSBattle;
import com.hawk.game.protocol.Instance.HSInstanceEnter;
import com.hawk.game.protocol.Instance.HSInstanceEnterRet;
import com.hawk.game.protocol.Instance.HSInstanceSettle;
import com.hawk.game.protocol.Instance.HSInstanceSettleRet;
import com.hawk.game.protocol.Status;

public class PlayerInstanceModule extends PlayerModule {

	private static final Logger logger = LoggerFactory.getLogger("Protocol");
	
	private List<HSBattle> battleList;

	public PlayerInstanceModule(Player player) {
		super(player);

		battleList = new ArrayList<HSBattle>();
	}
	
	/**
	 * 副本进入
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_ENTER_C_VALUE)
	private boolean onInstanceEnter(HawkProtocol cmd) {
		HSInstanceEnter protocol = cmd.parseProtocol(HSInstanceEnter.getDefaultInstance());
		int hsCode = cmd.getType();
		String cfgId = protocol.getCfgId();

		// TODO: 检测条件
		InstanceCfg instanceCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceCfg.class, cfgId);
		if(instanceCfg == null) {
			sendError(hsCode, Status.error.CONFIG_ERROR);
			return false;
		}
		
		// 生成对局
		if (battleList.isEmpty() == false) {
			logger.error("battle list not empty when enter instance");
			battleList.clear();
		}

		List<String> randList = new ArrayList<String>();
		for (Entry<String, Integer> entry : instanceCfg.getMonsterAmountList().entrySet()) {
			for (int i = entry.getValue(); i > 0; --i) {
				randList.add(entry.getKey());
			}
		}
		HawkRand.randomOrder(randList);
		
		Iterator<String> iter = randList.iterator();
		for (int i = 0; i < instanceCfg.getBattleAmount(); ++i) {
			HSBattle.Builder battle = HSBattle.newBuilder();
			for (int j = 0; j < instanceCfg.getBattleMonsterAmount(); ++j) {
				battle.addMonsterCfgId(iter.next());
			}
			battleList.add(battle.build());
		}
		
		// 计算奖励
		
		// 数据修改
//		PlayerEntity playerEntity = player.getPlayerData().getPlayerEntity();
//		playerEntity.notifyUpdate(true);
		int fatigueChange = 0;

		HSInstanceEnterRet.Builder response = HSInstanceEnterRet.newBuilder();
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		response.setCfgId(cfgId);
		response.addAllBattle(battleList);
		response.setFatigueChange(fatigueChange);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_ENTER_S, response));
		return true;
	}
	
	/**
	 * 副本结算
	 */
	@ProtocolHandler(code = HS.code.INSTANCE_SETTLE_C_VALUE)
	private boolean onInstanceSettle(HawkProtocol cmd) {
		HSInstanceSettle protocol = cmd.parseProtocol(HSInstanceSettle.getDefaultInstance());
		int hsCode = cmd.getType();
		List<Integer> passBattleList = protocol.getPassBattleIndexList();
		List<Integer> passBoxList = protocol.getPassBoxIndexList();
		
		// 验证
		for (Integer i : passBattleList) {
			if (i < 0 || i > battleList.size()) {
				sendError(hsCode, Status.error.PARAMS_INVALID);
				return false;
			}
			battleList.get(i);
		}
		
		// 发放奖励
		
		battleList.clear();
		
		HSInstanceSettleRet.Builder response = HSInstanceSettleRet.newBuilder();
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		sendProtocol(HawkProtocol.valueOf(HS.code.INSTANCE_SETTLE_S, response));
		return true;		
	}
}
