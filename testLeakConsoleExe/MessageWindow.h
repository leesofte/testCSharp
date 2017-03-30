#include <windows.h>
#include <stdio.h>
#include <tchar.h>

HWND CreateMessageWindow(WNDPROC lpfnWndProc);
LRESULT CALLBACK MsgWndProc(HWND hwnd,	UINT uMsg,	WPARAM wParam,	LPARAM lParam);
void TestSendMessage(HWND hDstWnd,HWND hSrcWnd);