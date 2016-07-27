using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGM : UIBase {
    
    public static string ViewName = "UIGm";
    public Button okButton;
    public InputField gmCommand;
    public Button backButton;
    public Text text;
    PB.GMOperation gmOperation;

	// Use this for initialization
	void Start () {
        EventTriggerListener.Get(backButton.gameObject).onClick = BackButtonClick;
        EventTriggerListener.Get(okButton.gameObject).onClick = OKButtonClick;
        BindListener();
	}
	
	// Update is called once per frame
	void Update () {
        
	} 

    void OnDestroy()
    {
        UnBindListener();
    }

    void BindListener()
    {
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.gm.GMOPERATION_C.GetHashCode().ToString(), OnGMOperationFinished);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.gm.GMOPERATION_S.GetHashCode().ToString(), OnGMOperationFinished);
    }

    void UnBindListener()
    {
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.gm.GMOPERATION_C.GetHashCode().ToString(), OnGMOperationFinished);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.gm.GMOPERATION_S.GetHashCode().ToString(), OnGMOperationFinished);
    }


    void OKButtonClick(GameObject go)
    {
        if (string.IsNullOrEmpty(gmCommand.text) == true)
        {
            text.text = "输入命令";
            return;
        }

        // 分解命令
        string commandStr = gmCommand.text;
        commandStr = commandStr.Trim();
        string[] data = commandStr.Split(' ');
        ArrayList command = new ArrayList();
        for (int i = 0; i < data.Length; i++)
        {
            if (!data[i].Equals("") && !data[i].Equals(" "))
            {
                command.Add(data[i]);
            }
        }

        gmOperation = new PB.GMOperation();
        gmOperation.action = command[0] as string;

        if (command[0].Equals("lv"))
        {
            if (command.Count != 3 || (command[1] as string).Equals("=") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
            
        }
        else if (command[0].Equals("petlv"))
        {
            if (command.Count != 4 || (command[1] as string).Equals("=") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
            gmOperation.targetId = int.Parse(command[3] as string);
        }
        else if (command[0].Equals("exp"))
        {
            if (command.Count != 3 || (command[1] as string).Equals("+") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
        }
        else if (command[0].Equals("petexp"))
        {
            if (command.Count != 4 || command[1].Equals("+") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
            gmOperation.targetId = int.Parse(command[3] as string);
        }
        else if (command[0].Equals("coin"))
        {
            if (command.Count != 3 || command[1].Equals("=") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
        }
        else if (command[0].Equals("gold"))
        {
            if (command.Count != 3 || command[1].Equals("=") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
        }
        else if (command[0].Equals("tili"))
        {
            if (command.Count != 3 || command[1].Equals("=") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
        }
        else if (command[0].Equals("grade"))
        {
            if (command.Count != 4 || command[1].Equals("=") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
            gmOperation.targetId = int.Parse(command[3] as string);
        }
        else if (command[0].Equals("resp"))
        {
            if (command.Count != 1 )
            {
                text.text = "格式错误";
                return;
            }
        }
        else if (command[0].Equals("pet"))
        {
            if (command.Count != 4 || command[1].Equals("+") == false)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
            gmOperation.itemId = command[3] as string;
        }
        else if (command[0].Equals("item"))
        {
            if ((command.Count != 4 && command.Count != 3) || (command[1].Equals("+") == false && command[1].Equals("-") == false))
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            if (command.Count == 3)
            {
                gmOperation.targetId = long.Parse(command[2] as string);
            }
            else
            {
                gmOperation.value = int.Parse(command[2] as string);
                gmOperation.itemId = command[3] as string;
            }
        }
        else if (command[0].Equals("clearbag"))
        {
            if (command.Count != 1 )
            {
                text.text = "格式错误";
                return;
            }
        }
        else if (command[0].Equals("reshop"))
        {
            if (command.Count != 2)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.value = int.Parse(command[1] as string);
        }
        else if (command[0].Equals("sys"))
        {
            if (command.Count != 3)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.operation = command[1] as string;
            gmOperation.value = int.Parse(command[2] as string);
        }
        else if (command[0].Equals("mail"))
        {
            if (command.Count != 4)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.targetId = int.Parse(command[1] as string);
            gmOperation.itemId = command[2] as string;
            gmOperation.value = int.Parse(command[3] as string);
        }
        else if (command[0].Equals("mailall"))
        {
            if (command.Count != 2)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.itemId = command[1] as string;
        }
        else if (command[0].Equals("questset"))
        {
            if (command.Count != 3)
            {
                text.text = "格式错误";
                return;
            }

            gmOperation.targetId = int.Parse(command[1] as string);
            gmOperation.value = int.Parse(command[2] as string);
        }
        else if (command[0].Equals("questclear"))
        {
            if (command.Count != 1)
            {
                text.text = "格式错误";
                return;
            }
        }
        else if (command[0].Equals("end"))
        {
            if (command.Count != 2)
            {
                text.text = "格式错误";
                return;
            }
            if (GameMain.Instance.IsCurModule<BattleModule>() == true)
            {
                BattleController.Instance.Process.forceResult = int.Parse(command[1] as string);
            }

            UIMgr.Instance.CloseUI_(this);
            return;
        }

        GameApp.Instance.netManager.SendMessage(PB.gm.GMOPERATION_C.GetHashCode(), gmOperation);
    }

    void BackButtonClick(GameObject go)
    {
        UIMgr.Instance.CloseUI_(this);
    }

    void OnGMOperationFinished(ProtocolMessage msg)
	{
		UINetRequest.Close ();
		if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {

            text.text = "GM命令执行失败";
            return;
        }

        text.text = "GM命令执行成功";
        if (gmOperation.action.Equals("grade"))
        {
            PbUnit unit = null;
            GameDataMgr.Instance.PlayerDataAttr.unitPbList.TryGetValue((int)gmOperation.targetId, out unit);
            if (unit != null)
            {
                unit.stage = (int)gmOperation.value;
            }
        }
        else if(gmOperation.action.Equals("resp"))
        {
            StatisticsDataMgr.Instance.SetSkillPoint(GameConfig.MaxSkillPoint);
        }
    }
}
