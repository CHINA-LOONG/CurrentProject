package com.hawk.game.entity;

import java.util.Calendar;
import java.util.LinkedList;
import java.util.List;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkTime;
import org.hawk.util.HawkJsonUtil;
import org.hibernate.annotations.GenericGenerator;

import com.google.gson.reflect.TypeToken;
import com.hawk.game.util.GsConst;

/**
 * 玩家基础数据
 * 
 * @author hawk
 * 
 */
@Entity
@Table(name = "player")
@SuppressWarnings("serial")
public class PlayerEntity extends HawkDBEntity {
	@Id
	@GenericGenerator(name = "AUTO_INCREMENT", strategy = "native")
	@GeneratedValue(generator = "AUTO_INCREMENT")
	@Column(name = "id", unique = true)
	private int id = 0;

	@Column(name = "puid", unique = true, nullable = false)
	protected String puid = "";

	@Column(name = "nickname", unique = true, nullable = false)
	protected String nickname = "";	

	@Column(name = "gender")
	protected byte gender = 0;
	
	@Column(name = "eye")
	protected byte eye = 0;

	@Column(name = "hair")
	protected byte hair = 0;
	
	@Column(name = "hairColor")
	protected byte hairColor = 0;
	
	@Column(name = "career")
	protected byte career = 0;
	
	@Column(name = "recharge")
	protected int recharge = 0;

	@Column(name = "vipLevel")
	protected int vipLevel = 0;
	
	@Column(name = "coin")
	protected long coin = 0;
	
	@Column(name = "level")
	protected short level = 1;

	@Column(name = "exp")
	protected int exp = 0;
	
	@Column(name = "gold")
	protected int gold = 0;
	
	@Column(name = "battleMonster", nullable = false)
	protected String battleMonsterJson = "";
	
	@Column(name = "language", nullable = false)
	protected String language = GsConst.DEFAULT_LANGUAGE;
	
	@Column(name = "device", nullable = false)
	protected String device = "";

	@Column(name = "platform", nullable = false)
	protected String platform = "";

	@Column(name = "phoneInfo", nullable = false)
	protected String phoneInfo = "";
	
	@Column(name = "loginTime")
	protected Calendar loginTime = null;

	@Column(name = "logoutTime")
	protected Calendar logoutTime = null;

	@Column(name = "resetTime")
	protected Calendar resetTime = null;

	@Column(name = "createTime", nullable = false)
	protected Calendar createTime = null;

	@Column(name = "updateTime")
	protected Calendar updateTime;

	@Column(name = "invalid")
	protected boolean invalid;

	@Transient
	private List<Integer> battleMonsterList = new LinkedList<Integer>();
	
	public PlayerEntity() {
		this.createTime = HawkTime.getCalendar();
		this.loginTime = HawkTime.getCalendar();
	}

	public PlayerEntity(String puid, String device, String platform, String phoneInfo) {
		this.puid = puid;
		this.createTime = HawkTime.getCalendar();
		this.loginTime = HawkTime.getCalendar();
	}
	
	public PlayerEntity(String puid, String nickname, byte career, int gender, int eye, int hair, int hairColor){
		this.puid = puid;
		this.createTime = HawkTime.getCalendar();
		this.loginTime = HawkTime.getCalendar();
		this.nickname = nickname;
		this.career = career;
		this.gender = (byte)gender;
		this.eye = (byte)eye;
		this.hair = (byte)hair;
		this.hairColor = (byte)hairColor;
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getPuid() {
		return puid;
	}

	public void setPuid(String puid) {
		this.puid = puid;
	}
	
	public String getNickname() {
		return this.nickname;
	}
	
	public short getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = (short)level;
	}
	
	public byte getCareer() {
		return career;
	}

	public void setCareer(byte career) {
		this.career = career;
	}
	
	public long getCoin() {
		return coin;
	}

	public void setCoin(long coin) {
		this.coin = coin;
	}

	public int getExp() {
		return exp;
	}

	public void setExp(int exp) {
		this.exp = exp;
	}
	
	public int getGold() {
		return gold;
	}

	public void setGold(int gold) {
		this.gold = gold;
	}
	
	public List<Integer> getBattleMonsterList() {
		return battleMonsterList;
	}

	public void setBattleMonsterList(List<Integer> list) {
		this.battleMonsterList = list;
	}

	public String getLanguage() {
		return language;
	}

	public void setLanguage(String language) {
		this.language = language;
	}

	public int getRecharge() {
		return recharge;
	}

	public void setRecharge(int recharge) {
		this.recharge = recharge;
	}

	public int getVipLevel() {
		return vipLevel;
	}

	public void setVipLevel(int vipLevel) {
		this.vipLevel = vipLevel;
	}
	
	public int getGender() {	
		return this.gender;
	}
	
	public int getEye() {
		return this.eye;
	}
	
	public int getHair() {
		return this.hair;
	}

	public int getHairColor() {
		return this.hairColor;
	}
	
	public String getDevice() {
		return device;
	}

	public void setDevice(String device) {
		this.device = device;
	}

	public String getPlatform() {
		return platform;
	}

	public void setPlatform(String platform) {
		this.platform = platform;
	}

	public String getPhoneInfo() {
		return phoneInfo;
	}

	public void setPhoneInfo(String phoneInfo) {
		this.phoneInfo = phoneInfo;
	}

	public Calendar getLoginTime() {
		return loginTime;
	}

	public void setLoginTime(Calendar loginTime) {
		this.loginTime = loginTime;
	}

	public Calendar getLogoutTime() {
		return logoutTime;
	}

	public void setLogoutTime(Calendar logoutTime) {
		this.logoutTime = logoutTime;
	}

	public Calendar getResetTime() {
		return resetTime;
	}

	public void setResetTime(Calendar resetTime) {
		this.resetTime = resetTime;
	}
	
	@Override
	public boolean decode() {
		if (battleMonsterJson != null && false == "".equals(battleMonsterJson) && false == "null".equals(battleMonsterJson)) {
			battleMonsterList = HawkJsonUtil.getJsonInstance().fromJson(battleMonsterJson, new TypeToken<List<Integer>>() {}.getType());
		}
		return true;
	}
	
	@Override
	public boolean encode() {
		battleMonsterJson = HawkJsonUtil.getJsonInstance().toJson(battleMonsterList);
		return true;
	}

	@Override
	public Calendar getCreateTime() {
		return createTime;
	}
	
	@Override
	public void setCreateTime(Calendar createTime) {
		this.createTime = createTime;
	}

	@Override
	public Calendar getUpdateTime() {
		return updateTime;
	}

	@Override
	public void setUpdateTime(Calendar updateTime) {
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
