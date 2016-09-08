using UnityEngine;
using System.Collections;

namespace Funplus {

	abstract public class BaseAccountWrapper {
		abstract public void SetGameObject (string gameObjectName);
		abstract public bool IsUserLoggedIn ();
		abstract public void GetAvailableAccountTypes ();
		abstract public void OpenSession ();
		abstract public void Login ();
		abstract public void Login (FunplusAccountType type);
		abstract public void Logout ();
		abstract public void ShowUserCenter ();
		abstract public void BindAccount ();
		abstract public void BindAccount (FunplusAccountType type);
	}

}