/* 
 * File:   CfgMgrBase.h
 * Author: Kidd
 *
 * Created on 2013年1月14日, 上午10:07
 */

#ifndef CFGMGRBASE_H
#define	CFGMGRBASE_H
#include "../../common/const_def.h"
#include "../../logic/ConfigUnit/CfgUnit.h"
#include <map>

#define M_INST_H(type) private: static type* g_pInst;\
public:static type* GetInst(){if (g_pInst==NULL){g_pInst = new type();} return g_pInst;}
#define M_INST_CPP(type) type* type::g_pInst=NULL;

class CfgMgrBase
{
public:
    CfgMgrBase();
    virtual ~CfgMgrBase();
public:
    typedef std::map<int, CfgUnit*> SearchList;
public:
    void            ClearList();
    static void     ClearList(SearchList& list);
public:
    virtual void    Load() = 0;
public:
    SearchList m_xSearchList;
public:
    CfgUnit*        GetUnit(int id);
    static CfgUnit* GetUnitFrom(SearchList& list, int id);

    bool            AddCfg(CfgUnit* pUnit);
    static bool     AddCfgTo(SearchList& list, CfgUnit* pUnit);
} ;

#endif	/* CFGMGRBASE_H */

