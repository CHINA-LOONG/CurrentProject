using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public enum SoundType
{
    UI = 0x001,
    Tips = 0x002,
    Click = 0x004
}

public enum TriggerType
{
    UI_Open = 0x010,
    UI_Close = 0x020,
    Tips_def = 0x040,
    Click_def = 0x080
}

[ExecuteInEditMode]
public class PlaySound : MonoBehaviour,IPointerClickHandler
{
    [HideInInspector][SerializeField]SoundType type = SoundType.Click;

    [HideInInspector][SerializeField]string sound1;
    [HideInInspector][SerializeField]string sound2;

    public SoundType Type
    {
        get { return type; }
        set { type = value; }
    }

    public string Sound1
    {
        get { return sound1; }
        set { sound1 = value; }
    }

    public string Sound2
    {
        get { return sound2; }
        set { sound2 = value; }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (type==SoundType.Click&&!string.IsNullOrEmpty(sound1))
        {
            //TODO：临时关闭按钮声音
            //AudioSystemMgr.Instance.PlaySoundByID(sound1);
        }
    }
}
