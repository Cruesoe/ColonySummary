using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ColonySummary
{
    // One skill as on the Bio tab: level, passion and a level bar, with the Bio tab's tooltip
    [StaticConstructorOnStartup]
    public class PawnColumnWorker_Skill : PawnColumnWorker
    {
        private const float HeaderHeight = 50f;
        private const float HeaderStagger = 20f;
        private const float PassionSize = 20f;

        private static readonly Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));
        private static readonly Color DisabledColor = new Color(1f, 1f, 1f, 0.5f);

        private string? label;
        private Vector2 labelSize;

        private SkillColumnDef Def => (SkillColumnDef)def;

        private string Label
        {
            get
            {
                if (label == null)
                {
                    label = Def.skill!.skillLabel.CapitalizeFirst();
                    Text.Font = GameFont.Small;
                    labelSize = Text.CalcSize(label);
                }
                return label;
            }
        }

        public override void DoHeader(Rect rect, PawnTable table)
        {
            base.DoHeader(rect, table);
            string text = Label;
            Text.Font = GameFont.Small;
            Rect labelRect = new Rect(rect.center.x - labelSize.x / 2f, rect.y + (Def.skillIndex % 2 == 1 ? HeaderStagger : 0f), labelSize.x, labelSize.y);
            MouseoverSounds.DoRegion(labelRect);
            Widgets.Label(labelRect, text);
            GUI.color = new Color(1f, 1f, 1f, 0.3f);
            Widgets.DrawLineVertical(labelRect.center.x, labelRect.yMax - 3f, rect.y + HeaderHeight - labelRect.yMax + 3f);
            GUI.color = Color.white;
        }

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            SkillRecord? skill = pawn.skills?.GetSkill(Def.skill);
            if (skill == null) return;
            Rect inner = rect.ContractedBy(1f, 3f);
            if (Mouse.IsOver(inner))
            {
                Widgets.DrawHighlight(inner);
                TooltipHandler.TipRegion(inner, new TipSignal(SkillTooltip.Get(pawn, skill), Def.skill!.GetHashCode() * 397945));
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleRight;
            if (skill.TotallyDisabled)
            {
                GUI.color = DisabledColor;
                Widgets.Label(inner, "-");
            }
            else
            {
                int level = skill.GetLevel();
                Widgets.FillableBar(inner, Mathf.Max(0.01f, level / 20f), BarTex, null, false);
                if (skill.passion != Passion.None)
                {
                    Rect icon = new Rect(inner.x, inner.y + (inner.height - PassionSize) / 2f, PassionSize, PassionSize);
                    GUI.DrawTexture(icon, skill.passion == Passion.Major ? SkillUI.PassionMajorIcon : SkillUI.PassionMinorIcon);
                }
                Widgets.Label(new Rect(inner.x, inner.y, inner.width - 2f, inner.height), level.ToStringCached());
            }
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override int GetMinHeaderHeight(PawnTable table) => Mathf.Max(base.GetMinHeaderHeight(table), (int)HeaderHeight);

        public override int GetMinWidth(PawnTable table) => Mathf.Max(base.GetMinWidth(table), 40);

        public override int GetOptimalWidth(PawnTable table) => Mathf.Max(GetMinWidth(table), 46);

        public override int GetMaxWidth(PawnTable table) => Mathf.Max(GetMinWidth(table), 60);

        // Disabled skills sort lowest; equal levels sort by passion
        public override int Compare(Pawn a, Pawn b) => SortValue(a).CompareTo(SortValue(b));

        private int SortValue(Pawn pawn)
        {
            SkillRecord? skill = pawn.skills?.GetSkill(Def.skill);
            if (skill == null || skill.TotallyDisabled) return -1;
            return skill.GetLevel() * 3 + (int)skill.passion;
        }
    }
}
