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
        EQUIP_LIST_TYPE      
    }

    public enum SkillIndex
    {
        SKILL_PHY_INDEX,
        SKILL_MAGIC_INDEX,
        SKILL_DAZHAO_INDEX,
        SKILL_BUFF_INDEX,
        SKILL_PASSIVE_INDEX
    }


    // asset & buddle name
    //public static string UIPetListBuddleName = "ui/petlist";
    //public static string UIPetDetailBuddleName = "ui/petdetail";
    //public static string UIPetAttrBuddleName = "ui/petrightattr";
    //public static string UIPetSKillBuddleName = "ui/petrightskill";
    //public static string UIPetStageBuddleName = "ui/petrightstage";
    //public static string UIPetEquipBuddleName = "ui/petrightequip";
    //public static string UIPetAdvanceBuddleName = "ui/petrightadvance";

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


    public static string ReloadPetListNotify = "reloadPetListNotify";
    public static string ReloadPetStageNotify = "reloadPetStageNotify";

    public static string PetListFull = "pet_tip_full";
    public static string PetListProperty = "pet_list_proprety";
    public static string PetListType = "pet_list_type";
    public static string PetDetailLeftProprety = "pet_detail_left_property";
    public static string PetDetailLeftBattle = "pet_detail_left_battle";
    public static string PetDetailLeftexperience = "pet_detail_left_experience";
    public static string PetDetailLeftcharacter = "pet_detail_left_character";
    public static string PetDetailLeftAttr = "pet_detail_left_attr";
  
    public static string PetDetailLeftAttrHealth = "pet_detail_left_attr_health";
    public static string PetDetailLeftAttrDefence = "pet_detail_left_attr_defence";
    public static string PetDetailLeftAttrSpeed = "pet_detail_left_attr_speed";
    public static string PetDetailLeftAttrStrenth = "pet_detail_left_attr_strenth";
    public static string PetDetailLeftAttrIntelligence = "pet_detail_left_attr_intelligence";

    public static string PetDetailLeftChangeCharacter = "pet_detail_left_change_character";
    public static string PetDetailLeftDetailAttr = "pet_detail_left_detail_attr";
    public static string PetDetailLeftSkill = "pet_detail_left_skill";
    public static string PetDetailLeftStage = "pet_detail_left_stage";
    public static string PetDetailLeftAdvance = "pet_detail_left_advance";
    public static string PetDetailLeftMainPet = "pet_detail_left_main_pet";

    public static string PetDetailSkillLevel = "pet_detail_skill_level";
    public static string PetDetailSkillCurrentLeve = "pet_detail_skill_current_level";
    public static string PetDetailSkillNextLeve = "pet_detail_skill_next_level";
    public static string PetDetailSkillMaxLeve = "pet_detail_skill_max_level";
    public static string PetDetailSkillCurrentPoint = "pet_detail_skill_current_point";
    public static string PetDetailSkillMaxPoint = "pet_detail_skill_max_point";
    public static string PetDetailSkillPointNotEnough = "pet_skill_point_not_enough";

    public static string PetDetailStageAttr = "pet_detail_stage_attr";
    public static string PetDetailStageDemandItem = "pet_detail_stage_demand_item";
    public static string PetDetailStageDemandLevel = "pet_detail_stage_demand_level";
    public static string PetDetailStage = "pet_detail_stage";
    public static string PetDetailStageFull = "pet_detail_stage_full";
    public static string PetDetailStageMonster = "pet_detail_stage_monster";
    public static string PetDetailStageBattle = "pet_detail_stage_battle";
    public static string PetDetailStageNoneMonster = "pet_detail_stage_none_monster";
    public static string PetDetailSkillNoUpgrade = "pet_detail_skill_no_upgrade";

    public static string PetListTitle = "pet_list_title";
    public static string PetProprety1 = "monster_property1";
    public static string PetProprety2 = "monster_property2";
    public static string PetProprety3 = "monster_property3";
    public static string PetProprety4 = "monster_property4";
    public static string PetProprety5 = "monster_property5";

}





