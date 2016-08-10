using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using DG.Tweening;

public class HitCombo : MonoBehaviour
{
	//public	Image	comboImage;
    public Text comboImage;
	static	int	ComboMax = 20;

	public	static	void	ShowCombo(Transform parent,int comboNum,float posX, float posY)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("hitCombo");
		go.transform.SetParent (parent);
		go.transform.localScale = Vector3.one;

		var combo = go.GetComponent<HitCombo> ();
		combo.SetCombo (comboNum, posX, posY);

	}

	public void	SetCombo(int comboNum,float posX, float posY)
	{
		string comboName;
		if (comboNum < 1 || comboNum > ComboMax) 
		{
			comboName = "M";
		}
		else
		{
			comboName = comboNum.ToString();
		}
		
		//Sprite comboSprite = ResourceMgr.Instance.LoadAssetType<Sprite> (comboName) as Sprite;
        if (null != comboImage)
		{
            comboImage.text = comboName;
		}
		
		RectTransform rt = transform as RectTransform;
		rt.anchoredPosition3D = new Vector3 (posX, posY,0);
		
		rt.DOAnchorPos (new Vector2 (posX, posY + 80), 0.5f).OnComplete (OnFinished);
	}

	void OnFinished()
	{
		Destroy (gameObject);
	}
}
