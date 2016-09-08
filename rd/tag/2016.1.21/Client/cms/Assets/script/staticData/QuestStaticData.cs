using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestStaticData
{
    public int id;             //#任务Id	
    public int group;           //任务组	
    public int type;            //类别	
    public string comments;     //注释
    public string name;         //名称	
    public string desc;         //任务描述
    public string icon;         //图标
    public int level;           //接取等级	
    public int cycle;           //循环性	

    public int timeBeginId;     //开放时间配置Id	
    public int timeEndId;       //关闭时间配置Id	

    public string goalType;     //目标类型
    public string goalParam;    //目标参数
    public int goalCount;       //目标值	

    public string rewardId;        //奖励
    public int expK;            //经验系数k	
    public int expB;            //经验系数b	

    public string speechId;        //后续对话Id
    public string path;           // 跳转途径
    private List<string> pathList;
    public List<string> PathList
    {
        get
        {
            if (!string.IsNullOrEmpty(path) && pathList == null)
            {
                pathList=new List<string>(path.Split('_'));
            }
            return pathList;
        }
    }

}
