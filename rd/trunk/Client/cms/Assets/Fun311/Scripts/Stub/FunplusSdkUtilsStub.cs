using UnityEngine;
using System.Collections;

namespace Funplus {
	
	public class FunplusSdkUtilsStub : BaseSdkUtilsWrapper {
		
		private static FunplusSdkUtilsStub instance;
		private static readonly object locker = new object ();
		
		private FunplusSdkUtilsStub() {
		}
		
		public static FunplusSdkUtilsStub GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusSdkUtilsStub ();
				}
			}
			return instance;
		}
		
		public override string GetTotalMemory () {
			Debug.Log ("Calling FunplusSdkUtilsStub.GetTotalMemory ().");
			return "100";
		}
		
		public override string GetAvailableMemory () {
			Debug.Log ("Calling FunplusSdkUtilsStub.GetAvailableMemory ().");
			return "60";
		}

		public override string GetDeviceName() {
			return null;
		}
		
		public override string GetOsName() {
			return null;
		}
		
		public override string GetOsVersion() {
			return null;
		}
		
		public override string GetCountry() {
			return null;
		}
		
		public override string GetDeviceType() {
			return null;
		}

		public override string GetScreenSize() {
			return null;
		}
		
		public override string GetScreenOrientation() {
			return null;
		}
		
		public override string GetScreenDensity() {
			return null;
		}
		
		public override string GetDisplayWidth() {
			return null;
		}
		
		public override string GetDisplayHeight() {
			return null;
		}
	}
	
}