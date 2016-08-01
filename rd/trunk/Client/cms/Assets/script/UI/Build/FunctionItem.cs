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
        functionDesc.text = functionData.DescAttr;
        int playerLevel = GameDataMgr.Instance.PlayerDataAttr.level;
        if(playerLevel >= functionData.needlevel)
        {
            functionState.text = StaticDataMgr.Instance.GetTextByID("已开启");
        }
        else
        {
            functionState.text = string.Format(StaticDataMgr.Instance.GetTextByID("{0}级开启"),functionData.needlevel);
        }
    }
}
