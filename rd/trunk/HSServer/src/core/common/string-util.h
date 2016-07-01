#pragma once

#include <vector>
#include <sstream>
#include "charwrap.h"
#include "const_def.h"
#include <time.h>
using namespace std;

bool gbk_to_utf8(const string& input, string &output);

bool safe_atoi(const char* str, int &num);
bool safe_atoi(const string &str, int &num);
bool safe_atoi(const string &str, time_t &num);
#ifndef _WIN32
bool safe_atoi(const string &str, size_t &num);
#endif
bool safe_atoi(const string &str, unsigned int &num);
bool safe_atoi(const string &str, unsigned char &num);
bool safe_atoll(const char* str, long long &num);
bool safe_atoll(const string &str, long long &num);
bool safe_atof(const string &str, float &num);
bool tokenize(const string& str, vector<string>& tokens, string &delims);
bool tokenize_as_int(const string& str, vector<int>& tokens, const string &delims);
bool tokenize_as_longlong(const string& str, vector<long long>& tokens, const string &delims);

bool StringToBool(const string& str);
std::string base64_encode(std::string const& input_str);
std::string base64_decode(std::string const& encoded_string);

static stringstream ssm;
static stringstream ssm1;
static stringstream ssm2;
static stringstream ssm3;
static const int g_nThreadSizeMax = 16;
static int g_nSsmRound = 0;
static stringstream g_pSsmSet[g_nThreadSizeMax * 2];

template<class T>
string toString(T data)
{
    stringstream& tssm = g_pSsmSet[g_nSsmRound++] ;
    if (g_nSsmRound >= g_nThreadSizeMax)
    {
        g_nSsmRound = 0;
    }
    tssm.clear();
    tssm.str("");
    tssm << data;
    return tssm.str();
}

template<class T>
string toStringEx(T data)
{
    ssm1.clear();
    ssm1.str("");
    ssm1 << data;
    return ssm1.str();
}

template<class T>
string toStringEx2(T data)
{
    ssm2.clear();
    ssm2.str("");
    ssm2 << data;
    return ssm2.str();
}

template<class T>
string toStringEx3(T data)
{
    ssm3.clear();
    ssm3.str("");
    ssm3 << data;
    return ssm3.str();
}

template<class T>
string toString(vector<T> &data)
{
    //    static stringstream ssm;
    ssm.clear();
    ssm.str("");
    for (int i = 0; i<int(data.size()) - 1; i++) ssm << data[i] << ",";
    if (data.size() > 0) ssm << *data.rbegin();
    return ssm.str();
}

template<class T>
string toString(int len, T* data)
{
    //    static stringstream ssm;
    ssm.clear();
    ssm.str("");
    for (int i = 0; i < len - 1; i++) ssm << data[i] << ",";
    if (len > 0) ssm << data[len - 1];
    return ssm.str();
}

string TimeToString(int64 llSecond);

template<class T>
bool AddValueToString(std::string& dust, const std::string& name, T data)
{
    std::string value = toStringEx<T > (data);
    if (dust.size() > 0)
    {
        dust.append("&");
    }
    dust.append(name);
    dust.append("=");
    dust.append(value);
    return true;
}

template<class T>
bool AddValueToStringEx(std::string& dust, const std::string& name, T data)
{
    std::string value = toStringEx2<T > (data);
    if (dust.size() > 0)
    {
        dust.append("&");
    }
    dust.append(name);
    dust.append("=");
    dust.append(value);
    return true;
}

template<class T>
bool AddValueToStringEx2(std::string& dust, const std::string& name, T data)
{
    std::string value = toStringEx3<T > (data);
    if (dust.size() > 0)
    {
        dust.append("&");
    }
    dust.append(name);
    dust.append("=");
    dust.append(value);
    return true;
}
long long StringToUid(const std::string& str);
int PlatId2Int(const std::string& plat_id);
time_t tm_val_to_time_t(int year, int month, int day, int hour, int minute, int second);
int64 GetTimeVal(const std::string& date);
void GatewayName(const std::string& s, const std::string& a, std::string& out);
int String2Int(const std::string& s);

template<class T>
bool
GetPtrArray(T** ppList, int nOldSize, int nSize)
{
    bool bSucc = true;
    static const char cDefaultValue = 0;
    if (nOldSize < nSize && ppList != NULL)
    {
        T* pOldList = *ppList;

        T* pListNew = (T*) malloc(sizeof (T) * nSize);
        memset(pListNew, cDefaultValue , sizeof (T) * nSize);
        if (pOldList != NULL)
        {
            memcpy(pListNew, pOldList, sizeof (T) * nOldSize);
            free(pOldList);
        }
        *ppList = pListNew;
    }
    return bSucc;
}

string TimeToStringDate(int64 llSecond);
time_t GetDaySecondBy8Data(const std::string& data);

int PickNumFromStr(const char* pStr, char cSepc);
const char* MoveToNext(const char* pStr, char cSpec);

void DismantlingKVLine(const std::string& line, std::string& out_key, std::string& out_value);

void BuildKeyWithId(std::string& key, int nIndex, const char* head = NULL);
