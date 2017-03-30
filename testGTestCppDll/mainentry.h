#pragma once
#ifndef MAINENTRY_H
#define MAINENTRY_H
#ifdef _DEBUG
#pragma comment(lib, "..\\gtest\\lib\\gtestd.lib")
#else
#pragma comment (lib, "..\\gtest\\lib\\gtest.lib")
#endif

#endif