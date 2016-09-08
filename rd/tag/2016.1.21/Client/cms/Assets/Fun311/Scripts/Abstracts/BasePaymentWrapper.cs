using UnityEngine;
using System.Collections;

namespace Funplus {

	abstract public class BasePaymentWrapper {
		abstract public void SetGameObject (string gameObjectName);
		abstract public bool CanMakePurchases ();
		abstract public void StartHelper ();
		abstract public void Buy (string productId, string throughCargo);
	}

}