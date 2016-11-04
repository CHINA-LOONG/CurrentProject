using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class GuideBattleStepData
{
    public int pauseBattle;        //pause the round or not
    public int pausePhyDazhao;     //pause physic dazhao or not
    public int showMirror;         //show mirror or not
    public int hintPos;            //where to pos the hint ui(3d)
    public int hintStyle;          //the hint ui size style
    public string speechID;        //whether has speech

    public string id;              //id
    public string tipsID;          //tips content id
    public string hintUIname;      //where to pos the hint ui(2d)
    public string endEvent;        //the event it causes this step end
}

public class GuideBattleStaticData
{
    public int id;                                          //id
    public int successStepIndex;                            //the step index after it,this guide means success
    public string condition;                                //trigger conditions
    public string battleStepIDList;                         //step list
}

//actual data used, converted form GuideBattleStaticData
public class GuideBattleData
{
    public int id;
    public int successStepIndex;
    public MethodInfo condition = null;
    public List<GuideBattleStepData> battleStepList;
}
