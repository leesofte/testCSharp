#ifndef NATIVELIB_H
#define NATIVELIB_H
#pragma once
#ifdef NATIVELIB_EXPORTS
#define NATIVELIB_API _declspec(dllexport)
#else 
#define NATIVELIB_API _declspec(dllimport)
#endif

#include <windows.h>
#include<vector>
#include <iostream>
#include <atlbase.h>
#include <atlstr.h>

#include "ComplexLibrary.h"
#include "MathLibrary.h"

using namespace std;

// �ۺ�ʾ��
DECLARE_HANDLE(NAMEFINDERHANDLE);
//used to test TLS 
DWORD dwTlsIndex; 
//used to test global Memory Alloc and Free
PBYTE pNewBuffer = NULL;
extern "C"
{
	// �������󣬲�����һ�����
	NATIVELIB_API NAMEFINDERHANDLE CreateNameFinderInstance(__in NameEntityType type);
	// ��ʼ�����󣬱���һЩģ�ͺ�����ʱ����������
	NATIVELIB_API bool Initialize(NAMEFINDERHANDLE hHandle,  __in_z const wchar_t* resourcePath);

	// ���ݸ�����һ�����֣��������а����ĸ�������
	NATIVELIB_API bool CreateNameEntitys(
		NAMEFINDERHANDLE hHandle,
		__in_z const wchar_t* text, 
		__out PNAMEENTITY* nameArray,
		__in UINT* arraySize);

	// �ͷŶ�����Դ
	NATIVELIB_API void UnInitialize(__in NAMEFINDERHANDLE hHandle);
	// ���ٶ���
	NATIVELIB_API void DestroyInstance(__in NAMEFINDERHANDLE hHandle);
	NATIVELIB_API bool CreateNameEntity(NAMEFINDERHANDLE hHandle, PNAMEENTITY* namePtr);
	//NATIVELIB_API bool CreateNameEntityByRef(NAMEFINDERHANDLE hHandle, PNAMEENTITY namePtr);
	NATIVELIB_API PNAMEENTITY CreateNameEntityByReturn(NAMEFINDERHANDLE hHandle);
}

extern "C"
{
	//without hHandle,for loadlibrary
	NATIVELIB_API bool CreateNameEntityByType(PNAMEENTITY* namePtr,int type = 0);
	NATIVELIB_API PNAMEENTITY CreateNameEntityByTypeByReturn(int type = 0);
	NATIVELIB_API bool CreateNameEntitysByType(		
		__in_z const wchar_t* text, 
		__out PNAMEENTITY* nameArray,
		__in UINT* arraySize,
		int type);
};

//Test Math Simple
extern "C"
{
	NATIVELIB_API double  Add(double a, double b);
	NATIVELIB_API double  Multiply(double a, double b);
	NATIVELIB_API double  AddMultiply(double a, double b);
}

//Test Memory Usage
extern "C"
{
	NATIVELIB_API void TestReturnStructFromArg(PNAMEENTITY* ppStruct);
	NATIVELIB_API PNAMEENTITY TestReturnNewStruct(void);
	NATIVELIB_API void FreeStruct(PNAMEENTITY pStruct);
};

//Test TLS
extern "C"
{
	NATIVELIB_API BOOL SetNAMEENTITYData();
	NATIVELIB_API BOOL GetNAMEENTITYData(PNAMEENTITY pdw);
};

void MemVirtual();
#endif