package com.hawk.game.gm;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hawk.script.HawkScript;
import org.hawk.script.HawkScriptManager;
import org.hawk.util.HawkJsonUtil;
import org.hawk.util.services.FunPlusPushService;
import org.hawk.util.services.HawkAccountService;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.GsConfig;
import com.hawk.game.ServerData;
import com.hawk.game.BILog.BIBehaviorAction.Action;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.entity.statistics.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.manager.ImManager;
import com.hawk.game.manager.ImManager.ImMsg;
import com.hawk.game.module.PlayerQuestModule;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.HS.code;
import com.hawk.game.protocol.HS.sys;
import com.hawk.game.protocol.Im.HSImDump;
import com.hawk.game.protocol.Quest.HSQuest;
import com.hawk.game.protocol.SysProtocol.HSWarnPlayer;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.MailUtil.MailInfo;
import com.hawk.game.util.QuestUtil;
import com.sun.net.httpserver.HttpExchange;

public class OpsGMHandler extends HawkScript{
	public int GM_OK = 0;
	public int GM_ERROR = 1;

	@Override
	public void action(String params, HttpExchange httpExchange) {
		JSONObject response = new JSONObject();
		response.put("result", GM_ERROR);

		try {
			JSONObject paramJson = JSONObject.fromObject(params);
			if (false == paramJson.containsKey("sessionId")) {
				response.put("info", "missing sessionId");
				return;
			}
			response.put("sessionId", paramJson.getInt("sessionId"));

			if (false == paramJson.containsKey("puid") && false == paramJson.containsKey("nickname")) {
				handleGMCommnad(null, paramJson, response);
			} else {
				String puid = paramJson.containsKey("puid") ? paramJson.getString("puid") : "";
				String nickname = paramJson.containsKey("nickname") ? paramJson.getString("nickname") : "";
				int playerId = 0;

				if (puid == null || puid.equals("")) {
					playerId = ServerData.getInstance().getPlayerIdByNickname(nickname);
					puid = ServerData.getInstance().getPuidByPlayerId(playerId);
				}
				else {
					playerId = ServerData.getInstance().getPlayerIdByPuid(puid);
				}

				if (playerId == 0) {
					response.put("info", "player not exist");
					return;
				}

				// 锁住玩家
				HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, playerId);
				HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
				try {
					if (objBase == null || false == objBase.isObjValid()) {
						objBase = GsApp.getInstance().createObj(xid);
						if (objBase != null) {
							objBase.lockObj();
						}

						if (objBase == null) {
							response.put("info", "server error");
							return;
						}

						// 加载离线数据
						Player player = (Player) objBase.getImpl();
						player.getPlayerData().setPuid(puid);
						player.getPlayerData().loadPlayer();
						player.getPlayerData().loadStatistics();
						player.getPlayerData().loadAllMonster();
						player.getPlayerData().loadAllItem();
						player.getPlayerData().loadAllEquip();
						player.getPlayerData().loadShop();
						player.getPlayerData().loadPlayerAlliance();
					}

					handleGMCommnad((Player)objBase.getImpl(), paramJson, response);
				} finally {
					if (objBase != null) {
						objBase.unlockObj();
					}
				}
			}

		} catch (Exception e) {
			HawkException.catchException(e);
		} finally{
			HawkScriptManager.sendResponse(httpExchange, response.toString());
		}
	}

	/**
	 *  处理GM命令
	 * @param player
	 * @param request
	 * @param response
	 */
	private void handleGMCommnad(Player player, JSONObject request, JSONObject response){
		String command = request.containsKey("commnad") ? request.getString("commnad") : "";
		boolean commadnHandled = false;
		ConsumeItems consume = new ConsumeItems();
		AwardItems award = new AwardItems();

		switch (command) {
		// 禁言 / 取消禁言
		case "dump": {
			int minute = request.containsKey("minute") ? request.getInt("minute") : 0;
			if (minute > 0) {
				StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
				statisticsEntity.setDumpTime(HawkTime.getSeconds() + minute * 60);
				statisticsEntity.notifyUpdate(true);

				if (player.getSession() != null) {
					HSImDump.Builder builder = HSImDump.newBuilder();
					builder.setDumpEndTime(statisticsEntity.getDumpTime());
					player.sendProtocol(HawkProtocol.valueOf(code.IM_DUMP_S, builder));
				}
				commadnHandled = true;
			}
			break;
		}
		case "dump-": {
			player.getPlayerData().getStatisticsEntity().setDumpTime(0);
			player.getPlayerData().getStatisticsEntity().notifyUpdate(true);

			if (player.getSession() != null) {
				HSImDump.Builder builder = HSImDump.newBuilder();
				builder.setDumpEndTime(0);
				player.sendProtocol(HawkProtocol.valueOf(code.IM_DUMP_S, builder));
			}
			commadnHandled = true;
			break;
		}
		// 锁定 / 取消锁定
		case "lock": {
			int minute = request.containsKey("minute") ? request.getInt("minute") : 0;
			if (minute > 0) {
				player.getPlayerData().getPlayerEntity().setLockTime(HawkTime.getSeconds() + minute * 60);
				player.getPlayerData().getPlayerEntity().notifyUpdate(true);
				commadnHandled = true;
			}
			break;
		}
		case "lock-": {
			player.getPlayerData().getPlayerEntity().setLockTime(0);
			player.getPlayerData().getPlayerEntity().notifyUpdate(true);
			commadnHandled = true;
			break;
		}
		// 踢角色下线
		case "kick": {
			if (player.getSession() != null) {
				player.kickout(Const.kickReason.GM_VALUE);
				player.getSession().setAppObject(null);
				player.setSession(null);
			}
			commadnHandled = true;
			break;
		}
		// 清空背包
		case "removeallitem": {
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

			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.GM_REMOVE_ALL_ITEM);
			if (mailCfg != null) {
				MailUtil.SendSysMail(mailCfg, player.getId());
			}

			commadnHandled = true;
			break;
		}
		// 删除物品
		case "removeitem": {
			String itemId = request.containsKey("id") ? request.getString("id") : "";
			int count = request.containsKey("count") ? request.getInt("count") : 0;
			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
			ItemEntity itemEntity = player.getPlayerData().getItemByItemId(itemId);
			if (itemCfg == null) {
				response.put("info", "item config not found");
			}
			else if (itemCfg.getType() != Const.itemType.ITEM_VALUE) {
				response.put("info", "itemId not item");
			}
			else if (itemEntity.getCount() < count) {
				response.put("info", "item not enough");
			}
			else {
				itemEntity.setCount(itemEntity.getCount() - count);
				itemEntity.notifyUpdate(true);
			}

			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.GM_REMOVE_ITEM);
			if (mailCfg != null) {
				MailUtil.SendSysMail(mailCfg, player.getId(), itemId, count);
			}

			commadnHandled = true;
			break;
		}
		// 发送系统邮件
		case "sendmailId": {
			String mailId = request.containsKey("id") ? request.getString("id") : "";
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, Integer.parseInt(mailId));
			if (mailCfg == null) {
				response.put("info", "item not enough");
			}
			else {
				MailUtil.SendSysMail(mailCfg, player.getId());
			}

			commadnHandled = true;
			break;
		}
		// 发送邮件
		case "sendmail": {
			int coin = request.containsKey("coin") ? request.getInt("coin") : 0;
			String subject = request.containsKey("subject") ? request.getString("subject") : "";
			String text = request.containsKey("text") ? request.getString("text") : "";
			String itemJson = request.containsKey("item") ? request.getString("item") : "{}";
			List<ItemInfo> rewardList = new LinkedList<ItemInfo>();

			if (coin > 0) {
				ItemInfo item = ItemInfo.valueOf(Const.itemType.PLAYER_ATTR_VALUE, String.valueOf(Const.changeType.CHANGE_COIN_VALUE), coin);
				rewardList.add(item);
			}

			JSONArray items = JSONArray.fromObject(itemJson);
			for (Object element : items) {
				JSONObject item = (JSONObject) element;
				String itemId = item.containsKey("id") ? item.getString("id") : "";
				int count = item.containsKey("count") ? item.getInt("count") : 0;
				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemId);
				if (itemCfg != null) {
					if (itemCfg.getType() == Const.itemType.ITEM_VALUE && count > 0) {
						rewardList.add(ItemInfo.valueOf(Const.itemType.ITEM_VALUE, itemId, count));
					}
					else if (itemCfg.getType() == Const.itemType.EQUIP_VALUE) {
						int stage = item.containsKey("stage") ? item.getInt("stage") : 1;
						int level = item.containsKey("level") ? item.getInt("level") : 1;
						rewardList.add(ItemInfo.valueOf(Const.itemType.EQUIP_VALUE, itemId, count, stage, level));
					}
				}
			}

			MailInfo mailInfo = new MailInfo();
			mailInfo.subject = subject;
			mailInfo.content = text;
			mailInfo.rewardList = rewardList;
			MailUtil.SendMail(mailInfo, player.getId(), 0, "GM");

			commadnHandled = true;
			break;
		}
		// 警告
		case "warn": {
			if (player.getSession() != null) {
				String content = request.containsKey("content") ? request.getString("content") : "";
				HSWarnPlayer.Builder builder = HSWarnPlayer.newBuilder();
				builder.setContent(content);
				player.sendProtocol(HawkProtocol.valueOf(sys.WARN_VALUE, builder));
			}

			commadnHandled = true;
			break;
		}
		// 查询玩家属性
		case "showattr": {
			JSONObject attrObject = new JSONObject();
			attrObject.put("device", player.getDevice());
			attrObject.put("server", GsConfig.getInstance().getServerId());
			attrObject.put("platform", player.getPlatform());
			attrObject.put("level", player.getLevel());
			attrObject.put("exp", player.getExp());
			attrObject.put("coin", player.getCoin());
			attrObject.put("buyDiamond", player.getPlayerData().getPlayerEntity().getBuyGold());
			attrObject.put("freeDiamond", player.getPlayerData().getPlayerEntity().getFreeGold());
			attrObject.put("energy", player.getPlayerData().getStatisticsEntity().getFatigue());
			response.put("attr", attrObject);

			commadnHandled = true;
			break;
		}
		// 查询怪物
		case "showmonster": {
			JSONArray monsters = new JSONArray();
			for (MonsterEntity monsterEntity : player.getPlayerData().getMonsterEntityMap().values()) {
				if (monsterEntity.isInvalid() == false) {
					JSONObject monster = new JSONObject();
					monster.put("id", monsterEntity.getId());
					monster.put("monster", monsterEntity.getCfgId());
					monster.put("stage", monsterEntity.getStage());
					monster.put("level", monsterEntity.getLevel());
					monster.put("exp", monsterEntity.getExp());
					monsters.add(monster);
				}
			}
			response.put("monsterList", monsters);

			commadnHandled = true;
			break;
		}
		// 查询道具
		case "showitem": {
			JSONArray items = new JSONArray();
			for (ItemEntity itemEntity : player.getPlayerData().getItemEntityMap().values()) {
				if (itemEntity.isInvalid() == false && itemEntity.getCount() != 0) {
					JSONObject item = new JSONObject();
					item.put("itemId", itemEntity.getItemId());
					item.put("count", itemEntity.getCount());
					items.add(item);
				}
			}
			response.put("itemList", items);

			commadnHandled = true;
			break;
		}
		// 查询装备
		case "showequip": {
			JSONArray equips = new JSONArray();
			for (EquipEntity equipEntity : player.getPlayerData().getEquipEntityMap().values()) {
				if (equipEntity.isInvalid() == false) {
					JSONObject equip = new JSONObject();
					equip.put("id", equipEntity.getId());
					equip.put("equipId", equipEntity.getItemId());
					equip.put("stage", equipEntity.getStage());
					equip.put("level", equipEntity.getLevel());
					equip.put("monsterId", equipEntity.getMonsterId());
					equips.add(equip);
				}
			}
			response.put("equipList", equips);

			commadnHandled = true;
			break;
		}
		// 查询公会
		case "showguild": {
			if (AllianceManager.getInstance().getPlayerAllinceId(player.getId()) != 0) {
				PlayerAllianceEntity playerAllianceEntity = AllianceManager.getInstance().getPlayerAllianceEntity(player.getId());
				if (playerAllianceEntity != null) {
					JSONObject guild = new JSONObject();
					guild.put("guildId", playerAllianceEntity.getAllianceId());
					guild.put("position", playerAllianceEntity.getPostion());
					guild.put("contribution", playerAllianceEntity.getContribution());
					guild.put("totalContribution", playerAllianceEntity.getTotalContribution());
					response.put("guild", guild);
				}
			}

			commadnHandled = true;
			break;
		}
		// 查询进度
		case "showprogress": {
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			response.put("instanceNormalTopChapter", statisticsEntity.getNormalTopChapter());
			response.put("instanceNormalTopIndex", statisticsEntity.getNormalTopIndex());
			response.put("instanceHardTopChapter", statisticsEntity.getHardTopChapter());
			response.put("instanceHardTopIndex", statisticsEntity.getHardTopIndex());
			response.put("hole", HawkJsonUtil.getJsonInstance().toJson(statisticsEntity.getHoleTimesMap()));
			response.put("tower", HawkJsonUtil.getJsonInstance().toJson(statisticsEntity.getTowerFloorMap()));

			commadnHandled = true;
			break;
		}
		// 查询任务
		case "showquest": {
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			response.put("complete", statisticsEntity.getQuestCompleteSet());
			response.put("completeDaily", statisticsEntity.getQuestDailyCompleteSet());

			if (false == player.isAssembleFinish()) {
				PlayerQuestModule module = (PlayerQuestModule) player.getModule(GsConst.ModuleType.QUEST_MODULE);
				try {
					Method loadQuest = module.getClass().getDeclaredMethod("loadQuest", Map.class);
					if (loadQuest != null) {
						loadQuest.setAccessible(true);
						loadQuest.invoke(module, QuestUtil.getQuestGroupMap());
					}
				} catch (Exception e) {
					HawkException.catchException(e);
				}
			}
			JSONObject questMap = new JSONObject();
			for (HSQuest quest : player.getPlayerData().getQuestMap().values()) {
				questMap.put(quest.getQuestId(), quest.getProgress());
			}
			response.put("current", questMap);

			commadnHandled = true;
			break;
		}
		// 查询玩家puid
		case "showid": {
			response.put("puid", player.getPuid());
			commadnHandled = true;
			break;
		}
		// 走马灯/系统公告
		case "broadcast": {
			int count = request.containsKey("count") ? request.getInt("count") : 0;
			int internalMinute = request.containsKey("interval") ? request.getInt("interval") : 0;
			String language = request.containsKey("language") ? request.getString("language") : GsConst.DEFAULT_LANGUAGE;
			String message = request.containsKey("message") ? request.getString("message") : "";
			int type = request.containsKey("type") ? request.getInt("type") : 0;

			if (count > 0 && internalMinute > 0 && message != "" && (type == 1 || type == 2 || type == 3 || type == 4)) {
				ImMsg msg = ImManager.getInstance().new ImMsg();
				msg.type = type;
				msg.channel = Const.ImChannel.WORLD_VALUE;
				msg.senderId = 0;
				msg.senderName = "";
				msg.origLang = language;
				msg.origText = message;
				msg.transText = null;
				msg.expansion = null;
				msg.times = count;
				msg.intervalSecond = internalMinute * 60;
				msg.nextTime = HawkTime.getSeconds();
				ImManager.getInstance().post(msg);
				commadnHandled = true;
			}
			break;
		}
		case "deletebroadcast": {
			int type = request.containsKey("type") ? request.getInt("type") : 0;
			ImManager.getInstance().clearClockMsg(type);
			commadnHandled = true;
			break;
		}
		// 修改人物昵称
		case "rename": {
			String newname = request.containsKey("newname") ? request.getString("newname") : "";
			ServerData.getInstance().replaceNameAndPlayerId(player.getName(), newname, player.getId());
			player.getEntity().setNickname(newname);
			player.getEntity().notifyUpdate(true);
			HawkAccountService.getInstance().report(new HawkAccountService.RenameRoleData(player.getPuid(), player.getId(), newname));

			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.GM_RENAME);
			if (mailCfg != null) {
				MailUtil.SendSysMail(mailCfg, player.getId(), newname);
			}
			commadnHandled = true;
			break;
		}
		// 扣金币
		case "decuctcoin": {
			long count = request.containsKey("count") ? request.getLong("count") : 0;
			if (count > 0) {
				if (count > player.getCoin()) {
					count = player.getCoin();
				}
				consume.addCoin(count);

				MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.GM_DECUCT_COIN);
				if (mailCfg != null) {
					MailUtil.SendSysMail(mailCfg, player.getId(), count);
				}
				commadnHandled = true;
			}
			break;
		}
		// 扣钻石
		case "decuctdiamond": {
			int count = request.containsKey("count") ? request.getInt("count") : 0;
			if (count > 0) {
				if (count > player.getGold()) {
					count = player.getGold();
				}
				consume.addGold(count);

				MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, GsConst.SysMail.GM_DECUCT_DIAMOND);
				if (mailCfg != null) {
					MailUtil.SendSysMail(mailCfg, player.getId(), count);
				}
				commadnHandled = true;
			}
			break;
		}
		// 推送
		case "push": {
			String message = request.containsKey("message") ? request.getString("message") : "";
			String funplusIdJson = request.containsKey("funplusid") ? request.getString("funplusid") : "";
			List<Integer> puidList = new ArrayList<Integer>();

			JSONArray puids = JSONArray.fromObject(funplusIdJson);
			for (Object element : puids) {
				puidList.add((Integer) element);
			}

			if (true == FunPlusPushService.getInstance().pushSimple(message, puidList)) {
				commadnHandled = true;
			}
			break;
		}
		default:
			break;
		}

		if (consume.hasConsumeItem() == true) {
			if (consume.checkConsume(player, 0) == false) {
				return;
			}
			consume.consumeTakeAffectAndPush(player, Action.GM, HS.gm.GMOPERATION_C_VALUE);
		}

		if (award.hasAwardItem() == true) {
			if (award.checkLimit(player, 0) == false) {
				return;
			}
			award.rewardTakeAffectAndPush(player, Action.GM, HS.gm.GMOPERATION_C_VALUE);
		}

		// gm处理结果
		if (commadnHandled) {
			response.put("result", GM_OK);
		}
		else {
			response.put("info", "unsupport command");
		}
	}
}
