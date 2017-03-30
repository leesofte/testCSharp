
#include <stdio.h>
#include <Windows.h>
#include "MemoryMapWriter.h"
/*
int _tmain(int argc, _TCHAR* argv[])
{
	CMemoryMapWriter writer;
	TCHAR* pFile = _T("test.txt");

	if (false == writer.Open(pFile, false))
	{
		printf("open fail \n");
		return 1;
	}

	char* data = "a";
	for (int i=0; i < 64*1024 ;++i)
	{
		writer.WriteData(data, strlen(data));
	}

	writer.Close();

	return 0;
}

*/