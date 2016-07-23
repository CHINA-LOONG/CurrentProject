using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NeedMonsterElement : MonoBehaviour
{
    public Button button;
    public Transform monsterPos;
    private MonsterIcon monsterIcon;
    public Text monsterCount;

    private string monsterId;
    private int monsterStage;

    public void Refresh(string monsterId,int monsterStage,int needCount,int curCount)
    {
        this.monsterId = monsterId;
        this.monsterStage = monsterStage;

        UpdateCount(needCount, curCount);
    }

    public void UpdateCount(int needCount, int curCount)
    {
        curCount = Mathf.Clamp(curCount, 0, 9999);
        monsterCount.text = curCount + "/" + needCount;
        if (needCount > curCount)
        {
            monsterCount.color = ColorConst.text_color_nReq;
        }
        else
        {
            monsterCount.color = ColorConst.text_color_Req;
        }
        if (curCount > 0)
        {
            if (monsterIcon == null)
            {
                monsterIcon = MonsterIcon.CreateIcon();
                UIUtil.SetParentReset(monsterIcon.transform, monsterPos);
                monsterIcon.SetMonsterStaticId(monsterId);
                monsterIcon.SetStage(monsterStage);
            }
            else if (!monsterIcon.gameObject.activeSelf)
            {
                monsterIcon.gameObject.SetActive(true);
                monsterIcon.Init();
                monsterIcon.SetMonsterStaticId(monsterId);
                monsterIcon.SetStage(monsterStage);
            }
        }
        else if (monsterIcon != null && monsterIcon.gameObject.activeSelf)
        {
            monsterIcon.gameObject.SetActive(false);
        }
    }


}
