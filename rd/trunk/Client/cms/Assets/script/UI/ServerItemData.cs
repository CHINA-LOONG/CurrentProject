using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ServerItemData : MonoBehaviour
{
    public Text sServerName;
    public Text sServerType;
    public Text sPlayerName;
    public Text sPlayerLv;
    [HideInInspector]
    public string sHostIp;
    [HideInInspector]
    public int sPort;
    [HideInInspector]
    public int lv;
}
