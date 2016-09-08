using UnityEngine;
using System.Collections.Generic;

public class TowerStaticData 
{
    public int id;
    public string textId;
    public string time;
    public int level;
    public string floor;
}

public class TowerData
{
    public int id;
    public string time;
    public int level;
    public List<string> floorList = new List<string>();
}
