using UnityEngine;
using System.Collections.Generic;

public class TowerStaticData 
{
    public string id;
    public string time;
    public int level;
    public string floor;
}

public class TowerData
{
    public string id;
    public string time;
    public int level;
    public List<string> floorList = new List<string>();
}
