using UnityEngine;
using System.Collections;

public class PetViewConst{

    public enum SortType
    {
        ALLTYPE = 0,
        GOLDTYPE,
        WOODTYPE,
        WATERTYPE,
        FIRETYPE,
        EARTHTYPE
    }

    public enum RightPanelType
    {
        NULL_RIGHT_TYPE,
        DETAIL_ATTR_TYPE,
        SKILL_PANEL_TYPE,
        STAGE_PANEL_TYPE,
        ADVANCE_PANEL_TYPE,
        EQUIP_INFO_TYPE,
        EQUIP_LIST_TYPE,
        EQUIP_INLAY_TYPE
    }

    public enum SkillIndex
    {
        SKILL_PHY_INDEX,
        SKILL_MAGIC_INDEX,
        SKILL_DAZHAO_INDEX,
        SKILL_BUFF_INDEX,
        SKILL_PASSIVE_INDEX
    }

    public const string UIPetListAssetName = "UIPetList";
    public const string UIPetListElementAssetName = "UIPetListElement";
    public const string UIPetDetailAssetName = "UIPetDetail";
    public const string UIPetModelAssetName = "PetModel";
    public const string UIPetModelCameraAssetName = "UIPetModelCamera";
    public const string UIPetAdvanceAssetName = "UIPetAdvance";
    public const string UIPetAttrAssetName = "UIPetAttr";
    public const string UIPetAttrElementAssetName = "UIPetAttrElement";
    public const string UIPetEquipAssetName = "UIPetEquip";
    public const string UIPetSkillAssetName = "UIPetSkill";
    public const string UIPetSkillElementAssetName = "UIPetSkillElement";
    public const string UIPetStageAssetName = "UIPetStage";
    public const string UIPetStageMonsterElementAssetName = "UIPetStageMonsterElement";
    public const string UIPetStageMonsterSelectAssetName = "UIPetStageMonsterSelect";
    public const string UIPetTypeBtnAssetName = "UIPetTypeBtn";
    public const string UIPetEquipInfoAssetName = "UIPetEquipInfo";
    public const string UIPetEquipListAssetName = "UIPetEquipList";
    public const string UIPetEquipInlayAssetName = "UIPetEquipInlay";


    public static string ReloadPetListNotify = "reloadPetListNotify";
    public static string ReloadPetStageNotify = "reloadPetStageNotify";
    public static string ReloadPetEquipNotify = "reloadPetEquipNotify";

    public static string PetListFull = "pet_tip_full";
    public static string PetListProperty = "pet_list_proprety";
    public static string PetListType = "pet_list_type";
    public static string PetDetailLeftDetailAttr = "pet_detail_left_detail_attr";
    public static string PetDetailLeftSkill = "pet_detail_left_skill";
    public static string PetDetailLeftStage = "pet_detail_stage";
    public static string PetDetailLeftAdvance = "pet_detail_left_advance";

    public static string PetDetailSkillLevel = "pet_detail_skill_level";
    public static string PetDetailSkillCurrentLeve = "pet_detail_skill_current_level";
    public static string PetDetailSkillNextLeve = "pet_detail_skill_next_level";
    public static string PetDetailSkillMaxLeve = "pet_detail_skill_max_level";
    public static string PetDetailSkillCurrentPoint = "pet_detail_skill_current_point";
    public static string PetDetailSkillMaxPoint = "pet_detail_skill_max_point";
    public static string PetDetailSkillPointNotEnough = "pet_skill_point_not_enough";

    public static string PetDetailStageAttr = "pet_detail_stage_attr";
    public static string PetDetailStageDemandItem = "pet_detail_stage_demand_item";
    public static string PetDetailStageDemandLevel = "pet_detail_stage_demand_level";//需求等级
    public static string PetDetailStage = "pet_detail_stage";           //进阶
    public static string PetDetailStageFull = "pet_detail_stage_full";  //进阶完成
    public static string PetDetailStageMonster = "pet_detail_stage_monster";
    public static string PetDetailStageBattle = "equip_forge_zhanli";
    public static string PetDetailStageNoneMonster = "pet_detail_stage_none_monster";
    public static string PetDetailSkillNoUpgrade = "pet_detail_skill_no_upgrade";

    public static string PetListTitle = "pet_list_title";

}





