using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Funplus {

	public class FunplusHelpshiftIOS : BaseHelpshiftWrapper {

		private static FunplusHelpshiftIOS instance;
		private static readonly object locker = new object ();
		
		public static FunplusHelpshiftIOS GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusHelpshiftIOS ();
				}
			}
			return instance;
		}

		public override void ShowConversation () {
			com_funplus_sdk_helpshift_showConversation ();
		}
		
		public override void ShowFAQs () {
			com_funplus_sdk_helpshift_showFAQs ();
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_helpshift_showConversation ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_helpshift_showFAQs ();

	}

}