package com.hawk.game.config;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;
import org.hawk.log.HawkLog;

import sun.print.resources.serviceui;

import com.hawk.game.util.InstanceUtil;

@HawkConfigManager.CsvResource(file = "staticData/instanceEntry.csv", struct = "map")
public class InstanceEntryCfg extends HawkConfigBase {

	@Id
	protected final String id;
	protected final String name;
	protected final int chapter;
	protected final int index;
	protected final int type;
	protected final int difficulty;
	protected final int fatigue;
	protected final int count;
	protected final String enemy1;
	protected final String enemy2;
	protected final String enemy3;
	protected final String enemy4;
	protected final String enemy5;
	protected final String enemy6;
	protected final String reward1;
	protected final String reward2;
	protected final String reward3;
	protected final String reward4;
	protected final String reward5;
	protected final String reward6;

	// client only
	protected final String desc= null;

	// assemble
	protected InstanceChapterCfg chapterCfg;
	protected List<MonsterCfg> enemyList;
	protected List<ItemCfg> rewardItemList;

	public InstanceEntryCfg() {
		id = "";
		name = "";
		chapter = 0;
		index = 0;
		type = 0;
		difficulty = 0;
		fatigue = 0;
		count = 0;
		enemy1= enemy2 = enemy3= enemy4 = enemy5= enemy6 = "";
		reward1 = reward2 = reward3 = reward4 = reward5 = reward6 = "";
	}

	@Override
	protected boolean assemble() {
		// 计算InstanceChapter
		InstanceUtil.addInstance(this);

		enemyList = new ArrayList<MonsterCfg>();
		rewardItemList = new ArrayList<ItemCfg>();
		return true;
	}

	@Override
	protected boolean checkValid() {
		chapterCfg = HawkConfigManager.getInstance().getConfigByKey(InstanceChapterCfg.class, chapter);
		if (null == chapterCfg) {
			HawkLog.errPrintln(String.format("config invalid InstanceChapterCfg : %s", chapter));
			return false;
		}

		// 检测敌人是否存在，并建立引用
		if (enemy1 != "") {
			MonsterCfg enemy = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, enemy1);
			if (null == enemy) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", enemy1));
				return false;
			}
			enemyList.add(enemy);
		}
		if (enemy2 != "") {
			MonsterCfg enemy = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, enemy2);
			if (null == enemy) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", enemy2));
				return false;
			}
			enemyList.add(enemy);
		}
		if (enemy3 != "") {
			MonsterCfg enemy = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, enemy3);
			if (null == enemy) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", enemy3));
				return false;
			}
			enemyList.add(enemy);
		}
		if (enemy4 != "") {
			MonsterCfg enemy = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, enemy4);
			if (null == enemy) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", enemy4));
				return false;
			}
			enemyList.add(enemy);
		}
		if (enemy5 != "") {
			MonsterCfg enemy = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, enemy5);
			if (null == enemy) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", enemy5));
				return false;
			}
			enemyList.add(enemy);
		}
		if (enemy6 != "") {
			MonsterCfg enemy = HawkConfigManager.getInstance().getConfigByKey(MonsterCfg.class, enemy6);
			if (null == enemy) {
				HawkLog.errPrintln(String.format("config invalid MonsterCfg : %s", enemy6));
				return false;
			}
			enemyList.add(enemy);
		}

		// 检测奖励是否存在，并建立引用
		if (reward1 != null && reward1.equals("") == false) {
			ItemCfg reward = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, reward1);
			if (null == reward) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", reward1));
				return false;
			}
			rewardItemList.add(reward);
		}
		if (reward2 != null && reward2.equals("") == false) {
			ItemCfg reward = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, reward2);
			if (null == reward) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", reward2));
				return false;
			}
			rewardItemList.add(reward);
		}
		if (reward3 != null && reward3.equals("") == false) {
			ItemCfg reward = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, reward3);
			if (null == reward) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", reward3));
				return false;
			}
			rewardItemList.add(reward);
		}
		if (reward4 != null && reward4.equals("") == false) {
			ItemCfg reward = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, reward4);
			if (null == reward) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", reward4));
				return false;
			}
			rewardItemList.add(reward);
		}
		if (reward5 != null && reward5.equals("") == false) {
			ItemCfg reward = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, reward5);
			if (null == reward) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", reward5));
				return false;
			}
			rewardItemList.add(reward);
		}
		if (reward6 != null && reward6.equals("") == false) {
			ItemCfg reward = HawkConfigManager.getInstance().getConfigByKey(ItemCfg.class, reward6);
			if (null == reward) {
				HawkLog.errPrintln(String.format("config invalid ItemCfg : %s", reward6));
				return false;
			}
			rewardItemList.add(reward);
		}

		return true;
	}

	public String getInstanceId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public int getChapter() {
		return chapter;
	}

	public int getIndex() {
		return index;
	}

	public InstanceChapterCfg getChapterCfg() {
		return chapterCfg;
	}

	public int getType() {
		return type;
	}

	public int getDifficult() {
		return difficulty;
	}

	public int getFatigue() {
		return fatigue;
	}

	public int getCount() {
		return count;
	}

	public List<MonsterCfg> getEnemyList() {
		return Collections.unmodifiableList(enemyList);
	}

	public List<ItemCfg> getRewardList() {
		return Collections.unmodifiableList(rewardItemList);
	}

}
