using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyTaskItem : MonoBehaviour
{
    public Image iconImage;
    public Text nameText;
    public Text descText;
    public GameObject noSelObject;
    public GameObject selObject;
    public Text lockLevelText;
    public Button itemButton;

    public SociatyTask sociatyTaskData = null;
    public static SociatyTaskItem CreateWith(SociatyTask taskData)
    {
        GameObject go = ResourceMgr.Instance.LoadAsset("SociatyTaskItem");
        SociatyTaskItem item = go.GetComponent<SociatyTaskItem>();
        item.InitWith(taskData);
        item.SetSelected(false);
        return item;
    }
	// Use this for initialization
	void Start ()
    {
        itemButton.onClick.AddListener(OnItemButtonClick);
	}
	
    public void InitWith(SociatyTask taskData)
    {
        sociatyTaskData = taskData;

        nameText.text = StaticDataMgr.Instance.GetTextByID(taskData.taskName);
        descText.text = StaticDataMgr.Instance.GetTextByID(taskData.taskDesc);
        lockLevelText.text = string.Format(StaticDataMgr.Instance.GetTextByID("towerBoss_instance_level"), taskData.minLevel);
        if(taskData.minLevel > GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            lockLevelText.color = new Color(1, 0, 0);
        } 
        else
        {
            lockLevelText.color = new Color(251.0f/255.0f, 241.0f/255.0f, 216.0f/255.0f);
        }
    }

	public void SetSelected(bool isSel)
    {
        noSelObject.SetActive(!isSel);
        selObject.SetActive(isSel);
    }

    void OnItemButtonClick()
    {
        if(sociatyTaskData.minLevel > GameDataMgr.Instance.PlayerDataAttr.LevelAttr)
        {
            UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("sociaty_record_031"), (int)PB.ImType.PROMPT);
            return;
        }
        SociatyTaskList.Instance.OnItemSelected(this);
    }
}
