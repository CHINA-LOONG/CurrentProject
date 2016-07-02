package com.hawk.game.util;

import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.protocol.Player.PlayerInfo;

public class BuilderUtil {

	/**
	 * 生成玩家协议同步信息
	 * 
	 * @return
	 */
	public static PlayerInfo.Builder genPlayerBuilder(PlayerEntity playerEntity) {
		PlayerInfo.Builder builder = PlayerInfo.newBuilder();
		builder.setPlayerId(playerEntity.getId());
		builder.setNickname(playerEntity.getNickname());
		builder.setGold(playerEntity.getGold());
		builder.setCoin(playerEntity.getCoin());
		builder.setExp(playerEntity.getExp());
		builder.setCareer(playerEntity.getCareer());
		builder.setLevel(playerEntity.getlevel());
		builder.setGender(playerEntity.getlevel());
		builder.setEye(playerEntity.getExp());
		builder.setHair(playerEntity.getHair());
		builder.setHairColor(playerEntity.getHairColor());
		builder.setRecharge(playerEntity.getRecharge());
		builder.setVipLevel(playerEntity.getVipLevel());
		return builder;
	}
}
