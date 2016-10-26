using UnityEngine;
using System.Collections;

using Funplus.Abstract;

namespace Funplus.Stub
{

	public class FunplusFacebookStub : BaseFacebookWrapper
	{
		private static FunplusFacebookStub instance;
		private static readonly object locker = new object ();
		public FunplusSocialUser User { get; set; }
		
		public static FunplusFacebookStub Instance
		{
			get 
			{
				if (instance == null) 
				{
					lock (locker) 
					{
						instance = new FunplusFacebookStub ();
					}
				}
				return instance;
			}
		}
		
		public override void SetGameObject (string gameObjectName)
		{
			Debug.Log ("[funsdk] Calling FunplusFacebookStub.SetGameObject ().");
		}

		public override bool HasFriendsPermission()
		{
			return true;
		}

		public override void AskFriendsPermission()
		{

		}

		public override bool HasPublishPermission ()
		{
			return true;
		}

		public override void AskPublishPermission (){

		}
		
		public override void GetUserData ()
		{
			if (User != null)
			{
				FunplusFacebook.Instance.OnFacebookGetUserDataSuccess (User.ToJsonString ());
			}
			else
			{
				FunplusFacebook.Instance.OnFacebookGetUserDataError (FunplusError.E (2102).ToJsonString ());
			}
		}
		
		public override void GetGameFriends ()
		{
			
		}

		public override void GetGameInvitableFriends ()
		{

		}
		
		public override void SendGameRequest (string message)
		{
		}

		public override void SendGameRequestWithPlatformId (string platformId,string message)
		{
		}
		
		public override void ShareLink (string title, string description, string url, string imageUrl)
		{
		}
		
		public override void ShareImage (string caption, string imagePath)
		{
		}
		
		public override void ShareOpenGraphStory ()
		{
		}
	}

}
