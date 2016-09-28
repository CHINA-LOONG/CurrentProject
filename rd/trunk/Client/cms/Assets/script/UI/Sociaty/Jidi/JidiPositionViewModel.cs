using UnityEngine;
using System.Collections;

public class JidiPositionViewModel : MonoBehaviour
{

    public GameObject openObject;
    public GameObject lockedObject;
	// Use this for initialization
    public  void ShowOpenObject(bool bshow)
    {
        openObject.SetActive(bshow);
    }
    public  void ShowLockedObject(bool bshow)
    {
        lockedObject.SetActive(bshow);
    }
}
