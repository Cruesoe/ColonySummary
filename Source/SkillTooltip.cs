using System;
using System.Reflection;
using RimWorld;
using Verse;

namespace ColonySummary
{
    // The Bio tab's skill tooltip for the hovered cell, rebuilt only when the hovered skill or its progress changes; cleared with SummaryCache
    public static class SkillTooltip
    {
        // SkillUI.GetSkillDescription is private; resolved once, with a short fallback if it's missing
        private static readonly Func<SkillRecord, string>? describe = Resolve();

        private static Pawn? lastPawn;
        private static SkillDef? lastSkill;
        private static float lastXp = -1f;
        private static int lastLevel = -1;
        private static Passion lastPassion;
        private static string lastTip = "";

        private static Func<SkillRecord, string>? Resolve()
        {
            MethodInfo? method = typeof(SkillUI).GetMethod("GetSkillDescription", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SkillRecord) }, null);
            return method == null ? null : (Func<SkillRecord, string>)Delegate.CreateDelegate(typeof(Func<SkillRecord, string>), method);
        }

        public static string Get(Pawn pawn, SkillRecord skill)
        {
            int level = skill.GetLevel();
            if (pawn != lastPawn || skill.def != lastSkill || skill.xpSinceLastLevel != lastXp || level != lastLevel || skill.passion != lastPassion)
            {
                lastPawn = pawn;
                lastSkill = skill.def;
                lastXp = skill.xpSinceLastLevel;
                lastLevel = level;
                lastPassion = skill.passion;
                lastTip = describe != null ? describe(skill) : skill.def.LabelCap + ": " + level;
            }
            return lastTip;
        }

        public static void Clear()
        {
            lastPawn = null;
            lastSkill = null;
            lastXp = -1f;
            lastLevel = -1;
            lastTip = "";
        }
    }
}
