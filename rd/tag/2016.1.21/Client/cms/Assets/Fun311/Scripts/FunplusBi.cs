using UnityEngine;
using System.Collections;

namespace Funplus {

	/// <summary>
	/// The BI module helps to integrate your game to Funplus Data System.
	/// </summary>
	public class FunplusBi {

		private static FunplusBi instance;
		private static readonly object locker = new object ();

		private BaseBiWrapper GetWrapper () {
			if (Application.platform == RuntimePlatform.Android) {
				return FunplusBiAndroid.GetInstance ();
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return FunplusBiIOS.GetInstance ();
			} else {
				return FunplusBiStub.GetInstance ();
			}
		}

		/// <summary>
		/// Gets the intance.
		/// </summary>
		/// <returns>The intance.</returns>
		public static FunplusBi GetIntance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusBi();
				}
			}
			return instance;
		}

		/// <summary>
		/// Traces an event.
		/// </summary>
		/// <param name="eventName">The event name.</param>
		/// <param name="properties">Properties of the event.</param>
		public void TraceEvent (string eventName, string properties) {
			GetWrapper ().TraceEvent (eventName, properties);
		}
	}

}