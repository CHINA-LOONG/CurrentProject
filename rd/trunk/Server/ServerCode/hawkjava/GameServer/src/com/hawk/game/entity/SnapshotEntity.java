package com.hawk.game.entity;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;
import javax.persistence.Transient;

import org.hawk.db.HawkDBEntity;
import org.hawk.os.HawkException;

import com.hawk.game.protocol.Snapshot.SnapshotInfo;

@Entity
@Table(name = "player_snapshot")
@SuppressWarnings("serial")
public class SnapshotEntity extends HawkDBEntity{

	@Id
	@Column(name = "playerId", nullable = false)
	private int playerId;
	
	@Column(name = "snapshot", nullable = false)
	private byte[] snapshot;
	
	@Column(name = "createTime", nullable = false)
	protected int createTime = 0;

	@Column(name = "updateTime")
	protected int updateTime;

	@Column(name = "invalid")
	protected boolean invalid;
	
	@Transient
	private SnapshotInfo.Builder snapshotInfo;

	public int getPlayerId() {
		return playerId;
	}

	public void setPlayerId(int playerId) {
		this.playerId = playerId;
	}

	public byte[] getSnapshot() {
		return snapshot;
	}

	public void setSnapshot(byte[] snapshot) {
		this.snapshot = snapshot;
	}
	
	public SnapshotInfo.Builder getSnapshotInfo() {
		return snapshotInfo;
	}

	public void setSnapshotInfo(SnapshotInfo.Builder snapshotInfo) {
		this.snapshotInfo = snapshotInfo;
	}

	@Override
	public boolean decode() {
		try {
			SnapshotInfo playerInfo = SnapshotInfo.parseFrom(snapshot);
			SnapshotInfo.Builder builder = playerInfo.toBuilder();
			
			this.snapshotInfo = builder;
			return true;
		} catch (Exception e) {
			HawkException.catchException(e);
		}

		return false;
	}

	@Override
	public boolean encode() {				
		if (snapshotInfo != null) {
			this.snapshot = snapshotInfo.build().toByteArray();
		}
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
