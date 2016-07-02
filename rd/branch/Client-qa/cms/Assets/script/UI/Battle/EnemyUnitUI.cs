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

	public void	UpdateShow(GameUnit battleUnit)
	{
		if (null == battleUnit || battleUnit.gameObject == null)
		{
			//gameObject.SetActive(false);
			//return;
		}
		Vector3 uiDumpPostion = battleUnit.gameObject.transform.position;
		Vector2 pos = RectTransformUtility.WorldToScreenPoint (BattleCamera.Instance.CameraAttr, 
		                                                       new Vector3 (uiDumpPostion.x,uiDumpPostion.y + GameConfig.Instance.EnmeyUnitOffsetYForBloodUI, uiDumpPostion.z));
		transform.position = pos;

		unitName.text = battleUnit.name;
		lifeBar.value = battleUnit.curLife / (float)battleUnit.maxLife;
		unitLevel.text = battleUnit.pbUnit.level.ToString ();




	}
}
