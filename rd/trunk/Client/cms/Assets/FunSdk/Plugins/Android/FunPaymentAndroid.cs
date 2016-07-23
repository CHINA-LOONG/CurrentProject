#if UNITY_ANDROID

using UnityEngine;
using System.Collections;

namespace Funplus {
	public class FunPaymentAndroid {

		private AndroidJavaClass jc;
		private AndroidJavaObject currentActivity, application;
		private AndroidJavaObject funPlugin;

		private void funApiCall(string aApi, params object[] aArgs) 
		{
			this.funPlugin.CallStatic (aApi, aArgs);
		}
		
		private void funApiCall(string aApi) 
		{
			this.funPlugin.CallStatic (aApi);
		}
		
		public FunPaymentAndroid() {
			if(Application.platform == RuntimePlatform.Android)
			{
				this.jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				this.currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
				this.application = this.currentActivity.Call<AndroidJavaObject>("getApplication");
				
				this.funPlugin = new AndroidJavaClass("com.funplus.sdk.unity3d.FunPayment4Unity3d");
			}
		}

		public void reset (string aGameObjName) 
		{
			MonoBehaviour.print ("------FunPaymentAndroid reset------");
			funApiCall("setGameObjName", aGameObjName);
		}

		public void getProductsInfo()
		{
			MonoBehaviour.print ("------FunPaymentAndroid getProductsInfo------");
			funApiCall("getProductsInfo");
		}
		
		public void setResultHandler()
		{
			MonoBehaviour.print ("------FunPaymentAndroid setOrderCompleteCallback------");
			funApiCall("setResultHandler");
		}
		
		public void buy(string aProId, string aThroughCargo)
		{
			MonoBehaviour.print ("------FunPaymentAndroid buy------");
			funApiCall("buy", new object[]{aProId, aThroughCargo});
		}
	}
}

#endif
