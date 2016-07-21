using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PetDetailRightAttr : PetDetailRightBase {

    public Text lifeLabel;
    public Text defenseLabel;
    public Text phyAttactLabel;
    public Text magicAttactLabel;
    public Text speedLabel;
    public Text recoveryLabel;
    public Text propertyAddLabel;
    public Text criticalRatioLabel;
    public Text enduranceLabel;
    public Text criticalDamageLabel;
    public Text hitRatioLabel;
    public Text healAffectLabel;
    public Text minusDamageRatioLabel;
    public Text additionDamageRatioLabel;


    public override void ReloadData(PetRightParamBase obj)
    {
        GameUnit unit = obj.unit;

        lifeLabel.text = unit.maxLife.ToString();
        defenseLabel.text = unit.defense.ToString();
        phyAttactLabel.text = unit.phyAttack.ToString();
        magicAttactLabel.text = unit.magicAttack.ToString();
        speedLabel.text = unit.speed.ToString();
        recoveryLabel.text = unit.recovery.ToString();
        propertyAddLabel.text = unit.health.ToString();
        criticalRatioLabel.text = unit.criticalRatio.ToString();
        enduranceLabel.text = unit.endurance.ToString();
        criticalDamageLabel.text = unit.criticalDamageRatio.ToString();
        hitRatioLabel.text = unit.hitRatio.ToString();
        healAffectLabel.text = unit.additionHealRatio.ToString();
        minusDamageRatioLabel.text = unit.minusDamageRatio.ToString();
        additionDamageRatioLabel.text = unit.additionDamageRatio.ToString();

    }
}
