#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace RePunkk.ReEditor.GrabSystem
{
    using RePunkk.GrabSystem;

    [CustomEditor(typeof(GrabSystem))]
    public class GrabSystemInspector : Editor
    {
        private static class Colors
        {
            public static readonly Color InspectorBackground = new Color(0.243f, 0.137f, 0.310f, 0.396f);
            public static readonly Color SectionContainer = new Color(0.174f, 0.149f, 0.200f, 0.900f);
            public static readonly Color TopBorder = new Color(0.600f, 0.000f, 0.600f, 0.800f);
            public static readonly Color HeaderText = new Color(0.800f, 0.800f, 0.900f, 1.000f);
            public static readonly Color InfoText = new Color(0.700f, 0.700f, 0.800f, 1.000f);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawBackground(() =>
            {
                DrawTitle();
                DrawFeatures();
                EditorGUILayout.Space(10);
                DrawGrabSettings();
                EditorGUILayout.Space(10);
                DrawGrabPointSettings();
                EditorGUILayout.Space(10);
                DrawPhysicsSettings();
                EditorGUILayout.Space(10);
                DrawRotationSettings();
                EditorGUILayout.Space(10);
                DrawThrowSettings();
                EditorGUILayout.Space(10);
                DrawVisualSettings();
                EditorGUILayout.Space(10);
                DrawDebugSettings();
            });

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBackground(System.Action drawContent)
        {
            Rect backgroundRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(backgroundRect, Colors.InspectorBackground);
            EditorGUILayout.Space(5);
            drawContent();
            EditorGUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawTitle()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Grab System", CreateHeaderStyle(16, TextAnchor.MiddleCenter));
            EditorGUILayout.Space(10);
        }

        private void DrawFeatures()
        {
            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Features", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableScrollDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableManualRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableThrow"));
            });
        }

        private void DrawGrabSettings()
        {
            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Grab Settings", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraTransform"));

                EditorGUILayout.LabelField("Defaults to Main Camera if unassigned",
                    new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Colors.InfoText }, fontSize = 9 });

                EditorGUILayout.PropertyField(serializedObject.FindProperty("grabRange"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDragDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dragPointIsOnHit"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("grabMask"));
            });
        }

        private void DrawGrabPointSettings()
        {
            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Grab Point Settings", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("grabPointOffset"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("scrollSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minGrabDistance"));
            });
        }

        private void DrawPhysicsSettings()
        {
            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Physics Settings", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("baseForce"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("damping"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heavyObjectThreshold"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dragDifficultyFactor"));

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Mass-Based Stabilization", CreateHeaderStyle(11));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lightObjectMass"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heavyObjectMass"));

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Objects between these masses will have interpolated stabilization",
                    new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Colors.InfoText }, fontSize = 9 });
            });
        }

        private void DrawRotationSettings()
        {
            SerializedProperty enableRotation = serializedObject.FindProperty("enableManualRotation");

            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Rotation Settings", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                if (!enableRotation.boolValue)
                {
                    EditorGUILayout.LabelField("Manual rotation is disabled",
                        new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Colors.InfoText }, fontSize = 10 });
                    EditorGUILayout.Space(5);
                }

                EditorGUI.BeginDisabledGroup(!enableRotation.boolValue);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateKey"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAngularVelocity"));
                EditorGUI.EndDisabledGroup();
            });
        }

        private void DrawThrowSettings()
        {
            SerializedProperty enableThrow = serializedObject.FindProperty("enableThrow");

            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Throw Settings", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                if (!enableThrow.boolValue)
                {
                    EditorGUILayout.LabelField("Throw is disabled",
                        new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Colors.InfoText }, fontSize = 10 });
                    EditorGUILayout.Space(5);
                }

                EditorGUI.BeginDisabledGroup(!enableThrow.boolValue);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("throwChargeKey"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("throwChargeSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minThrowDistanceOffset"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("throwForceMultiplier"));
                EditorGUI.EndDisabledGroup();
            });
        }

        private void DrawVisualSettings()
        {
            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Visual Settings", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                SerializedProperty drawGrabLine = serializedObject.FindProperty("drawGrabLine");
                EditorGUILayout.PropertyField(drawGrabLine);

                if (drawGrabLine.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("lineWidth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("lineVertices"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("lineMaterial"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("usingColor"));

                    if (serializedObject.FindProperty("usingColor").boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("lineColor"));
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("lineStartOffset"));
                }
            });
        }

        private void DrawDebugSettings()
        {
            DrawSectionContainer(() =>
            {
                EditorGUILayout.LabelField("Debug Settings", CreateHeaderStyle(12));
                EditorGUILayout.Space(8);

                SerializedProperty showGizmo = serializedObject.FindProperty("showGizmo");
                EditorGUILayout.PropertyField(showGizmo);

                if (showGizmo.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("gizmoColor"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("gizmoSize"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("minDistanceGizmoColor"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("minDistanceGizmoSize"));
                }
            });
        }

        private void DrawSectionContainer(System.Action drawContent)
        {
            Rect containerRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(containerRect, Colors.SectionContainer);
            EditorGUI.DrawRect(new Rect(containerRect.x, containerRect.y, containerRect.width, 2), Colors.TopBorder);
            EditorGUILayout.Space(10);

            drawContent();

            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8);
        }

        private GUIStyle CreateHeaderStyle(int fontSize, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            return new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = fontSize,
                alignment = alignment,
                normal = { textColor = Colors.HeaderText }
            };
        }
    }
}
#endif
