#include <Windows.h>  
#include <tchar.h>  
#pragma once  
#include "ComplexLibrary.h"
class ShareMemory  
{  
public:  
	ShareMemory();  
	~ShareMemory();  

public:  
	bool OnWriteInit(int size);  
	bool OnReadInit(int size);
	bool OnInit();
	bool OnWriteMemory(IN void* pszData,int size);  
	bool OnReadMemory(OUT void* pszData,int size);  		

private:
	HANDLE m_hWriteEvent;  
	HANDLE m_hReadEvent;
	HANDLE m_hWriteHandle;  
	HANDLE m_hReadHandle;
};  