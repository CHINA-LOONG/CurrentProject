using UnityEngine;
using System.Collections;

public class UISelectServer : UIBase {

    public static string ViewName = "UISelectServer";
    public ScrollView container;


    public void ResetServerList(ArrayList serverLists)
    {
        foreach (Hashtable element in serverLists)
        {
            GameObject go = ResourceMgr.Instance.LoadAsset("serverElement");
            container.AddElement(go);
            ScrollViewEventListener.Get(go).onClick = ServerSelect;
            UIServerElement serverElement = go.GetComponent<UIServerElement>();
            serverElement.serverInfo = element;
            serverElement.serverName.text = element["server"].ToString();
            serverElement.hostName.text = element["hostIp"].ToString() + " : " + element["port"].ToString();
            if (element["role"] != null)
            {
                Hashtable role = element["role"] as Hashtable;
                serverElement.nickName.text = role["nickname"].ToString();
            }
            else
            {
                serverElement.nickName.text = "该服务器上无角色";
            }
        }        
    }
    
    void ServerSelect(GameObject go)
    {
        GameEventMgr.Instance.FireEvent<Hashtable>(GameEventList.ServerClick, go.GetComponent<UIServerElement>().serverInfo);
    }

}
