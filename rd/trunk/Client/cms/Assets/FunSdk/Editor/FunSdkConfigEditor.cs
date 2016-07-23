using UnityEngine;
using UnityEditor;

using System.Collections;

[CustomEditor(typeof(FunSdkConfig))]
public class FunSdkConfigEditor : Editor {
	public override void OnInspectorGUI()
	{
		FunSdkConfig config = FunSdkConfig.Instance;

//		EditorGUILayout.LabelField("Game Id: ");
//		EditorGUILayout.HelpBox("Game Id, the unique id for your game", MessageType.None);
//		config.MGameId = EditorGUILayout.TextField(config.MGameId);
//
//		EditorGUILayout.LabelField("Game Key: ");
//		EditorGUILayout.HelpBox("Game Secret Key", MessageType.None);
//		config.MGameKey = EditorGUILayout.TextField(config.MGameKey);

		EditorGUILayout.Space();

		GUIContent enableFacebookLogin = new GUIContent(" Enable Facebook Login [?]", 
		                                             "Need Facebook app id/key and other config");

		GUIContent enableVkLogin = new GUIContent(" Enable Vk Login [?]", 
		                                                 "Need VK app id/key");

		GUIContent enableWechatLogin = new GUIContent(" Enable Wechat Login [?]", 
		                                                 "Need Wechat app id/key and other config");

		GUIContent enableItunsFileShare = new GUIContent ("Enable ItunssFileshare[?] ",
		                                                  "Need config ture/false");



		//FB items
		EditorGUILayout.BeginVertical();
		config.MEnableFb = EditorGUILayout.ToggleLeft(enableFacebookLogin, config.MEnableFb);
		if(config.MEnableFb) {

			EditorGUILayout.LabelField("Facebook App Id: ");
			EditorGUILayout.HelpBox("Add Facebook App Id (Only for you have Facebook login)", MessageType.None);
			config.MFbAppId = EditorGUILayout.TextField(config.MFbAppId);
			EditorGUILayout.LabelField("Facebook App Display Name: ");
			EditorGUILayout.HelpBox("Add Facebook App Display Name (Only for you have Facebook login)", MessageType.None);
			config.MFbAppDisName = EditorGUILayout.TextField(config.MFbAppDisName);
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();

		//Vk items
		EditorGUILayout.BeginVertical();
		config.MEnableVk = EditorGUILayout.ToggleLeft(enableVkLogin, config.MEnableVk);
		if(config.MEnableVk) {
			
			EditorGUILayout.LabelField("VK App Id: ");
			EditorGUILayout.HelpBox("Add VK App Id (Only for you have VK login)", MessageType.None);
			config.MVkAppId = EditorGUILayout.TextField(config.MVkAppId);
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();


		//Wechat items
		EditorGUILayout.BeginVertical();
		config.MEnableWx = EditorGUILayout.ToggleLeft(enableWechatLogin, config.MEnableWx);
		if(config.MEnableWx) {
			EditorGUILayout.LabelField("Wechat App Id: ");
			EditorGUILayout.HelpBox("Add Wechat App Id (Only for you have Wechat login)", MessageType.None);
			config.MWxAppId = EditorGUILayout.TextField(config.MWxAppId);
			
			EditorGUILayout.LabelField("Wechat App key: ");
			EditorGUILayout.HelpBox("Add Wechat App Id (Only for you have Wechat login)", MessageType.None);
			config.MWxAppKey = EditorGUILayout.TextField(config.MWxAppKey);
			
			EditorGUILayout.LabelField("Wechat MSDK URL: ");
			EditorGUILayout.HelpBox("Add Wechat MSDK URL (Only for you have Wechat login)", MessageType.None);
			config.MWxMsdkUrl = EditorGUILayout.TextField(config.MWxMsdkUrl);

			EditorGUILayout.LabelField("Wechat MSDK Offer ID: ");
			EditorGUILayout.HelpBox("Add Wechat MSDK Offer ID (Only for you have Wechat login)", MessageType.None);
			config.MWxMsdkOfferId = EditorGUILayout.TextField(config.MWxMsdkOfferId);
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical ();
		config.ITunesfilesharing = EditorGUILayout.ToggleLeft (enableItunsFileShare, config.ITunesfilesharing);
		EditorGUILayout.EndVertical ();

	}
}
