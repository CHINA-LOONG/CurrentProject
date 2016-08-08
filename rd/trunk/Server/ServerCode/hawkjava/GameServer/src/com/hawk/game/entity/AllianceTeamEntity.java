package com.hawk.game.entity;

import java.util.HashSet;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hibernate.annotations.GenericGenerator;


@Entity
@Table(name = "allianceTeam")
@SuppressWarnings("serial")
public class AllianceTeamEntity extends HawkDBEntity{
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "allianceId")
	private int allianceId = 0;
	
	@Column(name = "captain")
	private int captain = 0;
	
	@Column(name = "member1")
	private int member1 = 0;
	
	@Column(name = "member2")
	private int member2 = 0;
	
	@Column(name = "member3")
	private int member3 = 0;
	
	@Column(name = "taskId")
	private int taskId = 0;
	
	@Column(name = "coinQuest1")
	private int coinQuest1 = 0;
	
	@Column(name = "coinQuest2")
	private int coinQuest2 = 0;
	
	@Column(name = "itemQuest1")
	private int itemQuest1 = 0;
	
	@Column(name = "itemQuest2")
	private int itemQuest2 = 0;
	
	@Column(name = "itemQuest3")
	private int itemQuest3 = 0;
	
	@Column(name = "instanceQuest1")
	private int instanceQuest1 = 0;
	
	@Column(name = "instatnceQuest1Accept")
	private String instatnceQuest1Accept = null;
	
	@Column(name = "coinQuest1PlayerId")
	private int coinQuest1PlayerId = 0;
	
	@Column(name = "coinQuest2PlayerId")
	private int coinQuest2PlayerId = 0;
	
	@Column(name = "itemQuest1PlayerId")
	private int itemQuest1PlayerId = 0;
	
	@Column(name = "itemQuest2PlayerId")
	private int itemQuest2PlayerId = 0;
	
	@Column(name = "itemQuest3PlayerId")
	private int itemQuest3PlayerId = 0;
	
	@Column(name = "instanceQuest1PlayerId")
	private int instanceQuest1PlayerId = 0;
	
	@Column(name = "overTime", nullable = false)
	protected int overTime = 0;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;
	
	@Column(name = "updateTime")
	protected int updateTime;
	
	/**
	 * 0:正常,1:为删除状态
	 */
	@Column(name = "invalid")
	protected boolean invalid;
	
	/**
	 * 所有任务已经完成
	 */
	@Transient
	private boolean taskFinish = false;

	@Transient
	private Set<Integer> acceptList = new HashSet<>();
	
	/**
	 * 对应的工会
	 */
	@Transient
	private AllianceEntity allianceEntity = null;
	
	public AllianceTeamEntity() { 
		
	}
	
	public void setAllianceEntity(AllianceEntity allianceEntity) {
		this.allianceEntity = allianceEntity;
	}

	public int getId() {
		return id;
	}

	public void setAllianceId(int allianceId) {
		this.allianceId = allianceId;
	}

	public int getAllianceId() {
		return allianceId;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getCaptain() {
		return captain;
	}

	public void setCaptain(int captain) {
		this.captain = captain;
	}

	public int getMember1() {
		return member1;
	}

	public void setMember1(int member1) {
		this.member1 = member1;
	}

	public int getMember2() {
		return member2;
	}

	public void setMember2(int member2) {
		this.member2 = member2;
	}

	public int getMember3() {
		return member3;
	}

	public void setMember3(int member3) {
		this.member3 = member3;
	}

	public int getTaskId() {
		return taskId;
	}

	public void setTaskId(int taskId) {
		this.taskId = taskId;
	}

	public int getCoinQuest1() {
		return coinQuest1;
	}

	public void setCoinQuest1(int coinQuest1) {
		this.coinQuest1 = coinQuest1;
	}

	public int getCoinQuest2() {
		return coinQuest2;
	}

	public void setCoinQuest2(int coinQuest2) {
		this.coinQuest2 = coinQuest2;
	}

	public int getItemQuest1() {
		return itemQuest1;
	}

	public void setItemQuest1(int itemQuest1) {
		this.itemQuest1 = itemQuest1;
	}

	public int getItemQuest2() {
		return itemQuest2;
	}

	public void setItemQuest2(int itemQuest2) {
		this.itemQuest2 = itemQuest2;
	}

	public int getItemQuest3() {
		return itemQuest3;
	}

	public void setItemQuest3(int itemQuest3) {
		this.itemQuest3 = itemQuest3;
	}

	public int getInstanceQuest1() {
		return instanceQuest1;
	}

	public Set<Integer> getAcceptList() {
		return acceptList;
	}

	public void setInstanceQuest1(int instanceQuest1) {
		this.instanceQuest1 = instanceQuest1;
	}

	public int getCoinQuest1PlayerId() {
		return coinQuest1PlayerId;
	}

	public void setCoinQuest1PlayerId(int coinQuest1PlayerId) {
		this.coinQuest1PlayerId = coinQuest1PlayerId;
	}

	public int getCoinQuest2PlayerId() {
		return coinQuest2PlayerId;
	}

	public void setCoinQuest2PlayerId(int coinQuest2PlayerId) {
		this.coinQuest2PlayerId = coinQuest2PlayerId;
	}

	public int getItemQuest1PlayerId() {
		return itemQuest1PlayerId;
	}

	public void setItemQuest1PlayerId(int itemQuest1PlayerId) {
		this.itemQuest1PlayerId = itemQuest1PlayerId;
	}

	public int getItemQuest2PlayerId() {
		return itemQuest2PlayerId;
	}

	public void setItemQuest2PlayerId(int itemQuest2PlayerId) {
		this.itemQuest2PlayerId = itemQuest2PlayerId;
	}

	public int getItemQuest3PlayerId() {
		return itemQuest3PlayerId;
	}

	public void setItemQuest3PlayerId(int itemQuest3PlayerId) {
		this.itemQuest3PlayerId = itemQuest3PlayerId;
	}

	public int getInstanceQuest1PlayerId() {
		return instanceQuest1PlayerId;
	}

	public void setInstanceQuest1PlayerId(int instanceQuest1PlayerId) {
		this.instanceQuest1PlayerId = instanceQuest1PlayerId;
	}

	public boolean isTimeout(){
		return  HawkTime.getSeconds() > createTime + overTime;
	}
	
	public boolean isTeamFull(){
		return   member1 != 0 && member2 != 0 && (captain != 0 || member3 != 0);
	}
	
	public boolean isTeamEmpty(){
		return member1 == 0 && member2 == 0 && member3 == 0 && captain == 0;
	}
	
	public boolean hasTeamMember(){
		return member1 != 0 || member2 != 0 || member3 != 0;
	}
	
	public int getOverTime() {
		return overTime;
	}
	
	public void setOverTime(int overTime) {
		this.overTime = overTime;
	}
	
	public boolean isTaskFinish() {
		return taskFinish;
	}

	public void setTaskFinish(boolean taskFinish) {
		this.taskFinish = taskFinish;
	}
	
	/**
	 * 添加到队伍
	 * @param playerId
	 * @return
	 */
	public boolean addPlayerToTeam(int teamId, int playerId)
	{
		if (taskFinish == true) {
			return false;
		}else if (member1 == 0) {
			member1 = playerId;
		}else if (member2 == 0) {
			member2 = playerId;
		}else if (captain == 0 && member3 == 0) {
			member3 = playerId;
		}else {
			return false;
		}
		
		allianceEntity.addPlayerTeamMap(playerId, teamId);
		return true;
	}
	
	/**
	 * 从队伍中移除某个玩家
	 * @param playerId
	 * @return
	 */
	public boolean removePlayerFromTeam(int playerId)
	{
		if (captain == playerId) {
			captain = 0;
		}else if (member1 == playerId) {
			member1 = 0;
		}else if (member2 == playerId) {
			member2 = 0;
		}else if (member3 == playerId) {
			member3 = 0;
		}else {
			return false;
		}
		
		allianceEntity.removePlayerTeamMap(playerId);
		if (isTeamEmpty() == true) {
			if (allianceEntity.getFinishTeamList().get(id) != null) {
				allianceEntity.getFinishTeamList().remove(id);
			}
			else if (allianceEntity.getUnfinishTeamList().get(id) != null) {
				allianceEntity.getUnfinishTeamList().remove(id);
			}

			this.delete();
		}
		
		return true;
	}
	
	/**
	 * 清理队伍
	 * 
	 */
	public boolean clearTeam()
	{
		if (captain != 0) {
			allianceEntity.removePlayerTeamMap(captain);
		}else if (member1 != 0) {
			allianceEntity.removePlayerTeamMap(member1);
		}else if (member2 != 0) {
			allianceEntity.removePlayerTeamMap(member2);
		}else if (member3 != 0) {
			allianceEntity.removePlayerTeamMap(member3);
		}
		
		this.delete();
		
		return true;
	}
	
	
	/**
	 * 玩家小任务数量统计
	 * @param playerId
	 * @return
	 */
	public int getFinishCount(int playerId){
		int finishCount = 0;
		if(itemQuest1PlayerId == playerId) finishCount++;
	    if (itemQuest2PlayerId == playerId) finishCount++;
		if (itemQuest3PlayerId == playerId) finishCount++;
		if (coinQuest1PlayerId == playerId) finishCount++;
		if (coinQuest2PlayerId == playerId) finishCount++;
		if (instanceQuest1PlayerId == playerId) finishCount++;
		
		return finishCount;
	}
	
	public void addAcceptMap(int playerId){
		acceptList.add(playerId);
	}
	
	public void removeAcceptMap(int playerId){
		acceptList.remove(playerId);
	}
	
	public void setQuestFinish(int QuestId, int playerId){
		if(itemQuest1 == QuestId) itemQuest1PlayerId = playerId;
		else if (itemQuest2 == QuestId) itemQuest2PlayerId = playerId;
		else if (itemQuest3 == QuestId) itemQuest3PlayerId = playerId;
		else if (coinQuest1 == QuestId) coinQuest1PlayerId = playerId;
		else if (coinQuest2 == QuestId) coinQuest2PlayerId = playerId;
		else if (instanceQuest1 == QuestId) instanceQuest1PlayerId = playerId;
		
		if (itemQuest1PlayerId != 0 && itemQuest2PlayerId != 0 && itemQuest3PlayerId != 0 && coinQuest1PlayerId != 0 && coinQuest2PlayerId != 0 && instanceQuest1PlayerId != 0) {
			taskFinish = true;
			allianceEntity.setTaskFinish(id);
		}	
	}
	
	@Override
	public int getCreateTime() {
		return createTime;
	}
	
	@Override
	public void setCreateTime(int createTime) {
		this.createTime = createTime;
	}
	
	@Override
	public int getUpdateTime() {
		return updateTime;
	}
	
	@Override
	public void setUpdateTime(int updateTime) {
		this.updateTime = updateTime;
	}

	@Override
	public boolean isInvalid() {
		return invalid;
	}

	@Override
	public void setInvalid(boolean invalid) {
		this.invalid = invalid;
	}
	
	@Override
	public void notifyUpdate(boolean async) {
		super.notifyUpdate(async);
	}
	
	@Override
	public boolean decode() {
		if (itemQuest1PlayerId != 0 && itemQuest2PlayerId != 0 && itemQuest3PlayerId != 0 && coinQuest1PlayerId != 0 && coinQuest2PlayerId != 0 && instanceQuest1PlayerId != 0) {
			taskFinish = true;
		}
		
		if (instatnceQuest1Accept != null && false == "".equals(instatnceQuest1Accept) && false == "null".equals(instatnceQuest1Accept)) {
			String[] players = instatnceQuest1Accept.trim().split(" ");
			for (String player  : players) {
				acceptList.add(Integer.parseInt(player));
			}
		}
		
		return super.decode();
	}
	
	@Override
	public boolean encode() {
		instatnceQuest1Accept = "";
		for (Integer player : acceptList) {
			instatnceQuest1Accept += player + " ";
		}
		instatnceQuest1Accept.trim();
		return super.encode();
	}
}
