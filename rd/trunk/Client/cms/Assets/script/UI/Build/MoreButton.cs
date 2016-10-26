using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class MoreButton : MonoBehaviour
{
    enum MoreState
    {
        Open = 0,
        Close
    }
    public Transform listButtonParent;
    public Image moreImage;
    public Image moreButton;

    public RectTransform moveEndGameObject;
    public float animationTime = 0.4f;
    public Ease OpenEaseType = Ease.InBack;
    public Ease CloseEaseType = Ease.InBack;

    private List<BuildButton> listButton = new List<BuildButton>();
    private List<Vector2> listOldPosition = new List<Vector2>();

    private MoreState curMoreState = MoreState.Close;
    private bool isCanChangeState = true;

    private Vector2 minPosition = new Vector2(0, 0);

    public bool IsMoreButtonExpand
    {
        get
        {
            return curMoreState == MoreState.Open;
        }
    }
    public static MoreButton Instance = null;
	// Use this for initialization
	void Start ()
    {
        Instance = this;
        InitListButtons();
        EventTriggerListener.Get(moreButton.gameObject).onClick = OnMoreButtonClicked;
        OnMoreButtonClicked(null);
    }

    void    InitListButtons()
    {
        var szButtons = listButtonParent.GetComponentsInChildren<BuildButton>();
        minPosition = moveEndGameObject.anchoredPosition;

        for (int i =0;i<szButtons.Length;++i)
        {
            var subBtn = szButtons[i];
            listButton.Add(subBtn);
            RectTransform subRt = subBtn.transform as RectTransform;
            listOldPosition.Add(subRt.anchoredPosition);

            subRt.anchoredPosition = minPosition;
        }
    }

    public  void OnMoreButtonClicked(GameObject go)
    {
        if (isCanChangeState)
        {
            isCanChangeState = false;
            ChangeButtonState();
        }
    }

    void ChangeButtonState()
    {
        if (curMoreState == MoreState.Open)
        {
            curMoreState = MoreState.Close;
            StartCoroutine("Open2Close");
        }
        else
        {
            curMoreState = MoreState.Open;
            StartCoroutine("Close2Open");
        }
    }

    IEnumerator Close2Open()
    {
        for(int i =0; i< listButton.Count;++i)
        {
            var subButton = listButton[i].transform as RectTransform;
            subButton.anchoredPosition = minPosition;

            var oldPos = listOldPosition[i];
            subButton.DOAnchorPos(oldPos, animationTime).SetEase(OpenEaseType);
        }
        moreImage.transform.DORotate(new Vector3(0, 0, 90), animationTime).SetEase(OpenEaseType);
        yield return new WaitForSeconds(animationTime);
        OnAniFinished();
    }

    IEnumerator Open2Close()
    {
        for (int i = 0; i < listButton.Count; ++i)
        {
            var subButton = listButton[i].transform as RectTransform;
          //  subButton.anchoredPosition = minPosition;
            subButton.DOAnchorPos(minPosition, animationTime).SetEase(CloseEaseType);
        }
        moreImage.transform.DORotate(new Vector3(0, 0, 0), animationTime).SetEase(CloseEaseType);
        yield return new WaitForSeconds(animationTime);
        OnAniFinished();
    }

    void OnAniFinished()
    {
        isCanChangeState = true;
    }
}
