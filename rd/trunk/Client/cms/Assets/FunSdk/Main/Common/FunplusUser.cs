#if UNITY_IOS || UNITY_ANDROID


using UnityEngine;
using System.Collections;

namespace Funplus {
	public class FunplusUser {

		private string mUid = "";
		private string mName = "";
		private string mEmail = "";
		private string mGender = "";
		private string mAccessToken = "";

		public FunplusUser(string aUid, string aName, string aEmail, string aGender, string aAccessToken) {
			this.mUid = aUid;
			this.mName = aName;
			this.mEmail = aEmail;
			this.mGender = aGender;
			this.mAccessToken = aAccessToken;
		}

		public string getUid() {
			return mUid;
		}

		public string getName() {
			return mName;
		}

		public string getEmail() {
			return mEmail;
		}

		public string getGender() {
			return mGender;
		}

		public string getAccessToken() {
			return mAccessToken;
		}

	}
}

#endif

