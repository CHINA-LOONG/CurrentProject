// Mock the AndroidJava to compile on other platforms
#if !UNITY_ANDROID

namespace Funplus {

	class AndroidJavaObject
	{
	}
	
	class AndroidJavaClass
	{
		public AndroidJavaClass(string mock)
		{
		}
		
		public T CallStatic<T>(string method)
		{
			return default(T);
		}
		
		public void CallStatic(string method, params object[] args)
		{
		}

		public T GetStatic<T>(string mock)
		{
			return default(T);
		}
	}
	
}
#endif