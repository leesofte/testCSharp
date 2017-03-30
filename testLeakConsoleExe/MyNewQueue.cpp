#include <system.dll>
#using <system.messaging.dll>

using namespace System;
using namespace System::Messaging;

// This class represents an object the following example 
// sends to a queue and receives from a queue.
ref class Order
{
public:
	int orderId;
	DateTime orderTime;
};