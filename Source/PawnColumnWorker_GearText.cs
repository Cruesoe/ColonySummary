using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySummary
{
    // A centered value from the pawn's cached gear summary, sorted by its number
    public abstract class PawnColumnWorker_GearText : PawnColumnWorker
    {
        protected abstract int Width { get; }

        protected abstract string CellText(PawnGear gear);

        protected abstract float Value(PawnGear gear);

        protected virtual bool Warn(PawnGear gear) => false;

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            PawnGear gear = GearCache.Get(pawn);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            if (Warn(gear)) GUI.color = ColorLibrary.RedReadable;
            Widgets.Label(rect, CellText(gear));
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override int GetMinWidth(PawnTable table) => Mathf.Max(base.GetMinWidth(table), Width);

        public override int Compare(Pawn a, Pawn b) => Value(GearCache.Get(a)).CompareTo(Value(GearCache.Get(b)));
    }
}
