/* 
 * File:   CfgMgrBase.cpp
 * Author: Kidd
 * 
 * Created on 2013年1月14日, 上午10:07
 */

#include "CfgMgrBase.h"

CfgMgrBase::CfgMgrBase()
{
    ClearList();
}

CfgMgrBase::~CfgMgrBase()
{
    ClearList();
}

void
CfgMgrBase::ClearList()
{
    //    for (SearchList::iterator itor = m_xSearchList.begin(); itor != m_xSearchList.end(); itor++)
    //    {
    //        CfgUnit* pUnit = (CfgUnit*) (itor->second);
    //        if (pUnit != NULL)
    //        {
    //            delete pUnit;
    //        }
    //    }
    //    m_xSearchList.clear();
    ClearList(m_xSearchList);
}

void
CfgMgrBase::ClearList(SearchList& list)
{
    for (SearchList::iterator itor = list.begin(); itor != list.end(); itor++)
    {
        CfgUnit* pUnit = (CfgUnit*) (itor->second);
        if (pUnit != NULL)
        {
            delete pUnit;
        }
    }
    list.clear();
}

CfgUnit*
CfgMgrBase::GetUnit(int id)
{
    //    CfgUnit* pUnit = NULL;
    //    SearchList::iterator itor = m_xSearchList.find(id);
    //    if (itor != m_xSearchList.end())
    //    {
    //        pUnit = (CfgUnit*) (itor->second);
    //    }
    //    return pUnit;
    return GetUnitFrom(m_xSearchList, id);
}

CfgUnit*
CfgMgrBase::GetUnitFrom(SearchList& list, int id)
{
    CfgUnit* pUnit = NULL;
    SearchList::iterator itor = list.find(id);
    if (itor != list.end())
    {
        pUnit = (CfgUnit*) (itor->second);
    }
    return pUnit;
}

bool
CfgMgrBase::AddCfg(CfgUnit* pUnit)
{
    //    bool bResult = true;
    //    if (pUnit != NULL)
    //    {
    //        m_xSearchList[pUnit->GetId()] = pUnit;
    //    }
    //    else
    //    {
    //        bResult = false;
    //    }
    //    return bResult;
    return AddCfgTo(m_xSearchList, pUnit);
}

bool
CfgMgrBase::AddCfgTo(SearchList& list, CfgUnit* pUnit)
{
    bool bResult = true;
    if (pUnit != NULL)
    {
        list[pUnit->GetId()] = pUnit;
    }
    else
    {
        bResult = false;
    }
    return bResult;
}
