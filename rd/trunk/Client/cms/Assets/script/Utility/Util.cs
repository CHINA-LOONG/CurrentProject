using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class Util
{
    public static List<System.Diagnostics.Stopwatch> sWatchList = new List<System.Diagnostics.Stopwatch>();
    public static void EnterFunction()
    {
        System.Diagnostics.Stopwatch sWatch = new System.Diagnostics.Stopwatch();
        sWatchList.Add(sWatch);
        sWatch.Start();
    }

    public static void ExitFunction(string functionName)
    {
        int count = sWatchList.Count - 1;
        sWatchList[count].Stop(); //  停止监视
        System.TimeSpan timeSpan = sWatchList[count].Elapsed; //  获取总时间
        double milliseconds = timeSpan.TotalMilliseconds;  //  毫秒数

        Debug.LogFormat("{0}: ms:{1}", functionName, milliseconds);

        sWatchList.RemoveAt(count);
    }



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

    public static string AssetBundlePath
    {
        get
        {
            return Path.Combine(ResPath, Const.AssetDirname);
        }
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
    /// 从bytes解压zip文件
    /// </summary>
    public static void UnZipFromBytes(byte[] bytes, string destDir)
    {
        if (bytes.Length == 0)
            return;

        MemoryStream stream = new MemoryStream(bytes);
        using (ZipInputStream s = new ZipInputStream(stream))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);

                // create directory
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(Path.Combine(destDir, directoryName));

                if (fileName != string.Empty)
                {
                    using (FileStream streamWriter = File.Create(Path.Combine(destDir, theEntry.Name)))
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                                streamWriter.Write(data, 0, size);
                            else
                                break;
                        }
                    }
                }
            }
        }
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

    public static int StringByteLength(string str)
    {
        int length = 0;
        if (string.IsNullOrEmpty(str))
            return 0;

        char[] strArray = str.ToCharArray();
        foreach(var subChar in strArray)
        {
            int asc = subChar;
            if(subChar < 0 || subChar > 127)
            {
                length += 2;
            }
            else
            {
                length += 1;
            }
        }

        return length;
    }

    public static bool StringIsAllNumber(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;
        char[] strArray = str.ToCharArray();
        foreach (var subChar in strArray)
        {
            int asc = subChar;
            if (subChar < 48 || subChar > 57)
            {
                return false;
            }
        }
        return true;
    }

    public static PbUnit CreatePbUnitFromHsMonster(PB.HSMonster monster, UnitCamp camp)
    {
        PbUnit unit = new PbUnit();
        unit.slot = -1;
        unit.guid = monster.monsterId;
        unit.camp = camp;
        unit.id = monster.cfgId;
        unit.character = monster.disposition;
        unit.lazy = monster.lazy;
        unit.level = monster.level;
        unit.curExp = monster.exp;
        unit.stage = monster.stage;
        unit.spellPbList = monster.skill;
        unit.monsterState = monster.state;

        return unit;
    }
    public static int GetBpFromHsMonster(PB.HSMonster monster)
    {
        PbUnit tmpPbUnit = Util.CreatePbUnitFromHsMonster(monster, UnitCamp.Player);
        GameUnit tmpUnit = GameUnit.FromPb(tmpPbUnit, true);
        return tmpUnit.mBp;
    }
}
