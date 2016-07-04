using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyUnitUI : MonoBehaviour 
{
	public Text unitName;
	public Text unitLevel;
	public LifeBarUI lifeBar;
	public int	slot = 0;

    public GameUnit Unit { get; set; }
    private UIBuffView buffView;
	//RectTransform rectTrans;

    void Start()
    {
        buffView = gameObject.GetComponent<UIBuffView>();
        buffView.Init();
		//rectTrans = transform as RectTransform;
    }

	public void	UpdateShow(BattleObject battleUnit)
	{
        lifeBar.LifeTarget = battleUnit;

        buffView.SetTargetUnit(battleUnit);

		//Vector3 viewPosition = BattleCamera.Instance.CameraAttr.WorldToViewportPoint (battleUnit.gameObject.transform.position);
		//Vector3 screenPosition = UICamera.Instance.CameraAttr.ViewportToScreenPoint (viewPosition);
		//rectTrans.anchoredPosition = screenPosition;

		unitName.text = battleUnit.name;
        //if (battleUnit.gameObject.activeSelf)
        //{
        //    lifeBar.value = battleUnit.unit.curLife / (float)battleUnit.unit.maxLife;
        //    //battleUnit.isBorn = false;
        //}
		unitLevel.text = battleUnit.unit.pbUnit.level.ToString ();
	}
}
