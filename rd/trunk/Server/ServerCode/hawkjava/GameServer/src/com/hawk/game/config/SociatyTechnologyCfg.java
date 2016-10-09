package com.hawk.game.config;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.Map;
import java.util.List;

import org.hawk.config.HawkConfigBase;
import org.hawk.config.HawkConfigManager;

import com.hawk.game.item.ItemInfo;
import com.hawk.game.util.ConfigUtil;
import com.hawk.game.util.GsConst;

@HawkConfigManager.CsvResource(file = "staticData/sociatytechnology.csv", struct = "map")
public class SociatyTechnologyCfg extends HawkConfigBase{
	/**
	 * 配置id
	 */
	@Id
	protected final int id ;
	/**
	 * 类型
	 */
	protected final int type;
	/**
	 * 级别
	 */
	protected final int level;
	/**
	 * 工会等级限制
	 */
	protected final int sociatyLevel;
	/**
	 * 升级消耗
	 */
	protected final int levelUp;
	/**
	 * 容量
	 */
	protected final int gainPeople;
	/**
	 * 金币
	 */
	protected final float gainCoin;
	/**
	 * 经验药
	 */
	protected final String gainExp;

	// client only
	protected final String icon;
	protected final String tecDescript;
	protected final String tecName;

	// assemble
	/**
	 * 科技列表
	 */
	private static Map<Integer, List<SociatyTechnologyCfg>> technologyList = new HashMap<Integer, List<SociatyTechnologyCfg>>();
	/**
	 * 经验药
	 */
	private ItemInfo expItem;

	public SociatyTechnologyCfg(){
		id = 0;
		type = 0;
		level = 0;
		sociatyLevel = 0;
		levelUp = 0;
		gainPeople = 0;
		gainCoin = 0.0f;
		gainExp = null;
		expItem = null;
		icon = null;
		tecDescript = null;
		tecName = null;
	}

	public int getId() {
		return id;
	}

	public int getType() {
		return type;
	}

	public int getLevel() {
		return level;
	}

	public int getSociatyLevel() {
		return sociatyLevel;
	}

	public int getLevelUp() {
		return levelUp;
	}

	public int getGainPeople() {
		return gainPeople;
	}

	public float getGainCoin() {
		return gainCoin;
	}

	public ItemInfo getExpItem() {
		return expItem;
	}

	public String getIcon() {
		return icon;
	}

	public String getTecDescript() {
		return tecDescript;
	}

	public String getTecName() {
		return tecName;
	}

	public static boolean fullLevel(int type, int level){
		return level >= technologyList.get(type).size();
	}

	public static int levelLimit(int type, int level){
		return technologyList.get(type).get(level - 1).getSociatyLevel();
	}

	public static int levelUpContribution(int type, int level){
		return technologyList.get(type).get(level - 1).getLevelUp();
	}

	public static int getMemberPop(int level){
		return technologyList.get(GsConst.Alliance.ALLIANCE_TEC_MEMBER).get(level - 1).getGainPeople();
	}

	public static SociatyTechnologyCfg getSociatyTechnologyCfg(int type, int level){
		return technologyList.get(type).get(level - 1);
	}

	@Override
	protected boolean assemble() {
		List<SociatyTechnologyCfg> technology = technologyList.get(type);
		if (technology == null) {
			technology = new LinkedList<>();
			technologyList.put(type, technology);
		}

		technology.add(this);
		if (type == 4) {
			if (this.gainExp != null && this.gainExp.length() > 0 && !"0".equals(this.gainExp)) {
				expItem = ItemInfo.valueOf(gainExp, GsConst.ItemParseType.PARSE_DEFAULT);
				if (null == expItem) {
					return false;
				}
			}
		}
		return true;
	}

	@Override
	protected boolean checkValid() {
		if (null != expItem && false == ConfigUtil.checkItemInfoValid(expItem)) {
			return false;
		}
		return true;
	}
}
