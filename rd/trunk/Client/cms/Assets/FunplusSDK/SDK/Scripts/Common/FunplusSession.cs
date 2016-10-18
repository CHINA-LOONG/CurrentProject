/**
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015-Present Funplus
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HSMiniJSON;

namespace Funplus
{

	/// <summary>
	/// During the life circle of a game process, status and properties of the player
	/// may change quite often. Session hide the complexity behind this.
	/// </summary>
	public class FunplusSession 
	{

		/// <summary>
		/// Current account type.
		/// </summary>
		public FunplusAccountType AccountType { get; set; }

		/// <summary>
		/// Player's Funplus ID.
		/// </summary>
		public string Fpid { get; set; }

		/// <summary>
		/// Current session key.
		/// </summary>
		public string SessionKey { get; set; }

		/// <summary>
		/// Player's email address.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// The social platform which player is using for authentication.
		/// </summary>
		public string SnsPlatform { get; set; }

		/// <summary>
		/// The player's social ID.
		/// </summary>
		public string SnsId { get; set; }

		/// <summary>
		/// The player's social name.
		/// </summary>
		public string SnsName { get; set; }

		/// <summary>
		/// Timestamp in second when this session will expire.
		/// </summary>
		public long   ExpireOn { get; set; }

		//\cond

		/// <summary>
		/// Constructs a session by using the given message (sent from native code).
		/// </summary>
		/// <returns>The session.</returns>
		/// <param name="message">Message.</param>
		public static FunplusSession FromMessage (string message) 
		{
			try
			{
				var dict = Json.Deserialize (message) as Dictionary<string,object>;

				FunplusSession session = new FunplusSession {
					Fpid = (string)dict ["fpid"],
					SessionKey = (string)dict ["session_key"]
				};

				if (dict.ContainsKey ("account_type"))
				{
					session.AccountType = FunplusAccountTypeUtils.FromString ((string)dict ["account_type"]);
				}
				else
				{
					Debug.LogWarning ("[funsdk] Lack of the `account_type` field in session.");
				}

				if (dict.ContainsKey ("expire_on"))
				{
					session.ExpireOn = (long)dict ["expire_on"];
				}
				else
				{
					Debug.LogWarning ("[funsdk] Lack of the `expire_on` field in session.");
				}

				if (dict.ContainsKey ("email"))
				{
					session.Email = (string)dict ["email"];
				}

				if (dict.ContainsKey ("sns_platform"))
				{
					session.SnsPlatform = (string)dict ["sns_platform"];
				}

				if (dict.ContainsKey ("sns_id"))
				{
					session.SnsId = (string)dict ["sns_id"];
				}

				if (dict.ContainsKey ("sns_name"))
				{
					session.SnsName = (string)dict ["sns_name"];
				}
					
				return session;
			}
			catch (Exception e)
			{
				Debug.LogError ("[funsdk] Could not create session: " + e.Message);
				return null;
			}
		}

		//\endcond
		
		public FunplusAccountType GetAccountType () 
		{
			return AccountType;
		}

		/// <summary>
		/// Gets the Funplus ID of current session.
		/// </summary>
		/// <returns>The Funplus ID.</returns>
		public string GetFpid ()
		{
			return Fpid;
		}

		/// <summary>
		/// Gets the session key of current session.
		/// </summary>
		/// <returns>The session key.</returns>
		public string GetSessionKey ()
		{
			return SessionKey;
		}

		/// <summary>
		/// Gets the timestamp in second when this session will expire.
		/// </summary>
		/// <returns>The expire on.</returns>
		public long GetExpireOn ()
		{
			return ExpireOn;
		}

		/// <summary>
		/// Gets player's email address.
		/// </summary>
		/// <returns>The email.</returns>
		public string GetEmail () 
		{
			return Email;
		}

		/// <summary>
		/// Gets the social platform which player is using for authentication.
		/// </summary>
		/// <returns>The sns platform.</returns>
		public string GetSnsPlatform ()
		{
			return SnsPlatform;
		}

		/// <summary>
		/// Gets player's social ID.
		/// </summary>
		/// <returns>The sns identifier.</returns>
		public string GetSnsId ()
		{
			return SnsId;
		}

		/// <summary>
		/// Gets player's social name.
		/// </summary>
		/// <returns>The sns name.</returns>
		public string GetSnsName ()
		{
			return SnsName;
		}

		public string ToJsonString ()
		{
			return string.Format (
				@"{{""account_type"": ""{0}"", ""fpid"": ""{1}"", ""email"": ""{2}"", ""session_key"": ""{3}"", ""expire_on"": {4}}}",
				FunplusAccountTypeUtils.ToString (AccountType), Fpid, Email, SessionKey, ExpireOn
			);
		}
	}

}