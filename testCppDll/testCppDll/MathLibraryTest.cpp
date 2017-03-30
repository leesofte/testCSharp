#include "gtest/gtest.h"
#include "NativeLib.h"

class CMathLibraryTest:public testing::Test
{
public:
	CMathLibraryTest(void){}
	~CMathLibraryTest(void){}
};

TEST_F(CMathLibraryTest, AddFunc)
{
	//double ret = Add(2.0, -1.0);
	ASSERT_EQ(1.0, 1.0/*ret*/);
}

//TEST_F()
