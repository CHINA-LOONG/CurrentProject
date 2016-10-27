package com.hawk.game.util;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.hawk.app.HawkApp;
import org.hawk.config.HawkConfigManager;
import org.hawk.msg.HawkMsg;
import org.hawk.xid.HawkXID;

import com.hawk.game.config.QuestCfg;

public class QuestUtil {

	public static class QuestGroup {
		public int groupId;
		public List<QuestCfg> questList = new ArrayList<>();
	}

	/**
	 * @key groupId
	 */
	private static Map<Integer, QuestGroup> questGroupMap = new HashMap<>();
	/**
	 * @key cycleId
	 */
	private static Map<Integer, Map<Integer, QuestGroup>> cycleQuestGroupMap = new HashMap<>();

	// 构造阶段---------------------------------------------------------------------

	/**
	 * 添加任务
	 */
	public static void addQuestCfg(QuestCfg questCfg) {
		QuestGroup group = questGroupMap.get(questCfg.getGroup());
		if (group == null) {
			group = new QuestGroup();
			group.groupId = (questCfg.getGroup());
			questGroupMap.put(group.groupId, group);

			Map<Integer, QuestGroup> groupMap = cycleQuestGroupMap.get(questCfg.getCycle());
			if (groupMap == null) {
				groupMap = new HashMap<Integer, QuestGroup>();
				cycleQuestGroupMap.put(questCfg.getCycle(), groupMap);
			}
			groupMap.put(group.groupId, group);
		}
		group.questList.add(questCfg);
	}

	// 使用阶段----------------------------------------------------------------------

	/**
	 * 获得任务所属组
	 */
	public static QuestGroup getQuestGroupByQuest(int questId) {
		QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		if (questCfg == null) {
			return null;
		}
		return questGroupMap.get(questCfg.getGroup());
	}

	/**
	 * 获得同组下个任务
	 */
	public static QuestCfg getNextQuest(int questId) {
		QuestCfg questCfg = HawkConfigManager.getInstance().getConfigByKey(QuestCfg.class, questId);
		if (questCfg == null) {
			return null;
		}
		QuestGroup group = questGroupMap.get(questCfg.getGroup());
		int index = group.questList.indexOf(questCfg);
		if (index < group.questList.size() - 1 && index > -1) {
			return group.questList.get(index + 1);
		}

		return null;
	}

	/**
	 * 获得所有任务组
	 */
	public static Map<Integer, QuestGroup> getQuestGroupMap() {
		return Collections.unmodifiableMap(questGroupMap);
	}

	/**
	 * 获得某循环性任务组
	 */
	public static Map<Integer, QuestGroup> getCycleQuestGroupMap(int cycle) {
		return Collections.unmodifiableMap(cycleQuestGroupMap.get(cycle));
	}

	/**
	 * 生成任务相关数据有更新消息
	 */
	public static void postQuestDataUpdateMsg(HawkXID xid) {
		HawkMsg msg = HawkMsg.valueOf(GsConst.MsgType.STATISTICS_UPDATE, xid);
		if (false == HawkApp.getInstance().postMsg(msg)) {
			//HawkLog.errPrintln("post statistics update message failed: " + xid.getId());
		}
	}

}
