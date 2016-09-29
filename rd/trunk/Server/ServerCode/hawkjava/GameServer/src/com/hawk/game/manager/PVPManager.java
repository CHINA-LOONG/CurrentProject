package com.hawk.game.manager;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;

import org.hawk.app.HawkApp;
import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.log.HawkLog;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkRand;
import org.hawk.os.HawkTime;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.config.PVPCfg;
import com.hawk.game.entity.PVPDefenceEntity;
import com.hawk.game.entity.PVPDefenceRecordEntity;
import com.hawk.game.entity.PVPRankEntity;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Const;
import com.hawk.game.protocol.HS;
import com.hawk.game.protocol.Monster.HSMonsterDefence;
import com.hawk.game.protocol.PVP.HSPVPMatchTargetRet;
import com.hawk.game.protocol.PVP.HSPVPRankRet;
import com.hawk.game.protocol.PVP.HSPVPSettle;
import com.hawk.game.protocol.PVP.HSPVPSettleRet;
import com.hawk.game.protocol.PVP.PVPRankData;
import com.hawk.game.protocol.Status;
import com.hawk.game.util.GsConst;

public class PVPManager extends HawkAppObj {
	
	public static class PVPRoom{
		int targetId;
		PVPDefenceEntity pvpDefenceEntity;
		int startTime;
		
		public PVPRoom(int targetId, PVPDefenceEntity pvpDefenceEntity) {
			super();
			this.targetId = targetId;
			this.pvpDefenceEntity = pvpDefenceEntity;
			this.startTime = HawkTime.getSeconds();
		}
	}
	
	/**
	 * 全局对象, 便于访问
	 */
	private static PVPManager instance = null;
	
	/**
	 * 排行榜
	 */
	private List<PVPRankEntity> pvpRankList = new ArrayList<>(5000); 	
	
	/**
	 * 玩家和排行榜对应关系
	 */
	private Map<Integer, PVPRankEntity> playerRankMap = new HashMap<>();
	
	/**
	 * 匹配池
	 */
	private Map<Integer, HashMap<Integer, PVPDefenceEntity>> pvpPoolList = new HashMap<Integer, HashMap<Integer, PVPDefenceEntity>>();
	
	/**
	 * pvp房间
	 */
	private Map<Integer, PVPRoom> pvpRoomList = new HashMap<>();
	
	/**
	 * 排行榜刷新时间
	 */
	private long refreshTime = 0;
	
	private HSPVPRankRet.Builder pvpRankBuilder = null;
	
	/**
	 * 获取全局实例对象
	 */
	public static PVPManager getInstance() {
		return instance;
	}
	
	/**
	 * 构造函数
	 */
	public PVPManager(HawkXID xid) {
		super(xid);
		if (instance == null) {
			instance = this;
		}
	}
	
	/**
	 * 数据加载
	 */
	public boolean init() {	
		List<PVPRankEntity> PVPRankEntities = HawkDBManager.getInstance().query("from PVPRankEntity where invalid = 0 and point != 0 order by point desc");
		if (PVPRankEntities.isEmpty() == false) {
			int rank = 0;
			for (PVPRankEntity pvpRankEntity : PVPRankEntities) {
				pvpRankEntity.setRank(rank++);
				pvpRankList.add(pvpRankEntity);
				playerRankMap.put(pvpRankEntity.getPlayerId(), pvpRankEntity);
			}
		}
		
		for (PVPCfg pvpCfg : HawkConfigManager.getInstance().getConfigList(PVPCfg.class)) {
			pvpPoolList.put(pvpCfg.getStage(), new HashMap<Integer, PVPDefenceEntity>());
		}
		
		generatePVPPool();
		
		return true;
	}
	
