using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Funplus {

	public class FunplusSdkUtilsIOS : BaseSdkUtilsWrapper {

		private static FunplusSdkUtilsIOS instance;
		private static readonly object locker = new object ();

		public static FunplusSdkUtilsIOS GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusSdkUtilsIOS ();
				}
			}
			return instance;
		}
		
		public override string GetTotalMemory () {
			return com_funplus_sdk_getTotalMemory ();
		}
		
		public override string GetAvailableMemory () {
			return com_funplus_sdk_getAvailableMemory ();
		}

		public override string GetDeviceName () {
			return null;
		}
		
		public override string GetOsName () {
			return null;
		}
		
		public override string GetOsVersion () {
			return null;
		}
		
		public override string GetCountry () {
			return null;
		}
		
		public override string GetDeviceType () {
			return null;
		}

		public override string GetScreenSize () {
			return null;
		}
		
		public override string GetScreenOrientation () {
			return null;
		}
		
		public override string GetScreenDensity () {
			return null;
		}
		
		public override string GetDisplayWidth () {
			return null;
		}
		
		public override string GetDisplayHeight () {
			return null;
		}

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_getTotalMemory ();

		[DllImport ("__Internal")]
		private static extern string com_funplus_sdk_getAvailableMemory ();
	}
}