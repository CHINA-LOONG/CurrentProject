#include "../GameNetHandler.h"
#include "../GameDataHandler.h"
#include "../../event/event.pb.h"
#include "../../common/string-util.h"
#include "../../common/Msg2QQ.h"
#include "../../logic/User.h"
#include "../../logic/UserCtrl.h"
#include "DealMseRank.h"
#include "../../logic/Rank.h"
#include "../../logic/RankInfoMgr.h"

DealMseRank::DealMseRank()
: CBaseEvent()
{

}

DealMseRank::~DealMseRank()
{

}

bool DealMseRank::CheckEvent(Event* e)
{

    return false;
}

void DealMseRank::handle(Event* e)
{

    int64 nUserID = e->uid();
    GameDataHandler* dh = eh_->getDataHandler();

    const MseRank& request = e->mse_mserank();
    MseRank* pMseRank = e->mutable_mse_mserank();
    //write code start

    //if (e->state() == Status_Normal_Game)
    {
        Rank* pRank = RankInfoMgr::GetInst()->GetRank(request.rankname());
        if ( pRank == NULL )
            return;
        const SingleRankItem rank_item = request.rank_item();
        pRank->Add( rank_item.uid(), rank_item.name(), rank_item.num(), request.rank_version(), rank_item.url() );
    }

    //write code end
    //here send proto to flash
    //string text;
    //pMseRank->SerializeToString(&text);
    //eh_->sendDataToUser(pUser->fd(), S2C_MseRank, text);
}
