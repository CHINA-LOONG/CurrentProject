using UnityEngine;
using System.Collections;

namespace Funplus {

	public class FunplusHelpshiftAndroid : BaseHelpshiftWrapper {
		private AndroidJavaClass funplusHelpshiftWrapper;
		
		private static FunplusHelpshiftAndroid instance;
		private static readonly object locker = new object ();
		
		private FunplusHelpshiftAndroid () {
			funplusHelpshiftWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusHelpshiftWrapper");
		}
		
		public static FunplusHelpshiftAndroid GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusHelpshiftAndroid ();
				}
			}
			return instance;
		}
		
		public override void ShowConversation () {
			funplusHelpshiftWrapper.CallStatic ("showConversation");
		}	

		public override void ShowFAQs () {
			funplusHelpshiftWrapper.CallStatic ("showFAQs");
		}
	}
	
}