using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySummary
{
    // A row of item icons; hover for the item, click for its info card
    public abstract class PawnColumnWorker_GearIcons : PawnColumnWorker
    {
        private const float IconSize = 28f;
        private const float Step = 30f;
        private const int MaxOptimalIcons = 14;

        // "+N" labels, built once per count; session lifetime, holds only strings
        private static readonly Dictionary<int, string> overflowLabels = new Dictionary<int, string>();

        protected abstract List<GearItem> Items(PawnGear gear);

        protected abstract string ListTip(PawnGear gear);

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            PawnGear gear = GearCache.Get(pawn);
            List<GearItem> items = Items(gear);
            if (items.Count == 0) return;

            int fit = Mathf.Max(1, Mathf.FloorToInt(rect.width / Step));
            int shown = items.Count > fit ? fit - 1 : items.Count;
            float y = rect.y + (rect.height - IconSize) / 2f;
            for (int i = 0; i < shown; i++)
            {
                DrawItem(new Rect(rect.x + i * Step, y, IconSize, IconSize), items[i]);
            }
            if (shown < items.Count)
            {
                Rect more = new Rect(rect.x + shown * Step, y, IconSize, IconSize);
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(more, OverflowLabel(items.Count - shown));
                Text.Anchor = TextAnchor.UpperLeft;
                if (Mouse.IsOver(more))
                {
                    Widgets.DrawHighlight(more);
                    TooltipHandler.TipRegion(more, ListTip(gear));
                }
            }
        }

        private static void DrawItem(Rect rect, GearItem item)
        {
            if (Mouse.IsOver(rect))
            {
                Widgets.DrawHighlight(rect);
                TooltipHandler.TipRegion(rect, item.tip);
            }
            Widgets.ThingIcon(rect, item.thing);
            if (item.condition >= 0f)
            {
                Rect bar = new Rect(rect.x + 2f, rect.yMax - 3f, rect.width - 4f, 3f);
                Widgets.FillableBar(bar, item.condition, item.condition < 0.5f ? GearTextures.ConditionBarLow : GearTextures.ConditionBar, BaseContent.BlackTex, false);
            }
            if (item.countText != null)
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.LowerRight;
                Widgets.Label(new Rect(rect.x - 10f, rect.y, rect.width + 12f, rect.height + 4f), item.countText);
                Text.Anchor = TextAnchor.UpperLeft;
            }
            if (Widgets.ButtonInvisible(rect))
            {
                Find.WindowStack.Add(new Dialog_InfoCard(item.thing));
            }
        }

        private static string OverflowLabel(int count)
        {
            if (!overflowLabels.TryGetValue(count, out string? label))
            {
                label = "+" + count;
                overflowLabels[count] = label;
            }
            return label;
        }

        public override int GetMinWidth(PawnTable table)
        {
            return Mathf.Max(base.GetMinWidth(table), Mathf.CeilToInt(Step));
        }

        public override int GetOptimalWidth(PawnTable table)
        {
            int most = 1;
            List<Pawn> pawns = table.PawnsListForReading;
            for (int i = 0; i < pawns.Count; i++)
            {
                most = Mathf.Max(most, Items(GearCache.Get(pawns[i])).Count);
            }
            return Mathf.Max(GetMinWidth(table), Mathf.CeilToInt(Mathf.Min(most, MaxOptimalIcons) * Step));
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return Items(GearCache.Get(a)).Count.CompareTo(Items(GearCache.Get(b)).Count);
        }
    }
}
