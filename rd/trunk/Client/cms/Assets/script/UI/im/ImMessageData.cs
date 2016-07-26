using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImMessageData : MonoBehaviour {

    public Text mChannel;
    public Text mContent;
    public Text mSpeaker;
    [HideInInspector]
    public int speakerID;
}
