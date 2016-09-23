using UnityEngine;
using System.Collections.Generic;

public class SpeechStaticData
{
    public string id;
    public string skip;
    public string campType;
    public string name;
    public string image;
    public string face;
    public string position;
    public string speakId;

    private Vector2 facePos=Vector2.zero;

    public Vector2 FacePos
    {
        get
        {
            if (facePos==Vector2.zero&&!string.IsNullOrEmpty(position))
            {
                string[] strPos= position.Split(',');
                facePos.x = int.Parse(strPos[0]);
                facePos.y = int.Parse(strPos[1]);
            }
            return facePos;
        }
    }
}

public class SpeechData
{
    public string id;
    public string skip;
    public List<SpeechStaticData> speechList = new List<SpeechStaticData>();

}