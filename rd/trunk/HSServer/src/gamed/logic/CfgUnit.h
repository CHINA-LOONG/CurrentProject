/* 
 * File:   CfgUnit.h
 * Author: Kidd
 *
 * Created on 2013年1月14日, 上午9:43
 */

#ifndef CFGUNIT_H
#define	CFGUNIT_H
//#include "../../common/DBC.h"
class DBCFile;

class CfgUnit
{
public:
    CfgUnit();
    virtual ~CfgUnit();
    int id;
public:
    virtual void Clear() = 0;
    virtual void LoadFromCfgFile(const DBCFile& dbc, int line) = 0;
public:
    int     GetId();

    void    SetId(int v)
    {
        id = v;
    }
} ;

#endif	/* CFGUNIT_H */

