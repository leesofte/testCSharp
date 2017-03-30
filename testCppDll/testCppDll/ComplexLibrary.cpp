#include "ComplexLibrary.h"
#include <windows.h>
#include <atlbase.h>
#include <atlstr.h>

CNameEntityFinder::CNameEntityFinder(NameEntityType type)
{
	m_eNameEntityType = type;
	wprintf(L"*Native*: NameEntity finder object is created.\n");
}

CNameEntityFinder::CNameEntityFinder(int type)
{
	NameEntityType _type = PersonName;
	switch(type)
	{
	case 0:
		_type = PersonName; 
		break;
	case 1: 
		_type = PlaceName;
		break;
	case 2:
		_type = OrganizationName;
		break;
	default:
		break;
	}
	m_eNameEntityType = _type;
	wprintf(L"*Native*: NameEntity finder object is created.\n");
}

CNameEntityFinder::CNameEntityFinder()
{
	m_eNameEntityType = PersonName;
	wprintf(L"*Native*: NameEntity finder object is created.\n");
}

CNameEntityFinder::~CNameEntityFinder(void)
{
	wprintf(L"*Native*: NameEntity finder object is deleted.\n");
}

bool CNameEntityFinder::Initialize(const wchar_t* resourcePath)
{
	wprintf(L"*Native*: NameEntity finder object is initialized.\n");
	return true;
}

void CNameEntityFinder::UnInitialize()
{
	wprintf(L"*Native*: NameEntity finder object is uninitialized.\n");
}

PNAMEENTITY CNameEntityFinder::CreateNameEntity()
{
	wprintf(L"*Native*: NameEntity finder object is processing text.\n");
	PNAMEENTITY ppNameEntity = (PNAMEENTITY)CoTaskMemAlloc(sizeof(NAMEENTITY));
	memset(ppNameEntity, 0, sizeof(NAMEENTITY));
	FillNameEntity(L"北京", PlaceName, 0.8, ppNameEntity);
	return ppNameEntity;
}

bool CNameEntityFinder::CreateNameEntity(PNAMEENTITY* ppNameEntity)
{
	if( NULL != ppNameEntity)
	{
		*ppNameEntity = (PNAMEENTITY)CoTaskMemAlloc(
			sizeof(NAMEENTITY));
		(*ppNameEntity)->_name = (wchar_t*)CoTaskMemAlloc(255 * 2);
		wcscpy_s((*ppNameEntity)->_name, 255, L"linpeiwen");
		(*ppNameEntity)->_highlight_length = 4;
		(*ppNameEntity)->_score = 3.0;
		(*ppNameEntity)->_type = PersonName;
	}
	return true;
}
/*
bool CNameEntityFinder::CreateNameEntity(PNAMEENTITY pNameEntity)
{
	if( NULL != pNameEntity)
	{
		pNameEntity = (PNAMEENTITY)CoTaskMemAlloc(
			sizeof(NAMEENTITY));
		(pNameEntity)->_name = (wchar_t*)CoTaskMemAlloc(255 * 2);
		wcscpy_s((pNameEntity)->_name, 255, L"linpeiwen");
		(pNameEntity)->_highlight_length = 4;
		(pNameEntity)->_score = 3.0;
		(pNameEntity)->_type = PersonName;
	}
	return true;
}*/
//Struct Array
bool CNameEntityFinder::CreateNameEntitys(const wchar_t* text, PNAMEENTITY* ppNameEntity, UINT* nameCount)
{
	wprintf(L"*Native*: NameEntity finder object is processing text.\n");
	*nameCount = PopulateResult(text, ppNameEntity);
	return true;
}
//CoTaskMemAlloc, auto free
//(value)
//or
//(ref)
//IntPtr strOutIntPtr = IntPtr.Zero;  
//Marshal.PtrToStringUni(strOutIntPtr);  
//Marshal.FreeCoTaskMem(strOutIntPtr);  
int CNameEntityFinder::PopulateResult(const wchar_t* text, PNAMEENTITY* ppNameEntity)
{
	int nameCount = 4;
	*ppNameEntity = (PNAMEENTITY)CoTaskMemAlloc(nameCount * sizeof(NAMEENTITY));
	memset(*ppNameEntity, 0, nameCount * sizeof(NAMEENTITY));
	PNAMEENTITY pCurNameEntity = *ppNameEntity;

	FillNameEntity(L"北京", PlaceName, 0.8, pCurNameEntity);
	pCurNameEntity++;
	FillNameEntity(L"洪小文", PersonName, 0.8, pCurNameEntity);
	pCurNameEntity++;
	FillNameEntity(L"王坚", PersonName, 0.78, pCurNameEntity);
	pCurNameEntity++;
	FillNameEntity(L"郭百宁", PersonName, 0.78, pCurNameEntity);
	return nameCount;
}

void CNameEntityFinder::FillNameEntity(const wchar_t* name, NameEntityType type, double score, PNAMEENTITY pEntity)
{
	if(NULL != pEntity)
	{
		pEntity->_highlight_length = (int)wcslen(name);
		pEntity->_name = (WCHAR*)CoTaskMemAlloc((pEntity->_highlight_length + 1) * sizeof(WCHAR));
		wcscpy_s(pEntity->_name, (pEntity->_highlight_length + 1), name);
		pEntity->_type = type;
		pEntity->_score = score;
	}
}

