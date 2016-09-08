using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public enum TowerItemType 
{ 
	Item_Type_Curr,
	Item_Type_not,
	Item_Type_ok,
	Item_Type_end,
	Num_Item_Type
}//当前状态
public class TowerItemData : MonoBehaviour
{
    public Text towerNum;
    public Image towerImage;
    public GameObject selectedImage;
    [HideInInspector]
    public string itemTowerID;
    [HideInInspector]
	public TowerItemType currType = TowerItemType.Num_Item_Type;
}
