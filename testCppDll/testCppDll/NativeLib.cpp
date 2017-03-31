#include "NativeLib.h"

#include "objbase.h"    //CoTaskMemAlloc
#include <stdio.h>      //sprintf

//returnType
NAMEFINDERHANDLE CreateNameFinderInstance(__in NameEntityType type)
{
	CNameEntityFinder* finderObject = new(std::nothrow) CNameEntityFinder(type);
	return reinterpret_cast<NAMEFINDERHANDLE>(finderObject);
}

bool Initialize(NAMEFINDERHANDLE hHandle, const wchar_t* resourcePath)
{
	//mem link 
	char *p = (char*) HeapAlloc(GetProcessHeap(), 0, 1024*1024*256); 
	ZeroMemory(p,1024*1024*256);
	CNameEntityFinder* finderObject = reinterpret_cast<CNameEntityFinder*>(hHandle);
	if(NULL != hHandle)
	{
		return finderObject->Initialize(resourcePath);
	}
	else
	{
		return false;
	}
}

bool CreateNameEntitys(NAMEFINDERHANDLE hHandle,
	__in_z const wchar_t* text, 
	__out PNAMEENTITY* nameArray,
	__in UINT* arraySize)
{
	CNameEntityFinder* finderObject = reinterpret_cast<CNameEntityFinder*>(hHandle);	
	if(NULL != finderObject)
	{
		return finderObject->CreateNameEntitys(text, nameArray, arraySize);
	}
	else
	{
		return false;
	}
}

//with handle
PNAMEENTITY CreateNameEntityByReturn(NAMEFINDERHANDLE hHandle)
{
	CNameEntityFinder* finderObject = reinterpret_cast<CNameEntityFinder*>(hHandle);	
	if(NULL != finderObject)
	{
		PNAMEENTITY ret = finderObject->CreateNameEntity();
		return ret;
	}
	else
	{
		return NULL;
	}

}
bool CreateNameEntity(NAMEFINDERHANDLE hHandle,PNAMEENTITY* name)
{
	CNameEntityFinder* finderObject = reinterpret_cast<CNameEntityFinder*>(hHandle);	
	if(NULL != finderObject)
	{
		bool ret = finderObject->CreateNameEntity(name);
		return ret;
	}
	else
	{
		return false;
	}
}
/*
bool CreateNameEntityByRef(NAMEFINDERHANDLE hHandle, PNAMEENTITY namePtr)
{
	CNameEntityFinder* finderObject = reinterpret_cast<CNameEntityFinder*>(hHandle);	
	if(NULL != finderObject)
	{
		bool ret = finderObject->CreateNameEntity(namePtr);
		return ret;
	}
	else
	{
		return false;
	}
}
*/
//without handle
PNAMEENTITY CreateNameEntityByTypeByReturn(int type)
{
	CNameEntityFinder* finderObject = new CNameEntityFinder(type);
	if(NULL != finderObject)
	{
		PNAMEENTITY ret = finderObject->CreateNameEntity();
		delete finderObject;
		return ret;
	}
	else
	{
		return NULL;
	}
	
}
bool CreateNameEntityByType(PNAMEENTITY* name,int type)
{
	//mem link
	char *p = (char*) HeapAlloc(GetProcessHeap(), 0, 1024*1024*256); 
	ZeroMemory(p,1024*1024*256);
	CNameEntityFinder* finderObject = new CNameEntityFinder(type);
	if(NULL != finderObject)
	{
		bool ret = finderObject->CreateNameEntity(name);
		delete finderObject;
		return ret;
	}
	else
	{
		return false;
	}
}

bool CreateNameEntitysByType(
	__in_z const wchar_t* text, 
	__out PNAMEENTITY* nameArray,
	__in UINT* arraySize,
	int type)
{
	//mem link
	char *p = (char*) HeapAlloc(GetProcessHeap(), 0, 1024*1024*256); 
	ZeroMemory(p,1024*1024*256);
	CNameEntityFinder* finderObject = new CNameEntityFinder(type);
	if(NULL != finderObject)
	{
		bool ret = finderObject->CreateNameEntitys(text, nameArray, arraySize);
		delete finderObject;
		return ret;
	}
	else
	{
		return false;
	}
}
////
void UnInitialize(NAMEFINDERHANDLE hHandle)
{
	CNameEntityFinder* finderObject = reinterpret_cast<CNameEntityFinder*>(hHandle);
	if(NULL != finderObject)
	{
		finderObject->UnInitialize();
	}
}

