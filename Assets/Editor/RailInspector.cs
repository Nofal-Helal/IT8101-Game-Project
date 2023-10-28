using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct PropertyAction
{
    public SerializedProperty property;
    public Action action;
    public PropertyAction(SerializedProperty property, Action action)
    {
        this.property = property; this.action = action;
    }
}

[CustomEditor(typeof(RailRenderer))]
public class RailInspector : Editor
{
    SerializedProperty spacing;
    List<PropertyAction> propertyActions;
    void OnEnable()
    {
        spacing = serializedObject.FindProperty("spacing");
        propertyActions = new List<PropertyAction> {
            new PropertyAction(spacing, () => (target as RailRenderer).UpdateMesh())
        };
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        propertyActions.ForEach(propertyAction =>
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(spacing);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                propertyAction.action();
            }
        });

        if (GUILayout.Button("Regenerate Mesh"))
        {
            (target as RailRenderer).UpdateMesh();

        }

    }
}