	@Override
	public boolean onMessage(HawkMsg msg) {
		if(msg.getMsg() == GsConst.MsgType.PVP_MATCH_TARGET){
			onPVPMatch(msg);
			return true;
		}
		else if (msg.getMsg() == GsConst.MsgType.PVP_SETTLE) {
			onPVPSettle(msg);
			return true;
		}
		else if (msg.getMsg() == GsConst.MsgType.PVP_RANK_LIST) {
			onPVPRankList(msg);
			return true;
		}
		
		return super.onMessage(msg);
	}
	
	/**
	 * 重置匹配池
	 */
	public boolean generatePVPPool(){
		int endIndex = pvpRankList.size() - 1;
		int beginIndex = 0;
		HashSet<Integer> randomPlayers = new HashSet<Integer>();
		for (PVPCfg pvpCfg : HawkConfigManager.getInstance().getConfigList(PVPCfg.class)) {
			pvpPoolList.get(pvpCfg.getStage()).clear();
			randomPlayers.clear();
			beginIndex = endIndex;
			for (int i = endIndex; i >= 0; i--) {
				if (pvpCfg.getPoint() != 0 && pvpRankList.get(i).getPoint() > pvpCfg.getPoint()) {
					break;
				}

				beginIndex = i - 1;
			}
			
			// 选取种子用户
			if (endIndex - beginIndex > 2 * GsConst.PVP.PVP_POOL_STAGE_INIT_SIZE) {
				while (randomPlayers.size() < GsConst.PVP.PVP_POOL_STAGE_INIT_SIZE) {
					try {
						randomPlayers.add(HawkRand.randInt(beginIndex + 1, endIndex));
					} catch (Exception e) {
						
					}
				}
			}
			else if (endIndex - beginIndex > GsConst.PVP.PVP_POOL_STAGE_INIT_SIZE) {
				for (int i = beginIndex + 1; i < beginIndex + GsConst.PVP.PVP_POOL_STAGE_INIT_SIZE; i++) {
					randomPlayers.add(i);
				}
			}
			else if (endIndex - beginIndex > 0){
				for (int i = beginIndex + 1; i <= endIndex; i++) {
					randomPlayers.add(i);
				}
			}
			
			Map<Integer, PVPDefenceEntity> stageList = pvpPoolList.get(pvpCfg.getStage());
			for (int index : randomPlayers) {
				PVPDefenceEntity pvpDefenceEntity = getPVPDefenceEntity(pvpRankList.get(index).getPlayerId(), pvpRankList.get(index).getName(), pvpRankList.get(index).getLevel());
				stageList.put(pvpRankList.get(index).getPlayerId(), pvpDefenceEntity);
			}
			
			endIndex = beginIndex;
		}
			
		return true;
	}
	
	public void generatePVPRankBuilder(){
		pvpRankBuilder = HSPVPRankRet.newBuilder();
		
		int lastRank = 1;
		int lastPoint = 0;
		for (int i = 0; i < pvpRankList.size(); i++) {
			if (i == 0) {
				lastPoint = pvpRankList.get(0).getPoint();
			}

			PVPRankEntity pvpRankEntity = pvpRankList.get(i);
			PVPRankData.Builder pvpRankData = PVPRankData.newBuilder();
			pvpRankData.setLevel(pvpRankEntity.getLevel());
			pvpRankData.setPoint(pvpRankEntity.getPoint());
			pvpRankData.setName(pvpRankEntity.getName());
			
			// 设置排名 有并列情况
			if (pvpRankList.get(i).getPoint() == lastPoint) {
				pvpRankData.setRank(lastRank);
			}
			else{
				if (pvpRankBuilder.getPvpRankListCount() >= GsConst.PVP.PVP_RANK_SIZE) {
					break;
				}
				
				pvpRankData.setRank(i + 1);
				lastPoint = pvpRankList.get(i).getPoint();
				lastRank = i + 1;
			}
			
			pvpRankBuilder.addPvpRankList(pvpRankData);
		}
	}
	
