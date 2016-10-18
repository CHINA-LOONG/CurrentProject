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
	public class FunplusFBFriend
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Pic { get; set; }
		public string Gender { get; set; }

		public static List<FunplusFBFriend> FromFacebookFriendsMessage (string message)
		{
			List<FunplusFBFriend> friendsList = new List<FunplusFBFriend> ();

			try
			{
				var list = Json.Deserialize (message) as List<object>;
				foreach (object item in list)
				{
					var dict = item as Dictionary<string, object>;
					friendsList.Add(new FunplusFBFriend {
						Id = (string)dict ["id"],
						Name = (string)dict ["name"],
						Pic = (string)dict ["pic"], 
						Gender = (string)dict ["gender"]
					});
				}
			}
			catch (Exception e)
			{
				Debug.LogError ("[funsdk] JSON parse error: " + e.Message);
			}
			return friendsList;
		}

		public string GetID ()
		{
			return Id;
		}

		public string GetName ()
		{
			return Name;
		}

		public string GetPic ()
		{
			return Pic;
		}

		public string GetGender ()
		{
			return Gender;
		}
	}
}
