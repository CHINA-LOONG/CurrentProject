using UnityEngine;
using System.Collections.Generic;

public class SpeechStaticData
{
    public string id;
    public string skip;
    public string campType;
    public string name;
    public string image;
    public string speakId;
}

public class SpeechData
{
    public string id;
    public string skip;
    public List<SpeechStaticData> speechList = new List<SpeechStaticData>();

}