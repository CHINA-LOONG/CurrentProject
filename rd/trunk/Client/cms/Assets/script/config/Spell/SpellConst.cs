public struct SpellConst
{
    public const int spellCastOK = 0;
    public const int spellCastImmune = 1;

    public const int targetTypeTarget = 0;
    public const int targetTypeCaster = 1;

    public const int hitSuccess = 0;
    public const int hitMiss = 1;
    public const int hitImmune = 2;

    public const float criticalDamgeRatio = 1.5f;
    public const float criticalRatio = 0.1f;
    public const float hitRatio = 0.80f;
    public const float minHitRatio = 0.6f;

    public const int damagePhy = 0;
    public const int damageMagic = 1;

    public const float strengthToAttack = 1.0f;
    public const float intelligenceToAttack = 1.0f;
    public const float healthToLife = 1.0f;

    public const int propertyGold = 1;
    public const int propertyWood = 2;
    public const int propertyWater = 3;
    public const int propertyFire = 4;
    public const int propertyEarth = 5;
    public const float propertyEnhance = 1.25f;
    public const float propertyWeaken = 0.75f;

    //public const int maxEnergy = 100;//重复定义，在BattleConst中已存在

    public const float aniDelayTime = 1.0f;
    public const float buffShowInterval = 2.0f;
}

public enum EffectType
{
    Effect_Type_Set,
    Effect_Type_Search,
    Effect_Type_Persistent,
    Effect_Type_Damage,
    Effect_Type_Buff,
    Effect_Type_Switch,

    Num_Effect_Type
}

public enum BuffType
{
    Buff_Type_Normal,
    Buff_Type_Dot,
    Buff_Type_Hot,
    Buff_Type_Defend,
    Buff_Type_Debuff,
    Buff_Type_Benefit,
    Buff_Type_Dazhao,

    Num_Buff_Type
}

public enum SpellType
{
    Spell_Type_PhyAttack,
    Spell_Type_MgicAttack,
    Spell_Type_Cure,
    Spell_Type_Defense,
    Spell_Type_Passive,
    Spell_Type_Beneficial,
    Spell_Type_Negative,
    Spell_Type_Lazy,
    Spell_Type_PhyDaZhao,
	Spell_Type_MagicDazhao,
    Spell_Type_PrepareDazhao,
	Spell_Type_Dot,

    Num_Spell_Type
}

public enum VitalType
{
    Vital_Type_Default,
    Vital_Type_Miss,
    Vital_Type_Interrupt,

    Num_Vital_Type
}