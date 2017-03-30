testCppDll 简单数学运算以及结构体操作的dll库(C++实现)
testUseDllByCxx C++调用C++(testCppDll)库的测试用例
testUseDllByCSharp C#调用C++(testCppDll)库的测试用例
testNUnitUseDll 集成C#的NUnit测试框架
testGTestCppDll 集成C++的GTest测试框架

以上三个工程的程序生成在target目录
编译后生成的dll和exe均在target目录，svn不上传dll和exe，请自行编译
设置testUseDllByCSharp为启动程序，入口函数为main.cs的main方法，根据需求测试相关内容
TestComplexDllMemUsage   结构体变量
	测试用例：1, C++库使用new申请内存，函数返回值方式返回内存，C#使用该内存并释放 2, C++库使用CoTaskMemAlloc申请内存，参数形式返回内存，C#使用该内存并释放)
TestComplexDllLoadLibrary 结构体数组变量
	测试用例：1，C#定义(out 结构体数组变量，in 结构体数组大小的uint变量初值为0， 参数形式传入C++库复杂分配结构体数组内存和修改数组大小uint变量，C#使用该内存并释放)
	使用dynamic loadlibrary
TestComplexDllStatic
	测试用例：1，C#定义(out 结构体数组变量，in 结构体数组大小的uint变量初值为0， 参数形式传入C++库复杂分配结构体数组内存和修改数组大小uint变量，C#使用该内存并释放)
	使用dllimport		
TestSimpleDllStatic
	简单用例
TestSimpleDllLoadLibrary
	简单用例

已完成
1，C#调用C++库，使用LoadLibrary和dllImport两种
2，集成C#的NUnitTest，C++的GTest框架
3，托管代码调用非托管代码用例
4，继续完善复杂数据类型的动态库调用