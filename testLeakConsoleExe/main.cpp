#include "main.h"
using namespace std;

typedef double (*AddFunc)(double a, double b);
typedef bool (*InitFunc)(string resourcePath);
typedef bool (*CreateNameEntitysByType)(string text,PNAMEENTITY* nameArray,UINT* arraySize,int type);
typedef bool (*CreateNameEntityByType)(PNAMEENTITY* nameArray,int type);

int _tmain(int argc, _TCHAR* argv[])
{
	char *p = (char*) HeapAlloc(GetProcessHeap(), 0, 1024*1024*512); 
	ZeroMemory(p,1024*1024*256);
	
	//test MemoryMapReader/Writer
	//bool ret = CreateNameEntityByMemoryMapLib();
	
	//test sharememory
	CreateNameEntityServiceByShareMemory();

	//test MessageWindow
	/*
	SetConsoleTitle(_T("testLeakConsoleExe"));
	HWND hMsgWnd = CreateMessageWindow(MsgWndProc);
	_tprintf(_T("Message Window Handle = %d\n"),hMsgWnd);
	_tprintf(_T("Message Window(HANDLE=%d) Ready to receive Window Message...\n"),hMsgWnd);	
	//HWND hMsgDst = (HWND)1248692;//(HWND) FindWindow(0, _T("testNUnitUseDll"));  
	//TestSendMessage(hMsgDst,hMsgWnd);
	while(1)
	{
		MSG wd_msg;
		if(PeekMessageW(&wd_msg, NULL, 0, 0,PM_REMOVE)){

			if (wd_msg.message == WM_QUIT)
			{
				_bExit = false;
				break;
			}
			TranslateMessage(&wd_msg);
			DispatchMessage(&wd_msg);
		}
	}
	*/
	//system("pause");
	return 0;
}

void CreateNameEntity(void* paramPtr)
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
		/*
		PNAMEENTITY nameArray = NULL;
		UINT size = 0;
		CreateNameEntitysByType createNameEntitysByTypeFunc = (CreateNameEntitysByType)GetProcAddress(hDll, "CreateNameEntitysByType");
		createNameEntitysByTypeFunc(text,&nameArray,&size,type);
		*/
		PNAMEENTITY nameEntityOut = NULL;
		CreateNameEntityByType createNameEntityByTypeFunc = (CreateNameEntityByType)GetProcAddress(hDll, "CreateNameEntityByType");
		createNameEntityByTypeFunc((PNAMEENTITY*)paramPtr,type);

		FreeLibrary(hDll);
	}	 
}

bool CreateNameEntityServiceByShareMemory()
{
	PNAMEENTITY nameEntity = new NAMEENTITY();
	ShareMemory shareMemory;  

	if (!shareMemory.OnInit())  
	{  
		printf("Init Memory is error!\n");  
		return false;  
	}  
	PMESSAGE msg = new MESSAGE();
	
	if(!shareMemory.OnReadMemoryFirstStage(msg, sizeof(msg)))
	{
		printf("Read memory block is error.\n"); 
	}
	CreateNameEntity(nameEntity);
	if (!shareMemory.OnWriteMemory(nameEntity,sizeof(nameEntity)))  
	{  
		printf("write memory block is error.\n");  
	}
	if(!shareMemory.OnReadMemorySecondStage(msg, sizeof(msg)))
	{
		printf("Read memory block is error.\n"); 
	}
	return true;
}

bool CreateNameEntityByMemoryMapWriter()
{
	CMemoryMapWriter writer;
	TCHAR* pFile = _T("test.txt");

	if (false == writer.Open(pFile, false))
	{
		printf("open fail \n");
		return false;
	}
	PNAMEENTITY nameEntity = new NAMEENTITY();
	nameEntity->_type = PlaceName;
	writer.WriteData(nameEntity, sizeof(NAMEENTITY));
	writer.Close();
	return true;
	/*
	CMemoryMapReader reader;
	if(false == reader.Open(pFile))
	{
		return 1;
	}
	PNAMEENTITY pNameEntityOut = new NAMEENTITY();
	void* nameEntityOut = (void*)pNameEntityOut;
	reader.ReadData(nameEntityOut,sizeof(NAMEENTITY));
	PNAMEENTITY pNameEntity = (PNAMEENTITY)nameEntity;
	*/
	//reader.Close();
}