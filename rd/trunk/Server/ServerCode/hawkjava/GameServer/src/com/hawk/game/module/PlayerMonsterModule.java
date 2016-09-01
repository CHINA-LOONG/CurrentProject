package com.hawk.game.module;

import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.annotation.ProtocolHandler;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkRand;

import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.MonsterRarityCfg;
import com.hawk.game.config.MonsterStageCfg;
import com.hawk.game.config.SkillUpPriceCfg;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Monster.HSMonsterCompose;
import com.hawk.game.protocol.Monster.HSMonsterComposeRet;
import com.hawk.game.protocol.Monster.HSMonsterDecompose;
import com.hawk.game.protocol.Monster.HSMonsterDecomposeRet;
import com.hawk.game.protocol.Monster.HSMonsterLock;
import com.hawk.game.protocol.Monster.HSMonsterLockRet;
import com.hawk.game.protocol.Monster.HSMonsterSkillUp;
import com.hawk.game.protocol.Monster.HSMonsterSkillUpRet;
import com.hawk.game.protocol.Monster.HSMonsterStageUp;
import com.hawk.game.protocol.Monster.HSMonsterStageUpRet;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

/**
 * 怪物模块
 * 
 * @author walker
 */
public class PlayerMonsterModule extends PlayerModule {

	public PlayerMonsterModule(Player player) {
		super(player);
	}

