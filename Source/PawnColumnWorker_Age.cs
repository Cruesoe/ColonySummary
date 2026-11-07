namespace ColonySummary
{
    // Biological age in years; the tooltip has the full biological and chronological age
    public class PawnColumnWorker_Age : PawnColumnWorker_SummaryText
    {
        protected override int Width => 40;

        protected override string CellText(PawnSummary summary) => summary.ageText;

        protected override float Value(PawnSummary summary) => summary.ageTicks;

        protected override string CellTip(PawnSummary summary) => summary.ageTip;
    }
}
