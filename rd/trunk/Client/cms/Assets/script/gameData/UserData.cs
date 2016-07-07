using UnityEngine;
using System.Collections;

public class UserData : MonoBehaviour
{
	public	string guid ;
	public	string deviceID ;
	public	string platform;
	public 	string	version;
	public	string	token;

	public void Init()
	{
		deviceID = SystemInfo.deviceUniqueIdentifier;
		guid = PlayerPrefs.GetString ("testGuid");
		if (string.IsNullOrEmpty (guid)) 
		{
			guid = "TestGuID" + System.DateTime.Now.ToString();
			PlayerPrefs.SetString("testGuid",guid);
			Logger.Log ("New guid = " + guid);
		}

		platform = SystemInfo.deviceModel;
		version = "0.2.1";
		token = "tokenT";
	}
}
