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

    public  static  DefensiveRecordItem Create()
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("DefensiveRecordItem");
        DefensiveRecordItem item = go.GetComponent<DefensiveRecordItem>();
        return item;
    } 
    public void RefreshWith(PB.PVPDefenceRecordData defenseData)
    {
        if(defenseData.result == (int)PB.PvpResult.WIN)
        {
            resultText.text = StaticDataMgr.Instance.GetTextByID("pvp_resultwin");
        }
        else if(defenseData.result == (int)PB.PvpResult.LOSE)
        {
            resultText.text = StaticDataMgr.Instance.GetTextByID("pvp_resultlose");
        }
        else 
        {
            resultText.text = StaticDataMgr.Instance.GetTextByID("pvp_resultdraw");
        }
        scoreText.text =  defenseData.changePoint.ToString();
        attackerText.text = defenseData.attacker;
        lvlText.text = defenseData.level.ToString();
        gradeLevelText.text = defenseData.point.ToString();

        if (defenseData.changePoint >= 0)
        {
            scoreText.color = new Color(56.0f / 255.0f, 155.0f / 255.0f, 7.0f / 255.0f);
            if(defenseData.point > 0)
            {
                scoreText.text = string.Format("+{0}", defenseData.changePoint);
            }
        }
        else
        {
            scoreText.color = new Color(1, 0, 0);
        }
    }


}