	/**
	 * pvp匹配
	 * @param msg
	 */
	public void onPVPMatch(HawkMsg msg){
		Player player = msg.getParam(0);	
		if (!player.getPlayerData().getPVPDefenceEntity().hasSetDefenceMonsters()) {
			player.sendError(HS.code.PVP_MATCH_TARGET_C_VALUE, Status.pvpError.PVP_NOT_SET_DEFENCE_VALUE);
			return ;
		}
		
		PVPCfg pvpCfg = null;
		PVPRankEntity rankEntity = playerRankMap.get(player.getId());
		if (rankEntity == null) {
			rankEntity = player.getPlayerData().loadPVPRankData();
			// 进入匹配池
			pvpCfg = PVPCfg.getPVPStageCfg(rankEntity.getPoint());
			if (pvpCfg == null) {
				player.sendError(HS.code.PVP_MATCH_TARGET_C_VALUE, Status.error.CONFIG_NOT_FOUND_VALUE);
				return;
			}
			
			rankEntity.setRank(pvpRankList.size());
			// 进入排行榜
			pvpRankList.add(rankEntity);
			reRank(rankEntity);
			playerRankMap.put(player.getId(), rankEntity);
		}
		
		// 读取配置
		if (pvpCfg == null) {
			pvpCfg = PVPCfg.getPVPStageCfg(rankEntity.getPoint());
			if (pvpCfg == null) {
				player.sendError(HS.code.PVP_MATCH_TARGET_C_VALUE, Status.error.CONFIG_NOT_FOUND_VALUE);
				return;
			}
		}
			
		Map<Integer, PVPDefenceEntity> stageList = pvpPoolList.get(pvpCfg.getStage());
		
		if (!stageList.containsKey(player.getId())) {
			// TODO 检查是否会影响player的释放
			addToPool(stageList, player.getPlayerData().getPVPDefenceEntity(), rankEntity);
		}
		
		// 匹配玩家
		PVPDefenceEntity target = null;
		while (true) {	
			int i = pvpCfg.getStage();
			for (; i > 0; i--) {
				stageList = pvpPoolList.get(i);
				if (stageList != null && (stageList.containsKey(player.getId()) && stageList.size() > 1) || (!stageList.containsKey(player.getId()) && stageList.size() > 0)) {
					break;
				}
			}
			
			if (stageList == null || stageList.size() <= 0) {
				player.sendError(HS.code.PVP_MATCH_TARGET_C_VALUE, Status.pvpError.PVP_NOT_MATCH_TARGET_VALUE);
				return;
			}			
			
			target = stageList.get(randomPlayer(stageList));
			
			if(target.getPlayerId() != player.getId()){
				// 如果没有设置防守阵容 ，从匹配池中移除
				if (target.hasSetDefenceMonsters() == false) {
					stageList.remove(target.getPlayerId());
				}
				else {
					break;
				}
			}
			
			if (((stageList.containsKey(player.getId()) && stageList.size() == 1) || stageList.isEmpty()) && i == 1) {
				player.sendError(HS.code.PVP_MATCH_TARGET_C_VALUE, Status.pvpError.PVP_NOT_MATCH_TARGET_VALUE);
				return;
			}
		}
		
		// 加入到房间
		pvpRoomList.put(player.getId(), new PVPRoom(target.getPlayerId(), target));
		
		HSPVPMatchTargetRet.Builder response = HSPVPMatchTargetRet.newBuilder();
		response.setPlayerId(target.getPlayerId());
		response.setName(target.getName());
		response.setLevel(target.getLevel());
		response.setRank(0);
		response.setDefenceData(target.getMonsterDefenceBuilder());
		
		player.sendProtocol(HawkProtocol.valueOf(HS.code.PVP_MATCH_TARGET_S_VALUE, response));
	}
	
