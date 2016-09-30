using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class LoseGuide : UIBase
{
    public static string ViewName = "uiLoseGuide";
    public GameObject[] guide;
    string[] loseImage = new string[] {"shibaizhiyin_zhaoyaojing", "shibaizhiyin_choudan", "shibaizhiyin_chongwushengji",
    "shibaizhiyin_chongwujinjie","shibaizhiyin_jinengshengji"
    ,"shibaizhiyin_zhuangbeizhuangbei",
    "shibaizhiyin_dazaozhuangbei","shibaizhiyin_xiangqianbaoshi"};
    //-----------------------------------------------------------
    public void SetLoseGuide(bool hasWp)
    {
        int i = 0;
        int[] randomNumList = new int[3];
        randomNumList[0] = Random.Range(1, 8);
        randomNumList[1] = Random.Range(1, 8);
        randomNumList[2] = Random.Range(1, 8);
        while (randomNumList[0] == randomNumList[1])
        {
            randomNumList[1] = Random.Range(1, 8);
        }
        while (randomNumList[2] == randomNumList[1] || randomNumList[2] == randomNumList[0])
        {
            randomNumList[2] = Random.Range(1, 8);
        }
        LoseGuideData loseGuideData = guide[0].GetComponent<LoseGuideData>();
        if (!hasWp)
        {
            loseGuideData.loseTitle.text = StaticDataMgr.Instance.GetTextByID("lose_guide1");
            loseGuideData.loseHint.text = StaticDataMgr.Instance.GetTextByID("lose_guidetips1");
            loseGuideData.loseImage.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(loseImage[0]);
            loseGuideData.loseType = 0;
            loseGuideData.goToButton.SetActive(false);
            i++;
        }
        for (; i < guide.Length; i++)
        {       
            loseGuideData = guide[i].GetComponent<LoseGuideData>();
            loseGuideData.loseTitle.text = StaticDataMgr.Instance.GetTextByID("lose_guide" + (randomNumList[i] + 1));
            loseGuideData.loseHint.text = StaticDataMgr.Instance.GetTextByID("lose_guidetips" + (randomNumList[i] + 1));
            loseGuideData.loseImage.sprite = ResourceMgr.Instance.LoadAssetType<Sprite>(loseImage[randomNumList[i]]);
            loseGuideData.loseType = randomNumList[i];
        }
    }
}
