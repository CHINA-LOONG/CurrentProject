package com.hawk.game.util;

import com.hawk.game.entity.PlayerAllianceEntity;
import com.hawk.game.entity.PlayerEntity;
import com.hawk.game.player.PlayerData;
import com.hawk.game.protocol.Snapshot.AllianceSnapInfo;
import com.hawk.game.protocol.Snapshot.PlayerSnapInfo;
import com.hawk.game.protocol.Snapshot.SnapshotInfo;

public class SnapshotUtil {
	/**
	 * 生成在线玩家快照对象
	 * @param player
	 * @return
	 */
	public static SnapshotInfo.Builder genOnlineQuickPhoto(PlayerData playerData) {
		SnapshotInfo.Builder playerSnapshot = SnapshotInfo.newBuilder();
		playerSnapshot.setPlayerId(playerData.getId());

		// 玩家信息
		attachPlayerInfo(playerSnapshot, playerData.getPlayerEntity());
		
		// 工会信息
		attachAllianceInfo(playerSnapshot, playerData.getPlayerAllianceEntity());
		
		return playerSnapshot;
	}
	
	public static void attachPlayerInfo(SnapshotInfo.Builder snapshot, PlayerEntity playerEntity) {
		if(playerEntity != null){
			PlayerSnapInfo.Builder playerInfo = PlayerSnapInfo.newBuilder();
			playerInfo.setId(playerEntity.getId());
			playerInfo.setNickname(playerEntity.getNickname());
			playerInfo.setLevel(playerEntity.getLevel());
			snapshot.setPlayerInfo(playerInfo);
		}
	}
	
	public static void attachAllianceInfo(SnapshotInfo.Builder snapshot, PlayerAllianceEntity allianceEntity) {
		if(allianceEntity != null){
			AllianceSnapInfo.Builder allianceInfo = AllianceSnapInfo.newBuilder();
			allianceInfo.setId(allianceEntity.getId());
			allianceInfo.setAllianceId(allianceEntity.getAllianceId());
			allianceInfo.setPlayerId(allianceEntity.getPlayerId());
			allianceInfo.setContribution(allianceEntity.getContribution());
			allianceInfo.setPostion(allianceEntity.getPostion());
			snapshot.setAllianceInfo(allianceInfo);
		}
	}
}
