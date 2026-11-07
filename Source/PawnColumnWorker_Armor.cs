namespace ColonySummary
{
    // Sharp, blunt or heat armor, chosen by the column's def
    public class PawnColumnWorker_Armor : PawnColumnWorker_SummaryText
    {
        private int index = -1;

        private int Index
        {
            get
            {
                if (index < 0)
                {
                    index = def == ColonySummaryDefOf.ColonySummary_ArmorBlunt ? 1 : def == ColonySummaryDefOf.ColonySummary_ArmorHeat ? 2 : 0;
                }
                return index;
            }
        }

        protected override int Width => 60;

        protected override string CellText(PawnSummary summary) => summary.armorText[Index];

        protected override float Value(PawnSummary summary) => summary.armor[Index];
    }
}
