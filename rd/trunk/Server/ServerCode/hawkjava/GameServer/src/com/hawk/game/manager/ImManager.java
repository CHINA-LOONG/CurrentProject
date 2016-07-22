package com.hawk.game.manager;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Queue;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedQueue;

import org.hawk.app.HawkApp;
import org.hawk.app.HawkAppObj;
import org.hawk.cache.HawkCacheObj;
import org.hawk.net.HawkSession;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.thread.HawkTask;
import org.hawk.util.services.FunPlusTranslateService;
import org.hawk.util.services.FunPlusTranslateService.TranslateRequest;
import org.hawk.xid.HawkXID;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.GsConfig;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Im.HSImPush;
import com.hawk.game.util.BuilderUtil;
import com.hawk.game.util.GsConst;

/**
 * 即时通讯管理器
 * 
 * @author walker
 */
public class ImManager extends HawkAppObj {

	// 结构----------------------------------------------------------------------------------------------------------

	public class ImMsg {
		public int type;
		public int channel;
		public int senderId;
		public String senderName;
		public String origLang;
		public String origText;
		/**
		 * @key 语言IOS代码
		 */
		public Map<String, String> transText;
		public int guildId;
	}

	public class ImPlayer {
		public String language;
		public HawkSession session;

		public ImPlayer(String language, HawkSession session) {
			this.language = language;
			this.session = session;
		}
	}

	// 属性----------------------------------------------------------------------------------------------------------
	// 翻译批次大小
	public static final int TRANSLATE_BATCH_SIZE = 5;
	public static final int PUSH_BATCH_SIZE = 20;

	private static final Logger logger = LoggerFactory.getLogger("Server");

	/**
	 * 世界频道会话列表
	 * @key playerId
	 */
	private ConcurrentHashMap<Integer, ImPlayer> worldPlayerMap;

	/**
	 * 公会频道会话列表
	 * @key guildId
	 */
	private ConcurrentHashMap<Integer, ConcurrentHashMap<Integer, ImPlayer>> guildPlayerMap;

	/**
	 * 世界频道语言计数
	 * @key 语言ISO代码
	 * @value 该语言player数量
	 */
	private ConcurrentHashMap<String, Integer> worldLangMap;

	/**
	 * 公会频道语言计数
	 * @key guildId
	 */
	private ConcurrentHashMap<Integer, ConcurrentHashMap<String, Integer>> guildLangMap;

	/**
	 * 待推送消息队列
	 */
	private ConcurrentLinkedQueue<ImMsg> worldMsgQueue;
	private ConcurrentHashMap<Integer, ConcurrentLinkedQueue<ImMsg>> guildMsgQueueMap;

	/**
	 * 待翻译消息队列
	 */
	private ConcurrentLinkedQueue<ImMsg> transMsgQueue;

	private static ImManager instance = null;
	public static ImManager getInstance() {
		return instance;
	}

	public ImManager(HawkXID xid) {
		super(xid);
		if (instance == null) {
			instance = this;
		}

		worldPlayerMap = new ConcurrentHashMap<Integer, ImPlayer>();
		worldLangMap = new ConcurrentHashMap<String, Integer>();
		guildPlayerMap = new ConcurrentHashMap<Integer, ConcurrentHashMap<Integer, ImPlayer>>();
		guildLangMap = new ConcurrentHashMap<Integer, ConcurrentHashMap<String, Integer>>();
		worldMsgQueue = new ConcurrentLinkedQueue<ImMsg>();
		guildMsgQueueMap = new ConcurrentHashMap<Integer, ConcurrentLinkedQueue<ImMsg>>();
		transMsgQueue = new ConcurrentLinkedQueue<ImMsg>();
	}

	/**
	 * 加入世界频道
	 */
	public void joinWorld(int playerId, String language, HawkSession session) {
		if (session != null) {
			worldPlayerMap.put(playerId, new ImPlayer(language, session));

			synchronized (worldLangMap) {
				Integer count = worldLangMap.get(language);
				if (count == null) {
					count = 0;
				}
				worldLangMap.put(language, count + 1);
			}
		}
	}

