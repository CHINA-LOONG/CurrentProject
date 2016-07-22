package com.hawk.game.service;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.config.HawkConfigStorage;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkException;
import org.hawk.util.services.HawkAccountService;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.ServerData;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.TestAccountCfg;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.HS;
import com.hawk.game.service.GameService;
import com.hawk.game.util.GsConst;

/**
 * GM服务
 * 
 * @author walker
 */
public class GmService extends GameService {

	/**
	 * 消息拦截
	 */
	@Override
	public boolean onMessage(HawkAppObj appObj, HawkMsg msg) {
		return false;
	}

	/**
	 * 协议拦截
	 */
	@Override
	public boolean onProtocol(HawkAppObj appObj, HawkProtocol protocol) {
		// 生成测试账号
		if (protocol.checkType(HS.gm.GEN_TEST_ACCOUNT)) {
			genTestAccount();
			return true;
		}

		return false;
	}

	/**
	 * 生成测试账号
	 */
	@SuppressWarnings("unchecked")
	private void genTestAccount() {
		// 解析配置-------------------------------------------------------------------------------------
		Map<String, List<TestAccountCfg>> accountMap = new HashMap<>();

		try {
			HawkConfigStorage cfgStorgae = new HawkConfigStorage(TestAccountCfg.class);
			List<TestAccountCfg> accountCfgList = (List<TestAccountCfg>) cfgStorgae.getConfigList();

			for (TestAccountCfg cfg : accountCfgList) {
				if (false == accountMap.containsKey(cfg.getPuid())) {
					accountMap.put(cfg.getPuid(), new ArrayList<TestAccountCfg>());
				}
				accountMap.get(cfg.getPuid()).add(cfg);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		// 生成账号-------------------------------------------------------------------------------------
		for (Entry<String, List<TestAccountCfg>> entry : accountMap.entrySet()) {
			String puid = entry.getKey().toLowerCase();
			// 创建player entity
			int playerId = ServerData.getInstance().getPlayerIdByPuid(puid);
			if (playerId == 0) {
				String nickname = entry.getValue().get(0).getNickname();
				short level = entry.getValue().get(0).getPlayerLevel();

				PlayerEntity playerEntity = new PlayerEntity(puid, nickname, (byte)1, 1, 1, 1, 1);
				playerEntity.setLevel(level);
				if (false == playerEntity.notifyCreate()) {
					continue;
				}

				playerId = playerEntity.getId();
				ServerData.getInstance().addNameAndPlayerId(nickname, playerId);
				ServerData.getInstance().addPuidAndPlayerId(puid, playerId);
				ServerData.getInstance().addPlayerId(playerId);

				HawkAccountService.getInstance().report(new HawkAccountService.CreateRoleData(puid, playerId, nickname));
			} else {
				// 账号已存在
				continue;
			}

			// 创建player object
			GsApp app = GsApp.getInstance();
			HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, playerId);
			HawkObjBase<HawkXID, HawkAppObj> objBase = app.lockObject(xid);
			try {
				// 对象不存在即创建
				if (objBase == null || false == objBase.isObjValid()) {
					objBase = app.createObj(xid);
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
					player.getPlayerData().loadStatistics();

					// 生成宠物
					for (TestAccountCfg cfg : entry.getValue()) {
						MonsterCfg monster = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, cfg.getMonsterId());
						if (null == monster) {
							continue;
						}

						MonsterEntity monsterEntity = new MonsterEntity(cfg.getMonsterId(), playerId,
								cfg.getStage(), cfg.getMonsterLevel(), cfg.getExp(), cfg.getLazy(), cfg.getLazyExp(), cfg.getDisposition());

						Map<String, Integer> skillMap = cfg.getSkillLevelMap();
						for (Entry<String, Integer> skill : skillMap.entrySet()) {
							monsterEntity.setSkillLevel(skill.getKey(), skill.getValue());
						}

						if (true == monsterEntity.notifyCreate()) {
							player.getPlayerData().setMonsterEntity(monsterEntity);
							player.onIncreaseMonster(monsterEntity);
						}
					}
				}
			} finally {
				if (objBase != null) {
					objBase.unlockObj();
				}
			}
		}
	}
}
