namespace ColonySummary
{
    public class PawnColumnWorker_Comfort : PawnColumnWorker_GearText
    {
        protected override int Width => 120;

        protected override string CellText(PawnGear gear) => gear.comfyText;

        // Coldest-tolerant first when sorted descending
        protected override float Value(PawnGear gear) => -gear.comfyMin;
    }
}
