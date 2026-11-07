using System.Collections.Generic;

namespace ColonySummary
{
    public class PawnColumnWorker_Apparel : PawnColumnWorker_GearIcons
    {
        protected override List<GearItem> Items(PawnGear gear) => gear.apparel;

        protected override string ListTip(PawnGear gear) => gear.apparelTip;
    }
}
