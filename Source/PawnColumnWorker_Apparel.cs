using System.Collections.Generic;

namespace ColonySummary
{
    public class PawnColumnWorker_Apparel : PawnColumnWorker_GearIcons
    {
        protected override List<GearItem> Items(PawnSummary summary) => summary.apparel;

        protected override string ListTip(PawnSummary summary) => summary.apparelTip;
    }
}