	/**
	 * pvp结算
	 * @param msg
	 */
	public void onPVPSettle(HawkMsg msg){
		Player player = msg.getParam(0);
		HawkProtocol cmd = msg.getParam(1);
		
		PVPRoom pvpRoom = pvpRoomList.get(player.getId());
		if (pvpRoom == null) {
			player.sendError(HS.code.PVP_SETTLE_C_VALUE, Status.pvpError.PVP_NOT_MATCH_BEFORE_VALUE);
			return;
		}
		
		pvpRoomList.remove(player.getId());
		
		PVPRankEntity pvpRankEntity = playerRankMap.get(player.getId());
		PVPRankEntity targetRankEntity = playerRankMap.get(pvpRoom.targetId);
		int selfPoint = pvpRankEntity != null ? pvpRankEntity.getPoint() : GsConst.PVP.PVP_DEFAULT_POINT;
		int targetPoint = targetRankEntity != null ? targetRankEntity.getPoint() : GsConst.PVP.PVP_DEFAULT_POINT;
		
		PVPCfg pvpCfg = PVPCfg.getPVPStageCfg(selfPoint);
		if (pvpCfg == null) {
			player.sendError(HS.code.PVP_SETTLE_C_VALUE, Status.error.CONFIG_NOT_FOUND_VALUE);
			return;
		}
		
		HSPVPSettle protocol = cmd.parseProtocol(HSPVPSettle.getDefaultInstance());
		int changePoint = calculatePVPPoint(selfPoint, targetPoint, pvpCfg, protocol.getResult());
		if (pvpRankEntity != null) {	
			changePoolStage(pvpRankEntity, changePoint);
			pvpRankEntity.setPoint(pvpRankEntity.getPoint() + changePoint);
			pvpRankEntity.notifyUpdate(true);
			reRank(pvpRankEntity);
			
			if (protocol.getResult() == Const.PvpResult.WIN_VALUE) {
				player.getPlayerData().getPlayerEntity().addHonorPoint(pvpCfg.getWin());
				player.getPlayerData().getPlayerEntity().notifyUpdate(true);
			}
			else if (protocol.getResult() == Const.PvpResult.DRAW_VALUE) {
				player.getPlayerData().getPlayerEntity().addHonorPoint(pvpCfg.getDraw());
				player.getPlayerData().getPlayerEntity().notifyUpdate(true);
			}	
			
			PVPDefenceRecordEntity pvpDefenceRecordEntity = new PVPDefenceRecordEntity();
			pvpDefenceRecordEntity.setPlayerId(pvpRoom.targetId);
			pvpDefenceRecordEntity.setPoint(-changePoint);
			pvpDefenceRecordEntity.setResult(Const.PvpResult.NUM_VALUE - protocol.getResult());
			pvpDefenceRecordEntity.setAttackerGrade(pvpCfg.getStage());
			pvpDefenceRecordEntity.setAttackerName(pvpRankEntity.getName());
			pvpDefenceRecordEntity.setAttackerLevel(pvpRankEntity.getLevel());
			pvpDefenceRecordEntity.notifyCreate();
			
	 		HawkMsg recordMsg = HawkMsg.valueOf(GsConst.MsgType.PVP_RECORD, HawkXID.valueOf( GsConst.ObjType.PLAYER, pvpRoom.targetId));
	 		recordMsg.pushParam(pvpDefenceRecordEntity);
			HawkApp.getInstance().postMsg(recordMsg);
		}
		
		if (targetRankEntity != null) {
			changePoolStage(targetRankEntity, -changePoint);
			targetRankEntity.setPoint(targetRankEntity.getPoint() - changePoint);
			targetRankEntity.notifyUpdate(true);
			reRank(targetRankEntity);
		}
		
		HSPVPSettleRet.Builder response = HSPVPSettleRet.newBuilder();
		response.setPoint(pvpRankEntity.getPoint());
		response.setRank(pvpRankEntity.getRank());
		player.sendProtocol(HawkProtocol.valueOf(HS.code.PVP_SETTLE_S_VALUE, response));
	}
	
	/**
	 * pvp排行榜
	 * @param msg
	 */
	public void onPVPRankList(HawkMsg msg){
		Player player = msg.getParam(0);
		player.sendProtocol(HawkProtocol.valueOf(HS.code.PVP_RANK_LIST_S_VALUE, pvpRankBuilder));	
	}
	
