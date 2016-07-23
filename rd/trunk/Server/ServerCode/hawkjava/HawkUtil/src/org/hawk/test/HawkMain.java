package org.hawk.test;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.security.spec.AlgorithmParameterSpec;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;
import java.util.UUID;
import java.util.concurrent.atomic.AtomicInteger;

import javax.crypto.Cipher;
import javax.crypto.KeyGenerator;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import javax.swing.ListModel;

import net.sf.json.JSONObject;

import org.apache.commons.mail.Email;
import org.apache.commons.mail.SimpleEmail;
import org.hawk.app.HawkApp;
import org.hawk.app.HawkAppCfg;
import org.hawk.app.HawkAppObj;
import org.hawk.cryption.HawkAESCrypt;
import org.hawk.cryption.HawkDESCrypt;
import org.hawk.cryption.HawkRsaCrypt;
import org.hawk.db.mysql.HawkMysqlSession;
import org.hawk.log.HawkLog;
import org.hawk.net.protocol.HawkProtocol;
import org.hawk.os.HawkException;
import org.hawk.os.HawkOSOperator;
import org.hawk.os.HawkRand;
import org.hawk.os.HawkTime;
import org.hawk.rpc.HawkRpcServer;
import org.hawk.rpc.HawkRpcWorker;
import org.hawk.thread.HawkThread;
import org.hawk.timer.HawkTimerEntry;
import org.hawk.timer.HawkTimerListener;
import org.hawk.timer.HawkTimerManager;
import org.hawk.util.services.HawkOrderService;
import org.hawk.util.services.HawkReportService;
import org.hawk.util.services.HawkReportService.CommonData;
import org.hawk.util.services.HawkReportService.LoginData;
import org.hawk.util.services.HawkReportService.RechargeData;
import org.hawk.util.services.HawkReportService.RegisterData;
import org.hawk.xid.HawkXID;
import org.hawk.zmq.HawkZmq;
import org.hawk.zmq.HawkZmqManager;
import org.hibernate.dialect.DB2390Dialect;
import org.hibernate.sql.Update;

import com.sun.org.apache.xalan.internal.xsltc.cmdline.getopt.GetOpt;
import com.sun.org.apache.xml.internal.serialize.LineSeparator;

@SuppressWarnings("unused")
public class HawkMain {
	private static class RpcWorker extends HawkRpcWorker {
		@Override
		public HawkProtocol response(HawkProtocol protocol) {
			return protocol;
		}
	}

	private static void testRpcServer() {
		// 添加库加载目录
		HawkOSOperator.addUsrPath(System.getProperty("user.dir") + "/lib");

		// 初始化zmq管理器
		HawkZmqManager.getInstance().init(HawkZmq.HZMQ_CONTEXT_THREAD);

		HawkRpcServer rpcServer = new HawkRpcServer();
		if (rpcServer.init("tcp://*:9595", "tcp://*:9596", true)) {
			RpcWorker worker = new RpcWorker();
			if (worker.init("tcp://10.0.3.110:9596")) {
				rpcServer.addWorker(worker);
			}
		}

		while (true) {
			rpcServer.onTick();
			HawkOSOperator.osSleep(20);
		}
	}
	
	private static void genOrder(String suuid, String game, String platform, int serverId,
			int playerId, String puid, String device, String productId, int costMoney) {
		
		String channel = "";
		int pos = puid.indexOf("_");
		if (pos > 0) {
			channel = puid.substring(0, pos).toLowerCase();
		}
		
		// 初始化
		//HawkOrderService.getInstance().init(suuid, "123.59.62.233:9005", game, platform, serverId);
		
		HawkOSOperator.osSleep(2000);
		
		// 执行callback
		// INSERT INTO callback(myOrder,payMoney,pfOrder,date,createTime) values('*', 0, UUID(), CURDATE(), NOW());
	}
		
	/**
	 * 测试主函数入口
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		HawkApp app = new HawkApp(HawkXID.valueOf(0)) {
		};
		
		genOrder("84bd808a33014b1a8870d1cc9854283e", "qmby", "ios", 1, 0, "", "", 
				"com.feefoxes.qmby.1", 10);
		
		app.run();
	}
}
