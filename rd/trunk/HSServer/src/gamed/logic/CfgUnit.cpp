/* 
 * File:   CfgUnit.cpp
 * Author: Kidd
 * 
 * Created on 2013��1��14��, ����9:43
 */

#include "CfgUnit.h"
#include "../../common/const_def.h"

CfgUnit::CfgUnit()
{
    id = llInvalidId;
}

CfgUnit::~CfgUnit()
{
}

int CfgUnit::GetId()
{
    return id;
}