#if UNITY_IPHONE

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Funplus {

	public class FunPaymentIOS {

		[DllImport ("__Internal")]
		private static extern void obj_fp_payment_setObjName (string aGameObjName);
		[DllImport ("__Internal")]
		private static extern void obj_fp_payment_get_products_info ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_payment_set_order_complete_callback ();
		[DllImport ("__Internal")]
		private static extern void obj_fp_payment_buy (string aProId, string aThroughCargo);


		public FunPaymentIOS()
		{

		}

		public void reset(string aObjName)
		{
			MonoBehaviour.print ("------FunPaymentIOS reset------");

			obj_fp_payment_setObjName (aObjName);
		}

		public void getProductsInfo()
		{
			MonoBehaviour.print ("------FunPaymentIOS getProductsInfo------");

			obj_fp_payment_get_products_info ();
		}

		public void setOrderCompleteCallback()
		{
			MonoBehaviour.print ("------FunPaymentIOS setOrderCompleteCallback------");

			obj_fp_payment_set_order_complete_callback ();
		}

		public void buy(string aProId, string aThroughCargo)
		{
			MonoBehaviour.print ("------FunPaymentIOS buy------");
			obj_fp_payment_buy (aProId, aThroughCargo);
		}

	}
}


#endif