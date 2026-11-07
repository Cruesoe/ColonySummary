using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ColonySummary
{
    public class PawnColumnWorker_Weapon : PawnColumnWorker_GearIcons
    {
        protected override List<GearItem> Items(PawnSummary summary) => summary.weapons;

        protected override string ListTip(PawnSummary summary) => summary.weaponsTip;

        // Groups pawns by primary weapon; unarmed pawns sort after armed ones
        public override int Compare(Pawn a, Pawn b)
        {
            string? la = SummaryCache.Get(a).weaponLabel;
            string? lb = SummaryCache.Get(b).weaponLabel;
            if (la == null || lb == null) return (lb == null).CompareTo(la == null);
            return string.Compare(lb, la, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
