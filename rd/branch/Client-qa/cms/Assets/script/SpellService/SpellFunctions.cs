using UnityEngine;
using System.Collections;

public class SpellFunctions  
{
    public static float GetInjuryAdjustNum(int casterLvl, int targetLvl)
    {
        return 0.1f;
    }
    
    public static float GetPropertyInfluenceRatio(int prop1, int prop2)
    {
        return 2.5f;
    }

    public static float GetHitRatio(int casterLvl, int targetLvl)
    {
        return 0.0f;
    }

    public static float GetPropertyDamageRatio(int casterProp, int targetProp)
    {
        float damageRatio = 1.0f;
        if (casterProp == SpellConst.propertyGold)
        {
            if (targetProp == SpellConst.propertyWood)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyFire)
            {
                return SpellConst.propertyWeaken;
            }
        }
        else if (casterProp == SpellConst.propertyWood)
        {
            if (targetProp == SpellConst.propertyEarth)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyGold)
            {
                return SpellConst.propertyWeaken;
            }
        }
        else if (casterProp == SpellConst.propertyWater)
        {
            if (targetProp == SpellConst.propertyFire)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyEarth)
            {
                return SpellConst.propertyWeaken;
            }
        }
        else if (casterProp == SpellConst.propertyFire)
        {
            if (targetProp == SpellConst.propertyGold)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyWater)
            {
                return SpellConst.propertyWeaken;
            }
        }
        else if (casterProp == SpellConst.propertyEarth)
        {
            if (targetProp == SpellConst.propertyWater)
            {
                return SpellConst.propertyEnhance;
            }
            else if (targetProp == SpellConst.propertyWood)
            {
                return SpellConst.propertyWeaken;
            }
        }
        return damageRatio;
    }
}
