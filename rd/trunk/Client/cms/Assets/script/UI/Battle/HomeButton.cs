using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class HomeButton : MonoBehaviour 
{
	public delegate void HomeButtonDelegate (GameObject go);
	public	Image normalImage;
	public	Image checkedImage;
	public	Button button;

	public RectTransform  normalEffectPanel;
	public	RectTransform checkedEffectPanel;

	public HomeButtonDelegate onClick;

    public bool useDefault;
    public Color textNormalColor= ColorConst.text_tabColor_normal;
    public Color textSelectColor= ColorConst.text_tabColor_select;
    public Color outlineNormalColor=ColorConst.outline_tabColor_normal;
    public Color outlineSelectColor=ColorConst.outline_tabColor_select;
    private Text text;
    
    public Text Text
    {
        get 
        {
            if (text==null)
            {
                text = GetComponentInChildren<Text>();
            }
            return text; 
        }
    }
    private Outline outline;
    public Outline Outline
    {
        get 
        {
            if (outline==null&&Text!=null)
            {
                outline = Text.GetComponent<Outline>();
            }
            return outline; 
        }
    }

	[SerializeField]
	bool	isOn = false;
	public bool IsOn
	{
		set
		{
			isOn = value;
			UpdateButtonGraphic ();
			UpdateEffect ();

		}
		get
		{
			return isOn;
		}
	}

    public Image tipImage;
    public bool ShowTip
    {
        set
        {
            if (tipImage!=null)
            {
                tipImage.gameObject.SetActive(value);
            }
        }
    }

    public void SetButtonText(string text)
    {
        if (Text != null)
        {
            Text.text = text;
        }
    }
    // Use this for initialization
    void Start ()
	{
	
		EventTriggerListener.Get (button.gameObject).onClick = OnButtonClicked;
		UpdateButtonGraphic ();
		UpdateEffect ();
	}

	void OnButtonClicked(GameObject go)
	{
		isOn = !isOn;
		UpdateButtonGraphic ();
		UpdateEffect ();

		if (null != onClick) 
		{
			onClick(go);
		}
	}

	void UpdateButtonGraphic()
	{
		normalImage.gameObject.SetActive (!isOn);
		checkedImage.gameObject.SetActive (isOn);
		Image selImage = null;
		if (isOn)
		{
			selImage = checkedImage;
		} 
		else
		{
			selImage = normalImage;
		}
		button.targetGraphic = selImage;
	}

	void UpdateEffect()
	{
		if (null != normalEffectPanel) 
		{
			normalEffectPanel.gameObject.SetActive( !isOn);
		}
		if (null != checkedEffectPanel) 
		{
			checkedEffectPanel.gameObject.SetActive(isOn);
		}
        if (Text!=null)
        {
            Text.color = (isOn ? textSelectColor : textNormalColor);
        }
        if (Outline!=null)
        {
            Outline.effectColor = (isOn ? outlineSelectColor : outlineNormalColor);
        }
	}

}
