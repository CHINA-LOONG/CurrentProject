package com.hawk.game.module;

import java.util.ArrayList;
import java.util.Calendar;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.annotation.MessageHandler;
import org.hawk.annotation.ProtocolHandler;
import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.Check;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import sun.awt.image.OffScreenImage;

import com.hawk.game.config.MonsterStageCfg;
import com.hawk.game.config.SkillUpPriceCfg;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const.RewardReason;
import com.hawk.game.protocol.Consume.HSConsumeInfo;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Monster.HSMonsterAdd;
import com.hawk.game.protocol.Monster.HSMonsterBreakRet;
import com.hawk.game.protocol.Monster.HSMonsterCatch;
import com.hawk.game.protocol.Monster.HSMonsterDecompose;
import com.hawk.game.protocol.Monster.HSMonsterDecomposeRet;
import com.hawk.game.protocol.Monster.HSMonsterFeed;
import com.hawk.game.protocol.Monster.HSMonsterFeedRet;
import com.hawk.game.protocol.Monster.HSMonsterLock;
import com.hawk.game.protocol.Monster.HSMonsterLockRet;
import com.hawk.game.protocol.Monster.HSMonsterSkillUp;
import com.hawk.game.protocol.Monster.HSMonsterSkillUpRet;
import com.hawk.game.protocol.Monster.HSMonsterStageUp;
import com.hawk.game.protocol.Monster.HSMonsterStageUpRet;
import com.hawk.game.protocol.Skill.HSSkill;
import com.hawk.game.protocol.Status;
import com.hawk.game.protocol.Monster.HSMonsterBreak;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;
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

	/**
	 * 怪物分解
	 */
	@ProtocolHandler(code = HS.code.MONSTER_BREAK_C_VALUE)
	private boolean onMonsterBreak(HawkProtocol cmd) {
		HSMonsterBreak protocol = cmd.parseProtocol(HSMonsterBreak.getDefaultInstance());
		int hsCode = cmd.getType();
		int monsterId = protocol.getMonsterId();

		MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
		if (monsterEntity == null) {
			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.monsterError.MONSTER_NOT_EXIST_VALUE, 1));
			return true;
		}

		// TODO: break logic
		monsterEntity.setInvalid(true);

//		boolean succ = monsterEntity.updateSync();
//		if (succ == false) {
//			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.DATA_BASE_ERROR_VALUE, 1));
//			return true;
//		}
		monsterEntity.notifyUpdate(true);

		HSMonsterBreakRet.Builder response = HSMonsterBreakRet.newBuilder();
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		response.setMonsterId(monsterId);
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_BREAK_S, response));
		return true;
	}

	/**
	 * 怪物喂养
	 */
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
			return true;
		}

		// TODO: feed logic
		int exp = 0;
		int level = 0;
		foodMonsterEntity.setInvalid(true);