	/**
	 * 添加到匹配池
	 * @param stageList
	 * @param defenceEntity
	 * @param rankEntity
	 */
	public void addToPool(Map<Integer, PVPDefenceEntity> stageList, PVPDefenceEntity defenceEntity, PVPRankEntity rankEntity){
		if (stageList.size() >= GsConst.PVP.PVP_POOL_STAGE_MAX_SIZE) {
			stageList.remove(randomPlayer(stageList));
			stageList.put(defenceEntity.getPlayerId(), defenceEntity);
		}
		else {
			stageList.put(rankEntity.getPlayerId(), defenceEntity);
		}
	}
	
	/**
	 * 随机出一个玩家
	 * @param stageList
	 * @return
	 */
	public int randomPlayer(Map<Integer, PVPDefenceEntity> stageList){
		if (stageList.size() == 0) {
			return 0;
		}
		
		Object[] playerArray = stageList.keySet().toArray();
		int randomIndex = 0;
		try {
			randomIndex = HawkRand.randInt(0, playerArray.length - 1);
		} catch (Exception e) {
			randomIndex = 0;
		}
		
		return (int)(playerArray[randomIndex]);
	}
	
	/**
	 * 更新匹配池中的位置
	 * @param rankEntity
	 */
	public void changePoolStage(PVPRankEntity rankEntity, int changePoint){	
		PVPCfg currentPvpCfg = PVPCfg.getPVPStageCfg(rankEntity.getPoint());
		if (currentPvpCfg == null) {
			return;
		}
		
		PVPCfg nextPvpCfg = PVPCfg.getPVPStageCfg(rankEntity.getPoint() + changePoint);
		if (currentPvpCfg == nextPvpCfg) {
			return;
		}
		else if (pvpPoolList.get(currentPvpCfg.getStage()).containsKey(rankEntity.getPlayerId())) {
			PVPDefenceEntity defenceEntity = pvpPoolList.get(currentPvpCfg.getStage()).remove(rankEntity.getPlayerId());
			Map<Integer, PVPDefenceEntity> stageList = pvpPoolList.get(nextPvpCfg.getStage());
			if (stageList.size() >= GsConst.PVP.PVP_POOL_STAGE_MAX_SIZE) {
				stageList.remove(randomPlayer(stageList));
			}
			stageList.put(defenceEntity.getPlayerId(), defenceEntity);
		}
	}
	
	/**
	 * 重新排名
	 * @param rankEntity
	 */
	public void reRank(PVPRankEntity rankEntity){
		int rank = rankEntity.getRank();
		if (rank > 0 && rankEntity.getPoint() > pvpRankList.get(rank - 1).getPoint()) {
			for (int i = rank - 1; i >= 0; i--) {
				if (pvpRankList.get(i).getPoint() < rankEntity.getPoint()) {
					pvpRankList.get(i).addRank();
					pvpRankList.set(i + 1, pvpRankList.get(i));
				}
				else {
					pvpRankList.set(i + 1, rankEntity);
					rankEntity.setRank(i + 1);
					break;
				}
				
				if (i == 0) {
					pvpRankList.set(0, rankEntity);
					rankEntity.setRank(0);
				}
			}
		}
		else if (rank < pvpRankList.size() - 1 && rankEntity.getPoint() < pvpRankList.get(rank + 1).getPoint()){
			for (int i = rank + 1; i < pvpRankList.size(); i++) {
				if (pvpRankList.get(i).getPoint() > rankEntity.getPoint()) {
					pvpRankList.get(i).removeRank();
					pvpRankList.set(i - 1, pvpRankList.get(i));
				}
				else {
					pvpRankList.set(i - 1, rankEntity);
					rankEntity.setRank(i - 1);
					break;
				}
				
				if (i == pvpRankList.size() - 1) {
					pvpRankList.set(pvpRankList.size() - 1, rankEntity);
					rankEntity.setRank(pvpRankList.size() - 1);
				}
			}
		}
	}
	
