#ifndef _LOG_LEVEL_MACRO_H_
#define _LOG_LEVEL_MACRO_H_

/***********************************************��־������*********************************************************/
//ֵ	|����		|����								|�ı���洢ʱ�������Զ�̬���ã�		|�������־���鼶��
//0		|PANIC		|��ϵͳ������						|3��					|
//1		|ALERT		|��Ҫ�������޸ĵ�����				|2��					|
//2		|CRIT		|��ֹĳЩ���߻���ϵͳ����ʵ�ֵĴ���
//                  |����				                |1��					|
//3		|ERR		|��ֹ���߻�ĳЩ��ϵͳ���ֹ���ʵ�ֵ�
//                  |��������			                |6����					|�쳣��
//4		|WARNING	|Ԥ����Ϣ							|3����					|�߼�������
//5		|NOTICE		|������Ҫ�Ե���ͨ����				|2����					|��ͨ״̬��
//6		|INFO		|�ṩ��Ϣ����Ϣ						|1����					|Ĭ��
//7		|DEBUG		|���������������������������Ϣ		|15��					|
//8		|NONE		|û����Ҫ����ͨ�������Ŵ�			|1��					|

#define MAINTENANCE_LOG_LEVEL_PANIC	0	//��ϵͳ������

#define MAINTENANCE_LOG_LEVEL_ALERT	1	//��Ҫ�������޸ĵ�����

#define MAINTENANCE_LOG_LEVEL_CRIT	2	//��ֹĳЩ���߻���ϵͳ����ʵ�ֵĴ�������

#define MAINTENANCE_LOG_LEVEL_ERR	3	//��ֹ���߻�ĳЩ��ϵͳ���ֹ���ʵ�ֵĴ�������

#define MAINTENANCE_LOG_LEVEL_WARNING	4	//Ԥ����Ϣ

#define MAINTENANCE_LOG_LEVEL_NOTICE	5	//������Ҫ�Ե���ͨ����

#define MAINTENANCE_LOG_LEVEL_INFO	6	//�ṩ��Ϣ����Ϣ

#define MAINTENANCE_LOG_LEVEL_DEBUG	7	//���������������������������Ϣ

#define MAINTENANCE_LOG_LEVEL_NONE	8	//û����Ҫ����ͨ�������Ŵ�

#endif // _LOG_LEVEL_MACRO_H_
