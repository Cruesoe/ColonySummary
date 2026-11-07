using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ColonySummary
{
    // Draws a column header label rotated 60 degrees, as Division of Labor's inclined Work tab headers
    public static class InclinedLabel
    {
        private const float Angle = 60f;
        private static readonly Color LabelColor = new Color(0.8f, 0.8f, 0.8f);
        private static readonly Color LineColor = new Color(1f, 1f, 1f, 0.2f);

        // GUIClip.Unclip is internal to Unity's IMGUI (UnityStubs on 1.7); resolved once
        private static readonly Func<Vector2, Vector2>? unclip = ResolveUnclip();

        private static Func<Vector2, Vector2>? ResolveUnclip()
        {
            Type? clip = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip");
            MethodInfo? method = clip?.GetMethod("Unclip", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(Vector2) }, null);
            return method == null ? null : (Func<Vector2, Vector2>)Delegate.CreateDelegate(typeof(Func<Vector2, Vector2>), method);
        }

        // Height a header needs so a label of this width fits when rotated
        public static float HeaderHeightFor(float labelWidth) => labelWidth + GenUI.GapTiny;

        // Rotates the label so its bottom-left corner sits at the bottom right of the column; returns the label rect and its matrix
        public static (Rect rect, Matrix4x4 matrix) Draw(Rect rect, string label, float labelHeight)
        {
            Rect rotatedRect = new Rect(0f, 0f, rect.height, labelHeight) { center = rect.center };

            float theta = Angle * Mathf.Deg2Rad;
            Vector2 corner = new Vector2(-rotatedRect.width / 2f, -rotatedRect.height / 2f);
            float rotatedCornerX = Mathf.Cos(theta) * corner.x - Mathf.Sin(theta) * corner.y + rotatedRect.center.x;
            rotatedRect.x += rect.xMax - rotatedCornerX;

            Matrix4x4 originalMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;
            Vector2 pivot = unclip != null ? unclip(rotatedRect.center) : rotatedRect.center;
            Matrix4x4 matrix = originalMatrix
                * Matrix4x4.TRS(pivot, Quaternion.identity, Vector3.one)
                * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -Angle), Vector3.one)
                * Matrix4x4.TRS(-pivot, Quaternion.identity, Vector3.one);
            GUI.matrix = matrix;

            Color color = GUI.color;
            TextAnchor anchor = Text.Anchor;
            bool wrap = Text.WordWrap;
            GUI.color = LabelColor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Small;
            Text.WordWrap = false;
            Widgets.Label(rotatedRect, label);
            Text.WordWrap = wrap;
            Text.Anchor = anchor;
            GUI.color = color;
            Widgets.DrawLine(new Vector2(rotatedRect.xMax, rotatedRect.yMax), new Vector2(rotatedRect.xMin, rotatedRect.yMax), LineColor, 1f);

            GUI.matrix = originalMatrix;
            return (rotatedRect, matrix);
        }
    }
}