	/**
	 * 技能升级
	 */
	@ProtocolHandler(code = HS.code.MONSTER_SKILL_UP_C_VALUE)
	private boolean onMonsterSkillUp(HawkProtocol cmd) {
		HSMonsterSkillUp protocol = cmd.parseProtocol(HSMonsterSkillUp.getDefaultInstance());
		int hsCode = cmd.getType();
		int monsterId = protocol.getMonsterId();
		String skillId = protocol.getSkillId();

		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE);
			return true;
		}

		int newSkillLevel = 1 + monsterEntity.getSkillLevel(skillId);
		// 验证技能等级
		if (newSkillLevel > monsterEntity.getLevel()) {
			sendError(hsCode, Status.monsterError.SKILL_LEVEL_LIMIT_VALUE);
			return true;
		}

		// 更新技能点
		int newSkillPoint = player.regainSkillPoint() - 1;

		// 验证点数
		if (newSkillPoint < 0) {
			sendError(hsCode, Status.monsterError.SKILL_POINT_NOT_ENOUGH);
			return true;
		}

		// 验证金币
		SkillUpPriceCfg priceCfg = HawkConfigManager.getInstance().getConfigByKey(SkillUpPriceCfg.class, newSkillLevel);
		if (priceCfg == null) {
			sendError(hsCode, Status.error.PARAMS_INVALID);
			return true;
		}
		ConsumeItems consume = ConsumeItems.valueOf();
		consume.addCoin(priceCfg.getCoin());
		if (false == consume.checkConsume(player, hsCode)) {
			return true;
		}
		
		consume.consumeTakeAffectAndPush(player, Action.MONSTER_ABILITY_LEVEL_UP, HS.code.MONSTER_SKILL_UP_C_VALUE);

		player.consumeSkillPoint(1, Action.MONSTER_ABILITY_LEVEL_UP);

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		statisticsEntity.increaseUpSkillTimes();
		statisticsEntity.increaseUpSkillTimesDaily();
		statisticsEntity.notifyUpdate(true);

		monsterEntity.setSkillLevel(skillId, newSkillLevel);
		monsterEntity.notifyUpdate(true);

		HSMonsterSkillUpRet.Builder response = HSMonsterSkillUpRet.newBuilder();
		response.setSkillPoint(newSkillPoint);
		response.setSkillPointTimeStamp((int)(statisticsEntity.getSkillPointBeginTime().getTimeInMillis() / 1000));
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_SKILL_UP_S, response));

		return true;
	}

	/**
	 * 进阶
	 */
	@ProtocolHandler(code = HS.code.MONSTER_STAGE_UP_C_VALUE)
	private boolean onMonsterStageUp(HawkProtocol cmd) {
		HSMonsterStageUp protocol = cmd.parseProtocol(HSMonsterStageUp.getDefaultInstance());
		int hsCode = cmd.getType();
		int monsterId = protocol.getMonsterId();
		List<Integer> consumeMonsterIdList = protocol.getConsumeMonsterIdList();

		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST);
			return true;
		}

		int newStage = 1 + monsterEntity.getStage();

		// 验证品级上限
		MonsterStageCfg stageCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterStageCfg.class, newStage);
		if (stageCfg == null) {
			sendError(hsCode, Status.monsterError.STAGE_LIMIT);
			return true;
		}

		// 验证等级
		if (monsterEntity.getLevel() < stageCfg.getDemandLevel()) {
			sendError(hsCode, Status.monsterError.STAGE_LEVEL_NOT_ENOUGH);
			return true;
		}

		// 验证金币和物品消耗
		ConsumeItems consume = ConsumeItems.valueOf();
		consume.addCoin(stageCfg.getDemandCoin());
		consume.addItemInfos(stageCfg.getDemandItemList());

		if (false == consume.checkConsume(player, hsCode)) {
			return true;
		}

		// 验证怪物消耗
		List<ItemInfo> demandMonsterList = new LinkedList<>();
		for (ItemInfo cfg : stageCfg.getDemandMonsterList()) {
			ItemInfo copy = cfg.clone();
			if (copy.getItemId().equals(GsConst.MONSTER_CONSUME_SELF)) {
				copy.setItemId(monsterEntity.getCfgId());
			}
			demandMonsterList.add(copy);
		}

		if (true == demandMonsterList.isEmpty()) {
			if (false == consumeMonsterIdList.isEmpty()) {
				sendError(hsCode, Status.monsterError.STAGE_CONSUME);
				return true;
			}
		} else {
			int size = consumeMonsterIdList.size();
			for (int i = 0; i < size; ++i) {
				int id = consumeMonsterIdList.get(i);
				// 去重，通用于arrayList和linkedList优化
				if (i != consumeMonsterIdList.lastIndexOf(id)) {
					sendError(hsCode, Status.error.PARAMS_INVALID_VALUE);
					return true;
				}

				MonsterEntity consumeMonsterEntity = player.getPlayerData().getMonsterEntity(id);
				if (consumeMonsterEntity == null) {
					sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE);
					return true;
				}

				// 验证怪物锁定
				if (true == consumeMonsterEntity.isLocked()) {
					sendError(hsCode, Status.monsterError.LOCK_ALREADY_VALUE);
					return true;
				}

				// 找到最大能满足的需求
				boolean valid = false;
				Iterator<ItemInfo> iter = demandMonsterList.iterator();
				while (iter.hasNext()) {
					ItemInfo demandMonster = (ItemInfo) iter.next();
					if (demandMonster.getItemId().equals(consumeMonsterEntity.getCfgId()) &&
							demandMonster.getStage() <=	consumeMonsterEntity.getStage()) {
						valid = true;
						int count = demandMonster.getCount() - 1;
						demandMonster.setCount(count);
						if (count <= 0) {
							iter.remove();
							break;
						}
					}
				}

				if (false == valid) {
					sendError(hsCode, Status.monsterError.STAGE_CONSUME);
					return true;
				}

				consume.addMonster(id, consumeMonsterEntity.getCfgId());
			}

			if (false == demandMonsterList.isEmpty()) {
				sendError(hsCode, Status.monsterError.STAGE_CONSUME);
				return true;
			}
		}

		consume.consumeTakeAffectAndPush(player, Action.MONSTER_EVOLVE, HS.code.MONSTER_STAGE_UP_C_VALUE);

		monsterEntity.setStage((byte)newStage);
		monsterEntity.notifyUpdate(true);

		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity(); 
		// newStage = oldStage+1，不需循环增加
		statisticsEntity.increaseMonsterCountOverStage(newStage);
		statisticsEntity.notifyUpdate(true);

		HSMonsterStageUpRet.Builder response = HSMonsterStageUpRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_STAGE_UP_S, response));

		return true;
	}

	/**
	 * 锁与解锁
	 */
	@ProtocolHandler(code = HS.code.MONSTER_LOCK_C_VALUE)
	private boolean onMonsterLock(HawkProtocol cmd) {
		HSMonsterLock protocol = cmd.parseProtocol(HSMonsterLock.getDefaultInstance());
		int hsCode = cmd.getType();
		int monsterId = protocol.getMonsterId();
		boolean locked = protocol.getLocked();

		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST);
			return true;
		}

		if (monsterEntity.isLocked() == locked) {
			if (locked == true) {
				sendError(hsCode, Status.monsterError.LOCK_ALREADY);
				return true;
			} else {
				sendError(hsCode, Status.monsterError.UNLOCK_ALREADY);
				return true;
			}
		}

		monsterEntity.setLocked(locked);
		monsterEntity.notifyUpdate(true);

		/// TODO
		HSMonsterLockRet.Builder response = HSMonsterLockRet.newBuilder();
		response.setMonsterId(monsterId);
		response.setLocked(locked);
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_LOCK_S, response));
		return true;
	}

	/**
	 * 分解怪物
	 */
	@ProtocolHandler(code = HS.code.MONSTER_DECOMPOSE_C_VALUE)
	private boolean onMonsterDecompose(HawkProtocol cmd) {
		HSMonsterDecompose protocol = cmd.parseProtocol(HSMonsterDecompose.getDefaultInstance());
		int hsCode = cmd.getType();

		ConsumeItems consume = new ConsumeItems();
		AwardItems award = new AwardItems();
		for(int monsterId : protocol.getMonsterIdList()){
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (monsterEntity == null) {
				sendError(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE);
				return true;
			}

			MonsterStageCfg monsterStageCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterStageCfg.class, (int)monsterEntity.getStage());
			if (monsterStageCfg == null) {
				sendError(hsCode, Status.error.CONFIG_NOT_FOUND_VALUE);
				return true;
			}

			consume.addMonster(monsterId, monsterEntity.getCfgId());
			award.addItemInfos(monsterStageCfg.getDecomposeList());
		}

		if (consume.checkConsume(player, hsCode) == false) {
			return false;
		}

		consume.consumeTakeAffectAndPush(player, Action.MONSTER_DISENCHANT, hsCode);
		award.rewardTakeAffectAndPush(player, Action.MONSTER_DISENCHANT, hsCode);

		HSMonsterDecomposeRet.Builder response = HSMonsterDecomposeRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_DECOMPOSE_S_VALUE, response));
		return true;
	}

	/**
	 * 合成怪物
	 */
	@ProtocolHandler(code = HS.code.MONSTER_COMPOSE_C_VALUE)
	private boolean onMonsterCompose(HawkProtocol cmd) {
		HSMonsterCompose protocol = cmd.parseProtocol(HSMonsterCompose.getDefaultInstance());
		int hsCode = cmd.getType();
		String cfgId = protocol.getCfgId();
		boolean useCommon = protocol.getUseCommon();

		MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, cfgId);
		if (monsterCfg == null) {
			sendError(hsCode, Status.error.CONFIG_NOT_FOUND_VALUE);
			return true;
		}

		ConsumeItems consume = ConsumeItems.valueOf();
		AwardItems award = AwardItems.valueOf();
		int fragmentCount = monsterCfg.getFragmentCount();

		if (true == useCommon) {
			MonsterRarityCfg rarityCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterRarityCfg.class, monsterCfg.getRarity());
			float ratio = rarityCfg.getCommonRatio();
			int maxCommonCount = (int)(fragmentCount *  ratio);
			int curCommonCount = 0;
			ItemEntity itemEntity = player.getPlayerData().getItemByItemId(GsConst.COMMON_FRAGMENT);
			if(itemEntity != null) {
				curCommonCount = itemEntity.getCount();
			}

			if (curCommonCount >= maxCommonCount) {
				consume.addItem(GsConst.COMMON_FRAGMENT, maxCommonCount);
				fragmentCount -= maxCommonCount;
			} else if (curCommonCount > 0) {
				consume.addItem(GsConst.COMMON_FRAGMENT, curCommonCount);
				fragmentCount -= curCommonCount;
			}
		}
		consume.addItem(monsterCfg.getFragmentId(), fragmentCount);

		if (consume.checkConsume(player, hsCode) == false) {
			return false;
		}

		award.addMonster(cfgId, 0, 1, 1, monsterCfg.getDisposition());

		consume.consumeTakeAffectAndPush(player, Action.MONSTER_SUMMON, hsCode);
		award.rewardTakeAffectAndPush(player, Action.MONSTER_SUMMON, hsCode);

		HSMonsterComposeRet.Builder response = HSMonsterComposeRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_COMPOSE_S_VALUE, response));
		return true;
	}

	@Override
	protected boolean onPlayerLogin() {
		// 加载所有怪物数据
		player.getPlayerData().loadAllMonster();
		// 同步怪物信息
		player.getPlayerData().syncMonsterInfo(0);
		return true;
	}

	@Override
	protected boolean onPlayerLogout() {
		// 重要数据下线就存储
		for (Map.Entry<Integer, MonsterEntity> entry : player.getPlayerData().getMonsterEntityMap().entrySet()) {
			MonsterEntity monsterEntity = entry.getValue();
			monsterEntity.notifyUpdate(false);
		}
		return true;
	}

	// 未使用----------------------------------------------------------------------------------------------------------------------------

