using UnityEngine;
using System.Collections;

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
    public static Color text_color_Req = new Color(122.0f / 255.0f, 1, 0, 1);
    public static Color outline_color_Req = new Color(0, 95.0f / 255.0f, 50.0f / 255.0f, 1);	
    //需求不够
    public static Color text_color_nReq = new Color(1, 0, 0, 1);
    public static Color outline_color_nReq = new Color(148.0f / 255.0f, 0, 0, 1);


	//shopitem buyprice color
	public	static	Color	text_color_Enough = new Color (96.0f / 255.0f, 76.0f / 255.0f, 51.0f / 255.0f, 1);

}
