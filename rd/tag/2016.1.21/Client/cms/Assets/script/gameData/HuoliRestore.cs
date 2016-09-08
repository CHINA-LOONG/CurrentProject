using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HuoliRestore : MonoBehaviour
{
    void Start()
    {

    }
    bool isRestoreing = false;
    public  bool IsRestoring
    {
        get
        {
            return isRestoreing;
        }
        set
        {
            isRestoreing = value;
            if(isRestoreing)
            {
                StartCoroutine("RestoreHuoliCo");
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }

    IEnumerator RestoreHuoliCo()
    {
        PlayerData playerData =  GameDataMgr.Instance.PlayerDataAttr;
        int restoreTime = GameConfig.Instance.RestoreHuoLiNeedSeconds;//10分钟 
        while(true)
        {
            int lastRestore = playerData.HuoliBegintimeAttr;
            int curTime = GameTimeMgr.Instance.GetServerTimeStamp();
            int delta = curTime - lastRestore;
            int addHuoli = delta / restoreTime;
            if(addHuoli >= 1)
            {
                playerData.UpdateHuoli(playerData.HuoliAttr + addHuoli, lastRestore + addHuoli * restoreTime);
            }
            yield return new WaitForSeconds(1.5f);
        }
    }
}
