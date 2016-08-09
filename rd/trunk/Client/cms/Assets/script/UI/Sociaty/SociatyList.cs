using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SociatyList : UIBase
{
    public static string ViewName = "SociatyList";

    public Button closeButton;
    public Button createButton;
    public Button searchButton;
	// Use this for initialization

     public  static  void    OpenWith(string search)
    {
        SociatyList thislist = (SociatyList)UIMgr.Instance.OpenUI_(ViewName);
        thislist.InitWithSearch(search);
    }

	void Start ()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        createButton.onClick.AddListener(OnCreateButtonClick);
        searchButton.onClick.AddListener(OnSearchButtonClick);
	}

    public  void    InitWithSearch(string search)
    {

    }

    void OnCloseButtonClick()
    {
        UIMgr.Instance.CloseUI_(this);
    }
    
    void    OnCreateButtonClick()
    {
        CreateSociaty.Open();
    }

    void OnSearchButtonClick()
    {

    }

	
}
