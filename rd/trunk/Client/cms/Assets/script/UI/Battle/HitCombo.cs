using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using DG.Tweening;

public class HitCombo : MonoBehaviour
{
	public	Image	comboImage;

	static	int	ComboMax = 10;

	public	static	void	ShowCombo(Transform parent,int comboNum,float posX, float posY)
	{
		GameObject go = ResourceMgr.Instance.LoadAsset ("ui/hitcombo", "hitCombo");
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
			comboName = "combomax";
		}
		else
		{
			comboName = "combo_" + comboNum.ToString();
		}
		
		Sprite comboSprite = ResourceMgr.Instance.LoadAssetType<Sprite> ("ui/combo", comboName) as Sprite;
		if (null != comboSprite)
		{
			comboImage.sprite = comboSprite;
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
