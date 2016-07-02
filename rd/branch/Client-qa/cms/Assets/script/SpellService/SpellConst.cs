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
    public const float hitRatio = 0.8f;
    public const float minHitRatio = 0.6f;

    public const int damagePhy = 0;
    public const int damageMagic = 1;

    public const float strengthToAttack = 1.0f;
    public const float intelligenceToAttack = 1.0f;
    public const float healthToLife = 4.0f;

    public const int propertyGold = 1;
    public const int propertyWood = 2;
    public const int propertyWater = 3;
    public const int propertyFire = 4;
    public const int propertyEarth = 5;
}

public enum EffectType
{
    Effect_Type_Set,
    Effect_Type_Search,
    Effect_Type_Persistent,
    Effect_Type_Damage,
    Effect_Type_Buff,

    NUM_Effect_Type
}