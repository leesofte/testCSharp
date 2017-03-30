#ifndef MATHLIBRARYTEST_H
#define MATHLIBRARYTEST_H

#include "NativeLib.h"


#ifdef _DEBUG
#pragma comment(lib, "..\\..\\gtest\\lib\\gtestd.lib")
#else
#pragma comment (lib, "..\\..\\gtest\\lib\\gtest.lib")
#endif

#endif