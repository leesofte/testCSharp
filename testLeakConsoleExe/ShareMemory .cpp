#include "ShareMemory.h"  
#include "ShareDefine.h"  

ShareMemory::ShareMemory()  
{  
	//WaitForMultipleObjects(2,m_hWriteEvent,true,INFINITE);
	m_hReadEvent[0] = NULL;
	m_hReadEvent[1] = NULL;
	m_hWriteEvent = NULL;
	m_hWriteEvent = NULL;
	m_hReadHandle = NULL;
	m_hWriteHandle = NULL;  
}  

ShareMemory::~ShareMemory()  
{  
	if (NULL != m_hWriteHandle)  
	{  
		CloseHandle(m_hWriteHandle);  
		m_hWriteHandle = NULL;  
		CloseHandle(m_hWriteEvent);  
		m_hWriteEvent = NULL;  
	}  
	if (NULL != m_hReadHandle)  
	{  
		CloseHandle(m_hReadHandle);  
		m_hReadHandle = NULL;  
		CloseHandle(m_hReadEvent[0]);  
		m_hReadEvent[0] = NULL; 
		CloseHandle(m_hReadEvent[1]);  
		m_hReadEvent[1] = NULL;  
	} 
}  

bool ShareMemory::OnReadMemorySecondStage(OUT void* pszData,int size)  
{
	WaitForSingleObject(m_hReadEvent[1], INFINITE);  
	if (NULL == pszData)  
	{  
		return false;  
	}  

	if (NULL == m_hReadHandle)  
	{  
		return false;  
	}  

	void* pBuffer = NULL;  

	pBuffer = (void*)MapViewOfFile(  
		m_hReadHandle,  
		FILE_MAP_READ,  
		0,  
		0,  
		size);  
	if (NULL == pBuffer)  
	{  
		return false;  
	}  

	__try  
	{  
		CopyMemory((void*)pszData, pBuffer, sizeof(pBuffer));  
	}  
	__except(EXCEPTION_EXECUTE_HANDLER)  
	{  
		UnmapViewOfFile(pBuffer);  
		return false;  
	}  

	UnmapViewOfFile(pBuffer);  

	ReleaseSemaphore(m_hReadEvent[1],1,NULL);  

	return true;  
}
bool ShareMemory::OnReadMemoryFirstStage(OUT void* pszData,int size)  
{  	
	WaitForSingleObject(m_hReadEvent[0], INFINITE);  

	if (NULL == pszData)  
	{  
		return false;  
	}  

	if (NULL == m_hReadHandle)  
	{  
		return false;  
	}  

	void* pBuffer = NULL;  

	pBuffer = (void*)MapViewOfFile(  
		m_hReadHandle,  
		FILE_MAP_READ,  
		0,  
		0,  
		size);  
	if (NULL == pBuffer)  
	{  
		return false;  
	}  

	__try  
	{  
		CopyMemory((void*)pszData, pBuffer, sizeof(pBuffer));  
	}  
	__except(EXCEPTION_EXECUTE_HANDLER)  
	{  
		UnmapViewOfFile(pBuffer);  
		return false;  
	}  

	UnmapViewOfFile(pBuffer);  

	ReleaseSemaphore(m_hReadEvent[0],1,NULL);  

	return true;  
}  

bool ShareMemory::OnReadInit(int size)
{
	m_hReadEvent[0] = OpenSemaphore(SEMAPHORE_ALL_ACCESS, true,  GLOBAL_EVENT_IN1_NAME); 
	m_hReadEvent[1] = OpenSemaphore(SEMAPHORE_ALL_ACCESS, true,  GLOBAL_EVENT_IN2_NAME); 
	if (NULL == m_hReadEvent)  
	{  
		return false;  
	}  
	m_hReadHandle = OpenFileMapping(  
		FILE_MAP_ALL_ACCESS,  
		FALSE,  
		GLOBAL_MEMORY_IN_NAME);
	/*
	m_hReadHandle = CreateFileMapping(  
		INVALID_HANDLE_VALUE,  
		NULL,  
		PAGE_READWRITE,  
		0,  
		size,  
		GLOBAL_MEMORY_IN_NAME); */ 
	if (NULL == m_hReadHandle)  
	{  
		return false;  
	}  


	return true;
}

bool ShareMemory::OnWriteInit(int size)
{
	m_hWriteEvent = OpenSemaphore(SEMAPHORE_ALL_ACCESS, true,  GLOBAL_EVENT_OUT_NAME);  
	if (NULL == m_hWriteEvent)  
	{  
		return false;  
	}  
	m_hWriteHandle = OpenFileMapping(  
		FILE_MAP_ALL_ACCESS,  
		FALSE,  
		GLOBAL_MEMORY_OUT_NAME);  
	/*
	m_hWriteHandle = CreateFileMapping(  
		INVALID_HANDLE_VALUE,  
		NULL,  
		PAGE_READWRITE,  
		0,  
		size,  
		GLOBAL_MEMORY_OUT_NAME);  */
	if (NULL == m_hWriteHandle)  
	{  
		return false;  
	}  


	return true;
}
bool ShareMemory::OnInit()  
{  
	PNAMEENTITY nameEntity = new NAMEENTITY();
	OnWriteInit(sizeof(nameEntity));
	PMESSAGE msg = new MESSAGE();
	OnReadInit(sizeof(msg));
	return true;
}  

bool ShareMemory::OnWriteMemory(IN void* pszData,int size)  
{  
	if (NULL == pszData)  
	{  
		return false;  
	}  

	void* pBuffer = NULL;  

	if (NULL == m_hWriteHandle)  
	{  
		return false;  
	}  

	pBuffer = (void*)MapViewOfFile(  
		m_hWriteHandle,  
		FILE_MAP_WRITE,  
		0,  
		0,  
		size); 

	if (NULL == pBuffer)  
	{  
		return false;  
	}  

	__try  
	{  
		ZeroMemory(pBuffer, sizeof(pBuffer));  
		CopyMemory((void*)pBuffer, pszData, sizeof(pszData));  
	}  
	__except(EXCEPTION_EXECUTE_HANDLER)  
	{  
		UnmapViewOfFile(pBuffer);  
		return false;  
	}  

	UnmapViewOfFile(pBuffer);  
	ReleaseSemaphore(m_hWriteEvent,1,NULL);  

	return true;  
}  
