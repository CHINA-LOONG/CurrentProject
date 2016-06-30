using System;
using UnityEngine;
using LuaInterface;

public class PrimitiveTypeWrap
{
	static LuaMethod[] enums = new LuaMethod[]
	{
		new LuaMethod("Sphere", GetSphere),
		new LuaMethod("Capsule", GetCapsule),
		new LuaMethod("Cylinder", GetCylinder),
		new LuaMethod("Cube", GetCube),
		new LuaMethod("Plane", GetPlane),
		new LuaMethod("Quad", GetQuad),
		new LuaMethod("IntToEnum", IntToEnum),
	};

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "UnityEngine.PrimitiveType", typeof(PrimitiveType), enums);
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetSphere(IntPtr L)
	{
		LuaScriptMgr.Push(L, PrimitiveType.Sphere);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCapsule(IntPtr L)
	{
		LuaScriptMgr.Push(L, PrimitiveType.Capsule);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCylinder(IntPtr L)
	{
		LuaScriptMgr.Push(L, PrimitiveType.Cylinder);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCube(IntPtr L)
	{
		LuaScriptMgr.Push(L, PrimitiveType.Cube);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetPlane(IntPtr L)
	{
		LuaScriptMgr.Push(L, PrimitiveType.Plane);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetQuad(IntPtr L)
	{
		LuaScriptMgr.Push(L, PrimitiveType.Quad);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int IntToEnum(IntPtr L)
	{
		int arg0 = (int)LuaDLL.lua_tonumber(L, 1);
		PrimitiveType o = (PrimitiveType)arg0;
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

