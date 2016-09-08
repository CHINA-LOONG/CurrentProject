using UnityEngine;
using System.Collections;

namespace Funplus {

	public class FunplusBiAndroid : BaseBiWrapper {
		private AndroidJavaClass funplusBiWrapper;
		
		private static FunplusBiAndroid instance;
		private static readonly object locker = new object ();
		
		private FunplusBiAndroid () {
			funplusBiWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusBiWrapper");
		}
		
		public static FunplusBiAndroid GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusBiAndroid ();
				}
			}
			return instance;
		}

		public override void TraceEvent (string eventName, string eventData) {
			funplusBiWrapper.CallStatic ("traceEvent", eventName, eventData);
		}

	}

}