testCppDll ����ѧ�����Լ��ṹ�������dll��(C++ʵ��)
testUseDllByCxx C++����C++(testCppDll)��Ĳ�������
testUseDllByCSharp C#����C++(testCppDll)��Ĳ�������
testNUnitUseDll ����C#��NUnit���Կ��
testGTestCppDll ����C++��GTest���Կ��

�����������̵ĳ���������targetĿ¼
��������ɵ�dll��exe����targetĿ¼��svn���ϴ�dll��exe�������б���
����testUseDllByCSharpΪ����������ں���Ϊmain.cs��main������������������������
TestComplexDllMemUsage   �ṹ�����
	����������1, C++��ʹ��new�����ڴ棬��������ֵ��ʽ�����ڴ棬C#ʹ�ø��ڴ沢�ͷ� 2, C++��ʹ��CoTaskMemAlloc�����ڴ棬������ʽ�����ڴ棬C#ʹ�ø��ڴ沢�ͷ�)
TestComplexDllLoadLibrary �ṹ���������
	����������1��C#����(out �ṹ�����������in �ṹ�������С��uint������ֵΪ0�� ������ʽ����C++�⸴�ӷ���ṹ�������ڴ���޸������Сuint������C#ʹ�ø��ڴ沢�ͷ�)
	ʹ��dynamic loadlibrary
TestComplexDllStatic
	����������1��C#����(out �ṹ�����������in �ṹ�������С��uint������ֵΪ0�� ������ʽ����C++�⸴�ӷ���ṹ�������ڴ���޸������Сuint������C#ʹ�ø��ڴ沢�ͷ�)
	ʹ��dllimport		
TestSimpleDllStatic
	������
TestSimpleDllLoadLibrary
	������

�����
1��C#����C++�⣬ʹ��LoadLibrary��dllImport����
2������C#��NUnitTest��C++��GTest���
3���йܴ�����÷��йܴ�������
4���������Ƹ����������͵Ķ�̬�����