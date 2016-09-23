using UnityEngine;
using System.Collections;

public class SummonEffects : MonoBehaviour {
    public GameObject kuEnd;
    public GameObject xiaoEnd;
    public GameObject jingEnd;
    public GameObject xiaoMian;
    public GameObject jingMian;
    public GameObject taiyangB1;
    public GameObject taiyangB2;
    public GameObject yueliangB1;
    public GameObject yueliangB2;
    public Animator summAnim;
	// Use this for initialization
    public void Reset()
    {
        kuEnd.SetActive(false);
        xiaoEnd.SetActive(false);
        jingEnd.SetActive(false);
        xiaoMian.SetActive(false);
        jingMian.SetActive(false);
        taiyangB1.SetActive(false);
        taiyangB2.SetActive(false);
        yueliangB1.SetActive(false);
        yueliangB2.SetActive(false);
    }
}
