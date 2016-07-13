package com.hawk.version;

import java.io.File;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import javax.persistence.Entity;
import javax.persistence.Table;

import org.hawk.config.HawkClassScaner;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.hibernate.Query;
import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;
import org.hibernate.cfg.Configuration;

import com.hawk.version.entity.VersionEntity;

public class VersionServices {
	/**
	 * 运行状态
	 */
	volatile boolean running = true;
	
	/**
	 * 配置对象
	 */
	protected Configuration conf;
	/**
	 * db连接会话工厂
	 */
	protected SessionFactory sessionFactory;
	
	private Map<Integer, List<VersionEntity>> versionList = new TreeMap<Integer, List<VersionEntity>>();
	private int currentVersionCode = 0;
	
	private String resourceServerAddress = null;

	private String platform = null;
	
	private String chanel = null;
	/**
	 * 单例实例对象
	 */
	private static VersionServices instance = null;
	
	/**
	 * 获取实例对象
	 * 
	 * @return
	 */
	public static VersionServices getInstance() {
		if (instance == null) {
			instance = new VersionServices();
		}
		return instance;
	}

	public boolean init(String hbmXml, String connUrl, String userName, String passWord, String entityPackages, String platform, String chanel) {
		// 初始化数据库连接
		if (hbmXml != null && connUrl != null && userName != null && passWord != null && entityPackages != null) {
			//加载驱动程序
			try {
				Class.forName("com.mysql.jdbc.Driver");
			} catch (Exception e) {
				HawkException.catchException(e);
				return false;
			}
			
			try {
				if (this.conf == null) {
					String fileName = System.getProperty("user.dir") + hbmXml;
					this.conf = new Configuration();
					this.conf.configure(new File(fileName));
				}

				// 重新数据库设置
				this.conf.setProperty("hibernate.connection.url", connUrl);
				this.conf.setProperty("hibernate.connection.username", userName);
				this.conf.setProperty("hibernate.connection.password", passWord);

				String[] entityPackageArray = entityPackages.split(",");
				if (entityPackageArray != null) {
					for (String entityPackage : entityPackageArray) {
						HawkLog.logPrintln("init entity package: " + entityPackage);
						List<Class<?>> classList = HawkClassScaner.scanClassesFilter(entityPackage.trim(), Entity.class);
						for (Class<?> entityClass : classList) {
							if (entityClass.getAnnotation(Table.class) != null) {
								this.conf.addAnnotatedClass(entityClass);
								HawkLog.logPrintln("scan database entity: " + entityClass.getSimpleName());
							}
						}
					}
				}

				this.running = true;
				this.sessionFactory = this.conf.buildSessionFactory();
				HawkLog.logPrintln(String.format("init database, connUrl: %s, userName: %s, pwd: %s, hbmXml: %s", connUrl, userName, passWord, hbmXml));	
				
				loadVersionEntities(platform, chanel);			
				return true;
			} catch (Exception e) {
				HawkException.catchException(e);
			}
		}	

		return false;
	}	
	
	/**
	 * 获取db会话
	 * 
	 * @return
	 */
	public Session getSession() {
		if (sessionFactory != null) {
			return sessionFactory.getCurrentSession();
		}
		return null;
	}
	
	/**
	 * 根据查询条件返回指定个数的对象列表。先在远程库中进行查询，返回的对象列表将会被遍历
	 */
	@SuppressWarnings("unchecked")
	public <T> List<T> query(String hql, Object... values) {
		long beginTimeMs = HawkTime.getMillisecond();
		try {
			Session session = getSession();
			Transaction trans = session.beginTransaction();
			try {
				Query query = getSession().createQuery(hql);
				if (values != null) {
					for (int i = 0; i < values.length; i++) {
						query.setParameter(i, values[i]);
					}
				}
				List<T> list = (List<T>) query.list();
				trans.commit();
				return list;
			} catch (Exception e) {
				trans.rollback();
				HawkException.catchException(e);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
		} finally {
			long costTimeMs = HawkTime.getMillisecond() - beginTimeMs;
			HawkLog.logPrintln("dbmanager.query time, hql: " + hql + ", costtime: " + costTimeMs);
		}
		return null;
	}
	/**
	 * 加载所有版本数据
	 */
	public void loadVersionEntities(String platform, String chanel) {
		versionList.clear();
		List<VersionEntity> versionEntities = query("from VersionEntity where platform = ? and chanel = ? order by id asc", platform, chanel);
		for (VersionEntity element :versionEntities) {
			if (versionList.containsKey(element.getCode()) == false) {
				versionList.put(element.getCode(), new LinkedList<VersionEntity>());
			}
			
			versionList.get(element.getCode()).add(element);

			if (element.getCode() > currentVersionCode) {
				currentVersionCode = element.getCode();
			}
		}
	}
	
	public List<VersionEntity> getEntityList(int code) {
		return versionList.get(code);
	}
	
	public int getVersionCode() {
		return currentVersionCode;
	}

	public String getChanel() {
		return chanel;
	}

	public void setChanel(String chanel) {
		this.chanel = chanel;
	}

	public String getPlatform() {
		return platform;
	}

	public void setPlatform(String platform) {
		this.platform = platform;
	}
	
	public String getResourceServerAddress() {
		return resourceServerAddress;
	}

	public void setResourceServerAddress(String resourceServerAddress) {
		this.resourceServerAddress = resourceServerAddress;
	}

	public boolean tick() {
		return running;
	}
	
	/**
	 * 停止
	 */
	public void stop() {
		running = false;
	}
}
