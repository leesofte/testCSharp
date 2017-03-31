// TestMessage.cpp : �������̨Ӧ�ó������ڵ㡣
//
#include "MessageWindow.h"
#include "ComplexLibrary.h"
HWND CreateMessageWindow(WNDPROC lpfnWndProc)
{
	TCHAR lpszClassName[256] = {0};
	_stprintf_s(lpszClassName,_T("%d_msg"),GetCurrentThreadId());
	HINSTANCE hinstance = ::GetModuleHandle(NULL);
	WNDCLASSEX wcex;
	wcex.cbSize			= sizeof(WNDCLASSEX);
	wcex.style			= 0;
	wcex.lpfnWndProc	= lpfnWndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hinstance;
	wcex.hIcon			= NULL;
	wcex.hCursor		= NULL;
	wcex.hbrBackground	= NULL;
	wcex.lpszMenuName	= NULL;
	wcex.lpszClassName	= lpszClassName;
	wcex.hIconSm		= NULL;

	if (!RegisterClassEx(&wcex)) 
	{
		return FALSE; 
	}
	return CreateWindowW( lpszClassName, _T(""), WS_POPUP, 0, 0, 0, 0, HWND_MESSAGE, NULL, hinstance, NULL);
}
void TestSendMessage(HWND hDstWnd,HWND hSrcWnd)
{
	// TODO: �ڴ���ӿؼ�֪ͨ����������
	COPYDATASTRUCT data;
	data.dwData = 0;

	PNAMEENTITY nameEntity = new NAMEENTITY();
	data.lpData = (void*)nameEntity;
	data.cbData = sizeof((void*)nameEntity);

	::SendMessage(hDstWnd,WM_COPYDATA,(WPARAM)hSrcWnd,(LPARAM)&data);
}
LRESULT CALLBACK MsgWndProc(HWND hwnd,	UINT uMsg,	WPARAM wParam,	LPARAM lParam)
{
	COPYDATASTRUCT *pCopyDataStruct=(COPYDATASTRUCT*) lParam;
	LPTSTR lpData = (LPTSTR)malloc( pCopyDataStruct->cbData+2);
	switch (uMsg) 
	{
	case WM_COPYDATA:
		{
		HWND hMsgDst =(HWND) FindWindow(0, _T("testNUnitUseDll"));  
		memcpy(lpData,pCopyDataStruct->lpData,pCopyDataStruct->cbData);
		memset((LPBYTE)lpData+pCopyDataStruct->cbData,0,2);//ĩβ��ֵ���ַ�

		_tprintf(_T("pCopyDataStruct->dwData = %d\npCopyDataStruct->lpData = %s\n"),pCopyDataStruct->dwData,lpData);
		if(true/*test pCopyDataStruct->lpData is invoke"*/)
		{
			TestSendMessage(hMsgDst,hwnd);
		}
		break;
		}
	default:
		break;
	}
	free(lpData);
	return TRUE;
}