	/**
	 * 加入公会频道
	 */
	public void joinGuild(int guildId, int playerId, String language, HawkSession session) {
		if (session != null) {
			ConcurrentHashMap<Integer, ImPlayer> playerMap = null;
			synchronized (guildPlayerMap) {
				playerMap = guildPlayerMap.get(guildId);
				if (playerMap == null) {
					playerMap = new ConcurrentHashMap<Integer, ImPlayer>();
					guildPlayerMap.put(guildId, playerMap);
				}
			}
			// 直接覆盖，不需同步
			playerMap.put(playerId, new ImPlayer(language, session));

			synchronized (guildLangMap) {
				ConcurrentHashMap<String, Integer> langMap = guildLangMap.get(guildId);
				if (langMap == null) {
					langMap = new ConcurrentHashMap<String, Integer>();
					guildLangMap.put(guildId, langMap);
				}

				Integer count = langMap.get(language);
				if (count == null) {
					count = 0;
				}
				langMap.put(language, count + 1);
			}
		}
	}

	/**
	 * 退出世界频道
	 */
	public void quitWorld(int playerId) {
		ImPlayer imPlayer = worldPlayerMap.remove(playerId);
		if (imPlayer != null) {
			synchronized (worldLangMap) {
				Integer count = worldLangMap.get(imPlayer.language);
				if (count != null && count != 0) {
					worldLangMap.put(imPlayer.language, count - 1);
				}
			}
		}
	}

	/**
	 * 退出公会频道
	 */
	public void quitGuild(int guildId, int playerId) {
		Map<Integer, ImPlayer> playerMap = guildPlayerMap.get(guildId);
		if (playerMap != null) {
			ImPlayer imPlayer = playerMap.remove(playerId);
			if (imPlayer != null) {
				Map<String, Integer> langMap = guildLangMap.get(guildId);
				if (langMap != null) {
					synchronized(langMap) {
						Integer count = langMap.get(imPlayer.language);
						if (count != null && count != 0) {
							langMap.put(imPlayer.language, count - 1);
						}
					}
				}
			}
		}
	}

	/**
	 * 删除公会
	 */
	public void removeGuild(int guildId) {
		guildPlayerMap.remove(guildId);
		guildLangMap.remove(guildId);
	}

	/**
	 * 投递聊天消息(由player自身调用)
	 */
	public void postChat(Player player, String chatText, int channel) {
		ImMsg msgObj = new ImMsg();
		msgObj.type = Const.ImType.CHAT_VALUE;
		msgObj.channel = channel;
		msgObj.senderId = player.getId();
		msgObj.senderName = player.getName();
		msgObj.origLang = player.getLanguage();
		msgObj.origText = chatText;
		msgObj.transText = null;

		if (channel == Const.ImChannel.GUILD_VALUE) {
			// TODO
			//int guildId = player.getGuildId();
			msgObj.guildId = 0;
		}

		post(msgObj);
	}

	/**
	 * 投递消息
	 */
	public void post(ImMsg msgObj) {
		if (true == GsConfig.getInstance().isTranslate()) {
			// 加入待翻译列表
			if (false == transMsgQueue.offer(msgObj)) {
				// 失败直接加入待推送列表
				enqueueMsg(msgObj);
			}
		} else {
			// 加入待推送列表
			enqueueMsg(msgObj);
		}
	}

	/**
	 * 线程主执行函数
	 */
	@Override
	public boolean onTick() {
		// 推送
		if (false == worldMsgQueue.isEmpty() || false == guildMsgQueueMap.isEmpty()) {
			HawkApp.getInstance().postCommonTask(new ImPushTask());
		}

		// 翻译
		if (false == transMsgQueue.isEmpty()) {
			HawkApp.getInstance().postCommonTask(new ImTransBatchTask());
		}

		return true;
	}

	// 内部----------------------------------------------------------------------------------------------------------

	/**
	 * IM批量翻译任务
	 */
	private class ImTransBatchTask extends HawkTask {
		@Override
		protected HawkCacheObj clone() {
			return new ImTransBatchTask();
		}

