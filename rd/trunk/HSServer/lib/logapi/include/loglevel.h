#ifndef _LOG_LEVEL_MACRO_H_
#define _LOG_LEVEL_MACRO_H_

/***********************************************日志级别定义*********************************************************/
//值	|级别		|描述								|文本类存储时长（可以动态配置）		|监控类日志建议级别
//0		|PANIC		|该系统不可用						|3年					|
//1		|ALERT		|需要立即被修改的条件				|2年					|
//2		|CRIT		|阻止某些工具或子系统功能实现的错误
//                  |条件				                |1年					|
//3		|ERR		|阻止工具或某些子系统部分功能实现的
//                  |错误条件			                |6个月					|异常类
//4		|WARNING	|预警信息							|3个月					|逻辑错误类
//5		|NOTICE		|具有重要性的普通条件				|2个月					|普通状态类
//6		|INFO		|提供信息的消息						|1个月					|默认
//7		|DEBUG		|不包含函数条件或问题的其他信息		|15天					|
//8		|NONE		|没有重要级，通常用于排错			|1天					|

#define MAINTENANCE_LOG_LEVEL_PANIC	0	//该系统不可用

#define MAINTENANCE_LOG_LEVEL_ALERT	1	//需要立即被修改的条件

#define MAINTENANCE_LOG_LEVEL_CRIT	2	//阻止某些工具或子系统功能实现的错误条件

#define MAINTENANCE_LOG_LEVEL_ERR	3	//阻止工具或某些子系统部分功能实现的错误条件

#define MAINTENANCE_LOG_LEVEL_WARNING	4	//预警信息

#define MAINTENANCE_LOG_LEVEL_NOTICE	5	//具有重要性的普通条件

#define MAINTENANCE_LOG_LEVEL_INFO	6	//提供信息的消息

#define MAINTENANCE_LOG_LEVEL_DEBUG	7	//不包含函数条件或问题的其他信息

#define MAINTENANCE_LOG_LEVEL_NONE	8	//没有重要级，通常用于排错

#endif // _LOG_LEVEL_MACRO_H_
