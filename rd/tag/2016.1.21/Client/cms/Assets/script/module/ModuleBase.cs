using UnityEngine;
using System.Collections;

public abstract class ModuleBase : MonoBehaviour 
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

    public abstract void OnInit(object param);

    public abstract void OnEnter(object param);

    public abstract void OnExecute();

    public abstract void OnExit();

    public virtual IEnumerator LoadResourceFinish()
    {
        yield return null;
    }
}
