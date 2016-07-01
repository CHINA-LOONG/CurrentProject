#ifndef _SHAREMEM_API_H_
#define _SHAREMEM_API_H_
#include "ShareMemDef.h"


#ifdef	WIN32
#define INVALID_SM_HANDLE	 ((void*)0)
#else
#define INVALID_SM_HANDLE	 -1
#endif



namespace ShareMemAPI
{

	/*����ShareMem �ڴ���
	 *	
	 *	key   ����ShareMem �Ĺؼ�ֵ
	 *
	 *  Size  ������С
	 *
	 *	���� ��ӦShareMem����ֵ
	 */
	SMHandle		CreateShareMem(SM_KEY key,UINT Size);
	/*��ShareMem �ڴ���
	 *	
	 * key   ��ShareMem �Ĺؼ�ֵ
	 * 
	 * Size  �򿪴�С
	 *
	 * ����  ��ӦShareMem ����ֵ
	 */
	SMHandle		OpenShareMem(SM_KEY key,UINT Size);
	
	/*ӳ��ShareMem �ڴ���
	 *	
	 *	handle ӳ��ShareMem �ı���ֵ
	 *
	 *  ���� ShareMem ������ָ��
	 */
	char*			MapShareMem(SMHandle handle);
	
	/*�ر�ӳ�� ShareMem �ڴ���
	 *
	 *	MemoryPtr			ShareMem ������ָ��
	 *	
	 *  
	 */	
	void			UnMapShareMem(char* MemoryPtr);
	
	/*	�ر�ShareMem
	 * 	handle  ��Ҫ�رյ�ShareMem ����ֵ
	 */
	void			CloseShareMem(SMHandle handle);


}


#endif

