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
using System.Collections;
using Funplus.Abstract;

namespace Funplus.Android
{

	public class FunplusFacebookAndroid : BaseFacebookWrapper
	{
		private AndroidJavaClass funplusFacebookWrapper;
		
		private static FunplusFacebookAndroid instance;
		private static readonly object locker = new object ();
		
		private FunplusFacebookAndroid ()
		{
			funplusFacebookWrapper = new AndroidJavaClass ("com.funplus.sdk.unity3d.FunplusFacebookWrapper");
		}
		
		public static FunplusFacebookAndroid Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusFacebookAndroid ();
					}
				}
				return instance;
			}
		}
		
		public override void SetGameObject (string gameObjectName)
		{
			funplusFacebookWrapper.CallStatic ("setGameObject", gameObjectName);
		}

		public override bool HasFriendsPermission()
		{
			return funplusFacebookWrapper.CallStatic<bool> ("hasFriendsPermission");
		}

		public override void AskFriendsPermission()
		{
			funplusFacebookWrapper.CallStatic ("askFriendsPermission");
		}

		public override bool HasPublishPermission ()
		{
			return funplusFacebookWrapper.CallStatic<bool> ("hasPublishPermission");
		}

		public override void AskPublishPermission (){
			funplusFacebookWrapper.CallStatic ("askPublishPermission");
		}

		public override void GetUserData ()
		{
			funplusFacebookWrapper.CallStatic ("getUserData");
		}

		public override void GetGameFriends ()
		{
			funplusFacebookWrapper.CallStatic ("getGameFriends");
		}

		public override void GetGameInvitableFriends ()
		{
			funplusFacebookWrapper.CallStatic ("getGameInvitableFriends");
		}

		public override void SendGameRequest (string message)
		{
			funplusFacebookWrapper.CallStatic ("sendGameRequest", message);
		}

		public override void SendGameRequestWithPlatformId (string platformId,string message)
		{
			funplusFacebookWrapper.CallStatic ("sendGameRequestWithPlatformId", platformId, message);
		}

		public override void ShareLink (string title, string description, string url, string imageUrl)
		{
			funplusFacebookWrapper.CallStatic ("shareLink", new object[] {
				title,
				description,
				url,
				imageUrl
			});
		}

		public override void ShareImage (string caption, string imagePath)
		{
			funplusFacebookWrapper.CallStatic ("shareImage", new object[] {
				caption,
				imagePath
			});
		}

		public override void ShareOpenGraphStory ()
		{
			// TODO
		}
	}

}