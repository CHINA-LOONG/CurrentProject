using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnitData
{
    //TODO：标记查看是否修改
	private static System.ComponentModel.Int64Converter _unused = new System.ComponentModel.Int64Converter();
	private static System.ComponentModel.DecimalConverter _unused2 = new System.ComponentModel.DecimalConverter();
	private static System.ComponentModel.ByteConverter _unused3 = new System.ComponentModel.ByteConverter();
	private static System.ComponentModel.CollectionConverter _unused4 = new System.ComponentModel.CollectionConverter();
	private static System.ComponentModel.CharConverter _unused5 = new System.ComponentModel.CharConverter();
	private static System.ComponentModel.SByteConverter _unused6 = new System.ComponentModel.SByteConverter();
	private static System.ComponentModel.Int16Converter _unused7 = new System.ComponentModel.Int16Converter();
	private static System.ComponentModel.UInt16Converter _unused8 = new System.ComponentModel.UInt16Converter();
	private static System.ComponentModel.Int32Converter _unused9 = new System.ComponentModel.Int32Converter();
	private static System.ComponentModel.UInt32Converter _unused10 = new System.ComponentModel.UInt32Converter();
	private static System.ComponentModel.Int64Converter _unused11 = new System.ComponentModel.Int64Converter();
	private static System.ComponentModel.UInt64Converter _unused12 = new System.ComponentModel.UInt64Converter();
	private static System.ComponentModel.DoubleConverter _unused13 = new System.ComponentModel.DoubleConverter();
	private static System.ComponentModel.SingleConverter _unused14 = new System.ComponentModel.SingleConverter();
	private static System.ComponentModel.BooleanConverter _unused15 = new System.ComponentModel.BooleanConverter();
	private static System.ComponentModel.StringConverter _unused16 = new System.ComponentModel.StringConverter();
	private static System.ComponentModel.DateTimeConverter _unused17 = new System.ComponentModel.DateTimeConverter();
	//private static System.ComponentModel.EnumConverter _unused18 = new System.ComponentModel.EnumConverter(typeof(<any your enum>));
	private static System.ComponentModel.TimeSpanConverter _unused19 = new System.ComponentModel.TimeSpanConverter();

    public string id;
    public string assetID;
    public string uiAsset;
    public string nickName;
    public int type;
    public int rarity;
    public byte isEvolutionable;
    public string evolutionID;
    public int property;

    public float levelUpExpRate;
    public float healthModifyRate;
    public float strengthModifyRate;
    public float intelligenceModifyRate;
    public float speedModifyRate;
    public float defenseModifyRate;
    public float enduranceModifyRate;
    //public float goldNoteMinValueModifyRate;
   // public float goldNoteMaxValueModifyRate;
   // public float expMinValueModifyRate;
   // public float expMaxValueModifyRate;
    public float recoveryRate;

    public int equip;
   // public string AI;

    public string spellIDList;
    public string weakpointList;
	public int	friendship;
	public int disposition;
    public string closeUp;
    public string fragmentId;
    public int fragmentCount;
    public string say;

    public string NickNameAttr
	{
		get
		{
			return StaticDataMgr.Instance.GetTextByID(nickName);
		}
	}
}
