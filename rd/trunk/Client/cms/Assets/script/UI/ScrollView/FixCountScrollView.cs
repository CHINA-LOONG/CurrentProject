using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 无限循环List
/// xiaolong 2015-11-3 16:40:41
/// 需要事先配置好ScrollRect 和GridContent
/// </summary>
public interface IScrollView
{
    //List<Transform> CreateItem(int count);
    void IScrollViewReloadItem(FixCountScrollView scrollView,Transform item, int index);
    Transform IScrollViewCreateItem(FixCountScrollView scrollView, Transform parent);
    void IScrollViewCleanItem(FixCountScrollView scrollView, List<Transform> itemList);
}

[DisallowMultipleComponent]
public class FixCountScrollView : MonoBehaviour
{
    public IScrollView iScrollViewDelegate;

    /// <summary>
    /// ScrollRect
    /// </summary>
    public ScrollRect m_ScrollRect;
    RectTransform m_ScrollView;

    /// <summary>
    /// GridContent //用于获取配置属性
    /// </summary>
    public GridLayoutGroup m_Grid;
    RectTransform m_Content;

    public Vector2 ElementSize = Vector2.zero;

    /// <summary>
    /// 排列方式枚举
    /// </summary>
    public enum ArrangeType
    {
        Horizontal = 0,//水平排列
        Vertical = 1,//垂直排列
    }
    /// <summary>
    /// 排列方式
    /// </summary>
    private ArrangeType arrangeType = ArrangeType.Horizontal;


    /// <summary>
    /// 元素链表
    /// </summary>
    List<Transform> m_Child = new List<Transform>();

    int maxCount;

    /// <summary>
    /// 显示区域长度或高度的一半
    /// </summary>
    float extents = 0;

    Vector2 SR_size = Vector2.zero;//SrollRect的尺寸
    Vector3[] conners = new Vector3[4];//ScrollRect四角的世界坐标 
    Vector2 startPos;//ScrollRect的初始位置

    public void InitContentSize(int count, IScrollView iScrollView, bool record = false)
    {
        maxCount = count;
        record &= (m_Child.Count > 0);
        this.iScrollViewDelegate = iScrollView;
        if (!record)
        {
            m_ScrollView.pivot = new Vector2(0.5f, 0.5f);
            m_Content.anchorMin = new Vector2(0.5f, 0.5f);
            m_Content.anchorMax = new Vector2(0.5f, 0.5f);
            m_Content.pivot = new Vector2(0, 1);
            //Logger.Log(m_ScrollView.rect);
            m_Content.localPosition = new Vector2(-m_ScrollView.rect.size.x / 2f, m_ScrollView.rect.size.y / 2f);

            //四角坐标  横着数  矩形区域
            //一号位中心点
            //①       ②
            //
            //③       ④

            SR_size = m_ScrollView.rect.size;

            conners[0] = new Vector3(-SR_size.x / 2f, SR_size.y / 2f, 0);
            conners[1] = new Vector3(SR_size.x / 2f, SR_size.y / 2f, 0);
            conners[2] = new Vector3(-SR_size.x / 2f, -SR_size.y / 2f, 0);
            conners[3] = new Vector3(SR_size.x / 2f, -SR_size.y / 2f, 0);

            //for (int i = 0; i < 4; i++)
            //{
            //    Vector3 temp = m_Content.parent.TransformPoint(conners[i]);
            //    conners[i].x = temp.x;
            //    conners[i].y = temp.y;
            //}
        }

        int childCont = 0;                      //创建列表项的个数
        Vector2 contentSize;                    //活动面板的大小

        switch (m_Grid.constraint)
        {
            case GridLayoutGroup.Constraint.Flexible:
                Logger.LogError("你不能这样做");
                break;
            case GridLayoutGroup.Constraint.FixedColumnCount:
                childCont = ((int)Mathf.Ceil((m_ScrollView.rect.size.y - m_Grid.padding.top - m_Grid.padding.bottom + m_Grid.spacing.y) / (m_Grid.cellSize.y + m_Grid.spacing.y)) + 2) * m_Grid.constraintCount;

                contentSize.x = m_ScrollView.rect.size.x;
                contentSize.y = m_Grid.padding.top + m_Grid.padding.bottom + ((int)Mathf.Ceil((float)count / (float)m_Grid.constraintCount)) * (m_Grid.cellSize.y + m_Grid.spacing.y) - m_Grid.spacing.y;
                contentSize.y = Mathf.Max(contentSize.y, m_ScrollView.rect.size.y);

                m_Content.sizeDelta = contentSize;
                break;
            case GridLayoutGroup.Constraint.FixedRowCount:
                childCont = ((int)Mathf.Ceil((m_ScrollView.rect.size.x - m_Grid.padding.left - m_Grid.padding.right + m_Grid.spacing.x) / (m_Grid.cellSize.x + m_Grid.spacing.x)) + 2) * m_Grid.constraintCount;

                contentSize.y = m_ScrollView.rect.size.y;
                contentSize.x = m_Grid.padding.left + m_Grid.padding.right + ((int)Mathf.Ceil((float)count / (float)m_Grid.constraintCount)) * (m_Grid.cellSize.x + m_Grid.spacing.x) - m_Grid.spacing.x;
                contentSize.x = Mathf.Max(contentSize.x, m_ScrollView.rect.size.x);

                m_Content.sizeDelta = contentSize;
                break;
        }
        if (m_Child.Count <= 0)
        {
            CleanContent();
            Vector2 size = m_Grid.cellSize;
            Vector3 scale = Vector3.one;
            if (ElementSize != Vector2.zero)
            {
                size = ElementSize;
                scale.x = m_Grid.cellSize.x / ElementSize.x;
                scale.y = m_Grid.cellSize.y / ElementSize.y;
            }
            for (int i = 0; i < childCont; i++)
            {
                Transform item = iScrollViewDelegate.IScrollViewCreateItem(this, m_Content);
                item.localScale = scale;
                item.GetComponent<RectTransform>().sizeDelta = size;
                m_Child.Add(item);
            }
        }
        ResetChildPosition();
        InitListData();
        OnDrag(Vector2.zero);
        m_ScrollRect.onValueChanged.AddListener(OnDrag);
    }
    public void CleanContent()
    {
        if (iScrollViewDelegate != null)
        {
            iScrollViewDelegate.IScrollViewCleanItem(this, m_Child);
        }
        if (m_Child.Count != 0)
        {
            m_Child.ForEach(delegate (Transform item) { Destroy(item.gameObject); });
            m_Child.Clear();
        }
    }

