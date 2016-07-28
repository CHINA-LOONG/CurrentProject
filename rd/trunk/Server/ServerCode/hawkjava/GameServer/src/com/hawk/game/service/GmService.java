package com.hawk.game.service;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.hawk.app.HawkApp;
import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkTime;

import com.hawk.game.ServerData;
import com.hawk.game.config.ItemCfg;
import com.hawk.game.config.MailSysCfg;
import com.hawk.game.config.MonsterBaseCfg;
import com.hawk.game.config.MonsterCfg;
import com.hawk.game.config.MonsterStageCfg;
import com.hawk.game.config.PlayerAttrCfg;
import com.hawk.game.config.QuestCfg;
import com.hawk.game.config.RewardCfg;
import com.hawk.game.entity.EquipEntity;
import com.hawk.game.entity.ItemEntity;
import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.StatisticsEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.item.ConsumeItems;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.manager.ImManager;
import com.hawk.game.manager.ImManager.ImMsg;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.GM.GMOperation;
import com.hawk.game.protocol.GM.GMOperationRet;
import com.hawk.game.protocol.HS.gm;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Status.error;
import com.hawk.game.service.GameService;
import com.hawk.game.util.GsConst;
import com.hawk.game.util.MailUtil;
import com.hawk.game.util.MailUtil.MailInfo;

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
				else {
					consume.addCoin((int)(-coinChange));
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
				else {
					consume.addGold((int)(-goldChange));
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
				else {
					consume.addAttr(Const.changeType.CHANGE_FATIGUE_VALUE, -tiliChange);
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
				if (equipEntity.isInvalid() == false && equipEntity.getMonsterId() == GsConst.EQUIPNOTDRESS) {
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
				ImManager.getInstance().post(lanternMsg);

				ImMsg noticeMsg = ImManager.getInstance().new ImMsg();
				noticeMsg.type = Const.ImType.NOTICE_VALUE;
				noticeMsg.channel = Const.ImChannel.WORLD_VALUE;
				noticeMsg.senderId = 0;
				noticeMsg.senderName = "";
				noticeMsg.origLang = player.getLanguage();
				noticeMsg.origText = gmOperation;
				noticeMsg.transText = null;
				ImManager.getInstance().post(noticeMsg);
			}
			actionHandled = true;
			break;
		}
		// 给指定玩家发送邮件
		case "mail": {
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, gmItemId);
			if (mailCfg == null) {
				player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
				return;
			}

			MailInfo mailInfo = new MailInfo();
			mailInfo.subject = mailCfg.getSubject();
			mailInfo.content = mailCfg.getContent();
			RewardCfg reward = mailCfg.getReward();
			if (reward != null) {
				mailInfo.rewardList = reward.getRewardList();
			}

			for (int i = 0; i < gmValue; ++i) {
				MailUtil.SendMail(mailInfo, (int)gmTargetId, 0, mailCfg.getSender());
			}

			actionHandled = true;
			break;
		}
		// 给所有玩家发送邮件
		case "mailall": {
			MailSysCfg mailCfg = HawkConfigManager.getInstance().getConfigByKey(MailSysCfg.class, gmItemId);
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

			actionHandled = true;
			break;
		}
		// 清空当前角色所有已完成的任务
		case "questclear": {
			StatisticsEntity statisticsEntity = player.getPlayerData().getStatisticsEntity();
			statisticsEntity.clearQuestComplete();
			statisticsEntity.clearQuestCompleteDaily();
			statisticsEntity.notifyUpdate(true);
			actionHandled = true;
			break;
		}
		}

		if (actionHandled == false) {
			player.sendError(gm.GMOPERATION_C_VALUE, error.PARAMS_INVALID_VALUE);
			return;
		}

		if (consume.hasConsumeItem() == true) {
			if (consume.checkConsume(player, gm.GMOPERATION_C_VALUE) == false) {
				return;
			}
			else {
				consume.consumeTakeAffectAndPush(player, Action.GM_ACTION, HS.gm.GMOPERATION_C_VALUE);
			}
		}

		if (award.hasAwardItem() == true) {
			award.rewardTakeAffectAndPush(player, Action.GM_ACTION, HS.gm.GMOPERATION_C_VALUE);
		}

		GMOperationRet.Builder response = GMOperationRet.newBuilder();
		player.sendProtocol(HawkProtocol.valueOf(HS.gm.GMOPERATION_S_VALUE, response));
	}

}
