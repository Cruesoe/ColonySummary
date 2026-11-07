using System.Collections.Generic;
using Verse;

namespace ColonySummary
{
    // Per-pawn gear summaries; lifetime is the Summary window, cleared when it opens and closes
    public static class GearCache
    {
        private static readonly Dictionary<Pawn, PawnGear> entries = new Dictionary<Pawn, PawnGear>();

        // Set when a pawn's gear changes after the table was laid out
        public static bool TableStale;

        public static PawnGear Get(Pawn pawn)
        {
            int signature = PawnGear.Signature(pawn);
            if (entries.TryGetValue(pawn, out PawnGear? gear))
            {
                if (gear.signature != signature)
                {
                    gear.Rebuild(pawn, signature);
                    TableStale = true;
                }
                return gear;
            }
            gear = new PawnGear();
            gear.Rebuild(pawn, signature);
            entries[pawn] = gear;
            return gear;
        }

        public static void Clear()
        {
            entries.Clear();
            TableStale = false;
        }
    }
}
