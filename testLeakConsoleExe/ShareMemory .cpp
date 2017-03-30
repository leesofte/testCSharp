#include "ShareMemory.h"  
#include "ShareDefine.h"  

ShareMemory::ShareMemory()  
{  
	m_hReadEvent = NULL;
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
		CloseHandle(m_hReadEvent);  
		m_hReadEvent = NULL;  
	} 
}  

bool ShareMemory::OnReadMemory(OUT void* pszData,int size)  
{  
	WaitForSingleObject(m_hReadEvent, INFINITE);  

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

	PulseEvent(m_hReadEvent);  

	return true;  
}  

bool ShareMemory::OnReadInit(int size)
{
	m_hReadEvent = CreateEvent(NULL, FALSE, FALSE, GLOBAL_EVENT_IN_NAME);  
	if (NULL == m_hReadEvent)  
	{  
		return false;  
	}  
	m_hReadHandle = CreateFileMapping(  
		INVALID_HANDLE_VALUE,  
		NULL,  
		PAGE_READWRITE,  
		0,  
		size,  
		GLOBAL_MEMORY_IN_NAME);  
	if (NULL == m_hReadHandle)  
	{  
		return false;  
	}  


	return true;
}

bool ShareMemory::OnWriteInit(int size)
{
	m_hWriteEvent = CreateEvent(NULL, FALSE, FALSE, GLOBAL_EVENT_OUT_NAME);  
	if (NULL == m_hWriteEvent)  
	{  
		return false;  
	}  
	m_hWriteHandle = CreateFileMapping(  
		INVALID_HANDLE_VALUE,  
		NULL,  
		PAGE_READWRITE,  
		0,  
		size,  
		GLOBAL_MEMORY_OUT_NAME);  
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
	SetEvent(m_hWriteEvent);  

	return true;  
}  
