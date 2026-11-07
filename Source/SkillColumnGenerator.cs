using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ColonySummary
{
    // Replaces the ColonySummary_Skills marker column with one column per skill, in the Bio tab's order
    [StaticConstructorOnStartup]
    public static class SkillColumnGenerator
    {
        static SkillColumnGenerator()
        {
            PawnTableDef table = ColonySummaryDefOf.ColonySummary_Summary;
            PawnColumnDef marker = ColonySummaryDefOf.ColonySummary_Skills;
            int index = table.columns.IndexOf(marker);
            if (index < 0) return;
            table.columns.RemoveAt(index);

            List<SkillDef> skills = DefDatabase<SkillDef>.AllDefs.OrderByDescending(s => s.listOrder).ToList();
            for (int i = 0; i < skills.Count; i++)
            {
                SkillColumnDef column = new SkillColumnDef
                {
                    defName = "ColonySummary_Skill_" + skills[i].defName,
                    workerClass = typeof(PawnColumnWorker_Skill),
                    sortable = true,
                    headerTip = skills[i].description,
                    skill = skills[i],
                    skillIndex = i,
                    modContentPack = marker.modContentPack
                };
                DefDatabase<PawnColumnDef>.Add(column);
                table.columns.Insert(index + i, column);
            }
        }
    }
}
