#pragma once
#include <windows.h>

#include <atlbase.h>
#include <atlstr.h>

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

class CNameEntityFinder
{
private:
	NameEntityType m_eNameEntityType;
public:
	CNameEntityFinder(int type);
	CNameEntityFinder(NameEntityType type);
	CNameEntityFinder();
	~CNameEntityFinder(void);
	bool Initialize(const wchar_t* resourcePath);
	void UnInitialize();
	bool CreateNameEntitys(const wchar_t* text, 
		 PNAMEENTITY* ppNameEntity,
		 UINT* nameCount);
	bool CreateNameEntity(PNAMEENTITY* ppNameEntity);
	/*bool CreateNameEntity(PNAMEENTITY pNameEntity);*/
	PNAMEENTITY CreateNameEntity( );

private:
	int PopulateResult(const wchar_t* text, PNAMEENTITY* ppNameEntity);
	void FillNameEntity(const wchar_t* name, NameEntityType type, double score, PNAMEENTITY entity);
};
