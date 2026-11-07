using UnityEngine;

namespace ColonySummary
{
    // Number of conditions that can be tended now; the tooltip lists them
    public class PawnColumnWorker_Tend : PawnColumnWorker_SummaryText
    {
        private static readonly Color NeedsTendingColor = new Color(1f, 0.8f, 0.4f);

        protected override int Width => 44;

        protected override string CellText(PawnSummary summary) => summary.tendText;

        protected override float Value(PawnSummary summary) => summary.tendCount;

        protected override Color CellColor(PawnSummary summary) => NeedsTendingColor;

        protected override string CellTip(PawnSummary summary) => summary.tendTip;
    }
}
