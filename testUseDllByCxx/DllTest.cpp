#include "DllTest.h"

void TestSmpleLoadLibrary()
{
	HMODULE hDll = LoadLibrary(TEXT("testCppDll.dll"));
	if (hDll != NULL)
	{
		AddFunc add = (AddFunc)GetProcAddress(hDll, "Add");
		int err = GetLastError(); 
		if(add != NULL)
		{
			cout<<add(2.0, 3.0)<<endl;
		}
		FreeLibrary(hDll);
	}
}

void TestComplexLoadLibrary()
{
	HMODULE hDll = LoadLibrary(TEXT("testCppDll.dll"));
	if(hDll != NULL)
	{
		InitFunc initFunc = (InitFunc)GetProcAddress(hDll, "Initialize");
		int err   =   GetLastError(); 
		if (initFunc != NULL)
		{
			initFunc("");
		}

		string text = "";

		int type = 1;
		PNAMEENTITY nameArray = NULL;
		UINT size = 0;
		CreateNameEntitysByType createNameEntitysByTypeFunc = (CreateNameEntitysByType)GetProcAddress(hDll, "CreateNameEntitysByType");
		createNameEntitysByTypeFunc(text,&nameArray,&size,type);

		PNAMEENTITY nameEntityOut = NULL;
		CreateNameEntityByType createNameEntityByTypeFunc = (CreateNameEntityByType)GetProcAddress(hDll, "CreateNameEntityByType");
		createNameEntityByTypeFunc(&nameEntityOut,type);

		FreeLibrary(hDll);
	}
}

int _tmain(int argc, _TCHAR* argv[])
{
	TestComplexLoadLibrary();
	system("pause");
}