    void Awake()
    {
        if (m_ScrollRect == null)
        {
            m_ScrollRect = GetComponent<ScrollRect>();
        }

        m_ScrollView = m_ScrollRect.viewport == null ? m_ScrollRect.transform as RectTransform :
                                                   m_ScrollRect.viewport;

        if (m_Grid == null)
        {
            m_Grid = GetComponentInChildren<GridLayoutGroup>();
        }
        m_Content = m_Grid.transform as RectTransform;
        m_Grid.enabled = false;

        if ((m_ScrollRect.horizontal && m_ScrollRect.vertical) ||
            (!m_ScrollRect.horizontal && !m_ScrollRect.vertical) ||
            m_Grid.constraint == GridLayoutGroup.Constraint.Flexible ||
            (m_ScrollRect.vertical && m_Grid.constraint != GridLayoutGroup.Constraint.FixedColumnCount) ||
            (m_ScrollRect.horizontal && m_Grid.constraint != GridLayoutGroup.Constraint.FixedRowCount))
        {
            Logger.LogError("不能设置为纵横向都可以滑动,请检查ScrollRect和GridLayout配置");
            this.enabled = false;
            return;
        }
        arrangeType = m_ScrollRect.vertical ? ArrangeType.Vertical : ArrangeType.Horizontal;

    }

    void Start()
    {
    }
    void OnDrag(Vector2 delta)
    {
        //Debug.Log(gameObject.name+"         "+ Time.time+"    "+delta);
        Vector3[] conner_local = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            conner_local[i] = m_Content.InverseTransformPoint(m_Content.parent.TransformPoint(conners[i]));
        }
        //计算ScrollRect的中心坐标 相对于this的坐标
        Vector2 center = (conner_local[3] + conner_local[0]) / 2f;

