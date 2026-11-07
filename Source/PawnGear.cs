using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySummary
{
    // One pawn's gear, armor, comfortable temperatures and mass, built when the gear changes
    public class PawnGear
    {
        public int signature;
        public readonly List<GearItem> weapons = new List<GearItem>();
        public readonly List<GearItem> apparel = new List<GearItem>();
        public readonly List<GearItem> inventory = new List<GearItem>();
        public string weaponsTip = "";
        public string apparelTip = "";
        public string inventoryTip = "";
        public string? weaponLabel;

        public readonly float[] armor = new float[3];
        public readonly string[] armorText = new string[3];
        public float comfyMin;
        public string comfyText = "";
        public float mass;
        public string massText = "";
        public bool overweight;

        private static readonly List<Apparel> tmpApparel = new List<Apparel>();

        // Changes whenever an item is added, removed, damaged or restacked, or a hediff is added or removed
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
                hash = hash * 31 + 2;
                if (pawn.inventory != null)
                {
                    ThingOwner<Thing> list = pawn.inventory.innerContainer;
                    for (int i = 0; i < list.Count; i++) hash = Mix(hash, list[i]);
                }
                return hash * 31 + pawn.health.hediffSet.hediffs.Count;
            }
        }

        private static int Mix(int hash, Thing thing)
        {
            unchecked
            {
                return ((hash * 31 + thing.thingIDNumber) * 31 + thing.HitPoints) * 31 + thing.stackCount;
            }
        }

        public void Rebuild(Pawn pawn, int newSignature)
        {
            signature = newSignature;
            weapons.Clear();
            apparel.Clear();
            inventory.Clear();

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

            if (pawn.inventory != null)
            {
                ThingOwner<Thing> list = pawn.inventory.innerContainer;
                for (int i = 0; i < list.Count; i++) inventory.Add(new GearItem(list[i], ItemTip(pawn, list[i])));
            }

            weaponsTip = ListTip(weapons);
            apparelTip = ListTip(apparel);
            inventoryTip = ListTip(inventory);

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
