using UnityEngine;
using System.Collections;

namespace Funplus {

	abstract public class BaseSdkUtilsWrapper {
		abstract public string GetTotalMemory ();
		abstract public string GetAvailableMemory ();
		abstract public string GetDeviceName ();
		abstract public string GetOsName ();
		abstract public string GetOsVersion ();
		abstract public string GetCountry ();
		abstract public string GetDeviceType ();
		abstract public string GetScreenSize ();
		abstract public string GetScreenOrientation ();
		abstract public string GetScreenDensity ();
		abstract public string GetDisplayWidth ();
		abstract public string GetDisplayHeight ();
	}

}