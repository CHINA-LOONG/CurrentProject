using UnityEngine;
using System.Collections;

public class GoldChargeData 
{
	public	string	id;
	public	int	maxTimes;
	public	int	consume;
	public	int	consumeTimeAdd;
	public	int	consumeTimeDoubel;
	public	int	award;
	public	int	awardTimeAdd;
	public	float	levelAdd;
	public	float	twoMultiple;
	public	float	fiveMultiple;
	public	float	tenMultiple;

	public	int	GetBaseZuanshiWithTime(int iTime)
	{
		int baseConsum = consume + (iTime - 1) * consumeTimeAdd;
		int power = (int)System.Math.Floor ((iTime - 1) / (float)consumeTimeDoubel);
		int result = (int) (baseConsum * Mathf.Pow(2,power));

		return result;
	}

	public	int	GetBaseJinBiWithTime(int iTime)
	{
		int baseAward = award + awardTimeAdd * (iTime - 1);
        float levelAffect = GameDataMgr.Instance.PlayerDataAttr.level * levelAdd + 1.0f;
        float result = (baseAward * levelAffect);

        return (int)result;
	}
}
