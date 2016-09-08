using UnityEngine;
using System.Collections;
using DG.Tweening;

//摄像机 动画  时间配置
public class BattleCameraAni 
{
	//运动到物理大招模式
	public static	Tweener	MotionToPhyDazhao()
	{
		return	BattleCamera.Instance.cameraAni.MotionTo (BattleController.Instance.GetPhyDazhaoCameraNode(), 1.0f).SetEase(GameConfig.Instance.InPhyDazhaoEaseAni);
	}
	//运动到默认镜头模式
	public	static	Tweener	MotionToDefault()
	{
		return	BattleCamera.Instance.cameraAni.MotionTo (BattleController.Instance.GetDefaultCameraNode(), 1.0f).SetEase(GameConfig.Instance.OutPhyDazhaoEaseAni);
	}
	//直接切换到默认镜头模式
	public	static void		SetDefaultNoAni()
	{
		BattleCamera.Instance.cameraAni.MotionTo (BattleController.Instance.GetDefaultCameraNode(), 0,false);
	}
}
