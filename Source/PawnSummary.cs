using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySummary
{
    // One pawn's row data: gear icons are rebuilt when the gear changes, everything else once a game second
    public class PawnSummary
    {
        private const int StatsRefreshTicks = 60;
        // Below this bleed rate the vanilla Health tab shows no bleeding
        private const float MinBleedRate = 0.01f;

        public int signature;
        public readonly List<GearItem> weapons = new List<GearItem>();
        public readonly List<GearItem> apparel = new List<GearItem>();
        public string weaponsTip = "";
        public string apparelTip = "";
        public string? weaponLabel;

        public readonly float[] armor = new float[3];
        public readonly string[] armorText = new string[3];
        public float comfyMin;
        public string comfyText = "";
        public float mass;
        public string massText = "";
        public bool overweight;

        private int statsTick = int.MinValue;
        private int statsHediffCount = -1;
        public int ageTicks;
        public string ageText = "";
        public string ageTip = "";
        public float bleedRate;
        // Ticks until death by blood loss, int.MaxValue when not bleeding or not at risk soon
        public int bleedDeathTicks = int.MaxValue;
        public string bleedText = "";
        public string bleedTip = "";
        public bool bleedLethal;
        public int tendCount;
        public string tendText = "";
        public string tendTip = "";

        private static readonly List<Apparel> tmpApparel = new List<Apparel>();

        // Changes whenever a weapon or apparel item is added, removed or damaged
        public static int Signature(Pawn pawn)
        {
            unchecked
            {
                int hash = 17;
                if (pawn.equipment != null)
                {
                    List<ThingWithComps> list = pawn.equipment.AllEquipmentListForReading;
                    for (int i = 0; i < list.Count; i++) hash = Mix(hash, list[i]);
                }
                hash = hash * 31 + 1;
                if (pawn.apparel != null)
                {
                    List<Apparel> list = pawn.apparel.WornApparel;
                    for (int i = 0; i < list.Count; i++) hash = Mix(hash, list[i]);
                }
                return hash;
            }
        }

        private static int Mix(int hash, Thing thing)
        {
            unchecked
            {
                return ((hash * 31 + thing.thingIDNumber) * 31 + thing.HitPoints) * 31 + thing.stackCount;
            }
        }

        public void RebuildGear(Pawn pawn, int newSignature)
        {
            signature = newSignature;
            weapons.Clear();
            apparel.Clear();

            if (pawn.equipment != null)
            {
                List<ThingWithComps> list = pawn.equipment.AllEquipmentListForReading;
                for (int i = 0; i < list.Count; i++) weapons.Add(new GearItem(list[i], ItemTip(pawn, list[i])));
            }
            weaponLabel = pawn.equipment?.Primary?.LabelNoCount;

            if (pawn.apparel != null)
            {
                tmpApparel.Clear();
                tmpApparel.AddRange(pawn.apparel.WornApparel);
                // Outer layers first, then larger coverage
                tmpApparel.SortBy(a => -a.def.apparel.LastLayer.drawOrder, a => -a.def.apparel.HumanBodyCoverage);
                for (int i = 0; i < tmpApparel.Count; i++) apparel.Add(new GearItem(tmpApparel[i], ItemTip(pawn, tmpApparel[i])));
                tmpApparel.Clear();
            }

            weaponsTip = ListTip(weapons);
            apparelTip = ListTip(apparel);
            statsTick = int.MinValue;
        }

        // Stats, age and health, rebuilt once a game second, when hediffs change and after a gear change
        public void RefreshStats(Pawn pawn)
        {
            int now = Find.TickManager.TicksGame;
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            if (now - statsTick < StatsRefreshTicks && now >= statsTick && hediffs.Count == statsHediffCount) return;
            statsTick = now;
            statsHediffCount = hediffs.Count;

            SetArmor(pawn, 0, StatDefOf.ArmorRating_Sharp);
            SetArmor(pawn, 1, StatDefOf.ArmorRating_Blunt);
            SetArmor(pawn, 2, StatDefOf.ArmorRating_Heat);

            comfyMin = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin);
            float comfyMax = pawn.GetStatValue(StatDefOf.ComfyTemperatureMax);
            comfyText = comfyMin.ToStringTemperature("F0") + " ~ " + comfyMax.ToStringTemperature("F0");

            mass = MassUtility.GearAndInventoryMass(pawn);
            float capacity = MassUtility.Capacity(pawn);
            overweight = mass > capacity;
            massText = mass.ToString("0.#") + " / " + capacity.ToStringMass();

            ageTicks = (int)(pawn.ageTracker.AgeBiologicalTicks / 2500L);
            ageText = pawn.ageTracker.AgeBiologicalYears.ToStringCached();
            ageTip = pawn.ageTracker.AgeTooltipString;

            bleedRate = pawn.health.hediffSet.BleedRateTotal;
            bleedDeathTicks = int.MaxValue;
            bleedLethal = false;
            bleedText = "";
            bleedTip = "";
            if (bleedRate > MinBleedRate)
            {
                // Same text as the vanilla Health tab
                string tip = "BleedingRate".Translate() + ": " + bleedRate.ToStringPercent() + "/" + "LetterDay".Translate();
                int ticks = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);
                if (ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.HasActiveGene(GeneDefOf.Deathless))
                {
                    tip += " (" + "Deathless".Translate() + ")";
                    bleedText = bleedRate.ToStringPercent();
                }
                else if (ticks >= GenDate.TicksPerDay)
                {
                    tip += " (" + "WontBleedOutSoon".Translate() + ")";
                    bleedText = bleedRate.ToStringPercent();
                }
                else
                {
                    tip += " (" + "TimeToDeath".Translate(ticks.ToStringTicksToPeriod()) + ")";
                    bleedText = ticks.ToStringTicksToPeriod(allowSeconds: false, shortForm: true, canUseDecimals: false);
                    bleedDeathTicks = ticks;
                    bleedLethal = true;
                }
                bleedTip = tip;
            }

            StringBuilder sb = new StringBuilder();
            tendCount = 0;
            for (int i = 0; i < hediffs.Count; i++)
            {
                if (!hediffs[i].TendableNow()) continue;
                if (tendCount > 0) sb.AppendLine();
                sb.Append(hediffs[i].LabelCap);
                if (hediffs[i].Part != null) sb.Append(" (").Append(hediffs[i].Part.LabelCap).Append(")");
                tendCount++;
            }
            tendText = tendCount > 0 ? tendCount.ToStringCached() : "";
            tendTip = sb.ToString();
        }

        // Overall armor across body parts, as the vanilla Gear tab calculates it
        private void SetArmor(Pawn pawn, int index, StatDef stat)
        {
            float total = 0f;
            float natural = Mathf.Clamp01(pawn.GetStatValue(stat) / 2f);
            List<BodyPartRecord> parts = pawn.RaceProps.body.AllParts;
            List<Apparel>? worn = pawn.apparel?.WornApparel;
            for (int i = 0; i < parts.Count; i++)
            {
                float unblocked = 1f - natural;
                if (worn != null)
                {
                    for (int j = 0; j < worn.Count; j++)
                    {
                        if (worn[j].def.apparel.CoversBodyPart(parts[i])) unblocked *= 1f - Mathf.Clamp01(worn[j].GetStatValue(stat) / 2f);
                    }
                }
                total += parts[i].coverageAbs * (1f - unblocked);
            }
            armor[index] = Mathf.Clamp(total * 2f, 0f, 2f);
            armorText[index] = armor[index].ToStringPercent();
        }

        private static string ItemTip(Pawn pawn, Thing thing)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(thing.LabelCap);
            if (thing is Apparel ap)
            {
                if (pawn.outfits != null && pawn.outfits.forcedHandler.IsForced(ap)) sb.Append(", ").Append("ApparelForcedLower".Translate());
                if (pawn.apparel != null && pawn.apparel.IsLocked(ap)) sb.Append(" (").Append("ApparelLockedLower".Translate()).Append(")");
            }
            sb.AppendLine().AppendLine();
            sb.Append("ColonySummary_ClickForInfo".Translate());
            return sb.ToString();
        }

        private static string ListTip(List<GearItem> items)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0) sb.AppendLine();
                sb.Append(items[i].thing.LabelCap);
            }
            return sb.ToString();
        }
    }
}
