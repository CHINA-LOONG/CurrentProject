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

    void Start()
    {
        buffView = gameObject.GetComponent<UIBuffView>();
        buffView.Init();
    }

	public void	UpdateShow(GameUnit battleUnit)
	{
        lifeBar.LifeTarget = battleUnit;

		if (null == battleUnit || battleUnit.gameObject == null)
		{
			//gameObject.SetActive(false);
			//return;
		}
        buffView.SetTargetUnit(battleUnit);

		Vector3 uiDumpPostion = battleUnit.gameObject.transform.position;
		Vector2 pos = RectTransformUtility.WorldToScreenPoint (BattleCamera.Instance.CameraAttr, 
		                                                       new Vector3 (uiDumpPostion.x,uiDumpPostion.y + GameConfig.Instance.EnmeyUnitOffsetYForBloodUI, uiDumpPostion.z));
		transform.position = pos;

		unitName.text = battleUnit.name;
        if (battleUnit.isBorn)
        {
		    lifeBar.value = battleUnit.curLife / (float)battleUnit.maxLife;
            battleUnit.isBorn = false;
        }
		unitLevel.text = battleUnit.pbUnit.level.ToString ();
	}
}
