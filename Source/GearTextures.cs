using UnityEngine;
using Verse;

namespace ColonySummary
{
    [StaticConstructorOnStartup]
    public static class GearTextures
    {
        public static readonly Texture2D ConditionBar = SolidColorMaterials.NewSolidColorTexture(new Color(0.55f, 0.55f, 0.55f));
        public static readonly Texture2D ConditionBarLow = SolidColorMaterials.NewSolidColorTexture(new Color(0.75f, 0.2f, 0.2f));
    }
}
