#pragma once

#include <stdio.h>
#include <string>
#include <vector>
#include <map>
#include <list>

using namespace std;
#ifdef	WIN32
#define SMHandle void *
#else
typedef	int SMHandle;
#endif

typedef int SM_KEY;
typedef unsigned int UINT;
typedef long long int64;
