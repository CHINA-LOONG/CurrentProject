package com.hawk.game.gm;

import java.util.LinkedList;
import java.util.List;

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
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.HS.gm;
import com.hawk.game.protocol.HS.sys;
import com.hawk.game.protocol.SysProtocol.HSWarnPlayer;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.MailUtil.MailInfo;
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
			response.put("sessionID", paramJson.getInt("sessionID"));
			
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
				HawkScriptManager.sendResponse(httpExchange, response.toString());
				return;
			}
			
			// 锁住玩家
			HawkXID xid = HawkXID.valueOf(GsConst.ObjType.PLAYER, playerId);
			if(xid != null){
				HawkObjBase<HawkXID, HawkAppObj> objBase = GsApp.getInstance().lockObject(xid);
				try {
					if (objBase == null || !objBase.isObjValid()) {
						objBase = GsApp.getInstance().createObj(xid);
						if (objBase != null) {
							objBase.lockObj();
						}
						
						if (objBase == null) {
							response.put("info", "server error");
							HawkScriptManager.sendResponse(httpExchange, response.toString());
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
					}
				   
					handleGMCommnad((Player)objBase.getImpl(), paramJson, response);	
				} 
				finally {
					if (objBase != null) {
						objBase.unlockObj();
					}
				}
			}
			
			
		} catch (Exception e) {
			HawkException.catchException(e);
		}
		finally{
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
		
		// 禁言 / 取消禁言
		if (command.equals("dump")) {
			int minute = request.getInt("minute");
			if (minute > 0) {
				player.getPlayerData().getStatisticsEntity().setDumpTime(HawkTime.getSeconds() + minute * 60);
				player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
				commadnHandled = true;
			}
		}
		else if (command.equals("dump-")) {
			player.getPlayerData().getStatisticsEntity().setDumpTime(0);
			player.getPlayerData().getStatisticsEntity().notifyUpdate(true);
			commadnHandled = true;
		}
		// 锁定 / 取消锁定
		if (command.equals("lock")) {
			int minute = request.getInt("minute");
			if (minute > 0) {
				player.getPlayerData().getPlayerEntity().setLockTime(HawkTime.getSeconds() + minute * 60);
				player.getPlayerData().getPlayerEntity().notifyUpdate(true);
				commadnHandled = true;
			}
		}
		else if (command.equals("lock-")) {
			player.getPlayerData().getPlayerEntity().setLockTime(0);
			player.getPlayerData().getPlayerEntity().notifyUpdate(true);
			commadnHandled = true;
		}
		// 踢角色下线
		else if (command.equals("kickcharacter")) {
			if (player.getSession() != null) {
				player.kickout(Const.kickReason.GM_VALUE);
				player.getSession().setAppObject(null);
				player.setSession(null);
			}
			commadnHandled = true;
		}
		// 清空背包
		else if (command.equals("removeallitem")) {
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
			commadnHandled = true;
		}
		// 清空背包
		else if (command.equals("removeitem")) {
			String itemID = request.containsKey("itemID") ? request.getString("itemID") : "";
			int count = request.containsKey("count") ? request.getInt("count") : 0;
			ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemID);
			ItemEntity itemEntity = player.getPlayerData().getItemByItemId(itemID);
			if (itemCfg == null) {
				response.put("info", "item config not found");
			}
			else if (itemCfg.getType() != Const.itemType.ITEM_VALUE) {
				response.put("info", "itemID not item");
			}
			else if (itemEntity.getCount() < count) {
				response.put("info", "item not enough");
			}
			else {
				itemEntity.setCount(itemEntity.getCount() - count);
				itemEntity.notifyUpdate(true);
			}

			commadnHandled = true;
		}
		// 发送邮件
		else if (command.equals("sendmailID")) {
			String mailID = request.containsKey("id") ? request.getString("id") : "";				
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, Integer.parseInt(mailID));
			if (mailCfg == null) {
				response.put("info", "item not enough");
			}
			else {
				MailUtil.SendSysMail(mailCfg, player.getId());
			}
			
			commadnHandled = true;
		}
		// 发送邮件
		else if (command.equals("sendmail")) {
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
				String itemID = item.containsKey("itemID") ? item.getString("itemID") : "";
				int count = item.containsKey("count") ? item.getInt("count") : 0;
				ItemCfg itemCfg = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, itemID);
				if (itemCfg != null) {
					if (itemCfg.getType() == Const.itemType.ITEM_VALUE && count > 0) {
						rewardList.add(ItemInfo.valueOf(Const.itemType.ITEM_VALUE, itemID, count));
					}
					else if (itemCfg.getType() == Const.itemType.EQUIP_VALUE) {
						int stage = item.containsKey("stage") ? item.getInt("stage") : 1;
						int level = item.containsKey("level") ? item.getInt("level") : 1;
						rewardList.add(ItemInfo.valueOf(Const.itemType.EQUIP_VALUE, itemID, count, stage, level));
					}				
				}
			}
			
			MailInfo mailInfo = new MailInfo();
			mailInfo.subject = subject;
			mailInfo.content = text;	
			mailInfo.rewardList = rewardList;
			MailUtil.SendMail(mailInfo, player.getId(), 0, "GM");	
			
			commadnHandled = true;
		}
		// 警告
		else if (command.equals("warn")) {
			if (player.getSession() != null) {
				String content = request.containsKey("content") ? request.getString("content") : "";
				HSWarnPlayer.Builder builder = HSWarnPlayer.newBuilder();
				builder.setContent(content);
				player.sendProtocol(HawkProtocol.valueOf(sys.WARN_VALUE, builder));
			}
			
			commadnHandled = true;
		}
		// 查询玩家属性
		else if (command.equals("showattr")) {
			JSONObject attrObject = new JSONObject();
			attrObject.put("platform", player.getPlatform());
			attrObject.put("server", GsConfig.getInstance().getServerId());
			attrObject.put("platform", player.getPlatform());
			attrObject.put("level", player.getLevel());
			attrObject.put("exp", player.getExp());
			attrObject.put("coin", player.getCoin());
			attrObject.put("buyGold", player.getPlayerData().getPlayerEntity().getBuyGold());
			attrObject.put("freeGold", player.getPlayerData().getPlayerEntity().getFreeGold());
			attrObject.put("energy", player.getPlayerData().getStatisticsEntity().getFatigue());
			response.put("attr", attrObject);
			
			commadnHandled = true;
		}
		// 查询道具
		else if (command.equals("showitems")) {
			JSONArray items = new JSONArray();
			for (ItemEntity itemEntity : player.getPlayerData().getItemEntityMap().values()) {
				if (itemEntity.isInvalid() == false && itemEntity.getCount() != 0) {
					JSONObject item = new JSONObject();
					item.put("itemID", itemEntity.getItemId());
					item.put("count", itemEntity.getCount());
					items.add(item);
				}
			}
			
			response.put("item_list", items);
			
			commadnHandled = true;
		}
		// 查询装备
		else if (command.equals("showequips")) {
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
			
			response.put("equip_list", equips);
			
			commadnHandled = true;
		}
		// 查询怪物
		else if (command.equals("showmonsters")) {
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
			
			response.put("monster_list", monsters);
			
			commadnHandled = true;
		}
		// 查询工会
		else if (command.equals("showguild")) {
			if (AllianceManager.getInstance().getPlayerAllinceId(player.getId()) != 0) {
				PlayerAllianceEntity playerAllianceEntity = AllianceManager.getInstance().getPlayerAllianceEntity(player.getId());
				if (playerAllianceEntity != null) {
					JSONObject guild = new JSONObject();
					guild.put("guildId", playerAllianceEntity.getAllianceId());
					guild.put("position", playerAllianceEntity.getPostion());
					guild.put("contribution", playerAllianceEntity.getContribution());
					guild.put("totalContributuoin", playerAllianceEntity.getTotalContribution());
					request.put("guild", guild);
				}
			}
			
			commadnHandled = true;
		}
		// 查询工会
		else if (command.equals("showid")) {
			request.put("puid", player.getPuid());
			commadnHandled = true;
		}	
		
		if (consume.hasConsumeItem() == true) {
			if (consume.checkConsume(player, gm.GMOPERATION_C_VALUE) == false) {
				return;
			}
			consume.consumeTakeAffectAndPush(player, Action.GM, HS.gm.GMOPERATION_C_VALUE);
		}

		if (award.hasAwardItem() == true) {
			if (award.checkLimit(player, gm.GMOPERATION_C_VALUE) == false) {
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
