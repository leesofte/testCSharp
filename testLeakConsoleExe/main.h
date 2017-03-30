#ifndef MAIN_H
#define MAIN_H
#pragma once
#include "MemoryMapWriter.h"
#include "MemoryMapReader.h"
#include "ComplexLibrary.h"
#include "MessageWindow.h"
#include "ShareMemory.h"
#include <windows.h>
#include <atlbase.h>
#include <atlstr.h>
#include <iostream>
#include <stdio.h>
#include <tchar.h>
//#include  <vld.h>

bool CreateNameEntityByMemoryMapWriter();
bool CreateNameEntityServiceByShareMemory();
void CreateNameEntity(void* paramPtr);

#endif