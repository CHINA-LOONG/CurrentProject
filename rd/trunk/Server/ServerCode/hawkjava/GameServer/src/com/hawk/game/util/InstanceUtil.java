package com.hawk.game.util;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.hawk.config.HawkConfigManager;

import com.hawk.game.config.InstanceEntryCfg;

public class InstanceUtil {

	public static class InstanceChapter {
		public int chapterId;
		public List<InstanceEntryCfg> normalList = new ArrayList<>();
		public List<InstanceEntryCfg> hardList = new ArrayList<>();
	}

	/**
	 * @key chapterId
	 */
	public static Map<Integer, InstanceChapter> chapterMap = new HashMap<>();

	// 构造阶段---------------------------------------------------------------------

	/**
	 * 添加副本
	 */
	public static void addInstance(InstanceEntryCfg entryCfg) {
		InstanceChapter chapter = chapterMap.get(entryCfg.getChapter());
		if (chapter == null) {
			chapter = new InstanceChapter();
			chapter.chapterId = (entryCfg.getChapter());
			chapterMap.put(chapter.chapterId, chapter);
		}

		if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
			chapter.normalList.add(entryCfg);
		} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
			chapter.hardList.add(entryCfg);
		}
	}

	// 使用阶段----------------------------------------------------------------------
	
	public static Map<Integer, InstanceChapter> getInstanceChapterMap() {
		return Collections.unmodifiableMap(chapterMap);
	}

	public static int getInstanceChapterIndex(String instanceId) {
		InstanceEntryCfg entryCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceEntryCfg.class, instanceId);
		if (entryCfg != null) {
			InstanceChapter chapter = chapterMap.get(entryCfg.getChapter());
			if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.NORMAL_INSTANCE) {
				return chapter.normalList.indexOf(entryCfg);
			} else if (entryCfg.getDifficult() == GsConst.InstanceDifficulty.HARD_INSTANCE) {
				return chapter.hardList.indexOf(entryCfg);
			}
		}

		return -1;
	}

}