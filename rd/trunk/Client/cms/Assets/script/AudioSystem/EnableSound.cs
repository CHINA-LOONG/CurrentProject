//================================
//  Create by xuelong.
//  Mail:[xuelong@way4games.com]
//================================

using UnityEngine;
using System.Collections;

public class EnableSound : MonoBehaviour
{
    public string audioName;

    void OnAwake()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        AudioSystemMgr.Instance.PlaySoundByName(audioName);
        gameObject.SetActive(false);
    }
}
