using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SellConformDlg : UIBase
{
	public static string ViewName = "SellConformDlg";

	public static void OpenWith(int totalJinbi,MsgBox.PromptMsg.PrompDelegate callback)
	{
		SellConformDlg dlg = (SellConformDlg)UIMgr.Instance.OpenUI_ (ViewName);
		dlg.RefreshWith (totalJinbi, callback);
	}

	public	Text	titleText;
	public	Text	careDescText;
	public	Text	totalDescText;
	public	Text	sellTotalText;

	public	Button	okButton;
	public	Button	cancelButton;

	MsgBox.PromptMsg.PrompDelegate callback;

	bool isFirst = true;
	public override void Init()
	{
		if (isFirst)
		{
			isFirst = false;
			FirsInit();
		}

	}
	
	void FirsInit()
	{
		EventTriggerListener.Get (okButton.gameObject).onClick = OnOKButtonClicked;
		EventTriggerListener.Get (cancelButton.gameObject).onClick = OnCancelButtonClicked;
	}

	public override void Clean()
	{
	}

	public	void	RefreshWith(int totalJinbi,MsgBox.PromptMsg.PrompDelegate callback)
	{
		this.callback = callback;
		sellTotalText.text = totalJinbi.ToString ();
	}

	void	OnOKButtonClicked(GameObject go)
	{
		if (null != callback)
		{
			callback(MsgBox.PrompButtonClick.OK);
		}
		UIMgr.Instance.CloseUI_ (this);
	}

	void	OnCancelButtonClicked(GameObject go)
	{
		UIMgr.Instance.CloseUI_ (this);
	}
}
