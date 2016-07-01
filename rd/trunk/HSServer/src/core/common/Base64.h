/* 
 * File:   Base64.h
 * Author: Kidd
 *
 * Created on 2012年3月13日, 下午1:42
 */

#ifndef BASE64_H
#define	BASE64_H
#include "string-util.h"
class Base64
{
public:

    static inline bool is_base64(unsigned char c)
    {
        return (isalnum(c) || (c == '+') || (c == '/'));
    };
    static bool base64_encode(unsigned char const* s, unsigned int len, std::string& ret);
    static bool base64_encode(std::string const& s, std::string& ret);
    static bool base64_decode(unsigned char const* , unsigned int len, std::string& ret);
    static bool base64_decode(std::string const& s, std::string& ret);
} ;


#endif	/* BASE64_H */

