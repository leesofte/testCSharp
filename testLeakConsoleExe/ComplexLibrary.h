#ifndef COMPLEXLIBRARY_H
#define COMPLEXLIBRARY_H
#include <iostream>
using namespace std;

enum NameEntityType
{
	PersonName = 0,
	PlaceName,
	OrganizationName
};

typedef struct _NAMEENTITY
{
	wchar_t* _name;
	NameEntityType _type;
	int _highlight_length;
	double _score;
}NAMEENTITY, *PNAMEENTITY;

typedef struct _MESSAGE
{
	string flag;
}MESSAGE, *PMESSAGE;
#endif