void DestroyInstance(NAMEFINDERHANDLE hHandle)
{
	CNameEntityFinder* finderObject = reinterpret_cast<CNameEntityFinder*>(hHandle);	
	if(NULL != finderObject)
	{
		delete finderObject;
		finderObject = NULL;
	}
}
/////////////////////////////////////////////////////////
double  Add(double a, double b)
{
	return Functions::Add(a,b);
}
double  Multiply(double a, double b)
{
	return Functions::Multiply(a,b);
}
double  AddMultiply(double a, double b)
{
	return Functions::AddMultiply(a,b);
}
///////////////////////////////////////////////////////////
void TestReturnStructFromArg(PNAMEENTITY* ppStruct)
{
	if( NULL != ppStruct)
	{
		*ppStruct = (PNAMEENTITY)CoTaskMemAlloc(
			sizeof(NAMEENTITY));
		(*ppStruct)->_name = (wchar_t*)CoTaskMemAlloc(255 * 2);
		wcscpy_s((*ppStruct)->_name, 255, L"linpeiwen");
		(*ppStruct)->_highlight_length = 4;
		(*ppStruct)->_score = 3.0;
		(*ppStruct)->_type = PersonName;
	}
	return;
}

PNAMEENTITY TestReturnNewStruct(void)
{
	// 使用new分配内存
	PNAMEENTITY pStruct = new NAMEENTITY();
	pStruct->_name = (wchar_t*)CoTaskMemAlloc(255 * 2);
	wcscpy_s(pStruct->_name, 255, L"linpeiwen");
	pStruct->_highlight_length = 4;
	pStruct->_score = 3.0;
	pStruct->_type = PersonName;

	return pStruct;
}

void FreeStruct(PNAMEENTITY pStruct)
{
	if(NULL != pStruct)
	{
		delete pStruct;
		pStruct = NULL;
	}
}
////////////////////////////////////////////////////////////
//TLS 
BOOL SetNAMEENTITYData()
{
	LPVOID lpvData; 
	PNAMEENTITY pData = NULL;  // The stored memory pointer 

	lpvData = TlsGetValue(dwTlsIndex); 
	if (lpvData == NULL)
	{		
		pData = CreateNameEntityByTypeByReturn();
		lpvData = (LPVOID)pData ; 
		if (lpvData == NULL) 
			return FALSE;
		if (!TlsSetValue(dwTlsIndex, lpvData))
			return FALSE;
	}

	return TRUE;
}

BOOL GetNAMEENTITYData(PNAMEENTITY pdw)
{
	LPVOID lpvData; 
	PNAMEENTITY pData;  

	lpvData = TlsGetValue(dwTlsIndex); 
	if (lpvData == NULL)
		return FALSE;

	pData = (PNAMEENTITY) lpvData;
	(*pdw) = (*pData);
	return TRUE;
}

void MemVirtual(void) {
     //分配新内存大小。
     UINT nNewSize = (UINT) (1024*1024*256);
     pNewBuffer = (PBYTE) VirtualAlloc(NULL,nNewSize,MEM_COMMIT,PAGE_READWRITE);
     if (pNewBuffer){
         ZeroMemory(pNewBuffer,nNewSize);
         memcpy(pNewBuffer,_T("分配虚拟内存成功\r\n"),sizeof(_T("分配虚拟内存成功\r\n")));
         OutputDebugString((LPWSTR)pNewBuffer);
         //MEM_RELEASE
         //VirtualFree(pNewBuffer,0,MEM_RELEASE);
     }
}
///////////////////////////////////////////////////////////
BOOL DllMain(HINSTANCE hinstDLL,			// DLL module handle
		DWORD fdwReason,                    // reason called
		LPVOID lpvReserved)                 // reserved
{ 
	LPVOID lpvData; 
	BOOL fIgnore; 
	switch (fdwReason) 
	{ 
		// initialization or a call to LoadLibrary. 
	case DLL_PROCESS_ATTACH: 
		// Allocate a TLS index.
		MemVirtual();
		DisableThreadLibraryCalls(hinstDLL);
		printf("DLL_PROCESS_ATTACH\n");
		if ((dwTlsIndex = TlsAlloc()) == TLS_OUT_OF_INDEXES) 
			return FALSE; 
		// The attached process creates a new thread. 
		break;

	case DLL_THREAD_ATTACH: 
		// Initialize the TLS index for this thread.
		/*
		lpvData = (LPVOID) LocalAlloc(LPTR, 256); 
		if (lpvData != NULL) 
			fIgnore = TlsSetValue(dwTlsIndex, lpvData); 
		*/
		break; 

	case DLL_THREAD_DETACH: 
		// Release the allocated memory for this thread.
		lpvData = TlsGetValue(dwTlsIndex); 
		if (lpvData != NULL) 
			LocalFree((HLOCAL) lpvData); 
		break; 

	case DLL_PROCESS_DETACH: 
		// Release the allocated memory 
		VirtualFree(pNewBuffer,0,MEM_RELEASE);
		printf("DLL_PROCESS_DETACH\n");
		lpvData = TlsGetValue(dwTlsIndex); 
		if (lpvData != NULL) 
			LocalFree((HLOCAL) lpvData); 
		// Release the TLS index.
		TlsFree(dwTlsIndex); 
		break; 

	default: 
		break; 
	} 

	return TRUE; 
	UNREFERENCED_PARAMETER(hinstDLL); 
	UNREFERENCED_PARAMETER(lpvReserved); 
}