#include "ShareMemAPI.h"

#if WIN32
//#include <WinBase.h>
#include <Windows.h>
#else
#include <sys/types.h> 
#include <sys/ipc.h> 
#include <sys/shm.h> 
#include <errno.h>
#endif

namespace ShareMemAPI
{


SMHandle CreateShareMem(SM_KEY key,UINT Size)
{
	//__ENTER_FUNCTION
	//char keybuf[64];
	//memset(keybuf,0,64);
	//sprintf(keybuf,"./%d",key);
#if WIN32
	char keybuf[64];
	memset(keybuf,0,64);
	sprintf(keybuf,"%d",key);
	return  CreateFileMapping( (HANDLE)0xFFFFFFFFFFFFFFFF, NULL, PAGE_READWRITE, 0, Size, keybuf);
#else
	//key = ftok(keybuf,'w'); 
	SMHandle hd =  shmget(key ,Size,IPC_CREAT|IPC_EXCL|0666); 
	printf("handle = %d ,key = %d ,error: %d \r\n",hd,key,errno);
	return hd;

#endif

	//return SMHandle(-1);

}

SMHandle OpenShareMem(SM_KEY key,UINT Size)
{
	//__ENTER_FUNCTION
	//char keybuf[64];
	//memset(keybuf,0,64);
	//sprintf(keybuf,"./%d",key);
#if WIN32
	char keybuf[64];
	memset(keybuf,0,64);
	sprintf(keybuf,"%d",key);
	return OpenFileMapping( FILE_MAP_ALL_ACCESS, TRUE, keybuf);

#else 
	//key = ftok(keybuf,'w'); 
	SMHandle hd =   shmget(key , Size,0);
	printf("handle = %d ,key = %d ,error: %d \r\n",hd,key,errno);
	return hd;
#endif

		//return SMHandle(-1);
}

char* MapShareMem(SMHandle handle)
{
	//__ENTER_FUNCTION
#if WIN32
	return (char *)MapViewOfFile(handle, FILE_MAP_ALL_ACCESS, 0, 0, 0);
#else
	return  (char*)shmat(handle,0,0);
#endif

	//__LEAVE_FUNCTION
		return 0;

}


void UnMapShareMem(char* MemoryPtr)
{
	//__ENTER_FUNCTION
#if WIN32
	UnmapViewOfFile(MemoryPtr);
#else
	shmdt(MemoryPtr);
#endif
	//__LEAVE_FUNCTION
}


void CloseShareMem(SMHandle handle)
{
	//__ENTER_FUNCTION
#if WIN32
	CloseHandle(handle);
#else
	shmctl(handle,IPC_RMID,0); 
#endif
	//__LEAVE_FUNCTION
}

}