		@Override
		protected int run() {
			List<ImMsg> transList = new ArrayList<ImMsg>();

			for (int i = 0; i < TRANSLATE_BATCH_SIZE; ++i) {
				ImMsg msgObj = transMsgQueue.poll();
				if (msgObj != null) {
					transList.add(msgObj);
				} else {
					break;
				}
			}

			if (true == transList.isEmpty()) {
				return 0;
			}

			// 生成批量请求
			TranslateRequest[] requestArray = new TranslateRequest[transList.size()];
			for (int i = 0; i < transList.size(); ++i) {
				ImMsg msgObj = transList.get(i);
				msgObj.transText = new HashMap<String, String>();

				Map<String, Integer> langMap = null;
				if (msgObj.channel == Const.ImChannel.WORLD_VALUE) {
					langMap = worldLangMap;
				} else if (msgObj.channel == Const.ImChannel.GUILD_VALUE) {
					langMap = guildLangMap.get(msgObj.guildId);
				}

				requestArray[i] = new TranslateRequest();
				requestArray[i].sourceText = msgObj.origText;
				requestArray[i].sourceLang = msgObj.origLang;

				List<String> langList = new ArrayList<String>();
				for (Entry<String, Integer> entry : langMap.entrySet()) {
					if (entry.getValue() > 0) {
						langList.add(entry.getKey());
					}
				}

				requestArray[i].targetLangArray = new String[langList.size()];
				for (int j = 0; j < langList.size(); ++j) {
					requestArray[i].targetLangArray[j] = langList.get(j);
				}
			}

			// 异步翻译
			FunPlusTranslateService.getInstance().translateBatch(requestArray);

			// 结果加入待推送队列
			for (int i = 0; i < requestArray.length; ++i) {
				ImMsg msgObj = transList.get(i);
				if (requestArray[i].transTextArray != null) {
					for (int j = 0; j < requestArray[i].targetLangArray.length; ++j) {
						msgObj.transText.put(requestArray[i].targetLangArray[j], requestArray[i].transTextArray[j]);
					}
				}
				enqueueMsg(msgObj);
			}

			return 0;
		}
	}

	/**
	 * IM推送任务
	 */
	private class ImPushTask extends HawkTask {
		@Override
		protected HawkCacheObj clone() {
			return new ImPushTask();
		}

		@Override
		protected int run() {
			List<ImMsg> pushWorldList = new ArrayList<ImMsg>();
			Map<Integer, List<ImMsg>> pushGuildMap = new HashMap<>();

			for (int i = 0; i < PUSH_BATCH_SIZE; ++i) {
				ImMsg msgObj = worldMsgQueue.poll();
				if (msgObj != null) {
					pushWorldList.add(msgObj);
				} else {
					break;
				}
			}

			// TODO 根据运行情况考虑是否移除空queue
			for (Entry<Integer, ConcurrentLinkedQueue<ImMsg>> entry : guildMsgQueueMap.entrySet()) {
				Queue<ImMsg> queue = entry.getValue();
				if (true == queue.isEmpty()) {
					continue;
				}

				List<ImMsg> pushGuildList = new ArrayList<ImMsg>();
				pushGuildMap.put(entry.getKey(), pushGuildList);

				for (int i = 0; i < PUSH_BATCH_SIZE; ++i) {
					ImMsg msgObj = queue.poll();
					if (msgObj != null) {
						pushGuildList.add(msgObj);
					} else {
						break;
					}
				}
			}

			// 推送世界频道
			if (false == pushWorldList.isEmpty()) {
				// 生成每个语言builder
				Map<String, HSImPush.Builder> worldBuilderMap = new HashMap<>();
				genBuilderMap(pushWorldList, worldLangMap, worldBuilderMap);
				// 广播
				broadcast(worldPlayerMap, worldBuilderMap);
			}

			// 推送公会频道
			if (false == pushGuildMap.isEmpty()) {
				// 生成每个语言builder
				Map<Integer, Map<String, HSImPush.Builder>> guildBuilderMap = new HashMap<>();

				for (Entry<Integer, ConcurrentHashMap<String, Integer>> guildLangEntry : guildLangMap.entrySet()) {
					int guildId = guildLangEntry.getKey();
					List<ImMsg> pushGuildList = pushGuildMap.get(guildId);
					if (pushGuildList != null && pushGuildList.size() > 0) {
						Map<String, HSImPush.Builder> builderMap = new HashMap<>();
						guildBuilderMap.put(guildId, builderMap);
						genBuilderMap(pushGuildList, guildLangEntry.getValue(), builderMap);
					}
				}
				// 广播
				for(Entry<Integer, Map<String, HSImPush.Builder>> guildBuilderEntry : guildBuilderMap.entrySet()) {
					Map<String, HSImPush.Builder> builderMap = guildBuilderEntry.getValue();
					ConcurrentHashMap<Integer, ImPlayer> playerMap = guildPlayerMap.get(guildBuilderEntry.getKey());
					if (playerMap != null) {
						broadcast(playerMap, builderMap);
					}
				}
			}
			return 0;
		}
	}

