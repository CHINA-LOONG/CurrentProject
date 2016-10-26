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

﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Funplus.Abstract;
using Funplus.Android;
using Funplus.IOS;
using Funplus.Stub;

namespace Funplus
{
	/// <summary>
	/// Facebook wrapper used in the Funplus SDK.
	/// </summary>
	public class FunplusFacebook : MonoBehaviour
	{
		/// <summary>
		/// This interface defines methods which will be called
		/// when any Facebook action completes.
		/// </summary>
		public interface IDelegate
		{
			/// <summary>
			/// This method will be called when SDK succeeds or fails to request friends permission.
			/// </summary>
			/// <param name="result">If set to <c>true</c> result.</param>
			void OnAskFriendsPermission (bool result);

			/// <summary>
			/// This method will be called when SDK succeeds or fails to request publish permission.
			/// </summary>
			/// <param name="result">If set to <c>true</c> result.</param>
			void OnAskPublishPermission (bool result);

			/// <summary>
			/// This method will be called when SDK succeeds to retrieve player's Facebook profiles. 
			/// </summary>
			/// <param name="user">User.</param>
			void OnGetUserDataSuccess (FunplusSocialUser user);

			/// <summary>
			/// This method will be called when SDK failes to retrieve player's Facebook profiles.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnGetUserDataError (FunplusError error);

			/// <summary>
			/// This method will be called when SDK succeeds to retrieve player's Facebook friends.
			/// </summary>
			/// <param name="friends">Friends.</param>
			void OnGetGameFriendsSuccess (List<FunplusFBFriend> friends);

			/// <summary>
			/// This method will be called when SDK fails to retrieve player's Facebook friends.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnGetGameFriendsError (FunplusError error);

			/// <summary>
			/// This method will be called when SDK succeeds to retrieve player's Facebook invitable friends.
			/// </summary>
			/// <param name="friends">Friends.</param>
			void OnGetGameInvitableFriendsSuccess (List<FunplusFBFriend> friends);

			/// <summary>
			/// This method will be called when SDK fails to retrieve player's Facebook invitable friends.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnGetGameInvitableFriendsError (FunplusError error);

			/// <summary>
			/// This method will be called when SDK succeeds to send a Facebook game request.
			/// </summary>
			/// <param name="result">Result.</param>
			void OnSendGameRequestSuccess (string result);

			/// <summary>
			/// This method will be called when SDK fails to send a Facebook game request. 
			/// </summary>
			/// <param name="error">Error.</param>
			void OnSendGameRequestError (FunplusError error);

			/// <summary>
			/// This method will be called when SDK succeeds to share to Facebook.
			/// </summary>
			/// <param name="result">Result.</param>
			void OnShareSuccess (string result);

			/// <summary>
			/// This method will be called when SDK fails to share to Facebook.
			/// </summary>
			/// <param name="error">Error.</param>
			void OnShareError (FunplusError error);
		}

		public static FunplusFacebook Instance { get; set; }
		private static IDelegate _delegate;

