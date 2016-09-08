using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus {

	/// <summary>
	/// During the life circle of a game process, status and properties of the player
	/// may change quite often. Session hide the complexity behind this.
	/// </summary>
	public class FunplusSession {

		private string fpid;
		private string sessionKey;

		//\cond

		/// <summary>
		/// Constructs a session by using the given message (sent from native code).
		/// </summary>
		/// <returns>The session.</returns>
		/// <param name="message">Message.</param>
		public static FunplusSession FromMessage (string message) {
			FunplusSession session = new FunplusSession ();

			try {
				var dict = Json.Deserialize (message) as Dictionary<string,object>;

				session.fpid = (string)dict ["fpid"];
				session.sessionKey = (string)dict ["session_key"];
			} catch (Exception) {
				session.fpid = "none";
				session.sessionKey = "none";
			}

			return session;
		}

		//\endcond

		/// <summary>
		/// Gets the Funplus ID of current session.
		/// </summary>
		/// <returns>The Funplus ID.</returns>
		public string GetFpid () {
			return fpid;
		}

		/// <summary>
		/// Gets the session key of current session.
		/// </summary>
		/// <returns>The session key.</returns>
		public string GetSessionKey () {
			return sessionKey;
		}

		// TODO
		public string GetEmail () {
			return null;
		}

		// TODO
		public FunplusAccountType GetAccountType () {
			return FunplusAccountType.FPAccountTypeNotSpecified;
		}
	}

}