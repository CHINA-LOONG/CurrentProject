package com.hawk.version;

import org.hawk.config.HawkXmlCfg;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;

import com.hawk.version.http.VersionHttpServer;

public class VSMain {
	public static void main(String[] args) {
		try {
			// 打印启动参数
			for (int i = 0; i < args.length; i++) {
				HawkLog.logPrintln(args[i]);
			}

			HawkXmlCfg conf = new HawkXmlCfg(System.getProperty("user.dir") + "/cfg/config.xml");
			
			VersionServices.getInstance().init(conf.getString("db.dbHbmXml"), conf.getString("db.dbConnUrl"), conf.getString("db.dbUserName"), conf.getString("db.dbPassWord"), conf.getString("db.entityPackages"), conf.getString("app.platform"), conf.getString("app.chanel"));		
			VersionServices.getInstance().setResourceServerAddress(conf.getString("app.resourceServer"));
			VersionServices.getInstance().setChanel(conf.getString("app.chanel"));
			VersionServices.getInstance().setPlatform(conf.getString("app.platform"));
			VersionHttpServer versionServer = new VersionHttpServer();
			versionServer.setup(conf.getString("app.addr"), conf.getInt("app.port"), conf.getInt("app.pool"));
			versionServer.run();
			
			// 退出
			HawkLog.logPrintln("gameserver exit");
			System.exit(0);

		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}
}
