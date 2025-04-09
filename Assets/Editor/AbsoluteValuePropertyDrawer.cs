using _01_Scripts.Lib;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AbsoluteValueAttribute))]
    public class AbsoluteValuePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = Mathf.Max(EditorGUI.IntField(position, label, property.intValue), 0);
            }
            else if(property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = Mathf.Max(EditorGUI.FloatField(position, label, property.floatValue), 0f);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}