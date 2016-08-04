package com.hawk.game.util;

import java.util.Comparator;
import java.util.HashMap;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.hawk.config.HawkConfigManager;

import com.hawk.game.config.AllianceCfg;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.protocol.Alliance.AllianceInfo;

public class AllianceUtil {

	/**
	 * 检测公会名称是否合法
	 * @param name
	 * @return
	 */
	public static boolean checkName(String name) {  
		String regEx = "^[0-9A-Za-z\u4e00-\u9fa5]{2,6}$";  
		Pattern pat = Pattern.compile(regEx);  
		Matcher mat = pat.matcher(name);  
		return mat.find();     
    }
	
	/**
	 * 公会排行比较器
	 */
	public final static Comparator<AllianceEntity> SORTALLIANCE = new Comparator<AllianceEntity>() {
		@Override
		public int compare(AllianceEntity o1, AllianceEntity o2) {
			if(o1.getId() == o2.getId()) return 0;
			if (o1.getLevel() < o2.getLevel()) {
				return 1;
			} else if (o1.getLevel() == o2.getLevel()) {
				if (o1.getExp() == o2.getExp()) {
					if (o1.getId() > o2.getId())
						return 1;
					else 
						return -1;
				}
				if (o1.getExp() < o2.getExp()) {
					return 1;
				} else {
					return -1;
				}
			} else {
				return -1;
			}
		}
	};
	
	/**
	 * 检查工会通告
	 * @param name
	 * @return
	 */	
	public static boolean checkNotice(String name) {  
		return name.length() <= GsConst.Alliance.NOTICE_MAX_LENGTH;     
      }
	
	/**
	 * 检查是否是工会Id
	 * @param name
	 * @return
	 */
	public static boolean isAllianceId(String str){ 
	    Pattern pattern = Pattern.compile("[0-9]*"); 
	    return pattern.matcher(str).matches();    
	 } 

	/**
	 * 通过公会等级获取该等级对应的最大人数
	 * @param allianceLevel
	 * @return
	 */
	public static int getAllianceMaxPop(int allianceLevel){
		AllianceCfg allianceCfg = HawkConfigManager.getInstance().getConfigByKey(AllianceCfg.class, allianceLevel);
		if (allianceCfg != null) {
			return allianceCfg.getPop();
		}
		
		return 0;
	}

	/**
	 * 构造公会信息回复协议
	 * @param allianceEntity
	 * @param playerId
	 * @param remGold
	 * @return
	 */
	public static AllianceInfo.Builder getAllianceInfo(AllianceEntity allianceEntity){	
		AllianceCfg allianceCfg = HawkConfigManager.getInstance().getConfigByKey(AllianceCfg.class, allianceEntity.getLevel());
		if(allianceCfg == null){
			throw new NullPointerException("AllianceCfg not found");
		}
		AllianceInfo.Builder ret = AllianceInfo.newBuilder();
		ret.setId(allianceEntity.getId());
		ret.setLevel(allianceEntity.getLevel());
		ret.setCurrentExp(allianceEntity.getExp());
		ret.setCurrentPop(allianceEntity.getMemberList().size());
		ret.setMaxPop(allianceCfg.getPop());
		ret.setLiveness(0);
		ret.setLivenessTotal(0);
		ret.setAnnoucement(allianceEntity.getNotice());
		ret.setName(allianceEntity.getName());
		return ret;
	}
}
