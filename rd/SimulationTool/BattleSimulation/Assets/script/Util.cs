using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using ICSharpCode.SharpZipLib.Zip;

public class Util
{
    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath()
    {
        return Application.streamingAssetsPath;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 打包assetbundle时的路径，StreamingAssets下面可以打包到apk中
    /// </summary>
    public static string BuildPath
    {
        get
        {
            return Application.streamingAssetsPath;
        }
    }
#endif

    /// <summary>
    /// 取得数据存放目录，用来将从apk中解压出来的数据存到sd卡上
    /// </summary>
    public static string ResPath
    {
        get
        {
            if (Const.DebugMode)
                return Application.streamingAssetsPath;
            else
                //不能直接使用Application.persistentDataPath，由于涉及到删除目录操作，删除根目录无权限，故加上game子目录
                return Path.Combine(Application.persistentDataPath, "game");
        }
    }

    public static string StaticDataPath
    {
        get { return Path.Combine(ResPath, "staticData"); }
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null) Object.Destroy(ts[i]);
            }
            return go.gameObject.AddComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// 清理内存
    /// </summary>
    public static void ClearMemory()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

	/// <summary>
	/// 对象查找
	/// </summary>
	/// <returns>The child by name.</returns>
	/// <param name="go">Go.</param>
	/// <param name="name">Name.</param>
	public static GameObject FindChildByName(GameObject go, string name)
	{
        if (go == null)
        {
            return null;
        }

        //use transform.find() first,since sometimes the node has same name with different parent, we find them use exact address
        Transform curTran = go.transform.Find(name);
        if (curTran != null)
        {
            return curTran.gameObject;
        }

		Transform[] trans = go.transform.GetComponentsInChildren<Transform> (true);
		foreach (Transform tran in trans)
		{
			if (tran.name == name) 
			{
				return tran.gameObject;
			}
		}
		
		return null;
	}

	/// <summary>
	/// 带权重的随机
	/// </summary>
	/// <returns>The Index of List.</returns>
	/// <param name="weightList">Weight list.</param>
	public	static int RondomWithWeight(List<int> weightList)
	{
		int sum = 0;
		for (int i = 0; i<weightList.Count; ++i) 
		{
			if(weightList[i] < 0)
			{
				Logger.LogError("RondomWithWeight Eror param sum = " + sum);
				return -1;
			}

			sum += weightList[i];
		}
		if (sum <= 0) 
		{
			Logger.LogError("RondomWithWeight Eror param sum = " + sum);
			return -1;
		}

		int value = Random.Range (0, sum);// return [)

		int count = 0;
		int subWeight = 0;
		for(int i =0;i<weightList.Count ; ++i)
		{
			subWeight = weightList[i];
			count += subWeight;
			if(0 == subWeight)
			{
				continue;
			}
			if(value < count)
			{
				return i;
			}
		}
		return weightList.Count - 1;
	}

	public static List<int> RondomNoneReatNumbers(int min,int max,int numbers)
	{
		int Count = max - min;
		if (min == max) 
		{
			Count = 1;
		}
		int[] numberSZ = new int[Count];
		for (int i = 0; i<Count; ++i)
		{
			numberSZ[i] = min + i;
		}

		int tempIndex = 0;
		int tempValue = 0;
		for (int i =0; i<Count; ++i)
		{
			tempIndex = Random.Range(0,Count);
			tempValue = numberSZ[i];
			numberSZ[i] = numberSZ[tempIndex];
			numberSZ[tempIndex] = tempValue;
		}
		List<int> listReturn = new List<int> ();
		for (int i =0; i<numbers; ++i)
		{
			listReturn.Add(numberSZ[i]);
		}

		return listReturn;
	}

    public static float ParticleSystemLength(Transform transform)
    {
        ParticleSystem[] particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
        float maxDuration = 0;
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps.enableEmission)
            {
                if (ps.loop)
                {
                    return -1f;
                }
                float dunration = 0f;
                if (ps.emissionRate <= 0)
                {
                    dunration = ps.startDelay + ps.startLifetime;
                }
                else
                {
                    dunration = ps.startDelay + Mathf.Max(ps.duration, ps.startLifetime);
                }
                if (dunration > maxDuration)
                {
                    maxDuration = dunration;
                }
            }
        }
        return maxDuration;
    }

    //通过物体包含组件来设置事件
    static public void SetChild_UsedT<T>(Transform t, System.Action<Transform> action) where T : Component
    {
        if (action != null)
        {
            if (t.GetComponent<T>() != null)
            {
                action(t);
            }
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform child = t.GetChild(i);
                //if (child.GetComponent<T>() != null)
                //{
                //    action(child);
                //}
                SetChild_UsedT<T>(child, action);
            }
        }
    }
}
