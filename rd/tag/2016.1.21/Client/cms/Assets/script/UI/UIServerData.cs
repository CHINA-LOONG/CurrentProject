using UnityEngine;
using System.Collections;
public enum UIServerType
{
    New = 1,
    Full = 2,
    Hot = 3,
    Maintain = 4,
    SERVER_TYPE_UNKNOW = 5
}
public class UIServerData 
{
    public string serverName;
    public UIServerType serverType;
    public int serverIndex;
    public string hostIp;
    public int port;
    public string nickName = null;
    public int level = 0;
}
