using UnityEngine;
using System.Collections;
using System;

public class ColorConst 
{
	//品质边框对应的品质文字颜色( + 3 )
	public	static	Color GetStageTextColor(int stage)
	{
		switch (stage) 
		{
		case 1:
			return text_color_grade_1;
		case 2:
			return text_color_grade_2;
		case 3:
			return text_color_grade_3;
		case 4:
			return text_color_grade_4;
		case 5:
			return text_color_grade_5;
		case 6:
			return   text_color_grade_6;
		}
		return text_color_grade_1;
	}
	
	public	static	Color GetStageOutLineColor(int stage)
	{
		switch (stage) 
		{
		case 1:
			return outline_color_grade_1;
		case 2:
			return outline_color_grade_2;
		case 3:
			return outline_color_grade_3;
		case 4:
			return outline_color_grade_4;
		case 5:
			return outline_color_grade_5;
		case 6:
			return   outline_color_grade_6;
		}
		return outline_color_grade_1;
	}
    //系统字色黑
    public static Color system_color_black = new Color(251.0f / 255.0f, 241.0f / 255.0f, 216.0f / 255.0f, 1.0f);
    //系统字色白
    public static Color system_color_white = new Color(96.0f / 255.0f, 76.0f / 255.0f, 51.0f / 255.0f, 1.0f);

	//白色品质
	public	static 	Color	text_color_grade_1 = new Color(1,1,1,1);
	public	static	Color	outline_color_grade_1 = new Color(0.45f,0.41f,0.63f,1);	
	//绿色品质
	public	static 	Color	text_color_grade_2 = new Color(122.0f/255.0f,1,0,1);
	public	static	Color	outline_color_grade_2 = new Color(0,95.0f/255.0f,50.0f/255.0f,1);	
	//蓝色品质
	public	static 	Color	text_color_grade_3 = new Color(0,198.0f/255.0f,1,1);
	public	static	Color	outline_color_grade_3 = new Color(0,65.0f/255.0f,235.0f/255.0f,1);	
	//紫色品质
	public	static 	Color	text_color_grade_4 = new Color(200.0f/255.0f,160.0f/255.0f,1,1);
	public	static	Color	outline_color_grade_4 = new Color(140.0f/255.0f,15.0f/255.0f,1,1);
	//橙色品质
	public	static 	Color	text_color_grade_5 = new Color(1,135.0f/255.0f,6.0f/255.0f,1);
	public	static	Color	outline_color_grade_5 = new Color(185.0f/255.0f,30.0f/255.0f,0,1);	
	//红色品质
	public	static 	Color	text_color_grade_6 = new Color(1,65.0f/255.0f,98.0f/255.0f,1);
    public static Color outline_color_grade_6 = new Color(148.0f / 255.0f, 0, 0, 1);
    //需求足够
    public static Color text_color_Req = new Color(251.0f / 255.0f, 241.0f/255.0f, 216.0f/255.0f, 1);
    public static Color outline_color_Req = new Color(0, 95.0f / 255.0f, 50.0f / 255.0f, 1);	
    //需求不够
    public static Color text_color_nReq = new Color(1, 65.0f/255.0f, 98.0f/255.0f, 1);
    public static Color outline_color_nReq = new Color(148.0f / 255.0f, 0, 0, 1);
	//shopitem buyprice color
	public	static	Color	text_color_Enough = new Color (96.0f / 255.0f, 76.0f / 255.0f, 51.0f / 255.0f, 1);
    //频道字体颜色
    public static Color globalColor = new Color(251.0f / 255.0f, 241.0f / 255.0f, 216.0f / 255.0f, 1);//世界字体颜色
    public static Color guildColor = new Color(251.0f / 255.0f, 241.0f / 255.0f, 216.0f / 255.0f, 1);//工會字体颜色
    public static Color systemColor = new Color(255.0f / 255.0f, 65.0f / 255.0f, 98.0f / 255.0f, 1);//系统字体颜色
    public static Color nameColor = new Color(255.0f / 255.0f, 204.0f / 255.0f, 0.0f / 255.0f);//默认名字颜色
    public static Color guildTaskColor = new Color(0f, 198.0f / 255.0f, 1f);//默认名字颜色
    //选项卡颜色
    public static Color text_tabColor_normal = new Color(250f/255.0f,247f/255f,241/255f,1f);
    public static Color outline_tabColor_normal = new Color(202f/255f,151f/255f,13/255f,1f);
    public static Color text_tabColor_select = new Color(255f/255f,255f/255f,255/255f,1f);
    public static Color outline_tabColor_select = new Color(20f/255f,15f/255f,16f/255f,1f);
    //服务器状态颜色
    public static Color server_color_new = new Color(122f / 255f, 1f, 0f, 1f);
    public static Color server_color_full = new Color(1f, 0f, 0f, 1f);
    public static Color server_color_hot = new Color(1f, 204f / 255f, 0f, 1f);
    public static Color server_color_Maintain = new Color(143f/255f, 143f / 255f, 143f/255f, 1f);
    //合成碎片足够颜色
    public static Color text_color_fullFragment = new Color(140f / 255f, 15f / 255f, 1f, 1f);
    public static string colorTo_Hstr(Color color)
    {
        string strColor = "#";
        strColor += string.Format("{0:X2}", (int)(color.r * 255));
        strColor += string.Format("{0:X2}", (int)(color.g * 255));
        strColor += string.Format("{0:X2}", (int)(color.b * 255));
        strColor += string.Format("{0:X2}", (int)(color.a * 255));
        return strColor;
    }


}
