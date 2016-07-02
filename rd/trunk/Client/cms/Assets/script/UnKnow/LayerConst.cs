using UnityEngine;
using System.Collections;

public class LayerConst  
{
	public static	int	WeakpointLayer = 0;

	public static	 void	Init()
	{
		WeakpointLayer = 1 << LayerMask.NameToLayer ("WeakPointLayer");
	}
}
