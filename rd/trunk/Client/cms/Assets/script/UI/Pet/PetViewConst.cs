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
        EQUIP_PANEL_TYPE,
        ADVANCE_PANEL_TYPE
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

    public static string UIPetListAssetName = "UIPetList";
    public static string UIPetListContainerAssetName = "UIPetListContainer";
    public static string UIPetListElementAssetName = "UIPetListElement";
    public static string UIPetDetailAssetName = "UIPetDetail";
    public static string UIPetModelAssetName = "PetModel";
    public static string UIPetModelCameraAssetName = "UIPetModelCamera";
    public static string UIPetAdvanceAssetName = "UIPetAdvance";
    public static string UIPetAttrAssetName = "UIPetAttr";
    public static string UIPetAttrElementAssetName = "UIPetAttrElement";
    public static string UIPetEquipAssetName = "UIPetEquip";
    public static string UIPetSkillAssetName = "UIPetSkill";
    public static string UIPetSkillElementAssetName = "UIPetSkillElement";
    public static string UIPetStageAssetName = "UIPetStage";
    public static string UIPetStageMonsterElementAssetName = "UIPetStageMonsterElement";
    public static string UIPetStageMonsterSelectAssetName = "UIPetStageMonsterSelect";
    public static string UIPetTypeBtnAssetName = "UIPetTypeBtn";

    public static string ReloadPetListNotify = "reloadPetListNotify";
    public static string ReloadPetStageNotify = "reloadPetStageNotify";

    public static string PetListProperty = "pet_list_proprety";
    public static string PetListType = "pet_list_type";
    public static string PetDetailLeftProprety = "pet_detail_left_property";
    public static string PetDetailLeftBattle = "pet_detail_left_battle";
    public static string PetDetailLeftexperience = "pet_detail_left_experience";
    public static string PetDetailLeftcharacter = "pet_detail_left_character";

    public static string PetDetailLeftPropretyLabel = "pet_detail_left_property_label";
    
}
