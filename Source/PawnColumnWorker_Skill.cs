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
        // Size of the Work tab's priority box
        private const float BoxSize = 25f;

        private static readonly Texture2D BarTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));
        private static readonly Texture2D SortingTex = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
        private static readonly Texture2D SortingDescendingTex = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
        private static readonly Color DisabledColor = new Color(1f, 1f, 1f, 0.5f);

        // Header height that fits the longest skill label when inclined; session lifetime, from defs only
        private static int headerHeight;

        private string? label;
        private float labelHeight;

        private SkillColumnDef Def => (SkillColumnDef)def;

        private string Label
        {
            get
            {
                if (label == null)
                {
                    label = Def.skill!.skillLabel.CapitalizeFirst();
                    Text.Font = GameFont.Small;
                    labelHeight = Text.CalcSize(label).y;
                }
                return label;
            }
        }

        // Inclined label like Division of Labor's Work tab; clicks and hover are tested in the label's rotated space
        public override void DoHeader(Rect rect, PawnTable table)
        {
            if (table.SortingBy == def)
            {
                Texture2D tex = table.SortingDescending ? SortingDescendingTex : SortingTex;
                GUI.DrawTexture(new Rect(rect.center.x - tex.width / 2f, rect.yMax - tex.height, tex.width, tex.height), tex);
            }

            string text = Label;
            (Rect labelRect, Matrix4x4 labelMatrix) = InclinedLabel.Draw(rect, text, labelHeight);

            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = labelMatrix;
            bool mouseOver = labelRect.Contains(Event.current.mousePosition);
            if (mouseOver) Widgets.DrawHighlight(labelRect);
            bool clicked = Widgets.ButtonInvisible(labelRect);
            GUI.matrix = originalMatrix;

            if (clicked) HeaderClicked(rect, table);
            if (mouseOver)
            {
                MouseoverSounds.DoRegion(rect);
                TooltipHandler.TipRegion(new Rect(0f, 0f, UI.screenWidth, UI.screenHeight), GetHeaderTip(table));
            }
        }

        public override int GetMinHeaderHeight(PawnTable table)
        {
            if (headerHeight == 0)
            {
                float longest = 0f;
                Text.Font = GameFont.Small;
                foreach (SkillDef skill in DefDatabase<SkillDef>.AllDefs)
                {
                    longest = Mathf.Max(longest, Text.CalcSize(skill.skillLabel.CapitalizeFirst()).x);
                }
                headerHeight = Mathf.CeilToInt(InclinedLabel.HeaderHeightFor(longest));
            }
            return Mathf.Max(base.GetMinHeaderHeight(table), headerHeight);
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
            Text.Anchor = TextAnchor.MiddleCenter;
            if (skill.TotallyDisabled)
            {
                GUI.color = DisabledColor;
                Widgets.Label(inner, "-");
            }
            else
            {
                int level = skill.GetLevel();
                Widgets.FillableBar(inner, Mathf.Max(0.01f, level / 20f), BarTex, null, false);
                // Grey flame in the bottom-right quarter of a box around the number, behind it, as on the Work tab
                if (skill.passion != Passion.None)
                {
                    Rect box = new Rect(inner.center.x - BoxSize / 2f, inner.center.y - BoxSize / 2f, BoxSize, BoxSize);
                    Rect icon = new Rect(box.center.x, box.center.y, BoxSize / 2f, BoxSize / 2f);
                    GUI.DrawTexture(icon, skill.passion == Passion.Major ? WidgetsWork.PassionWorkboxMajorIcon : WidgetsWork.PassionWorkboxMinorIcon);
                }
                Widgets.Label(inner, level.ToStringCached());
            }
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

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
