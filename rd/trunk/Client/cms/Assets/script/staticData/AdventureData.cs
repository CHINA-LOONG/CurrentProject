
public enum AdventureType
{
    QIANGHUA = 1,
    JINJIE = 2,
    BOSS = 3
}
public class AdventureData
{
    public int id;
    public string comments;
    public string image;
    public int type;
    public int time;
    public string level;   //12,20
    public string basicReward;
    public string extraReward;

    public int minLevel;
    public int maxLevel;

    public AdventureType Type
    {
        get { return (AdventureType)type; }
    }

    public string CommentsText
    {
        get { return StaticDataMgr.Instance.GetTextByID(comments); }
    }
    public string TypeText
    {
        get
        {
            switch (type)
            {
                case 1:
                    return StaticDataMgr.Instance.GetTextByID("adventure_qianghuashi");
                case 2:
                    return StaticDataMgr.Instance.GetTextByID("adventure_jinjieshi");
                case 3:
                    return StaticDataMgr.Instance.GetTextByID("adventure_boss");
                default:
                    return "type error";
            }
        }
    }
    //在读取配置表单的时候调用，有些需要解析字符串获取数值
    public void InitData()
    {
        string[]level=this.level.Split(',');
        if (level.Length!=2)
        {
            Logger.LogError("大冒险等级限制配置错误");
        }
        minLevel = int.Parse(level[0]);
        maxLevel = int.Parse(level[1]);
    }
}

public class AdventureConditionNumData
{
    public int id;
    public string level;
    public string num;
    public int weight;
}

public class AdventureConditionTypeData
{
    public int id;
    public string comments;
    public string level;
    public int monsterType;
    public int monsterProperty;
    public int weight;
    public string desc;
}

public class AdventureTeamPriceData
{
    public int id;
    public int gold;
}

public class Sociatybase
{
    public int bpMax;
    public int coinDefend;
    public int coinHire;
    public int coinHireget;
}