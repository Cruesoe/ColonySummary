namespace ColonySummary
{
    // Sharp, blunt or heat armor, chosen by the column's def
    public class PawnColumnWorker_Armor : PawnColumnWorker_GearText
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

        protected override string CellText(PawnGear gear) => gear.armorText[Index];

        protected override float Value(PawnGear gear) => gear.armor[Index];
    }
}
