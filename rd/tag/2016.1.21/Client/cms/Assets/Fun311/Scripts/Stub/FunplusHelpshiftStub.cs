using UnityEngine;
using System.Collections;

namespace Funplus {

	public class FunplusHelpshiftStub : BaseHelpshiftWrapper {

		private static FunplusHelpshiftStub instance;
		private static readonly object locker = new object ();
		
		private FunplusHelpshiftStub () {
		}
		
		public static FunplusHelpshiftStub GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusHelpshiftStub ();
				}
			}
			return instance;
		}

		public override void ShowConversation () {
			Logger.Log ("Calling FunplusHelpshiftStub.ShowConversation ().");
		}

		public override void ShowFAQs () {
			Logger.Log ("Calling FunplusHelpshiftStub.ShowFAQs ().");
		}

	}

}