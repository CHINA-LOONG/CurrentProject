using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ServerStageItemData : MonoBehaviour
{
    public Text stageName;
    public GameObject selectBox;
    [HideInInspector]
    public int serverStageType = 0;//0正常服务器/1推荐服务器
    [HideInInspector]
    public string interval;	
}
