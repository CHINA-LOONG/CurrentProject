using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class PetDetailsAbilities : PetDetailsRight
{
    public static string ViewName = "PetDetailsAbilities";

    public Text text_Title;
    public Text textName;
    public Text textLevel;
    public Text textDescription;
    public Text text_Current;
    public Text text_Next;
    public Text textCurrentDesc;
    public Text textNextDesc;
    public Text textTips;

    public Text text_MaxLevel;
    public GameObject objLevelUP;

    public Image imgCoin;
    public Text textCoin;
    public Button btnLevelUP;
    public Text textLevelUP;

    public Transform spellParent;

    //public GameObject effectLevelUp;
    public Animator animatorEffect;

    private int index = 0;
    private int spellCount;
    private List<AbilitieOptionItem> spellIcons = new List<AbilitieOptionItem>(5) { null,null,null,null,null};
    private AbilitieOptionItem curAbilitie;
    
    private GameUnit curData;
    
    void Start()
    {
        text_Title.text = StaticDataMgr.Instance.GetTextByID("pet_detail_left_skill");
        text_Current.text = StaticDataMgr.Instance.GetTextByID("pet_detail_skill_current_level");
        text_Next.text = StaticDataMgr.Instance.GetTextByID("pet_detail_skill_next_level");
        textLevelUP.text = StaticDataMgr.Instance.GetTextByID("sociaty_levelupup");

        btnLevelUP.onClick.AddListener(OnClickLevelUPBtn);
    }

    public void ReloadData(GameUnit gameUnit, int select = -1)
    {
        SetInitEffect();
        curData = gameUnit;

        StopAllCoroutines();
        StartCoroutine(IncreasePoint());

        ReloadSpell(curData.spellList);
        index = (select == -1) ? index : select;
        ReloadAbilitie(spellIcons[index]);
    }

    void ReloadSpell(Dictionary<string,Spell> spellDict)
    {
        spellCount = 0;//最大5个

        UnitData unit = StaticDataMgr.Instance.GetUnitRowData(curData.pbUnit.id);
        List<SpellProtoType> spellList = unit.SpellList;

        for (int i = 0; i < spellList.Count; i++)
        {
            if (null == spellList[i])
            {
                continue;
            }
            if (string.IsNullOrEmpty(spellList[i].tips))
            {
                continue;
            }
            if (spellDict.ContainsKey(spellList[i].id))
            {
                if (null == spellIcons[spellCount])
                {
                    GameObject go = ResourceMgr.Instance.LoadAsset("AbilitieOptionItem");
                    UIUtil.SetParentReset(go.transform, spellParent);
                    spellIcons[spellCount] = go.GetComponent<AbilitieOptionItem>();
                    ScrollViewEventListener.Get(go).onClick = OnClickAbiliteOption;
                }
                else
                {
                    spellIcons[spellCount].gameObject.SetActive(true);
                }
                spellIcons[spellCount].ReloadData(spellDict[spellList[i].id]);
                spellCount++;
                if (spellCount >= spellIcons.Count)
                {
                    break;
                }
            }
        }
        for (int i = spellCount; i < spellIcons.Count; i++)
        {
            if (null != spellIcons[i])
            {
                spellIcons[i].gameObject.SetActive(false);
            }
        }
    }
    void ReloadAbilitie(AbilitieOptionItem abilitie)
    {
        if (curAbilitie != null)
        {
            curAbilitie.IsSelect = false;
        }
        curAbilitie = abilitie;
        curAbilitie.IsSelect = true;
        index = spellIcons.IndexOf(abilitie);

        Spell spell = curAbilitie.curData;
        SpellProtoType spellData = spell.spellData;
        textName.text = spellData.NameAttr;
        textDescription.text = spellData.TipsDescriptionAttr;

        if (spellData.category == (int)SpellType.Spell_Type_Passive ||
            spellData.category == (int)SpellType.Spell_Type_Beneficial ||
            spellData.category == (int)SpellType.Spell_Type_Negative ||
            string.IsNullOrEmpty(spellData.tipsCurlvl)
            )
        {
            textLevel.gameObject.SetActive(false);
            text_Current.gameObject.SetActive(false);
            text_Next.gameObject.SetActive(false);
            objLevelUP.SetActive(false);
            text_MaxLevel.gameObject.SetActive(true);
            text_MaxLevel.text = StaticDataMgr.Instance.GetTextByID("pet_detail_skill_cannt_level");
        }
        else
        {
            textLevel.gameObject.SetActive(true);
            text_Current.gameObject.SetActive(true);
            textLevel.text = string.Format("LVL({0}/{1})", spell.level, GameConfig.MaxMonsterLevel);
            textCurrentDesc.text = string.Format(spellData.TipsCurlvlAttr, ((spellData.baseTipValue + ((spell.level-1) * spellData.levelAdjust))*100).ToString("G"));
            if (spell.level >= GameConfig.MaxMonsterLevel)
            {
                text_MaxLevel.gameObject.SetActive(true);
                text_Next.gameObject.SetActive(false);
                text_MaxLevel.text = StaticDataMgr.Instance.GetTextByID("pet_detail_skill_max_level");
                objLevelUP.SetActive(false);
            }
            else
            {
                text_MaxLevel.gameObject.SetActive(false);
                text_Next.gameObject.SetActive(true);
                objLevelUP.SetActive(true);
                textNextDesc.text = string.Format(spellData.TipsNextlvlAttr, ((spellData.baseTipValue + spell.level * spellData.levelAdjust)*100).ToString("G"));
                textCoin.text = StaticDataMgr.Instance.GetSPellLevelPrice(spell.level + 1).ToString();
                //判断金币
                if (StaticDataMgr.Instance.GetSPellLevelPrice(spell.level + 1) > GameDataMgr.Instance.PlayerDataAttr.coin)
                {
                    textCoin.color = ColorConst.outline_color_nReq;
                }
                else
                {
                    textCoin.color = ColorConst.system_color_black;
                }
            }
        }
    }



    void OnClickAbiliteOption(GameObject go)
    {
        AbilitieOptionItem abilitie = go.GetComponent<AbilitieOptionItem>();
        ReloadAbilitie(abilitie);
    }
    void OnClickLevelUPBtn()
    {
        Spell spell = curAbilitie.curData;
        //判断金币
        if (StaticDataMgr.Instance.GetSPellLevelPrice(spell.level + 1) > GameDataMgr.Instance.PlayerDataAttr.coin)
        {
            GameDataMgr.Instance.ShopDataMgrAttr.JinbiNoEnough();
        }
        //判断宠物等级
        else if (spell.level >= curData.pbUnit.level)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("monster_record_013"), (int)PB.ImType.PROMPT);
        }
        //判断技能点
        else if (StatisticsDataMgr.Instance.GetSkillPoint() < 1)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("monster_record_014"), (int)PB.ImType.PROMPT);
        }
        else
        {
            PB.HSMonsterSkillUp param = new PB.HSMonsterSkillUp();
            param.monsterId = curData.pbUnit.guid;
            param.skillId = spell.spellData.id;
            GameApp.Instance.netManager.SendMessage(PB.code.MONSTER_SKILL_UP_C.GetHashCode(), param);
        }
    }


    void OnMonsterSkillUpReturn(ProtocolMessage msg)
    {
        UINetRequest.Close();

        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            PB.HSErrorCode error = msg.GetProtocolBody<PB.HSErrorCode>();

            if (error.errCode == (int)PB.monsterError.SKILL_POINT_NOT_ENOUGH)
            {
                MsgBox.PromptMsg.Open(MsgBox.MsgBoxType.Conform,
                                      StaticDataMgr.Instance.GetTextByID("pet_skill_point_not_enough"));
            }
            return;
        }

        //effectLevelUp.gameObject.SetActive(true);
        //Tweener tweener = effectLevelUp.transform.DOScale(2.0f, 0.5f)
        //                                         .SetEase(Ease.Linear)
        //                                         .OnComplete(delegate ()
        //                                                    {
        //                                                        SetInitEffect();
        //                                        levelup            });
        animatorEffect.SetTrigger("levelup");
        curAbilitie.PlayEffect();


        PB.HSMonsterSkillUpRet skillUpResponse = msg.GetProtocolBody<PB.HSMonsterSkillUpRet>();
        curData.spellList[curAbilitie.curData.spellData.id].level += 1;
        curData.LevelUpdateSpell(curAbilitie.curData.spellData.id);

        StatisticsDataMgr.Instance.ResetSkillPointState(skillUpResponse.skillPoint, skillUpResponse.skillPointTimeStamp);
        ReloadAbilitie(curAbilitie);
    }

    void SetInitEffect()
    {
        //effectLevelUp.SetActive(false);
        //effectLevelUp.transform.localScale = Vector3.one;
    }

    void OnCoinChangedRefresh(long coin)
    {
        ReloadData(curData);
    }
    void OnEnable()
    {
        BindLisetener();
    }
    void OnDisable()
    {
        UnBindListener();
    }
    void BindLisetener()
    {
        GameEventMgr.Instance.AddListener<long>(GameEventList.CoinChanged, OnCoinChangedRefresh);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_S.GetHashCode().ToString(), OnMonsterSkillUpReturn);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_C.GetHashCode().ToString(), OnMonsterSkillUpReturn);
    }
    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<long>(GameEventList.CoinChanged, OnCoinChangedRefresh);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_S.GetHashCode().ToString(), OnMonsterSkillUpReturn);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.MONSTER_SKILL_UP_C.GetHashCode().ToString(), OnMonsterSkillUpReturn);
    }

    
    IEnumerator IncreasePoint()
    {
        string currentPoint;
        string currentDesc;
        while (true)
        {
            currentPoint = string.Format(StaticDataMgr.Instance.GetTextByID("pet_detail_skill_current_point")+"{0}", StatisticsDataMgr.Instance.GetSkillPoint());
            if (StatisticsDataMgr.Instance.isMaxPoint() == false)
            {
                int refreshTime = StatisticsDataMgr.Instance.GetSkillPointLeftTime();
                if (refreshTime> GameConfig.SkillPointTime)
                {
                    Logger.LogError("更新时间有误差:"+ (refreshTime - GameConfig.SkillPointTime).ToString());
                    refreshTime = GameConfig.SkillPointTime;
                }
                currentDesc = string.Format(StaticDataMgr.Instance.GetTextByID("pet_detail_skill_current_huifu"), string.Format("{0:D2}:{1:D2}", refreshTime / 60, refreshTime % 60));
            }
            else
            {
                currentDesc = StaticDataMgr.Instance.GetTextByID("pet_detail_skill_max_point");
            }
            textTips.text = currentPoint + currentDesc;
            yield return new WaitForSeconds(1);
        }
    }

}