	/**
	 * ELO 算法计算得分
	 * @param point
	 * @param targetPoint
	 * @param pvpCfg
	 * @param result
	 * @return
	 */
	public int calculatePVPPoint(int point, int targetPoint, PVPCfg pvpCfg, int result){
		double finalResult = 0;
		if (result == Const.PvpResult.WIN_VALUE) {
			finalResult = pvpCfg.getK() * (1 - (1.0 / (1 + Math.pow(10, (targetPoint - point) / 400.0))));
		}
		else if (result == Const.PvpResult.LOSE_VALUE) {
			finalResult = pvpCfg.getK() * (0.0f - (1.0 / (1 + Math.pow(10, (targetPoint - point) / 400.0))));
		}
		else if (result == Const.PvpResult.DRAW_VALUE) {
			finalResult = pvpCfg.getK() * (0.5 - (1.0 / (1 + Math.pow(10, (targetPoint - point) / 400.0))));
		}
		
		return (int)finalResult;
	}
	
	/**
	 * 获取pvp防守数据
	 */
	public synchronized PVPDefenceEntity getPVPDefenceEntity(int playerId, String nickName, int level){
		PVPDefenceEntity pvpDefenceEntity = null;
		PVPRankEntity rankEntity = playerRankMap.get(playerId);
		if (rankEntity != null) {
			PVPCfg pvpCfg = PVPCfg.getPVPStageCfg(rankEntity.getPoint());
			pvpDefenceEntity = pvpPoolList.get(pvpCfg.getStage()).get(playerId);
		}
		
		Player player = GsApp.getInstance().queryPlayer(playerId);
		if (player != null) {
			pvpDefenceEntity = player.getPlayerData().getPVPDefenceEntity();
		}
		
		if (pvpDefenceEntity == null) {
			List<PVPDefenceEntity> resultList = HawkDBManager.getInstance().query("from PVPDefenceEntity where playerId = ? and invalid = 0", playerId);
			if (resultList != null && resultList.size() > 0) {
				pvpDefenceEntity = resultList.get(0);
				pvpDefenceEntity.decode();
			}
			
			if (pvpDefenceEntity == null) {
				pvpDefenceEntity = new PVPDefenceEntity();
				pvpDefenceEntity.setPlayerId(playerId);
				pvpDefenceEntity.setName(nickName);
				pvpDefenceEntity.setLevel(level);
				pvpDefenceEntity.setMonsterDefenceBuilder(HSMonsterDefence.newBuilder());
				pvpDefenceEntity.notifyCreate();
			}
		}
		
		return pvpDefenceEntity;
	}
	
	/**
	 * 获取pvp排名数据
	 * @param playerId
	 * @return
	 */
	public PVPRankEntity getPVPRankEntity(int playerId){
		return playerRankMap.get(playerId); 
	}
	
	@Override
	public boolean onTick(long tickTime) {
		if (refreshTime + 5000 < tickTime) {
			refreshTime = tickTime;
			
			HawkLog.logPrintln("排行榜:");
			for (PVPRankEntity rankEntity : pvpRankList) {
				HawkLog.logPrintln(rankEntity.getName() + " " + rankEntity.getPlayerId() + " " + rankEntity.getRank() + " " + rankEntity.getPoint());
			}
			
			HawkLog.logPrintln("匹配池:");
			for (Map.Entry<Integer, HashMap<Integer, PVPDefenceEntity>> stageList : pvpPoolList.entrySet()) {
				HawkLog.logPrintln("stage: " + stageList.getKey());
				for (PVPDefenceEntity pvpDefenceEntity : stageList.getValue().values()) {
					HawkLog.logPrintln(pvpDefenceEntity.getName() + " " + pvpDefenceEntity.getPlayerId() + " ");
				}
			}
			
			generatePVPRankBuilder();
		}
	
		return true;
	}
}
