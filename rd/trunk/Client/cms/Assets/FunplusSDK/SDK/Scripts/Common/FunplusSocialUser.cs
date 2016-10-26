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
	/// This class represents a social user.
	/// </summary>
	public class FunplusSocialUser
	{
		public string Uid { get; set; }
		public string Pic { get; set; }
		public string Name { get; set; }
		public string Gender { get; set; }
		public string Email { get; set; }
		public string AccessToken { get; set; }

		public static FunplusSocialUser GetUserDataFromMessage(string message){
			try
			{
				var dict = Json.Deserialize (message) as Dictionary<string,object>;
				return new FunplusSocialUser {
					Uid = (string)dict ["uid"],
					Pic = (string)dict ["pic"],
					Name = (string)dict ["name"],
					Gender = (string)dict ["gender"],
					Email = (string)dict ["email"],
					AccessToken = (string)dict ["access_token"]
				};
			}
			catch(Exception e)
			{
				Debug.LogError ("[funsdk] JSON parse error: " + e.Message);
				return null;
			};
		}

		public override string ToString ()
		{
			return string.Format ("[uid={0}, pic={1}, name={2}, gender={3}, email={4}]",
				Uid, Pic, Name, Gender, Email);
		}

		public string GetUid ()
		{
			return Uid;
		}

		public string GetPic ()
		{
			return Pic;
		}

		public string GetName ()
		{
			return Name;
		}

		public string GetGender ()
		{
			return Gender;
		}

		public string GetEmail ()
		{
			return Email;
		}

		public string GetAccessToken ()
		{
			return AccessToken;
		}

		public string ToJsonString ()
		{
			return string.Format (
				@"{{""uid"": ""{0}"", ""pic"": ""{1}"", ""name"": ""{2}"", ""gender"": ""{3}"", ""email"": ""{4}"", ""access_token"": ""{5}""}}",
				Uid, Pic, Name, Gender, Email, AccessToken
			);
		}
	}

}