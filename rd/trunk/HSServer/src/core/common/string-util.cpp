#include "string-util.h"
#include <iostream>
#include <sstream>
#include <ctime>
using namespace std;

bool
safe_atoi(const char* str, int &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}

bool
safe_atoi(const string &str, int &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}

bool
safe_atoi(const string &str, time_t &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}
#ifndef _WIN32

bool
safe_atoi(const string &str, size_t &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}
#endif // _WIN32

bool
safe_atoi(const string &str, unsigned int &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}

bool
safe_atoi(const string &str, unsigned char &num)
{
    int value;
    istringstream iss(str);
    bool ret = !(iss >> value).fail();
    num = (unsigned char) value;
    return ret;
}

bool
safe_atoll(const char* str, long long &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}

bool
safe_atoll(const string &str, long long &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}

bool
safe_atof(const string &str, float &num)
{
    istringstream iss(str);
    return !(iss >> num).fail();
}

bool
tokenize(const string& str, vector<string>& tokens, string &delims)
{
    tokens.clear();
    string::size_type lastPos = 0;
    string::size_type pos;
    int posadd = 1; //delims.size();
    if (posadd == 0)
    {
        posadd = 1;
    }
    while ((pos = str.find_first_of(delims, lastPos)) != str.npos)
    {
        tokens.push_back(str.substr(lastPos, pos - lastPos));
        lastPos = pos + posadd;
    }
    tokens.push_back(str.substr(lastPos));
    return true;
}

bool
tokenize_as_int(const string& str, vector<int>& tokens, const string &delims)
{
    tokens.clear();
    if (str.length() == 0) return true;
    std::string strset1 = "1234567890";
    string::size_type first_pos = 0;
    first_pos = str.find_first_of(strset1, first_pos);
    if (first_pos == string::npos)
    {
        return false;
    }
    string::size_type lastPos = first_pos;
    string::size_type pos;
    bool succ = true;
    int int_value;
    while ((pos = str.find_first_of(delims, lastPos)) != str.npos)
    {
        succ = succ && safe_atoi(str.substr(lastPos, pos - lastPos).c_str(), int_value);
        if (succ) tokens.push_back(int_value);
        lastPos = pos + 1;
    }
    succ = succ && safe_atoi(str.substr(lastPos).c_str(), int_value);
    if (succ) tokens.push_back(int_value);
    return succ;
}

bool
tokenize_as_longlong(const string& str, vector<long long>& tokens, const string &delims)
{
    tokens.clear();
    std::string strset1 = "1234567890";
    string::size_type first_pos = 0;
    first_pos = str.find_first_of(strset1, first_pos);
    if (first_pos == string::npos)
    {
        return false;
    }
    string::size_type lastPos = first_pos;
    string::size_type pos;
    bool succ = true;
    long long int_value;
    while ((pos = str.find_first_of(delims, lastPos)) != str.npos)
    {
        succ = succ && safe_atoll(str.substr(lastPos, pos - lastPos).c_str(), int_value);
        if (succ) tokens.push_back(int_value);
        lastPos = pos + 1;
    }
    succ = succ && safe_atoll(str.substr(lastPos).c_str(), int_value);
    if (succ) tokens.push_back(int_value);
    return succ;
}

static const char *codes =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

/**
 base64 Encode a buffer (NUL terminated)
 @param in      The input buffer to encode
 @param inlen   The length of the input buffer
 @param out     [out] The destination of the base64 encoded data
 @param outlen  [in/out] The max size and resulting size
 @return 0 if successful
 */
int
base64_encode(const unsigned char *in,  unsigned long inlen,
        unsigned char *out, unsigned long *outlen)
{
    unsigned long i, len2, leven;
    unsigned char *p;

    if (in == NULL)
    {
        return 1;
    }
    if (out == NULL)
    {
        return 1;
    }
    if (outlen == NULL)
    {
        return 1;
    }

    /* valid output size ? */
    len2 = 4 * ((inlen + 2) / 3);
    if (*outlen < len2 + 1)
    {
        return 1;
    }
    p = out;
    leven = 3 * (inlen / 3);
    for (i = 0; i < leven; i += 3)
    {
        *p++ = codes[(in[0] >> 2) & 0x3F];
        *p++ = codes[(((in[0] & 3) << 4) + (in[1] >> 4)) & 0x3F];
        *p++ = codes[(((in[1] & 0xf) << 2) + (in[2] >> 6)) & 0x3F];
        *p++ = codes[in[2] & 0x3F];
        in += 3;
    }
    /* Pad it if necessary...  */
    if (i < inlen)
    {
        unsigned a = in[0];
        unsigned b = (i + 1 < inlen) ? in[1] : 0;

        *p++ = codes[(a >> 2) & 0x3F];
        *p++ = codes[(((a & 3) << 4) + (b >> 4)) & 0x3F];
        *p++ = (i + 1 < inlen) ? codes[(((b & 0xf) << 2)) & 0x3F] : '=';
        *p++ = '=';
    }

    /* append a NULL byte */
    *p = '\0';

    /* return ok */
    *outlen = p - out;
    return 0;
}

