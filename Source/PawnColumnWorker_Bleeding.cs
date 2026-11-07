using UnityEngine;
using Verse;

namespace ColonySummary
{
    // Time until death by blood loss in red, or the bleed rate when it isn't lethal soon
    public class PawnColumnWorker_Bleeding : PawnColumnWorker_SummaryText
    {
        private static readonly Color NotLethalColor = new Color(1f, 0.8f, 0.4f);

        protected override int Width => 56;

        protected override string CellText(PawnSummary summary) => summary.bleedText;

        // Soonest death first when sorted descending, then the fastest bleeding
        protected override float Value(PawnSummary summary) => summary.bleedLethal ? 1e9f - summary.bleedDeathTicks : summary.bleedRate;

        protected override Color CellColor(PawnSummary summary) => summary.bleedLethal ? ColorLibrary.RedReadable : NotLethalColor;

        protected override string CellTip(PawnSummary summary) => summary.bleedTip;
    }
}
