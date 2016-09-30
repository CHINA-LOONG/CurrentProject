using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LoseGuideData : MonoBehaviour
{
    public Text loseTitle;
    public Image loseImage;
    public Text loseHint;
    public GameObject goToButton;
    public int loseType;
    void Start()
    {
        EventTriggerListener.Get(goToButton).onClick = LoseGoToClick;
    }
    //-----------------------------------------------------------
    void LoseGoToClick(GameObject go)
    {
        int loseGoType = go.transform.parent.gameObject.GetComponent<LoseGuideData>().loseType;
        if (loseGoType == 2)
            BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_Summon);
        else
            BattleController.Instance.UnLoadBattleScene(ExitInstanceType.Exit_Instance_Pet);
    }
}
