package org.hawk.dbmerge;

import java.util.HashMap;
import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.log.HawkLog;

import net.sf.json.JSONArray;
import net.sf.json.JSONObject;

/**
 * 映射关系配置
 * 
 * @author xulinqs
 * 
 */
public class MapperCfg {

	public static class MapperUnit {
		private String table;
		private String column;

		public static MapperUnit valueOf(String table, String column) {
			MapperUnit mapperUnit = new MapperUnit();
			mapperUnit.table = table;
			mapperUnit.column = column;
			return mapperUnit;
		}

		public static MapperUnit valueOf(String tableColumn) {
			String[] items = tableColumn.split(".");
			if (items.length != 2) {
				HawkLog.errPrintln("table column error: " + tableColumn);
				return null;
			}
			MapperUnit mapperUnit = new MapperUnit();
			mapperUnit.table = items[0];
			mapperUnit.column = items[1];
			return mapperUnit;
		}

		public String getTable() {
			return table;
		}

		public String getColumn() {
			return column;
		}
	}

	Map<MapperUnit, List<MapperUnit>> mapper = new HashMap<MapperCfg.MapperUnit, List<MapperUnit>>();

	public MapperCfg(JSONObject jsonCfg) {
		Iterator<?> keyIter = jsonCfg.keys();
		while (keyIter.hasNext()) {
			List<MapperUnit> mapperUnits = new LinkedList<MapperCfg.MapperUnit>();
			String key = (String) keyIter.next();
			JSONArray value = jsonCfg.getJSONArray(key);
			for (int i = 0; i < value.size(); i++) {
				MapperUnit mapperUnit = MapperUnit.valueOf(value.getString(i));
				if (mapperUnit != null) {
					mapperUnits.add(mapperUnit);
				}
			}
			mapper.put(MapperUnit.valueOf(key), mapperUnits);
		}
	}
}
