using UnityEngine;
using System.Collections;

public class LayerConst  
{
	public static	int	WeakpointLayer = 0;
	public static	int WeakpointLayerMask = 0;

	public static	 void	Init()
	{
		WeakpointLayer =   LayerMask.NameToLayer ("WeakPointLayer");
		WeakpointLayerMask = 1 << WeakpointLayer;
		//Debug.LogError ("dhhel "+ WeakpointLayer);
	}
}
