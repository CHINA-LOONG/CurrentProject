using UnityEngine;
using System.Collections;

public class LastEvenType : MonoBehaviour 
{

	EventType  eventType = EventType.MouseDown;

	static LastEvenType instance = null;
	public static LastEvenType Instance
	{
		get
		{
			return instance;
		}
	}

	public bool	IsDrag()
	{
		return eventType == EventType.MouseDrag;
	}

	// Use this for initialization
	void Start ()
	{
		instance = this;
	}

	void OnGUI()
	{
		if (Event.current.type == EventType.mouseDown)
		{
			eventType = EventType.MouseDown;
			//Debug.LogError ("--------------------click");
		}

		if (Event.current.type == EventType.MouseDrag)
		{
			eventType = EventType.MouseDrag;
			//Debug.LogError("Drag---------------------");
		}
	}

}
