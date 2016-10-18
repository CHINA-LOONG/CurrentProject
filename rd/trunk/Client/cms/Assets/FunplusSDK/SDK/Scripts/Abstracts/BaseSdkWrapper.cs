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

namespace Funplus.Abstract
{

	abstract public class BaseSdkWrapper
	{
		abstract public void SetGameObject (string gameObjectName);

		abstract public void Install (string gameId, string gameKey, string environment);
		abstract public void SetDebug (bool isDebug);
		abstract public bool IsSdkInstalled ();
		abstract public bool IsFirstLaunch ();

		abstract public void LogUserLogin (string uid);
		abstract public void LogNewUser (string uid);
		abstract public void LogUserLogout ();
		abstract public void LogUserInfoUpdate (string serverId, string userId, string userName, string userLevel, string userVipLevel, bool isPaidUser);
		abstract public void LogPayment (string productId, string throughCargo, string purchaseData);
	}

}