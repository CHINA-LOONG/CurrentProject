/*
 * FunSdkIOS.cs
 * 
 * Create by Lei Gu 12/11/14
 */

#if UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Funplus
{	
	public class FunSdkIOS {
		
		[DllImport ("__Internal")]
		private static extern void obj_fp_funsdk_setObjName (string aGameObjName);
		[DllImport ("__Internal")]
		private static extern void obj_fp_funsdk_install (string aId, string aKey, bool aIsProduct);
		[DllImport ("__Internal")]
		private static extern void obj_fp_sdk_set_debug (bool aIsDebug);

		public FunSdkIOS() {
			
		}
		
		public void reset (string aGameObjName) 
		{
			MonoBehaviour.print ("------FunSdkIOS reset------");
			obj_fp_funsdk_setObjName (aGameObjName);
		}
		
		public void install(string aId, string aKey, bool aIsProduct) 
		{
			if(String.IsNullOrEmpty(aId) || String.IsNullOrEmpty(aKey) )
			{
				return;
			}
			obj_fp_funsdk_install (aId, aKey, aIsProduct);
		}

		public void setDebug(bool aIsDebug)
		{
			obj_fp_sdk_set_debug(aIsDebug);
		}

	};
}

#endif


