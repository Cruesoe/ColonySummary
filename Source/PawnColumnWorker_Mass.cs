namespace ColonySummary
{
    // Mass carried out of capacity, in red when over capacity
    public class PawnColumnWorker_Mass : PawnColumnWorker_GearText
    {
        protected override int Width => 110;

        protected override string CellText(PawnGear gear) => gear.massText;

        protected override float Value(PawnGear gear) => gear.mass;

        protected override bool Warn(PawnGear gear) => gear.overweight;
    }
}
