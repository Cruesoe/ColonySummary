namespace ColonySummary
{
    public class PawnColumnWorker_Comfort : PawnColumnWorker_SummaryText
    {
        protected override int Width => 120;

        protected override string CellText(PawnSummary summary) => summary.comfyText;

        // Coldest-tolerant first when sorted descending
        protected override float Value(PawnSummary summary) => -summary.comfyMin;
    }
}
