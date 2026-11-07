using RimWorld;

namespace ColonySummary
{
    // A Summary column for one skill, generated at startup by SkillColumnGenerator
    public class SkillColumnDef : PawnColumnDef
    {
        public SkillDef? skill;
        // Position among the skill columns; odd columns draw their header lower so labels don't overlap
        public int skillIndex;
    }
}
