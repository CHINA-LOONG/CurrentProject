using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SaodangResult : UIBase
{

    public static string ViewName = "SaodangResult";
    public static void OpenWith(List<PB.HSRewardInfo> listReward)
    {
        SaodangResult saodangResult = (SaodangResult)UIMgr.Instance.OpenUI_(ViewName, false);
        saodangResult.InitWith(listReward);
    }

    public Image succImage;
    public ScrollView scrollView;
    public Transform mask;
    public Button OKButton;

    private List<PB.HSRewardInfo> listReward = null;
    private ScrollRect scrollRect;

    bool maskCanClick = false;
    private float scrollRectVertical = 1.2f;
    private float ScrlolRectVertical
    {
        get
        {
            return scrollRectVertical;
        }
        set
        {
            scrollRectVertical = value;
            scrollRect.verticalNormalizedPosition = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        EventTriggerListener.Get(OKButton.gameObject).onClick = OnClose;
        EventTriggerListener.Get(mask.gameObject).onClick = OnMaskClicked;
    }

    public void InitWith(List<PB.HSRewardInfo> listReward)
    {
        OKButton.GetComponentInChildren<Text>().text = StaticDataMgr.Instance.GetTextByID("ui_queding");
        scrollRect = scrollView.GetComponent<ScrollRect>();
        this.listReward = listReward;
        PB.HSRewardInfo subRewardInfo = null;
        for (int i = 0; i < listReward.Count; ++i)
        {
            subRewardInfo = listReward[i];
            string title = "";
            if (i == listReward.Count - 1)
            {
                title = StaticDataMgr.Instance.GetTextByID("arrayselect_saodang_reward");
            }
            else
            {
                title = string.Format(StaticDataMgr.Instance.GetTextByID("arrayselect_saodang_num"), i + 1);
            }
            var boxReward = SaodangReward.CreateWith(title, subRewardInfo);
            scrollView.AddElement(boxReward.gameObject);
        }
        scrollView.gameObject.SetActive(false);
        StartCoroutine("saodangResultAnimationCo");
    }

    IEnumerator saodangResultAnimationCo()
    {

        RectTransform rt = succImage.transform as RectTransform;

        Vector3 endPos = rt.anchoredPosition;
        rt.anchoredPosition = new Vector2(endPos.x, endPos.y + 300);

        rt.DOAnchorPos(endPos, 1.0f).SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(1.0f);

        int itemsCount = listReward.Count;
        int moveCount = 0;
        float moveStep = 0;
        if (itemsCount > 2)
        {
            moveStep = 1.0f / (float)(itemsCount - 2);
            moveCount = (itemsCount - 2);
        }

        scrollView.gameObject.SetActive(true);
        maskCanClick = true;

        SetRectVertical(2.5f);
        DOTween.To(GetRectVertical, SetRectVertical, 1.0f, 1.0f);
        if (moveCount > 0)
        {
            yield return new WaitForSeconds(1.5f);
        }

        for (int i = 0; i < moveCount; ++i)
        {
            DOTween.To(GetRectVertical, SetRectVertical, 1.0f - (i + 1.0f) * moveStep, 1.0f);
            yield return new WaitForSeconds(1.5f);
        }

        mask.gameObject.SetActive(false);
    }

    float GetRectVertical()
    {
        return ScrlolRectVertical;
    }

    void SetRectVertical(float value)
    {
        ScrlolRectVertical = value;
    }

    void OnScrllRectValueChanged(Vector2 value)
    {
        Logger.LogError("---- value = " + value.y);
    }

    public void OnClose(GameObject go)
    {
        listReward.Clear();
        UIMgr.Instance.CloseUI_(this);
    }

    void OnMaskClicked(GameObject go)
    {
        if (maskCanClick)
        {
            DOTween.Clear();
            StopAllCoroutines();
            DOTween.To(GetRectVertical, SetRectVertical, 0.0f, 0.5f);
            mask.gameObject.SetActive(false);
        }
    }
}
