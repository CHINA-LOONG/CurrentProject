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

using System.Runtime.InteropServices;
using Funplus.Abstract;

namespace Funplus.IOS
{

	public class FunplusFacebookIOS : BaseFacebookWrapper
	{
		private static FunplusFacebookIOS instance;
		private static readonly object locker = new object ();
		
		public static FunplusFacebookIOS Instance
		{
			get
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusFacebookIOS ();
					}
				}
				return instance;
			}
		}
		
		public override void SetGameObject (string gameObjectName)
		{
			com_funplus_sdk_social_facebook_setGameObject (gameObjectName);
		}

		public override bool HasFriendsPermission()
		{
			return com_funplus_sdk_social_facebook_hasFriendsPermission ();
		}

		public override void AskFriendsPermission()
		{
			com_funplus_sdk_social_facebook_askFriendsPermission ();
		}

		public override bool HasPublishPermission ()
		{
			return com_funplus_sdk_social_facebook_hasPublishPermission ();
		}

		public override void AskPublishPermission ()
		{
			com_funplus_sdk_social_facebook_askPublishPermission ();
		}

		public override void GetUserData ()
		{
			com_funplus_sdk_social_facebook_getUserData ();
		}
		
		public override void GetGameFriends ()
		{
			com_funplus_sdk_social_facebook_getGameFriends ();
		}

		public override void GetGameInvitableFriends ()
		{
			com_funplus_sdk_social_facebook_getGameInvitableFriends ();
		}
		
		public override void SendGameRequest (string message)
		{
			com_funplus_sdk_social_facebook_sendGameRequest (message);
		}

		public override void SendGameRequestWithPlatformId (string platformId,string message)
		{
			com_funplus_sdk_social_facebook_sendGameRequestWithPlatformId (platformId,message );
		}
		
		public override void ShareLink (string title, string description, string url, string imageUrl)
		{
			com_funplus_sdk_social_facebook_shareLink (title, description, url, imageUrl);
		}
		
		public override void ShareImage (string caption, string imagePath)
		{
			com_funplus_sdk_social_facebook_shareImage (caption, imagePath);
		}
		
		public override void ShareOpenGraphStory ()
		{
			// TODO
		}

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_setGameObject (string gameObjectName);

		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_social_facebook_hasFriendsPermission ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_askFriendsPermission ();

		[DllImport ("__Internal")]
		private static extern bool com_funplus_sdk_social_facebook_hasPublishPermission ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_askPublishPermission ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_getUserData ();
		
		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_getGameFriends ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_getGameInvitableFriends ();

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_sendGameRequest (string message);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_sendGameRequestWithPlatformId (string platformId,string message);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_shareLink (string title, string description, string url, string imageUrl);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_shareImage (string caption, string imagePath);

		[DllImport ("__Internal")]
		private static extern void com_funplus_sdk_social_facebook_shareOpenGraphStory ();
	}

}