package com.hawk.game.util;

import java.util.Comparator;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.hawk.config.HawkConfigManager;
import org.hawk.os.HawkTime;

import com.hawk.game.config.SociatyBaseCfg;
import com.hawk.game.config.SociatyTechnologyCfg;
import com.hawk.game.entity.AllianceApplyEntity;
import com.hawk.game.entity.AllianceEntity;
import com.hawk.game.entity.AllianceTeamEntity;
import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.item.ItemInfo;
import com.hawk.game.manager.AllianceManager;
import com.hawk.game.player.Player;
import com.hawk.game.protocol.Alliance.AllianceApply;
import com.hawk.game.protocol.Alliance.AllianceInfo;
import com.hawk.game.protocol.Alliance.AllianceMember;
import com.hawk.game.protocol.Alliance.AllianceSimpleInfo;
import com.hawk.game.protocol.Alliance.AllianceTeamInfo;
import com.hawk.game.protocol.Alliance.AllianceTeamMemInfo;
import com.hawk.game.protocol.Alliance.AllianceTeamQuestInfo;
import com.hawk.game.protocol.Alliance.HSAllianceApplyListRet;
import com.hawk.game.protocol.Alliance.HSAllianceContributionRet;
import com.hawk.game.protocol.Alliance.HSAllianceMembersRet;
import com.hawk.game.protocol.Alliance.HSAllianceSelfTeamRet;
import com.hawk.game.protocol.Alliance.HSAllianceTeamListRet;

public class AllianceUtil {

	/**
	 * 检测公会名称是否合法
	 * @param name
	 * @return
	 */
	public static boolean checkName(String name) {  
		int chineseLength = getChineseCount(name);
		int length = chineseLength * 2 + name.length() - chineseLength;
		if (length > GsConst.Alliance.NAME_MAX_LENGTH) {
			return false;
		}
		return true;
    }

	/**
	 * 检测公会通告是否合法
	 * @param name
	 * @return
	 */
	public static boolean checkNotice(String notice) {  
		int chineseLength = getChineseCount(notice);
		int length = chineseLength * 2 + notice.length() - chineseLength;
		if (length > GsConst.Alliance.NOTICE_MAX_LENGTH) {
			return false;
		}
		return true;
    }
	
	/**
	 * 检测汉子字符
	 * @param content
	 * @return
	 */
	public static int getChineseCount(String content){
		int count = 0;      
        String regEx = "[\\u4e00-\\u9fa5]";   
        Pattern p = Pattern.compile(regEx);      
        Matcher m = p.matcher(content);      
        while (m.find()) {      
           for (int i = 0; i <= m.groupCount(); i++) {      
                count = count + 1;      
            }      
        }      
		
        return count;
	}
	
	/**
	 * 统计副会长数量
	 * @param name
	 * @return
	 */
	public static int getCopyMainCount(AllianceEntity alliance) {  
		int count = 0;
		for (PlayerAllianceEntity playerAlliance : alliance.getMemberList().values()) {
			if (playerAlliance.getPostion() == GsConst.Alliance.ALLIANCE_POS_COPYMAIN) {
				count ++;
			}
		}
		
		return count;
    }
	
	/**
	 * 检测公会金币加成
	 * @param name
	 * @return
	 */
	public static float getAllianceCoinRatio(Player player){
		if (player.getAllianceId() == 0) {
			return 0;
		}
		
		AllianceEntity allianeEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianeEntity == null || allianeEntity.getCoinLevel() == 0) {
			return 0;
		}
		
		SociatyTechnologyCfg techCfg = SociatyTechnologyCfg.getSociatyTechnologyCfg(GsConst.Alliance.ALLIANCE_TEC_COIN, allianeEntity.getCoinLevel());
		if (techCfg == null) {
			return 0;
		}
		
