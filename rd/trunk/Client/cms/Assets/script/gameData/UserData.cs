using UnityEngine;
using System.Collections;

public class UserData : MonoBehaviour
{
	public	string guid ;
	public	string deviceID ;
	//public	string platform;
	//public 	string	version;
	public	string	token;

	public void Init()
	{
		deviceID = SystemInfo.deviceUniqueIdentifier;
		guid = PlayerPrefs.GetString ("testGuid");
		//platform = SystemInfo.deviceModel;
		token = "tokenT";
	}
}
