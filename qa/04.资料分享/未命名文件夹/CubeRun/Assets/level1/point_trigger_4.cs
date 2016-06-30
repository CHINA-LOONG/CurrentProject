using UnityEngine;
using System.Collections;

public class point_trigger_4 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider collider)
	{
		//GameObject.Destroy(GameObject.Find ("door_1"));
		transform.GetChild (0).gameObject.SetActive (true);
		GameObject.Find ("door_4").transform.RotateAround (new Vector3(-1,0.625f,4f),new Vector3(0,1,0),-90f);
	}
	void OnTriggerExit(Collider collider){
		transform.GetChild (0).gameObject.SetActive (false);
		GameObject.Find ("door_4").transform.RotateAround (new Vector3(-1,0.625f,4f),new Vector3(0,1,0),90f);
	}
}
