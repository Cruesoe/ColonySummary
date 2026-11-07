using System.Collections.Generic;
using Verse;

namespace ColonySummary
{
    // Per-pawn row data; lifetime is the Summary window, cleared when it opens and closes
    public static class SummaryCache
    {
        private static readonly Dictionary<Pawn, PawnSummary> entries = new Dictionary<Pawn, PawnSummary>();

        // Set when a pawn's gear changes after the table was laid out
        public static bool TableStale;

        public static PawnSummary Get(Pawn pawn)
        {
            int signature = PawnSummary.Signature(pawn);
            if (entries.TryGetValue(pawn, out PawnSummary? summary))
            {
                if (summary.signature != signature)
                {
                    summary.RebuildGear(pawn, signature);
                    TableStale = true;
                }
            }
            else
            {
                summary = new PawnSummary();
                summary.RebuildGear(pawn, signature);
                entries[pawn] = summary;
            }
            summary.RefreshStats(pawn);
            return summary;
        }

        public static void Clear()
        {
            entries.Clear();
            SkillTooltip.Clear();
            TableStale = false;
        }
    }
}
