using UnityEngine;
using Verse;

namespace ColonySummary
{
    // Mass carried out of capacity, in red when over capacity
    public class PawnColumnWorker_Mass : PawnColumnWorker_SummaryText
    {
        protected override int Width => 110;

        protected override string CellText(PawnSummary summary) => summary.massText;

        protected override float Value(PawnSummary summary) => summary.mass;

        protected override Color CellColor(PawnSummary summary) => summary.overweight ? ColorLibrary.RedReadable : Color.white;
    }
}
