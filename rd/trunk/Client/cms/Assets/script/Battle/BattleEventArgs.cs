using System;
using System.Collections.Generic;

public class ShowSwitchPetUIArgs : EventArgs
{
    public int targetId;
    public List<GameUnit> idleUnits;
}