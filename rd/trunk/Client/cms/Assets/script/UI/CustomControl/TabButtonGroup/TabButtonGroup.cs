using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface TabButtonDelegate
{
    void OnTabButtonChanged(int index, TabButtonGroup tab);
}

public class TabButtonGroup : MonoBehaviour {

	public List<TabButton> tabButtonList = new List<TabButton>();
	public	int	selectedIndex = 0;

	public TabButtonDelegate tabDelegate =null;

	// Use this for initialization

	public void InitWithDelegate( TabButtonDelegate pDelegate)
	{
		TabButton subButton = null;
		for (int i =0; i < tabButtonList.Count; ++i)
		{
			subButton = tabButtonList[i];
			subButton.index = i;
			subButton.group = this;
			subButton.SetIsOn(i == selectedIndex);
		}
		tabDelegate = pDelegate;
	}

	public void OnChangeItem(int itemIndex)
	{
        //if (itemIndex == selectedIndex)
        //    return;
		if (itemIndex < 0 || itemIndex > tabButtonList.Count - 1)
			return;
		tabButtonList [selectedIndex].SetIsOn (false);
		tabButtonList [itemIndex].SetIsOn (true);

		selectedIndex = itemIndex;
		if (tabDelegate != null)
		{
            tabDelegate.OnTabButtonChanged(itemIndex, this);
		}
	}

}
