using UnityEngine;
using System.Collections;

namespace Funplus {
	
	public class FunplusBiStub : BaseBiWrapper {
		
		private static FunplusBiStub instance;
		private static readonly object locker = new object ();
		
		private FunplusBiStub() {
		}
		
		public static FunplusBiStub GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusBiStub ();
				}
			}
			return instance;
		}
		
		public override void TraceEvent (string eventName, string properties) {
			Debug.Log ("Calling FunplusBiStub.TraceEvent ().");
		}
		
	}
	
}