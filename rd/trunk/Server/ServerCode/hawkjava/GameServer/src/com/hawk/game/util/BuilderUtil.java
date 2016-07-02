package com.hawk.game.util;

import java.util.Map.Entry;

import com.hawk.game.entity.MonsterEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.protocol.Monster.HSMonster;
import com.hawk.game.protocol.Player.PlayerInfo;
import com.hawk.game.protocol.Skill.HSSkill;

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
	
	public static HSMonster.Builder genMonsterBuilder(MonsterEntity monsterEntity) {
		HSMonster.Builder builder = HSMonster.newBuilder();
		builder.setMonsterId(monsterEntity.getId());
		builder.setCfgId(monsterEntity.getCfgId());
		builder.setStage(monsterEntity.getStage());
		builder.setLevel(monsterEntity.getLevel());
		builder.setExp(monsterEntity.getExp());
		builder.setLazy(monsterEntity.getLazy());
		builder.setAi(monsterEntity.getAi());
		
		HSSkill.Builder skill = HSSkill.newBuilder();
		for (Entry<Integer, Integer> entry : monsterEntity.getSkillMap().entrySet()) {
			skill.setSkillId(entry.getKey());
			skill.setLevel(entry.getValue());
			builder.addSkill(skill);
		}
		return builder;
	}
}
