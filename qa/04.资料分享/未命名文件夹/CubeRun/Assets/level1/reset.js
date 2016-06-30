#pragma strict

function Start () {

}

function Update () {

}
function OnGUI () {

//var tmp = 150 * Time.fixedDeltaTime;
var tmp =8;
//if(GUI.Button(Rect(600,280,100,50),"RESET")||Input.GetKey(KeyCode.R))
if(Input.GetKey(KeyCode.R))
{
Application.LoadLevelAsync(0);
}
}