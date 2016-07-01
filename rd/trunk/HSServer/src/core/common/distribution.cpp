#include "distribution.h"

#if _MSC_VER >= 1400 // VC++ 8.0
#pragma warning(disable: 4996)   // disable warning about security.
#endif

int getPlatidHash(const string& platid)
{
	int hash = 0, mid = 0;
	for(size_t i = 0; i < platid.length(); i += 4)
	{
		sscanf(platid.substr(i, 4).c_str(), "%x", &mid);
		hash += mid;
	}
	hash &= 0xffff;
	return hash;
}

int getUidHash(int64 uid)
{
	return (int)(uid >> 32) & 0xffff;
}
