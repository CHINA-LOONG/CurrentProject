using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//---------------------------------------------------------------------------------------------
public class SpellFireArgs : EventArgs
{
    public float triggerTime;
    public string spellID;
    public int casterID;
    public int targetID;
    public float aniTime;
    public string firstSpell;
    public int category;
    //public int castResult;
}
//---------------------------------------------------------------------------------------------
//both life change event and energy change event use this as event args
public class SpellVitalChangeArgs : EventArgs
{
    //NOTE: if server notify client, make targetID to list
    public int vitalType;
    public float triggerTime;
    public int casterID;
    public int targetID;
    public string wpID;//TODO: remove log only
    public string wpNode;
    public bool isCritical;
    public int vitalChange;
    public int vitalCurrent;
    public int vitalMax; 
}
//---------------------------------------------------------------------------------------------
public class SpellUnitDeadArgs : EventArgs
{
    public float triggerTime;
    public int deathID;
    public int casterID;//who makes deathID dead
}
//-------------------------------------------------------------------------------------------------
public class SpellUnitBornArgs : EventArgs
{
    public float triggerTime;
    public int bornID;
}
//---------------------------------------------------------------------------------------------
public class SpellBuffArgs : EventArgs
{
    public float triggerTime;
    public int casterID;
    public int targetID;
    public string buffID;
    public bool isAdd;
}
//---------------------------------------------------------------------------------------------
public class SpellEffectArgs : EventArgs
{
    public float triggerTime;
    public int casterID;
    public int targetID;
    public string effectID;
    public string wpNode;
}
//---------------------------------------------------------------------------------------------
//miss,immune,stun
public class SpellStateArgs : EventArgs
{
    public float triggerTime;
    public int targetID;
    public int casterID;
    public string effectID;
}
//---------------------------------------------------------------------------------------------
