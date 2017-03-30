#pragma once
#include <iostream>
#include <windows.h>
#include <stdio.h>
#include <tchar.h>
using namespace std;

enum NameEntityType
{
	PersonName,
	PlaceName,
	OrganizationName
};

typedef struct _NAMEENTITY
{
	wchar_t* _name;
	NameEntityType _type;
	int _highlight_length;
	double _score;
}NAMEENTITY, *PNAMEENTITY;

/*
delegate bool CreateNameEntitysByType(string text,
	out IntPtr nameArray,
	ref uint arraySize,
	int type);
delegate bool CreateNameEntityByType(out IntPtr namePtr, int type);
*/

typedef double (*AddFunc)(double a, double b);
typedef bool (*InitFunc)(string resourcePath);
typedef bool (*CreateNameEntitysByType)(string text,PNAMEENTITY* nameArray,UINT* arraySize,int type);
typedef bool (*CreateNameEntityByType)(PNAMEENTITY* nameArray,int type);

VOID ErrorExit(LPSTR); 
void TestSmpleLoadLibrary();
void TestComplexLoadLibrary();