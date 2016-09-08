package org.hawk.dbmerge;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.hawk.log.HawkLog;
import org.hawk.thread.HawkThreadPool;

/**
 * 
 * @author xulinqs
 *
 */
public class MergeChannel {
	/**
	 * 配置表信息
	 */
	private MapperCfg mapperCfg ;
	/**
	 * 源服务器
	 */
	private ServerInfo srcServer;
	/**
	 * 目标服务器
	 */
	private ServerInfo dstServer;
	/**
	 * 修改表信息
	 */
	private List<Table> modifyTables = new LinkedList<Table>();
	
	/**
	 * 构造函数
	 * 
	 * @param srcServer
	 * @param dstServer
	 * @param mapperCfg
	 */
	public MergeChannel(ServerInfo srcServer, ServerInfo dstServer, MapperCfg mapperCfg) {
		this.srcServer = srcServer;
		this.dstServer = dstServer;
		this.mapperCfg = mapperCfg;
	}

	/**
	 * 构建依赖关系
	 */
	private List<Table> genDependency() {
		for(MapperCfg.MapperUnit key : mapperCfg.mapper.keySet()) {
			Table table = new Table();
			table.name = key.getTable();
			int index = key.getColumn().indexOf("pk_");
			if(index >= 0) {
				table.pkType = key.getColumn().substring(index + 3, key.getColumn().length());
				table.pkName = key.getColumn().substring(0, index - 1);
			} else {
				table.pkName = key.getColumn();
			}
			table.createPKColumn(key.getTable(), table.pkName);
			modifyTables.add(table);
		}
		
		for(Map.Entry<MapperCfg.MapperUnit, List<MapperCfg.MapperUnit>> entry : mapperCfg.mapper.entrySet()) {
			Table depTable = findTable(entry.getKey().getTable());
			if(depTable == null) {
				continue;
			}
			
			for(MapperCfg.MapperUnit mapperUnit : entry.getValue()) {
				Table table = findTable(mapperUnit.getTable());
				if(table != null) {
					table.addDependencyColumn(mapperUnit.getColumn(), depTable.getPkColumn());
				}
			}
		}
		
		return modifyTables;
	}
	
	/**
	 * 查找修改表信息
	 * 
	 * @param tableName
	 * @return
	 */
	private Table findTable(String tableName) {
		for(Table table : modifyTables) {
			if(table.name.equals(tableName)) {
				return table;
			}
		}
		return null;
	}
	
	/**
	 * 开始合并
	 */
	public void doMerge() {
		MergeMain.threadPool = new HawkThreadPool(getClass().getSimpleName());
		MergeMain.threadPool.initPool(8);
		MergeMain.threadPool.start();
		
		// 生成依赖关系
		genDependency();
		
		// 先预先构建所有的内存映射关系
		for(Table table : modifyTables) {
			table.genPkRef(srcServer.getSession(), dstServer.getSession());
		}
		
		// 生成插入的sql 语句并且执行
		HawkLog.logPrintln("merge table start.");
		for(Table table : modifyTables) {
			HawkLog.logPrintln("table: " + table.name + "running...");
			table.doMerge(srcServer, dstServer);
			HawkLog.logPrintln("table: " + table.name + " finish.");
		}
		
		HawkLog.logPrintln("\n========================================\n" + 
							srcServer.toString() + 
							"\n --->\n" + 
							dstServer.toString() + 
							"\n========================================\n");
		
		HawkLog.logPrintln("wait table merge thread finish...");
		MergeMain.threadPool.close(true);
		HawkLog.logPrintln("table merge thread exit.");
	}
	
}