//	/**
//	 * 怪物分解
//	 */
//	@ProtocolHandler(code = HS.code.MONSTER_BREAK_C_VALUE)
//	private boolean onMonsterBreak(HawkProtocol cmd) {
//		HSMonsterBreak protocol = cmd.parseProtocol(HSMonsterBreak.getDefaultInstance());
//		int hsCode = cmd.getType();
//		int monsterId = protocol.getMonsterId();
//
//		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
//		if (monsterEntity == null) {
//			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE, 1));
//			return true;
//		}
//
//		// TODO: break logic
//		monsterEntity.setInvalid(true);
//
////		boolean succ = monsterEntity.updateSync();
////		if (succ == false) {
////			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.DATA_BASE_ERROR_VALUE, 1));
////			return true;
////		}
//		monsterEntity.notifyUpdate(true);
//
//		HSMonsterBreakRet.Builder response = HSMonsterBreakRet.newBuilder();
//		response.setStatus(Status.error.NONE_ERROR_VALUE);
//		response.setMonsterId(monsterId);
//		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_BREAK_S, response));
//		return true;
//	}
//
//	/**
//	 * 怪物喂养
//	 */
//	@ProtocolHandler(code = HS.code.MONSTER_FEED_C_VALUE)
//	private boolean onMonsterFeed(HawkProtocol cmd) {
//		HSMonsterFeed protocol = cmd.parseProtocol(HSMonsterFeed.getDefaultInstance());
//		int hsCode = cmd.getType();
//		int monsterId = protocol.getMonsterId();
//		int foodMonsterId = protocol.getFoodMonsterId();
//
//		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
//		MonsterEntity foodMonsterEntity = player.getPlayerData().getMonsterEntity(foodMonsterId);
//		if (monsterEntity == null || foodMonsterEntity == null) {
//			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE, 1));
//			return true;
//		}
//
//		// TODO: feed logic
//		int exp = 0;
//		int level = 0;
//		foodMonsterEntity.setInvalid(true);
//
////		boolean succ = monsterEntity.updateSync();
////		if (succ == false) {
////			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.DATA_BASE_ERROR_VALUE, 1));
////			return true;
////		}
//		foodMonsterEntity.notifyUpdate(true);
//		monsterEntity.notifyUpdate(true);
//
//		HSMonsterFeedRet.Builder response = HSMonsterFeedRet.newBuilder();
//		response.setStatus(Status.error.NONE_ERROR_VALUE);
//		response.setMonsterId(monsterId);
//		response.setFoodMonsterId(foodMonsterId);
//		response.setExp(exp);
//		response.setLevel(level);
//		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_FEED_S, response));
//		return true;
//	}
//
//	/**
//	 * 怪物捕捉
//	 */
//	@ProtocolHandler(code = HS.code.MONSTER_CATCH_C_VALUE)
//	private boolean onMonsterCatch(HawkProtocol cmd) {
//		HSMonsterCatch protocol = cmd.parseProtocol(HSMonsterCatch.getDefaultInstance());
//		// int hsCode = cmd.getType();
//		int playerId = player.getId();
//		String cfgId = protocol.getCfgId();
//		int stage = protocol.getStage();
//		int level = protocol.getLevel();
//		int lazy = protocol.getLazy();
//		int disposition = protocol.getDisposition();
//		List<HSSkill> skillList = protocol.getSkillList();
//
//		// TODO: catch logic 
//
//		MonsterEntity monsterEntity = new MonsterEntity(cfgId, playerId, (byte)stage, (short)level, 0, (byte)lazy, 0, (byte)disposition);
//		for (HSSkill skill : skillList) {
//			monsterEntity.setSkillLevel(skill.getSkillId(), skill.getLevel());
//		}
//
//		return addMonster(RewardReason.CATCH, monsterEntity);
//	}
}
