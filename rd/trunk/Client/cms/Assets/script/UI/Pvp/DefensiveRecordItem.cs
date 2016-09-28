using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DefensiveRecordItem : MonoBehaviour
{
    public Text resultText;
    public Text scoreText;
    public Text attackerText;
    public Text lvlText;
    public Text gradeLevelText;

    public  static  DefensiveRecordItem CreateWith()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("DefensiveRecordItem");
        DefensiveRecordItem item = go.GetComponent<DefensiveRecordItem>();
        item.InitWith();
        return item;
    } 
    public void InitWith()
    {

    }


}