		void Awake ()
		{
			Instance = this;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		[Obsolete("This method is deprecated, please use FunplusFacebook.Instance instead.")]
		public static FunplusFacebook GetInstance ()
		{
			return Instance;
		}

		private BaseFacebookWrapper Wrapper 
		{
			get
			{
				#if UNITY_EDITOR
				return FunplusFacebookStub.Instance;
				#elif UNITY_ANDROID
				return FunplusFacebookAndroid.Instance;
				#elif UNITY_IOS
				return FunplusFacebookIOS.Instance;
				#else
				return FunplusFacebookStub.Instance;
				#endif
			}
		}

		public FunplusFacebook SetDelegate (IDelegate sdkDelegate)
		{
			if (sdkDelegate == null)
			{
				Debug.LogError ("[funsdk] sdkDelegate must not be null.");
				return Instance;
			}
			SetGameObjectAndDelegate (this.gameObject.name, sdkDelegate);

			return Instance;
		}

		/// <summary>
		/// Sets the game object and delegate to the Facebook module.
		/// </summary>
		/// <param name="gameObjectName">Name of the game object.</param>
		/// <param name="sdkDelegate">Sdk delegate.</param>
		public void SetGameObjectAndDelegate (string gameObjectName, IDelegate facebookDelegate)
		{
			if (!FunplusSdk.Instance.IsSdkInstalled ()) 
			{
				Debug.LogError ("[funsdk] Please call FunplusSdk.Instance.Install() first.");
				return;
			}
			
			if (_delegate == null) 
			{
				_delegate = facebookDelegate;
				Wrapper.SetGameObject (gameObjectName);
			} 
			else 
			{
				Debug.LogWarning ("[funsdk] Delegate has already been set.");
			}
		}
			
		public bool HasFriendsPermission ()
		{
			return Wrapper.HasFriendsPermission ();
		}

		/// <summary>
		/// Asks the Facebook friends permission.
		/// </summary>
		public void AskFriendsPermission ()
		{
			Wrapper.AskFriendsPermission ();
		}

		public bool HasPublishPermission ()
		{
			return Wrapper.HasPublishPermission ();
		}

		/// <summary>
		/// Asks the Facebook publish permission.
		/// </summary>
		public void AskPublishPermission ()
		{
			Wrapper.AskPublishPermission ();
		}

		/// <summary>
		/// Gets player's Facebook profiles.
		/// </summary>
		public void GetUserData ()
		{
			Wrapper.GetUserData ();
		}

		/// <summary>
		/// Gets player's Facebook friends.
		/// </summary>
		public void GetGameFriends ()
		{
			Wrapper.GetGameFriends ();
		}

		/// <summary>
		/// Gets player's Facebook invitable friends.
		/// </summary>
		public void GetGameInvitableFriends ()
		{
			Wrapper.GetGameInvitableFriends ();
		}

		/// <summary>
		/// Sends a game request to a selected group of friends.
		/// </summary>
		/// <param name="message">Message.</param>
		public void SendGameRequest (string message)
		{
			Wrapper.SendGameRequest (message);
		}

		/// <summary>
		/// Sends a game request to a specific friend indicated by the <c>platformId</c>.
		/// </summary>
		/// <param name="platformId">Platform identifier.</param>
		/// <param name="message">Message.</param>
		public void SendGameRequestWithPlatformId (string platformId,string message)
		{
			Wrapper.SendGameRequestWithPlatformId (platformId, message);
		}
	
		/// <summary>
		/// Shares a link to Facebook.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="description">Description.</param>
		/// <param name="url">URL.</param>
		/// <param name="imageUrl">Image URL.</param>
		public void ShareLink (string title, string description, string url, string imageUrl)
		{
			Wrapper.ShareLink (title,description,url,imageUrl);
		}

		/// <summary>
		/// Shares an image to Facebook.
		/// </summary>
		/// <param name="imagePath">Image path.</param>
		/// <param name="message">Message.</param>
		/// <param name="url">URL.</param>
		public void ShareImage (string imagePath, string message, string url)
		{
		}

		/// <summary>
		/// Shares an open graph story to Facebook.
		/// </summary>
		public void ShareOpenGraphStory ()
		{
			
		}

		//\cond
		#region callbacks
		public void OnFacebookAskFriendsPermission (string message)
		{
			if (message == "true") {
				_delegate.OnAskFriendsPermission (true);
			} else {
				_delegate.OnAskFriendsPermission (false);
			}
		}

		public void OnFacebookAskPublishPermission (string message)
		{
			if (message == "true") {
				_delegate.OnAskPublishPermission (true);
			} else {
				_delegate.OnAskPublishPermission (false);
			}
		}
			
		public void OnFacebookGetUserDataSuccess (string message)
		{
			_delegate.OnGetUserDataSuccess (FunplusSocialUser.GetUserDataFromMessage(message));
		}

		public void OnFacebookGetUserDataError (string message)
		{
			_delegate.OnGetUserDataError (FunplusError.FromMessage(message));
		}
			
		public void OnFacebookGetGameFriendsSuccess (string message)
		{
			_delegate.OnGetGameFriendsSuccess (FunplusFBFriend.FromFacebookFriendsMessage(message));
		}

		public void OnFacebookGetGameFriendsError (string message)
		{
			_delegate.OnGetGameFriendsError (FunplusError.FromMessage(message));
		}

		public void OnFacebookGetGameInvitableFriendsSuccess (string message)
		{
			_delegate.OnGetGameInvitableFriendsSuccess (FunplusFBFriend.FromFacebookFriendsMessage(message));
		}

		public void OnFacebookGetGameInvitableFriendsError (string message)
		{
			_delegate.OnGetGameInvitableFriendsError (FunplusError.FromMessage(message));
		}
			
		public void OnFacebookSendGameRequestSuccess (string message)
		{
			_delegate.OnSendGameRequestSuccess (message);
		}

		public void OnFacebookSendGameRequestError (string message)
		{
			_delegate.OnSendGameRequestError (FunplusError.FromMessage(message));
		}
			
		public void OnFacebookShareSuccess (string message)
		{
			_delegate.OnShareSuccess (message);
		}

		public void OnFacebookShareError (string message)
		{
			_delegate.OnShareError (FunplusError.FromMessage(message));
		}
		#endregion //callbacks
	}

}