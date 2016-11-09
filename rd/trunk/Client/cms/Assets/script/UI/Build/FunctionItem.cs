using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FunctionItem : MonoBehaviour
{
    public Text functionName;
    public Text functionDesc;
    public Text functionState;


    public  static  FunctionItem    CreateWith(FunctionData functionData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("FunctionItem");
        var funItem = go.GetComponent<FunctionItem>();
        funItem.InitWith(functionData);
        return funItem;
    }

    public  void    InitWith(FunctionData functionData)
    {
        functionName.text = functionData.NameAttr;
       // functionDesc.text = functionData.DescAttr;
        //int playerLevel = GameDataMgr.Instance.PlayerDataAttr.LevelAttr;
        //if(playerLevel >= functionData.needlevel)
        //{
        //    functionState.text = StaticDataMgr.Instance.GetTextByID("main_levelup_kaiqi");
        //    functionState.color = new Color(0, 1, 0);
        //}
        //else
        //{
        //    functionState.text = string.Format(StaticDataMgr.Instance.GetTextByID("main_levelup_weikaiqi"),functionData.needlevel);
        //    functionState.color = new Color(1, 0, 0);
        //}
    }
}
