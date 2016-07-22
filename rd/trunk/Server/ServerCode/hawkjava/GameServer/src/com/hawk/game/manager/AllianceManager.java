package com.hawk.game.manager;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

import org.hawk.app.HawkAppObj;
import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.obj.HawkObjBase;
import org.hawk.os.HawkException;
import org.hawk.xid.HawkXID;

import com.hawk.game.GsApp;
import com.hawk.game.ServerData;
import com.hawk.game.config.AllianceCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.AwardItems;
import com.hawk.game.log.BehaviorLogger;
import com.hawk.game.log.BehaviorLogger.Action;
import com.hawk.game.log.BehaviorLogger.Params;
import com.hawk.game.log.BehaviorLogger.Source;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.AllianceInfo;
import com.hawk.game.protocol.Alliance.AllianceMember;
import com.hawk.game.protocol.Mail;
import com.hawk.game.util.AllianceUtil;
import com.hawk.game.util.GsConst;

/**
 * @author zs 
 * 公会管理器
 */

public class AllianceManager extends HawkAppObj {
	/**
	 * 全局对象, 便于访问
	 */
	private static AllianceManager instance = null;
	
	/**
	 * 获取全局实例对象
	 */
	public static AllianceManager getInstance() {
		return instance;
	}

	/**
	 * 公会管理对象 公会ID,对象
	 */
	private ConcurrentHashMap<Integer, AllianceEntity> allianceMap;

	/**
	 * 已存在公会名字
	 */
	private List<String> existName;

	/**
	 * 构造函数
	 */
	public AllianceManager(HawkXID xid) {
		super(xid);
		if (instance == null) {
			instance = this;
		}
		allianceMap = new ConcurrentHashMap<Integer, AllianceEntity>();
	}
	
	/**
	 * 数据加载
	 */
	public boolean init() {		
		List<AllianceEntity> allianceEntities = HawkDBManager.getInstance().query("from AllianceEntity where invalid = 0");
		List<PlayerAllianceEntity> allianceMembers = HawkDBManager.getInstance().query(
				"from PlayerAllianceEntity where allianceId > 0 and invalid = 0");
		if (allianceEntities.size() > 0) {
			for (AllianceEntity allianceEntity : allianceEntities) {
				AllianceCfg allianceCfg = HawkConfigManager.getInstance().getConfigByKey(AllianceCfg.class, allianceEntity.getLevel());
				if (allianceCfg == null)
					throw new NullPointerException(" AllianceCfg not find level: " + allianceEntity.getLevel() + " data !!!");
				addAlliance(allianceEntity);
			}
		}

		// 填充成员
		for (PlayerAllianceEntity playerAllianceEntity : allianceMembers) {
			AllianceEntity allianceEntity = getAlliance(playerAllianceEntity.getAllianceId());
			if (allianceEntity != null ) {
				allianceEntity.addMember(playerAllianceEntity.getPlayerId());
			}
		}
		
		existName = HawkDBManager.getInstance().query("select name from AllianceEntity");
		if (existName == null)
			existName = new ArrayList<String>();
		return true;
	}
	
	/**
	 * 消息响应
	 * 
	 * @param msg
	 * @return
	 */
	@Override
	public boolean onMessage(HawkMsg msg) {
		return super.onMessage(msg);
	}

	/**
	 * 线程主执行函数
	 */
	@Override
	public boolean onTick() {
		return true;
	}

	/**
	 * 获取所有公会
	 * @return
	 */
	public ConcurrentHashMap<Integer, AllianceEntity> getAllianceMap() {
		return allianceMap;
	}

	/**
	 * 增加公会
	 * @param allianceEntity
	 */
	public void addAlliance(AllianceEntity allianceEntity) {
		allianceMap.put(allianceEntity.getId(), allianceEntity);
	}

	/**
	 * 通过公会Id获取公会数据
	 * @param allianceId
	 * @return
	 */
	public AllianceEntity getAlliance(int allianceId) {
		return allianceMap.get(allianceId);
	}

	/**
	 * 获取所有已经存在的公会名
	 * @return
	 */
	public List<String> getExistName() {
		return existName;
	}

	/**
	 * 构造公会信息回复协议
	 * @param allianceEntity
	 * @param playerId
	 * @param remGold
	 * @return
	 */
	public AllianceInfo.Builder getAllianceInfo(AllianceEntity allianceEntity, int playerId, int remGold){	
		AllianceCfg allianceCfg = HawkConfigManager.getInstance().getConfigByKey(AllianceCfg.class, allianceEntity.getLevel());
		if(allianceCfg == null){
			throw new NullPointerException("AllianceCfg not found");
		}
		AllianceInfo.Builder ret = AllianceInfo.newBuilder();
		ret.setId(allianceEntity.getId());
		ret.setLevel(allianceEntity.getLevel());
		ret.setCurrentExp(allianceEntity.getExp());
		ret.setNextExp(0);
		ret.setCurrentPop(allianceEntity.getMemberList().size());
		ret.setMaxPop(allianceCfg.getPop());
		ret.setName(allianceEntity.getName());
		return ret;
	}
	
}