static const std::string base64_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

static inline bool
is_base64(unsigned char c)
{
    return (isalnum(c) || (c == '+') || (c == '/'));
}

std::string
base64_encode(std::string const& input_str)
{
    unsigned char const* bytes_to_encode = (unsigned char const*) input_str.c_str();
    unsigned int in_len = input_str.length();

    std::string ret;
    int i = 0;
    int j = 0;
    unsigned char char_array_3[3];
    unsigned char char_array_4[4];

    while (in_len--)
    {
        char_array_3[i++] = *(bytes_to_encode++);
        if (i == 3)
        {
            char_array_4[0] = (char_array_3[0] & 0xfc) >> 2;
            char_array_4[1] = ((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4);
            char_array_4[2] = ((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6);
            char_array_4[3] = char_array_3[2] & 0x3f;

            for (i = 0; (i < 4) ; i++)
            {
                ret += base64_chars[char_array_4[i]];
            }
            i = 0;
        }
    }

    if (i)
    {
        for (j = i; j < 3; j++)
        {
            char_array_3[j] = '\0';
        }

        char_array_4[0] = (char_array_3[0] & 0xfc) >> 2;
        char_array_4[1] = ((char_array_3[0] & 0x03) << 4) + ((char_array_3[1] & 0xf0) >> 4);
        char_array_4[2] = ((char_array_3[1] & 0x0f) << 2) + ((char_array_3[2] & 0xc0) >> 6);
        char_array_4[3] = char_array_3[2] & 0x3f;

        for (j = 0; (j < i + 1); j++)
        {
            ret += base64_chars[char_array_4[j]];
        }

        while ((i++ < 3))
        {
            ret += '=';
        }
    }
    return ret;
}

std::string
base64_decode(std::string const& encoded_string)
{
    int in_len = encoded_string.size();
    int i = 0;
    int j = 0;
    int in_ = 0;
    unsigned char char_array_4[4], char_array_3[3];
    std::string ret;

    while (in_len-- && ( encoded_string[in_] != '=') && is_base64(encoded_string[in_]))
    {
        char_array_4[i++] = encoded_string[in_];
        in_++;
        if (i == 4)
        {
            for (i = 0; i < 4; i ++)
            {
                char_array_4[i] = base64_chars.find(char_array_4[i]);
            }

            char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
            char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
            char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

            for (i = 0; (i < 3); i++)
            {
                ret += char_array_3[i];
            }
            i = 0;
        }
    }

    if (i)
    {
        for (j = i; j < 4; j++)
        {
            char_array_4[j] = 0;
        }
        for (j = 0; j < 4; j++)
        {
            char_array_4[j] = base64_chars.find(char_array_4[j]);
        }
        char_array_3[0] = (char_array_4[0] << 2) + ((char_array_4[1] & 0x30) >> 4);
        char_array_3[1] = ((char_array_4[1] & 0xf) << 4) + ((char_array_4[2] & 0x3c) >> 2);
        char_array_3[2] = ((char_array_4[2] & 0x3) << 6) + char_array_4[3];

        for (j = 0; (j < i - 1); j++)
        {
            ret += char_array_3[j];
        }
    }

    return ret;
}

bool
gbk_to_utf8( const string& input, string &output )
{
    int len = gbk_to_ucs16(input.c_str(), NULL, 0);
    if (len <= 0 )
    {
        return false;
    }
    unsigned short *us;
    us = new unsigned short[len + 1];
    memset(us, 0, len);
    gbk_to_ucs16(input.c_str(), us, len);
    us[len] = 0;
    int utf8_length = ucs16_to_utf8(us, NULL, 0);
    if (utf8_length <= 0 )
    {
        delete []us;
        return false;
    }
    char* utf8s = new char[utf8_length + 1];
    memset(utf8s, 0, utf8_length);
    ucs16_to_utf8(us, (utf8s), utf8_length);
    utf8s[utf8_length] = 0;
    output = utf8s;
    delete []us;
    delete []utf8s;
    return true;
}

long long
StringToUid(const std::string& str)
{
    long long id = -1;
    safe_atoll(str, id);
    return id;
}

int
PlatId2Int(const std::string& plat_id)
{
    int nID = 0;
    string strPlatFormID = plat_id;
    if (strPlatFormID.size() > 8)
    {
        string str = strPlatFormID.substr(strPlatFormID.size() - 8);
        int nVal = 0;
        for (int i = 0; i < 7; i++)
        {
            nVal = 0;
            char ch = str[i];
            if (ch >= '0' && ch <= '9')
                nVal = ch - '0';
            else if (ch >= 'a' && ch <= 'f')
                nVal = 10 + ch - 'a';
            else if (ch >= 'A' && ch <= 'F')
                nVal = 10 + ch - 'A';
            nID += nVal;
            nID *= 16;
        }
        nVal = 0;
        char ch = str[7];
        if (ch >= '0' && ch <= '9')
            nVal = ch - '0';
        else if (ch >= 'a' && ch <= 'f')
            nVal = 10 + ch - 'a';
        else if (ch >= 'A' && ch <= 'F')
            nVal = 10 + ch - 'A';
        nID += nVal;
    }
    return nID;
}

time_t
tm_val_to_time_t(int year, int month, int day, int hour, int minute, int second)
{
    struct tm  tm_val;
    time_t rawtime = 0;
    //time_t time_now = 
    time ( &rawtime );
    tm_val = *localtime ( &rawtime );
    tm_val.tm_year = year - 1900;
    tm_val.tm_mon = month - 1;
    tm_val.tm_mday = day;
    tm_val.tm_hour = hour;
    tm_val.tm_min = minute;
    tm_val.tm_sec = second;
    return mktime(&tm_val);
}

int64
GetTimeVal(const std::string& date)
{
    int y = 0;
    int ma = 0;
    int d = 0;
    int h = 0;
    int m = 0;
    int s = 0;
    sscanf(date.c_str(), "%d-%d-%d-%d:%d:%d", &y, &ma, &d, &h, &m, &s);
    return tm_val_to_time_t(y, ma, d, h, m, s);
}

void
GatewayName(const std::string& s, const std::string& a, std::string& out)
{
    out = s + "." + a;
}

int
String2Int(const std::string& s)
{
    int n = llInvalidId;
    safe_atoi(s, n);
    return n;
}

bool
StringToBool(const string& str)
{
    bool bResult = false;
    if (str.size() > 0)
    {
        char c = str.c_str()[0];
        bResult = c == '1'
                || c == 't'
                || c == 'T';

    }
    return bResult;
}

string
TimeToString(int64 llSecond)
{
    ssm.clear();
    ssm.str("");
    time_t timep = llSecond;
    struct tm *p;
    //time(&timep);
    p = localtime(&timep); //取得当地时间
    ssm << (1900 + p->tm_year) << "-" << (1 + p->tm_mon) << "-" << (p->tm_mday) << " " << p->tm_hour << ":" << p->tm_min << ":" << p->tm_sec ;
    return ssm.str();
}

string
TimeToStringDate(int64 llSecond)
{
    ssm.clear();
    ssm.str("");
    time_t timep = llSecond;
    struct tm *p;
    //time(&timep);
    p = localtime(&timep); //取得当地时间
    ssm << (1900 + p->tm_year) << "-" << (1 + p->tm_mon) << "-" << (p->tm_mday);
    return ssm.str();
}

time_t
GetDaySecondBy8Data(const std::string& date)
{
    if (date.size() != 8)
    {
        return (time_t) (-1);
    }
    int y = 0;
    int ma = 0;
    int d = 0;
    int h = 0;
    int m = 0;
    int s = 0;
    //sscanf(date.c_str(), "%d-%d-%d-%d:%d:%d", &y, &ma, &d, &h, &m, &s);
    y = String2Int(date.substr(0, 4));
    ma = String2Int(date.substr(4, 2));
    d = String2Int(date.substr(6, 2));

    return tm_val_to_time_t(y, ma, d, h, m, s);
}

int
PickNumFromStr(const char* pStr, char cSepc)
{
    if (pStr == NULL)
    {
        return 0;
    }
    int nRlt = 0;
    int nIndex = 0;
    int nPON = 1;
    for (; pStr[nIndex] != '\0' && pStr[nIndex] != cSepc; nIndex++)
    {
        if (pStr[nIndex] == '-' && nIndex == 0)
        {
            nPON = -1;
        }
        else if (pStr[nIndex] >= '0' && pStr[nIndex] <= '9')
        {
            nIndex = nIndex * 10 + pStr[nIndex] - '0' ;
        }
    }
    return nIndex * nPON;
}

const char*
MoveToNext(const char* pStr, char cSpec)
{
    const char* pResult = pStr;

    int i = 0;
    //if (*pResult == cSpec) i = 1;

    for (; pStr != NULL && pStr[i] != '\0' && pStr[i] != cSpec; i++);
    pResult = &(pStr[i]);
    if (*pResult != '\0') pResult++;
    return pResult;
}

void
DismantlingKVLine(const std::string& line, std::string& out_key, std::string& out_value)
{
    int nSpec = line.find_first_of(':');
    out_key   = line.substr(0, nSpec);
    out_value = line.substr(nSpec + 1);
}

void
BuildKeyWithId(std::string& key, int nIndex, const char* head/* = NULL*/)
{
    char buff[128];
    if (head == NULL)
    {
        sprintf(buff, "%s_%08d", key.c_str(), nIndex);
    }
    else
    {
        sprintf(buff, "%s_%08d", head, nIndex);
    }
    key = buff;
}