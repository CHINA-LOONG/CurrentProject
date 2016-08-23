using UnityEngine;
using System.Collections;
using Funplus;

public class FunPlus : MonoBehaviour {

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        FunplusSdk.GetInstance().SetGameObjectAndDelegate("Funplus", new FunPlusCallbacks());
        FunplusAccount.GetInstance().SetGameObjectAndDelegate("Funplus", new FunPlusCallbacks());

        FunplusSdk.GetInstance().Install(Const.FunPlusID, Const.FunPlusKey, "sandbox");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