	/**
	 * 消息进队列
	 */
	private void enqueueMsg(ImMsg msgObj) {
		if (msgObj.channel == Const.ImChannel.WORLD_VALUE) {
			worldMsgQueue.offer(msgObj);
		} else if (msgObj.channel == Const.ImChannel.GUILD_VALUE) {
			ConcurrentLinkedQueue<ImMsg> guildMsgQueue = null;
			synchronized (guildMsgQueueMap) {
				guildMsgQueue = guildMsgQueueMap.get(msgObj.guildId);
				if (null == guildMsgQueue) {
					guildMsgQueue = new ConcurrentLinkedQueue<>();
					guildMsgQueueMap.put(msgObj.guildId, guildMsgQueue);
				}
			}
			guildMsgQueue.offer(msgObj);
		}
	}

	/**
	 * 生成builder
	 * @param imMsgList 消息队列
	 * @param langMap 每个语言计数表
	 * @param builderMap 每个语言builder表，由每个线程提供，不需同步
	 */
	private void genBuilderMap(List<ImMsg> imMsgList, Map<String, Integer> langMap, Map<String, HSImPush.Builder> langBuilderMap) {
		if (true == GsConfig.getInstance().isTranslate()) {
			for (Entry<String, Integer> entry : langMap.entrySet()) {
				if (entry.getValue() > 0) {
					HSImPush.Builder builder = HSImPush.newBuilder();
					for (ImMsg msgObj : imMsgList) {
						builder.addImMsg(BuilderUtil.genImMsgBuilder(msgObj, entry.getKey()));
					}
					langBuilderMap.put(entry.getKey(), builder);
				}
			}
		}

		HSImPush.Builder nullLangBuilder = HSImPush.newBuilder();
		for (ImMsg msgObj : imMsgList) {
			nullLangBuilder.addImMsg(BuilderUtil.genImMsgBuilder(msgObj, GsConst.TRANSLATE_LANGUAGE_NULL));
		}
		langBuilderMap.put(GsConst.TRANSLATE_LANGUAGE_NULL, nullLangBuilder);
	}

	/**
	 * 广播聊天消息
	 * @param playerMap 玩家表，需要同步
	 * @param langBuilderMap 每个语言builder表
	 */
	private void broadcast(ConcurrentHashMap<Integer, ImPlayer> playerMap, Map<String, HSImPush.Builder> langBuilderMap) {
		Iterator<Entry<Integer, ImPlayer>> iterator = playerMap.entrySet().iterator();
		while (iterator.hasNext()) {
			Entry<Integer, ImPlayer> entry = iterator.next();
			HSImPush.Builder builder = langBuilderMap.get(entry.getValue().language);
			if (builder == null) {
				builder = langBuilderMap.get(GsConst.TRANSLATE_LANGUAGE_NULL);
			}

			entry.getValue().session.sendProtocol(HawkProtocol.valueOf(HS.code.IM_CHAT_PUSH_S, builder));

			if (false == entry.getValue().session.isActive()) {
				iterator.remove();
			}
		}
	}

}
