#pragma strict

function Start () {

}

function OnGUI () {
var translation : float = Time.deltaTime * 60;
//var tmp = 150 * Time.fixedDeltaTime;
var tmp =8;
//if(GUI.Button(Rect(400,360,100,50),"D")||Input.GetKey(KeyCode.D))
if(Input.GetKey(KeyCode.D))
{
//transform.Rotate(Vector3.up * Time.deltaTime*(-2000));
	//transform.Translate(translation,0,0);
	GetComponent.<Rigidbody>().AddForce(tmp,0,0);
	//GetComponent.<Rigidbody>().velocity = new Vector3(1,0,0)* tmp;
}
//if(GUI.Button(Rect(200,360,100,50),"A")||Input.GetKey(KeyCode.A))
if(Input.GetKey(KeyCode.A))
{
	//transform.Translate(-translation,0,0);
	GetComponent.<Rigidbody>().AddForce(-tmp,0,0);
	//GetComponent.<Rigidbody>().velocity = new Vector3(-1,0,0)* tmp;
}


//if(GUI.Button(Rect(300,300,100,50),"W")||Input.GetKey(KeyCode.W))
if(Input.GetKey(KeyCode.W))
{
	//transform.Translate(0,0,translation);
 GetComponent.<Rigidbody>().AddForce(0,0,tmp);
	//GetComponent.<Rigidbody>().velocity = new Vector3(0,0,1)* tmp;
}
//if(GUI.Button(Rect(300,360,100,50),"S")||Input.GetKey(KeyCode.S))
if(Input.GetKey(KeyCode.S))
{
	//transform.Translate(0,0,-translation);
	GetComponent.<Rigidbody>().AddForce(0,0,-tmp);
   // GetComponent.<Rigidbody>().velocity = new Vector3(0,0,-1)* tmp;
}
}