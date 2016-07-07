//#define LOG_ALL_MESSAGES
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE
//#define REQUIRE_LISTENER

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameEventMgr
{
	static GameEventMgr mInst = null;
	public static GameEventMgr Instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = new GameEventMgr();
			}
			return mInst;
		}
	}

	public void Init()
	{

	}
	public Dictionary<string, Delegate> mEventTable = new Dictionary<string, Delegate>();
	public List<string> mPermanentMessages = new List<string> ();
	public void MarkAsPermanent(string eventType) 
	{
		#if LOG_ALL_MESSAGES
		Logger.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
		#endif
		mPermanentMessages.Add(eventType);
	}

	public void Cleanup()
	{
		#if LOG_ALL_MESSAGES
		Logger.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
		#endif
		
		List<string> messagesToRemove = new List<string>();
		
		foreach (KeyValuePair<string, Delegate> pair in mEventTable) 
		{
			bool wasFound = false;
			
			foreach (string message in mPermanentMessages) 
			{
				if (pair.Key == message) 
				{
					wasFound = true;
					break;
				}
			}
			
			if (!wasFound)
				messagesToRemove.Add( pair.Key );
		}
		foreach (string message in messagesToRemove) 
		{
			mEventTable.Remove( message );
		}
	}

	public void PrintEventTable()
	{
		Logger.Log("\t\t\t=== MESSENGER PrintEventTable ===");
		
		foreach (KeyValuePair<string, Delegate> pair in mEventTable) {
			Logger.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
		}
		
		Logger.Log("\n");
	}

	public void OnListenerAdding(string eventType, Delegate listenerBeingAdded) 
	{
		#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
		Logger.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
		#endif
		
		if (!mEventTable.ContainsKey(eventType)) {
			mEventTable.Add(eventType, null );
		}
		
		Delegate d = mEventTable[eventType];
		if (d != null && d.GetType() != listenerBeingAdded.GetType()) {
			throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
		}
	}

	public void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved) {
		#if LOG_ALL_MESSAGES
		Logger.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
		#endif
		
		if (mEventTable.ContainsKey(eventType)) 
		{
			Delegate d = mEventTable[eventType];
			
			if (d == null) 
			{
				throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
			} 
			else if (d.GetType() != listenerBeingRemoved.GetType()) 
			{
				throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
			}
		} 
		else 
		{
			throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
		}
	}
	
	public void OnListenerRemoved(string eventType) 
	{
		if (mEventTable[eventType] == null) {
			mEventTable.Remove(eventType);
		}
	}

	public void OnFireEvent(string eventType) {
		#if REQUIRE_LISTENER
		if (!mEventTable.ContainsKey(eventType)) {
			throw new FireEventException(string.Format("FireEvent message \"{0}\" but no listener found. Try marking the message with Messenger.MarkAsPermanent.", eventType));
		}
		#endif
	}
	
	public FireEventException CreateFireEventSignatureException(string eventType) 
	{
		return new FireEventException(string.Format("FireEvent message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
	}
	
	public class FireEventException : Exception 
	{
		public FireEventException(string msg)
		: base(msg) 
		{
		}
	}
	
	public class ListenerException : Exception 
	{
		public ListenerException(string msg)
		: base(msg) 
		{
		}
	}

	//No parameters
	public void AddListener(string eventType, Callback handler) 
	{
		OnListenerAdding(eventType, handler);
		mEventTable[eventType] = (Callback)mEventTable[eventType] + handler;
	}
	
	//Single parameter
	public void AddListener<T>(string eventType, Callback<T> handler) 
	{
		OnListenerAdding(eventType, handler);
		mEventTable[eventType] = (Callback<T>)mEventTable[eventType] + handler;
	}
	
	//Two parameters
	public void AddListener<T, U>(string eventType, Callback<T, U> handler) 
	{
		OnListenerAdding(eventType, handler);
		mEventTable[eventType] = (Callback<T, U>)mEventTable[eventType] + handler;
	}
	
	//Three parameters
	public void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler) 
	{
		OnListenerAdding(eventType, handler);
		mEventTable[eventType] = (Callback<T, U, V>)mEventTable[eventType] + handler;
	}

	//No parameters
	public void RemoveListener(string eventType, Callback handler) 
	{
		OnListenerRemoving(eventType, handler);   
		mEventTable[eventType] = (Callback)mEventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}
	
	//Single parameter
	public void RemoveListener<T>(string eventType, Callback<T> handler) 
	{
		OnListenerRemoving(eventType, handler);
		mEventTable[eventType] = (Callback<T>)mEventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}
	
	//Two parameters
	public void RemoveListener<T, U>(string eventType, Callback<T, U> handler) 
	{
		OnListenerRemoving(eventType, handler);
		mEventTable[eventType] = (Callback<T, U>)mEventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}
	
	//Three parameters
	public void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler) 
	{
		OnListenerRemoving(eventType, handler);
		mEventTable[eventType] = (Callback<T, U, V>)mEventTable[eventType] - handler;
		OnListenerRemoved(eventType);
	}

	//No parameters
	public void FireEvent(string eventType) 
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Logger.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnFireEvent(eventType);
		Delegate d;
		if (mEventTable.TryGetValue(eventType, out d)) {
			Callback callback = d as Callback;
			
			if (callback != null) {
				callback();
			} else {
				throw CreateFireEventSignatureException(eventType);
			}
		}
	}
	
	//Single parameter
	public void FireEvent<T>(string eventType, T arg1) 
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Logger.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnFireEvent(eventType);
		
		Delegate d;
		if (mEventTable.TryGetValue(eventType, out d)) {
			Callback<T> callback = d as Callback<T>;
			
			if (callback != null) {
				callback(arg1);
			} else {
				throw CreateFireEventSignatureException(eventType);
			}
		}
	}
	
	//Two parameters
	public void FireEvent<T, U>(string eventType, T arg1, U arg2) 
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Logger.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnFireEvent(eventType);
		
		Delegate d;
		if (mEventTable.TryGetValue(eventType, out d)) {
			Callback<T, U> callback = d as Callback<T, U>;
			
			if (callback != null) {
				callback(arg1, arg2);
			} else {
				throw CreateFireEventSignatureException(eventType);
			}
		}
	}
	
	//Three parameters
	public void FireEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3) 
	{
		#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
		Logger.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
		#endif
		OnFireEvent(eventType);
		
		Delegate d;
		if (mEventTable.TryGetValue(eventType, out d)) {
			Callback<T, U, V> callback = d as Callback<T, U, V>;
			
			if (callback != null) {
				callback(arg1, arg2, arg3);
			} else {
				throw CreateFireEventSignatureException(eventType);
			}
		}
	}
}
