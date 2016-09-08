using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExchangeCoinlEffect : MonoBehaviour
{
    public Text exchangeTips;
    public float destroyAfterSecond = 2.0f;
	
    public static   void CreateWith(Transform parantTrans, bool isBaoji, int getcoin)
    {
        GameObject go = null;
        if (isBaoji)
        {
            go = ResourceMgr.Instance.LoadAsset("ExchangeCoinBaojiEffect");
        }
        else
        {
            go = ResourceMgr.Instance.LoadAsset("ExchangeCoinNormalEffect");
        }
        go.transform.SetParent(parantTrans);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        ExchangeCoinlEffect coinEffect = go.GetComponent<ExchangeCoinlEffect>();
        coinEffect.InitWith(isBaoji, getcoin);
    }

    public void InitWith(bool isBaoji,int getcoin)
    {
        if (isBaoji)
        {
            exchangeTips.text = string.Format(StaticDataMgr.Instance.GetTextByID("succbuy_baoji"), getcoin);
        }
        else
        {
            exchangeTips.text = string.Format(StaticDataMgr.Instance.GetTextByID("shop_succbuy"), getcoin);
        }
        StartCoroutine(exitCo());
    }
    IEnumerator exitCo()
    {
        yield return new WaitForSeconds(destroyAfterSecond);
        ResourceMgr.Instance.DestroyAsset(gameObject);
    }
}
