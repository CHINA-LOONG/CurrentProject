using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//---------------------------------------------------------------------------------------------
public class SpellFireArgs : EventArgs
{
    public int triggerTime;
    public string spellID;
    public int casterID;
    public int targetID;
    //public int castResult;
}
//---------------------------------------------------------------------------------------------
//both life change event and energy change event use this as event args
public class SpellVitalChangeArgs : EventArgs
{
    //NOTE: if server notify client, make targetID to list
    public int triggerTime;
    public int casterID;
    public int targetID;
    public bool isCritical;
    public int vitalChange;
    public int vitalCurrent;
    //public int vitalMax; //no maxlife change in design
}
//---------------------------------------------------------------------------------------------
public class SpellUnitDeadArgs : EventArgs
{
    public int triggerTime;
    public int deathID;
    public int casterID;//who makes deathID dead
}
//---------------------------------------------------------------------------------------------
public class SpellBuffArgs : EventArgs
{
    public int triggerTime;
    public int casterID;
    public int targetID;
    public string buffID;
    public bool isAdd;
}
//---------------------------------------------------------------------------------------------
public class SpellEffectArgs : EventArgs
{
    public int triggerTime;
    public int casterID;
    public int targetID;
    public string effectID;
}
//---------------------------------------------------------------------------------------------
//miss,immune,stun
public class SpellStateArgs : EventArgs
{
    public int triggerTime;
    public int targetID;
    public int casterID;
    public string effectID;
}
//---------------------------------------------------------------------------------------------
