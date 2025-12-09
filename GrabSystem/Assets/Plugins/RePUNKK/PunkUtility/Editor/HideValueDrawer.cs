#if UNITY_EDITOR

using RePunkk.ReTools;
using UnityEditor;
using UnityEngine;

namespace RePunkk.ReEditor
{
    [CustomPropertyDrawer(typeof(HideValueAttribute))]
    public class HideValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HideValueAttribute hideValueAttribute = (HideValueAttribute)attribute;
            bool shouldHide = ShouldHide(hideValueAttribute, property);
            if (!shouldHide)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideValueAttribute hideValueAttribute = (HideValueAttribute)attribute;
            bool shouldHide = ShouldHide(hideValueAttribute, property);
            if (shouldHide)
                return 0f;

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        private bool ShouldHide(HideValueAttribute hideValueAttribute, SerializedProperty property)
        {
            string propertyPath = property.propertyPath;

            string conditionPath = propertyPath;
            int lastDotIndex = propertyPath.LastIndexOf('.');

            if (lastDotIndex != -1)
            {
                string pathPrefix = propertyPath.Substring(0, lastDotIndex + 1);
                conditionPath = pathPrefix + hideValueAttribute.conditionalSourceField;
            }
            else conditionPath = hideValueAttribute.conditionalSourceField;

            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue == null)
            {
                sourcePropertyValue = property.serializedObject.FindProperty(hideValueAttribute.conditionalSourceField);
                if (sourcePropertyValue == null)
                {
                    Debug.LogWarning($"Conditional source property '{hideValueAttribute.conditionalSourceField}' not found for property '{property.name}'");
                    return false;
                }
            }

            bool conditionValue = false;
            switch (sourcePropertyValue.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    conditionValue = sourcePropertyValue.boolValue;
                    break;
                case SerializedPropertyType.Enum:
                    conditionValue = sourcePropertyValue.enumValueIndex > 0;
                    break;
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Float:
                    conditionValue = sourcePropertyValue.floatValue > 0;
                    break;
                case SerializedPropertyType.String:
                    conditionValue = !string.IsNullOrEmpty(sourcePropertyValue.stringValue);
                    break;
                case SerializedPropertyType.ObjectReference:
                    conditionValue = sourcePropertyValue.objectReferenceValue != null;
                    break;
                case SerializedPropertyType.Generic:
                    conditionValue = sourcePropertyValue.hasVisibleChildren;
                    break;
                default:
                    conditionValue = true;
                    break;
            }

            return conditionValue == hideValueAttribute.hideValue;
        }
    }
}
#endif