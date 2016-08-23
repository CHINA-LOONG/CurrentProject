using UnityEngine;
using System.Collections;

namespace Funplus {

	/// <summary>
	/// The Helpshift module.
	/// </summary>
	public class FunplusHelpshift {

		private static FunplusHelpshift instance;
		private static readonly object locker = new object ();

#if UNITY_ANDROID
		private FunplusHelpshiftAndroid GetWrapper () {
			return FunplusHelpshiftAndroid.GetInstance ();
		}
#elif UNITY_IOS
		private FunplusHelpshiftIOS GetWrapper () {
			return FunplusHelpshiftIOS.GetInstance ();
		}
#else
		private FunplusHelpshiftStub GetWrapper () {
			return FunplusHelpshiftStub.GetInstance ();
		}
#endif

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public static FunplusHelpshift GetInstance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusHelpshift();
				}
			}
			return instance;
		}

		/// <summary>
		/// Shows the conversation window.
		/// </summary>
		public void ShowConversation () {
			GetWrapper ().ShowConversation ();
		}

		/// <summary>
		/// Shows the FAQ window.
		/// </summary>
		public void ShowFAQs() {
			GetWrapper ().ShowFAQs ();
		}
	}

}