//		boolean succ = monsterEntity.updateSync();
//		if (succ == false) {
//			sendProtocol(ProtoUtil.genErrorProtocol(hsCode, Status.error.DATA_BASE_ERROR_VALUE, 1));
//			return true;
//		}
		foodMonsterEntity.notifyUpdate(true);
		monsterEntity.notifyUpdate(true);

		HSMonsterFeedRet.Builder response = HSMonsterFeedRet.newBuilder();
		response.setStatus(Status.error.NONE_ERROR_VALUE);
		response.setMonsterId(monsterId);
		response.setFoodMonsterId(foodMonsterId);
		response.setExp(exp);
		response.setLevel(level);
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_FEED_S, response));
		return true;
	}

	/**
	 * 怪物捕捉
	 */
	@ProtocolHandler(code = HS.code.MONSTER_CATCH_C_VALUE)
	private boolean onMonsterCatch(HawkProtocol cmd) {
		HSMonsterCatch protocol = cmd.parseProtocol(HSMonsterCatch.getDefaultInstance());
		// int hsCode = cmd.getType();
		int playerId = player.getId();
		String cfgId = protocol.getCfgId();
		int stage = protocol.getStage();
		int level = protocol.getLevel();
		int lazy = protocol.getLazy();
		int disposition = protocol.getDisposition();
		List<HSSkill> skillList = protocol.getSkillList();

		// TODO: catch logic 

		MonsterEntity monsterEntity = new MonsterEntity(cfgId, playerId, (byte)stage, (short)level, 0, (byte)lazy, 0, (byte)disposition);
		for (HSSkill skill : skillList) {
			monsterEntity.setSkillLevel(skill.getSkillId(), skill.getLevel());
		}

		return addMonster(RewardReason.CATCH, monsterEntity);
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
		
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
		// 更新技能点
		Calendar beginTime = statisticsEntity.getSkillPointBeginTime();
		Calendar curTime = HawkTime.getCalendar();
		int delta = (int)((curTime.getTimeInMillis() - beginTime.getTimeInMillis()) / 1000);
		int curSkillPoint = statisticsEntity.getSkillPoint() + delta / GsConst.SKILL_POINT_TIME;
		if (curSkillPoint > GsConst.MAX_SKILL_POINT) {
			curSkillPoint = GsConst.MAX_SKILL_POINT;
		}
		beginTime.setTimeInMillis(curTime.getTimeInMillis() - delta % GsConst.SKILL_POINT_TIME  * 1000);

		// 验证点数
		if (curSkillPoint < 1) {
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
		consume.consumeTakeAffectAndPush(player, Action.SKILL_UP, HS.code.MONSTER_SKILL_UP_C_VALUE);

		// 更新技能点
		if (curSkillPoint == GsConst.MAX_SKILL_POINT) {
			beginTime = curTime;
		}
		curSkillPoint -= 1;

		statisticsEntity.setSkillPoint(curSkillPoint);
		statisticsEntity.setSkillPointBeginTime(beginTime);
		statisticsEntity.addSkillUpCount();
		statisticsEntity.addSkillUpCountDaily();
		statisticsEntity.notifyUpdate(true);
		
		monsterEntity.setSkillLevel(skillId, newSkillLevel);
		monsterEntity.notifyUpdate(true);

		HSMonsterSkillUpRet.Builder response = HSMonsterSkillUpRet.newBuilder();
		response.setSkillPoint(curSkillPoint);
		response.setSkillPointTimeStamp((int)(beginTime.getTimeInMillis() / 1000));
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
			List<MonsterEntity> consumeMonsterList = new ArrayList<>();
			for (int id : consumeMonsterIdList) {
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

				consumeMonsterList.add(consumeMonsterEntity);
				consume.addMonster(id, consumeMonsterEntity.getCfgId());
			}

			if (false == demandMonsterList.isEmpty()) {
				sendError(hsCode, Status.monsterError.STAGE_CONSUME);
				return true;
			}
			
			List<Integer> battleMonsterList = player.getEntity().getBattleMonsterList();
			for (int i = 0; i < consumeMonsterList.size(); ++i) {
				MonsterEntity consumeMonster = consumeMonsterList.get(i);
				battleMonsterList.remove(Integer.valueOf(consumeMonster.getId()));
			}
			player.getEntity().notifyUpdate(true);
		}

		consume.consumeTakeAffectAndPush(player, Action.STAGE_UP, HS.code.MONSTER_STAGE_UP_C_VALUE);

		monsterEntity.setStage((byte)newStage);
		monsterEntity.notifyUpdate(true);

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
		ConsumeItems consume = new ConsumeItems();
		AwardItems award = new AwardItems();
		for(int monsterId : protocol.getMonsterIdList()){
			MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity(monsterId);
			if (monsterEntity == null) {
				sendError(HS.code.MONSTER_DECOMPOSE_C_VALUE, Status.monsterError.MONSTER_NOT_EXIST_VALUE);
				return true;
			}
			
			MonsterStageCfg monsterStageCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterStageCfg.class, (int)monsterEntity.getStage());
			if (monsterStageCfg == null) {
				sendError(HS.code.MONSTER_DECOMPOSE_C_VALUE, Status.error.CONFIG_NOT_FOUND_VALUE);
				return true;
			}
			
			consume.addMonster(monsterId, monsterEntity.getCfgId());
			award.addItemInfos(monsterStageCfg.getDecomposeList());
		}
		
		if (consume.checkConsume(player, HS.code.MONSTER_DECOMPOSE_C_VALUE) == false) {
			return false;
		}
		
		consume.consumeTakeAffectAndPush(player, Action.MONSTER_DECOMPOSE, HS.code.MONSTER_DECOMPOSE_C_VALUE);
		award.rewardTakeAffectAndPush(player, Action.MONSTER_DECOMPOSE, HS.code.MONSTER_DECOMPOSE_C_VALUE);
		
		HSMonsterDecomposeRet.Builder response = HSMonsterDecomposeRet.newBuilder();
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_DECOMPOSE_S_VALUE, response));
		return true;
	}
	
	/**
	 * 奖励怪物
	 */
	@MessageHandler(code = GsConst.MsgType.PRESENT_MONSTER)
	private boolean onMonsterPresent(HawkMsg msg) {
		addMonster(RewardReason.SYS_PRESENT, (MonsterEntity)msg.getParam(0));
		return true;
	}
	
	// 内部函数--------------------------------------------------------------------------------------
	
	private boolean addMonster(RewardReason reason, MonsterEntity monsterEntity) {
		if (false == monsterEntity.notifyCreate()) {
			logger.error("database error, create monster entity fail");
			return true;
		}
		player.getPlayerData().setMonsterEntity(monsterEntity);

		HSMonsterAdd.Builder response = HSMonsterAdd.newBuilder();
		HSMonster.Builder monsterData = BuilderUtil.genMonsterBuilder(monsterEntity);
		response.setMonster(monsterData);
		response.setReason(reason);
		sendProtocol(HawkProtocol.valueOf(HS.code.MONSTER_ADD_S, response));
		
		// 统计数据
		StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity(); 
		boolean update = false;
		
		// collection
		if (false == statisticsEntity.getMonsterCollectSet().contains(monsterEntity.getCfgId())) {
			statisticsEntity.addMonsterCollect(monsterEntity.getCfgId());
			update = true;
		}

		// level
		int level = monsterEntity.getLevel();
		int history = statisticsEntity.getMonsterCountOverLevel(level);
		int cur = player.getPlayerData().getMonsterCountOverLevel(level);
		if (cur > history) {
			statisticsEntity.setMonsterCountOverLevel(level, cur);
			update = true;
		}
		
		history = statisticsEntity.getMonsterMaxLevel();
		if (level > history) {
			statisticsEntity.setMonsterMaxLevel(level);
			update = true;
		}
		
		// stage
		int stage = monsterEntity.getStage();
		history = statisticsEntity.getMonsterCountOverStage(stage);
		cur = player.getPlayerData().getMonsterCountOverStage(stage);
		if (cur > history) {
			statisticsEntity.setMonsterCountOverStage(stage, cur);
			update = true;
		}
		
		history = statisticsEntity.getMonsterMaxStage();
		if (stage > history) {
			statisticsEntity.setMonsterMaxStage(stage);
			update = true;
		}
		
		// count
		history = statisticsEntity.getMonsterMaxCount();
		cur = player.getPlayerData().getMonsterEntityMap().size();
		if (cur > history) {
			statisticsEntity.setMonsterMaxCount(cur);
			update = true;
		}
		
		if (true == update) {
			statisticsEntity.notifyUpdate(true);
			
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.STATISTICS_UPDATE, player.getXid());
			msg.pushParam(GsConst.StatisticsType.OTHER_STATISTICS);
			if (false == HawkApp.getInstance().postMsg(msg)) {
				HawkLog.errPrintln("post statistics update message failed: " + player.getName());
			}
		}

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
}
