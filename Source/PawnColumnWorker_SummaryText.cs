using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySummary
{
    // A centered value from the pawn's cached summary, with an optional tooltip, sorted by its number
    public abstract class PawnColumnWorker_SummaryText : PawnColumnWorker
    {
        protected abstract int Width { get; }

        protected abstract string CellText(PawnSummary summary);

        protected abstract float Value(PawnSummary summary);

        protected virtual Color CellColor(PawnSummary summary) => Color.white;

        protected virtual string CellTip(PawnSummary summary) => "";

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            PawnSummary summary = SummaryCache.Get(pawn);
            string text = CellText(summary);
            if (text.Length == 0) return;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = CellColor(summary);
            Widgets.Label(rect, text);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            if (Mouse.IsOver(rect))
            {
                string tip = CellTip(summary);
                if (tip.Length > 0)
                {
                    Widgets.DrawHighlight(rect);
                    TooltipHandler.TipRegion(rect, tip);
                }
            }
        }

        public override int GetMinWidth(PawnTable table) => Mathf.Max(base.GetMinWidth(table), Width);

        public override int Compare(Pawn a, Pawn b) => Value(SummaryCache.Get(a)).CompareTo(Value(SummaryCache.Get(b)));
    }
}
