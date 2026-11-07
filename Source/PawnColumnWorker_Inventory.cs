using System.Collections.Generic;

namespace ColonySummary
{
    public class PawnColumnWorker_Inventory : PawnColumnWorker_GearIcons
    {
        protected override List<GearItem> Items(PawnGear gear) => gear.inventory;

        protected override string ListTip(PawnGear gear) => gear.inventoryTip;
    }
}
