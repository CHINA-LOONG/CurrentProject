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

	@Column(name = "gender", nullable = false)
	protected byte gender = 0;

	@Column(name = "eye", nullable = false)
	protected byte eye = 0;

	@Column(name = "hair", nullable = false)
	protected byte hair = 0;

	@Column(name = "hairColor", nullable = false)
	protected byte hairColor = 0;

	@Column(name = "career", nullable = false)
	protected byte career = 0;

	@Column(name = "recharge", nullable = false)
	protected int recharge = 0;

	@Column(name = "vipLevel", nullable = false)
	protected int vipLevel = 0;

	@Column(name = "level", nullable = false)
	protected short level = 1;

	@Column(name = "exp", nullable = false)
	protected int exp = 0;

	@Column(name = "coin", nullable = false)
	protected long coin = 0;

	@Column(name = "goldBuy", nullable = false)
	protected int goldBuy = 0;

	@Column(name = "goldFree", nullable = false)
	protected int goldFree = 0;

	@Column(name = "towerCoin", nullable = false)
	protected int towerCoin = 0;

	@Column(name = "blockPlayer", nullable = false)
	protected String blockPlayerJson = "";

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
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime;

	@Column(name = "invalid", nullable = false)
	protected boolean invalid;

	@Transient
	private List<Integer> blockPlayerList = new LinkedList<Integer>();

	public PlayerEntity() {
		this.loginTime = HawkTime.getCalendar();
	}

	public PlayerEntity(String puid, String device, String platform, String phoneInfo) {
		this.puid = puid;
		this.loginTime = HawkTime.getCalendar();
	}

	public PlayerEntity(String puid, String nickname, byte career, int gender, int eye, int hair, int hairColor){
		this.puid = puid;
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

	public byte getCareer() {
		return career;
	}

	public void setCareer(byte career) {
		this.career = career;
	}

	public short getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = (short)level;
	}

	public int getExp() {
		return exp;
	}

	public void setExp(int exp) {
		this.exp = exp;
	}

	public long getCoin() {
		return coin;
	}

	public void setCoin(long coin) {
		this.coin = coin;
	}

	public int getBuyGold() {
		return goldBuy;
	}

	public int getFreeGold() {
		return goldFree;
	}

	public void setBuyGold(int gold) {
		this.goldBuy = gold;
	}

	public void setFreeGold(int gold) {
		this.goldFree = gold;
	}

	public int getTowerCoin() {
		return towerCoin;
	}

	public void setTowerCoin(int towerCoin) {
		this.towerCoin = towerCoin;
	}

	public List<Integer> getBlockPlayerList() {
		return blockPlayerList;
	}

	public void addBlockPlayer(int playerId) {
		if (false == blockPlayerList.contains(playerId)) {
			blockPlayerList.add(playerId);
		}
	}

	public void removeBlockPlayer(int playerId) {
		blockPlayerList.remove(Integer.valueOf(playerId));
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
		if (blockPlayerJson != null && false == "".equals(blockPlayerJson) && false == "null".equals(blockPlayerJson)) {
			blockPlayerList = HawkJsonUtil.getJsonInstance().fromJson(blockPlayerJson, new TypeToken<List<Integer>>() {}.getType());
		}

		return true;
	}

	@Override
	public boolean encode() {
		blockPlayerJson = HawkJsonUtil.getJsonInstance().toJson(blockPlayerList);
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
