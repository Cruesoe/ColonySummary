using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySummary
{
    public class MainTabWindow_ColonySummary : MainTabWindow_PawnTable
    {
        protected override PawnTableDef PawnTableDef => ColonySummaryDefOf.ColonySummary_Summary;

        // Same pawns as the vanilla Assign tab: free colonists on every map, in caravans and in transporters, except babies
        protected override IEnumerable<Pawn> Pawns
        {
            get
            {
                List<Pawn> all = new List<Pawn>(PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_FreeColonists);
                List<Pawn> result = new List<Pawn>(all.Count);
                for (int i = 0; i < all.Count; i++)
                {
                    if (!all[i].DevelopmentalStage.Baby()) result.Add(all[i]);
                }
                return result;
            }
        }

        public override void PostOpen()
        {
            GearCache.Clear();
            base.PostOpen();
        }

        public override void DoWindowContents(Rect rect)
        {
            // Gear changed while open: rebuild column widths and sort order
            if (GearCache.TableStale)
            {
                GearCache.TableStale = false;
                SetDirty();
            }
            base.DoWindowContents(rect);
        }

        public override void PostClose()
        {
            base.PostClose();
            GearCache.Clear();
        }
    }
}