        if (m_Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            //float top = conner_local[0].y + (m_Grid.cellSize.y + m_Grid.spacing.y) / 2f;
            //float bottom = conner_local[3].y - (m_Grid.cellSize.y + m_Grid.spacing.y) / 2f;

            for (int i = 0; i < m_Child.Count; i++)
            {
                Transform temp = m_Child[i];
                float distance = temp.localPosition.y - center.y;
                if (distance < -extents * 0.5f)
                {
                    Vector2 pos = temp.localPosition;
                    pos.y += extents * (int)Mathf.Ceil(Mathf.Abs(distance) / extents);
                    temp.localPosition = pos;
                    int realIndex = getRealIndex(pos);
                    ReloadData(temp, realIndex);
                }
                else if (distance > extents * 0.5f)
                {
                    Vector2 pos = temp.localPosition;
                    pos.y -= extents * (int)Mathf.Ceil(Mathf.Abs(distance) / extents); ;
                    temp.localPosition = pos;
                    int realIndex = getRealIndex(pos);
                    ReloadData(temp, realIndex);
                }
            }
        }
        else //水平
        {
            //float left = conner_local[0].x - (m_Grid.cellSize.x + m_Grid.spacing.x) / 2f;
            //float right = conner_local[3].x + (m_Grid.cellSize.x + m_Grid.spacing.x) / 2f;

            for (int i = 0; i < m_Child.Count; i++)
            {
                Transform temp = m_Child[i];
                float distance = temp.localPosition.x - center.x;

                if (distance < -extents * 0.5f)
                {
                    Vector2 pos = temp.localPosition;
                    pos.x += extents * (int)Mathf.Ceil(Mathf.Abs(distance) / extents);
                    temp.localPosition = pos;
                    int realIndex = getRealIndex(pos);
                    ReloadData(temp, realIndex);
                }
                else if (distance > extents * 0.5f)
                {
                    Vector2 pos = temp.localPosition;
                    pos.x -= extents * (int)Mathf.Ceil(Mathf.Abs(distance) / extents);
                    temp.localPosition = pos;
                    int realIndex = getRealIndex(pos);
                    ReloadData(temp, realIndex);
                }
            }
        }
    }
    int getRealIndex(Vector2 pos)//计算realindex 从0开始
    {
        int x = (int)Mathf.Ceil((-pos.y - m_Grid.padding.top) / (m_Grid.cellSize.y + m_Grid.spacing.y)) - 1;
        int y = (int)Mathf.Ceil((pos.x - m_Grid.padding.left) / (m_Grid.cellSize.x + m_Grid.spacing.x)) - 1;
        int realIndex;
        if (m_Grid.constraint==GridLayoutGroup.Constraint.FixedColumnCount)
            realIndex = x * m_Grid.constraintCount + y;
        else
            realIndex = x + m_Grid.constraintCount * y;
        return realIndex;
    }
    
    void ResetChildPosition()
    {
        Vector2 startAxis;                      //元素开始的位置
        int imax=m_Child.Count;
        int rows = 1, cols = 1;
        startAxis = new Vector2(m_Grid.padding.left + m_Grid.cellSize.x / 2f, -m_Grid.padding.top - m_Grid.cellSize.y / 2f);
        if (m_Grid.constraint== GridLayoutGroup.Constraint.FixedColumnCount)
        {
            cols = m_Grid.constraintCount;
            rows = (int)Mathf.Ceil((float)imax / (float)cols);
            extents = (imax / m_Grid.constraintCount) * (m_Grid.cellSize.y + m_Grid.spacing.y);
        }
        else
        {
            rows = m_Grid.constraintCount;
            cols = (int)Mathf.Ceil((float)imax / (float)rows);
            extents = (imax / m_Grid.constraintCount) * (m_Grid.cellSize.x + m_Grid.spacing.x);
        }
        for (int i = 0; i < imax; i++)
        {
            Transform temp = m_Child[i];
            int x = 0, y = 0;//行列号
            if (m_Grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                x = i / cols;
                y = i % cols;
            }
            else
            {
                x = i % rows;
                y = i / rows;
            }
            temp.localPosition = new Vector2(startAxis.x + y * (m_Grid.cellSize.x + m_Grid.spacing.x), startAxis.y - x * (m_Grid.cellSize.y + m_Grid.spacing.y));
        }
    }

    void InitListData()
    {
        int realIndex;
        for (int i = 0; i < m_Child.Count; i++)
        {
            realIndex = getRealIndex(m_Child[i].localPosition);
            ReloadData(m_Child[i], realIndex);
        }
    }

    void ReloadData(Transform item, int index)
    {
        if (index >= 0 && index < maxCount)
        {
            item.gameObject.SetActive(true);
            if (iScrollViewDelegate != null)
            {
                iScrollViewDelegate.IScrollViewReloadItem(this, item, index);
            }
        }
        else
        {
            item.gameObject.SetActive(false);
        }
    }
}
