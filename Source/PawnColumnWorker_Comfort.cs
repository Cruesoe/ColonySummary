using UnityEngine;
using Verse;

namespace ColonySummary
{
    // Comfortable temperature range, in green while the pawn's current temperature is inside it
    public class PawnColumnWorker_Comfort : PawnColumnWorker_SummaryText
    {
        protected override int Width => 120;

        protected override string CellText(PawnSummary summary) => summary.comfyText;

        // Coldest-tolerant first when sorted descending
        protected override float Value(PawnSummary summary) => -summary.comfyMin;

        protected override Color CellColor(PawnSummary summary) => summary.comfyNow ? ColorLibrary.Green : Color.white;

        protected override string CellTip(PawnSummary summary) => summary.comfyTip;
    }
}
