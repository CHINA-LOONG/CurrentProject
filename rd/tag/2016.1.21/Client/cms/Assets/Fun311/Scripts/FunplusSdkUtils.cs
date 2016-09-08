using UnityEngine;
using System.Collections;

namespace Funplus {

	/// <summary>
	/// This class provides some helper methods for Funplus SDK.
	/// </summary>
	public class FunplusSdkUtils {
		private static FunplusSdkUtils instance;
		private static readonly object locker = new object ();

		private BaseSdkUtilsWrapper GetWrapper() {
			if (Application.platform == RuntimePlatform.Android) {
				return FunplusSdkUtilsAndroid.GetInstance ();
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return FunplusSdkUtilsIOS.GetInstance ();
			} else {
				return FunplusSdkUtilsStub.GetInstance ();
			}
		}

		public static FunplusSdkUtils GetIntance () {
			if (instance == null) {
				lock (locker) {
					instance = new FunplusSdkUtils();
				}
			}
			return instance;
		}

		/// <summary>
		/// Gets the total memory.
		/// </summary>
		/// <returns>The total memory.</returns>
		public string GetTotalMemory () {
			return GetWrapper ().GetTotalMemory ();
		}

		/// <summary>
		/// Gets the available memory.
		/// </summary>
		/// <returns>The available memory.</returns>
		public string GetAvailableMemory() {
			return GetWrapper ().GetAvailableMemory ();
		}

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		/// <returns>The device name.</returns>
		public string GetDeviceName() {
			return GetWrapper ().GetDeviceName ();
		}

		/// <summary>
		/// Gets the name of the OS.
		/// </summary>
		/// <returns>The OS name.</returns>
		public string GetOsName() {
			return GetWrapper ().GetOsName ();
		}

		/// <summary>
		/// Gets the OS version.
		/// </summary>
		/// <returns>The OS version.</returns>
		public string GetOsVersion() {
			return GetWrapper ().GetOsVersion ();
		}

		/// <summary>
		/// Gets country.
		/// </summary>
		/// <returns>The country code.</returns>
		public string GetCountry() {
			return GetWrapper ().GetCountry ();
		}

		/// <summary>
		/// Gets the type of the device.
		/// </summary>
		/// <returns>The device type.</returns>
		public string GetDeviceType() {
			return GetWrapper ().GetDeviceType ();
		}

		/// <summary>
		/// Gets the size of the screen.
		/// </summary>
		/// <returns>The screen size.</returns>
		public string GetScreenSize() {
			return GetWrapper ().GetScreenSize ();
		}

		/// <summary>
		/// Gets the screen orientation.
		/// </summary>
		/// <returns>The screen orientation.</returns>
		public string GetScreenOrientation() {
			return GetWrapper ().GetScreenOrientation ();
		}

		/// <summary>
		/// Gets the screen density.
		/// </summary>
		/// <returns>The screen density.</returns>
		public string GetScreenDensity() {
			return GetWrapper ().GetScreenDensity ();
		}

		/// <summary>
		/// Gets the display width.
		/// </summary>
		/// <returns>The display width.</returns>
		public string GetDisplayWidth() {
			return GetWrapper ().GetDisplayWidth ();
		}

		/// <summary>
		/// Gets the display height.
		/// </summary>
		/// <returns>The display height.</returns>
		public string GetDisplayHeight() {
			return GetWrapper ().GetDisplayHeight ();
		}
	}

}