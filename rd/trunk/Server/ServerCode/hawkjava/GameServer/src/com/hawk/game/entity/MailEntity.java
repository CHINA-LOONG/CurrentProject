package com.hawk.game.entity;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hibernate.annotations.GenericGenerator;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.util.GsConst.ItemParseType;

/**
 * 邮件数据
 * 
 * @author walker
 * 
 */
@Entity
@Table(name = "mail")
public class MailEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "receiverId", nullable = false)
	protected int receiverId = 0;

	@Column(name = "senderId", nullable = false)
	protected int senderId = 0;

	@Column(name = "senderName", nullable = false)
	protected String senderName = "";

	@Column(name = "state", nullable = false)
	protected byte state = 0;

	@Column(name = "subject", nullable = false)
	protected String subject = "";

	@Column(name = "content", nullable = false)
	protected String content = "";

	@Column(name = "rewardList", nullable = false)
	protected String rewardStr = "";

	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime = 0;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid = false;

	@Transient
	protected List<ItemInfo> rewardList = new ArrayList<ItemInfo>();

	public MailEntity() {

	}

	public MailEntity(int receiverId, int senderId, String senderName, byte state, String subject, String content) {
		this.receiverId = receiverId;
		this.senderId = senderId;
		this.senderName = senderName;
		this.state = state;
		this.subject = subject;
		this.content = content;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public int getReceiverId() {
		return receiverId;
	}

	public void setReceiverId(int receiverId) {
		this.receiverId = receiverId;
	}

	public int getSenderId() {
		return senderId;
	}

	public void setSenderId(int senderId) {
		this.senderId = senderId;
	}

	public String getSenderName() {
		return senderName;
	}

	public void setSenderName(String senderName) {
		this.senderName = senderName;
	}

	public byte getState() {
		return state;
	}

	public void setState(byte state) {
		this.state = state;
	}

	public String getSubject() {
		return subject;
	}

	public void setSubject(String subject) {
		this.subject = subject;
	}

	public String getContent() {
		return content;
	}

	public void setContent(String content) {
		this.content = content;
	}

	public List<ItemInfo> getRewardList() {
		return rewardList;
	}

	public void setRewardList(List<ItemInfo> rewardList) {
		this.rewardList = rewardList;
	}

	@Override
	public boolean decode() {
		if (null != rewardStr && false == "".equals(rewardStr) && false == "null".equals(rewardStr)) {
			rewardList = ItemInfo.GetItemInfoList(this.rewardStr, ItemParseType.PARSE_DEFAULT);
		}
		return true;
	}

	@Override
	public boolean encode() {
		rewardStr = ItemInfo.toString(rewardList);
		return true;
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
}