		return techCfg.getGainCoin();
	}
	
	/**
	 * 检测公会经验药品加成
	 * @param name
	 * @return
	 */
	public static ItemInfo getAllianceExp(Player player){
		if (player.getAllianceId() == 0) {
			return null;
		}
		
		AllianceEntity allianeEntity = AllianceManager.getInstance().getAlliance(player.getAllianceId());
		if (allianeEntity == null || allianeEntity.getCoinLevel() == 0) {
			return null;
		}
		
		SociatyTechnologyCfg techCfg = SociatyTechnologyCfg.getSociatyTechnologyCfg(GsConst.Alliance.ALLIANCE_TEC_EXP, allianeEntity.getExpLevel());
		if (techCfg == null || techCfg.getGainExp() == null || techCfg.getGainExp().equals("")) {
			return null;
		}
		
		return ItemInfo.valueOf(techCfg.getGainExp(), GsConst.ItemParseType.PARSE_DEFAULT);
	}
	
	/**
	 * 获取驻兵配置
	 * @param bp
	 * @return
	 */
	public static SociatyBaseCfg getAllianceBaseConfig(int bp){
		List<SociatyBaseCfg> sociatyBaseList = HawkConfigManager.getInstance().getConfigList(SociatyBaseCfg.class);
		int i = 0;
		for (; i < sociatyBaseList.size(); i++) {
			if (bp < sociatyBaseList.get(i).getBpMax()) {
				break;
			}
		}
		
		// 超过上限使用最后一个值
		if (i == sociatyBaseList.size()) {
			i = sociatyBaseList.size() - 1;
		}
		
		return sociatyBaseList.get(i);
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
				if (o1.getContribution() == o2.getContribution()) {
					if (o1.getId() > o2.getId())
						return 1;
					else 
						return -1;
				}
				if (o1.getContribution() < o2.getContribution()) {
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
	 * 检查是否是工会Id
	 * @param name
	 * @return
	 */
	public static boolean isAllianceId(String str){ 
		if (str.length() == 5 || str.length() == 6) {
			  Pattern pattern = Pattern.compile("[0-9]*"); 
			    return pattern.matcher(str).matches();  
		}
		return false;
	 } 

	/**
	 * 构造公会信息回复协议
	 * @param allianceEntity
	 * @param playerId
	 * @param remGold
	 * @return
	 */
	public static AllianceInfo.Builder getAllianceInfo(AllianceEntity allianceEntity){	
		AllianceInfo.Builder builder = AllianceInfo.newBuilder();
		builder.setId(allianceEntity.getId());
		builder.setLevel(allianceEntity.getLevel());
		builder.setCurrentPop(allianceEntity.getMemberList().size());
		builder.setMaxPop(SociatyTechnologyCfg.getMemberPop(allianceEntity.getMemLevel()));
		builder.setContribution(allianceEntity.getContribution());
		builder.setContributionToday(allianceEntity.getContribution0());
		builder.setContribution3Day(allianceEntity.get3DaysContribution());
		builder.setNotice(allianceEntity.getNotice());
		builder.setExpLevel(allianceEntity.getExpLevel());
		builder.setMemLevel(allianceEntity.getMemLevel());
		builder.setCoinLevel(allianceEntity.getCoinLevel());
		builder.setName(allianceEntity.getName());
		builder.setAutoAccept(allianceEntity.isAutoAccept());
		builder.setMinLevel(allianceEntity.getMinLevel());
		builder.setCaptainId(allianceEntity.getPlayerId());
		builder.setCaptaionName(allianceEntity.getPlayerName());
		return builder;
	}

	/**
	 * 构造公会信息回复协议
	 * @param allianceEntity
	 * @param playerId
	 * @param remGold
	 * @return
	 */
	public static AllianceSimpleInfo.Builder getSimpleAllianceInfo(AllianceEntity allianceEntity, boolean isApply){	
		AllianceSimpleInfo.Builder builder = AllianceSimpleInfo.newBuilder();
		builder.setId(allianceEntity.getId());
		builder.setLevel(allianceEntity.getLevel());
		builder.setCurrentPop(allianceEntity.getMemberList().size());
		builder.setMaxPop(SociatyTechnologyCfg.getMemberPop(allianceEntity.getMemLevel()));
		builder.setContribution3Day(allianceEntity.get3DaysContribution());
		builder.setNotice(allianceEntity.getNotice());
		builder.setName(allianceEntity.getName());
		builder.setMinLevel(allianceEntity.getMinLevel());
		builder.setCaptainId(allianceEntity.getPlayerId());
		builder.setCaptaionName(allianceEntity.getPlayerName());
		builder.setApply(isApply);
		builder.setAutoAccept(allianceEntity.isAutoAccept());
		return builder;
	}
	
	/**
	 * 获取成员数据
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static AllianceMember.Builder getMemberInfo(PlayerAllianceEntity playerAllianceEntity, PlayerAllianceEntity selfEntity) {
		AllianceMember.Builder builder = AllianceMember.newBuilder();
		builder.setLevel(playerAllianceEntity.getLevel());
		builder.setName(playerAllianceEntity.getName());
		builder.setBattlePoint(0);
		builder.setContribution(playerAllianceEntity.getContribution());
		builder.setTotalContribution(playerAllianceEntity.getTotalContribution());
		builder.setPostion(playerAllianceEntity.getPostion());
		builder.setId(playerAllianceEntity.getPlayerId());
		builder.setLoginTime(playerAllianceEntity.getLoginTime());
		builder.setLogoutTime(playerAllianceEntity.getLogoutTime());
		if (selfEntity != null && selfEntity.getRefreshTime() > HawkTime.getSeconds() && selfEntity.getFatigueSet().contains(playerAllianceEntity.getPlayerId())) {
			builder.setSendFatigue(true);
		}
		else
		{
			builder.setSendFatigue(false);
		}
		return builder;
	}
	
	/**
	 * 构造公会成员回复协议
	 * @return 
	 */
	public static HSAllianceMembersRet.Builder getAllianceMembersInfo(AllianceEntity allianceEntity, PlayerAllianceEntity selfEntity, int filterId) {
		HSAllianceMembersRet.Builder builder = HSAllianceMembersRet.newBuilder();
		for (PlayerAllianceEntity eneity : allianceEntity.getMemberList().values()) {
			if (eneity.getPlayerId() != filterId) {
				builder.addMemberList(AllianceUtil.getMemberInfo(eneity, selfEntity));
			}
		}
		
		return builder;
	}
	
	/**
	 * 构造公会贡献值回复协议
	 * @return 
	 */
	public static HSAllianceContributionRet.Builder getAllianceContributionInfo(AllianceEntity allianceEntity) {
		HSAllianceContributionRet.Builder builder = HSAllianceContributionRet.newBuilder();
		builder.setContribution(allianceEntity.getContribution());
		builder.setContributionToday(allianceEntity.getContribution0());
		builder.setContribution3Day(allianceEntity.get3DaysContribution());
		return builder;
	}
	
	/**
	 * 申请同步通知
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static AllianceApply.Builder getApplyNotify(AllianceApplyEntity applyEntity) {
		AllianceApply.Builder builder = AllianceApply.newBuilder();
		builder.setAllianceId(applyEntity.getAllianceId());
		builder.setPlayerId(applyEntity.getPlayerId());
		builder.setNickname(applyEntity.getName());
		builder.setLevel(applyEntity.getLevel());
		return builder;
	}
	
	/**
	 * 工会队伍列表
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static HSAllianceTeamListRet.Builder getTeamList(AllianceEntity allianceEntity) {
		HSAllianceTeamListRet.Builder builder = HSAllianceTeamListRet.newBuilder();
		for (AllianceTeamEntity teamEntity : allianceEntity.getUnfinishTeamList().values()) {
			builder.addAllianceTeams(getTeamInfo(teamEntity, allianceEntity, false));
		}
		return builder;
	}
	
	/**
	 * 工会队伍
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static AllianceTeamInfo.Builder getTeamInfo(AllianceTeamEntity teamEntity, AllianceEntity allianceEntity, boolean questInfo) {
		AllianceTeamInfo.Builder element = AllianceTeamInfo.newBuilder();
		element.setStartTime(teamEntity.getCreateTime());
		element.setTaskId(teamEntity.getTaskId());
		element.setTeamId(teamEntity.getId());
		
		if (teamEntity.getCaptain() != 0) {
			AllianceTeamMemInfo.Builder member = getTeamMemberInfo(teamEntity.getCaptain(), allianceEntity);
			member.setIsCaptain(true);
			element.addMembers(member);
		}
		
		if (teamEntity.getMember1() != 0) {
			element.addMembers(getTeamMemberInfo(teamEntity.getMember1(), allianceEntity));
		}
		
		if (teamEntity.getMember2() != 0) {
			element.addMembers(getTeamMemberInfo(teamEntity.getMember2(), allianceEntity));
		}
		
		if (teamEntity.getMember3() != 0) {
			element.addMembers(getTeamMemberInfo(teamEntity.getMember3(), allianceEntity));
		}
		
		if (questInfo == true) {
			element.addQuestInfos(getTaskInfo(teamEntity.getItemQuest1PlayerId(), teamEntity.getItemQuest1(), teamEntity, allianceEntity));
			element.addQuestInfos(getTaskInfo(teamEntity.getItemQuest2PlayerId(), teamEntity.getItemQuest2(), teamEntity, allianceEntity));
			element.addQuestInfos(getTaskInfo(teamEntity.getItemQuest3PlayerId(), teamEntity.getItemQuest3(), teamEntity, allianceEntity));
			element.addQuestInfos(getTaskInfo(teamEntity.getCoinQuest1PlayerId(), teamEntity.getCoinQuest1(), teamEntity, allianceEntity));
			element.addQuestInfos(getTaskInfo(teamEntity.getCoinQuest2PlayerId(), teamEntity.getCoinQuest2(), teamEntity, allianceEntity));
			element.addQuestInfos(getTaskInfo(teamEntity.getInstanceQuest1PlayerId(), teamEntity.getInstanceQuest1(), teamEntity, allianceEntity));
		}
		
		return element;
	}
	
	/**
	 * 工会任务信息
	 * @param playerId
	 * @param questId
	 * @param teamEntity
	 * @param allianceEntity
	 * @return
	 */
	public static AllianceTeamQuestInfo.Builder getTaskInfo(int playerId, int questId, AllianceTeamEntity teamEntity, AllianceEntity allianceEntity){
		AllianceTeamQuestInfo.Builder questBuilder = AllianceTeamQuestInfo.newBuilder();
		questBuilder.setQuestId(questId);
		questBuilder.setPlayerId(playerId);
		if (playerId != 0 && !isMemberInTeam(playerId, teamEntity) ) {
			PlayerAllianceEntity playerEntity = allianceEntity.getMember(teamEntity.getItemQuest1PlayerId());
			if (playerEntity != null) {
				questBuilder.setNickname(playerEntity.getName());
				questBuilder.setLevel(playerEntity.getLevel());
				questBuilder.setAvatar(0);
			}
		}	
		return questBuilder;
	}
	
	/**
	 * 工会队伍队员
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static AllianceTeamMemInfo.Builder getTeamMemberInfo(int playerId, AllianceEntity allianceEntity) {
		AllianceTeamMemInfo.Builder member = AllianceTeamMemInfo.newBuilder();
		PlayerAllianceEntity playerEntity = allianceEntity.getMember(playerId);
		if (playerEntity != null) {
			member.setPlayerId(playerId);
			member.setNickname(playerEntity.getName());
			member.setLevel(playerEntity.getLevel());
			member.setAvatar(0);
			member.setIsCaptain(false);
		}	
		return member;
	}
	
	/**
	 * 所有队伍
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static HSAllianceApplyListRet.Builder getApplyList(AllianceEntity allianceEntity) {
		HSAllianceApplyListRet.Builder builder = HSAllianceApplyListRet.newBuilder();
		for (AllianceApplyEntity applyEntity : allianceEntity.getApplyList().values()) {
			builder.addApply(getApplyNotify(applyEntity));
		}
		return builder;
	}
	
	/**
	 * 自身组队信息
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static HSAllianceSelfTeamRet.Builder getSelfTeamInfo(AllianceTeamEntity teamEntity, AllianceEntity allianceEntity) {
		HSAllianceSelfTeamRet.Builder builder = HSAllianceSelfTeamRet.newBuilder();
		builder.setSelfTeam(getTeamInfo(teamEntity, allianceEntity, true));
		return builder;
	}
	
	/**
	 * 自身组队信息
	 * @param allianceId
	 * @param player
	 * @return
	 */
	public static boolean isMemberInTeam(int playerId, AllianceTeamEntity teamEntity){
		if (teamEntity.getCaptain() != playerId && teamEntity.getMember1() != playerId && teamEntity.getMember2() != playerId && teamEntity.getMember3() != playerId) {
			return false;
		}
		
		return true;
	}

	/**
	 * 公会贡献值每日领奖
	 */
	public static boolean isAllianceContriRewardDaily(int index) {
		if (index == 0) {
			return (index & 1) != 0;
		} else if (index == 1) {
			return (index & 2) != 0;
		} else if (index == 2) {
			return (index & 4) != 0;
		}
		return false;
	}
}
