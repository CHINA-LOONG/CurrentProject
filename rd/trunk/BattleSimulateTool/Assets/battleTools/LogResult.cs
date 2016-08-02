using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
public class LogResult : MonoBehaviour {    
    static LogResult mInstance;
    public static LogResult Instance
    {
        get { return mInstance; }
    }
    public GameObject panel;
    public GameObject logPanel;
    public Text logContent;
    public int xhNumber = 0;
    public bool isHitSuccessP = false;
    public bool isCriticalP = false;
    public bool isHitSuccessE = false;
    public bool isCriticalE = false;         
    public LogData[] logData;//攻击方输出数据 yueguangsenlin11 xgLangren
    void Awake()
    {
        mInstance = this;
    }
	// Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(logPanel).onClick = logClick;
        panel.SetActive(false);
    }
    public void ShowLogPanel()
    {
        panel.SetActive(true);
        string savepath = @"Assets/StreamingAssets/staticData/FightResul.csv";
        FileStream fs = new FileStream(savepath, FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine("循环次数,对局数,回合数,战斗结果（1=胜利，0=失败）,怪物1剩余血量,怪物2剩余血量,怪物3剩余血量,怪物4剩余血量,怪物5剩余血量,剩余怪物数,总出手回合数,命中回合数,暴击回合数,大招使用次数,怪物1剩余血量, 怪物2剩余血量,怪物3剩余血量,怪物4剩余血量,怪物5剩余血量,剩余怪物数,总出手回合数,命中回合数,暴击回合数,大招使用次数");        
        for (int i = 0; i < logData.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                sw.WriteLine((logData[i].logXhNumber + 1) + "," + (j + 1) + "," +
                    logData[i].logRoundNumber[j] + "," + logData[i].logIsWin[i] + "," +
                    logData[i].playerData[j].attBloodNumber[0] + "%," +
                    logData[i].playerData[j].attBloodNumber[1] + "%," +
                    logData[i].playerData[j].attBloodNumber[2] + "%," +
                    logData[i].playerData[j].attBloodNumber[3] + "%," +
                    logData[i].playerData[j].attBloodNumber[4] + "%," +
                    logData[i].playerData[j].monsterNumber + "," +
                    logData[i].playerData[j].monsterAttNumber + "," +
                    logData[i].playerData[j].monsterHitNumber + "," +
                    logData[i].playerData[j].monsterCritNumber + "," +
                    logData[i].playerData[j].monsterDazhaoNumber + "," +
                    logData[i].enemyData[j].attBloodNumber[0] + "%," +
                    logData[i].enemyData[j].attBloodNumber[1] + "%," +
                    logData[i].enemyData[j].attBloodNumber[2] + "%," +
                    logData[i].enemyData[j].attBloodNumber[3] + "%," +
                    logData[i].enemyData[j].attBloodNumber[4] + "%," +
                    logData[i].enemyData[j].monsterNumber + "," +
                    logData[i].enemyData[j].monsterAttNumber + "," +
                    logData[i].enemyData[j].monsterHitNumber + "," +
                    logData[i].enemyData[j].monsterCritNumber + "," +
                    logData[i].enemyData[j].monsterDazhaoNumber);
            }
        }
        sw.Close();
        fs.Close();
        logContent.text = "Assets/StreamingAssets/staticData/FightResul.csv";
    }
    public void logClick(GameObject obj)
    {
        panel.SetActive(false);
    }    
}
