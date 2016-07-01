using UnityEngine;
using System.Collections;

public class ModuleBase : MonoBehaviour 
{

	[SerializeField]
	string mModuleName;
	public string ModuleNameAttr
	{
		get
		{
			return mModuleName;
		}
		set
		{
			mModuleName = value;
		}
	}
		
	public virtual void OnInit(object param)
	{
		
	}
	
	public virtual void OnEnter(ModuleBase prevModule, object param)
	{
		
	}
	
	public virtual void OnExecute()
	{
		
	}
	
	public virtual void OnExit(ModuleBase nextModule)
	{
		
	}	
}
