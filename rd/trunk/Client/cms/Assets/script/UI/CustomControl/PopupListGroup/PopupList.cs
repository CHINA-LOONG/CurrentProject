using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public interface PopupListDelegate { }
public interface PopupListContentDelegate : PopupListDelegate
{
    void OnPopupListChanged(string selection);
}
public interface PopupListIndextDelegate : PopupListDelegate
{
    void OnPopupListChanged(int index);
}

public class PopupList : MonoBehaviour
{
    public static PopupList current;
    public PopupListContentDelegate popupContentDelegate = null;
    public PopupListIndextDelegate popupIndexDelegate = null;
    public delegate void optionHandler();

    public class PopupOption
    {
        public string content;
        public optionHandler optionEvent;
    }
    public enum Position
    {
        Above,
        Below
    }
    public Position position = Position.Above;

    //public Font textFont;
    public Color fontColor = new Color(0, 0, 0);
    public TextAnchor itemAnchor=TextAnchor.MiddleCenter;
    public FontStyle itemStyle=FontStyle.Normal;

    public bool setInEditor = false;
    public List<string> itemList = new List<string>();

    List<PopupOption> optionss = new List<PopupOption>();

    public Button m_cutSelect;
    public Text m_cutText;

    public Sprite spr_listBg;

    private Image m_listPanel;
    private GridLayoutGroup grid;
    private ContentSizeFitter adapt;

    private List<Button> options = new List<Button>();

    string mSelectedItem;
    int mSelectedIndex;
    public string value
    {
        get
        {
            return mSelectedItem;
        }
        set
        {
            mSelectedItem = value;
            m_cutText.text = mSelectedItem;
            //if (mSelectedItem == null) return;
            if (popupContentDelegate != null) popupContentDelegate.OnPopupListChanged(mSelectedItem);
            for (int i = 0; i < optionss.Count; i++)
			{
                if (optionss[i].content==mSelectedItem)
                {
                    mSelectedIndex = i;
                    if (optionss[i].optionEvent != null) optionss[i].optionEvent();
                    if (popupIndexDelegate != null) popupIndexDelegate.OnPopupListChanged(i);
                }
            }
        }
    }


    void Start()
    {
        EventTriggerListener.Get(m_cutSelect.gameObject).onClick = OnClickBtn;
        (transform as RectTransform).sizeDelta = (m_cutSelect.transform as RectTransform).rect.size;
        if (setInEditor)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                AddItem(i, itemList[i]);
            }
        }
    }

    private int index;

    public void Initialize<T>(T pDelegate)
    {
        Logger.Log(typeof(T));
        popupContentDelegate = null;
        popupIndexDelegate = null;
        if (typeof(T)==typeof(PopupListContentDelegate))
            popupContentDelegate = pDelegate as PopupListContentDelegate;
        else if (typeof(T) == typeof(PopupListIndextDelegate))
            popupIndexDelegate = pDelegate as PopupListIndextDelegate;
        setInEditor = false;
        index = 0;
        itemList.Clear();
        optionss.Clear();
    }
    public void AddItem(string item, optionHandler optionEvent = null)
    {
        AddItem(index, item, optionEvent);
        index++;
    }
    public void AddItem(int index,string item, optionHandler optionHandler = null)
    {
        optionss.Insert(index, new PopupOption() { content = item, optionEvent = optionHandler });
        this.index = index;
    }
    public void RefrshItem(int index, string item)
    {
        if (optionss[index]!=null)
        {
            optionss[index].content = item;
        }
        else
        {
            Logger.LogError("error: not found this option");
        }
        if (mSelectedIndex==index)
        {
            m_cutText.text = item;
        }
    }

    public void SetSelection(string selection)
    {
        value = selection;
    }
    public void SetSelection(int index)
    {
        value = optionss[index].content;
    }

    void OnClickBtn(GameObject go)
    {
        if (m_listPanel == null || !m_listPanel.gameObject.activeSelf)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        if (m_listPanel == null)
        {
            GameObject go = new GameObject("list Panel");
            go.layer = gameObject.layer;
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            m_listPanel = go.AddComponent<Image>();
            grid = m_listPanel.gameObject.AddComponent<GridLayoutGroup>();
            Vector2 vec2=(m_cutSelect.transform as RectTransform).rect.size;
            grid.cellSize = vec2;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 1;
            adapt = m_listPanel.gameObject.AddComponent<ContentSizeFitter>();
            adapt.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else if (!m_listPanel.gameObject.activeSelf) m_listPanel.gameObject.SetActive(true);
        m_listPanel.sprite = spr_listBg;
        m_listPanel.type = Image.Type.Sliced;
        if (position == Position.Above)
        {
            m_listPanel.rectTransform.pivot = new Vector2(0, 0);
            m_listPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            m_listPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        }
        else
        {
            m_listPanel.rectTransform.pivot = new Vector2(0, 1);
            m_listPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            m_listPanel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        }
        m_listPanel.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (transform as RectTransform).rect.size.x);

        for (int i = 0; i < options.Count; i++)
        {
            if (i >= optionss .Count) options[i].gameObject.SetActive(false);
            else options[i].gameObject.SetActive(true);
        }
        for (int i = options.Count; i < optionss.Count; i++)
        {
            Button item = GameObject.Instantiate<Button>(m_cutSelect)as Button;
            item.gameObject.layer = gameObject.layer;
            item.transform.parent = m_listPanel.transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            Text text = item.GetComponentInChildren<Text>();
			text.font = m_cutText.font;
            text.color = fontColor;
            text.fontStyle = itemStyle;
            text.alignment = itemAnchor;
            options.Add(item);

            EventTriggerListener.Get(item.gameObject).onDown = OnSelect;
        }
        for (int i = 0; i < optionss.Count; i++)
        {
            if (options[i] != null)
            {
                options[i].GetComponentInChildren<Text>().text = optionss[i].content;
            }
            else
            {
                options[i].gameObject.SetActive(false);
            }
        }
    }

    private void Hide()
    {
        if (m_listPanel!=null&&m_listPanel.gameObject.activeSelf)
        {
            m_listPanel.gameObject.SetActive(false);
        }
    }

    void OnSelect(GameObject go)
    {
        value = go.GetComponentInChildren<Text>().text;
        Hide();
    }

    void OnDisable() { Hide(); }
}