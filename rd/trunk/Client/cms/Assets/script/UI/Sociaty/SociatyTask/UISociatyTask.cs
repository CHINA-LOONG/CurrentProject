using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum SociatyTaskContenType : int
{
    MyTeam = 0,//我的队伍
    OtherTeam,
    Count
}
public class UISociatyTask : UIBase, TabButtonDelegate
{
    public static string ViewName = "UISociatyTask";
    public Text title;
    public Button closeButton;
    public TabButtonGroup tabBtnGroup;
    public RectTransform contentParent;

    private SociatyTaskList taskList;
    private SociatyTaskRunning taskRunning;
    private SociatyTaskOther taskOther;

    private SociatyTaskContenType taskType = SociatyTaskContenType.Count;

    public static UISociatyTask Instance = null;
    private string topOtherTeamId = null;

    public static   void Open(SociatyTaskContenType taskType = SociatyTaskContenType.MyTeam,string otherTeamId = null)
    {
        UISociatyTask taskUi = (UISociatyTask)UIMgr.Instance.OpenUI_(ViewName);
        taskUi.SetOtherTeamId(otherTeamId);
        taskUi.SetTaskType(taskType);
    }

    public override void Init()
    {
        
    }

    public override void Clean()
    {
        if(null != taskList)
        {
            ResourceMgr.Instance.DestroyAsset(taskList.gameObject);
            taskList = null;
        }
        if(null != taskRunning)
        {
            ResourceMgr.Instance.DestroyAsset(taskRunning.gameObject);
            taskRunning = null;
        }
        if(null != taskOther)
        {
            ResourceMgr.Instance.DestroyAsset(taskOther.gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
        Instance = this;
        title.text = StaticDataMgr.Instance.GetTextByID("sociaty_task");
        tabBtnGroup.InitWithDelegate(this);
        tabBtnGroup.tabButtonList[0].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_myteam"));
        tabBtnGroup.tabButtonList[1].SetButtonTitleName(StaticDataMgr.Instance.GetTextByID("sociaty_otherteam"));
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    public void OnTabButtonChanged(int index)
    {
        if ((int)taskType != index)
        {
            SetTaskType((SociatyTaskContenType)index);
        }
    }
    /// <summary>
    /// 设置 定位到其它公会的id
    /// </summary>
    /// <param name="otherTeamId"></param>
    void SetOtherTeamId(string otherTeamId)
    {
        topOtherTeamId = otherTeamId;
    }

    public void SetTaskType(SociatyTaskContenType type)
    {
        taskType = type;
        if(taskType == SociatyTaskContenType.MyTeam)
        {
            if(GameDataMgr.Instance.SociatyDataMgrAttr.taskTeamId >0)
            {
                ShowTaskList(false);
                ShowTaskOther(false);
                ShowTaskRunning(true);
            }
            else
            {
                ShowTaskList(true);
                ShowTaskOther(false);
                ShowTaskRunning(false);
            }
        }
        else if (taskType == SociatyTaskContenType.OtherTeam)
        {
            ShowTaskList(false);
            ShowTaskOther(true);
            ShowTaskRunning(false);
        }
    }

    void ShowTaskList(bool bshow)
    {
        if(!bshow)
        {
            if(taskList != null)
            {
                taskList.gameObject.SetActive(false);
            }
            return;
        }
        if (taskList == null)
        {
            taskList = SociatyTaskList.Instance;
            taskList.transform.SetParent(contentParent);
            RectTransform rt = taskList.transform as RectTransform;
            rt.anchoredPosition = new Vector2(0, 0);
            rt.localScale = new Vector3(1, 1, 1);
        }
        taskList.gameObject.SetActive(true);
    }
    void ShowTaskRunning(bool bshow)
    {
        if (!bshow)
        {
            if (taskRunning != null)
            {
                taskRunning.gameObject.SetActive(false);
            }
            return;
        }
        if (null == taskRunning)
        {
            taskRunning = SociatyTaskRunning.Instance;
            taskRunning.transform.SetParent(contentParent);
            RectTransform rt = taskRunning.transform as RectTransform;
            rt.anchoredPosition = new Vector2(0, 0);
            rt.localScale = new Vector3(1, 1, 1);
        }
        taskRunning.gameObject.SetActive(true);
        taskRunning.RequestMyTeamInfo();
    }
    void ShowTaskOther(bool bshow)
    {
        if (!bshow)
        {
            if (taskOther != null)
            {
                taskOther.gameObject.SetActive(false);
            }
            return;
        }
        if(null == taskOther)
        {
            taskOther = SociatyTaskOther.Instance;
            taskOther.transform.SetParent(contentParent);
            RectTransform rt = taskOther.transform as RectTransform;
            rt.anchoredPosition = new Vector2(0, 0);
            rt.localScale = new Vector3(1, 1, 1);
        }
        taskOther.gameObject.SetActive(true);
        int otherTeamid = -1;
        if (!string.IsNullOrEmpty(topOtherTeamId))
        {
            otherTeamid = int.Parse(topOtherTeamId);
            topOtherTeamId = null;
        }

        taskOther.SetTopTeamId(otherTeamid);
        topOtherTeamId = null;
        taskOther.RequestTeamList();
    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }

   
}
