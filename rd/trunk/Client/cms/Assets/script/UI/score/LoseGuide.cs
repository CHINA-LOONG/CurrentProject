using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class LoseGuide : UIBase
{
    public static string ViewName = "uiLoseGuide";
    public GameObject[] guide;
    string[] loseImage = new string[] {"shibaizhiyin_zhaoyaojing", "shibaizhiyin_choudan", "shibaizhiyin_chongwushengji",
    "shibaizhiyin_chongwujinjie","shibaizhiyin_jinengshengji"
    ,"shibaizhiyin_zhuangbeizhuangbei",
		"shibaizhiyin_dazaozhuangbei","shibaizhiyin_xiangqianbaoshi"};
	List<int> level = new List<int>();
    //----------------------------------------------------------
    public void SetLoseGuide(bool hasWp)
    {
		level.Clear();
        for (int j = 1; j < loseImage.Length; j++)
        {
            if (!LevelLimits.IsOpen(LimitsType.petPromotionLimits) && j == 3)
                continue;
            else if (!LevelLimits.IsOpen(LimitsType.equipstrengthenLimits) && j == 6)
                continue;
            else if (!LevelLimits.IsOpen(LimitsType.equipinlayLimits) && j == 7)
                continue;
            level.Add(j);
        }
        int i = 0;
        int[] randomNumList = new int[3];
        randomNumList[0] = Random.Range(1, level.Count);
        randomNumList[1] = Random.Range(1, level.Count);
        randomNumList[2] = Random.Range(1, level.Count);
        while (randomNumList[0] == randomNumList[1])
        {
            randomNumList[1] = Random.Range(1, level.Count);
        }
        while (randomNumList[2] == randomNumList[1] || randomNumList[2] == randomNumList[0])
        {
            randomNumList[2] = Random.Range(1, level.Count);
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
