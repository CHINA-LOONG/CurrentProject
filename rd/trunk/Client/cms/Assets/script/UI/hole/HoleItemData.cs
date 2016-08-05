using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HoleItemData : MonoBehaviour
{
    public Image difficultyImage;
    public Text difficultyText;
    public Text vitalityNumText;
    public GameObject leveLimit;
    public GameObject consume;
    public GameObject[] difficulty;
    [HideInInspector]
    public string fbID;
    [HideInInspector]
    public int holeLevel;
}
