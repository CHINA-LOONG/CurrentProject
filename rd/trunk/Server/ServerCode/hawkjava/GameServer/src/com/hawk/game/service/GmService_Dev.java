package com.hawk.game.service;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.hawk.app.HawkApp;
import org.hawk.app.HawkAppObj;
import org.hawk.app.HawkObjModule;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;

import com.hawk.game.ServerData;
import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.config.MonsterBaseCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.MonsterStageCfg;
import com.hawk.game.config.PlayerAttrCfg;
import com.hawk.game.config.QuestCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.manager.ImManager.ImMsg;
import com.hawk.game.module.PlayerQuestModule;
import com.hawk.game.player.Player;
import com.hawk.game.player.PlayerModule;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.GM.GMInstancePush;
import com.hawk.game.protocol.GM.GMOperation;
import com.hawk.game.protocol.GM.GMOperationRet;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.HS.gm;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.Statistics.ChapterState;
import com.hawk.game.protocol.Statistics.InstanceState;
import com.hawk.game.protocol.Status.error;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.QuestUtil;

/**
 * GM服务开发版
 * 上线时合并此文件到GameService.GmService
 * 
 * @author walker
 */
public class GmService_Dev extends GameService {

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
	 if (protocol.checkType(HS.gm.GMOPERATION_C_VALUE)) {
			gmOperation(appObj, protocol);
			return true;
		}
		return false;
	}

	/**
	 * gm 命令
	 */
	private void gmOperation(HawkAppObj appObj, HawkProtocol cmd) {
		Player player = (Player) appObj;
		GMOperation protocol = cmd.parseProtocol(GMOperation.getDefaultInstance());
		ConsumeItems consume = new ConsumeItems();
		AwardItems award = new AwardItems();
		boolean actionHandled = false;

		String gmAction = protocol.getAction();
		String gmOperation = protocol.getOperation();
		long gmTargetId = protocol.getTargetId();
		long gmValue = protocol.getValue();
		String gmItemId = protocol.getItemId();

		switch (gmAction) {
		//修改人物等级
		case "lv": {
			Map<Object, PlayerAttrCfg> playAttrCfg = HawkConfigManager.getInstance().getConfigMap(PlayerAttrCfg.class);
			if (gmOperation.equals("=")) {
				if (gmValue <= 0 || gmValue > playAttrCfg.size()) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				award.addAttr(Const.changeType.CHANGE_PLAYER_LEVEL_VALUE, (int)gmValue);
				actionHandled = true;
			}
			break;
		}
		//修改宠物等级
		case "petlv": {
			Map<Object, MonsterBaseCfg> monsterBaseCfg = HawkConfigManager.getInstance().getConfigMap(MonsterBaseCfg.class);
			if (gmOperation.equals("=")) {
				if (gmValue <= 0 || gmValue > monsterBaseCfg.size() || player.getPlayerData().getMonsterEntity((int)gmTargetId) == null) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				award.addMonsterAttr(Const.changeType.CHANGE_MONSTER_LEVEL_VALUE, (int)gmValue, (int)gmTargetId);
				actionHandled = true;
			}
			break;
		}
		//修改人物经验
		case "exp": {
			if (gmOperation.equals("+")){
				if (gmValue <= 0) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				award.addAttr(Const.changeType.CHANGE_PLAYER_EXP_VALUE, (int)gmValue);
				actionHandled = true;
			}
			break;
		}
		//修改宠物经验
		case "petexp": {
			if (gmOperation.equals("+")){
				if (gmValue <= 0 || player.getPlayerData().getMonsterEntity((int)gmTargetId) == null) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				award.addMonsterAttr(Const.changeType.CHANGE_MONSTER_EXP_VALUE, (int)gmValue, (int)gmTargetId);
				actionHandled = true;
			}
			break;
		}
		//修改金币
		case "coin": {
			long coinChange = gmValue;
			if (gmOperation.equals("=")) {
				coinChange = gmValue - player.getCoin();
				if (coinChange > 0) {
					award.addCoin((int)coinChange);
				}
				else if (coinChange < 0) {
					consume.addCoin((int)(0 - coinChange));
				}
				actionHandled = true;
			}
			break;
		}
		//修改钻石
		case "gold": {
			long goldChange = gmValue;
			if (gmOperation.equals("=")) {
				goldChange = gmValue - player.getGold();
				if (goldChange > 0) {
					award.addFreeGold((int)goldChange);
				}
				else if (goldChange < 0) {
					consume.addGold((int)(0 - goldChange));
				}
				actionHandled = true;
			}
			break;
		}
		//修改通天塔币
		case "towercoin": {
			long towercoinChange = gmValue;
			if (gmOperation.equals("=")) {
				towercoinChange = gmValue - player.getTowerCoin();
				if (towercoinChange > 0) {
					award.addTowerCoin((int)towercoinChange);
				}
				else if (towercoinChange < 0) {
					consume.addTowerCoin((int)(0 - towercoinChange));
				}
				actionHandled = true;
			}
			break;
		}
		//修改活力值
		case "tili": {
			if (gmOperation.equals("=")) {
				int tiliChange = (int)gmValue - player.getPlayerData().getStatisticsEntity().getFatigue();
				if (tiliChange > 0) {
					award.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, tiliChange);
				}
				else if (tiliChange < 0) {
					consume.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, 0 - tiliChange);
				}

				actionHandled = true;
			}
			break;
		}
		//修改宠物品质
		case "grade": {
			if (gmOperation.equals("=")) {
				// 验证品级上限
				MonsterStageCfg stageCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterStageCfg.class, (int)gmValue);
				MonsterEntity monsterEntity = player.getPlayerData().getMonsterEntity((int)gmTargetId);
				if (stageCfg == null || monsterEntity == null || monsterEntity.getLevel() < stageCfg.getDemandLevel()) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				monsterEntity.setStage((byte)gmValue);
				monsterEntity.notifyUpdate(true);
				actionHandled = true;
			}
			break;
		}
		//恢复技能点数到满
		case "resp": {
			player.getPlayerData().getStatisticsEntity().setSkillPoint(GsConst.MAX_SKILL_POINT);
			player.getPlayerData().getStatisticsEntity().setSkillPointBeginTime(HawkTime.getCalendar());
			player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
			actionHandled = true;
			break;
		}
		//获得宠物
		case "pet": {
			if (gmOperation.equals("+")) {
				MonsterCfg monsterCfg = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, gmItemId);
				if (gmValue <= 0 || monsterCfg == null) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				award.addMonster(gmItemId, (int)gmValue, 0, 1, 1, 1);
				actionHandled = true;
			}
			break;
		}
		//获取道具
		case "item": {
			// 道具
			if (gmItemId != null && !gmItemId.equals("") && gmValue != 0 && gmTargetId == 0) {
				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, gmItemId);
				if (itemCfg == null || gmValue <= 0) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				if (gmOperation.equals("+")) {
					if (itemCfg.getType() == Const.toolType.EQUIPTOOL_VALUE) {
						award.addEquip(gmItemId, (int)gmValue, 1, 1);
					}
					else
					{
						award.addItem(gmItemId, (int)gmValue);
					}
					actionHandled = true;
				}
				else if (gmOperation.equals("-")) {
					if (itemCfg.getType() != Const.toolType.EQUIPTOOL_VALUE) {
						consume.addItem(gmItemId, (int)gmValue);
						actionHandled = true;
					}
				}
			}
			// 装备
			else if (gmTargetId != 0 && (gmItemId == null || gmItemId.equals("")) && gmValue == 0) {
				if (player.getPlayerData().getEquipById(gmTargetId) == null ) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, player.getPlayerData().getEquipById(gmTargetId).getItemId());
				if (itemCfg == null || itemCfg.getType() != Const.toolType.EQUIPTOOL_VALUE) {
					player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
					return;
				}

				if (gmOperation.equals("-")) {
					consume.addEquip(gmTargetId, itemCfg.getId());
					actionHandled = true;
				}
			}
			break;
		}
		//清理背包
		case "clearbag": {
			for (ItemEntity itemEntity : player.getPlayerData().getItemEntityMap().values()) {
				if (itemEntity.isInvalid() == false && itemEntity.getCount() != 0) {
					consume.addItem(itemEntity.getItemId(), itemEntity.getCount());
				}
			}

			for (EquipEntity equipEntity : player.getPlayerData().getEquipEntityMap().values()) {
				if (equipEntity.isInvalid() == false && equipEntity.getMonsterId() == GsConst.EQUIP_NOT_DRESS) {
					consume.addEquip(equipEntity.getId(), equipEntity.getItemId());
				}
			}

			actionHandled = true;
			break;
		}
		//刷新商店
		case "reshop": {
			HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.REFRESH_SHOP);
			msg.pushParam((int)gmValue);
			if (false == HawkApp.getInstance().postMsg(player.getXid(), msg)) {
				HawkLog.errPrintln("post refresh message failed");
				player.sendError(gm.GMOPERATION_C_VALUE, error.SERVER_ERROR_VALUE);
				return;
			}

			actionHandled = true;
			break;
		}
		// 发送走马灯和系统公告
		case "sys": {
			for (int i = 0; i < gmValue; ++i) {
				ImMsg lanternMsg = ImManager.getInstance().new ImMsg();
				lanternMsg.type = Const.ImType.LANTERN_VALUE;
				lanternMsg.channel = Const.ImChannel.WORLD_VALUE;
				lanternMsg.senderId = 0;
				lanternMsg.senderName = "";
				lanternMsg.origLang = player.getLanguage();
				lanternMsg.origText = gmOperation;
				lanternMsg.transText = null;
				lanternMsg.expansion = null;
				ImManager.getInstance().post(lanternMsg);

				ImMsg noticeMsg = ImManager.getInstance().new ImMsg();
				noticeMsg.type = Const.ImType.NOTICE_VALUE;
				noticeMsg.channel = Const.ImChannel.WORLD_VALUE;
				noticeMsg.senderId = 0;
				noticeMsg.senderName = "";
				noticeMsg.origLang = player.getLanguage();
				noticeMsg.origText = gmOperation;
				noticeMsg.transText = null;
				noticeMsg.expansion = null;
				ImManager.getInstance().post(noticeMsg);
			}
			actionHandled = true;
			break;
		}
		// 给指定玩家发送邮件
		case "mail": {
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, Integer.parseInt(gmItemId));
			if (mailCfg == null) {
				player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
				return;
			}

			int receiverId = (int)gmTargetId;
			for (int i = 0; i < gmValue; ++i) {
				MailUtil.SendSysMail(mailCfg, receiverId);
			}
			actionHandled = true;
			break;
		}
		// 给所有玩家发送邮件
		case "mailall": {
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, Integer.parseInt(gmItemId));
			if (mailCfg == null) {
				player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
				return;
			}

			List<Integer> receiverList = new ArrayList<>();
			for(Integer playerId : ServerData.getInstance().getAllPlayerIdSet()) {
				receiverList.add(playerId);
			}

			MailUtil.SendSysMail(mailCfg, receiverList);
			actionHandled = true;
			break;
		}
		// 设置任务是否完成过
		case "questset": {
			QuestCfg quest = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, (int)gmTargetId);
			if (quest == null || (gmValue != 0 && gmValue != 1)) {
				player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
				return;
			}

			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			if (gmValue == 1) {
				statisticsEntity.addQuestComplete((int)gmTargetId);
			} else if (gmValue == 0) {
				statisticsEntity.removeQuestComplete((int)gmTargetId);
			}
			statisticsEntity.notifyUpdate(true);

			// 重新加载并推送任务
			Map<Integer, HSQuest> questMap = player.getPlayerData().getQuestMap();
			questMap.clear();
			PlayerQuestModule module = (PlayerQuestModule)player.getModule(GsConst.ModuleType.QUEST_MODULE);
			try {
				Method loadQuest = module.getClass().getDeclaredMethod("loadQuest", Map.class);
				if (loadQuest != null) {
					loadQuest.setAccessible(true);
					loadQuest.invoke(module, QuestUtil.getQuestGroupMap());
				}
				player.getPlayerData().syncQuestInfo();
			} catch (Exception e) {
				HawkException.catchException(e);
			}

			actionHandled = true;
			break;
		}
		// 清空当前角色所有已完成的任务
		case "questclear": {
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			statisticsEntity.clearQuestComplete();
			statisticsEntity.clearQuestCompleteDaily();
			statisticsEntity.notifyUpdate(true);

			// 重新加载并推送任务
			Map<Integer, HSQuest> questMap = player.getPlayerData().getQuestMap();
			questMap.clear();
			PlayerQuestModule module = (PlayerQuestModule)player.getModule(GsConst.ModuleType.QUEST_MODULE);
			try {
				Method loadQuest = module.getClass().getDeclaredMethod("loadQuest", Map.class);
				if (loadQuest != null) {
					loadQuest.setAccessible(true);
					loadQuest.invoke(module, QuestUtil.getQuestGroupMap());
				}
				player.getPlayerData().syncQuestInfo();
			} catch (Exception e) {
				HawkException.catchException(e);
			}

			actionHandled = true;
			break;
		}
		// 每日刷新
		case "dailyrefresh": {
			List<Integer> refreshIndexList = new ArrayList<Integer>();
			for (int i = 0; i < GsConst.PlayerRefreshMask.length; ++i) {
				if (0 != (GsConst.PlayerRefreshMask[i] & GsConst.RefreshMask.DAILY )) {
					refreshIndexList.add(i);
					break;
				}
			}
			if (false == refreshIndexList.isEmpty()) {
				for (HawkObjModule module : player.getObjModules().values()) {
					PlayerModule playerModule = (PlayerModule) module;
					try {
						playerModule.onPlayerRefresh(refreshIndexList, false);
					} catch (Exception e) {
						HawkException.catchException(e);
					}
				}
			}

			// 公会不走统一刷新机制
			for (AllianceEntity alliance : AllianceManager.getInstance().getAllianceMap().values()) {
				alliance.dailyRefresh();
				alliance.notifyUpdate(true);
			}

			actionHandled = true;
			break;
		}
		// 设置副本星级
		case "setstar": {
			// 因为是GM未优化
			InstanceEntryCfg targetCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, gmItemId);
			if (targetCfg == null) {
				player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
				return;
			}

			int star = (int)gmValue;
			if (star < 1 || star > 3) {
				player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
				return;
			}

			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			int oldStar = statisticsEntity.getInstanceStar(gmItemId);
			if (oldStar != star) {
				statisticsEntity.setInstanceStar(gmItemId, star);

				if (oldStar == 0) {
					// 修改所有前置未完成副本星级
					int targetChapter = targetCfg.getChapter();
					int targetIndex = targetCfg.getIndex();
					int targetDifficult = targetCfg.getDifficult();

					Map<Object, InstanceEntryCfg> cfgMap = HawkConfigManager.getInstance().getConfigMap(InstanceEntryCfg.class);
					for (InstanceEntryCfg entry : cfgMap.values()) {
						if(targetDifficult == entry.getDifficult()) {
							if (entry.getChapter() < targetChapter
									|| ( entry.getChapter() == targetChapter && entry.getIndex() < targetIndex )) {
								if (0 == statisticsEntity.getInstanceStar(entry.getInstanceId())) {
									statisticsEntity.setInstanceStar(entry.getInstanceId(), 1);
								}
							}
						} else if (targetDifficult == GsConst.InstanceDifficulty.HARD_INSTANCE
								&& entry.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
							if (entry.getChapter() <= targetChapter) {
								if (0 == statisticsEntity.getInstanceStar(entry.getInstanceId())) {
									statisticsEntity.setInstanceStar(entry.getInstanceId(), 1);
								}
							}
						}
					}

					// 重计算章节和索引
					int normalTopChapter = 0;
					int hardTopChapter = 0;
					int normalTopIndex = 0;
					int hardTopIndex = 0;
					for (Entry<String, Integer> entry : statisticsEntity.getInstanceStarMap().entrySet()) {
						if (entry.getValue() <= 0) {
							continue;
						}
						InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, entry.getKey());
						if (entryCfg != null) {
							int chapter = entryCfg.getChapter();
							int index = entryCfg.getIndex();
							if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
								if (chapter > normalTopChapter) {
									normalTopChapter = chapter;
									normalTopIndex = index;
								} else if (chapter == normalTopChapter && index > normalTopIndex) {
									normalTopIndex = index;
								}
							} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
								if (chapter > hardTopChapter) {
									hardTopChapter = chapter;
									hardTopIndex = index;
								} else if (chapter == hardTopChapter && index > hardTopIndex) {
									hardTopIndex = index;
								}
							}
						}
					}
					statisticsEntity.setNormalTopChapter(normalTopChapter);
					statisticsEntity.setNormalTopIndex(normalTopIndex);
					statisticsEntity.setHardTopChapter(hardTopChapter);
					statisticsEntity.setHardTopIndex(hardTopIndex);

					statisticsEntity.notifyUpdate(true);
				}

				// 推送副本数据
				GMInstancePush.Builder gmPush = GMInstancePush.newBuilder();
				for (Entry<String, Integer> entry : statisticsEntity.getInstanceStarMap().entrySet()) {
					InstanceState.Builder instanceState = InstanceState.newBuilder();
					instanceState.setInstanceId(entry.getKey());
					instanceState.setStar(entry.getValue());
					instanceState.setCountDaily(statisticsEntity.getInstanceEnterTimesDaily(entry.getKey()));
					gmPush.addInstanceState(instanceState);
				}
				ChapterState.Builder chapterState = ChapterState.newBuilder();
				chapterState.setNormalTopChapter(statisticsEntity.getNormalTopChapter());
				chapterState.setNormalTopIndex(statisticsEntity.getNormalTopIndex());
				chapterState.setHardTopChapter(statisticsEntity.getHardTopChapter());
				chapterState.setHardTopIndex(statisticsEntity.getHardTopIndex());
				chapterState.addAllNormalBoxState(statisticsEntity.getNormalChapterBoxStateList());
				chapterState.addAllHardBoxState(statisticsEntity.getHardChapterBoxStateList());
				gmPush.setChapterState(chapterState);
				player.sendProtocol(HawkProtocol.valueOf(HS.gm.GM_INSTANCE_PUSH_S_VALUE, gmPush));
			}

			actionHandled = true;
			break;
		}
		// 设置副本是否通关
		case "setpass": {
			// 因为是GM未优化
			InstanceEntryCfg targetCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, gmItemId);
			if (targetCfg == null) {
				player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
				return;
			}

			boolean isPass = (gmValue == 0) ? false : true;
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			boolean update = true;
			int oldStar = statisticsEntity.getInstanceStar(gmItemId);

			if (true == isPass && 0 == oldStar) {
				update = true;
				statisticsEntity.setInstanceStar(gmItemId, 1);

				// 修改所有前置未完成副本星级
				int targetChapter = targetCfg.getChapter();
				int targetIndex = targetCfg.getIndex();
				int targetDifficult = targetCfg.getDifficult();

				Map<Object, InstanceEntryCfg> cfgMap = HawkConfigManager.getInstance().getConfigMap(InstanceEntryCfg.class);
				for (InstanceEntryCfg entry : cfgMap.values()) {
					if(targetDifficult == entry.getDifficult()) {
						if (entry.getChapter() < targetChapter
								|| ( entry.getChapter() == targetChapter && entry.getIndex() < targetIndex )) {
							if (0 == statisticsEntity.getInstanceStar(entry.getInstanceId())) {
								statisticsEntity.setInstanceStar(entry.getInstanceId(), 1);
							}
						}
					} else if (targetDifficult == GsConst.InstanceDifficulty.HARD_INSTANCE
							&& entry.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
						if (entry.getChapter() <= targetChapter) {
							if (0 == statisticsEntity.getInstanceStar(entry.getInstanceId())) {
								statisticsEntity.setInstanceStar(entry.getInstanceId(), 1);
							}
						}
					}
				}
			} else if (false == isPass && 0 != oldStar){
				update = true;
				statisticsEntity.setInstanceStar(gmItemId, 0);

				// 修改所有后续已完成副本星级
				int targetChapter = targetCfg.getChapter();
				int targetIndex = targetCfg.getIndex();
				int targetDifficult = targetCfg.getDifficult();

				Map<Object, InstanceEntryCfg> cfgMap = HawkConfigManager.getInstance().getConfigMap(InstanceEntryCfg.class);
				for (InstanceEntryCfg entry : cfgMap.values()) {
					if (targetDifficult == entry.getDifficult()) {
						if (entry.getChapter() > targetChapter
								|| (entry.getChapter() == targetChapter && entry.getIndex() > targetIndex)) {
							if (0 != statisticsEntity.getInstanceStar(entry.getInstanceId())) {
								statisticsEntity.setInstanceStar(entry.getInstanceId(), 0);
							}
						}
					} else if (targetDifficult == GsConst.InstanceDifficulty.NORMAL_INSTANCE
							&& entry.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
						if (entry.getChapter() >= targetChapter) {
							if (0 != statisticsEntity.getInstanceStar(entry.getInstanceId())) {
								statisticsEntity.setInstanceStar(entry.getInstanceId(), 0);
							}
						}
					}
				}
			}

			if (true == update) {
				// 重计算章节和索引
				int normalTopChapter = 0;
				int hardTopChapter = 0;
				int normalTopIndex = 0;
				int hardTopIndex = 0;
				for (Entry<String, Integer> entry : statisticsEntity.getInstanceStarMap().entrySet()) {
					if (entry.getValue() <= 0) {
						continue;
					}
					InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, entry.getKey());
					if (entryCfg != null) {
						int chapter = entryCfg.getChapter();
						int index = entryCfg.getIndex();
						if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
							if (chapter > normalTopChapter) {
								normalTopChapter = chapter;
								normalTopIndex = index;
							} else if (chapter == normalTopChapter && index > normalTopIndex) {
								normalTopIndex = index;
							}
						} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
							if (chapter > hardTopChapter) {
								hardTopChapter = chapter;
								hardTopIndex = index;
							} else if (chapter == hardTopChapter && index > hardTopIndex) {
								hardTopIndex = index;
							}
						}
					}
				}
				statisticsEntity.setNormalTopChapter(normalTopChapter);
				statisticsEntity.setNormalTopIndex(normalTopIndex);
				statisticsEntity.setHardTopChapter(hardTopChapter);
				statisticsEntity.setHardTopIndex(hardTopIndex);

				statisticsEntity.notifyUpdate(true);

				// 推送副本数据
				GMInstancePush.Builder gmPush = GMInstancePush.newBuilder();
				for (Entry<String, Integer> entry : statisticsEntity.getInstanceStarMap().entrySet()) {
					InstanceState.Builder instanceState = InstanceState.newBuilder();
					instanceState.setInstanceId(entry.getKey());
					instanceState.setStar(entry.getValue());
					instanceState.setCountDaily(statisticsEntity.getInstanceEnterTimesDaily(entry.getKey()));
					gmPush.addInstanceState(instanceState);
				}
				ChapterState.Builder chapterState = ChapterState.newBuilder();
				chapterState.setNormalTopChapter(statisticsEntity.getNormalTopChapter());
				chapterState.setNormalTopIndex(statisticsEntity.getNormalTopIndex());
				chapterState.setHardTopChapter(statisticsEntity.getHardTopChapter());
				chapterState.setHardTopIndex(statisticsEntity.getHardTopIndex());
				chapterState.addAllNormalBoxState(statisticsEntity.getNormalChapterBoxStateList());
				chapterState.addAllHardBoxState(statisticsEntity.getHardChapterBoxStateList());
				gmPush.setChapterState(chapterState);
				player.sendProtocol(HawkProtocol.valueOf(HS.gm.GM_INSTANCE_PUSH_S_VALUE, gmPush));
			}

			actionHandled = true;
			break;
		}
		default:
			break;
		}

		if (actionHandled == false) {
			player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
			return;
		}

		if (consume.hasConsumeItem() == true) {
			if (consume.checkConsume(player, gm.GMOPERATION_C_VALUE) == false) {
				return;
			}
			consume.consumeTakeAffectAndPush(player, Action.GM_ACTION, HS.gm.GMOPERATION_C_VALUE);
		}

		if (award.hasAwardItem() == true) {
			if (award.checkLimit(player, gm.GMOPERATION_C_VALUE) == false) {
				return;
			}
			award.rewardTakeAffectAndPush(player, Action.GM_ACTION, HS.gm.GMOPERATION_C_VALUE);
		}

		GMOperationRet.Builder response = GMOperationRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.gm.GMOPERATION_S_VALUE, response));
	}

}
