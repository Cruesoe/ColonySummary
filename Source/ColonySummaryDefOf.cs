using RimWorld;

namespace ColonySummary
{
    [DefOf]
    public static class ColonySummaryDefOf
    {
        public static PawnTableDef ColonySummary_Summary = null!;
        public static PawnColumnDef ColonySummary_Skills = null!;
        public static PawnColumnDef ColonySummary_ArmorSharp = null!;
        public static PawnColumnDef ColonySummary_ArmorBlunt = null!;
        public static PawnColumnDef ColonySummary_ArmorHeat = null!;

        static ColonySummaryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ColonySummaryDefOf));
        }
    }
}
