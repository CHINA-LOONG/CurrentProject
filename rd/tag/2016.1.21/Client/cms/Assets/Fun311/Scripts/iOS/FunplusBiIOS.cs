using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Funplus {

	public class FunplusBiIOS : BaseBiWrapper {

		private static FunplusBiIOS instance;
		private static readonly object locker = new object ();
		
		public static FunplusBiIOS GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusBiIOS ();
				}
			}
			return instance;
		}

		public override void TraceEvent (string eventName, string eventData) {
			com_funplus_sdk_bi_traceEvent (eventName, eventData);
		}
		
		public void LogUserLogin (string uid) {
			com_funplus_sdk_bi_logUserLogin (uid);
		}
		
		public void LogNewUser (string uid) {
			com_funplus_sdk_bi_logNewUser (uid);
		}
		
		public void LogUserLogout () {
			com_funplus_sdk_bi_logUserLogout ();
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_traceEvent (string eventName, string eventData);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_logUserLogin (string uid);
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_logNewUser (string uid);
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_bi_logUserLogout ();
	}

}