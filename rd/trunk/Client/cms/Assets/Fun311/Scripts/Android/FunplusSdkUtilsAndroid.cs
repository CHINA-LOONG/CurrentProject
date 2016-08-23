using UnityEngine;
using System.Collections;

namespace Funplus {

	public class FunplusSdkUtilsAndroid : BaseSdkUtilsWrapper {

		private AndroidJavaClass funplusSdkUtilsWrapper;

		private static FunplusSdkUtilsAndroid instance;
		private static readonly object locker = new object ();

		private FunplusSdkUtilsAndroid () {
			funplusSdkUtilsWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusSdkUtilsWrapper");
		}

		public static FunplusSdkUtilsAndroid GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusSdkUtilsAndroid ();
				}
			}
			return instance;
		}

		public override string GetTotalMemory () {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getTotalMemory");
		}

		public override string GetAvailableMemory () {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getAvailableMemory");
		}

		public override string GetDeviceName() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDeviceName");
		}

		public override string GetOsName() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getOsName");
		}

		public override string GetOsVersion() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getOsVersion");
		}

		public override string GetCountry() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getCountry");
		}

		public override string GetDeviceType() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDeviceType");
		}

		public override string GetScreenSize() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getScreenSize");
		}
		
		public override string GetScreenOrientation() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getScreenOrientation");
		}
		
		public override string GetScreenDensity() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getScreenDensity");
		}

		public override string GetDisplayWidth() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDisplayWidth");
		}

		public override string GetDisplayHeight() {
			return funplusSdkUtilsWrapper.CallStatic<string> ("getDisplayHeight");
		}
	}

}